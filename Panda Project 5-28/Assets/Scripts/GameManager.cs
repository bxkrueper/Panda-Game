/***************************************************************
* file: GameManager.cs
* author: BaDkINgZ
* class: CS 470 Game Development
*
* assignment: final project
* date last modified: 5/6/2017
*
* purpose: controls higher game commands
*
****************************************************************/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour {
    public static GameObject panda;//gives everyone access to the panda


    // Static instance of the Game Manager,
    // can be access from anywhere
    public static GameManager instance = null;
    //method: Awake
    //purpose: constructor
    void Awake()
    {
        panda = GameObject.Find("Panda");
        // if it doesn't exist
        if (instance == null)
        {
            // Set the instance to the current object (this)
            instance = this;
        }
        // There can only be a single instance of the game manager
        else if (instance != this)
        {
            // Destroy the current object, so there is just one manager
            Destroy(gameObject);
        }
        // Don't destroy this object when loading scenes
        DontDestroyOnLoad(gameObject);



        LoadObjects();
        
    }

    //method: LoadObjects
    //purpose: loads objects in the lever
    //can use this concept to make more levels and delete old ones?
    void LoadObjects()
    {
        //Instantiate(Resources.Load("Heart"), new Vector2(1, 1), new Quaternion());
        //Instantiate(Resources.Load("Hunter"), new Vector2(1, -5), new Quaternion());
        //Instantiate(Resources.Load("Hunter"), new Vector2(2, -5), new Quaternion());
        //Instantiate(Resources.Load("Hunter"), new Vector2(25, 5), new Quaternion());
        //Instantiate(Resources.Load("Trap"), new Vector2(20, -5), new Quaternion());
        //Instantiate(Resources.Load("Bamboo"), new Vector2(30, -5), new Quaternion());
        //Instantiate(Resources.Load("Goal"), new Vector2(35, -6), new Quaternion());
    }
}
