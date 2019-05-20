using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHealth : MonoBehaviour
{
    public PlayerMovementManager movementManager;
    public RectTransform healthBar;

    [SerializeField] private float maxHealth;
    [SerializeField] private float currentHealth;

    private bool isInvulnerable;
    private float invulnTimer = 1f;

	// Use this for initialization
	void Start ()
    {
		
	}
	
	// Update is called once per frame
	void Update ()
    {
		if(isInvulnerable)
        {
            invulnTimer -= Time.deltaTime;
        }
        if(invulnTimer <= 0)
        {
            isInvulnerable = false;
        }
	}

    private void FixedUpdate()
    {
        if(currentHealth <= 0)
        {
            OnDeath();
        }
    }

    /// <summary>
    /// Called upon the player's death
    /// </summary>
    private void OnDeath()
    {
        Debug.Log("You dead bitch");
    }

    /// <summary>
    /// Player is healed for @amount of health
    /// </summary>
    /// <param name="amount"></param>
    public void Heal(float amount)
    {
        currentHealth += amount;
        if(currentHealth > maxHealth)
        {
            currentHealth = maxHealth;
        }
    }

    /// <summary>
    /// Player takes @amount of damage
    /// </summary>
    /// <param name="amount"></param>
    public void TakeDamage(float amount)
    {
        if (!isInvulnerable)
        {
            currentHealth -= amount;
            healthBar.sizeDelta = new Vector2(currentHealth, healthBar.sizeDelta.y);
            isInvulnerable = true;
        }
    }

    /// <summary>
    /// Player takes @amount of damage and is knocked back based on @enemyPOsition. WIP.
    /// </summary>
    /// <param name="amount"></param>
    /// <param name="enemyPosition"></param>
    public void TakeDamageWithKnockback(float amount, Vector3 enemyPosition)
    {
        if(!isInvulnerable)
        {
            currentHealth -= amount;
            healthBar.sizeDelta = new Vector2(currentHealth, healthBar.sizeDelta.y);
            isInvulnerable = true;
            movementManager.DamageKnockback((movementManager.transform.position - enemyPosition).normalized);
        }
    }

    /// <summary>
    /// Returns player's maximum health
    /// </summary>
    /// <returns></returns>
    public float GetMaxHealth()
    {
        return maxHealth;
    }

    /// <summary>
    /// Returns player's current health
    /// </summary>
    /// <returns></returns>
    public float GetCurrentHealth()
    {
        return currentHealth;
    }
}
