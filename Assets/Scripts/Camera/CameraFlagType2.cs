using UnityEngine;
using System.Collections;
using UnityStandardAssets._2D;

// type2 camera flag does the following camera actions:
// when player passes this flag, look ahead (gracefully transit Camera2DFollow's target to CameraTarget)
// (optional) then come back (set Camera2DFollow's target back to the player)
// Note: only work for static CameraTarget
public class CameraFlagType2 : MonoBehaviour {

	#region public vars
	[Tooltip ("Target to look ahead, must be an invisible transform")]
	public Transform CameraTarget;

	public enum Status {UNFIRED, AUTOPLAY, FIRED}
	public Status status;

	[Tooltip ("Time in seconds for the autoplay (look ahead transition)")]
	public float AutoPlayTime = 2f;

	[Tooltip ("Is camera going to reverse back to its original follower?")]
	public bool IsReversingBack = true;

	[Tooltip ("Time in seconds for staying at the look-ahead target (before reversing back)")]
	public float WaitTime = 1f;

	[Tooltip ("Do we need to update player's respawn location when passing this flag?")]
	public bool IsUpdatingRespawnLoc = false;

	[Tooltip ("Is this flag one-off?")]
	public bool IsOneOff = true;
	#endregion

	#region protected vars
	protected Camera2DFollow _camera;
	protected Vector3 _cameraTargetPos;
	protected Transform _playerTransform;
	protected Vector3 _playerPos;

	protected float _autoPlayStartTime = 0;
	#endregion

	#region Unity funcs
	// Use this for initialization
	protected void Awake () {
		if (CameraTarget == null) {
			Debug.LogError (name + ": CameraTarget not set!");
		}

		_camera = GameObject.FindObjectOfType<Camera2DFollow> ();
		if (_camera == null) {
			Debug.LogError (name + ": can not find Camera2DFollow!");
		}

		_playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
		if (_playerTransform == null) {
			Debug.LogError (name + ": can not find Player!");
		}

		// record the position of CameraTarget
		_cameraTargetPos = CameraTarget.position;

		// initialize
		Reset ();
	}

	// Update is called once per frame
	protected void Update () {
		if (status == Status.AUTOPLAY) {
			float deltaTime = Time.time - _autoPlayStartTime;
			if (deltaTime < AutoPlayTime) {
				// gracefully set the camera follower to CameraTarget in a linear fasion
				CameraTarget.position = Vector3.Lerp(_playerPos, _cameraTargetPos, deltaTime / AutoPlayTime);
				SetCameraToFollow (CameraTarget);
			} else {
				// the last time, set the camera follower
				CameraTarget.position = _cameraTargetPos;
				SetCameraToFollow (CameraTarget);

				if (IsReversingBack == true) {
					// start coroutine to reverse back the camera to the player
					StartCoroutine (ReverseBackCamera ());
				} else {
					CompleteStatus ();
				}
			}
		}
	}

	protected void OnTriggerEnter2D (Collider2D collider) {

		if ((status == Status.UNFIRED) && (collider.tag == "Player")) {
			// set autoplay start time
			_autoPlayStartTime = Time.time;

			// set player's position
			_playerPos = collider.transform.position;

			// update the respawn location to this flag (in order to avoid unexpected camera following effect after respawning) 
			if (IsUpdatingRespawnLoc == true && GameManager.gm != null) { // update the spawn location through the game manager, if it is available
				GameManager.gm.SetSpawnLocation (transform.position);
			}

			// mark status
			status = Status.AUTOPLAY;
		}
	}
	#endregion

	#region protected funcs
	protected void SetCameraToFollow (Transform transform) {
		_camera.target = transform;
	}

	// coroutine to reverse back the camera to the player
	protected IEnumerator ReverseBackCamera () {
		yield return new WaitForSeconds(WaitTime); 

		SetCameraToFollow (_playerTransform);

		CompleteStatus ();
	}

	void CompleteStatus ()
	{
		// mark status
		if (IsOneOff == true) {
			// one-off flag, mark fired
			status = Status.FIRED;
		}
		else {
			// not one-off, reset to fire again
			Reset ();
		}
	}
	#endregion

	#region public funcs
	public void Reset ()
	{
		// initialize vars
		status = Status.UNFIRED;
		_autoPlayStartTime = 0;
	}
	#endregion
}
