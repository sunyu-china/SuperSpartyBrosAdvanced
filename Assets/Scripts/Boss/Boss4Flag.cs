using UnityEngine;
using System.Collections;
using UnityStandardAssets._2D;

public class Boss4Flag : BossFlag {

	#region protected vars
	protected Transform _cameraOldTargetTransform;
	protected Vector3 _cameraOldTargetPos;
	#endregion

	#region public funcs
	// Use this for initialization (overriding base.Awake())
	protected override void Awake () {
		if (CameraTarget == null) {
			Debug.LogError (name + ": CameraTarget not set!");
		}

		if (BossPrefab == null) {
			Debug.LogError (name + ": BossPrefab not set!");
		}

		if (BossPosTransform == null) {
			Debug.LogError (name + ": BossPosTransform not set!");
		}

		if (RespawnPosTransform == null) {
			Debug.LogError (name + ": RespawnPosTransform not set!");
		}

		_camera = GameObject.FindObjectOfType<Camera2DFollow> ();
		if (_camera == null) {
			Debug.LogError (name + ": can not find Camera2DFollow!");
		}

		// record the position of CameraTarget
		_cameraTargetPos = CameraTarget.position;

		// diff to Base: avoid calling Reset so that the camera is not resetting (special case for level 4)
		status = Status.UNFIRED;
	}

	// Update is called once per frame (overriding base.Update())
	protected override void Update () {
		if (status == Status.AUTOPLAY) {
			float deltaTime = Time.time - _autoPlayStartTime;
			if (deltaTime < AutoPlayTime) {
				// gracefully set the camera follower to CameraTarget in a linear fasion (diff to Base: not from the player but from the camera's old target)
				CameraTarget.position = Vector3.Lerp(_cameraOldTargetPos, _cameraTargetPos, deltaTime / AutoPlayTime);
				SetCameraToFollow (CameraTarget);
			} else {
				// the last time, set the camera follower
				CameraTarget.position = _cameraTargetPos;
				SetCameraToFollow (CameraTarget);

				// enable boss battle specialized game objects
				EnableObjects ();

				// instantiate the BOSS!
				Instantiate(BossPrefab, BossPosTransform.position, Quaternion.identity);

				// autoplay completes, mark status
				status = Status.FIRED;
			}
		}
	}

	// When triggered (overriding base.OnTriggerEnter2D())
	protected override void OnTriggerEnter2D (Collider2D collider) {

		if ((status == Status.UNFIRED) && (collider.tag == "Player")) {
			// set autoplay start time
			_autoPlayStartTime = Time.time;

			// get camera's current (old) target (diff to Base: instead of getting player's position)
			_cameraOldTargetTransform = _camera.target;
			if (_cameraOldTargetTransform == null) {
				Debug.LogError (name + ": camera's current follower is null!");
			}
			// set position
			_cameraOldTargetPos = _cameraOldTargetTransform.position;

			// update the checkpoint of the game, set to the pre-set respawn position
			if (GameManager.gm) { // update the spawn location through the game manager, if it is available
				GameManager.gm.SetSpawnLocation (RespawnPosTransform.position);
			}

			// mark status
			status = Status.AUTOPLAY;
		}
	}
	#endregion

	#region public funcs
	// override this function since the projectile type is different
	public override void Reset ()
	{
		// initialize vars
		status = Status.UNFIRED;
		_autoPlayStartTime = 0;

		// deactivate boss battle objects
		DisableObjects ();

		// reset camera (diff to Base: not to the player but to the camera's old target)
		if (_cameraOldTargetTransform != null) {
			SetCameraToFollow (_cameraOldTargetTransform);
		}

		// destroy boss (in the last battle)
		GameObject[] bosses = GameObject.FindGameObjectsWithTag("Boss");
		foreach (GameObject boss in bosses) {
			Destroy (boss);
		}

		// destroy boss's projectiles (note FlyingSpear is the base class)
		FlyingSpear[] taijiballs = GameObject.FindObjectsOfType<FlyingSpear>();
		foreach (FlyingSpear taijiball in taijiballs) {
			Destroy (taijiball.gameObject);
		}

		// destroy victory (in the last battle)
		GameObject victory = GameObject.FindGameObjectWithTag("Victory");
		if (victory != null) {
			Destroy (victory);
		}
	}
	#endregion
}
