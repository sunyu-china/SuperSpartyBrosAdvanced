using UnityEngine;
using System.Collections;

// for Enemy which moves horizontally
public class EnemyMove : Enemy {

	#region public vars
	[Range(0f,10f)]
	public float moveSpeed = 3f;  // enemy move speed when moving

	public GameObject[] myWaypoints; // to define the movement waypoints

	[Tooltip("How much time in seconds to wait at each waypoint.")]
	public float waitAtWaypointTime = 1f;   // how long to wait at a waypoint

	public bool loopWaypoints = true; // should it loop through the waypoints
	#endregion

	#region protected vars
	// movement tracking
	[SerializeField]
	protected int _myWaypointIndex = 0; // used as index for My_Waypoints
	protected float _moveTime; 
	protected float _vx = 0f;
	protected bool _moving = true;	// this is used in waypoints loop, not for the global state
	#endregion

	#region Unity funcs
	// Use this for initialization (overriding base.Awake())
	protected override void Awake () {
		// Call base func
		base.Awake();

		// setup moving defaults
		_moveTime = 0f;
		_moving = true;
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
	// Move the enemy horizontally through its rigidbody based on its waypoints
	protected void MoveTowardsWaypoint() {
		// if there isn't anything in My_Waypoints
		if ((myWaypoints.Length != 0) && (_moving == true)) {
			// make sure the enemy is facing the waypoint (based on previous movement)
			Flip (_vx);

			// determine distance between waypoint and enemy
			_vx = myWaypoints[_myWaypointIndex].transform.position.x-_transform.position.x;

			// if the enemy is close enough to waypoint, make it's new target the next waypoint
			if (Mathf.Abs(_vx) <= moveSpeed * Time.deltaTime) {
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

				Move ();
			}
		}
	}

	// generic horizontal movement
	protected void Move () {
		if (_animator.GetBool ("Moving") == true) {
			// Set the enemy's velocity to moveSpeed in the x direction.
			_rigidbody.velocity = new Vector2 (_transform.localScale.x * moveSpeed, _rigidbody.velocity.y);
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
	#endregion
}
