using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IDamageable
{
    void Damage(float damageAmount, Vector2 hitDirection = default);

    void Die();

    float MaxHealth { get; set; }
    float CurrentHealth { get; set; }
}