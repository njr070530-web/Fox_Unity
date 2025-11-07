using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class bushcontroller : MonoBehaviour
{
    Rigidbody2D rb;
    public float speed ;
    public float jumpforce;
    // Start is called before the first frame update
    void Start()
    {
        rb  = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
    

    }
    void run()
    {
        float move = 0f;
        if (Input.GetKey(KeyCode.A)) move = -1f;
        if (Input.GetKey(KeyCode.D)) move = 1f;

        rb.velocity = new Vector2(move * speed, rb.velocity.y);
    }

    
    void jump()
    {
        if (Input.GetKeyDown(KeyCode.Space) && Mathf.Abs(rb.velocity.y) < 0.01f)
        {       
            rb.velocity = new Vector2(rb.velocity.x, jumpforce);
        }

    }
}
