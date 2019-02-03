/***************************************************************
* file: Hunter.cs
* author: BaDkINgZ
* class: CS 470 Game Development
*
* assignment: final project
* date last modified: 5/28/2017
*
* purpose: manages the Hunter object
*
****************************************************************/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hunter : MonoBehaviour {

    public int maxHealth;
    public float maxPursuitTime;
    public float maxHurtFlashTime;
    public float maxFireCooldown;
    public float leftDistanceFromCenter;//adjust to match sprite's feet
    public float rightDistanceFromCenter;//adjust to match sprite's feet
    public float verticalDistanceFromCenter;//adjust to match sprite's feet
    public float walkSpeed;
    public float bulletSpeed;
    public float sightRange;//how far the hunters sight rays search for the panda
    public float deathTime;
    public float moveTime;
    public float idleTime;

    public LayerMask shootScanLayerMask;//which layers the raycasts will interact with while searching for the panda
    public LayerMask platformLayerMask;//which layers count as ground
    public LayerMask movementObstructionLayerMask;//which layers make the hunter turn around
    public GameObject gun;
    public GameObject bullet;

    private Color hurtColor;
    private Color normalColor;
    private Vector3 rightAimingDirection;
    private Vector3 leftAimingDirection;
    private Vector2 firingDirection;//the current directions the gun is aiming
    private SpriteRenderer spriteRenderer;
    private BoxCollider2D boxCollider;
    private Rigidbody2D rb2d;
    public Panda panda;
    private int state;//0: idle, 1: walking, 2, firing 3: dying

    private float moveOrIdleTime;
    private float pursuitTime;
    private float hurtFlashTime;
    private float fireCooldown; // time until another gas cloud is emitted
    private bool facingRight;
    private int health;

    //UI
    Score score;

	//Audio
	public AudioClip gunShotSound;

	private AudioSource source;
	private float vol = .25f;

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
    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        boxCollider = GetComponent<BoxCollider2D>();
        anim = gameObject.GetComponent<Animator>();
        rb2d = gameObject.GetComponent<Rigidbody2D>();
        firingDirection = new Vector2(-1, 0);
        health = maxHealth;
        fireCooldown = maxFireCooldown;
        facingRight = false;
        normalColor = new Color(1f, 1f, 1f,1f);
        hurtColor = new Color(0.5f, 1f, 1f, 1f);//i tried making this public and setting it in the inspector panel, but it didn't work
        rightAimingDirection = new Vector3(0, 0, 0);
        leftAimingDirection = new Vector3(0, 0, Vector2.Angle(Vector2.left, Vector2.right));
        hurtFlashTime = 0;
        pursuitTime = 0;
        state = 0;
        moveOrIdleTime = 0.0f;

        score = GameObject.Find("Panda").GetComponent<Score>();

        gun = (GameObject) Instantiate(Resources.Load("Gun"), new Vector2(1, 1), new Quaternion());
        gun.transform.parent = this.transform;

        panda = GameObject.Find("Panda").GetComponent<Panda>();
    }

    //method: HurtFlashManager
    //purpose: keeps track of when the hunter should return to a normal color
    private void HurtFlashManage()
    {
        if (hurtFlashTime > 0)
        {
            hurtFlashTime -= Time.deltaTime;
        }else
        {
            spriteRenderer.color = normalColor;
        }
    }

    //method: AnimationManager
    //purpose: handles animations
    private void AnimationManager()
    {
        anim.SetInteger("state", state);
    }

    //method: Update
    //purpose: Manages the hunter's AI frame by frame
    void Update()
    {
        KillIfOutOfBounds();
        AnimationManager();
        if (state == 3)//if dying, don't continue
            return;
        HurtFlashManage();
        AnimationManager();
        gun.transform.position = this.transform.position;

        bool seesPanda = CanSeePanda();
        if (seesPanda)
        {
            pursuitTime = maxPursuitTime;
        }
            

        if (seesPanda || pursuitTime > 0)
        {
            state = 2;//firing
            moveOrIdleTime = 0;
            FacePanda();
            AimAtPanda();
            if (pursuitTime > 0)
                pursuitTime -= Time.deltaTime;
        }
        else
        {
            MoveOrIdle();
        }

        if (fireCooldown <= 0 && (seesPanda))
        {
            Fire();
            fireCooldown = maxFireCooldown;
        }else
        {
            if (fireCooldown > 0)
                fireCooldown -= Time.deltaTime;
        }
        
    }

    //method: FacePanda
    //purpose: faces in the direction of the panda
    void FacePanda()
    {
        if (panda.transform.position.x > this.transform.position.x)
            FaceRight();
        else
            FaceLeft();
    }

    //method: AimAtPanda
    //purpose: sets the firingDirection and gun angle to point at the panda
    void AimAtPanda()
    {
        float xToPanda = panda.transform.position.x - this.transform.position.x;
        float yToPanda = panda.transform.position.y - this.transform.position.y;
        firingDirection = new Vector2(xToPanda,yToPanda).normalized;

        if (yToPanda >= 0)
        {
            gun.transform.eulerAngles = new Vector3(0, 0, Vector2.Angle(firingDirection,Vector2.right));
        }else
        {
            if(xToPanda>=0)
                gun.transform.eulerAngles = new Vector3(0, 0, -Vector2.Angle(firingDirection, Vector2.right));
            else
                gun.transform.eulerAngles = new Vector3(0, 0, Mathf.PI*2-Vector2.Angle(firingDirection, Vector2.right));
        }
        
    }

    //method: CanSeePanda
    //purpose: tests if there is a direct line of sight to the panda with no platforms in the way
    bool CanSeePanda()
    {
        if ((panda.transform.position - this.transform.position).magnitude<4)
            return true;


        float startAngle, endAngle;
        if (facingRight)
        {
            startAngle = -Mathf.PI / 4;
            endAngle = Mathf.PI / 2;
        }else
        {
            startAngle = Mathf.PI / 2.0f;
            endAngle = Mathf.PI * 1.25f;
        }
        for(float theta = startAngle; theta < endAngle; theta += .1f)
        {
            RaycastHit2D ray = Physics2D.Raycast(transform.position, new Vector2(Mathf.Cos(theta),Mathf.Sin(theta)), sightRange, shootScanLayerMask.value);
            if (!ray) continue;//if the ray didn't colide with anything in the shootScan layers
            if (ray.collider.gameObject.tag.Equals("Panda"))//instead of platform
                return true;
        }
        
        return false;
    }

    //method: Fire
    //purpose: fires a bullet in the current firing direction
    void Fire()
    {
		source.PlayOneShot (gunShotSound);
        GameObject newBullet = (GameObject)Instantiate(Resources.Load("Bullet"));
        newBullet.transform.position = this.transform.position;
        newBullet.GetComponent<Rigidbody2D>().velocity = firingDirection * bulletSpeed;
        newBullet.transform.eulerAngles = gun.transform.eulerAngles;
    }
    //gun = (GameObject) Instantiate(Resources.Load("Gun"), new Vector2(1, 1), new Quaternion());
    //gun.transform.parent = this.transform;

    //GameObject newBullet = Instantiate(bullet);
    

    //method: MoveOrIdle
    //purpose: determines whether the hunter should move or not
    void MoveOrIdle()
    {
        if (moveOrIdleTime > 0)
        {
            moveOrIdleTime -= Time.deltaTime;
        }else
        {
            state = Random.value < 0.5 ? 0 : 1;
            if (state == 0)
                moveOrIdleTime = idleTime;
            else
                moveOrIdleTime = moveTime;
        }

        if (state == 0)
        {
            Idle();
        }else
        {
            Move();
        }
    }

    //method: Move
    //purpose: makes the hunter move and turn around at the edges
    void Move()
    {

        bool leftCollide = LeftSideOnGround();
        bool centerCollide = CenterSideOnGround();
        bool rightCollide = RightSideOnGround();
        bool grounded = leftCollide || centerCollide || rightCollide;
        bool obstructionLeft = ObstructionOnLeft();
        bool obstructionRight = ObstructionOnRight();

        if (grounded)
        {
            if (!leftCollide)
                MoveRight();
            else if (!rightCollide)
                MoveLeft();
            else
                MoveForward();

            if (obstructionLeft)
                MoveRight();
            else if (obstructionRight)
                MoveLeft();

        }else
        {
            MoveForward();
        }
        
    }

    //method: Idle
    //purpose: makes the hunter's gun face the right way when idle
    void Idle()
    {
        if (facingRight)
        {
            gun.GetComponent<SpriteRenderer>().flipY = false;
            gun.transform.eulerAngles = rightAimingDirection;
        }else
        {
            gun.GetComponent<SpriteRenderer>().flipY = true;
            gun.transform.eulerAngles = leftAimingDirection;
        }
            
    }

    //method: MoveRight
    //purpose: moves the hunter right
    void MoveRight()
    {
        FaceRight();
        this.transform.position += this.transform.right * walkSpeed * Time.deltaTime;
    }

    //method: MoveLeft
    //purpose: moves the hunter left
    void MoveLeft()
    {
        FaceLeft();
        this.transform.position -= this.transform.right * walkSpeed * Time.deltaTime;
    }

    //method: MoveForward
    //purpose: moves the hunter in whatever direction it is facing
    void MoveForward()
    {
        if (facingRight)
            MoveRight();
        else
            MoveLeft();
    }

    //method: FaceRight
    //purpose: makes the hunter face right
    void FaceRight()
    {
        facingRight = true;
        spriteRenderer.flipX = false;
        gun.GetComponent<SpriteRenderer>().flipY = false;
        gun.transform.eulerAngles = rightAimingDirection;
    }

    //method: FaceLeft
    //purpose: makes the hunter face left
    void FaceLeft()
    {
        facingRight = false;
        spriteRenderer.flipX = true;
        gun.GetComponent<SpriteRenderer>().flipY = true;
        gun.transform.eulerAngles = leftAimingDirection;
    }

    //method: LeftSideOnGround
    //purpose: tests if the left corner of the sprite is on the ground
    bool LeftSideOnGround()
    {
        RaycastHit2D left = Physics2D.Raycast(transform.position + new Vector3(-leftDistanceFromCenter, -verticalDistanceFromCenter, 0), Vector2.down, 0.1f, platformLayerMask);
        return (left && left.collider.gameObject.tag.Equals("Platform"));
    }
    //method: CenterSideOnGround
    //purpose: tests if the bottom center of the sprite is on the ground
    bool CenterSideOnGround()
    {
        RaycastHit2D center = Physics2D.Raycast(transform.position + new Vector3(0, -verticalDistanceFromCenter, 0), Vector2.down, 0.1f, platformLayerMask);
        return (center && center.collider.gameObject.tag.Equals("Platform"));
    }
    //method: RightSideOnGround
    //purpose: tests if the right corner of the sprite is on the ground
    bool RightSideOnGround()
    {
        RaycastHit2D right = Physics2D.Raycast(transform.position + new Vector3(rightDistanceFromCenter, -verticalDistanceFromCenter, 0), Vector2.down, 0.1f, platformLayerMask);
        return (right && right.collider.gameObject.tag.Equals("Platform"));
    }

    //method: ObstructionOnLeft
    //purpose: tests if there is something in the way on the left side
    bool ObstructionOnLeft()
    {
        RaycastHit2D center = Physics2D.Raycast(transform.position + new Vector3(-leftDistanceFromCenter - .1f, verticalDistanceFromCenter, 0), Vector2.down, verticalDistanceFromCenter*1.8f, movementObstructionLayerMask);
        return center;
    }

    //method: ObstructionOnRight
    //purpose: tests if there is something in the way on the right side
    bool ObstructionOnRight()
    {
        RaycastHit2D center = Physics2D.Raycast(transform.position + new Vector3(rightDistanceFromCenter + .1f, verticalDistanceFromCenter, 0), Vector2.down, verticalDistanceFromCenter * 1.8f, movementObstructionLayerMask);
        return center;
    }


    //method: OnTriggerEnter2D
    //purpose: describes what to do when the hunter enters a collider
    //current: just reacts to farts
    void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.gameObject.tag == "GasCloud")
        {
            spriteRenderer.color = hurtColor;

            Hurt(collider.gameObject.GetComponent<GasCloud>().getDamage());
        }
    }

    //method: Hurt
    //purpose: gives damage to the hunter
    //input: >0
    public void Hurt(int ammountAttempted)
    {
        spriteRenderer.color = hurtColor;
        hurtFlashTime = maxHurtFlashTime;
        if (health > 0)
        {
            if (health - ammountAttempted <= 0)
            {
                health = 0;
                score.IncreaseScore(100);
                print("hunter death");
                state = 3;//death
                Destroy(gun);
                Destroy(this.gameObject, deathTime);
            }
            else
            {
                health -= ammountAttempted;
            }
            print("hunter health: " + health);
        }
    }

    //method: KillIfOutOfBounds
    //purpose: kills hunter if it falls off the stage
    public void KillIfOutOfBounds()
    {
        if (transform.position.y < -14)
            Hurt(1000);
    }
}
