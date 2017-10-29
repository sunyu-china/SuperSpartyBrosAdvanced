using UnityEngine;
using System.Collections;

public class DamageZone : MonoBehaviour {

	public int damageAmount = 1;

	// Handle gameobjects collider with a damagezone object
	void OnTriggerEnter2D (Collider2D collider) {
		if (collider.gameObject.tag == "Player")
		{
			// if player then tell the player to do its ApplyDamage
			collider.gameObject.GetComponent<CharacterController2D>().ApplyDamage(damageAmount);
		}
	}

	// if stays on the hazard, keep applying damage
	void OnTriggerStay2D (Collider2D collider) {
		if (collider.gameObject.tag == "Player")
		{
			// if player then tell the player to do its ApplyDamage
			collider.gameObject.GetComponent<CharacterController2D>().ApplyDamage(damageAmount);
		}
	}
}
