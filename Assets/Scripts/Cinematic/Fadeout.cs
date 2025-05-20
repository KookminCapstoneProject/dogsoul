using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fadeout : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(CoFadeOut());
    }

    IEnumerator CoFadeOut()
    {
        yield return new WaitForSeconds(0.5f); // 처음 딜레이
        float elapsedTime = 0f; 
        float fadedTime = 3f; 

        while (elapsedTime <= fadedTime)
        {
            GetComponent<CanvasRenderer>().SetAlpha(Mathf.Lerp(1f, 0f, elapsedTime / fadedTime));

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        Destroy(gameObject);
        yield break;
    }
}