using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : DamageableObject
{
    Rigidbody2D rb;
    public float speed = 20f;
    public float startLifetime = 5f;
    public bool dieOnAnyCollision = true;

    private float lifetime;
    protected bool noImpacts = false;

    void Reset()
    {
        maxHealth = 1f;
    }

    protected float GetLifetime()
    {
        return lifetime;
    }

    protected override void Start()
    {
        base.Start();

        rb = GetComponent<Rigidbody2D>();
        lifetime = startLifetime;
        rb.velocity = transform.rotation * Vector3.up * speed;
    }

    protected virtual void FixedUpdate()
    {
        lifetime -= Time.fixedDeltaTime;

        if (lifetime <= 0f)
        {
            Kill();

            if (gameObject)
            {
                Destroy(gameObject);
            }
        }
    }

    protected override void OnCollisionEnter2D(Collision2D collision)
    {
        base.OnCollisionEnter2D(collision);

        if (noImpacts) return;

        var obj = collision.gameObject.GetComponent<DamageableObject>();

        if (!dieOnAnyCollision && obj && !obj.IsEntity())
        {
            return;
        }

        Kill();
    }

    protected override void OnTriggerEnter2D(Collider2D collision)
    {
        base.OnTriggerEnter2D(collision);

        if (noImpacts) return;

        var obj = collision.gameObject.GetComponent<DamageableObject>();

        if (!dieOnAnyCollision && obj && !obj.IsEntity())
        {
            return;
        }

        Kill();
    }
}
