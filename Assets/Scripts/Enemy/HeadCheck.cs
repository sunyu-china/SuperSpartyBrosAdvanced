using UnityEngine;
using System.Collections;

public class HeadCheck : MonoBehaviour {

	// if Player hits the stun point of the enemy, then call Stunned on the enemy
	void OnCollisionEnter2D(Collision2D collider)
	{
		if (collider.gameObject.tag == "Player")
		{
			// tell the enemy to be stunned
			GetComponentInParent<Enemy>().Stunned();	// note this function is polymorphic

			// make the player bounce off the enemy (aka, jump)
			collider.gameObject.GetComponent<CharacterController2D>().EnemyBounce();
		}
	}
}
