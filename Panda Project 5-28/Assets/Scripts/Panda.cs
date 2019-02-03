/***************************************************************
* file: Panda.cs
* author: BaDkINgZ
* class: CS 470 Game Development
*
* assignment: final project
* date last modified: 5/28/2017
*
* purpose: manages the panda
*
****************************************************************/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Panda : MonoBehaviour {

    public float leftDistanceFromCenter;//adjust to match sprite's feet
    public float rightDistanceFromCenter;//adjust to match sprite's feet
    public float bottomDistanceFromCenter;//adjust to match sprite's feet
    public float walkSpeed;
    public float jumpPower;
    public float fartPower;//how much the farts push the panda
    public const float FART_COOLDOWN = 0.05f;
    public float maxFartEnergy;
    public int criticalFartEnergy;
    public const int LOW_FART_ENERGY = 10; // if your fart energy is below this amount, you build energy more quickly
    public const float FART_ENERGY_RECOVER_TIME = 0.7f;
    public float fartSpeed;
    public float maxHealth;
    public float maxHurtFlashTime;
    public float fartEnergy;
    public int normalFartDamage;
    public int criticalFartDamage;
    public int giantFartDamage;
    public LayerMask platformLayerMask;
    public Vector2 respawnVector;

    private float health;
    private float randomNum;
    private bool facingRight;
    private bool grounded;
    private float fartCooldown; // time until another gas cloud is emitted
    private float fartEnergyRecoverTime;
    private float hurtFlashTime;
    private float oldX;
    private bool endGame;

    private Color hurtColor;
    private Color normalColor;
    BoxCollider2D boxCollider;
    private Rigidbody2D rb2d;
    SpriteRenderer spriteRenderer;


	//Audio
	public AudioClip hitSound;
	public AudioClip healSound;
	public AudioClip jumpSound;
	public AudioClip deathSound;
	public AudioClip biteSound;
	public AudioClip lowHPSound;

	private AudioSource source;
	private float vol = .5f;

    //Pause controller
    public bool paused;

    //UI Instance variables
    HpBarUI hpBar;
    FartGaugeUI fartGauge;
    Score score;
    Timer timer;

    //animation
    private Animator anim;

    //method: Awake
    //purpose: Get AudioSource component
    void Awake()
	{
		source = GetComponent<AudioSource>();
	}
    //method: Start
    //purpose: constructor
    void Start () {
        boxCollider = GetComponent<BoxCollider2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        normalColor = new Color(1f, 1f, 1f, 1f);
        hurtColor = new Color(1f, 0.5f, 0.5f, 1f);
        hurtFlashTime = 0;
        health = maxHealth;
        fartEnergyRecoverTime = 0.0f;
        fartCooldown = 0.0f;
        fartEnergyRecoverTime = FART_ENERGY_RECOVER_TIME;
        facingRight = true;
        paused = false;
        endGame = false;
        hpBar = GetComponent<HpBarUI>();
        fartGauge = GetComponent<FartGaugeUI>();
        score = GetComponent<Score>();
        timer = GetComponent<Timer>();
        anim = gameObject.GetComponent<Animator>();
        rb2d = gameObject.GetComponent<Rigidbody2D>();
        grounded = true;
        oldX = 0;
    }
	
	//method: Update
    //purpose: controls the panda given the input
	void Update () {
        KillIfOutOfBounds();
        AnimationManager();

		if (health == 1) {
			source.PlayOneShot (lowHPSound, .5f);
		}
        UpdatePandaAffairs();
        grounded = IsGrounded();

        if (!endGame)
        {
            //move left
            if (Input.GetKey(KeyCode.A))
            {
                MoveLeft();

            }

            // move right
            if (Input.GetKey(KeyCode.D))
            {
                MoveRight();
            }

            // jump
            if (Input.GetKey(KeyCode.Space))
            {
                Jump();
            }

            // fart
            if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.LeftShift))
            {
                if (fartCooldown <= 0 && fartEnergy > 0)
                    Fart();
            }

            // pause
            if (Input.GetKeyDown(KeyCode.P))
            {
                paused = !paused;
            }
            if (paused)
            {
                Time.timeScale = 0;
            }
            else if (!paused)
            {
                Time.timeScale = 1;
            }
        }
    }

    //method: AnimationManager
    //purpose: handles animations
    private void AnimationManager()
    {
        anim.SetBool("Grounded", grounded);
        anim.SetFloat("Speed", Mathf.Abs(transform.position.x-oldX));
        anim.SetBool("Hit", hurtFlashTime > 0);
        anim.SetBool("Fart", fartCooldown>0);
    }

    //method: GetMaxHealth
    //purpose: Getter for maxHealth
    public float GetMaxHealth() {
        return maxHealth;
    }

    //method: GetMaxFartEnergy
    //purpose: Getter for maxFartEnergy
    public float GetMaxFartEnergy() {
        return maxFartEnergy;
    }

    //method: GetFartEnergy
    //purpose: Getter for fartEnergy
    public float GetFartEnergy()
    {
        return fartEnergy;
    }

    //method: HurtFlashManager
    //purpose: keeps track of when the panda should return to a normal color
    private void HurtFlashManage()
    {
        if (hurtFlashTime > 0)
        {
            hurtFlashTime -= Time.deltaTime;
        }
        else
        {
            spriteRenderer.color = normalColor;
        }
    }

    //method: FacingRight()
    //purpose: returns whether or not it is facing right. if false, it is facing left
    public bool FacingRight()
    {
        return facingRight;
    }

    //method: MoveRight
    //purpose: moves the panda right and makes sure it is facing the right direction
    void MoveRight()
    {
        facingRight = true;
        spriteRenderer.flipX = false;
        this.transform.position += this.transform.right * walkSpeed * Time.deltaTime;
        
    }

    //method: MoveLeft
    //purpose: moves the panda left and makes sure it is facing the right direction
    void MoveLeft()
    {
        if (!Input.GetKey(KeyCode.D)) // facing right takes priority if both A and D keys are pressed
        {
            facingRight = false;
            spriteRenderer.flipX = true;
        }
        this.transform.position -= this.transform.right * walkSpeed * Time.deltaTime;

    }

    //method: Jump
    //method: makes the panda jump if it is grounded
    void Jump()
    {
        //this.GetComponent<Rigidbody2D>().AddForce(Vector2.up * jumpPower);
        if (grounded)
        {
            Vector2 velocity = rb2d.velocity;
            velocity.y = jumpPower;
            rb2d.velocity = velocity;
			source.PlayOneShot(jumpSound,.25f);
        }
    }

    //method: IsGrounded
    //purpose: tests if the player is on solid ground. casts 3 rays downwards a short ways. if any of them hit a platform, return true
    bool IsGrounded()
    {
        return LeftSideOnGround() || CenterSideOnGround() || RightSideOnGround();
    }

    //method: LeftSideOnGround
    //purpose: tests if the left corner of the sprite is on the ground
    bool LeftSideOnGround()
    {
        RaycastHit2D left = Physics2D.Raycast(transform.position + new Vector3(-leftDistanceFromCenter, -bottomDistanceFromCenter, 0), -Vector2.up, 0.1f, platformLayerMask);
        return (left && left.collider.gameObject.tag.Equals("Platform"));
    }
    //method: CenterSideOnGround
    //purpose: tests if the bottom center of the sprite is on the ground
    bool CenterSideOnGround()
    {
        RaycastHit2D center = Physics2D.Raycast(transform.position + new Vector3(0, -bottomDistanceFromCenter, 0), -Vector2.up, 0.1f, platformLayerMask);
        return (center && center.collider.gameObject.tag.Equals("Platform"));
    }
    //method: RightSideOnGround
    //purpose: tests if the right corner of the sprite is on the ground
    bool RightSideOnGround()
    {
        RaycastHit2D right = Physics2D.Raycast(transform.position + new Vector3(rightDistanceFromCenter, -bottomDistanceFromCenter, 0), -Vector2.up, 0.1f, platformLayerMask);
        return (right && right.collider.gameObject.tag.Equals("Platform"));
    }


    //method: Fart
    //purpose: makes the panda fart in the right direction. Can only fart in one direction at a time.
    //farting up takes priority, then side to side, last, giant fart.
    void Fart()
    {
        if (Input.GetKey(KeyCode.W))
        {
            GameObject gasCloud = (GameObject)Instantiate(Resources.Load("Gas Cloud"));
            GasCloud g = gasCloud.GetComponent<GasCloud>();
			if (fartEnergy < 40) {
				g.setFartNum (2);
			} else {
				g.setFartNum (1);
			}
            rb2d.AddForce(Vector2.up * fartPower);
            g.xSpeed = 0.0f;
            g.ySpeed = -fartSpeed;
            g.transform.position = this.transform.position + new Vector3(0, -boxCollider.size.y / 2.2f);
            fartCooldown = FART_COOLDOWN;
            fartEnergy--;
            fartGauge.DecreaseFart(1);
            g.setDamage(normalFartDamage);
        }
        else if (Input.GetKey(KeyCode.S))
        {
            if (facingRight)
            {
                GameObject gasCloud = (GameObject)Instantiate(Resources.Load("Gas Cloud"));
                GasCloud g = gasCloud.GetComponent<GasCloud>();
				if (fartEnergy < 40) {
					g.setFartNum (2);
				} else {
					g.setFartNum (1);
				}
                rb2d.AddForce(Vector2.right * fartPower * 0.8f);
                g.transform.position = this.transform.position + new Vector3(-boxCollider.size.x / 2.2f, 0);
                g.xSpeed = -fartSpeed;
                g.ySpeed = 0.0f;
                g.growthRate *= 0.4f;
                fartCooldown = FART_COOLDOWN;
                fartEnergy--;
                fartGauge.DecreaseFart(1);
                g.setDamage(normalFartDamage);

            }

            else
            {
                GameObject gasCloud = (GameObject)Instantiate(Resources.Load("Gas Cloud"));
				GasCloud g = gasCloud.GetComponent<GasCloud>();
				if (fartEnergy < 40) {
					g.setFartNum (2);
				} else {
					g.setFartNum (1);
				}
                rb2d.AddForce(-Vector2.right * fartPower * 0.8f);

                g.transform.position = this.transform.position + new Vector3(boxCollider.size.x / 2.2f, 0);
                g.xSpeed = fartSpeed;
                g.ySpeed = 0.0f;
                g.growthRate *= 0.4f;
                fartCooldown = FART_COOLDOWN;
                fartEnergy--;
                fartGauge.DecreaseFart(1);
                g.setDamage(normalFartDamage);
            }
        }
        else if (fartEnergy >= 4)
        {
            GameObject gasCloud = (GameObject)Instantiate(Resources.Load("Gas Cloud"));
            GasCloud g = gasCloud.GetComponent<GasCloud>();
			if (fartEnergy < 40) {
				g.setFartNum (2);
			} else {
				g.setFartNum (1);
			}
            fartCooldown = FART_COOLDOWN * 20.0f;
            fartEnergy-= 4;
            fartGauge.DecreaseFart(4);
            g.transform.position = this.transform.position;
            g.xSpeed = 0.0f;
            g.ySpeed = 0.0f;
            g.growthRate *= 3.5f;
            g.setDamage(giantFartDamage);
        }
        
    }


    //method: CriticalFart
    //purpose: makes the panda fart randomly if he's too "full"... he can't hold it.
                                                                        //make the expresion and sound effects funny for this!
    void CriticalFart()
    {
        fartCooldown = FART_COOLDOWN;
        fartEnergy-= 5;
        fartGauge.DecreaseFart(5);
        GameObject gasCloud = (GameObject)Instantiate(Resources.Load("Gas Cloud"));
        GasCloud g = gasCloud.GetComponent<GasCloud>();
		g.setFartNum (3);
        g.setDamage(criticalFartDamage);
        if (Input.GetKey(KeyCode.W))
        {
            rb2d.AddForce(Vector2.up * fartPower * 5.0f);
            g.xSpeed = 0.0f;
            g.ySpeed = -fartSpeed * 2.0f;
            g.transform.position = this.transform.position + new Vector3(0, -boxCollider.size.y / 2.2f);
        }
        else
        {
            if (facingRight)
            {
                rb2d.AddForce(Vector2.right * fartPower * 5.0f);
                g.transform.position = this.transform.position + new Vector3(-boxCollider.size.x / 2.2f, 0);
                g.xSpeed = -fartSpeed * 2.0f;
                g.ySpeed = 0.0f;
                g.growthRate *= 0.4f;

            }

            else
            {
                rb2d.AddForce(-Vector2.right * fartPower * 5.0f);

                g.transform.position = this.transform.position + new Vector3(boxCollider.size.x / 2.2f, 0);
                g.xSpeed = fartSpeed * 2.0f;
                g.ySpeed = 0.0f;
                g.growthRate *= 0.4f;
            }
        }
        
    }

    //method: UpdatePandaAffairs
    //purpose: keep track of fart stats and hurtflash
    void UpdatePandaAffairs()
    {
        oldX = transform.position.x;
        HurtFlashManage();
        if (fartCooldown > 0)
        {
            fartCooldown -= Time.deltaTime;
        }


        //automatically gain fart energy if tank is low
        fartEnergyRecoverTime -= Time.deltaTime;
        if (fartEnergyRecoverTime <= 0)
        {
            fartEnergy++;
            fartGauge.IncreaseFart(1);
        if(fartEnergy < LOW_FART_ENERGY)
            fartEnergyRecoverTime = FART_ENERGY_RECOVER_TIME;
        else
            fartEnergyRecoverTime = FART_ENERGY_RECOVER_TIME*2;
        }

        // randomly fart if your tank is too full
        if (fartEnergy > criticalFartEnergy)
        {
             if (fartEnergy >= maxFartEnergy)
            {
                CriticalFart();
            }

            else{
                float fartChance = (float)(fartEnergy - criticalFartEnergy) * Time.deltaTime;
                randomNum = Random.Range(0.0f, (float)maxFartEnergy);
                if (fartChance > randomNum)
                {
                    CriticalFart();
                }
            }
        }




    }

    //method: OnTriggerEnter2D
    //purpose: describes what to do when the panda enters a collider
    void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.gameObject.tag == "Heart")
        {
            print("collected heart");
            Heal(1);
            score.IncreaseScore(50);
            Destroy(collider.gameObject);
        }
        else if (collider.gameObject.tag == "Bamboo")
        {
            print("eat bamboo");
            EatBamboo(10);
            score.IncreaseScore(50);
            Destroy(collider.gameObject);
        }
        else if (collider.gameObject.tag == "Bullet")
        {
            print("shot by bullet");
            Hurt(1);
            //Destroy(collider.gameObject);
        }
        else if (collider.gameObject.tag == "Goal")
        {
            print("Goal reached!");
            score.IncreaseScore(1000);
            timer.Finished();
            endGame = true;
            StartCoroutine(triggersCredit());
        }
    }

    //method: EatBamboo
    //purpose: adds the fartEnergy to the total (capped)
    void EatBamboo(int ammountAttempted)
    {
		source.PlayOneShot(biteSound,vol);
        if (fartEnergy + ammountAttempted >= maxFartEnergy)
        {
            fartEnergy = maxFartEnergy;
            fartGauge.IncreaseFart(maxFartEnergy);
        }
        else
        {
            fartEnergy += ammountAttempted;
            fartGauge.IncreaseFart(ammountAttempted);
        }
        print("fart energy: " + fartEnergy);
    }

    //method: Heal
    //purpose: heals the panda (capped at maxHealth)
    //input: >0
    public void Heal(int ammountAttempted)
    {
		source.PlayOneShot(healSound,vol);
        if (health + ammountAttempted >= maxHealth)
        {
            health = maxHealth;
            hpBar.IncreaseHp(maxHealth);
        }else
        {
            health += ammountAttempted;
            hpBar.IncreaseHp(ammountAttempted);
        }
        print("health: " + health);
    }

    //method: Hurt
    //purpose: gives damage to the panda
    //input: >0
    public void Hurt(int ammountAttempted)
    {
		source.PlayOneShot(hitSound,vol);
        spriteRenderer.color = hurtColor;
        hurtFlashTime = maxHurtFlashTime;
        if (health - ammountAttempted <= 0)
        {
            float louder = vol * 2;
			source.PlayOneShot(deathSound,louder);
            health = 0;
            hpBar.DecreaseHp(maxHealth);
            print("game over");

            Respawn();
        }
        else
        {
            health -= ammountAttempted;
            hpBar.DecreaseHp(ammountAttempted);
        }
        print("health: " + health);
    }

    //method: Respawn
    //purpose: respawns the panda at the last checkpoint, restores health and fart enerty
    //and reactivates traps
    void Respawn()
    {
        this.transform.position = respawnVector;
        health = maxHealth;
        hpBar.IncreaseHp(maxHealth);
        fartEnergy = 50;
        fartGauge.SetFart(50);

        Trap[] traps = GameManager.FindObjectsOfType<Trap>();
        for (int i = 0; i < traps.Length; i++)
        {
            traps[i].Reactivate();
        }

    }

    //method: SetRespawn
    //purpose: sets the panda's respawn point to the given argument
    public void SetRespawn(Vector2 vect)
    {
        respawnVector = vect;
    }

    //method: KillIfOutOfBounds
    //purpose: kills the panda if he falls off the stage
    public void KillIfOutOfBounds()
    {
        if (transform.position.y < -14)
            Hurt(1000);
    }

    //method: OnGUI
    //purpose: displays pause menu when the game is paused
    public void OnGUI()
    {
        if (paused)
        {
            GUI.BeginGroup(new Rect(((Screen.width / 2) - 100), ((Screen.height / 2) - 85), 200, 170));
            if (GUI.Button(new Rect(0, 0, 200, 50), "Main Menu"))
            {
                SceneManager.LoadScene("Start");
            }
            if (GUI.Button(new Rect(0, 60, 200, 50), "Restart Game"))
            {
                SceneManager.LoadScene("Level1");
            }
            if (GUI.Button(new Rect(0, 120, 200, 50), "Quit Game"))
            {
                Application.Quit();
            }
            GUI.EndGroup();
        }
    }

    //method: TriggersCredit
    //purpose: Wait 5 seconds and transition to credit scene
    public IEnumerator triggersCredit()
    {
        yield return new WaitForSeconds(5);
        SceneManager.LoadScene("Credit");
    }
}
