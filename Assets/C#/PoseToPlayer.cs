using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoseToPlayer : MonoBehaviour
{
    [Header("引用")]
    public PlayerControl player;
    public PoseReceiver poseReceiver;

    [Header("人声控制")]
    // public float pitchDeadZoneLow = 10000.0f;
    // public float pitchDeadZoneHigh =20000.0f;
    // public float yourPitch = 1000f;

    public float pitchDeadZoneLow;
    public float pitchDeadZoneHigh;
    public float minVolumeToMove = 2.0f;

    void Start()
    {
        pitchDeadZoneLow = player.yourPitch - 150.0f;
        pitchDeadZoneHigh = player.yourPitch + 150.0f;
        
    }
    // public float minVolumeToJump = 0.25f;
    // public float jumpForceMin = 0.5f;
    // public float jumpForceMax = 1.0f;

    [Header("姿势控制")]
    public float poseJumpCooldown = 0.3f;
    private float lastPoseJumpTime = 0f;
    private float previousDirection = 0f;

    void Update()
    {
        var data = poseReceiver.latestPose;
        if (data == null) return;
        // Debug.Log($"Received landmarks={count}, volume={volume:F2}pitch={pitch:F2}, isSpeech={isSpeech}");


        // ------------------- 🎤 人声控制 -------------------
        var voice = data.voice;

        if (voice != null && voice.isSpeech && voice.pitch>500.0f)
        {
            // float direction = 0f;
            // if (voice.pitch < pitchDeadZoneLow) direction = -1f;
            // else if (voice.pitch > pitchDeadZoneHigh) direction = 1f;
            // pitchNormalized: 0~1，0 = 左，0.5 = 静止，1 = 右
            float pitchNormalized = Mathf.Clamp((voice.pitch - pitchDeadZoneLow) / (pitchDeadZoneHigh - pitchDeadZoneLow), 0f, 1f);
            float direction = pitchNormalized * 2f - 1f; // -1 左，0 静止，1 右

            float smoothedDirection = Mathf.Lerp(previousDirection, direction, 0.1f);
            previousDirection = smoothedDirection;
            player.Move(smoothedDirection, 0.6f);
            Debug.Log($"posetoplayer:Pitch={voice.pitch:F2}, volume={voice.volume:F2}, direction={direction:F2}");

            // 音量映射移动速度
            // float moveSpeed = Mathf.Clamp01(voice.volume);
            // player.Move(direction, 1.0f);

            // 跳跃控制
            // if (voice.volume > minVolumeToJump)
            // {
            //     float jumpForce = Mathf.Clamp(voice.volume, jumpForceMin, jumpForceMax);
            //     player.Jump(jumpForce);
            // }
        }
        else
        {
            player.Idle();
        }


            // ------------------- 🧍 姿势跳跃 -------------------
            var lm = data.landmarks;
        if (lm != null && lm.ContainsKey("LEFT_WRIST") && lm.ContainsKey("RIGHT_WRIST") && lm.ContainsKey("NOSE"))
        {
            float leftY = lm["LEFT_WRIST"][1];
            float rightY = lm["RIGHT_WRIST"][1];
            float headY = lm["NOSE"][1];

            // y=0 顶部, y=1 底部 => 举手时 y 更小
            if ((leftY < headY || rightY < headY) && Time.time - lastPoseJumpTime > poseJumpCooldown)
            {
                player.Jump(1f);
                lastPoseJumpTime = Time.time;
            }
        }
    }
}
