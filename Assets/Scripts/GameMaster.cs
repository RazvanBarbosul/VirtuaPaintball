using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameMaster : MonoBehaviour {
    public static GameMaster gm;
    public int DifficultyLevel;
    public Text PlayerScore;
    private int ScoreValue;
    private DifficultyManager DifficultyManager;
    private void Start()
    {
        DifficultyManager = FindObjectOfType<DifficultyManager>();
        if (gm == null)
        {
            gm = GameObject.FindGameObjectWithTag("GM").GetComponent<GameMaster>();
        }
        ScoreValue = 0;
        PlayerScore.text = ScoreValue.ToString();
    }

    public Transform playerPrefab;
    public GameObject enemyPrefab;
    public Transform spawnPoint;
    public int spawnDelay = 2;

    public IEnumerator RespawnPlayer()
    {
        yield return new WaitForSeconds(spawnDelay);
        Instantiate(playerPrefab, spawnPoint.position, spawnPoint.rotation);
    }
    public static void KillPlayer(Player player)
    {
        Destroy(player.gameObject);
        gm.StartCoroutine(gm.RespawnPlayer());
    }

    public void SpawnEnemy(Vector3 position, Quaternion rotation)
    {
        Instantiate(enemyPrefab, position, rotation);
    }

    public static void KillEnemy(Enemy enemy)
    {
        Destroy(enemy.gameObject);
        gm.ScoreValue += 1 * gm.DifficultyManager.SurvivalDifficulty;
        gm.PlayerScore.text = gm.ScoreValue.ToString();
        //TODO: play effect
    }
}
