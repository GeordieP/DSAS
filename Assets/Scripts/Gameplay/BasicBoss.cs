using UnityEngine;

public class BasicBoss : Boss {
    protected override void Spawn() {
        // set up the shoot timer, attach it to shoot method
        bossBulletPool = GameManager.Instance.BossBulletPool;
        shootTimer = TimerManager.Instance.CreateTimerRepeat(timeBetweenShots);
        shootTimer.onFinish += shootTimer_onFinish;

        // get bullet pool, begin initial phase - set initial position and velocity
        base.Spawn();
    }

    protected override void shootTimer_onFinish() {
        Shoot();
    }

    // override void Shoot() { // TOOD }

    private void Update() {
        BasicBossUpdate();
    }
}
