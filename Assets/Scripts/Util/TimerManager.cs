/*
Written by Geordie Powers
https://github.com/GeordieP/
https://gist.github.com/GeordieP/
MIT License
*/

/*
USAGE:
Create a one-shot timer named "myTimer" with duration 3 seconds, and 
call a method (return type void) named "myTimer_onFinish" when finished

	Timer myTimer = TimerManager.Instance.CreateTimerOneshot(3.0f);
	myTimer.onFinish += myTimer_onFinish;
	myTimer.Start();

To create and start the timer immediately:

	Timer myTimer = TimerManager.Instance.StartTimerOneshot(3.0f);
	myTimer.onFinish += myTimer_onFinish;

Repeating timers:

	Timer myTimer = TimerManager.Instance.CreateTimerRepeat(3.0f);
	myTimer.onFinish += myTimer_onFinish;
	myTimer.Start();

	myTimer.Stop(); // call to stop, and .Start() to start again
*/

using UnityEngine;
using System.Collections.Generic;

public class TimerManager : PersistentUnitySingleton<TimerManager> {
	private List<Timer> timers = new List<Timer>();

	void Update() {
		if (timers.Count < 1) return;

		for (int i = 0; i < timers.Count; i++) {
			if (timers[i].running) {
				timers[i].Update(Time.deltaTime);
				if (timers[i].finished && timers[i].DeleteOnFinish) {
					timers.RemoveAt(i);
					i--;
				}
			}
		}
	}

	// One shot timers will run until they fire, then stop executing and wait until they're started again
	// Create a timer and return it
	public Timer CreateTimerOneshot(float duration) {
		Timer temp = new Timer(duration);
		timers.Add(temp);
		return temp;
	}

	// Create a timer, start it, and return it
	public Timer StartTimerOneshot(float duration) {
		Timer temp = new Timer(duration);
		timers.Add(temp);
		temp.Start();
		return temp;
	}

	// Temporary one shot timers are the same as one shot timers, but they'll get deleted after they fire once
	// Create a timer and return it
	public Timer CreateTimerTemporaryOneshot(float duration) {
		Timer temp = new Timer(duration);
		temp.DeleteOnFinish = true;
		timers.Add(temp);
		return temp;
	}

	// Create a timer, start it, and return it
	public Timer StartTimerTemporaryOneshot(float duration) {
		Timer temp = new Timer(duration);
		temp.DeleteOnFinish = true;
		timers.Add(temp);
		temp.Start();
		return temp;
	}

	// Repeating timers work the exact same way except they don't set the finished and running
	// flags once they complete, they'll just invoke their callback and run again
	// Repeating timers won't stop until .Stop() is called
	public Timer CreateTimerRepeat(float duration) {
		Timer temp = new Timer(duration);
		temp.Repeat = true;
		timers.Add(temp);
		return temp;
	}

	// Create a timer, start it, and return it
	public Timer StartTimerRepeat(float duration) {
		Timer temp = new Timer(duration);
		temp.Repeat = true;
		timers.Add(temp);
		temp.Start();
		return temp;
	}

	public void StopAndDeleteAll() {
		timers = new List<Timer>();
	}
}

public class Timer {
	public float elapsed, duration;
	public bool running, finished;
	public delegate void FinishAction();
	public event FinishAction onFinish;

	private bool deleteOnFinish;
	public bool DeleteOnFinish {
		get { return deleteOnFinish; }
		set { deleteOnFinish = true; }
	}

	private bool repeat;
	public bool Repeat {
		get { return repeat; }
		set { repeat = true; }
	}

	public Timer(float duration) {
		elapsed = 0f;
		running = false;
		finished = false;
		this.duration = duration;
	}

	public void Start() {
		running = true;
	}

	// start running and finish immediately
	// for when we want to essentially skip waiting for the first cycle to complete
	public void StartInstant() {
		running = true;
		Finish();
	}

	public void Stop() {
		running = false;
		elapsed = 0f;
	}

	public void Delete() {
		// set duration to 0 so on next Update, elapsed will be > duration and trigger Finish()
		// since deleteOnFinish is now true, the timer manager will delete the finished timer
		duration = 0f;
		deleteOnFinish = true;
	}

	public void Update(float deltaTime) {
		elapsed += deltaTime;
		if (elapsed >= duration) Finish();
	}

	public void Finish() {
		onFinish();
		elapsed = 0f;
		if (repeat) return;
		finished = true;
		running = false;
	}
}
