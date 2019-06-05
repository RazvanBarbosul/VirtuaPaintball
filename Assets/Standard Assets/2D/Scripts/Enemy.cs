using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Enemy : MonoBehaviour {
    public SurvivalPlayer Player;
    public int JumpForce;
   
    [SerializeField]
    private float MaxSpeed = 10f;

    [SerializeField]
    private UnityStandardAssets._2D.SurvivalPlatformerCharacter2D Controller;
    
    public bool Direction;
    [SerializeField]
    private SurvivalWeapon Weapon;
    [SerializeField]
    private Image HealthBar;
    
    private float EnemyStartHealth;
    private float EnemyCurrentHealth;
    private bool wasDamaged;
    private int JumpChance;
    private bool IsJumping;
    private DifficultyManager DifficultyManager;
    // Use this for initialization
    void Start () {

		if(Player == null)
        {
            Player = FindObjectOfType<SurvivalPlayer>();
        }
        Controller.m_Anim.SetFloat("Speed", 0.2f);
        if (Player.transform.position.x > gameObject.transform.position.x)
        {
            Direction = true;
        }
        else
        {
            Direction = false;
            Controller.Flip();
        }
        EnemyStartHealth = 100;
        EnemyCurrentHealth = EnemyStartHealth;
        wasDamaged = false;
       // JumpForce = 35;
        IsJumping = false;
        DifficultyManager = FindObjectOfType<DifficultyManager>();
    }
    public void Damage(int amount)
    {
        EnemyCurrentHealth -= amount;
        HealthBar.fillAmount = EnemyCurrentHealth / EnemyStartHealth;
        Debug.Log("Fill amount: " + EnemyCurrentHealth / EnemyStartHealth);
        wasDamaged = false;
        if (EnemyCurrentHealth <= 0)
        {
            GameMaster.KillEnemy(this);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Bullet" && !wasDamaged)
        {
            wasDamaged = true;
            Destroy(other.gameObject);
            Debug.Log("Enemy Damaged");

            StartCoroutine(DealDamage());
           // Damage(5);

            
        }
        if ((other.tag == "Wall" || other.tag == "Ceiling") && !IsJumping)
        {
            IsJumping = true;
            Controller.m_Rigidbody2D.AddForce(new Vector2(0f, JumpForce));
            StartCoroutine(StartJump());
            Debug.Log("Hiting wall with face");
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if ((other.tag == "Wall" || other.tag == "Ceiling") && !IsJumping)
        {
            IsJumping = true;
            Controller.m_Rigidbody2D.AddForce(new Vector2(0f, JumpForce));
            StartCoroutine(StartJump());
            Debug.Log("Hiting wall with face");
        }
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        if ((other.tag == "Wall" || other.tag == "Ceiling") && !IsJumping)
        {
            IsJumping = true;
            Controller.m_Rigidbody2D.AddForce(new Vector2(0f, JumpForce));
            StartCoroutine(StartJump());
            Debug.Log("Hiting wall with face");
        }
    }
    IEnumerator DealDamage()
    {
        yield return new WaitForSeconds(0.1f);
        Damage(Player.playerWeaponDamage);
    }

    void Jump()
    {
        JumpChance = 1;
        IsJumping = true;
        Controller.m_Rigidbody2D.AddForce(new Vector2(0f, JumpForce));

        StartCoroutine(StartJump());
        // Vector2 Jmp = new Vector2(Player.transform.position.x, Player.transform.position.y + JumpForce);
        // gameObject.transform.position = Vector2.MoveTowards(gameObject.transform.position, Jmp, MaxSpeed * 3);

        // IsJumping = false;
    }
    // Update is called once per frame
    void Update ()
    {
        if(Player != null )
        {
            // Debug.Log("Enemy pos: " + gameObject.transform.position + " player pos: " + Player.transform.position + " dir: "+ Direction);
            gameObject.transform.position = Vector2.MoveTowards(gameObject.transform.position, Player.transform.position, MaxSpeed);
        }
        else
        {
            Player = FindObjectOfType<SurvivalPlayer>();
          
            
        }
        if (!IsJumping)
        {
            
            JumpChance = Random.Range(0, 20);
        }
        else if(JumpChance == 0)
        {
            
                
               // Jump();
           
            // Vector2 Test = new Vector2(Player.transform.position.x, Player.transform.position.y + JumpForce);
          //  gameObject.transform.position = Vector2.MoveTowards(gameObject.transform.position, new Vector2(Player.transform.position.x, Player.transform.position.y + JumpForce), 0.2f);
            //gameObject.transform.position = new Vector2(gameObject.transform.position.x, Mathf.Lerp(gameObject.transform.position.y, JumpForce, Time.deltaTime));
           
        }

        
        if (JumpChance == 0)
        {
            // IsJumping = true;
          //  Jump();
        }
        // If the input is moving the player right and the player is facing left...
        if (Player.transform.position.x > gameObject.transform.position.x && !Direction && Player!= null)
        {
            // ... flip the player.
            Controller.Flip();
            Direction = !Direction;
        }
        // Otherwise if the input is moving the player left and the player is facing right...
        else if (Player.transform.position.x < gameObject.transform.position.x && Direction && Player !=null)
        {
            // ... flip the player.
            Controller.Flip();
            Direction = !Direction;
        }

        //AIShoot();
        StartCoroutine(FireGun());
    }
    IEnumerator StartJump()
    {
        Debug.Log("Should jump");
       // IsJumping = true;
        //Jump();
        yield return new WaitForSeconds(2);
        IsJumping = false;
    }


    void AIShoot()
    {
        
    }
    IEnumerator FireGun()
    {
        yield return new WaitForSeconds(1f);
        if (Weapon.fireRate == 0)
        {

            Weapon.AIShoot();

        }
        else if (Time.time > Weapon.TimeToFire)
        {
            Weapon.TimeToFire = Time.time + Weapon.fireRate;
            Weapon.AIShoot();
            
        }
    }
}
