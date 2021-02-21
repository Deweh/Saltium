using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RespawnController : MonoBehaviour
{
    public float startRespawnTime = 5;
    public GameObject respawnPlayer;

    private GameObject[] spawners;
    private float respawnTime;
    private bool playerDead = false;
    private HealthBar healthBar;

    void Start()
    {
        respawnTime = startRespawnTime;
    }

    private void FixedUpdate()
    {
        if (playerDead)
        {
            respawnTime -= Time.fixedDeltaTime;

            if (respawnTime <= 0f)
            {
                GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");

                foreach (GameObject enemy in enemies)
                {
                    Destroy(enemy);
                }

                foreach (GameObject spawn in spawners)
                {
                    spawn.SetActive(true);
                }

                GameObject player = Instantiate(respawnPlayer, transform.position, Quaternion.identity);
                player.GetComponent<DamageableObject>().healthBar = healthBar;
                playerDead = false;
            }
        }
    }

    void OnPlayerDeath(Player.PlayerDeathArgs e)
    {
        respawnTime = startRespawnTime;
        spawners = GameObject.FindGameObjectsWithTag("Spawner");
        healthBar = e.player.GetComponent<DamageableObject>().healthBar;

        foreach (GameObject spawn in spawners)
        {
            spawn.SetActive(false);
        }

        playerDead = true;
    }
}
