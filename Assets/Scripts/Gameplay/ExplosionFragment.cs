﻿using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class ExplosionFragment : PooledEntity {
    private Vector3 velocity;
    private float MOVE_SPEED;
    private float explosionStartTime, timeDiff;
    private const float velocityLimitY = -6f;     // max velocity of particles moving downwards
    private static Vector2 acceleration = new Vector2(0.97f, -0.2f);
    private float rotationStep;

	private void Start () {
        GetComponent<SpriteRenderer>().sprite = GameManager.Instance.ExplosionFragmentSprites[Random.Range(0, GameManager.Instance.ExplosionFragmentSprites.Length)];
	}

    public void Spawn(Vector3 origin, Vector3 directionInfluence, float explosionStartTime) {
        this.explosionStartTime = explosionStartTime;
        transform.position = origin;
        MOVE_SPEED = Random.Range(3f, 15f);
        velocity = new Vector3(Random.Range(-1f, 1f), 0.8f, 0f) + directionInfluence;
        rotationStep = Random.Range(-8f, 8f);
    }

    public override void Despawn() {
        GetComponent<SpriteRenderer>().sprite = GameManager.Instance.ExplosionFragmentSprites[Random.Range(0, GameManager.Instance.ExplosionFragmentSprites.Length)];
        base.Despawn();
    }   

	private void FixedUpdate () {
        timeDiff = Time.timeSinceLevelLoad - explosionStartTime;
        velocity.x *= acceleration.x;
        if (velocity.y > velocityLimitY) velocity.y += acceleration.y * timeDiff;

        transform.Rotate(new Vector3(0f, 0f, 1f), rotationStep);
        transform.Translate(velocity * MOVE_SPEED * Time.deltaTime, Space.World);

        if (transform.position.y > Balance.DespawnBounds.top || transform.position.y < Balance.DespawnBounds.bottom || transform.position.x < Balance.DespawnBounds.left || transform.position.x > Balance.DespawnBounds.right) {
            Despawn();
        }
	}
}
