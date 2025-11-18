using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class ScreenFade : MonoBehaviour
{
    public Image blackScreen;   // 黑屏 UI
    public float fadeTime = 1f; // 淡入淡出时间

    public IEnumerator FadeIn() // 亮 → 黑
    {
        blackScreen.gameObject.SetActive(true);
        for (float t = 0; t < fadeTime; t += Time.deltaTime)
        {
            float a = t / fadeTime;
            blackScreen.color = new Color(0, 0, 0, a);
            yield return null;
        }
        blackScreen.color = new Color(0, 0, 0, 1f);
    }

    public IEnumerator FadeOut() // 黑 → 亮
    {
        for (float t = 0; t < fadeTime; t += Time.deltaTime)
        {
            float a = 1f - (t / fadeTime);
            blackScreen.color = new Color(0, 0, 0, a);
            yield return null;
        }
        blackScreen.color = new Color(0, 0, 0, 0f);
        blackScreen.gameObject.SetActive(false);
    }
}
