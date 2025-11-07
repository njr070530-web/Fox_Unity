using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoseToPlayer : MonoBehaviour
{
    [Header("引用")]
    public PlayerControl player;
    public PoseReceiver poseReceiver;

    [Header("人声控制")]
    public float pitchDeadZoneLow = 0.03f;
    public float pitchDeadZoneHigh = 0.04f;
    public float minVolumeToMove = 5.0f;
    // public float minVolumeToJump = 0.25f;
    // public float jumpForceMin = 0.5f;
    // public float jumpForceMax = 1.0f;

    [Header("姿势控制")]
    public float poseJumpCooldown = 0.3f;
    private float lastPoseJumpTime = 0f;

    void Update()
    {
        var data = poseReceiver.latestPose;
        if (data == null) return;

        // ------------------- 🎤 人声控制 -------------------
        var voice = data.voice;

        if (voice != null && voice.isSpeech 
        && voice.volume > minVolumeToMove && voice.pitch>0.05f)
        {
            float direction = 0f;
            if (voice.pitch < pitchDeadZoneLow) direction = -1f;
            else if (voice.pitch > pitchDeadZoneHigh) direction = 1f;

            // 音量映射移动速度
            float moveSpeed = Mathf.Clamp01(voice.volume);
            player.Move(direction, moveSpeed);

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
