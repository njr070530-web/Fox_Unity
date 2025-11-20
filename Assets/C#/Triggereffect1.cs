using UnityEngine;
using UnityEngine.UI;

public class TriggerEffect1 : MonoBehaviour
{
    public Image effectImage;

    [Header("Scale Settings")]
    public float startScale = 0.1f;     // 初始缩放
    public float targetScale = 6f;      // 目标缩放（越大越超过屏幕）
    public float scaleSpeed = 2f;

    [Header("Fade Settings")]
    public float stayDuration = 1f;     // 放大后停留
    public float fadeDuration = 1.0f;   // 淡出时间

    private bool triggered = false;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!triggered && collision.CompareTag("Player"))
        {
            triggered = true;
            effectImage.gameObject.SetActive(true);
            StartCoroutine(PlayEffect());
        }
    }

    private System.Collections.IEnumerator PlayEffect()
    {
        RectTransform rt = effectImage.rectTransform;

        // ★ Step 1: 重置大小 & alpha
        rt.localScale = new Vector3(startScale, startScale, 1);

        Color c = effectImage.color;
        c.a = 1;                    // 初始全不透明
        effectImage.color = c;

        // ----------- 放大动画 -----------
        while (rt.localScale.x < targetScale)
        {
            float newScale = rt.localScale.x + Time.deltaTime * scaleSpeed;
            rt.localScale = new Vector3(newScale, newScale, 1);   // ★ 保持比例
            yield return null;
        }

        // ----------- 停留 -----------
        yield return new WaitForSeconds(stayDuration);

        // ----------- 淡出动画 -----------
        float t = 0f;
        while (t < fadeDuration)
        {
            t += Time.deltaTime;
            float alpha = Mathf.Lerp(1f, 0f, t / fadeDuration);
            c.a = alpha;
            effectImage.color = c;
            yield return null;
        }

        // ----------- 隐藏 -----------
        effectImage.gameObject.SetActive(false);
    }
}
