using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using TMPro;

public class Player : NetworkBehaviour
{
    public TextMeshProUGUI healthText;
    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI endGameText;
    public Player player;
    float nextTimeToSearch = 0;
    public Transform EnemyAimLocation;
    [SerializeField]
    private Weapon Weapon;
    private DifficultyManager DifficultyManager;
    public NetworkManager NetworkManager;
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
    [SyncVar(hook = "CmdOnScoreChanged")]
    public int playerScore = 0;
    public string PlayerName;
    public HealthPickup healthPickup;
    public Rigidbody2D m_Rigidbody2D;

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
      //  scoreText.SetText(playerScore.ToString());
        if (owner != null && Network.player == owner && isLocalPlayer)
        {
            //Only the client that owns this object executes this code
            if (myCam.enabled == false)
                myCam.enabled = true;
            if (CameraScript.target == null)
            {
               CameraScript.FindPlayer(this.gameObject);
            }

            if (scoreText.enabled == false)
                scoreText.enabled = true;
           
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
        //healthText.text = player.playerStats.playerHealth.ToString();

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
    }

    [Command]
    public void CmdSpawnHealthPickup()
    {
        int ran = (int)Random.Range(0, 1);
        if(ran == 0)
        {
            GameObject Pickup = Instantiate(healthPickup.gameObject, player.transform.position, player.transform.rotation);
            NetworkServer.Spawn(Pickup);
            Destroy(Pickup, 10);
        }
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
        NetworkManager = FindObjectOfType<NetworkManager>();
        playerWeaponDamage = 10;
        owner = Network.player;
        scoreText.enabled = false;
        int ran = (int)Random.Range(0, 999);
        PlayerName = "Player" + ran.ToString();

        RpcRespawn();
        Debug.Log(PlayerName);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Bullet")
        {
           
            if(PlayerHP - 10 <= 0)
            {
                CmdChangeScore(1, other.gameObject);
            }
            CmdDamage(10, other.gameObject);            Destroy(other.gameObject);
        }

        if (other.tag == "HealthPickup")
        {
            CmdDamage(-25, other.gameObject);
            Destroy(other.gameObject);
        }
        if(other.tag == "Trampolie")
        {
            Debug.LogError("Should jump");
           m_Rigidbody2D.AddForce(new Vector2(0f, 3500));
        }
        if(other.tag == "SpikeBall")
        {
            CmdChangeScore(-1, other.gameObject);
        }
    }

    [Command]
    public void CmdDamage(int amount, GameObject proj)
    {
        if(!isServer)
        {
            return;
        }
        PlayerHP -= amount;
        if (PlayerHP <= 0)
        {
            PlayerHP = 100;
            RpcRespawn();
        }
        if(PlayerHP > 100)
        {
            PlayerHP = 100;
        }
    }

    [Command]
    void CmdChangeScore(int newScore, GameObject Player)
    {
        if(!isServer)
        {
            return;
        }
        if (Player.GetComponent<Bullet>())
        {
            Player.GetComponent<Bullet>().player.playerScore += newScore;
        }
        else
        {
            this.playerScore += newScore;
        }

        Player.GetComponent<Bullet>().player.scoreText.SetText(Player.GetComponent<Bullet>().player.playerScore.ToString());
    }

    [ClientRpc]
    void RpcRespawn()
    {
        if(isLocalPlayer)
        {
            CmdSpawnHealthPickup();
            int ran = (int)Random.Range(0, GM.spawnPoint.Length);
            transform.position = GM.spawnPoint[ran].position;
            PlayerHP = 100;
        }
    }

    void CallEnd(GameObject winner)
    {
        CmdStartOver(winner);
    }

    [ClientRpc]
    void RpcEndGame(GameObject winner)
    {
        if (isLocalPlayer)
        {
            endGameText.enabled = true;
            if (this.gameObject == winner)
            {
                endGameText.SetText("Victory!");
            }
            else
            {
                endGameText.SetText("Defeat!");
            }
            StartCoroutine(RestartGame());
        }
        
    }

    [Command]
    void CmdStartOver(GameObject winner)
    {
        if (!isServer)
        {
            return;
        }
        RpcEndGame(winner);
        
    }

    public IEnumerator RestartGame()
    {
        yield return new WaitForSeconds(10);

        playerScore = 0;
        endGameText.enabled = false;
        RpcRespawn();
    }

    void CmdOnChangeHealth(float Health)
    {
        HealthBar.fillAmount = Health / 100;
    }

    void CmdOnScoreChanged(int score)
    {
        scoreText.SetText(score.ToString());
      if(score == 5)
        {
            CallEnd(this.gameObject);
        }
    }

    void FindTMPro()
    {
        if (nextTimeToSearch <= Time.time)
        {
            TextMeshProUGUI searchResult = TextMeshProUGUI.FindObjectOfType<TextMeshProUGUI>();

            if (searchResult != null)
            {
              ;
                nextTimeToSearch = Time.time + 0.5f;
            }
        }
    }
}
