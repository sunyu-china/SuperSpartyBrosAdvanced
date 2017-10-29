using UnityEngine;
using System.Collections;

public class Level1Boss : EnemyMove {

	#region public vars
	public int StunsToDie = 6;
	public int StunsToStartRockfall = 3;

	public GameObject RockfallPrefab;
	public float RockfallPosXMin;
	public float RockfallPosXMax;
	public float RockfallPosY;
	public float RockfallPerSec = 0.5f;

	public GameObject DustEruption;

	public GameObject PilotPrefab;
	public GameObject VictoryPrefab;
	#endregion

	#region private vars
	HealthBar _healthBar;
	int _maxStunsToDie;
	bool _rockfallEnabled;
	Vector3 _rockfallPos;
	GameObject _rockfallParent;
	Vector3 _diePos;
	#endregion

	#region Unity funcs
	// Use this for initialization (overriding base.Awake())
	protected override void Awake () {
		// Call base func
		base.Awake();

		if (RockfallPrefab == null)
			Debug.LogError(name + ": RockfallPrefab not set!");

		if (DustEruption == null)
			Debug.LogError(name + ": DustEruption not set!");

		if (PilotPrefab == null)
			Debug.LogError(name + ": PilotPrefab not set!");
		
		if (VictoryPrefab == null)
			Debug.LogError(name + ": VictoryPrefab not set!");

		_healthBar = GameObject.FindObjectOfType<HealthBar> ();
		if (_healthBar == null)
			Debug.LogError(name + ": can not find Health Bar for the boss!");

		// create a parent (for rockfalls) if necessary
		_rockfallParent = GameObject.Find ("BossProjectiles");
		if (_rockfallParent == null) {
			_rockfallParent = new GameObject("BossProjectiles");
		}

		// initialize
		_rockfallPos.y = RockfallPosY;
		StopRockfall ();
		_maxStunsToDie = StunsToDie;
	}

	// Update is called once per frame (overriding base.Update())
	protected override void Update () {
		// Call base func
		base.Update();

		// rockfall
		if (_rockfallEnabled == true)
			DoRockfall ();
	}
	#endregion

	#region private funcs
	void StartRockfall () {
		_rockfallEnabled = true;

		DustEruption.SetActive (true);
	}

	void StopRockfall () {
		_rockfallEnabled = false;

		DustEruption.SetActive (false);
	}

	void DoRockfall ()
	{
		float probability = Time.deltaTime * RockfallPerSec;

		if (probability >= 1f) {
			Debug.LogWarning (name + "Change rate capped by frame rate!");
		}

		if (Random.value < probability) {
			_rockfallPos.x = Random.Range (RockfallPosXMin, RockfallPosXMax);
			GameObject rockfall = Instantiate (RockfallPrefab, _rockfallPos, Quaternion.identity) as GameObject;
			// child spawned objects
			rockfall.transform.parent = _rockfallParent.transform;
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

				// stop moving
				Stop ();

				// switch layer to stunned layer so no collisions with the player while stunned
				gameObject.layer = _stunnedLayer;
				stunnedCheck.layer = _stunnedLayer;

				// start coroutine to stand up eventually
				StartCoroutine (Stand ());

				if (_rockfallEnabled == false && StunsToDie <= StunsToStartRockfall)
					StartRockfall ();
			}
		}
	}

	// Die function for Boss1 (overriding base.Die())
	public override void Die ()
	{
		// disable health bar
		_healthBar.gameObject.SetActive (false);

		// stop everything
		Stop ();
		StopRockfall ();

		// play death sfx
		playSound(deathSFX);

		// record the current position for coroutine to use
		_diePos = transform.position;
		// move the particle effect in front, in case it ejects inside the screen (in 3D env)
		Vector3 pos = _diePos;
		pos.z -= 5;
		// play death animation
		Instantiate(deathExplosionPrefab, pos, Quaternion.identity);

		// start coroutine to stand up eventually
		StartCoroutine (Win ());
	}

	// coroutine to unstun the enemy and stand back up (virtual function that to be overridden by inheritance)
	public IEnumerator Win ()
	{
		yield return new WaitForSeconds(1f);

		// instantiate pilot
		Instantiate(PilotPrefab, _diePos, Quaternion.identity);

		// instantiate victory obj
		_diePos.x += 0.7f;	// make it next to the stunned pilot
		Instantiate(VictoryPrefab, _diePos, Quaternion.identity);

		// kill itseft
		Destroy(gameObject);
	}
	#endregion
}
