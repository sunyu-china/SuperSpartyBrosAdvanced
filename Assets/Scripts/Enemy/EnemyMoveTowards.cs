using UnityEngine;
using System.Collections;

// for Enemy which moves horizontally
public class EnemyMoveTowards: Enemy {

	#region public vars
	[Range(0f,10f)]
	public float MoveSpeed = 3f;  // enemy move speed when moving

	public Transform MoveTarget; // target to move to
	#endregion

	#region protected vars
	// movement tracking
	[SerializeField]
	protected float _moveTime; 
	#endregion

	#region Unity funcs
	// Use this for initialization (overriding base.Awake())
	protected override void Awake () {
		// Call base func
		base.Awake();

		// setup moving defaults
		_moveTime = 0f;
	}
	
	// Update is called once per frame (overriding base.Update())
	protected override void Update () {
		// Call base func
		base.Update();

		// if not stunned then move the enemy when time is > _moveTime
		if (isStunned == false)
		{
			if (MoveTarget != null && Time.time >= _moveTime) {
				_animator.SetBool("Moving", true);
				MoveTowardsTarget ();
			} else {
				_animator.SetBool("Moving", false);
			}
		}
	}

	protected override void OnTriggerEnter2D(Collider2D collider)
	{
		// attack player (replacing the same function in base)
		if ((collider.tag == "Player") && (isStunned == false))
		{
			CharacterController2D player = collider.gameObject.GetComponent<CharacterController2D>();
			if (player.playerCanMove == true) {
				// Make sure the enemy is facing the player on attack
				Flip(collider.transform.position.x - _transform.position.x);

				// attack sound
				playSound(attackSFX);

				// stop moving
				Stop();

				// apply damage to the player
				player.ApplyDamage (damageAmount);

				// stop to enjoy killing the player
				_moveTime = Time.time + attackSFX.length;
			}
		}
	}
	#endregion

	#region protected funcs
	// Move the enemy towards the target transform
	protected void MoveTowardsTarget() {
		
		Vector3 current = transform.position;
		Vector3 target = MoveTarget.position;
		float step = MoveSpeed * Time.deltaTime;

		if (Vector3.Distance (current, target) > step) {
			// Make sure the enemy is facing the player on attack
			Flip(target.x - current.x);
			// move
			transform.position = Vector3.MoveTowards (current, target, step);
		}
		// do not move when close enough
	}

	// stop movement
	protected void Stop () {
		_rigidbody.velocity = Vector2.zero;
	}
	#endregion

	#region public funcs
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

			// stop moving
			Stop();

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

		// switch layer back to regular layer for regular collisions with the player
		this.gameObject.layer = _enemyLayer;
		stunnedCheck.layer = _enemyLayer;

		// provide the player with feedback
		_animator.SetTrigger("Stand");
	}

	// chase the target transform (e.g. called when player enters enemy's chase area)
	public void OnChase (Transform target) {
		// set target to chase
		MoveTarget = target;
	}

	// stop chasing (e.g. called when player exits enemy's chase area)
	public void OffChase () {
		// clear target to chase
		MoveTarget = null;
		// stop
		Stop();
	}
	#endregion
}
