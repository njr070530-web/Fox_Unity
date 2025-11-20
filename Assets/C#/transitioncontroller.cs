using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class transitioncontroller : MonoBehaviour
{
    public Image fadeImage;      
    public Text endingText;      

    public float fadeDuration = 1.5f;

    public void StartEndingSequence(string msg)
    {
        StartCoroutine(DoEnding(msg));
    }

    IEnumerator DoEnding(string msg)
    {
        // 初始化：启用 UI
        fadeImage.gameObject.SetActive(true);
        endingText.gameObject.SetActive(true);

        // 黑屏从透明 → 不透明
        Color c = fadeImage.color;
        c.a = 0;
        fadeImage.color = c;

        float t = 0;
        while (t < fadeDuration)
        {
            t += Time.deltaTime;
            c.a = Mathf.Lerp(0, 1, t / fadeDuration);
            fadeImage.color = c;
            yield return null;
        }

        // 显示结局文字
        endingText.text = msg;
        yield return new WaitForSeconds(3f);

        // 游戏结束后可执行其他操作：
        // Application.Quit();
        // 或显示重来按钮
    }
}
