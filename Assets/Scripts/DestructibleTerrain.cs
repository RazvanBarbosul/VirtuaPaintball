using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestructibleTerrain : MonoBehaviour {

    public int health;
    public GameObject destroyEffect;
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		if(health <= 0)
        {
            Instantiate(destroyEffect, transform.position, Quaternion.identity);
            Destroy(destroyEffect);
            Destroy(gameObject);
        }

	}
}
