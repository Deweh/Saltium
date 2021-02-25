using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : Entity
{
    public float speed = 5;

    private Vector2 moveVelocity;

    void Update()
    {
        Vector2 moveInput = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
        moveVelocity = moveInput.normalized * speed;

        if (Input.GetMouseButtonDown(0) && !PauseMenu.gamePaused)
        {
            ProjectileWeapon weapon = GetComponent<ProjectileWeapon>();
            if (weapon)
            {
                weapon.Fire(Camera.main.ScreenToWorldPoint(Input.mousePosition));
            }
        }
    }

    private void FixedUpdate()
    {
        TryMove(moveVelocity);
    }

    public struct PlayerDeathArgs
    {
        public float damageAmount;
        public ElementType elementType;
        public string damageDealer;
        public GameObject player;
    }
}
