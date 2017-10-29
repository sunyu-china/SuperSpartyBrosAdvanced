using UnityEngine;
using System.Collections;

public class MaxHealth : MonoBehaviour {

	private bool taken = false;

	// if the player touches the max health object, it has not already been taken, and the player can move (not dead or victory)
	// then the player increases max health by one
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
			characterController.AddMaxHealth();

			// destroy the extra life gameobject
			DestroyObject(gameObject);
		}
	}
}
