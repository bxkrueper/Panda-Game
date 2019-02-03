/***************************************************************
* file: Splash.cs
* author: BaDkINgZ
* class: CS 470 Game Development
*
* assignment: final project
* date last modified: 5/28/2017
*
* purpose: manages the Splash Screen behavior
*
****************************************************************/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Splash : MonoBehaviour {

    public float timer = 3f;
    public string levelToLoad = "Start";

    //method: Start
    //purpose: constructor
    void Start () {
        StartCoroutine("DisplayScene");
	}
	
    //Method: DisplayScene
    //Purpose: display splash screen for (timer) seconds, then load start screen
	IEnumerator DisplayScene() {
        yield return new WaitForSeconds(timer);
        SceneManager.LoadScene(levelToLoad);
    }
}
