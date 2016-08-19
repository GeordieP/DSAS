using UnityEngine;
using System.Collections;

public interface IDamageable {
    Color originalColor { get; set; }
    float initialHealth { get; set; }
    float health { get; set; }

    void CheckHealth();
    void Dead();
    void Knockback();

    IEnumerator ColorFlash();
    IEnumerator ShootDelay();

}
