using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using System.Collections.Generic;
using KoreanTyper;

public class Opening : MonoBehaviour
{

    [System.Serializable]
    public class SplashPage
    {
        public Sprite image;
        public string[] scripts;
    }

    public Image backgorund;
    public Image splashImage;
    public TextMeshProUGUI[] texts;
    public SplashPage[] pages;

    void Start()
    {
        StartCoroutine(TypingText());
    }

    void LoadNextScene()
    {
        // UnityEngine.SceneManagement.SceneManager.LoadScene("MainMenu");
    }

    public IEnumerator TypingText()
    {
        for (int index = 0; index < pages.Length; index++) // 페이지 불러오기
        {
            splashImage.sprite = pages[index].image; // 이미지 전환
            foreach (TextMeshProUGUI t in texts) t.text = "";

            for (int t = 0; t < texts.Length && t < pages[index].scripts.Length; t++) // 문장 불러오기
            {
                int strTypingLength = pages[index].scripts[t].GetTypingLength();

                for (int i = 0; i <= strTypingLength; i++) // 한국어 타이핑
                {
                    texts[t].text = pages[index].scripts[t].Typing(i);
                    yield return new WaitForSeconds(0.02f);
                }
                yield return new WaitForSeconds(0.5f);
            }
            yield return new WaitForSeconds(0.5f);
        }
        StartCoroutine(CoFadeOut());
    }

    IEnumerator CoFadeOut()
    {
        float elapsedTime = 0f; 
        float fadedTime = 3f; 

        while (elapsedTime <= fadedTime)
        {
            backgorund.GetComponent<CanvasRenderer>().SetAlpha(Mathf.Lerp(1f, 0f, elapsedTime / fadedTime));
            splashImage.GetComponent<CanvasRenderer>().SetAlpha(Mathf.Lerp(1f, 0f, elapsedTime / fadedTime));
            foreach (TextMeshProUGUI t in texts) t.GetComponent<CanvasRenderer>().SetAlpha(Mathf.Lerp(1f, 0f, elapsedTime / fadedTime));

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        Destroy(gameObject);
        yield break;
    }
}
