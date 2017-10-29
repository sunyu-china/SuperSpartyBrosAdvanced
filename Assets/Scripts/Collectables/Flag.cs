using UnityEngine;
using System.Collections;

public class Flag : MonoBehaviour {

	public bool taken = false;
	public GameObject explosion;

	// private variables below
	
	// store references to components on the gameObject
	Transform _transform;
	Animator _animator;

	void Awake () {
		// get a reference to the components we are going to be changing and store a reference for efficiency purposes
		_transform = GetComponent<Transform> ();
		
		_animator = GetComponent<Animator>();
		if (_animator==null) // if Animator is missing
			Debug.LogError("Animator component missing from this gameobject");
	}
	
	// if the player passes the flag check point, it has not already been taken, and the player can move (not dead or victory)
	// then deactivate the flag object (animation)
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

			// Set the grounded animation states
			_animator.SetBool("FlagPassed", true);
			
			// do the flag checkpoint thing
			characterController.FlagPass(_transform.position);
		}
	}
}
