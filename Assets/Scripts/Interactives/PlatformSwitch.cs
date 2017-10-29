using UnityEngine;
using System.Collections;

public class PlatformSwitch : Interactive {

	public GameObject ControlledMovingPlatform;

	private MovingPlatform _movingPlatform;

	void Awake () {
		if (ControlledMovingPlatform == null) {
			Debug.LogError (name + ": ControlledMovingPlatform not set!");
		} else {
			_movingPlatform = ControlledMovingPlatform.GetComponent<MovingPlatform> ();
			if (_movingPlatform == null)
				Debug.LogError (name + ": ControlledMovingPlatform not correctly set!");
		}
	}

	// called by animation
	void PlatformStartMoving () {
		_movingPlatform.StartMoving ();
	}

	// called by animation
	void PlatformStopMoving () {
		_movingPlatform.StopMoving ();
	}
}
