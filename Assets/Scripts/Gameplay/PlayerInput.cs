using UnityEngine;

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

	Timer bombTriggerDelayTimer;
	private const float BOMB_TRIGGER_DELAY_DURATION = 1f;

	void Start() {
		halfScreenWidth = Screen.width / 2;
		worldSpaceScreenBound = -Camera.main.ScreenToWorldPoint(Vector3.zero).x;
		bombTriggerDelayTimer = TimerManager.Instance.CreateTimerOneshot(BOMB_TRIGGER_DELAY_DURATION);
		bombTriggerDelayTimer.onFinish += bombTriggerDelayTimer_onFinish;
	}

	void Update() {
		// user is touching screen
		if (Input.touches.Length > 0) {
			// more than one finger
			if (Input.touches.Length > 1) {
				// stop movement, start bomb timer
				MOVE_DIR = NONE;
				SMOOTHING_TIME = SMOOTHING_TIME_DECELERATING;
				if (!bombTriggerDelayTimer.running)
					bombTriggerDelayTimer.Start();
			} else {
				// only one finger
				// move appropriate direction and stop bomb timer if it's running
				
				if (Input.GetTouch(Input.touches.Length - 1).position.x < halfScreenWidth) {
					MOVE_DIR = LEFT;
					TOUCH_POS_SPEED_SCALING = (halfScreenWidth - Input.GetTouch(Input.touches.Length - 1).position.x) / halfScreenWidth;
				} else {
					MOVE_DIR = RIGHT;
					TOUCH_POS_SPEED_SCALING = (Input.GetTouch(Input.touches.Length - 1).position.x - halfScreenWidth) / halfScreenWidth;
				}
				SMOOTHING_TIME = SMOOTHING_TIME_ACCELERATING;

				if (bombTriggerDelayTimer.running)
					bombTriggerDelayTimer.Stop();
			}
		} else {
			// no fingers on screen
			MOVE_DIR = NONE;
			SMOOTHING_TIME = SMOOTHING_TIME_DECELERATING;

			if (bombTriggerDelayTimer.running)
				bombTriggerDelayTimer.Stop();
		}
	}

	void FixedUpdate() {
		// Move character based on input. Smooth out acceleration using SmoothDamp
		targetVelocity = MOVE_DIR * MOVESPEED * TOUCH_POS_SPEED_SCALING;
		velocity = Mathf.SmoothDamp(velocity, targetVelocity, ref velocitySmoothing, SMOOTHING_TIME);
		transform.Translate(Vector3.right * velocity * Time.deltaTime);

		transform.position = new Vector3(Mathf.Clamp(transform.position.x, -worldSpaceScreenBound, worldSpaceScreenBound), transform.position.y, transform.position.z);
	}

	void bombTriggerDelayTimer_onFinish() {
		GameManager.Instance.PlayerBomb();
	}
}
