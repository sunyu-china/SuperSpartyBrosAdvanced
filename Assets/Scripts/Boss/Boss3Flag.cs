using UnityEngine;
using System.Collections;

public class Boss3Flag : BossFlag {

	#region public funcs
	// override this function since the projectile type is different
	public override void Reset ()
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

		// destroy boss's projectiles (note FlyingSpear is the base class)
		FlyingSpear[] iceballs = GameObject.FindObjectsOfType<FlyingSpear>();
		foreach (FlyingSpear iceball in iceballs) {
			Destroy (iceball.gameObject);
		}

		// destroy victory (in the last battle)
		GameObject victory = GameObject.FindGameObjectWithTag("Victory");
		if (victory != null) {
			Destroy (victory);
		}
	}
	#endregion
}
