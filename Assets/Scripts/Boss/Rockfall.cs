using UnityEngine;
using System.Collections;

// this has become the base class for boss's projectile
public class Rockfall : MonoBehaviour {

	#region public vars
	public int damageAmount = 1;

	// SFXs
	public AudioClip hitSFX;	// sfx when hitting player
	public AudioClip crashSFX;	// sfx when crashing
	#endregion

	#region protected vars
	protected Animator _animator;
	protected AudioSource _audio;
	#endregion

	#region Unity funcs
	// Use this for initialization
	protected virtual void Awake () {
		_animator = GetComponent<Animator>();
		if (_animator==null) // if Animator is missing
			Debug.LogError(name + ": Animator component missing from this gameobject");

		_audio = GetComponent<AudioSource> ();
		if (_audio==null) { // if AudioSource is missing
			Debug.LogWarning(name + ": AudioSource component missing from this gameobject. Adding one.");
			// let's just add the AudioSource component dynamically
			_audio = gameObject.AddComponent<AudioSource>();
		}
	}
	
	// Update is called once per frame
	protected virtual void Update () {
	
	}

	protected virtual void OnCollisionEnter2D(Collision2D collision)
	{
		if (collision.gameObject.tag == "Player") {
			CharacterController2D player = collision.gameObject.GetComponent<CharacterController2D>();
			if (player.playerCanMove == true) {
				// hit sound
				if (hitSFX != null)
					playSound(hitSFX);

				// apply damage to the player
				player.ApplyDamage (damageAmount);

				// crash when hitting player
				_animator.SetTrigger ("Crash");
				// leave the killitself action (Die) to animation
			}
		} else if (collision.gameObject.tag == "Ground") {
			// crash when hitting ground
			_animator.SetTrigger ("Crash");
			// leave the killitself action (Die) to animation
		}
	}
	#endregion

	#region protected funcs
	// play sound through the audiosource on the gameobject
	protected void playSound(AudioClip clip)
	{
		_audio.PlayOneShot(clip);
	}

	// called by animation
	protected void playCrashSound ()
	{
		// crash sound
		if (crashSFX != null)
			// must use this method because we're going to destroy this in later code
			AudioSource.PlayClipAtPoint (crashSFX, Camera.main.transform.position);
	}

	// called by animation
	protected virtual void Die() {
		Destroy (gameObject);
	}
	#endregion
}
