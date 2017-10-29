using UnityEngine;
using System.Collections;
using UnityStandardAssets._2D;

// this has become the base class for boss battle flags
public class BossFlag : MonoBehaviour {

	#region public vars
	[Tooltip ("Target the camera to follow during boss battle")]
	public Transform CameraTarget;

	public enum Status {UNFIRED, AUTOPLAY, FIRED}
	public Status status;

	[Tooltip ("Time in seconds for the autoplay(cutscene) before boss battle")]
	public float AutoPlayTime = 2f;

	[Tooltip ("Objects only emerge(set active) in the boss battle (e.g. health bar, road blockers)")]
	public GameObject[] ObjectsToEmerge;

	public GameObject BossPrefab;
	[Tooltip ("Empty transform to hold the spawn position of the Boss")]
	public Transform BossPosTransform;
	[Tooltip ("Empty transform to hold the re-spawn position for the boss battle")]
	public Transform RespawnPosTransform;
	#endregion

	#region protected vars
	protected Camera2DFollow _camera;
	protected Vector3 _cameraTargetPos;
	protected Vector3 _playerPos;

	protected float _autoPlayStartTime = 0;
	#endregion

	#region Unity funcs
	// Use this for initialization
	protected virtual void Awake () {
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

		// initialize
		Reset ();
	}
	
	// Update is called once per frame
	protected virtual void Update () {
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

				// enable boss battle specialized game objects
				EnableObjects ();

				// instantiate the BOSS!
				Instantiate(BossPrefab, BossPosTransform.position, Quaternion.identity);

				// autoplay completes, mark status
				status = Status.FIRED;
			}
		}
	}

	protected virtual void OnTriggerEnter2D (Collider2D collider) {
		
		if ((status == Status.UNFIRED) && (collider.tag == "Player")) {
			// set autoplay start time
			_autoPlayStartTime = Time.time;

			// set player's position
			_playerPos = collider.transform.position;

			// update the checkpoint of the game, set to the pre-set respawn position
			if (GameManager.gm) { // update the spawn location through the game manager, if it is available
				GameManager.gm.SetSpawnLocation (RespawnPosTransform.position);
			}

			// mark status
			status = Status.AUTOPLAY;
		}
	}
	#endregion

	#region protected funcs
	protected void EnableObjects () {
		foreach (GameObject gameObj in ObjectsToEmerge)
			gameObj.SetActive (true);
	}

	protected void DisableObjects () {
		foreach (GameObject gameObj in ObjectsToEmerge)
			gameObj.SetActive (false);
	}

	protected void SetCameraToFollow (Transform transform) {
		_camera.target = transform;
	}
	#endregion

	#region public funcs
	public virtual void Reset ()
	{
		// initialize vars
		status = Status.UNFIRED;
		_autoPlayStartTime = 0;

		// deactivate boss battle objects
		DisableObjects ();

		// reset camera
		GameObject sparty = GameObject.FindGameObjectWithTag("Player");
		if (sparty != null) {
			SetCameraToFollow (sparty.transform);
		} else {
			Debug.LogError (name + ": can not find the player (Sparty), set camera to follow failed!");
		}

		// destroy boss (in the last battle)
		GameObject[] bosses = GameObject.FindGameObjectsWithTag("Boss");
		foreach (GameObject boss in bosses) {
			Destroy (boss);
		}

		// destroy boss's projectiles (note Rockfall is the base class)
		Rockfall[] rockfalls = GameObject.FindObjectsOfType<Rockfall>();
		foreach (Rockfall rockfall in rockfalls) {
			Destroy (rockfall.gameObject);
		}

		// destroy victory (in the last battle)
		GameObject victory = GameObject.FindGameObjectWithTag("Victory");
		if (victory != null) {
			Destroy (victory);
		}
	}
	#endregion
}
