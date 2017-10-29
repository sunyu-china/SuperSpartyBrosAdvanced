using UnityEngine;
using System.Collections;
using UnityStandardAssets._2D;

public class Boss2Flag : BossFlag {

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

		// avoid calling Reset so that the camera is not resetting (special case for level 2)
		status = Status.UNFIRED;
	}
}
