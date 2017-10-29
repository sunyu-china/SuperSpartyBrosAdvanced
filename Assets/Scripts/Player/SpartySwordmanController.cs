using UnityEngine;
using System.Collections;
using UnityStandardAssets.CrossPlatformInput;

public class SpartySwordmanController : CharacterController2D {

	#region public vars
	// FlyingSwords prefab
	public GameObject flyingSwordsPrefab;
	[Tooltip("Time in seconds to retrieve the thrown out swords")]
	public float retrieveTime = 0.5f;

	// SFXs
	public AudioClip attackSFX;
	public AudioClip retrievedSFX;
	#endregion

	#region protected vars
	protected GameObject flyingSwords;
	protected float attackTime;
	#endregion

	#region Unity funcs
	// Use this for initialization (overriding base.Awake())
	protected override void Awake () {
		// Call base func
		base.Awake();

		if (flyingSwordsPrefab == null)
			Debug.LogError(name + ": flying swords prefab is missing!");

		attackTime = 0;
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
			if (Time.time > attackTime) {		// bug fixing (disappearing swords)
				DoAttack ();
			}
		}
	}
	#endregion

	#region protected funcs
	// make the player attack
	protected void DoAttack ()
	{
		// play the attack animation
		_animator.SetBool ("Attacking", true);
		// leave the swords throwing action to animation
	}

	// called by animation
	protected void ThrowSwords () {

		// play the attack sound
		PlaySound (attackSFX);

		float direction = 0;
		// need to know Sparty is facing right or left
		if (GameObject.FindObjectOfType<CharacterController2D> ().IsFacingRight () == true) {
			direction = 1f;
		} else {
			direction = -1f;
		}

		// adjust instantiate position for better display effect
		Vector2 position = gameObject.transform.position;
		position.x += 0.2f * direction;
		position.y -= 0.1f;

		// instantiate flying swords
		flyingSwords = Instantiate (flyingSwordsPrefab, position, Quaternion.identity) as GameObject;
		// flip the fireball image if necessary (facing left)
		Vector3 localScale = flyingSwords.transform.localScale;
		localScale.x *= direction;
		flyingSwords.transform.localScale = localScale;
		// set velocity
		flyingSwords.GetComponent<Rigidbody2D>().velocity = new Vector2(FlyingSwords.speed * direction, 0);

		// coroutine to retrieve the flying out swords
		StartCoroutine(RetrieveSwords());
	}

	protected IEnumerator RetrieveSwords () {
		yield return new WaitForSeconds (retrieveTime);

		if (flyingSwords != null) {
			flyingSwords.GetComponent<FlyingSwords> ().StartFlyBack ();
		}
	}
	#endregion

	#region public funcs
	// public function called by flyingswords when the swords get back to sparty
	public void Retrieved () {
		// set the animation flag
		_animator.SetBool ("Attacking", false);
		// bug fixing (disappearing swords)
		attackTime = Time.time + 0.1f;
		// play the retrieved sound
		PlaySound (retrievedSFX);
	}
	#endregion
}
