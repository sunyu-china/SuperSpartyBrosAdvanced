using UnityEngine;
using System.Collections;

public class Breakable : MonoBehaviour {

	// SFXs
	public AudioClip breakSFX;

	public GameObject explosionPrefab;

	public virtual void Break () {
		if (breakSFX != null) {
			// must use this method because we're going to destroy this in later code
			AudioSource.PlayClipAtPoint (breakSFX, Camera.main.transform.position);
		}

		if (explosionPrefab != null) {
			// move the particle effect in front, in case it ejects inside the screen (in 3D env)
			Vector3 pos = transform.position;
			pos.z -= 5;
			// play explosion effect (particle system)
			Instantiate(explosionPrefab, pos, Quaternion.identity);
		}

		// play the animation
		GetComponent<Animator> ().SetTrigger ("Break");

		// leave the kill function to be called by animation
	}

	// called by animation
	protected virtual void Die () {
		Destroy (gameObject);
	}
}
