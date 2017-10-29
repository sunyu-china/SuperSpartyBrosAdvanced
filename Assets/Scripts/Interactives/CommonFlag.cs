using UnityEngine;
using System.Collections;

// base class for common flag to activate things when passing by
public class CommonFlag : MonoBehaviour {

	public GameObject[] ThingsToActivate;

	protected bool triggered = false;

	// Use this for initialization
	protected virtual void Awake () {

		if (ThingsToActivate.Length == 0) {
			Debug.LogError (name + ": ThingsToActivate not set!");
		} else {
			for (int i = 0; i < ThingsToActivate.Length; i++) {
				if (ThingsToActivate [i] == null)
					Debug.LogError (name + ": ThingsToActivate[" + i + "] not set!");
			}
		}
	
	}
	
	protected virtual void OnTriggerEnter2D (Collider2D collider)
	{
		if ( (collider.tag == "Player" ) && (triggered == false) )
		{
			// mark as triggered so doesn't get triggered multiple times
			triggered = true;

			// activate things
			for (int i = 0; i < ThingsToActivate.Length; i++) {
				ThingsToActivate [i].SetActive (true);
			}
		}
	}
}
