/***************************************************************
* file: Timer.cs
* author: BaDkINgZ
* class: CS 470 Game Development
*
* assignment: final project
* date last modified: 5/28/2017
*
* purpose: manages the Timer UI
*
****************************************************************/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Timer : MonoBehaviour {

    public Text timerVal;
    private float startTime;
    private bool finished = false;

	//method: Start
    //purpose: constructor
	void Start () {
        startTime = Time.time;
	}
	
	//method: Update
    //purpose: counts time
	void Update () {
        if (finished) return;
        float t = Time.time - startTime;

        string minutes = ((int)t / 60).ToString();
        string seconds = (t % 60).ToString("f0");

        timerVal.text = minutes + ':' + seconds;
	}

    //Method: Finished
    //Purpose: When the timer stops, the timer turns green
    public void Finished()
    {
        finished = true;
        timerVal.color = Color.green;
    }
}
