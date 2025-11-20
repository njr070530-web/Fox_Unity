using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;
using System.Collections; 


public class StoryController : MonoBehaviour
{
    public RawImage cameraView;
    public Image fadeImage;
    public TextMeshProUGUI storyText;
    public Image bar; // 进度条或其他 UI 元素

    public float fadeDuration = 2.5f;

    void Start()
    {
        StartCoroutine(StartStorySequence());
    }

    IEnumerator StartStorySequence()
    {
        // 1. 黑屏淡入
        fadeImage.color = new Color(0,0,0,1);
        cameraView.gameObject.SetActive(false);
        bar.gameObject.SetActive(false);
        storyText.enabled = false;

        yield return StartCoroutine(FadeOutBlack());

        // 2. 显示摄像头画面
        cameraView.gameObject.SetActive(true);
        bar.gameObject.SetActive(true);

        // 3. 打字机显示剧情
        yield return new WaitForSeconds(0.5f);
        storyText.enabled = true;

        string line1 = "You are trapped in a pixelated world.";
        yield return StartCoroutine(TypeText(line1));
        yield return new WaitForSeconds(2f);


        string line2 = "\nFind your way out before it's too late...";
        yield return StartCoroutine(TypeText(line2));
        yield return new WaitForSeconds(2f);


        string line3 = "\nFeel free to scream load!";
        yield return StartCoroutine(TypeText(line3));
        yield return new WaitForSeconds(5f);

        // 这里可以跳转到实际游戏关卡
        SceneManager.LoadScene("SampleScene");
    }

    IEnumerator TypeText(string fullText)
    {
        storyText.text = "";
        foreach (char c in fullText)
        {
            storyText.text += c;
            yield return new WaitForSeconds(0.03f);
        }
    }

    IEnumerator FadeOutBlack()
    {
        float t = 0;
        while (t < fadeDuration)
        {
            t += Time.deltaTime;
            float a = Mathf.Lerp(1f, 0f, t / fadeDuration);
            fadeImage.color = new Color(0,0,0,a);
            yield return null;
        }
    }
}
