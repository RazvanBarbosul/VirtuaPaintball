using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour {
    [SerializeField]
    private GameMaster GM;
    [SerializeField]
    private Transform SpawnLocation;
    private DifficultyManager DifficultyManager;
	// Use this for initialization
	void Start () {

        if (GM == null)
        {
            GM = GameObject.FindGameObjectWithTag("GM").GetComponent<GameMaster>();
        }
        DifficultyManager = FindObjectOfType<DifficultyManager>();
        int SpawnChance = Random.Range(0, 11 - DifficultyManager.SurvivalDifficulty * 2);
        if(SpawnChance == 0)
        {
            GM.SpawnEnemy(SpawnLocation.position, SpawnLocation.rotation);
        }
    }
	
	// Update is called once per frame
	void Update () {
		
	}
}
