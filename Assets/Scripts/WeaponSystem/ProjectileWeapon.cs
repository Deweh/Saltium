using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileWeapon : MonoBehaviour
{
    public GameObject projectile;

    public void Fire(Vector3 target)
    {
        target.z = transform.position.z;
        Vector3 vectorToTarget = target - transform.position;
        Quaternion targetRotation = Quaternion.LookRotation(Vector3.forward, vectorToTarget);

        Instantiate(projectile, transform.position + vectorToTarget.normalized * 0.5f, targetRotation);
    }
}
