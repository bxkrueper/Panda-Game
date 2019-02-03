/***************************************************************
* file: GasCloud.cs
* author: BaDkINgZ
* class: CS 470 Game Development
*
* assignment: final project
* date last modified: 5/6/2017
*
* purpose: manages the GasCloud object
*
****************************************************************/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GasCloud : MonoBehaviour {

    public float xSpeed;
    public float ySpeed;
    public float lifeSpan;
    public float size;
    public float growthRate;

    private int damage;
    private float timeExisted;

    SpriteRenderer spriteRenderer;

	//Audio
	public AudioClip fartSound;
	public AudioClip fartWeak;
	public AudioClip fartLoud;

	private AudioSource source;
	private float vol = .5f;
	private int fartNum;



    //method: Start
    //purpose: constructor
    void Start () {
        spriteRenderer = GetComponent<SpriteRenderer>();
        timeExisted = 0.0f;
		switch (fartNum)
		{
			case 1:
				source.PlayOneShot(fartSound,vol);
				break;
			case 2:
				source.PlayOneShot(fartWeak,vol);
				break;
			case 3:
				source.PlayOneShot(fartLoud,vol);
				break;
			default:
				break;
		}
    }

    //method: Awake
    //purpose: destroys the object after a certain amount of time defined by lifespan and get AudioSource component
    void Awake()
    {
        //this.transform.parent = GameManager.panda.transform;
        Destroy(this.gameObject, lifeSpan); // destroy after set time has passed
		source = GetComponent<AudioSource>();
    }

    //method: setDamage
    //purpose: sets the damage this gas cloud will do
    public void setDamage(int d)
    {
        damage = d;
    }

    //method: getDamage
    //purpose: returns the damage this gas cloud does
    public int getDamage()
    {
        return damage;
    }
	
	//method: Update
    //purpose: manages the gas cloud's position and scale and sets its opacity as a function of the percentage of its lifespan
	void Update () {
        //change opacity
        float opacity = (lifeSpan - timeExisted) / lifeSpan;
        if (opacity < 0) opacity = 0;
        spriteRenderer.color = new Color(1f, 1f, 1f, opacity);
        timeExisted += Time.deltaTime;

        //position
        this.transform.position += new Vector3(xSpeed,ySpeed) * Time.deltaTime;

        // increase size
        this.transform.localScale += new Vector3(growthRate * Time.deltaTime, growthRate * Time.deltaTime);
        GetComponent<CircleCollider2D>().radius += growthRate * Time.deltaTime / 10;
    }


    //method: OnTriggerEnter2D
    //purpose: makes the fart stop if it hits a wall
    void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.gameObject.tag == "Platform")
        {
            // stop
            xSpeed = 0;
            ySpeed = 0;
        }
    }
	public void setFartNum(int i){
		fartNum = i;
	}




}
