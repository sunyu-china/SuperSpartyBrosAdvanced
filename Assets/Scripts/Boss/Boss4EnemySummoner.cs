using UnityEngine;
using System.Collections;

public class Boss4EnemySummoner : MonoBehaviour {

	// Summoned-enemies related (summon when fury)
	public GameObject RedKnightPrefab;
	public Transform RedKnightSummonPos;
	public float RedKnightSummonInterval = 5f;

	public GameObject GreenKnightPrefab;
	public Transform GreenKnightSummonPos;
	public float GreenKnightSummonInterval = 5f;

	public GameObject BlackKnightPrefab;
	public Transform BlackKnightSummonPos;
	public float BlackKnightSummonInterval = 5f;

	// SFX
	public AudioClip summonSFX;

	bool _existRedKnight;
	bool _existGreenKnight;
	bool _existBlackKnight;
	float _nextTimeToSummonRedKnight;
	float _nextTimeToSummonGreenKnight;
	float _nextTimeToSummonBlackKnight;

	void Awake () {
		if (RedKnightPrefab == null)
			Debug.LogError(name + ": RedKnightPrefab not set!");
		if (RedKnightSummonPos == null)
			Debug.LogError(name + ": RedKnightSummonPos not set!");

		if (GreenKnightPrefab == null)
			Debug.LogError(name + ": GreenKnightPrefab not set!");
		if (GreenKnightSummonPos == null)
			Debug.LogError(name + ": GreenKnightSummonPos not set!");

		if (BlackKnightPrefab == null)
			Debug.LogError(name + ": BlackKnightPrefab not set!");
		if (BlackKnightSummonPos == null)
			Debug.LogError(name + ": BlackKnightSummonPos not set!");

		// initialization
		Reset();
	}

	void Update () {
		// summon red knight
		if (_existRedKnight == false && Time.time > _nextTimeToSummonRedKnight) {
			Summon (RedKnightPrefab, RedKnightSummonPos.position);
			_existRedKnight = true;
		}

		// summon green knight
		if (_existGreenKnight == false && Time.time > _nextTimeToSummonGreenKnight) {
			Summon (GreenKnightPrefab, GreenKnightSummonPos.position);
			_existGreenKnight = true;
		}

		// summon black knight
		if (_existBlackKnight == false && Time.time > _nextTimeToSummonBlackKnight) {
			Summon (BlackKnightPrefab, BlackKnightSummonPos.position);
			_existBlackKnight = true;
		}

		// check previously summoned enemies
		if (_existRedKnight == true) {
			EnemyScout redKnight = GetComponentInChildren<EnemyScout> ();
			if (redKnight == null) {	// red knight is gone!
				_nextTimeToSummonRedKnight = Time.time + RedKnightSummonInterval;
				_existRedKnight = false;
			}	// else do nothing
		}

		if (_existGreenKnight == true) {
			EnemyShoot greenKnight = GetComponentInChildren<EnemyShoot> ();
			if (greenKnight == null) {	// green knight is gone!
				_nextTimeToSummonGreenKnight = Time.time + GreenKnightSummonInterval;
				_existGreenKnight = false;
			}	// else do nothing
		}

		if (_existBlackKnight == true) {
			EnemyChaseShoot blackKnight = GetComponentInChildren<EnemyChaseShoot> ();
			if (blackKnight == null) {	// black knight is gone!
				_nextTimeToSummonBlackKnight = Time.time + BlackKnightSummonInterval;
				_existBlackKnight = false;
			}	// else do nothing
		}
	}

	void Summon (GameObject enemyPrefab, Vector3 position) {
		if (summonSFX != null) {
			AudioSource.PlayClipAtPoint(summonSFX, Camera.main.transform.position);
		}
		// instantiate enemy
		GameObject enemy = Instantiate (enemyPrefab, position, Quaternion.identity) as GameObject;
		// child spawned objects to this
		enemy.transform.parent = transform;
	}

	public void Reset () {
		// destroy existing summoned enemies
		EnemyScout redKnight = GetComponentInChildren<EnemyScout> ();
		if (redKnight != null) {
			redKnight.Die ();
		}

		EnemyShoot greenKnight = GetComponentInChildren<EnemyShoot> ();
		if (greenKnight != null) {
			greenKnight.Die ();
		}

		EnemyChaseShoot blackKnight = GetComponentInChildren<EnemyChaseShoot> ();
		if (blackKnight != null) {
			blackKnight.Die ();
		}

		// reset local vars
		_existRedKnight = false;
		_existGreenKnight = false;
		_existBlackKnight = false;
		_nextTimeToSummonRedKnight = 0;
		_nextTimeToSummonGreenKnight = 0;
		_nextTimeToSummonBlackKnight = 0;
	}
}
