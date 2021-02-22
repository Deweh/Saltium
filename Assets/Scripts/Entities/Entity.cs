using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Entity : DamageableObject
{
    public GameObject spawnOrigin;

    private void Awake()
    {
        isEntity = true;
    }
}
