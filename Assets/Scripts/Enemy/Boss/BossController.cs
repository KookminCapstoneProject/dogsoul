using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine.Animations;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UIElements;

[RequireComponent(typeof(EnemyState))]
abstract public class BossController : MonoBehaviour
{
    public Transform target;

    [Header("Basic Settings")]
    [SerializeField] protected float basicSpeed;
    [SerializeField] protected float attackDistance;
    [SerializeField] protected float rangedThresholdDistance;
    [SerializeField] protected int attackPatterns = 2;
    [SerializeField] protected int rangedPatterns = 2;

    [Header("Combats")]
    [SerializeField] protected GameObject rockPrefab;
    [SerializeField] protected Transform rockInitPos;
    [SerializeField] protected float jumpDamage = 30f;
    [SerializeField] protected float rangedCool = 3f;
    [SerializeField] protected float attackCoolMin = 1f;
    [SerializeField] protected float attackCoolMax = 2f; // random max

    public AudioClip attackSound;
    public AudioClip jumpSound;
    public AudioClip stompSound;
    public AudioClip crashSound;
    [Range(0, 1)] float volume = 0.5f;


    protected Animator animator;
    protected Vector3 spawnPosition;
    protected EnemyState enemyState;
    protected NavMeshAgent agent;
    protected NavMeshPath path;
    protected DamageCollider damageCollider;
    protected EnemyDetection enemyDetection;

    protected float distance = Mathf.Infinity;
    protected int attackPatternNo = 0;
    protected int rangedPatternNo = 0;
    protected float attackCoolDelta = 0;
    protected float rangedCoolDelta = 0;

    protected virtual void Awake()
    {
        animator = GetComponent<Animator>();
        agent = GetComponent<NavMeshAgent>();
        damageCollider = GetComponentInChildren<DamageCollider>();
        enemyState = GetComponent<EnemyState>();
        enemyDetection = GetComponent<EnemyDetection>();
        path = new NavMeshPath();

        agent.speed = basicSpeed;
    }

    protected void Start()
    {
        spawnPosition = transform.position;

    }

    protected void Update()
    {
        
        if (animator.GetBool("IsInteracting")) return;
        if (target == null)
        {
            //Find closest player
            target = enemyDetection.GetClosestPlayer();
            if (target == null) return;
        }
        float distanceToPlayer = CalculDistance();
        


        if (distanceToPlayer < attackDistance)
        {
            if (attackCoolDelta <= 1) 
            {
                Attack();
            }
            else
            {
                Chase();
            }
        } 
        else if (distanceToPlayer > attackDistance && distanceToPlayer < rangedThresholdDistance)
        {
            Chase();
        }
        else if (distanceToPlayer > rangedThresholdDistance)
        {
            if (rangedCoolDelta <= 1)
            {
                RangedAttack();
            }
            else
            {
                Chase();
            }
        }
        if (rangedCoolDelta > 0)
        {
            rangedCoolDelta -= Time.deltaTime;
        }

        if (attackCoolDelta > 0)
        {
            attackCoolDelta -= Time.deltaTime;
        }
    }

    protected float CalculDistance()
    {
        if (agent.CalculatePath(target.position, path))
        {
            distance = 0f;
            for (int i = 0; i < path.corners.Length - 1; i++)
            {
                distance += Vector3.Distance(path.corners[i], path.corners[i + 1]);
            }
        }
        return distance;
    }

    protected void Chase()
    {
        agent.isStopped = false;
        agent.SetDestination(target.position);

        animator.SetBool("Stop", false);
        animator.SetBool("Following", true);
    }

    protected void Attack()
    {
        transform.LookAt(target);

        agent.isStopped = true;
        agent.velocity = Vector3.zero;

        animator.SetBool("Following", false);
        attackPatternNo = Random.Range(0, attackPatterns);
        animator.SetTrigger("Attack");
        animator.SetInteger("AttackPatternNo", attackPatternNo);
        animator.SetBool("IsInteracting", true);
        animator.SetBool("Attacking", true);
    }

    public virtual void DieData()
    {

    }

    public void PlayAttackSound()
    {
        AudioSource.PlayClipAtPoint(attackSound, transform.position, volume);
    }

    public void PlayStompSound()
    {
        AudioSource.PlayClipAtPoint(stompSound, transform.position, volume);
    }

    #region Attack
    public void EnableAttack()
    {
        damageCollider.EnableDamageCollider();
    }
    public void UnableAttack()
    {
        damageCollider.UnableDamageCollider();
    }
    public void AttackCooltime()
    {
        attackCoolDelta = Random.Range(attackCoolMin, attackCoolMax);
    }
    #endregion

    #region Ranged Attack
    protected void RangedAttack()
    {
        transform.LookAt(target);

        agent.isStopped = true;
        agent.velocity = Vector3.zero;

        attackPatternNo = Random.Range(0, attackPatterns);

        animator.SetBool("Following", false);
        animator.SetTrigger("RangedAttack");
        animator.SetBool("Attacking", true);

        rangedPatternNo = Random.Range(0, rangedPatterns);
        animator.SetInteger("RangedPatternNo", rangedPatternNo);
        animator.SetBool("IsInteracting", true);

        
    }
    public void JumpAttack()
    {
        StartCoroutine(JumpCorutine());
    }

    abstract public void ThrowRock();

    public void RangedCooltime()
    {
        rangedCoolDelta = rangedCool;
    }
    
    protected IEnumerator JumpCorutine()
    {
        Vector3 startPos = transform.position;
        Vector3 targetPos = target.position;

        float jumpHeight = 5f;
        float duration = 1.1f;
        float time = 0f;

        AudioSource.PlayClipAtPoint(jumpSound, transform.position, volume);

        while (time < duration)
        {
            float t = time / duration;
            // 포물선 보간
            Vector3 currentPos = Vector3.Lerp(startPos, targetPos, t);
            currentPos.y += Mathf.Sin(Mathf.PI * t) * jumpHeight;

            transform.position = currentPos;

            time += Time.deltaTime;
            yield return null;
        }

        // 착지 처리
        OnLand();

        // 점프 쿨타임
        yield return new WaitForSeconds(3f);
    }

    protected void OnLand()
    {
        float damageRadius = 10f;
        animator.SetBool("IsInteracting", true);

        AudioSource.PlayClipAtPoint(crashSound, transform.position, volume);

        Collider[] hitColliders = Physics.OverlapSphere(transform.position, damageRadius);
        foreach (var hit in hitColliders)
        {
            if (hit.CompareTag("Player"))
            {
                hit.GetComponent<PlayerHealth>().TakeDamage(jumpDamage, null, Vector3.zero, null, true);
                
            }
        }

        // 착지 이펙트, 사운드
        // 예: Instantiate(landingEffect, transform.position, Quaternion.identity);
    }
    #endregion

}