﻿using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public abstract class Bullet : PooledEntity {
    protected float MOVE_SPEED = Balance.BULLET_INITIAL_SPEED;
    protected float dmg_value = Balance.BULLET_BASE_DMG;
    public float Dmg_Value { get { return dmg_value; } }
    protected Vector3 moveDirection = Vector3.down;

    private static readonly int DamageableLayer = LayerMask.NameToLayer("Damageable");

    public virtual void Spawn(Vector3 shooterPosition) {
        transform.position = shooterPosition + new Vector3(0f, 0f, Balance.BULLET_Z_POSITION);
    }

    public virtual void Spawn(Vector3 shooterPosition, Vector3 moveDirection) {
        transform.position = shooterPosition + new Vector3(0f, 0f, Balance.BULLET_Z_POSITION);
        this.moveDirection = moveDirection;
    }

    public abstract void SetType(int typeIndex);

    private void OnTriggerEnter2D(Collider2D other) {
        if (other.gameObject.layer == DamageableLayer)
            other.GetComponent<IDamageable>().HitByBullet(this);
    }
}
