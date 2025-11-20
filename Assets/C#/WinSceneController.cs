using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class WinSceneController : MonoBehaviour
{
    public Image fadeImage;
    public TextMeshProUGUI winText;

    public float fadeTime = 2f;

    void Start()
    {
        // 开始执行过场动画
        StartCoroutine(PlayEnding());
    }

    IEnumerator PlayEnding()
    {
        // 1. 黑屏淡入（从黑色透明 → 不透明）
        fadeImage.color = new Color(0, 0, 0, 0);

        float t = 0;
        while (t < fadeTime)
        {
            t += Time.deltaTime;
            float a = Mathf.Lerp(0f, 1f, t / fadeTime);
            fadeImage.color = new Color(0, 0, 0, a);
            yield return null;
        }

        // 2. 延迟 0.5 秒
        yield return new WaitForSeconds(0.5f);

        // 3. 淡入文字
        string message = "Welcome back to the real world...";
        winText.text = message;

        t = 0;
        winText.color = new Color(1, 1, 1, 0);

        while (t < fadeTime)
        {
            t += Time.deltaTime;
            float a = Mathf.Lerp(0f, 1f, t / fadeTime);
            winText.color = new Color(1, 1, 1, a);
            yield return null;
        }
    }
}
