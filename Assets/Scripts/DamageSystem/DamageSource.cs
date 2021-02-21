using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageSource : MonoBehaviour
{
    public float damageAmount = 1f;
    public ElementType elementType;
    public bool active = true;
}

public enum ElementType
{
    Magic
}
