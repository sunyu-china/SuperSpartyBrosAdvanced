using UnityEngine;
using System.Collections;

public class OldSwordman : MonoBehaviour {

	public AudioClip frozenSFX;

	bool _isFrozen = false;
	Animator _animator;
	AudioSource _audio;

	void Awake () {
		if (frozenSFX == null)
			Debug.LogError(name + ": frozenSFX not set!");

		_animator = GetComponent<Animator>();
		if (_animator == null) // if Animator is missing
			Debug.LogError(name + ": Animator component missing from this gameobject");

		_audio = GetComponent<AudioSource> ();
		if (_audio == null) { // if AudioSource is missing
			Debug.LogWarning(name + ": AudioSource component missing from this gameobject. Adding one.");
			// let's just add the AudioSource component dynamically
			_audio = gameObject.AddComponent<AudioSource>();
		}
	}

	void OnTriggerEnter2D (Collider2D collider)
	{
		if (_isFrozen == false) {
			// special interaction with iceball (Boss3's projectile, base class FlyingSpear)
			FlyingSpear iceball = collider.GetComponent<FlyingSpear> ();
			if (iceball != null) {
				_audio.PlayOneShot(frozenSFX);
				SetFrozen ();
				_animator.SetTrigger ("Freeze");
			}
		}
	}

	void SetFrozen () {
		_isFrozen = true;
	}

	// called by animation
	void ClearFrozen () {
		_isFrozen = false;
	}
}
