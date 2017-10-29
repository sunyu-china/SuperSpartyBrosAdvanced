using UnityEngine;
using System.Collections;
using UnityStandardAssets.CrossPlatformInput;

public class SpartyWizardController : CharacterController2D {

	#region public vars
	// Fireball prefab
	public GameObject fireballPrefab;

	// SFXs
	public AudioClip attackSFX;
	#endregion

	#region protected vars
	protected GameObject fireballParent;
	#endregion

	#region Unity funcs
	// Use this for initialization (overriding base.Awake())
	protected override void Awake () {
		// Call base func
		base.Awake();

		if (fireballPrefab == null)
			Debug.LogError(name + ": fireball prefab is missing!");

		// create a parent (for fireballs) if necessary
		fireballParent = GameObject.Find ("Fireballs");
		if (fireballParent == null) {
			fireballParent = new GameObject("Fireballs");
		}
	}

	// Update is called once per frame (overriding base.Update())
	protected override void Update () {
		// Call base func
		base.Update();

		// exit update if player cannot move or game is paused
		if (!playerCanMove || (Time.timeScale == 0f))
			return;

		// player attack
		if(CrossPlatformInputManager.GetButtonDown("Fire1"))
		{
			DoAttack ();
		}
	}
	#endregion

	#region protected funcs
	// make the player attack
	protected void DoAttack ()
	{
		// play the attack animation
		_animator.SetTrigger ("Attack");
		// play the attack sound
		PlaySound (attackSFX);
		// leave the fireball casting action to animation
	}

	// called by animation
	protected void CastFireball () {

		float direction = 0;
		// need to know Sparty is facing right or left
		if (GameObject.FindObjectOfType<CharacterController2D> ().IsFacingRight () == true) {
			direction = 1f;
		} else {
			direction = -1f;
		}

		// adjust instantiate position for better display effect
		Vector2 position = gameObject.transform.position;
		position.x += 0.5f * direction;
		position.y -= 0.1f;

		// instantiate fireball
		GameObject fireball = Instantiate (fireballPrefab, position, Quaternion.identity) as GameObject;
		// flip the fireball image if necessary (facing left)
		Vector3 localScale = fireball.transform.localScale;
		localScale.x *= direction;
		fireball.transform.localScale = localScale;
		// child spawned objects
		fireball.transform.parent = fireballParent.transform;
		// set velocity
		fireball.GetComponent<Rigidbody2D>().velocity = new Vector2(Fireball.speed * direction, 0);
	}
	#endregion
}
