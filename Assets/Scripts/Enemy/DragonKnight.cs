using UnityEngine;
using System.Collections;

public class DragonKnight : Enemy {

	#region public vars
	public GameObject RollingBarrelPrefab;
	public Transform ThrowPos;

	public float ThrowPerSec = 1f;
	#endregion

	#region private vars
	bool _isThrowing;
	GameObject _rollingBarrelParent;
	#endregion

	#region Unity funcs
	// Use this for initialization (overriding base.Awake())
	protected override void Awake () {
		// Call base func
		base.Awake();

		if (RollingBarrelPrefab == null)
			Debug.LogError(name + ": RollingBarrelPrefab not set!");

		if (ThrowPos == null)
			Debug.LogError(name + ": ThrowPos not set!");
		
		// create a parent (for bombmans) if necessary
		_rollingBarrelParent = GameObject.Find ("EnemyProjectiles");
		if (_rollingBarrelParent == null) {
			_rollingBarrelParent = new GameObject("EnemyProjectiles");
		}

		// initialize
		StopThrowing();
	}

	// Update is called once per frame (overriding base.Update())
	protected override void Update () {
		// Call base func
		base.Update();

		// throw rolling barrels
		if (_isThrowing == true && isStunned == false)
		{
			ThrowBarrels ();
		}
	}
	#endregion

	#region private funcs
	void ThrowBarrels ()
	{
		float probability = Time.deltaTime * ThrowPerSec;

		if (probability >= 1f) {
			Debug.LogWarning (name + "Change rate capped by frame rate!");
		}

		if (Random.value < probability) {
			GameObject rollingBarrel = Instantiate (RollingBarrelPrefab, ThrowPos.position, Quaternion.identity) as GameObject;
			// child spawned objects
			rollingBarrel.transform.parent = _rollingBarrelParent.transform;
			// set the velocity
			rollingBarrel.GetComponent<Rigidbody2D> ().AddForce (new Vector2 (Random.Range (-500f, -50f), Random.Range (0f, 50f)));
		}
	}

	void StopThrowing () {
		_isThrowing = false;
	}
	#endregion

	#region public funcs
	public void StartThrowing () {
		_isThrowing = true;
	}
	#endregion
}
