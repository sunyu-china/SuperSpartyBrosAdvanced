using UnityEngine;
using System.Collections;

public class Fireball : MonoBehaviour {

	[Tooltip("moving speed in the x-axis")]
	public static float speed = 7f;

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

		// destroy itself when hitting an object
		Destroy (gameObject);
	}

	void OnBecameInvisible () {
		// destroy itself when out of screen
		Destroy (gameObject);
	}
}
