using UnityEngine;
using System.Collections;

// specific flag for triggering barrel throwing for the dragon knight
public class DragonKnightFlag : MonoBehaviour {

	public GameObject DragonKnight;

	DragonKnight _dragonKnight;
	bool triggered = false;

	// Use this for initialization
	void Awake () {
		if (DragonKnight == null)
			Debug.LogError(name + ": DragonKnight not set!");
		_dragonKnight = DragonKnight.GetComponent<DragonKnight> ();
		if (_dragonKnight == null)
			Debug.LogError(name + ": DragonKnight not correctly set!");

		triggered = false;
	}
	
	void OnTriggerEnter2D (Collider2D collider)
	{
		if ( (collider.tag == "Player" ) && (triggered == false) )
		{
			// mark as triggered so doesn't get triggered multiple times
			triggered = true;

			// dragon knight start throwing barrels
			_dragonKnight.StartThrowing ();
		}
	}
}
