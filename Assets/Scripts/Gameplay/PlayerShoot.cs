using UnityEngine;

public class PlayerShoot : MonoBehaviour {
    public GameObject _bulletPrefab;
    private bool _shooting = true;
    public bool Shooting {
        get { return _shooting; }
        set {
            _shooting = value;
            if (value && !shootTimer.running)
                shootTimer.Start();
            else if (!value && shootTimer != null && shootTimer.running)
                shootTimer.Stop();
        }
    }

    public Powerup.PlayerShootPatternDelegate shootPattern;

    private const float FIRE_RATE = Balance.PLAYER_FIRE_RATE;
    private Timer shootTimer;
    private GameObjectPool playerBulletPool;

    // initial start
    private void Start () {
        playerBulletPool = GameManager.Instance.PlayerBulletPool;
        shootTimer = TimerManager.Instance.CreateTimerRepeat(1 / FIRE_RATE);
        shootTimer.onFinish += shootTimer_onFinish;
        shootPattern = DefaultShoot;

        if (_shooting) shootTimer.Start();
    }

    private void shootTimer_onFinish() {
        shootPattern(transform.position, playerBulletPool);
    }

    private void DefaultShoot(Vector3 origin, GameObjectPool pool) {
        GameObject bullet = playerBulletPool.Borrow();
        bullet.GetComponent<PlayerBullet>().SetType(0);
        bullet.GetComponent<PlayerBullet>().Spawn(transform.position);
        bullet.SetActive(true);
    }
}
