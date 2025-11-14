using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    private Animator anim;

    private void Awake()
    {
        anim = GetComponent<Animator>();
    }

    public void Die()
    {
        anim.SetTrigger("die");
        Debug.Log("💀 Enemy Died!");
        Destroy(gameObject, 1f);
    }
}
