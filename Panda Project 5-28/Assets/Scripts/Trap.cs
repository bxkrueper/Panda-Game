/***************************************************************
* file: Trap.cs
* author: BaDkINgZ
* class: CS 470 Game Development
*
* assignment: final project
* date last modified: 5/28/2017
*
* purpose: manages the trap object
*
****************************************************************/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Trap : MonoBehaviour {
    SpriteRenderer spriteRenderer;
    private Animator anim;
    private bool active;
    //public Panda panda;

    //method: Start
    //purpose: constructor
    void Start () {
        spriteRenderer = GetComponent<SpriteRenderer>();
        active = true;
        
        anim = gameObject.GetComponent<Animator>();
        anim.SetBool("active", true);
    }

    //method: OnTriggerEnter2D
    //purpose: if the panda triggered the trap, hurt the panda and make the trap inert
    void OnTriggerEnter2D(Collider2D collider)
    {
        if (active && collider.gameObject.tag == "Panda")
        {
            print("trap triggered!!");
            active = false;
            anim.SetBool("active", false);
            collider.gameObject.GetComponent<Panda>().Hurt(3);

        }
    }

    //method: Reactivate
    //purpose: reactivates traps
    public void Reactivate()
    {
        active = true;
        anim.SetBool("active", true);
    }
}
