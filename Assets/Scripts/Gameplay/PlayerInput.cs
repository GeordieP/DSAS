using UnityEngine;
using System.Collections;

public class PlayerInput : MonoBehaviour {
	// Movement directions 
	private const System.SByte NONE = 0;
	private const System.SByte RIGHT = 1;
	private const System.SByte LEFT = -1;
	// Movement direction - basically just simplifies setting the sign of movement velocity (negative = left)
	private System.SByte MOVE_DIR = NONE;
	private const float MOVESPEED = Balance.PLAYER_BASE_MOVE_SPEED;
	private float TOUCH_POS_SPEED_SCALING;		// scale based on how close the touch is to the edge of the screen as a percentage (screen resolution independent)
	private float halfScreenWidth;
	// screen bounds converted to world space. Positive is right edge, negative is left
	private float worldSpaceScreenBound;

	private const float SMOOTHING_TIME_ACCELERATING = 0.03f;
	private const float SMOOTHING_TIME_DECELERATING = 0.05f;
	private float SMOOTHING_TIME = SMOOTHING_TIME_ACCELERATING;

	private float velocity, targetVelocity;
	private float velocitySmoothing;

	Timer nukeTriggerDelayTimer;
	private const float NUKE_TRIGGER_DELAY_DURATION = 1f;

	private const float NUKE_MAX_CHARGE_LEVEL = Balance.NUKE_MAX_CHARGE_LEVEL;
	private float nukeChargeLevel = NUKE_MAX_CHARGE_LEVEL;

	private void Start() {
		halfScreenWidth = Screen.width / 2;
		worldSpaceScreenBound = -Camera.main.ScreenToWorldPoint(Vector3.zero).x;
		nukeTriggerDelayTimer = TimerManager.Instance.CreateTimerOneshot(NUKE_TRIGGER_DELAY_DURATION);
		nukeTriggerDelayTimer.onFinish += nukeTriggerDelayTimer_onFinish;
	}

	private void Update() {

		if (Application.platform == RuntimePlatform.WindowsEditor || Application.platform == RuntimePlatform.WindowsPlayer) {
			if (Input.GetKey(KeyCode.Space)) {
				MOVE_DIR = NONE;
				SMOOTHING_TIME = SMOOTHING_TIME_DECELERATING;
				if (!nukeTriggerDelayTimer.running)
					nukeTriggerDelayTimer.Start();
			} else {
				if (Input.GetKey(KeyCode.A)) {
					MOVE_DIR = LEFT;
					SMOOTHING_TIME = SMOOTHING_TIME_ACCELERATING;
					TOUCH_POS_SPEED_SCALING = 1f;
				} else if (Input.GetKey(KeyCode.D)) {
					MOVE_DIR = RIGHT;
					SMOOTHING_TIME = SMOOTHING_TIME_ACCELERATING;
					TOUCH_POS_SPEED_SCALING = 1f;
				} else {
					MOVE_DIR = NONE;
					SMOOTHING_TIME = SMOOTHING_TIME_DECELERATING;
				}
				if (nukeTriggerDelayTimer.running)
					nukeTriggerDelayTimer.Stop();

			}
		} else {
			// user is touching screen
			if (Input.touches.Length > 0) {
				// more than one finger
				if (Input.touches.Length > 1) {
					// stop movement, start nuke timer
					MOVE_DIR = NONE;
					SMOOTHING_TIME = SMOOTHING_TIME_DECELERATING;
					if (!nukeTriggerDelayTimer.running)
						nukeTriggerDelayTimer.Start();
				} else {
					// only one finger
					// move appropriate direction and stop nuke timer if it's running

					if (Input.GetTouch(Input.touches.Length - 1).position.x < halfScreenWidth) {
						MOVE_DIR = LEFT;
						TOUCH_POS_SPEED_SCALING = (halfScreenWidth - Input.GetTouch(Input.touches.Length - 1).position.x) / halfScreenWidth;
					} else {
						MOVE_DIR = RIGHT;
						TOUCH_POS_SPEED_SCALING = (Input.GetTouch(Input.touches.Length - 1).position.x - halfScreenWidth) / halfScreenWidth;
					}
					SMOOTHING_TIME = SMOOTHING_TIME_ACCELERATING;

					if (nukeTriggerDelayTimer.running)
						nukeTriggerDelayTimer.Stop();
				}
			} else {
				// no fingers on screen
				MOVE_DIR = NONE;
				SMOOTHING_TIME = SMOOTHING_TIME_DECELERATING;

				if (nukeTriggerDelayTimer.running)
					nukeTriggerDelayTimer.Stop();
			}
		}
	}
	
	private void nukeTriggerDelayTimer_onFinish() {
		if (nukeChargeLevel < NUKE_MAX_CHARGE_LEVEL) return;
		GameManager.Instance.PlayerNuke();
		nukeChargeLevel = 0f;
		GameManager.Instance.UpdateNukeBar(0f);
		StartCoroutine(ChargeNuke());
	}

	private IEnumerator ChargeNuke() {
		while (nukeChargeLevel < NUKE_MAX_CHARGE_LEVEL) {
			nukeChargeLevel += Time.deltaTime;
			GameManager.Instance.UpdateNukeBar(nukeChargeLevel / NUKE_MAX_CHARGE_LEVEL);
			yield return new WaitForFixedUpdate();
		}
	} 

	private void FixedUpdate() {
		// Move character based on input. Smooth out acceleration using SmoothDamp
		targetVelocity = MOVE_DIR * MOVESPEED * TOUCH_POS_SPEED_SCALING;
		velocity = Mathf.SmoothDamp(velocity, targetVelocity, ref velocitySmoothing, SMOOTHING_TIME);
		transform.Translate(Vector3.right * velocity * Time.deltaTime);

		transform.position = new Vector3(Mathf.Clamp(transform.position.x, -worldSpaceScreenBound, worldSpaceScreenBound), transform.position.y, transform.position.z);
	}
}
