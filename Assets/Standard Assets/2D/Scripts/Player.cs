using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Player : MonoBehaviour {
    public TextMeshProUGUI healthText;
    public Player player;
    float nextTimeToSearch = 0;
    public Transform EnemyAimLocation;
    [SerializeField]
    private Weapon Weapon;
    private DifficultyManager DifficultyManager;
    public int playerWeaponDamage;

    [System.Serializable]
public class PlayerStats
    {
        public int playerHealth = 100;
        
    }

    public PlayerStats playerStats = new PlayerStats();

    private void Update()
    {
        
    }

    public void PlayerUpdate()
    {
        FindTMPro();
        if (transform.position.y <= -20)
        {
            Damage(100);
        }
        healthText.text = player.playerStats.playerHealth.ToString();

        if (Weapon.fireRate == 0)
        {
            if (Input.GetButtonDown("Fire1"))
            {
                Weapon.Shoot();
            }
        }
        else if (Input.GetButton("Fire1") && Time.time > Weapon.TimeToFire)
        {
            Weapon.TimeToFire = Time.time + 1 / Weapon.fireRate;
            Weapon.Shoot();
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

        DifficultyManager = FindObjectOfType<DifficultyManager>();
        playerWeaponDamage = 10 * (6 - DifficultyManager.SurvivalDifficulty);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "EnemyBullet")
        {
            Destroy(other.gameObject);
            Damage(10 * DifficultyManager.SurvivalDifficulty);
        }
    }

    public void Damage(int amount)
    {
        playerStats.playerHealth -= amount;
        if (playerStats.playerHealth <= 0)
        {
            GameMaster.KillPlayer(this);
        }
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
