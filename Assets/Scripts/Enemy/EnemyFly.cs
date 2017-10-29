using UnityEngine;
using System.Collections;

// for Enemy which moves vertically
public class EnemyFly : Enemy {

	#region public vars
	[Range(0f,10f)]
	public float moveSpeed = 4f;  // enemy move speed when moving

	public GameObject[] myWaypoints; // to define the movement waypoints

	[Tooltip("How much time in seconds to wait at each waypoint.")]
	public float waitAtWaypointTime = 1f;   // how long to wait at a waypoint

	public bool loopWaypoints = true; // should it loop through the waypoints
	#endregion

	#region protected vars
	// movement tracking
	[SerializeField]
	protected int _myWaypointIndex = 0; // used as index for My_Waypoints
	protected float _moveTime = 0f; 
	protected float _vy = 0f;
	protected bool _moving = true;	// this is used in waypoints loop, not for the global state
	protected int _localScaleY = 1; // used in vertical movement
	#endregion

	#region Unity funcs
	// Use this for initialization (overriding base.Awake())
	protected override void Awake () {
		// Call base func
		base.Awake();

		// setup moving defaults
		_moveTime = 0f;
		_moving = true;

		// set animation flag "Flying"
		_animator.SetBool("Flying", true);
	}
	
	// Update is called once per frame (overriding base.Update())
	protected override void Update () {
		// Call base func
		base.Update();

		// if not stunned then move the enemy when time is > _moveTime
		if (isStunned == false)
		{
			if (Time.time >= _moveTime) {
				MoveTowardsWaypoint ();
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
	// Move the enemy vertically through its rigidbody based on its waypoints
	protected void MoveTowardsWaypoint() {
		// if there isn't anything in My_Waypoints
		if ((myWaypoints.Length != 0) && (_moving == true)) {
			// do not need to flip in y axis, but need to set up the moving direction
			if ( (_vy > 0f) && (_localScaleY < 0) )
				_localScaleY *= -1;
			else if ( (_vy < 0f) && (_localScaleY > 0))
				_localScaleY *= -1;

			// determine distance between waypoint and enemy
			_vy = myWaypoints[_myWaypointIndex].transform.position.y - _transform.position.y;

			// if the enemy is close enough to waypoint, make it's new target the next waypoint
			if (Mathf.Abs(_vy) <= moveSpeed * Time.deltaTime) {			// close enough, the sensitivitiy needs to be well tuned
				// At waypoint so stop moving
				Stop();

				// increment to next index in array
				_myWaypointIndex++;

				// reset waypoint back to 0 for looping
				if(_myWaypointIndex >= myWaypoints.Length) {
					if (loopWaypoints)
						_myWaypointIndex = 0;
					else
						_moving = false;
				}

				// setup wait time at current waypoint
				_moveTime = Time.time + waitAtWaypointTime;
			} else {
				// enemy is moving
				_animator.SetBool("Moving", true);

				// Set the enemy's velocity to moveSpeed in the x direction.
				_rigidbody.velocity = new Vector2(_rigidbody.velocity.x, _localScaleY * moveSpeed);
			}
		}
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
	#endregion
}
