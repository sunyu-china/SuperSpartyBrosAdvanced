using UnityEngine;
using System.Collections;

public class Level4Boss : Enemy {

	#region public vars
	[Tooltip ("Also works for hits!")]
	public int StunsToDie = 12;
	public int StunsToFury = 6;

	public float IdleDuration = 4f;

	public GameObject TaiJiBallYellowPrefab;
	public GameObject TaiJiBallRedPrefab;
	public Transform FirePos;

	public GameObject Boss4EnemySummoner;

	public GameObject VictoryPrefab;

	// SFXs
	public AudioClip thunderSFX;
	public AudioClip fireSFX;
	public AudioClip explosionSFX;
	#endregion

	#region private vars
	HealthBar _healthBar;
	int _maxStunsToDie;
	bool _isFury;
	float _nextFireTime;
	Vector3 _diePos;

	Transform _playerTransform;

	GameObject _taijiballParent;

	Boss4EnemySummoner _boss4EnemySummoner;
	#endregion

	#region Unity funcs
	// Use this for initialization (overriding base.Awake())
	protected override void Awake () {
		// Call base func
		base.Awake();

		if (TaiJiBallYellowPrefab == null)
			Debug.LogError(name + ": TaiJiBallYellowPrefab not set!");

		if (TaiJiBallRedPrefab == null)
			Debug.LogError(name + ": TaiJiBallRedPrefab not set!");

		if (FirePos == null)
			Debug.LogError(name + ": FirePos not set!");

		if (Boss4EnemySummoner == null)
			Debug.LogError (name + ": Boss4EnemySummoner not set!");
		else {
			_boss4EnemySummoner = Boss4EnemySummoner.GetComponent<Boss4EnemySummoner> ();
			if (_boss4EnemySummoner == null)
				Debug.LogError (name + ": Boss4EnemySummoner not correctly set!");
			// disable Boss4EnemySummoner
			Boss4EnemySummoner.SetActive(false);
		}

		if (VictoryPrefab == null)
			Debug.LogError (name + ": VictoryPrefab not set!");

		_healthBar = GameObject.FindObjectOfType<HealthBar> ();
		if (_healthBar == null)
			Debug.LogError(name + ": can not find Health Bar for the boss!");

		_playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
		if (_playerTransform == null)
			Debug.LogError(name + ": Can not find Player! Nothing to aim to!");

		// create a parent (for taijiballs) if necessary
		_taijiballParent = GameObject.Find ("BossProjectiles");
		if (_taijiballParent == null) {
			_taijiballParent = new GameObject("BossProjectiles");
		}

		// initialize private vars
		_isFury = false;
		_nextFireTime = Time.time + IdleDuration;
		_maxStunsToDie = StunsToDie;
	}

	// Update is called once per frame (overriding base.Update())
	protected override void Update () {
		// Call base func
		base.Update();

		if (Time.time >= _nextFireTime) {		// time to fire
			_nextFireTime = Time.time + 100f;	// bug fixing, prevent setting the flag multiple times
			// IMPORTANT: all calls to fire, cease-fire as well as next-time-to-fire logics are controlled by animation
			_animator.SetTrigger("Fire");
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
	void FireTaiJiBallYellow ()
	{
		// play SFX
		if (fireSFX != null)
			playSound (fireSFX);

		// instantiate projectile
		GameObject taijiball = Instantiate (TaiJiBallYellowPrefab, FirePos.position, Quaternion.identity) as GameObject;
		// child spawned objects
		taijiball.transform.parent = _taijiballParent.transform;
		// set direction and speed (TaiJiBallYellow aims to the player's current position)
		taijiball.GetComponent<Rigidbody2D>().velocity = Vector3.Normalize(_playerTransform.position - FirePos.position) * TaiJiBallYellow.speed;
	}

	void FireTaiJiBallRed ()
	{
		// play SFX
		if (fireSFX != null)
			playSound (fireSFX);

		// instantiate projectile
		GameObject taijiball = Instantiate (TaiJiBallRedPrefab, FirePos.position, Quaternion.identity) as GameObject;
		// child spawned objects
		taijiball.transform.parent = _taijiballParent.transform;
		// no need to set speed because TaiJiBallRed (controlled by its only script) chases the player

		// set next fire time
		_nextFireTime = Time.time + IdleDuration + Random.Range(-1f, 1f);
	}

	void AttackPlayer (Collider2D collider)
	{
		CharacterController2D player = collider.gameObject.GetComponent<CharacterController2D> ();
		if (player.playerCanMove == true) {
			// no need to flip (special case for boss4)
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
				if (_isFury == false && StunsToDie <= StunsToFury) {
					_isFury = true;
					// enable Boss4EnemySummoner
					Boss4EnemySummoner.SetActive (true);
				}
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
		_nextFireTime = Time.time + 100f;		// no method to stop firing, so simply just delay it
		// including Boss4EnemySummoner
		if (_isFury == true) {
			_boss4EnemySummoner.Reset ();
			Boss4EnemySummoner.SetActive (false);
		}

		// play death sfx (special case for Boss4, 2 sounds are played)
		playSound(deathSFX);
		playSound(explosionSFX);

		// record the current position for coroutine to use
		_diePos = transform.position;
		// move the particle effect in front, in case it ejects inside the screen (in 3D env)
		Vector3 pos = _diePos;
		pos.z -= 5f;
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

		// instantiate victory obj
		Instantiate(VictoryPrefab, _diePos, Quaternion.identity);

		// kill itseft
		Destroy(gameObject);
	}
	#endregion
}
