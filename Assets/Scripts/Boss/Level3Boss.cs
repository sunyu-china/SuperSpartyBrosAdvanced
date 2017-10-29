using UnityEngine;
using System.Collections;

public class Level3Boss : Enemy {

	#region public vars
	[Tooltip ("Also works for hits!")]
	public int StunsToDie = 10;
	public int StunsToFury = 5;

	[Tooltip ("Must be 2 waypoints - topleft and topright")]
	public GameObject[] Waypoints;

	public GameObject IceballPrefab;
	public Transform FirePos;

	public float IdleDuration = 4f;
	public float FlyDuration = 3f;
	public float FireInterval = 2f;
	public float FireIntervalInFury = 2f;		// no difference (by default), for difficulty's sake

	public GameObject DragonKnightPrefab;
	public GameObject VictoryPrefab;
	#endregion

	#region private vars
	HealthBar _healthBar;
	int _maxStunsToDie;
	bool _isFury;
	float _nextFlyTime;
	float _nextFireTime;
	Vector3 _diePos;

	// Flying related local vars
	Vector3 _startFlyPos;
	Vector3 _targetFlyPos;
	float _startFlyTime;

	GameObject _iceballParent;
	private float[] _iceballSpeedX = {-8, -8f, -8f, -6f, -3f, 0f, 2f};
	private float[] _iceballSpeedY = {-4f, -6f, -8f, -8f, -8f, -8f, -8f};
	#endregion

	#region Unity funcs
	// Use this for initialization (overriding base.Awake())
	protected override void Awake () {
		// Call base func
		base.Awake();

		if (Waypoints.Length != 2 || Waypoints[0] == null || Waypoints[1] == null)
			Debug.LogError(name + ": Waypoints (must be 2 - topleft and topright) not set!");

		if (IceballPrefab == null)
			Debug.LogError(name + ": IceballPrefab not set!");

		if (FirePos == null)
			Debug.LogError(name + ": FirePos not set!");

		if (DragonKnightPrefab == null)
			Debug.LogError(name + ": DragonKnightPrefab not set!");

		if (VictoryPrefab == null)
			Debug.LogError(name + ": VictoryPrefab not set!");

		_healthBar = GameObject.FindObjectOfType<HealthBar> ();
		if (_healthBar == null)
			Debug.LogError(name + ": can not find Health Bar for the boss!");

		// create a parent (for iceballs) if necessary
		_iceballParent = GameObject.Find ("BossProjectiles");
		if (_iceballParent == null) {
			_iceballParent = new GameObject("BossProjectiles");
		}

		// initialize private vars
		// IMPORTANT: use this animator flag for the state of this boss - Flying = true (Flying), Flying = false (Idle)
		_animator.SetBool ("Flying", false);
		_isFury = false;
		_nextFlyTime = Time.time + IdleDuration;
		_nextFireTime = Time.time + FireInterval;
		_maxStunsToDie = StunsToDie;
		_startFlyPos = Waypoints[1].transform.position;
		_targetFlyPos = Waypoints[0].transform.position;
	}

	// Update is called once per frame (overriding base.Update())
	protected override void Update () {
		// Call base func
		base.Update();

		if (_animator.GetBool ("Flying") == false) {	// Idle state
			if (Time.time >= _nextFlyTime) {			// time to fly
				// set flag
				_animator.SetBool ("Flying", true);

				// set start fly time
				_startFlyTime = Time.time;
			} else {									// stay in idle
				if (Time.time >= _nextFireTime) {		// time to fire
					Fire ();

					// set next fire time
					if (_isFury == false) {
						_nextFireTime = Time.time + FireInterval;
					} else {
						_nextFireTime = Time.time + FireIntervalInFury;
					}
				}
			}
		} else {										// Flying state
			// in the function of Fly(), it will handle the logic for the state change (flying -> idle)
			Fly ();

			// also fire during flight when fury
			if (_isFury == true && Time.time >= _nextFireTime) {
				Fire ();
				_nextFireTime = Time.time + FireIntervalInFury;
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
	#endregion

	#region private funcs
	void Fly () {
		// Spherically movement between _startFlyPos and _targetFlyPos
		Vector3 center = (_startFlyPos + _targetFlyPos) * 0.5f;
		center += new Vector3 (0, 1, 0);
		Vector3 startRelCenter = _startFlyPos - center;
		Vector3 targetRelCenter = _targetFlyPos - center;
		float farcComplete = (Time.time - _startFlyTime) / FlyDuration;
		transform.position = Vector3.Slerp (startRelCenter, targetRelCenter, farcComplete);
		transform.position += center;

		if (transform.position == _targetFlyPos) {		// flying complete
			// just flip
			Flip ((transform.localScale.x * -1f));

			// switch start/target positions for the next fly
			Vector3 temp = _startFlyPos;
			_startFlyPos = _targetFlyPos;
			_targetFlyPos = temp;

			// set flag
			_animator.SetBool ("Flying", false);
			// set next fly time
			_nextFlyTime = Time.time + IdleDuration;
		}
	}

	void Fire ()
	{
		int iceballMax;
		// Fire X iceballs once
		if (_isFury == false) {
			iceballMax = 3;							// fire the first 3 iceballs normally
		} else {
			iceballMax = _iceballSpeedX.Length;		// when fury, fire all of them (7)
		}

		for (int i = 0; i < iceballMax; i++) {
			GameObject iceball = Instantiate (IceballPrefab, FirePos.position, Quaternion.identity) as GameObject;

			// child spawned objects
			iceball.transform.parent = _iceballParent.transform;

			// localeScale.x of the iceball and the dragon should be opposite (becaue the default ones are opposite)
			Vector3 iceballLocalScale = iceball.transform.localScale;
			iceballLocalScale.x = -transform.localScale.x;
			iceball.transform.localScale = iceballLocalScale;

			// speed and direction are controlled by this script (ignoring the speed setting of iceball)
			float speedX = (_iceballSpeedX[i] + Random.Range(-1f, 1f)) * transform.localScale.x;
			float speedY = _iceballSpeedY [i] + Random.Range (-1f, 1f);
			iceball.GetComponent<Rigidbody2D> ().velocity = new Vector2 (speedX, speedY);

			// commented out 2016/8/31, for the actual effect is worse than with no-rotation
			// set rotation
			//float angle = Mathf.Atan2 (-speedX, -speedY) * Mathf.Rad2Deg;
			//iceball.transform.rotation = Quaternion.Euler(0, 0, angle);
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

				// start coroutine to stand up eventually
				StartCoroutine (Stand ());

				// fury time!
				if (_isFury == false && StunsToDie <= StunsToFury)
					_isFury = true;
			}
		}
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
	}

	// Die function (overriding base.Die())
	public override void Die ()
	{
		// disable health bar
		_healthBar.gameObject.SetActive (false);

		// stop everything
		_nextFlyTime = Time.time + 100f;		// bug fixing, must delay next flying time
		_animator.SetBool("Flying", false);		// stop moving
		_nextFireTime = Time.time + 100f;		// no method to stop firing, so simply just delay it

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
		Instantiate(DragonKnightPrefab, _diePos, Quaternion.identity);

		// instantiate victory obj
		_diePos.x += 0.7f;	// make it next to the stunned knight
		Instantiate(VictoryPrefab, _diePos, Quaternion.identity);

		// kill itseft
		Destroy(gameObject);
	}
	#endregion
}
