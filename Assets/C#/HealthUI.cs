using UnityEngine;
using UnityEngine.UI;

public class HealthUI : MonoBehaviour
{
    public Image[] hearts;            // 心图标数组
    public Sprite fullHeart;          // 实心心
    // public Sprite emptyHeart;         // 空心心

    public void UpdateHearts(int currentHP)
    {
        for (int i = 0; i < hearts.Length; i++)
        {
            if (i < currentHP)
                hearts[i].sprite = fullHeart;
            else
                hearts[i].enabled = false;
                // hearts[i].sprite = emptyHeart;
        }
    }
}
