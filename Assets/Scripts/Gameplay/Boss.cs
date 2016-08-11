using UnityEngine;

public class Boss : MonoBehaviour {
    byte currentPhase;

    // Intro
    bool introMoveComplete, introRotationComplete;
    public const float IntroDuration = 4f;        // seconds
    private float introVelocity;
    private Vector3 postIntroPosition = new Vector3(0f, 3f, 0f);

    // Object Pooling
    private GameObjectPool bossBulletPool;

    // Timers
    Timer shootTimer;

    // Shooting
    private const float timeBetweenShots = 0.2f;

    private void Awake() {  
        bossBulletPool = GameManager.Instance.BossBulletPool;
        shootTimer = TimerManager.Instance.CreateTimerRepeat(timeBetweenShots);
        shootTimer.onFinish += shootTimer_onFinish;

        // initial phase - intro - moving to initial position before doing anything
        currentPhase = 0;
        // move body to starting position
        transform.position = new Vector3(0f, Balance.BossSpawnBounds.top, 0f);

        // velocity is based on the duration of the intro and how far above the screen the boss is set to spawn
        introVelocity = (postIntroPosition.y - Balance.BossSpawnBounds.top) / IntroDuration;
    }

    private void shootTimer_onFinish() {
        Shoot();
    }

    private void Update() {
        switch (currentPhase) {
            case 0:
                if (transform.position.y > postIntroPosition.y) {
                    transform.Translate(new Vector3(0f, introVelocity * Time.deltaTime, 0f), Space.World);
                    // transform.Rotate(new Vector3(0f, 0f, 1f), 1f);
                } else {
                    transform.position = postIntroPosition;
                    introMoveComplete = true;
                    ++currentPhase;
                    shootTimer.Start();
                }
            break;

            case 1:
                // rotate slowly and shoot bullets out radially
                transform.Rotate(Vector3.forward, 1f);
            break;

        }
    }

    private void Shoot() {
        int bulletCount = 5;
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
    }
}
