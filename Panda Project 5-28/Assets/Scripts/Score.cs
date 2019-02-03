/***************************************************************
* file: Score.cs
* author: BaDkINgZ
* class: CS 470 Game Development
*
* assignment: final project
* date last modified: 5/28/2017
*
* purpose: manages the Score UI
*
****************************************************************/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Score : MonoBehaviour {

    public Text scoreTxt;
    public int scoreVal;

    //method: Start
    //purpose: constructor
    void Start () {
        scoreVal = 0;
        UpdateScore();
	}
	
	//Method: UpdateScore
    //Purpose: Updates score on the Score UI
	void UpdateScore () {
        scoreTxt.text = scoreVal.ToString();
	}

    //Method: IncreaseScore(int amt)
    //Purpose: increases score by amt amount
    public void IncreaseScore(int amt) {
        if (amt > 0)
            scoreVal += amt;
        UpdateScore();
    }
}
