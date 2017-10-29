using UnityEngine;
using System.Collections;

// This is the class for aimed shoot (targeting the player) behavior
public class EnemyAimShoot : EnemyShoot {

	#region protected vars
	protected Transform _playerTransform;
	#endregion

	#region Unity funcs
	// Use this for initialization (overriding base.Awake())
	protected override void Awake () {
		// Call base func
		base.Awake ();

		_playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
		if (_playerTransform == null)
			Debug.LogError(name + ": Can not find Player! Nothing to aim to!");
	}
	#endregion

	#region protected funcs
	// override the original straightforward-shooting method, ignore its name which should be AimedShoot for perfection
	protected override void ThrowSpear () {

		// need to know the direction (1 - facing right, -1 - facing left)
		float direction;
		if (_playerTransform.position.x - transform.position.x > 0) {
			direction = 1f;
		} else {
			direction = -1f;
		}

		// this can prevent shooting backward (in other words, only shoot when facing the player)
		if (transform.localScale.x != direction) {
			return;
		}

		// play SFX
		if (throwSFX != null)
			playSound (throwSFX);

		// adjust instantiate position for better display effect
		Vector2 position = transform.position;
		position.x += 0.5f * transform.localScale.x;
		position.y += 0.1f;

		// instantiate projectile
		GameObject projectile = Instantiate (FlyingSpearPrefab, position, Quaternion.identity) as GameObject;
		// flip the image if necessary (facing left)
		Vector3 localScale = projectile.transform.localScale;
		localScale.x *= direction;
		projectile.transform.localScale = localScale;
		// child spawned objects
		projectile.transform.parent = _flyingSpearsParent.transform;
		// set direction and speed
		projectile.GetComponent<Rigidbody2D>().velocity = Vector3.Normalize(_playerTransform.position - new Vector3(position.x, position.y)) * SnakeMissile.speed;
	}
	#endregion
}
