using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour {
    public float fireRate = 5f;
    public float Damage = 10;
    public LayerMask whatToHit;
    public GameObject bullet;

    public float TimeToFire = 0;
    Transform firePoint;
    [SerializeField]
    UnityStandardAssets._2D.PlatformerCharacter2D Player;
    [SerializeField]
    private Enemy Enemy;
    void Awake()
    {
        firePoint = transform.Find("FirePoint");
        if (firePoint == null)
        {
            Debug.LogError("No firepoint");
        }
    }
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		//if (fireRate == 0)
  //      {
  //          if (Input.GetButtonDown("Fire1"))
  //          {
  //              Shoot();
  //          }
  //      }
  //      else if(Input.GetButton ("Fire1") && Time.time > TimeToFire)
  //      {
  //          TimeToFire = Time.time + 1 / fireRate;
  //          Shoot();
  //      }
	}

    public void Shoot()
    {
        Vector2 mousePosition = new Vector2(Camera.main.ScreenToWorldPoint(Input.mousePosition).x, Camera.main.ScreenToWorldPoint(Input.mousePosition).y);
        Vector2 firePointPosition = new Vector2(firePoint.position.x, firePoint.position.y);
     
        GameObject project = Instantiate(bullet, firePointPosition, firePoint.rotation);
        Rigidbody2D rb = project.GetComponent<Rigidbody2D>();


        // rb.AddForce(project.transform.right * 45, ForceMode.VelocityChange);
        if (Player.Direction)
        {
            rb.velocity = project.transform.right * 30;
        }
        else
        {
            rb.velocity = project.transform.right * -30;
        }
      //add sound


    }
     public void AIShoot()
    {
         
        Vector2 firePointPosition = new Vector2(firePoint.position.x, firePoint.position.y);

        GameObject project = Instantiate(bullet, firePointPosition, firePoint.rotation);
        Rigidbody2D rb = project.GetComponent<Rigidbody2D>();


        // rb.AddForce(project.transform.right * 45, ForceMode.VelocityChange);
        if (Enemy.Direction)
        {
            rb.velocity = project.transform.right * 30;
        }
        else
        {
            rb.velocity = project.transform.right * -30;
        }

        //add sound


    }


}
