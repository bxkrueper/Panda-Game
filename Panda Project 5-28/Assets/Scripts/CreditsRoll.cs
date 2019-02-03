/***************************************************************
* file: CreditsRoll.cs
* author: BaDkINgZ
* class: CS 470 Game Development
*
* assignment: final project
* date last modified: 5/28/2017
*
* purpose: manages the credit scene
*
****************************************************************/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CreditsRoll : MonoBehaviour {

    public GameObject camera;
    public int speed = 2;

    //method: Update
    //purpose: scrolls credits down
    void Update () {
        if (camera.transform.position.y >= -50)
        {
            camera.transform.Translate(Vector2.down * Time.deltaTime * speed);
        } else
        {
            StartCoroutine(waitFor());
        }
    }

    //wait 5 second before transitioning back to start screen
    IEnumerator waitFor()
    {
        yield return new WaitForSeconds(5);
        SceneManager.LoadScene("Start");
    }
}
