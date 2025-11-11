using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoseToPlayer : MonoBehaviour
{
    [Header("引用")]
    public PlayerControl player;
    public PoseReceiver poseReceiver;

    [Header("人声控制")]
    public float pitchDeadZoneLow;
    public float pitchDeadZoneHigh;
    public float minVolumeToMove = 2.0f;

    void Start()
    {
        pitchDeadZoneLow = player.yourPitch - 150.0f;
        pitchDeadZoneHigh = player.yourPitch + 150.0f;
    }

    [Header("姿势控制")]
    public float poseJumpCooldown = 0.3f;
    private float lastPoseJumpTime = 0f;

    public float attackCooldown = 0.5f;
    private float lastAttackTime = 0f;

    private float previousDirection = 0f;

    // 攻击判定阈值（根据相机远近调整）
    public float attackThreshold = 0.5f;

    void Update()
    {
        var data = poseReceiver.latestPose;
        if (data == null) return;

        bool didMoveOrJumpOrAttack = false;

        // ------------------- 🎤 人声控制（保持原样） -------------------
        var voice = data.voice;
        if (voice != null && voice.isSpeech && voice.pitch > 500.0f)
        {
            float pitchNormalized = Mathf.Clamp((voice.pitch - pitchDeadZoneLow) / (pitchDeadZoneHigh - pitchDeadZoneLow), 0f, 1f);
            float direction = pitchNormalized * 2f - 1f; // -1 左，0 静止，1 右

            float smoothedDirection = Mathf.Lerp(previousDirection, direction, 0.1f);
            previousDirection = smoothedDirection;
            player.Move(smoothedDirection, 0.6f);
            didMoveOrJumpOrAttack = true;

            Debug.Log($"posetoplayer:Pitch={voice.pitch:F2}, volume={voice.volume:F2}, direction={direction:F2}");
        }

        // ------------------- 🧍 姿势控制 -------------------
        var lm = data.landmarks;
        bool leftRaised = false;
        if (lm != null && lm.ContainsKey("LEFT_WRIST") && lm.ContainsKey("NOSE"))
        {
            float leftY = lm["LEFT_WRIST"][1];
            float headY = lm["NOSE"][1];
            // y=0 顶部, y=1 底部 => 举手时 y 更小
            leftRaised = leftY < headY;
        }

        // 只用左手举高跳（且有冷却），左手举起时禁止攻击判定
        if (leftRaised && Time.time - lastPoseJumpTime > poseJumpCooldown)
        {
            // 进一步保证不会在正在攻击时跳（互斥）
            if (!player.IsAttacking())
            {
                player.Jump(1f);
                lastPoseJumpTime = Time.time;
                didMoveOrJumpOrAttack = true;
                Debug.Log("PoseToPlayer: Left hand jump triggered.");
            }
        }

        // 攻击判定：需要 LEFT_WRIST 和 RIGHT_ELBOW，两者足够靠近且左手**没有举起**
        if (lm != null && lm.ContainsKey("LEFT_WRIST") && lm.ContainsKey("RIGHT_ELBOW"))
        {
            if (!leftRaised && Time.time - lastAttackTime > attackCooldown && !player.IsAttacking())
            {
                var leftWrist = lm["LEFT_WRIST"];
                var rightElbow = lm["RIGHT_ELBOW"];

                float dx = leftWrist[0] - rightElbow[0];
                float dy = leftWrist[1] - rightElbow[1];
                float dz = leftWrist[2] - rightElbow[2];
                float distance = Mathf.Sqrt(dx * dx + dy * dy + dz * dz);

                if (distance < attackThreshold)
                {
                    player.Attack();
                    lastAttackTime = Time.time;
                    didMoveOrJumpOrAttack = true;
                    Debug.Log($"💥 Attack triggered! distance={distance:F3}");
                }
            }
        }

        // 如果没有任何输入（语音、动作），回到 Idle（避免覆盖在移动/跳/攻时被调用）
        if (!didMoveOrJumpOrAttack)
        {
            // 只有当不是在攻击时才设为 Idle（保持攻击动画）
            if (!player.IsAttacking())
                player.Idle();
        }
    }
}
