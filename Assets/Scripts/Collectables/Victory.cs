using UnityEngine;
using System.Collections;

public class Victory : MonoBehaviour {

	public bool taken = false;
	public GameObject explosion;

	// if the player touches the victory object, it has not already been taken, and the player can move (not dead or victory)
	// then the player has reached the victory point of the level
	void OnTriggerEnter2D (Collider2D collider)
	{
		CharacterController2D characterController = collider.gameObject.GetComponent<CharacterController2D> ();
		if ((collider.tag == "Player" ) && 
			(taken == false) && 
			(characterController != null) && 
			(characterController.playerCanMove == true))
		{
			// mark as taken so doesn't get taken multiple times
			taken=true;

			// if explosion prefab is provide, then instantiate it
			if (explosion)
			{
				Instantiate(explosion,transform.position,transform.rotation);
			}

			// do the player victory thing
			characterController.Victory();

			// destroy the victory gameobject
			DestroyObject(gameObject);
		}
	}

}
