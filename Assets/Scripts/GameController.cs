﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;
using TMPro;

public class GameController : MonoBehaviour
{
    public List<PooledPrefab> pooledPrefabs;

    #region Singleton

    public static GameController Instance;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(Instance);
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    #endregion

    #region Death Callback Handling

    private static List<DeathCallback> deathCalls = new List<DeathCallback>();

    public static bool RegisterDeathCallback(DeathCallback newCall)
    {
        if (!deathCalls.Any(x => GameObject.ReferenceEquals(x.refObj, newCall.refObj)))
        {
            deathCalls.Add(newCall);
            return true;
        }

        return false;
    }

    public static bool UnregisterDeathCallback(GameObject refObj)
    {
        var existingCallback = deathCalls.Where(x => GameObject.ReferenceEquals(x.refObj, refObj)).FirstOrDefault();

        if (existingCallback != null)
        {
            deathCalls.Remove(existingCallback);
            return true;
        }

        return false;
    }

    public static void OnDeathCallback(float damageAmount, ElementType elementType, string damageDealer, GameObject obj, GameObject dealerObj)
    {
        bool IsCallRelevant(DeathCallback call, GameObject causeOfDeath)
        {
            if (call.mode == CallbackMode.Tag)
            {
                return (string.IsNullOrEmpty(call.targetTag) || obj.CompareTag(call.targetTag)) && call.refObj && call.refObj.activeSelf;
            }
            else
            {
                return GameObject.ReferenceEquals(call.targetObj, obj) && call.refObj && call.refObj.activeSelf;
            }
        }

        var relevantCalls = deathCalls.Where(x => IsCallRelevant(x, dealerObj)).ToArray();

        for (int i = 0; i < relevantCalls.Count(); i++)
        {
            relevantCalls[i].callback(damageAmount, elementType, damageDealer, obj, dealerObj);
        }
    }

    public class DeathCallback
    {
        public CallbackMode mode;
        public GameObject targetObj;
        public string targetTag;
        public Action<float, ElementType, string, GameObject, GameObject> callback;
        public GameObject refObj;
    }

    public enum CallbackMode
    {
        Tag,
        Object
    }

    #endregion

    #region Prefab Pooling

    private Dictionary<string, Queue<GameObject>> poolDict;

    [Serializable]
    public struct PooledPrefab
    {
        public GameObject prefab;
        public string tag;
        public int poolSize;
    }

    private void Start()
    {
        poolDict = new Dictionary<string, Queue<GameObject>>();

        foreach (PooledPrefab pool in pooledPrefabs)
        {
            Queue<GameObject> objPool = new Queue<GameObject>();

            for (int i = 0; i < pool.poolSize; i++)
            {
                GameObject obj = Instantiate(pool.prefab);
                obj.SetActive(false);
                objPool.Enqueue(obj);
            }

            poolDict.Add(pool.tag, objPool);
        }
    }

    public GameObject SpawnPooledPrefab(string tag, Vector3 position, Quaternion rotation, bool initialize = true)
    {
        if (!poolDict.ContainsKey(tag))
        {
            Debug.LogWarning("Prefab pool " + tag + " does not exist.");
            return null;
        }

        GameObject toSpawn = poolDict[tag].Dequeue();

        toSpawn.SetActive(true);
        toSpawn.transform.position = position;
        toSpawn.transform.rotation = rotation;

        IPooledObject pooledObj = toSpawn.GetComponent<IPooledObject>();

        if (pooledObj != null && initialize)
        {
            pooledObj.OnSpawned();
        }

        poolDict[tag].Enqueue(toSpawn);

        return toSpawn;
    }

    #endregion
}
