/***************************************************************
* file: Heart.cs
* author: BaDkINgZ
* class: CS 470 Game Development
*
* assignment: final project
* date last modified: 5/6/2017
*
* purpose: manages the heart object
*
****************************************************************/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Heart : MonoBehaviour {

    public float beatSizeMax;
    public float longBeatPeriod;
    public float beatPeriod;
    public float shortBeatPeriod;
    private int beatState;//0=neutral, 1=first beat, 2=inbetween beats, 3=second beat
    private float animationClock;

	//method: Start
    //purpose: constructor
	void Start () {
        beatState = 0;
        animationClock = 0.0f;
	}
	
	//method: Update
    //purpose: manages the heart's animation
	void Update () {
        animationClock += Time.deltaTime;
        switch (beatState)
        {
            
            case 0:
                if (animationClock > longBeatPeriod)
                {
                    beatState = 1;
                    transform.localScale *= beatSizeMax;
                }
                    

                break;
            case 1:
                if (animationClock > longBeatPeriod+beatPeriod)
                {
                    beatState = 2;
                    transform.localScale /= beatSizeMax;
                }
                break;
            case 2:
                if (animationClock > longBeatPeriod + beatPeriod+shortBeatPeriod)
                {
                    beatState = 3;
                    transform.localScale *= beatSizeMax;
                }
                break;
            case 3:
                if (animationClock > longBeatPeriod + 2*beatPeriod + shortBeatPeriod)
                {
                    beatState = 0;
                    transform.localScale /= beatSizeMax;
                    animationClock = 0.0f;
                }
                break;
        }
	}
}
