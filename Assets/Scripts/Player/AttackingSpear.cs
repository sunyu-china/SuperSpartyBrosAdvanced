using UnityEngine;
using System.Collections;

public class AttackingSpear : MonoBehaviour {

	void OnTriggerEnter2D (Collider2D collider) {
		Enemy enemy = collider.GetComponent<Enemy> ();
		if (enemy != null) {	// collide with an enemy
			if (collider.tag == "Boss") {	// for boss, call stunned to deal with damage
				enemy.Stunned();
			} else {						// for others just die
				enemy.Die ();
			}
		}

		Breakable breakable = collider.GetComponent<Breakable>();
		if (breakable != null) {	// destroy breakable objects
			breakable.Break();
		}

		Interactive interactive = collider.GetComponent<Interactive>();
		if (interactive != null) {	// trigger interactive objects
			interactive.Trigger();
		}
	}
}
