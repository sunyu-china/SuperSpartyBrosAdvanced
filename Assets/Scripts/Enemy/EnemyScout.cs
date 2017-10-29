using UnityEngine;
using System.Collections;

public class EnemyScout: EnemyMove {

	#region public funcs
	// be alert to the target transform (e.g. called when player enters EnemyScout's alert area)
	public void OnAlert (Transform target) {
		// determine distance between target and enemy
		_vx = target.position.x - _transform.position.x;

		// enemy should face the target
		Flip (_vx);

		// play the animation (Alert and Run)
		_animator.SetBool ("Moving", true);
		// leave the call to Move() action to animation
	}

	// clear the alert (e.g. called when player exits EnemyScout's alert area)
	public void OffAlert () {
		// stop the animation
		_animator.SetBool ("Moving", false);
		// stop
		Stop();
	}
	#endregion
}
