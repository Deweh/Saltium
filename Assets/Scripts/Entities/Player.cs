using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : DamageableObject
{
    public float speed = 5;

    private Rigidbody2D rb;
    private Vector2 moveVelocity;

    protected override void Start()
    {
        base.Start();
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        Vector2 moveInput = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
        moveVelocity = moveInput.normalized * speed;

        rb.MovePosition(rb.position + moveVelocity * Time.fixedDeltaTime);

        if (Input.GetMouseButtonDown(0))
        {
            ProjectileWeapon weapon = GetComponent<ProjectileWeapon>();
            if (weapon)
            {
                weapon.Fire(Camera.main.ScreenToWorldPoint(Input.mousePosition));
            }
        }
    }

    protected override void OnDeath(float damageAmount, ElementType elementType, string dealer)
    {
        base.OnDeath(damageAmount, elementType, dealer);

        GameObject[] controllers = GameObject.FindGameObjectsWithTag("GameController");

        foreach (GameObject control in controllers)
        {
            control.SendMessage("OnPlayerDeath", new PlayerDeathArgs() { damageAmount = damageAmount, elementType = elementType, damageDealer = dealer, player = gameObject }, SendMessageOptions.DontRequireReceiver);
        }
    }

    public struct PlayerDeathArgs
    {
        public float damageAmount;
        public ElementType elementType;
        public string damageDealer;
        public GameObject player;
    }
}
