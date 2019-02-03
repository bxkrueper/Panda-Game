/***************************************************************
* file: Bullet.cs
* author: BaDkINgZ
* class: CS 470 Game Development
*
* assignment: final project
* date last modified: 5/6/2017
*
* purpose: manages the bullet object
*
****************************************************************/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour {

    public float lifeSpan;
    private Animator anim;

    //method: Awake
    //purpose: destroys the object after its lifespan is up
    void Awake()
    {
        anim = gameObject.GetComponent<Animator>();
        Destroy(this.gameObject, lifeSpan); // destroy after set time has passed
    }


    //method: OnTriggerEnter2D
    //purpose: destoys the bullet if it hits a platform
    void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.gameObject.tag == "Platform")
        {
            Destroy(this.gameObject);
        }
        if (collider.gameObject.tag == "Panda")
        {
            Destroy(this.gameObject);
        }
    }
}
