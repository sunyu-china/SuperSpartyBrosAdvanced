using UnityEngine;
using System.Collections;

// Base Enemy Controller Class
public class Enemy : MonoBehaviour {

	#region public vars
	public int damageAmount = 1;

	[Tooltip("Child gameObject for detecting stun.")]
	public GameObject stunnedCheck; // what gameobject is the stunnedCheck

	public float stunnedTime = 3f;   // how long to stay in stunned
	
	public string stunnedLayer = "StunnedEnemy";  // name of the layer to put enemy on when stunned
	public string playerLayer = "Player";  // name of the layer to ignore collision with the stunnedLayer

	public GameObject deathExplosionPrefab;

	[HideInInspector]
	public bool isStunned = false;  // flag for isStunned

	// SFXs
	public AudioClip attackSFX;
	public AudioClip stunnedSFX;
	public AudioClip deathSFX;
	#endregion
	
	#region protected vars
	// store references to components on the gameObject
	protected Transform _transform;
	protected Rigidbody2D _rigidbody;
	protected Animator _animator;
	protected AudioSource _audio;
	
	// store the layer number the enemy is on (setup in Awake)
	protected int _enemyLayer;

	// store the layer number the enemy should be moved to when stunned
	protected int _stunnedLayer;
	#endregion

	#region Unity funcs
	protected virtual void Awake() {
		// get a reference to the components we are going to be changing and store a reference for efficiency purposes
		_transform = GetComponent<Transform> ();
		
		_rigidbody = GetComponent<Rigidbody2D> ();
		if (_rigidbody==null) // if Rigidbody is missing
			Debug.LogError(name + ": Rigidbody2D component missing from this gameobject");
		
		_animator = GetComponent<Animator>();
		if (_animator==null) // if Animator is missing
			Debug.LogError(name + ": Animator component missing from this gameobject");
		
		_audio = GetComponent<AudioSource> ();
		if (_audio==null) { // if AudioSource is missing
			Debug.LogWarning(name + ": AudioSource component missing from this gameobject. Adding one.");
			// let's just add the AudioSource component dynamically
			_audio = gameObject.AddComponent<AudioSource>();
		}

		if (stunnedCheck==null) {
			// change from LogError to Log to allow enemy types which can not be stunned!
			Debug.Log(name + ": does not have a stunnedCheck set up");
		}
		
		// determine the enemies specified layer
		_enemyLayer = gameObject.layer;

		// determine the stunned enemy layer number
		_stunnedLayer = LayerMask.NameToLayer(stunnedLayer);

		// make sure collision are off between the playerLayer and the stunnedLayer
		// which is where the enemy is placed while stunned
		Physics2D.IgnoreLayerCollision(LayerMask.NameToLayer(playerLayer), _stunnedLayer, true);
	}

	protected virtual void Update () {
		
	}
		
	protected virtual void OnTriggerEnter2D(Collider2D collider)
	{
		// attack player
		if ((collider.tag == "Player") && (isStunned == false))
		{
			CharacterController2D player = collider.gameObject.GetComponent<CharacterController2D>();
			if (player.playerCanMove == true) {
				// Make sure the enemy is facing the player on attack
				Flip(collider.transform.position.x - _transform.position.x);

				// attack sound
				playSound(attackSFX);

				// apply damage to the player
				player.ApplyDamage (damageAmount);
			}
		}
	}

	// if the Enemy collides with a MovingPlatform, then make it a child of that platform
	// so it will go for a ride on the MovingPlatform
	protected void OnCollisionEnter2D(Collision2D other)
	{
		if (other.gameObject.tag=="MovingPlatform")
		{
			this.transform.parent = other.transform;
		}
	}

	// if the enemy exits a collision with a moving platform, then unchild it
	protected void OnCollisionExit2D(Collision2D other)
	{
		if (other.gameObject.tag=="MovingPlatform")
		{
			this.transform.parent = null;
		}
	}
	#endregion

	#region protected funcs
	// flip the enemy to face torward the direction he is moving in
	protected void Flip(float _vx) {
		
		// get the current scale
		Vector3 localScale = _transform.localScale;
		
		if ((_vx>0f)&&(localScale.x<0f))
			localScale.x*=-1;
		else if ((_vx<0f)&&(localScale.x>0f))
			localScale.x*=-1;
		
		// update the scale
		_transform.localScale = localScale;
	}

	// play sound through the audiosource on the gameobject
	protected void playSound(AudioClip clip)
	{
		_audio.PlayOneShot(clip);
	}
	#endregion

	#region public funcs
	// setup the enemy to be stunned (virtual function that to be overridden by inheritance)
	public virtual void Stunned()
	{
		if (isStunned == false) 
		{
			isStunned = true;

			// provide the player with feedback that enemy is stunned
			playSound(stunnedSFX);
			_animator.SetTrigger("Stunned");

			// switch layer to stunned layer so no collisions with the player while stunned
			this.gameObject.layer = _stunnedLayer;
			stunnedCheck.layer = _stunnedLayer;

			// start coroutine to stand up eventually
			StartCoroutine (Stand ());
		}
	}
	
	// coroutine to unstun the enemy and stand back up (virtual function that to be overridden by inheritance)
	public virtual IEnumerator Stand()
	{
		yield return new WaitForSeconds(stunnedTime); 
		
		// no longer stunned
		isStunned = false;

		// switch layer back to regular layer for regular collisions with the player
		this.gameObject.layer = _enemyLayer;
		stunnedCheck.layer = _enemyLayer;
		
		// provide the player with feedback
		_animator.SetTrigger("Stand");
	}

	// setup the enemy to die
	public virtual void Die() {
		// play the death sfx
		//playSound(deathSFX);	// IMPORTANT: can not use this method because of the following destory logic
		AudioSource.PlayClipAtPoint(deathSFX, Camera.main.transform.position);

		// move the particle effect in front, in case it ejects inside the screen (in 3D env)
		Vector3 pos = transform.position;
		pos.z -= 5;
		// play the explosion animation
		Instantiate(deathExplosionPrefab, pos, Quaternion.identity);

		// do the kill
		Destroy(gameObject);
	}
	#endregion
}
