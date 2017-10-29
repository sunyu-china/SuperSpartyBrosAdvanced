using UnityEngine;
using System.Collections;

public class DeathZone : MonoBehaviour {

	public bool destroyNonPlayerObjects = true;

	// Handle gameobjects collider with a deathzone object
	void OnCollisionEnter2D (Collision2D collider) {
		if (collider.gameObject.tag == "Player")
		{
			// if player then tell the player to do its FallDeath
			collider.gameObject.GetComponent<CharacterController2D>().FallDeath ();
		} else if (destroyNonPlayerObjects) { // not playe so just kill object - could be falling enemy for example
			DestroyObject(collider.gameObject);
		}
	}
}
