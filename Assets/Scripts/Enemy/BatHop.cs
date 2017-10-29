using UnityEngine;
using System.Collections;

// this is specific for the hopping bat, inheriting generic Enemy class
public class BatHop : Enemy {

	// Use this for initialization (overriding base.Awake())
	protected override void Awake () {
		// Call base func
		base.Awake();

		// set animation flag "Hopping"
		_animator.SetTrigger("Hop");
	}
	
	// setup the enemy to be stunned (function that overrides its virtual ancestor)
	public override void Stunned()
	{
		if (isStunned == false) 
		{
			isStunned = true;

			// for flying enemies, when stunned, enabling gravity by setting kinematic to false
			_rigidbody.isKinematic = false;

			// provide the player with feedback that enemy is stunned
			playSound(stunnedSFX);
			_animator.SetTrigger("Stunned");

			// switch layer to stunned layer so no collisions with the player while stunned
			gameObject.layer = _stunnedLayer;
			stunnedCheck.layer = _stunnedLayer;

			// start coroutine to stand up eventually
			StartCoroutine (Stand ());
		}
	}

	// coroutine to unstun the enemy and stand back up (function that overrides its virtual ancestor)
	public override IEnumerator Stand()
	{
		yield return new WaitForSeconds(stunnedTime); 

		// no longer stunned
		isStunned = false;

		// for flying enemies, when returning from stunned, disabling gravity by setting kinematic to true
		_rigidbody.isKinematic = true;

		// when standing up, make it a bit (1 unit) higher
		Vector2 pos = _transform.position;
		pos.y += 1f;
		_transform.position = pos;

		// switch layer back to regular layer for regular collisions with the player
		this.gameObject.layer = _enemyLayer;
		stunnedCheck.layer = _enemyLayer;

		// provide the player with feedback
		_animator.SetTrigger("Stand");

		// (re)start hopping
		_animator.SetTrigger("Hop");
	}
}
