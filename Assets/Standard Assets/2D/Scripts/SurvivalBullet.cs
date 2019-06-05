using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SurvivalBullet : MonoBehaviour
{

    public GameObject bullet;
    public SurvivalPlayer player;
    public float areaOfEffect;
    public int damage;

    // Use this for initialization
    void Start()
    {
        // Destroy(gameObject, 10);
    }

    private void Awake()
    {
        damage = 1;
    }
    // Update is called once per frame
    void Update()
    {

    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, areaOfEffect);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Terrain"))
        {
            Collider2D[] objectsToDamage = Physics2D.OverlapCircleAll(gameObject.transform.position, areaOfEffect);
            for (int i = 0; i < objectsToDamage.Length; i++)
            {
                if (objectsToDamage[i].gameObject.CompareTag("Terrain"))
                {
                    objectsToDamage[i].GetComponent<DestructibleTerrain>().health -= damage;
                }
            }
            Destroy(gameObject);
        }
        else
        if (other.tag == "Ground" || other.tag == "Wall" || other.tag == "Ceiling")
        {
            Destroy(gameObject);
        }
    }
}