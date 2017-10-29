using UnityEngine;
using System.Collections;

// This was previously named as EnemyMovePikeman, but later realized as the base class for general shoot (straightforward) behavior
public class EnemyMoveShoot : EnemyMove {

	#region public vars
	public GameObject FlyingSpearPrefab;

	public float ThrowPerSec = 0.5f;

	// SFXs
	public AudioClip throwSFX;
	#endregion

	#region protected vars
	protected bool _canThrow = false;
	protected GameObject _flyingSpearsParent;
	#endregion

	#region Unity funcs
	// Use this for initialization (overriding base.Awake())
	protected override void Awake () {
		// Call base func
		base.Awake ();

		if (FlyingSpearPrefab == null) // if FlyingSpearPrefab is missing
			Debug.LogError(name + ": FlyingSpearPrefab not set!");

		// create a parent (for flying spears) if necessary
		_flyingSpearsParent = GameObject.Find ("EnemyProjectiles");
		if (_flyingSpearsParent == null) {
			_flyingSpearsParent = new GameObject("EnemyProjectiles");
		}
	}

	// Update is called once per frame (overriding base.Update())
	protected override void Update () {
		// Call base func
		base.Update();

		// logic for throwing spears
		if (_canThrow == true && isStunned == false)
		{
			float probability = Time.deltaTime * ThrowPerSec;

			if (probability >= 1f) {
				Debug.LogWarning (name + "Change rate capped by frame rate!");
			}

			if (Random.value < probability) {
				ThrowSpear ();
			}
		}
	}

	protected void OnBecameInvisible () {
		// do not throw spears when out of screen
		_canThrow = false;
	}

	protected void OnBecameVisible () {
		// only throw spears when in the screen
		_canThrow = true;
	}
	#endregion

	#region protected funcs
	protected virtual void ThrowSpear () {

		// play SFX
		if (throwSFX != null)
			playSound (throwSFX);
		
		// need to know the direction (1 - facing right, -1 - facing left)
		float direction = transform.localScale.x;

		// adjust instantiate position for better display effect
		Vector2 position = transform.position;
		position.x += 0.5f * direction;
		position.y -= 0.1f;

		// instantiate spear
		GameObject flyingSpear = Instantiate (FlyingSpearPrefab, position, Quaternion.identity) as GameObject;
		// flip the image if necessary (facing left)
		Vector3 localScale = flyingSpear.transform.localScale;
		localScale.x *= direction;
		flyingSpear.transform.localScale = localScale;
		// child spawned objects
		flyingSpear.transform.parent = _flyingSpearsParent.transform;
		// set velocity
		flyingSpear.GetComponent<Rigidbody2D>().velocity = new Vector2(FlyingSpear.speed * direction, 0);
	}
	#endregion
}
