using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : DamageableObject
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

    void Update()
    {
        if (playerPos)
        {
            transform.position = Vector2.MoveTowards(transform.position, playerPos.position, speed * Time.deltaTime);
        }
    }

    private void FixedUpdate()
    {
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

    protected override void OnTriggerEnter2D(Collider2D col)
    {
        base.OnTriggerEnter2D(col);

        if (col.CompareTag("Player"))
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
