using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;




public class PlayerLife : MonoBehaviour
{
    private Rigidbody2D rb;
    private Animator anim;
    public int health = 3;

    [SerializeField] private AudioSource dieSoundEffect;

    [SerializeField] private AudioSource winSoundEffect;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Enemy"))
        {
            Hurt();
        }
    }

    private void Hurt()
    {
        anim.SetTrigger("hurt");
        Debug.Log($"💔 Hurt! Current HP = {health}");
        health -= 1;

        if (health <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        rb.bodyType = RigidbodyType2D.Static;
        dieSoundEffect.Play();
        anim.SetTrigger("die");
        Invoke("RestartLevel", 5f);
    }

    private void RestartLevel()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
    // private void OnTriggerEnter2D(Collider2D collision)
    // {
    //     if (collision.gameObject.CompareTag("Finish"))
    //     {
    //         Win();
    //     }
    // }
}
