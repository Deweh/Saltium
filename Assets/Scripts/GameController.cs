using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

public class GameController : MonoBehaviour
{
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
}
