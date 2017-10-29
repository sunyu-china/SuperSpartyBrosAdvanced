using UnityEngine;
using System.Collections;

// base class for interactive objects
public class Interactive : MonoBehaviour {

	public AudioClip triggerSFX;

	// called by outside collider (e.g. player weapon)
	public virtual void Trigger () {
		// play animation
		GetComponent<Animator> ().SetTrigger ("Trigger");

		// play SFX
		if (triggerSFX == true) {
			AudioSource.PlayClipAtPoint(triggerSFX, Camera.main.transform.position);
		}

		// leave the call of other methods to animation
	}
}
