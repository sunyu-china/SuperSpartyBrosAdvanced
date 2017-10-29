using UnityEngine;
using System.Collections;

public class HealthPotion : MonoBehaviour {

	private bool taken = false;

	// if the player touches the health potion object, it has not already been taken, and the player can move (not dead or victory)
	// then the player recovers health by one
	void OnTriggerEnter2D (Collider2D collider)
	{
		CharacterController2D characterController = collider.gameObject.GetComponent<CharacterController2D> ();
		if ((collider.tag == "Player" ) && 
			(taken == false) && 
			(characterController != null) && 
			(characterController.playerCanMove == true))
		{
			// mark as taken so doesn't get taken multiple times
			taken = true;

			// do the extra life thing
			characterController.AddHealth(1);

			// destroy the extra life gameobject
			DestroyObject(gameObject);
		}
	}
}
