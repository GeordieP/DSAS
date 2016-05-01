using UnityEngine;

public class PlayerInput : MonoBehaviour {
	// Movement directions 
	private const System.SByte NONE = 0;
	private const System.SByte RIGHT = 1;
	private const System.SByte LEFT = -1;
	// Movement direction - basically just simplifies setting the sign of movement velocity (negative = left)
	private System.SByte MOVE_DIR = NONE;
	private const float moveSpeed = 10;
	private float halfScreenWidth;

	private const float SMOOTHING_TIME_ACCELERATING = 0.04f;
	private const float SMOOTHING_TIME_DECELERATING = 0.06f;
	private float SMOOTHING_TIME = SMOOTHING_TIME_ACCELERATING;

	private float velocity, targetVelocity;
	private float velocitySmoothing;

	Timer bombTriggerDelayTimer;
	float bombTriggerDelayDuration = 1f;

	// Use this for initialization
	void Start() {
		halfScreenWidth = Screen.width / 2;
		bombTriggerDelayTimer = TimerManager.Instance.CreateTimerOneshot(bombTriggerDelayDuration);
		bombTriggerDelayTimer.onFinish += bombTriggerDelayTimer_onFinish;
	}

	// Update is called once per frame
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
				MOVE_DIR = (Input.GetTouch(Input.touches.Length - 1).position.x < halfScreenWidth) ? LEFT : RIGHT;
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

		// Move character based on input. Smooth out acceleration using SmoothDamp
		targetVelocity = MOVE_DIR * moveSpeed;
		velocity = Mathf.SmoothDamp(velocity, targetVelocity, ref velocitySmoothing, SMOOTHING_TIME);
		transform.Translate(Vector3.right * velocity * Time.deltaTime);
	}

	void bombTriggerDelayTimer_onFinish() {
		print("BOOM");
	}
}
