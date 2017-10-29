using UnityEngine;
using System.Collections;

// this becomes the base class for enemy's flying projectiles
public class FlyingSpear : MonoBehaviour {

	public int damageAmount = 1;
	[Tooltip("moving speed in the x-axis")]
	public static float speed = 6f;

	void OnTriggerEnter2D (Collider2D collider) {
		if (collider.tag == "Player") {
			CharacterController2D player = collider.GetComponent<CharacterController2D> ();
			if (player.playerCanMove == true) {
				// attack sound
				//playSound(attackSFX);

				// apply damage to the player
				player.ApplyDamage (damageAmount);
			}
		} else {
			// can enemy destroy breakable objects? yes
			Breakable breakable = collider.GetComponent<Breakable> ();
			if (breakable != null) {	// destroy breakable objects
				breakable.Break ();
			}
		}

		// destroy itself when hitting an object
		Destroy (gameObject);
	}

	void OnBecameInvisible () {
		// destroy itself when out of screen
		Destroy (gameObject);
	}
}
