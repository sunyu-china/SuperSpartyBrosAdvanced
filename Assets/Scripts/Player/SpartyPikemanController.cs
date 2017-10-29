using UnityEngine;
using System.Collections;
using UnityStandardAssets.CrossPlatformInput;

public class SpartyPikemanController : CharacterController2D {

	#region public vars
	// SFXs
	public AudioClip attackSFX;
	#endregion

	#region protected vars
	// child object for attacking spear
	protected AttackingSpear attackingSpear;
	#endregion

	#region Unity funcs
	// Use this for initialization (overriding base.Awake())
	protected override void Awake () {
		// Call base func
		base.Awake();

		attackingSpear = GetComponentInChildren<AttackingSpear> ();
		if (attackingSpear == null) {
			Debug.LogError(name + ": Can not find AttackingSpear in children!");
		}
		DisableAttackingSpear ();
	}
	
	// Update is called once per frame (overriding base.Update())
	protected override void Update () {
		// Call base func
		base.Update();

		// exit update if player cannot move or game is paused
		if (!playerCanMove || (Time.timeScale == 0f))
			return;

		// Player attack
		if(CrossPlatformInputManager.GetButtonDown("Fire1"))
		{
			DoAttack ();
		}
	}
	#endregion

	#region protected funcs
	// make the player attack
	protected void DoAttack ()
	{
		// play the attack animation
		_animator.SetTrigger ("Attack");
		// play the attack sound
		PlaySound (attackSFX);
		// leave the attacking field enable/disable logic to animation
	}

	// called by animation
	protected void EnableAttackingSpear () {
		attackingSpear.gameObject.SetActive (true);
	}

	// called by animation
	protected void DisableAttackingSpear () {
		attackingSpear.gameObject.SetActive (false);
	}
	#endregion
}
