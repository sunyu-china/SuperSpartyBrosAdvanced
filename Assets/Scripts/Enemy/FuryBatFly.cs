using UnityEngine;
using System.Collections;

public class FuryBatFly : EnemyFly {

	#region public vars
	public GameObject FuryBatDumpPrefab;

	public float DumpPerSec = 1f;

	// SFXs
	public AudioClip dumpSFX;
	#endregion

	#region protected vars
	protected bool _canDump = false;
	protected GameObject _furyBatDumpParent;
	#endregion

	#region Unity funcs
	// Use this for initialization (overriding base.Awake())
	protected override void Awake () {
		// Call base func
		base.Awake ();

		if (FuryBatDumpPrefab == null) // if FuryBatDumpPrefab is missing
			Debug.LogError(name + ": FuryBatDumpPrefab not set!");

		// create a parent (for fury bat dumps) if necessary
		_furyBatDumpParent = GameObject.Find ("EnemyProjectiles");
		if (_furyBatDumpParent == null) {
			_furyBatDumpParent = new GameObject("EnemyProjectiles");
		}
	}

	// Update is called once per frame (overriding base.Update())
	protected override void Update () {
		// Call base func
		base.Update();

		// logic for dropping dumps
		if (_canDump == true && isStunned == false)
		{
			float probability = Time.deltaTime * DumpPerSec;

			if (probability >= 1f) {
				Debug.LogWarning (name + "Change rate capped by frame rate!");
			}

			if (Random.value < probability) {
				DropDump ();
			}
		}
	}

	protected void OnBecameInvisible () {
		// do not drop dumps when out of screen
		_canDump = false;
	}

	protected void OnBecameVisible () {
		// only drop dumps when in the screen
		_canDump = true;
	}
	#endregion

	#region protected funcs
	protected void DropDump () {

		// play SFX
		if (dumpSFX != null)
			playSound (dumpSFX);
		
		// need to know the direction (1 - facing right, -1 - facing left)
		float direction = transform.localScale.x;

		// adjust instantiate position for better display effect
		Vector2 position = transform.position;
		position.y -= 0.1f;

		// instantiate spear
		GameObject furyBatDump = Instantiate (FuryBatDumpPrefab, position, Quaternion.identity) as GameObject;
		// flip the image if necessary (facing left)
		Vector3 localScale = furyBatDump.transform.localScale;
		localScale.x *= direction;
		furyBatDump.transform.localScale = localScale;
		// child spawned objects
		furyBatDump.transform.parent = _furyBatDumpParent.transform;
		// do not need to set velocity, using gravity (physics) for non-kinematic objects
	}
	#endregion
}
