using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Entity : DamageableObject
{
    public GameObject spawnOrigin;

    protected Rigidbody2D rb;

    private void Awake()
    {
        isEntity = true;
        rb = GetComponent<Rigidbody2D>();
    }

    /// <summary>
    /// Move the entity for 1 frame at a certain velocity. Should be called from FixedUpdate.
    /// </summary>
    /// <param name="velocity"></param>
    protected void TryMove(Vector2 velocity)
    {
        rb.MovePosition(rb.position + velocity * Time.fixedDeltaTime);
    }
}
