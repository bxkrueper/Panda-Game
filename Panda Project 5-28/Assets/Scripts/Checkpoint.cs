/***************************************************************
* file: Checkpoint.cs
* author: BaDkINgZ
* class: CS 470 Game Development
*
* assignment: final project
* date last modified: 5/13/2017
*
* purpose: manages the checkpoint object
*
****************************************************************/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    SpriteRenderer spriteRenderer;
    public Sprite img2;
    private bool setActive;

    //UI
    Score score;

    //method: Start
    //purpose: constructor
    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        setActive = false;
        score = GameObject.Find("Panda").GetComponent<Score>();
    }

    //method: OnTriggerEnter2D
    //purpose: if the panda triggered the checkpoint, update the panda's spawn and activate the checkpoint
    void OnTriggerEnter2D(Collider2D collider)
    {
        if (!setActive && collider.gameObject.tag == "Panda")
        {
            print("checkpoint triggered!!");
            setActive = true;
            collider.gameObject.GetComponent<Panda>().SetRespawn(transform.position);
            spriteRenderer.sprite = img2;
            score.IncreaseScore(100);
        }
    }
}
