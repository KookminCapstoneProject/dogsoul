using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

public class ProjectileShooter : MonoBehaviour
{
    public GameObject projectilePrefab;
    public GameObject projectilePrefab_M;

    WeaponHolderSlot_M weaponHolderSlot;

    private void OnEnable()
    {
        if(SceneController.Instance.GetCurrentSceneName() != "Village")
        {
            weaponHolderSlot = GetComponentInParent<WeaponHolderSlot_M>();
            weaponHolderSlot.shooter = this;
        }
            
    }



    LockOnTarget nearestTarget;

    private string targetTag;
    public float angleToTarget = 60f;

    

    public void Shoot()
    {
        targetTag = transform.root.tag == "Player" ? "Enemy" : "Player";
        nearestTarget = FindNearestTarget();

        GameObject projectile = Instantiate(projectilePrefab, transform.position, Quaternion.identity);

        
        projectile.tag = transform.root.tag == "Player" ? "PlayerWeapon" : "EnemyWeapon";
        projectile.GetComponent<DamageCollider>().EnableDamageCollider();

        Vector3 direction = new Vector3();
        if (nearestTarget != null)
        {
            //projectile.transform.LookAt(nearestTarget.lockOnTarget.transform);
            projectile.transform.eulerAngles = transform.root.eulerAngles;
            if (projectile.transform.rotation.x < 0)
            {
                direction = new Vector3(0, transform.eulerAngles.y, transform.eulerAngles.z);
                projectile.transform.eulerAngles = direction;
            }
        }
        else
        {
            direction = transform.eulerAngles;
            projectile.transform.eulerAngles = direction;
        }
        if (weaponHolderSlot != null) MultiplayProjectile(direction);
    }

    public void Shoot_(Vector3 direction)
    {
        GameObject projectile = Instantiate(projectilePrefab_M, transform.position, Quaternion.identity);


        projectile.tag = transform.root.tag == "Player" ? "PlayerWeapon" : "EnemyWeapon";
        projectile.transform.eulerAngles = direction;
    }

    private LockOnTarget FindNearestTarget()
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position, 10f);
        LockOnTarget nearestEnemy = null;
        float minDistance = Mathf.Infinity;

        foreach (Collider collider in colliders)
        {
            if (collider.tag == targetTag)
            {
                Vector3 dirToTarget = (collider.transform.position - transform.position).normalized;
                float viewAngle = Vector3.Angle(transform.forward, dirToTarget);

                if (viewAngle < angleToTarget)
                {
                    float distance = Vector3.Distance(transform.position, collider.transform.position);
                    if (distance < minDistance)
                    {
                        minDistance = distance;
                        nearestEnemy = collider.GetComponent<LockOnTarget>();
                    }
                }
            }
        }
        return nearestEnemy;
    }

    /*[PunRPC]
    private void ShootArrow()
    {
        GameObject projectile = Instantiate(projectilePrefab_M, transform.position, Quaternion.identity);
        projectile.tag = transform.root.tag == "Player" ? "PlayerWeapon" : "EnemyWeapon";
        if (nearestTarget != null)
        {
            //projectile.transform.LookAt(nearestTarget.lockOnTarget.transform);
            projectile.transform.eulerAngles = transform.root.eulerAngles;
            if (projectile.transform.rotation.x < 0)
            {
                projectile.transform.eulerAngles = new Vector3(0, transform.eulerAngles.y, transform.eulerAngles.z);
            }
        }
        else
        {
            projectile.transform.eulerAngles = transform.root.eulerAngles;
        }
    }*/


    private void MultiplayProjectile(Vector3 dirction)
    {

    }
    

}
