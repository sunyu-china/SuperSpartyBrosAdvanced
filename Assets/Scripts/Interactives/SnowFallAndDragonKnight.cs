using UnityEngine;
using System.Collections;

public class SnowFallAndDragonKnight : CommonFlag {

	public GameObject DragonKnight;

	DragonKnight _dragonKnight;

	// Use this for initialization (overriding base.Awake())
	protected override void Awake () {
		// Call base func
		base.Awake ();

		if (DragonKnight == null)
			Debug.LogError(name + ": DragonKnight not set!");
		_dragonKnight = DragonKnight.GetComponent<DragonKnight> ();
		if (_dragonKnight == null)
			Debug.LogError(name + ": DragonKnight not correctly set!");
	}

	// This overrides base.OnTriggerEnter2D()
	protected override void OnTriggerEnter2D (Collider2D collider)
	{
		if ( (collider.tag == "Player" ) && (triggered == false) )
		{
			// mark as triggered so doesn't get triggered multiple times
			triggered = true;

			// activate things
			for (int i = 0; i < ThingsToActivate.Length; i++) {
				ThingsToActivate [i].SetActive (true);
			}

			// if DragonKnight is still alive, destroy it!
			if (DragonKnight != null) {
				_dragonKnight.Die ();
			}
		}
	}
}
