using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpikeBall : MonoBehaviour {
    public GameObject Chain;
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.tag == "Player")
        {
            StartCoroutine(DamagePlayer(collision.GetComponent<Player>()));
            //collision.GetComponent<Player>().CmdDamage(50);
        }
    }

    public IEnumerator DamagePlayer(Player Player)
    {
        Player.CmdDamage(200, Player.gameObject);
        yield return new WaitForSeconds(3);
    }
}
