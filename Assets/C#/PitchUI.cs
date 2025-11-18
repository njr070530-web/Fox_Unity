using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PitchUI : MonoBehaviour
{
    public Slider yourPitchSlider;
    public TextMeshProUGUI yourPitchText;

    private PlayerControl player;

    void Start()
    {
        player = FindObjectOfType<PlayerControl>();

        // 初始化 Slider 与文本
        yourPitchSlider.value = player.yourPitch;
        UpdatePitchText(player.yourPitch);

        // 注册滑动事件
        yourPitchSlider.onValueChanged.AddListener(OnPitchChanged);
    }

    void OnPitchChanged(float newValue)
    {
        player.yourPitch = newValue;
        UpdatePitchText(newValue);
    }

    void UpdatePitchText(float value)
    {
        yourPitchText.text = $"Your Pitch: {value:F0} Hz";
    }
}
