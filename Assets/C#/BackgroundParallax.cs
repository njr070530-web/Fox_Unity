using UnityEngine;

public class BackgroundParallax : MonoBehaviour
{
    public Transform target;          // 玩家
    public float parallaxFactor = 0.3f; // 0~1 越小越远
    public float spacing = 10f;       // 山与山之间的距离（非常重要）

    private float startX;
    private float startY;
    private float spriteWidth;

    void Start()
    {
        startX = transform.position.x;
        startY = transform.position.y;

        spriteWidth = GetComponent<SpriteRenderer>().bounds.size.x;
    }

    void LateUpdate()
    {
        if (target == null) return;

        // 视差效果
        float distX = (target.position.x * parallaxFactor);
        float distY = (target.position.y * parallaxFactor * 1f); // y轻一点，不要太夸张

        transform.position = new Vector3(startX + distX, startY + distY, transform.position.z);

        // 玩家相对于山的移动（决定是否循环）
        float temp = target.position.x * (1 - parallaxFactor);

        // 是否移动下一山（一个sprite宽度 + spacing）
        float moveRange = spriteWidth + spacing;

        if (temp > startX + moveRange)
            startX += moveRange;

        else if (temp < startX - moveRange)
            startX -= moveRange;
    }
}
