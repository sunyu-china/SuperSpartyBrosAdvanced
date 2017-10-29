using UnityEngine;
using System.Collections;

public class OldWizard : MonoBehaviour {

	Animator _animator;
	bool _meet = false;

	// Use this for initialization
	void Awake () {
		_animator = GetComponent<Animator>();
		if (_animator==null) // if Animator is missing
			Debug.LogError(name + ": Animator component missing from this gameobject");
	}
	
	void OnTriggerEnter2D (Collider2D collider) {

		if ((_meet == false) && (collider.tag == "Player")) {
			// set flag
			_meet = true;

			// set animation
			_animator.SetTrigger("Meet");
		}
	}

	void OnTriggerExit2D (Collider2D collider) {

		if (collider.tag == "Player") {
			// flip to face torward the player
			Flip (collider.transform.position.x - transform.position.x);
		}
	}

	void Flip(float _vx) {

		// get the current scale
		Vector3 localScale = transform.localScale;

		if ((_vx>0f)&&(localScale.x<0f))
			localScale.x*=-1;
		else if ((_vx<0f)&&(localScale.x>0f))
			localScale.x*=-1;

		// update the scale
		transform.localScale = localScale;
	}
}
