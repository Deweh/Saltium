using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RespawnPoint : MonoBehaviour
{
    public float startRespawnTime = 5;
    public GameController.CallbackMode mode;
    public GameObject prefabToSpawn;
    public string targetTag;
    public GameObject targetObject;
    public bool preserveHealthBar = true;

    private GameObject[] spawners;
    private float respawnTime;
    private bool targetDead = false;
    private HealthBar healthBar;

    void Start()
    {
        respawnTime = startRespawnTime;
        GameController.RegisterDeathCallback(new GameController.DeathCallback()
        {
            mode = mode,
            targetObj = targetObject,
            targetTag = targetTag,
            callback = OnDeath,
            refObj = gameObject
        });
    }

    private void OnDestroy()
    {
        GameController.UnregisterDeathCallback(gameObject);
    }

    private void FixedUpdate()
    {
        if (targetDead)
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

                GameObject newInst = Instantiate(prefabToSpawn, transform.position, Quaternion.identity);
                if (preserveHealthBar)
                {
                    newInst.GetComponent<DamageableObject>().healthBar = healthBar;
                }
                
                targetDead = false;
            }
        }
    }

    void OnDeath(float damageAmount, ElementType elementType, string damageDealer, GameObject obj, GameObject dealerObj)
    {
        respawnTime = startRespawnTime;

        if (preserveHealthBar)
        {
            healthBar = obj.GetComponent<DamageableObject>().healthBar;
        }

        spawners = GameObject.FindGameObjectsWithTag("Spawner");
        foreach (GameObject spawn in spawners)
        {
            spawn.SetActive(false);
        }

        targetDead = true;
    }
}