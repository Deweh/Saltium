using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageableObject : MonoBehaviour
{
    /// <summary>
    /// Current health. Do not set programatically, use Damage, Hit, or Kill instead.
    /// </summary>
    public float health = 10f;
    /// <summary>
    /// Maximum health. Do not set programatically, use SetMaxHealth instead.
    /// </summary>
    public float maxHealth = 10f;
    public bool invulnerable = false;
    protected bool isEntity = false;
    public GameObject deathEffect;
    public GameObject damageTakenEffect;
    public DeathBehavior deathBehavior;
    public HealthBar healthBar;
    /// <summary>
    /// Percentage resistances to each element type. If resistance is above 1f, that damage type will
    /// add health instead of subtracting. If resistance is negative, that damage type will be multiplied.
    /// </summary>
    public Dictionary<ElementType, float> resistances = new Dictionary<ElementType, float>();

    private bool dead = false;
    private float editorOldMax = 10f;
    protected float deathEffectRadius = 0f;
    protected float damagedEffectRadius = 0f;

    protected virtual void OnValidate()
    {
        if (maxHealth != editorOldMax && health == editorOldMax)
        {
            health = maxHealth;
        }

        editorOldMax = maxHealth;
    }

    protected virtual void Start()
    {
        if (healthBar)
        {
            healthBar.SetMaxHealth(maxHealth);
            healthBar.SetHealth(health);
        }
    }

    protected virtual void OnCollisionEnter2D(Collision2D collision)
    {
        DamageSource damageSource = collision.gameObject.GetComponent<DamageSource>();
        if (damageSource)
        {
            DamageReceived(damageSource);
        }
    }

    protected virtual void OnTriggerEnter2D(Collider2D collision)
    {
        DamageSource damageSource = collision.gameObject.GetComponent<DamageSource>();
        if (damageSource)
        {
            DamageReceived(damageSource);
        }
    }

    public bool IsEntity()
    {
        return isEntity;
    }

    public void SetMaxHealth(float max)
    {
        if (health > max)
        {
            health = max;
        }

        maxHealth = max;
        if (healthBar)
        {
            healthBar.SetMaxHealth(max);
            healthBar.SetHealth(health);
        }
    }

    protected virtual void OnDeath(float damageAmount, ElementType elementType, string dealer, GameObject dealerObj = null)
    {
        GameController.OnDeathCallback(damageAmount, elementType, dealer, gameObject, dealerObj);

        if (deathEffect)
        {
            var effect = Instantiate(deathEffect, transform.position, Quaternion.identity);

            if (deathEffectRadius > 0f)
            {
                var ps = effect.GetComponent<ParticleSystem>();
                var s = ps.shape;
                s.radius = deathEffectRadius;
                var burst = ps.emission.GetBurst(0);
                ps.emission.SetBurst(0, new ParticleSystem.Burst(0, burst.count.constant * deathEffectRadius));
            }
        }

        switch (deathBehavior)
        {
            case DeathBehavior.Destroy:
                Destroy(gameObject);
                break;
            case DeathBehavior.Disable:
                gameObject.SetActive(false);
                break;
        }
    }

    public void Kill(string dealerName = "System", GameObject dealerObj = null)
    {
        DamageReceived(health, ElementType.Magic, dealerName, true, dealerObj);
    }

    public void Damage(float damage, ElementType elementType, string damageDealer, GameObject dealerObj = null)
    {
        DamageReceived(damage, elementType, damageDealer, true, dealerObj);
    }

    public void Hit(DamageSource damageSource)
    {
        DamageReceived(damageSource);
    }

    protected virtual void DamageReceived(float amount, ElementType elementType, string dealer, bool sourceActive = true, GameObject dealerObject = null)
    {
        if (!sourceActive)
        {
            return;
        }

        float damageAmount = CalculateDamage(amount, elementType);

        if (damageAmount > 0f)
        {
            health -= damageAmount;

            if (health > maxHealth) health = maxHealth;

            if (healthBar)
            {
                healthBar.SetHealth(health);
            }

            if (damageTakenEffect)
            {
                var effect = Instantiate(damageTakenEffect, transform.position, Quaternion.identity);

                if (damagedEffectRadius > 0f)
                {
                    var ps = effect.GetComponent<ParticleSystem>();
                    var s = ps.shape;
                    s.radius = damagedEffectRadius;
                    var burst = ps.emission.GetBurst(0);
                    ps.emission.SetBurst(0, new ParticleSystem.Burst(0, burst.count.constant * damagedEffectRadius));
                }
            }

            if (health <= 0f && !dead)
            {
                dead = true;
                OnDeath(amount, elementType, dealer, dealerObject);
            }
        }
    }

    private void DamageReceived(DamageSource damageSource)
    {
        DamageReceived(damageSource.damageAmount, damageSource.elementType, damageSource.name, damageSource.active, damageSource.gameObject);
    }

    protected virtual float CalculateDamage(float amount, ElementType elementType)
    {
        if (invulnerable) return 0f;

        if (resistances.ContainsKey(elementType))
        {
            return amount * (1f - resistances[elementType]);
        }

        return amount;
    }
}

public enum DeathBehavior
{
    Destroy,
    Disable,
    None
}