using UnityEngine;
using System.Collections;

public class ChaseArea : MonoBehaviour {

	EnemyMoveTowards _enemy;

	// Use this for initialization
	void Awake () {
		_enemy = GetComponentInParent<EnemyMoveTowards> ();
		if (_enemy == null)
			Debug.LogError(name + ": can not find EnemyMoveTowards in parent gameobject!");
	}

	// change the method from enter to stay, but it seems not a big difference
	void OnTriggerStay2D (Collider2D collider) {
		if (collider.tag == "Player") {		// player enters the chase area, notify the enemy (its parent)
			_enemy.OnChase(collider.transform);
		}
	}

	void OnTriggerExit2D (Collider2D collider) {
		if (collider.tag == "Player") {		// player exits the chase area, notify the enemy (its parent)
			_enemy.OffChase();
		}
	}
}
