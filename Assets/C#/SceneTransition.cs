using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class SceneTransition : MonoBehaviour
{
    public CanvasGroup blackScreen;
    public CanvasGroup storyText;  // Text 也有 CanvasGroup
    public GameObject cameraView;  // RawImage

    public float fadeDuration = 1.5f;

    public void StartEndingSequence(string text)
    {
        StartCoroutine(EndingSequence(text));
    }

    private IEnumerator EndingSequence(string story)
    {
        //-----------------------------------
        // 1. 黑屏淡入
        //-----------------------------------
        for (float t = 0; t < fadeDuration; t += Time.deltaTime)
        {
            blackScreen.alpha = t / fadeDuration;
            yield return null;
        }
        blackScreen.alpha = 1;

        //-----------------------------------
        // 2. 显示剧情文字淡入
        //-----------------------------------
        storyText.GetComponent<TMPro.TMP_Text>().text = story;

        for (float t = 0; t < fadeDuration; t += Time.deltaTime)
        {
            storyText.alpha = t / fadeDuration;
            yield return null;
        }

        yield return new WaitForSeconds(2f);

        //-----------------------------------
        // 3. 显示摄像头画面
        //-----------------------------------
        cameraView.SetActive(true);

        // 文字淡出
        for (float t = 0; t < fadeDuration; t += Time.deltaTime)
        {
            storyText.alpha = 1 - t / fadeDuration;
            yield return null;
        }

        //-----------------------------------
        // 4. 停留几秒 → 游戏结束
        //-----------------------------------
        yield return new WaitForSeconds(3f);

        Debug.Log("Game End.");
        Application.Quit();
    }
}
