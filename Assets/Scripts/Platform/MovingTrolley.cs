using UnityEngine;
using System.Collections;

public class MovingTrolley : MovingPlatform {

	protected Animator _animator;

	// Use this for initialization (overriding base.Awake())
	protected override void Awake () {
		_animator = GetComponent<Animator>();
		if (_animator == null) // if Animator is missing
			Debug.LogError(name + ": Animator component missing from this gameobject");

		// Call base func
		base.Awake ();
	}

	// override the original method to add animation control
	public override void StopMoving () {
		_moving = false;
		_animator.SetBool ("Moving", false);
	}

	// override the original method to add animation control
	public override void StartMoving () {
		_moving = true;
		_animator.SetBool ("Moving", true);
	}
}
