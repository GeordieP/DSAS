using UnityEngine;

public class BasicBoss : Boss {
    private float rotateSpeed;
    private int bulletCount;

    protected override void Spawn() {
        // get bullet pool, begin initial phase - set initial position and velocity
        base.Spawn();

        initialHealth = 2580;
        health = initialHealth;

        rotateSpeed = 0.5f;
        bulletCount = 2;

        // set up the shoot timer, attach it to shoot method
        bossBulletPool = GameManager.Instance.BossBulletPool;
        shootTimer = TimerManager.Instance.CreateTimerRepeat(timeBetweenShots);
        shootTimer.onFinish += shootTimer_onFinish;

    }

    protected override void shootTimer_onFinish() {
        Shoot();
    }

    protected override void Shoot() {
        switch (currentPhase) {
            case 1: case 2: case 3:
                // shoot bullets out radially

                GameObject[] bullets = GameManager.Instance.BossBulletPool.Borrow(bulletCount);
                BossBullet currentBullet;
                
                float angleIncrement = 360 / bulletCount;

                for (int i = 0; i < bulletCount; i++) {
                    currentBullet = bullets[i].GetComponent<BossBullet>();
                    currentBullet.SetType(0);

                    float angle = (transform.rotation.eulerAngles.z * Mathf.Rad2Deg) + angleIncrement + angleIncrement * i;
                    currentBullet.SetRotation(angle);
                    Vector3 moveDirection = new Vector3(Mathf.Cos(Mathf.Deg2Rad * angle), Mathf.Sin(Mathf.Deg2Rad * angle), 0f);
                    currentBullet.Spawn(transform.position, moveDirection);
                    bullets[i].SetActive(true);
                }
                break;
        }
    }

    protected override void AdvancePhase() {
        currentPhase++;
        switch (currentPhase) {
            case 2:
                rotateSpeed = 2f;
                bulletCount = 5;
                break;
            case 3:
                rotateSpeed = 4f;
                bulletCount = 8;
            break;
        }
    }

    private void Update() {
        switch (currentPhase) {
            case 0:
                if (transform.position.y > postIntroPosition.y) {
                    transform.Translate(new Vector3(0f, introVelocity * Time.deltaTime, 0f), Space.World);
                } else {
                    transform.position = postIntroPosition;
                    introMoveComplete = true;
                    ++currentPhase;
                    shootTimer.Start();
                }
            break;

            case 1: case 2: case 3:
                transform.Rotate(Vector3.forward, rotateSpeed);
            break;

        }
    }
}
