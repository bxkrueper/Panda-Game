/***************************************************************
* file: ButtonManager.cs
* author: BaDkINgZ
* class: CS 470 Game Development
*
* assignment: final project
* date last modified: 5/19/2017
*
* purpose: manages the buttons on start screen
*
****************************************************************/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ButtonManager : MonoBehaviour {

    //Method: StartGameBtn
    //Purpose: load level1 when pressed
    public void StartGameBtn(string levelToLoad)  {
        SceneManager.LoadScene(levelToLoad);    
    }

    //Method: ExitGameBtn
    //Purpose: Exit application when pressed
    public void ExitGameBtn() {
        Application.Quit();
    }

}
