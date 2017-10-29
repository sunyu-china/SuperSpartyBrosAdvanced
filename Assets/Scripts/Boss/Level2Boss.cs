using UnityEngine;
using System.Collections;

public class Level2Boss : Enemy {

	#region public vars
	[Tooltip ("Also works for hits!")]
	public int StunsToDie = 8;
	public int StunsToFury = 4;

	public GameObject BombmanPrefab;
	public float FirePerSec = 1f;
	public float FirePerSecInFury = 2f;
	public float FireDuration = 5f;
	public float IdleDuration = 4f;
	public float MoveDuration = 2f;

	[Tooltip ("Must be 2 waypoints - left and right")]
	public GameObject[] Waypoints;

	public GameObject GunnerPrefab;
	public GameObject VictoryPrefab;

	[Tooltip ("Fire position")]
	public Transform Gunpoint;
	#endregion

	#region private vars
	HealthBar _healthBar;
	int _maxStunsToDie;
	bool _isFury;
	bool _isFiring;
	float _nextFireTime;
	float _nextIdleTime;
	int _waypointIndex;		// used as index for Waypoints
	float _moveSpeed;
	GameObject _bombmanParent;
	Vector3 _diePos;
	#endregion

	#region Unity funcs
	// Use this for initialization (overriding base.Awake())
	protected override void Awake () {
		// Call base func
		base.Awake();

		if (BombmanPrefab == null)
			Debug.LogError(name + ": BombmanPrefab not set!");

		if (Waypoints.Length != 2 || Waypoints[0] == null || Waypoints[1] == null)
			Debug.LogError(name + ": Waypoints (must be 2 - left and right) not set!");
		
		if (GunnerPrefab == null)
			Debug.LogError(name + ": GunnerPrefab not set!");

		if (VictoryPrefab == null)
			Debug.LogError(name + ": VictoryPrefab not set!");

		_healthBar = GameObject.FindObjectOfType<HealthBar> ();
		if (_healthBar == null)
			Debug.LogError(name + ": can not find Health Bar for the boss!");

		// create a parent (for bombmans) if necessary
		_bombmanParent = GameObject.Find ("BossProjectiles");
		if (_bombmanParent == null) {
			_bombmanParent = new GameObject("BossProjectiles");
		}

		// initialize private vars
		_isFury = false;
		_isFiring = false;
		_nextFireTime = 0;
		_nextIdleTime = 0;
		_maxStunsToDie = StunsToDie;

		_waypointIndex = 0;
		_moveSpeed = Vector3.Distance(Waypoints[0].transform.position, Waypoints[1].transform.position) / MoveDuration;
	}

	// Update is called once per frame (overriding base.Update())
	protected override void Update () {
		// Call base func
		base.Update();

		if (_isFiring == false) {	// Idle state
			if (Time.time >= _nextFireTime) {	// Idle time is up
				_animator.SetTrigger("Fire");
				// leave the call of StartFire() to animation
			} else if (_isFury == true) {
				MoveTowardsWaypoint ();
			}
			// else do nothing (leave it in idle)
		} else {					// Fire state
			if (Time.time >= _nextIdleTime) {	// Fire time is up
				_animator.SetTrigger("Ceasefire");
				StopFire ();
			} else {							// Fire in progress
				Fire ();
			}
		}
	}

	protected override void OnTriggerEnter2D(Collider2D collider)
	{
		// attack player
		if ((collider.tag == "Player") && (isStunned == false))
		{
			AttackPlayer (collider);
		}
	}

	// bug fixing, boss is too big, so staying in its collider needs to trigger damage again and again
	protected void OnTriggerStay2D(Collider2D collider)
	{
		// attack player
		if ((collider.tag == "Player") && (isStunned == false))
		{
			AttackPlayer (collider);
		}
	}
	#endregion

	#region private funcs
	void StartFire () {
		// calculate (next) stop firing time
		_nextIdleTime = Time.time + FireDuration + Random.Range(-1f, 1f);	// add some randomness

		// set flag
		_isFiring = true;

		// bug fixing, need to clear this tigger which can be set reduntantly (because of the delay in animation before calling this func)
		_animator.ResetTrigger ("Fire");
	}

	void StopFire () {
		// calculate (next) start firing time
		if (_isFury == true) {
			_nextFireTime = Time.time + MoveDuration;
			_animator.SetBool ("Moving", true);
		} else {
			_nextFireTime = Time.time + IdleDuration + Random.Range(-1f, 1f);	// add some randomness
		}

		// set flag
		_isFiring = false;
	}

	void Fire ()
	{
		float probability;
		if (_isFury == false) {		// Normal fire rate
			probability = Time.deltaTime * FirePerSec;
		} else {					// Fury fire rate
			probability = Time.deltaTime * FirePerSecInFury;
		}

		if (probability >= 1f) {
			Debug.LogWarning (name + "Change rate capped by frame rate!");
		}

		if (Random.value < probability) {
			GameObject bombman = Instantiate (BombmanPrefab, Gunpoint.position, Quaternion.identity) as GameObject;
			// child spawned objects
			bombman.transform.parent = _bombmanParent.transform;
			// commented out, too difficult for playing
//			if (isStunned == true) {	// in stunned state the attack direction should be more straight-forward, to force the player go far away
//				bombman.GetComponent<Rigidbody2D> ().AddForce (new Vector2 (Random.Range (-300f, -100f), Random.Range (-100f, 300f)));
//			} else {					// normal case
//				bombman.GetComponent<Rigidbody2D> ().AddForce (new Vector2 (Random.Range (-500f, -100f), Random.Range (-100f, 500f)));
//			}
			bombman.GetComponent<Rigidbody2D> ().AddForce (new Vector2 (Random.Range (-500f * transform.localScale.x, -100f * transform.localScale.x), Random.Range (-100f, 500f)));
		}
	}

	void AttackPlayer (Collider2D collider)
	{
		CharacterController2D player = collider.gameObject.GetComponent<CharacterController2D> ();
		if (player.playerCanMove == true) {
			// do not flip (special case for boss2)
			// attack sound
			playSound (attackSFX);
			// apply damage to the player
			player.ApplyDamage (damageAmount);
		}
	}

	void MoveTowardsWaypoint() {
		// this boss is actually using this animator flag for moving
		if (_animator.GetBool("Moving") == true) {
			// move towards waypoint
			Vector3 current = transform.position;
			Vector3 target = Waypoints [_waypointIndex].transform.position;
			float step = _moveSpeed * Time.deltaTime;
			transform.position = Vector3.MoveTowards (current, target, step);

			float vx = Waypoints[_waypointIndex].transform.position.x - transform.position.x;
			// move complete
			if (Mathf.Abs(vx) <= 0.8f) {	// close enough, the sensitivitiy needs to be well tuned becaused this boss is not moving perfectly horizontally
				// clear flag
				_animator.SetBool ("Moving", false);

				// just flip
				Flip ((transform.localScale.x * -1f));

				// stop movement
				Stop();

				// increment to next index in array
				_waypointIndex++;
				// reset waypoint back to 0 for looping
				if (_waypointIndex >= Waypoints.Length) {
					_waypointIndex = 0;
				}
			}
		}
	}

	void Stop () {
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

			StunsToDie--;

			if (StunsToDie <= 0) {
				// boss is defeated!
				Die();
			} else {
				// update health bar
				_healthBar.UpdateHealthBar ((float)StunsToDie / (float)_maxStunsToDie);

				// provide the player with feedback that enemy is stunned
				playSound (stunnedSFX);
				_animator.SetTrigger ("Stunned");

				// switch layer to stunned layer so no collisions with the player while stunned
				gameObject.layer = _stunnedLayer;
				stunnedCheck.layer = _stunnedLayer;

				// commented out (2016/8/30) since the boss will move after becoming fury, so players can not wait steadily
				// boss 2 must fire in stunned state, otherwise player can make use of it by keep attacking while waiting for the stunned period
				//StartCoroutine (WaitToFire (0.3f));	// add some delay before firing

				// start coroutine to stand up eventually
				StartCoroutine (Stand ());

				// fury time!
				if (_isFury == false && StunsToDie <= StunsToFury)
					_isFury = true;
			}
		}
	}

	// simple wait function
	public IEnumerator WaitToFire (float time)
	{
		yield return new WaitForSeconds(time);

		StartFire ();
	}

	// coroutine to unstun the enemy and stand back up (function that overrides its virtual ancestor)
	public override IEnumerator Stand()
	{
		yield return new WaitForSeconds(stunnedTime); 

		// no longer stunned
		isStunned = false;

		// switch layer back to regular layer for regular collisions with the player
		this.gameObject.layer = _enemyLayer;
		stunnedCheck.layer = _enemyLayer;

		// provide the player with feedback
		_animator.SetTrigger("Stand");

		// commented out (2016/8/30)
		// should set state to Idle as well
		//StopFire ();
	}

	// Die function (overriding base.Die())
	public override void Die ()
	{
		// disable health bar
		_healthBar.gameObject.SetActive (false);

		// stop everything
		StopFire ();							// bug fixing, should stop firing before stop moving
		_animator.SetBool ("Moving", false);	// bug fixing, this really stops moving
		Stop ();

		// play death sfx
		playSound(deathSFX);

		// record the current position for coroutine to use
		_diePos = transform.position;
		// move the particle effect in front, in case it ejects inside the screen (in 3D env)
		Vector3 pos = _diePos;
		pos.z -= 5f;
		pos.x += 0.5f;
		pos.y -= 0.3f;
		// play death animation
		Instantiate(deathExplosionPrefab, pos, Quaternion.identity);

		// disable the image (for better visual effect)
		gameObject.GetComponent<SpriteRenderer> ().enabled = false;

		// start coroutine to stand up eventually
		StartCoroutine (Win ());
	}

	// coroutine to unstun the enemy and stand back up (virtual function that to be overridden by inheritance)
	public IEnumerator Win ()
	{
		yield return new WaitForSeconds(1f);

		// instantiate pilot
		Instantiate(GunnerPrefab, _diePos, Quaternion.identity);

		// instantiate victory obj
		_diePos.x += 0.7f;	// make it next to the stunned gunner
		Instantiate(VictoryPrefab, _diePos, Quaternion.identity);

		// kill itseft
		Destroy(gameObject);
	}
	#endregion
}
