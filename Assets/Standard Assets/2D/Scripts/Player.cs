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
        playerWeaponDamage = 10;// * (6 - DifficultyManager.SurvivalDifficulty);
        owner = Network.player;
        scoreText.enabled = false;
        int ran = (int)Random.Range(0, 999);
        PlayerName = "Player" + ran.ToString();
        // PlayerNameText.SetText(PlayerName);

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
            CmdDamage(10, other.gameObject);//* DifficultyManager.SurvivalDifficulty);
            Destroy(other.gameObject);
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
            PlayerHP = 0;
            if(proj)
            {
               // proj.GetComponent<Bullet>().player.playerScore++;
               // proj.GetComponent<Bullet>().player.playerScore++;
              //  proj.GetComponent<Bullet>().player.CmdChangeScore(1);
                //proj.GetComponent<Bullet>().player.scoreText.SetText(proj.GetComponent<Bullet>().player.playerScore.ToString());
            }
            PlayerHP = 100;
            RpcRespawn();
        }
    }

    [Command]
    void CmdChangeScore(int newScore, GameObject Player)
    {
        if(!isServer)
        {
            return;
        }
        Player.GetComponent<Bullet>().player.playerScore++;
        if(Player.GetComponent<Bullet>().player.playerScore == 1)
        {
            //RpcEndGame();
            // CmdStartOver();
           // CallEnd();
        }
        Player.GetComponent<Bullet>().player.scoreText.SetText(Player.GetComponent<Bullet>().player.playerScore.ToString());
    }

    [ClientRpc]
    void RpcRespawn()
    {
        if(isLocalPlayer)
        {
            int ran = (int)Random.Range(0, GM.spawnPoint.Length);
            transform.position = GM.spawnPoint[ran].position;
            PlayerHP = 100;
        }
    }

    void CallEnd()
    {
        CmdStartOver();
    }

    [ClientRpc]
    void RpcEndGame()
    {
        if (isLocalPlayer)
        {
            endGameText.enabled = true;
            if (playerScore == 1)
            {
                endGameText.SetText("Victory!");
            }
            else
            {
                endGameText.SetText("Defeat!");
            }
            Debug.LogError("ENDGAME");
            StartCoroutine(RestartGame());
           // playerScore = 0;
          //  endGameText.enabled = false;
           // RpcRespawn();
        }
        
    }

    [Command]
    void CmdStartOver()
    {
        if (!isServer)
        {
            return;
        }
        // RpcRespawn();
        RpcEndGame();
        
    }

    public IEnumerator RestartGame()
    {
        yield return new WaitForSeconds(10);

        playerScore = 0;
        endGameText.enabled = false;
        RpcRespawn();
       // CmdStartOver();
    }

    void CmdOnChangeHealth(float Health)
    {
        HealthBar.fillAmount = Health / 100;
    }

    void CmdOnScoreChanged(int score)
    {
        scoreText.SetText(score.ToString());
      if(score == 1)
        {
            CallEnd();
        }
    }

    void FindTMPro()
    {
        if (nextTimeToSearch <= Time.time)
        {
            TextMeshProUGUI searchResult = TextMeshProUGUI.FindObjectOfType<TextMeshProUGUI>(); //GameObject.FindGameObjectWithTag("TMPro");



            if (searchResult != null)
            {
              //  healthText = searchResult;
               // healthText.text = playerStats.playerHealth.ToString();
                nextTimeToSearch = Time.time + 0.5f;
                //Debug.Log("GG!");
            }
        }
    }
}
