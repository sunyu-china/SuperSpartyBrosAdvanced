using UnityEngine;
using System.Collections;

// this class is the combination of EnemyMoveTowards and EnemyShoot
public class EnemyChaseShoot : EnemyMoveTowards {

	#region public vars
	public GameObject FlyingSwordPrefab;

	public float ThrowPerSec = 0.5f;

	// SFXs
	public AudioClip throwSFX;
	#endregion

	#region protected vars
	protected bool _canThrow = false;
	protected GameObject _flyingSwordParent;
	#endregion

	#region Unity funcs
	// Use this for initialization (overriding base.Awake())
	protected override void Awake () {
		// Call base func
		base.Awake ();

		if (FlyingSwordPrefab == null) // if FlyingSwordPrefab is missing
			Debug.LogError(name + ": FlyingSwordPrefab not set!");

		// create a parent (for flying spears) if necessary
		_flyingSwordParent = GameObject.Find ("EnemyProjectiles");
		if (_flyingSwordParent == null) {
			_flyingSwordParent = new GameObject("EnemyProjectiles");
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
				ThrowSword ();
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
	protected void ThrowSword () {

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
		GameObject flyingSword = Instantiate (FlyingSwordPrefab, position, Quaternion.identity) as GameObject;
		// flip the image if necessary (facing left)
		Vector3 localScale = flyingSword.transform.localScale;
		localScale.x *= direction;
		flyingSword.transform.localScale = localScale;
		// child spawned objects
		flyingSword.transform.parent = _flyingSwordParent.transform;
		// set velocity
		flyingSword.GetComponent<Rigidbody2D>().velocity = new Vector2(EnemyFlyingSword.speed * direction, 0);
	}
	#endregion
}
