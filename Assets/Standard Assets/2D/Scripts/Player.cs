using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using TMPro;

public class Player : NetworkBehaviour
{
    public TextMeshProUGUI healthText;
    public Player player;
    float nextTimeToSearch = 0;
    public Transform EnemyAimLocation;
    [SerializeField]
    private Weapon Weapon;
    private DifficultyManager DifficultyManager;
    public int playerWeaponDamage;
    [SerializeField]
    private Image HealthBar;
    [SerializeField]
    private Camera PlayerCamera;
    [SerializeField]
    Transform firePoint;
    public GameObject bullet;
    [SerializeField]
    UnityStandardAssets._2D.PlatformerCharacter2D PlayerChar;
    [SerializeField]
    public GameMaster GM;
    public Camera myCam;
    public NetworkPlayer owner;
    [SerializeField]
    int playerScore = 0;

    public UnityStandardAssets._2D.Camera2DFollow CameraScript;

    [System.Serializable]
public class PlayerStats
    {
        public int playerHealth = 100;
        
    }
    [SyncVar(hook = "CmdOnChangeHealth")]
    public float PlayerHP = 100;
    public PlayerStats playerStats = new PlayerStats();

    private void Update()
    {
        PlayerUpdate();
        //if (isLocalPlayer)
        //{
        //    if(CameraScript.target == null)
        //    {
        //        CameraScript.FindPlayer(this.gameObject);

        //    }
        //}

        if (owner != null && Network.player == owner && isLocalPlayer)
        {
            //Only the client that owns this object executes this code
            if (myCam.enabled == false)
                myCam.enabled = true;
            if (CameraScript.target == null)
           {
               CameraScript.FindPlayer(this.gameObject);

                   }
        }

       // Debug.Log("Player velocity: " + player.GetComponent<Rigidbody2D>().velocity);
    }

    public void PlayerUpdate()
    {
        FindTMPro();
        if (transform.position.y <= -20)
        {
            CmdDamage(100, this.gameObject);
        }
        healthText.text = player.playerStats.playerHealth.ToString();

        if (Weapon.fireRate == 0)
        {
            if (Input.GetButtonDown("Fire1"))
            {
                //if (!isServer)
                //{
                //    return;
                //}
                if (!isLocalPlayer)
                {
                    return;
                }
                //  Weapon.CmdShoot();
                CmdShoot();
            }
        }
        else if (Input.GetButton("Fire1") && Time.time > Weapon.TimeToFire)
        {
            //if(!isServer)
            //{
            //    return;
            //}
            if (!isLocalPlayer)
            {
                return;
            }
            CmdShoot();
            Weapon.TimeToFire = Time.time + 1 / Weapon.fireRate;
            //Weapon.CmdShoot();

        }
    }

    [Command]
    public void CmdShoot()
    {
        Vector2 mousePosition = new Vector2(PlayerCamera.ScreenToWorldPoint(Input.mousePosition).x, PlayerCamera.ScreenToWorldPoint(Input.mousePosition).y);
        Vector2 firePointPosition = new Vector2(firePoint.position.x, firePoint.position.y);

        GameObject project = Instantiate(bullet, firePointPosition, firePoint.rotation);
        Rigidbody2D rb = project.GetComponent<Rigidbody2D>();


        // rb.AddForce(project.transform.right * 45, ForceMode.VelocityChange);
        if (PlayerChar.Direction)
        {
            rb.velocity = project.transform.right * 30;
        }
        else
        {
            rb.velocity = project.transform.right * -30;
        }

        project.GetComponent<Bullet>().player = this;
        NetworkServer.Spawn(project);
        //add sound


    }

    private void Awake()
    {
       // healthText.text = playerStats.playerHealth.ToString();
       
    }

    private void Start()
    {
        if (player == null)
        {
            player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
        }
        GM = FindObjectOfType<GameMaster>();
        DifficultyManager = FindObjectOfType<DifficultyManager>();
        playerWeaponDamage = 10 * (6 - DifficultyManager.SurvivalDifficulty);
        owner = Network.player;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Bullet")
        {
            Destroy(other.gameObject);
            CmdDamage(10, other.gameObject);//* DifficultyManager.SurvivalDifficulty);
        }
    }

    [Command]
    public void CmdDamage(int amount, GameObject proj)
    {
        if(!isServer)
        {
            return;
        }
        // playerStats.playerHealth -= amount;
        PlayerHP -= amount;
        if (PlayerHP <= 0)
        {
            PlayerHP = 0;
            if(proj)
            {
                proj.GetComponent<Bullet>().player.playerScore++;
                Debug.LogError("Player " + player.name + " score: " + proj.GetComponent<Bullet>().player.playerScore);
            }
            PlayerHP = 100;
            RpcRespawn();
           // GameMaster.KillPlayer(this);
        }
    }

    [ClientRpc]
    void RpcRespawn()
    {
        if(isLocalPlayer)
        {
            //TODO: Add more spawn points and choose a random

            transform.position = GM.spawnPoint.position;
            PlayerHP = 100;
        }
    }


    void CmdOnChangeHealth(float Health)
    {
        HealthBar.fillAmount = Health / 100;
       // healthText.text = PlayerHP.ToString();
        //Debug.Log("Remaining health " + Health);
    }

    void FindTMPro()
    {
        if (nextTimeToSearch <= Time.time)
        {
            TextMeshProUGUI searchResult = TextMeshProUGUI.FindObjectOfType<TextMeshProUGUI>(); //GameObject.FindGameObjectWithTag("TMPro");



            if (searchResult != null)
            {
                healthText = searchResult;
               // healthText.text = playerStats.playerHealth.ToString();
                nextTimeToSearch = Time.time + 0.5f;
                //Debug.Log("GG!");
            }
        }
    }
}
