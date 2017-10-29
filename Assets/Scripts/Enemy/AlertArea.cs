using UnityEngine;
using System.Collections;

public class AlertArea : MonoBehaviour {

	EnemyScout _enemy;

	// Use this for initialization
	void Awake () {
		_enemy = GetComponentInParent<EnemyScout> ();
		if (_enemy == null)
			Debug.LogError(name + ": can not find EnemyScout in parent gameobject!");
	}
	
	// change the method from enter to stay, but it seems not a big difference
	void OnTriggerStay2D (Collider2D collider) {
		if (collider.tag == "Player") {		// player enters the alert area, notify the enemy (its parent)
			_enemy.OnAlert(collider.transform);
		}
	}

	void OnTriggerExit2D (Collider2D collider) {
		if (collider.tag == "Player") {		// player exits the alert area, notify the enemy (its parent)
			_enemy.OffAlert();
		}
	}
}
