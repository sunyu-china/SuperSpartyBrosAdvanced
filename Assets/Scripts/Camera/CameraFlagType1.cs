using UnityEngine;
using System.Collections;
using UnityStandardAssets._2D;

// type1 camera flag does the following camera actions:
// if player passes forward this flag, set Camera2DFollow's target to CameraTargetForward;
// if player passes backward this flag, set Camera2DFollow's target to CameraTargetBackward;
public class CameraFlagType1 : MonoBehaviour {

	#region public vars
	[Tooltip ("Target the camera to follow when passing forward this flag")]
	public Transform CameraTargetForward;
	[Tooltip ("Target the camera to follow when passing backward this flag")]
	public Transform CameraTargetBackward;

	public enum Status {UNFIRED, AUTOPLAY, FIRED}	// AUTOPLAY not used in this case because the cametra transition is done instantaneously
	public Status status;

	[Tooltip ("Do we need to update player's respawn location when passing this flag?")]
	public bool IsUpdatingRespawnLoc = false;

	[Tooltip ("Is this flag one-off?")]
	public bool IsOneOff = false;
	#endregion

	#region protected vars
	protected Camera2DFollow _camera;
	#endregion

	#region Unity funcs
	// Use this for initialization
	protected void Awake () {
		if (CameraTargetForward == null) {
			Debug.LogError (name + ": CameraTargetForward not set!");
		}

		if (CameraTargetBackward == null) {
			Debug.LogError (name + ": CameraTargetBackward not set!");
		}

		_camera = GameObject.FindObjectOfType<Camera2DFollow> ();
		if (_camera == null) {
			Debug.LogError (name + ": can not find Camera2DFollow!");
		}

		// initialize
		Reset ();
	}

	// trigger when player leaves
	protected void OnTriggerExit2D (Collider2D collider) {

		if ((status == Status.UNFIRED) && (collider.tag == "Player")) {
			Vector3 playerPos = collider.transform.position;
			if (playerPos.x >= transform.position.x) {	// pass the flag forward
				SetCameraToFollow (CameraTargetForward);
			} else {									// pass the flag backward
				SetCameraToFollow (CameraTargetBackward);
			}

			// update the respawn location to this flag (in order to avoid unexpected camera following effect after respawning) 
			if (IsUpdatingRespawnLoc == true && GameManager.gm != null) { // update the spawn location through the game manager, if it is available
				GameManager.gm.SetSpawnLocation (transform.position);
			}

			// mark status
			if (IsOneOff == true) {	// one-off flag, mark fired
				status = Status.FIRED;
			} else {				// not one-off, reset to fire again
				Reset ();
			}
		}
	}
	#endregion

	#region protected funcs
	protected void SetCameraToFollow (Transform transform) {
		_camera.target = transform;
	}

	public void Reset ()
	{
		// initialize vars
		status = Status.UNFIRED;
	}
	#endregion
}
