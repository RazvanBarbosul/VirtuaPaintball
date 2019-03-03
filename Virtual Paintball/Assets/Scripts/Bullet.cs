using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour {

    public GameObject bullet;
    public Player player;
    
	// Use this for initialization
	void Start () {
        Destroy(gameObject, 10);
    }
	
	// Update is called once per frame
	void Update () {
		
	}

    
    
    private void OnTriggerEnter2D(Collider2D other)
    {
       
      
        if (other.tag == "Ground" )
        {
            Destroy(gameObject);
        }
    }
}
