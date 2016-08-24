using UnityEngine;
using System.Collections;

public class Powerup : PooledEntity {
    
    private float MOVE_SPEED = Balance.POWERUP_BASE_MOVE_SPEED;

    /*  

    powerup types:
        - health
            - increase (or fill?) health
        - shield
            - shield to absorb damage
        - 2x size bullets
            - scale player's bullets (make SetSize in PlayerBullet, set size in PlayerShoot timer finish)
        - smaller ship
            - reduce ship size
        - different shoot patterns?


        need a timer to un-do these effects when they should wear off
        effects should probably have their duration bundled with them

        - player calls a delegate and passes itself, a delegate in here accepts the player and applies the effects and sets the effect duration
    
        player.cs:
            private delegate ApplyPlayerEffect(player);
            private ApplyPlayerEffect PlayerEffect;

            OnHit():
                PlayerEffect = 





    */

    public void Spawn() {
        transform.position = new Vector3(Random.Range(Balance.ScreenBounds.left, Balance.ScreenBounds.right), Balance.SpawnBounds.top, 0f);
    }

	// Update is called once per frame
	void Update () {
        transform.position = new Vector3(0f, -MOVE_SPEED * Time.deltaTime, 0f);
	}

    private void OnTriggerEnter2D(Collider2D other) {
        if (other.gameObject.tag == "Player")
            other.GetComponent<Player>().GotPowerup(this);
    }
}
