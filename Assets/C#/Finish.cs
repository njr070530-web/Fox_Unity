using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Finish : MonoBehaviour
{
    private bool levelCompleted = false;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!levelCompleted && collision.CompareTag("Player"))
        {
            levelCompleted = true;
            StartCoroutine(DelayedLoad());
        }
    }

    IEnumerator DelayedLoad()
    {
        yield return new WaitForSeconds(1f);  // 等待 1 秒
        SceneManager.LoadScene("WinScene");
    }
}
