using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RocketProjectile : Projectile
{
    public float explosionRadius = 2f;
    public float startSafetyFuse = 0.2f;
    public GameObject startTrailEffect;

    private float safetyFuse;
    private CircleCollider2D c;
    private DamageSource source;
    private GameObject trailEffect;

    protected override void Start()
    {
        base.Start();
        safetyFuse = startSafetyFuse;

        c = GetComponent<CircleCollider2D>();
        c.enabled = false;

        deathEffectRadius = explosionRadius;

        source = GetComponent<DamageSource>();
        source.active = false;

        if (startTrailEffect)
        {
            trailEffect = Instantiate(startTrailEffect, transform.position, Quaternion.identity);
        }
    }

    private void Reset()
    {
        maxHealth = 2.5f;
        speed = 3.5f;
        startLifetime = 8f;
    }

    private void Update()
    {
        if (trailEffect)
        {
            trailEffect.transform.position = transform.position;
        }
    }

    protected override void FixedUpdate()
    {
        base.FixedUpdate();

        if (safetyFuse > 0f)
        {
            safetyFuse -= Time.fixedDeltaTime;
        }
        else
        {
            c.enabled = true;
        }
    }

    protected override void OnDeath(float damageAmount, ElementType elementType, string dealer)
    {
        var colliders = Physics2D.OverlapCircleAll(transform.position, explosionRadius);
        source.active = true;

        for (int i = 0; i < colliders.Length; i++)
        {
            DamageableObject obj = colliders[i].gameObject.GetComponent<DamageableObject>();
            if (obj && obj.gameObject != gameObject)
            {
                obj.Hit(source);
            }
        }

        source.active = false;
        if (trailEffect) trailEffect.GetComponent<ParticleSystem>().Stop();

        var ffObj = new GameObject();
        ffObj.transform.position = transform.position;
        ffObj.name = "Explosion Force";

        var ff = ffObj.AddComponent<ParticleSystemForceField>();
        ff.shape = ParticleSystemForceFieldShape.Sphere;
        ff.endRange = explosionRadius;
        ff.gravity = new ParticleSystem.MinMaxCurve(-1f);
        Destroy(ffObj, 0.5f);

        base.OnDeath(damageAmount, elementType, dealer);
    }
}
