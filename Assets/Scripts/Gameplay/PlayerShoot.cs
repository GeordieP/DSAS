using UnityEngine;

public class PlayerShoot : MonoBehaviour {
    public GameObject _bulletPrefab;
    public bool _shooting;

    private const float FIRE_RATE = 5;
    private float timeUntilNextShot;
    private float betweenShotCounter;

	void Start () {
        timeUntilNextShot = 1 / FIRE_RATE;
	}
	
	void Update () {
        if (_shooting) {
            if ((betweenShotCounter += Time.deltaTime) > timeUntilNextShot) {
                betweenShotCounter = 0f;
                Instantiate(_bulletPrefab, transform.position, Quaternion.identity);
            }
        }
	}
}
