using UnityEngine;
using System.Collections;

public class BreakableBrick : Breakable {

	[Tooltip ("How many hits to break it")]
	public int hitsToBreak = 0;

	// SFXs
	public AudioClip hitSFX;

	// override the old Break method to cope with hits
	public override void Break () {

		hitsToBreak--;

		if (hitsToBreak > 0) {	// not yet broken
			// play hit sfx
			if (hitSFX != null) {
				AudioSource.PlayClipAtPoint (hitSFX, Camera.main.transform.position);
			}

			// play the animation
			GetComponent<Animator> ().SetTrigger ("Hit");

		} else {				// broken
			// play break sfx
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

			Die ();
		}
	}
}
