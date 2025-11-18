using System.Collections;
using UnityEngine;

public class Finish : MonoBehaviour
{
    private bool levelCompleted = false;

    public SceneTransition transition;      // 负责黑屏 + 文字 + Camera 显示的控制器

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.name == "player" && !levelCompleted)
        {
            levelCompleted = true;
            Debug.Log("Level Completed!");
            StartCoroutine(FinishGame());
        }
    }

    private IEnumerator FinishGame()
    {
        // 等待 0.2 秒保证玩家触发感到自然（可选）
        // yield return new WaitForSeconds(0.2f);

        // 🚀 调用你自己写的过场动画系统
        transition.StartEndingSequence("你在黑暗中听到了自己的呼吸...");

        yield return null;
    }
}
