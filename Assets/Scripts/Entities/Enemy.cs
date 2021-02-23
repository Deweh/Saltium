using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : Entity
{
    public float speed;
    public float startTimeBtwProjectiles = 1;

    private Transform playerPos;
    private float timeBtwProjectiles;

    protected override void Start()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player)
        {
            playerPos = GameObject.FindGameObjectWithTag("Player").GetComponent<Transform>();
        }

        timeBtwProjectiles = startTimeBtwProjectiles;
    }

    private void FixedUpdate()
    {
        if (playerPos)
        {
            TryMove((playerPos.position - transform.position).normalized * speed);
        }

        timeBtwProjectiles -= Time.fixedDeltaTime;

        if (timeBtwProjectiles <= 0f)
        {
            ProjectileWeapon weapon = GetComponent<ProjectileWeapon>();
            if (weapon && playerPos)
            {
                weapon.Fire(playerPos.position);
            }

            timeBtwProjectiles = startTimeBtwProjectiles;
        }
    }

    protected override void OnCollisionEnter2D(Collision2D col)
    {
        base.OnCollisionEnter2D(col);

        if (col.gameObject.CompareTag("Player"))
        {
            var obj = col.gameObject.GetComponent<DamageableObject>();
            if (obj)
            {
                obj.Damage(1, ElementType.Magic, "Enemy");
            }
            Damage(health, ElementType.Magic, "Enemy");
        }
    }
}
