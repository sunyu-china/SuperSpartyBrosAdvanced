using UnityEngine;
using System.Collections;

public class BatLairTrigger : MonoBehaviour {

	BatLair _batLair;
	bool _triggered;

	// Use this for initialization
	void Awake () {
		_batLair = GetComponentInParent<BatLair> ();
		if (_batLair == null)
			Debug.LogError(name + ": can not find BatLair in parent gameobject!");

		_triggered = false;
	}

	void OnTriggerEnter2D (Collider2D collider) {
		if (_triggered == false && collider.tag == "Player") {		// player enters the trigger area, notify the batlair (its parent)
			_triggered = true;		// only trigger once
			_batLair.Trigger(collider.transform);
		}
	}
}
