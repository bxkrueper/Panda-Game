/***************************************************************
* file: Goal.cs
* author: BaDkINgZ
* class: CS 470 Game Development
*
* assignment: final project
* date last modified: 5/28/2017
*
* purpose: manages the goal
*
****************************************************************/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Goal : MonoBehaviour
{
    SpriteRenderer spriteRenderer;
    private bool setActive;
    private Animator anim;
    private bool triggered;
    private float timeSinceTriggered;
    public float leftDistanceFromCenter;//adjust to match sprite's feet
    public float rightDistanceFromCenter;//adjust to match sprite's feet
    public float bottomDistanceFromCenter;//adjust to match sprite's feet
    Vector3 basePosition;
    private bool creditsShown = false;

    public LayerMask platformLayerMask;
    private Rigidbody2D rb2d;


    //method: Start
    //purpose: constructor
    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        setActive = false;

        anim = gameObject.GetComponent<Animator>();
        anim.SetBool("active", false);
        rb2d = gameObject.GetComponent<Rigidbody2D>();
        basePosition = this.transform.position;
    }


    //method: Update
    //purpose: checks if panda has touched the goal. If it has, show cutscene
    private void Update()
    {
        if (triggered)
        {
            timeSinceTriggered += Time.deltaTime;
            float yOffset = 1.5f*Mathf.Sin(5*timeSinceTriggered);
            if (yOffset < 0)
            {
                yOffset = 0;
            }
            Vector3 offset = new Vector3(0, yOffset, 0);
            this.transform.position = basePosition + offset;

            if (timeSinceTriggered > 3 && !creditsShown)
            {
                creditsShown = true;
                print("show credits in goal script");
                
            }

        }
    }

    //method: OnTriggerEnter2D
    //purpose: if the panda triggered the goal the level is beat
    void OnTriggerEnter2D(Collider2D collider)
    {
        if (!setActive && collider.gameObject.tag == "Panda")
        {
            print("goal triggered!!");
            setActive = true;
            anim.SetBool("active", true);
            triggered = true;

            Hunter[] hunters = GameManager.FindObjectsOfType<Hunter>();
            for (int i = 0; i < hunters.Length; i++)
            {
                hunters[i].Hurt(1000);
            }



        }
    }

}
