/***************************************************************
* file: CameraMovement.cs
* author: BaDkINgZ
* class: CS 470 Game Development
*
* assignment: final project
* date last modified: 5/6/2017
*
* purpose: manages the camera
*
****************************************************************/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMovement : MonoBehaviour {
    public float maxOffset;
    public float currentOffset;
    public float offsetVelocity; // how fast the camera scrolls relitive to the player
    public float frameLimit; // how long to wait until the camera starts scrolling

    private int frameCount; // keeps track of how long you've been facing the same way
    private Panda panda;

    //method: Start
    //purpose: constructor
    void Start () {
        panda = GameObject.Find("Panda").GetComponent<Panda>();
        frameCount = 0;
    }

    //method: Update
    //purpose: sets the camera to the player, offseted by a little to let the player
    // see more of what's in front of them
    void Update()
    {
        
        if ((panda.FacingRight()) && (currentOffset < maxOffset))
        {
            if (frameCount < frameLimit)
            {
                frameCount++;
            }

            if (frameCount >= frameLimit) {
                currentOffset += offsetVelocity * Time.deltaTime;
            }
        }
        if ((!panda.FacingRight()) && (currentOffset > -maxOffset))
        {
            if (frameCount > -frameLimit)
            {
                frameCount--;
            }
            if (frameCount <= -frameLimit)
            {
                currentOffset -= offsetVelocity * Time.deltaTime;
            }
        }
    }
	
	//method: LateUpdate
    //purpose: updates the position after all updates have been called
	void LateUpdate () {
        this.transform.position = panda.transform.position + new Vector3(currentOffset,0.0f,-10.0f);
    }
}
