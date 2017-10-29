using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using UnityStandardAssets.CrossPlatformInput;

// Base Player Controller Class
public class CharacterController2D : MonoBehaviour {

	#region public vars
	// player controls
	[Range(0.0f, 10.0f)] // create a slider in the editor and set limits on moveSpeed
	public float moveSpeed = 3f;

	public float jumpForce = 600f;

	// LayerMask to determine what is considered ground for the player
	public LayerMask whatIsGround;

	// Transform just below feet for checking if player is grounded
	public Transform groundCheck;

	public float invincibleTime = 1f;   // how long to stay in invincible

	public string invincibleLayer = "InvinciblePlayer";  // name of the layer to put player on when invincible
	public string enemyLayer = "Enemy";  // name of the layer to ignore collision with the stunnedLayer

	// player can move?
	// we want this public so other scripts can access it but we don't want to show in editor as it might confuse designer
	[HideInInspector]
	public bool playerCanMove = true;

	// SFXs
	public AudioClip coinSFX;
	public AudioClip hurtSFX;
	public AudioClip deathSFX;
	public AudioClip fallSFX;
	public AudioClip jumpSFX;
	public AudioClip victorySFX;
	public AudioClip extraLifeSFX;
	public AudioClip flagSFX;
	public AudioClip healthPotionSFX;
	public AudioClip maxHealthSFX;
	#endregion

	#region protected vars
	// store references to components on the gameObject
	protected Transform _transform;
	protected Rigidbody2D _rigidbody;
	protected Animator _animator;
	protected AudioSource _audio;

	// hold player motion in this timestep
	protected float _vx;
	protected float _vy;

	// player tracking
	protected bool _facingRight = true;
	protected bool _isGrounded = false;
	protected bool _isRunning = false;
	protected bool _canDoubleJump = false;
	protected bool _isInvincible = false;

	// store the layer the player is on (setup in Awake)
	protected int _playerLayer;

	// store the layer number the player should be moved to when invincible
	protected int _invincibleLayer;

	// number of layer that Platforms are on (setup in Awake)
	protected int _platformLayer;
	#endregion

	#region Unity funcs
	protected virtual void Awake () {
		// get a reference to the components we are going to be changing and store a reference for efficiency purposes
		_transform = GetComponent<Transform> ();
		
		_rigidbody = GetComponent<Rigidbody2D> ();
		if (_rigidbody==null) // if Rigidbody is missing
			Debug.LogError("Rigidbody2D component missing from this gameobject");
		
		_animator = GetComponent<Animator>();
		if (_animator==null) // if Animator is missing
			Debug.LogError("Animator component missing from this gameobject");
		
		_audio = GetComponent<AudioSource> ();
		if (_audio==null) { // if AudioSource is missing
			Debug.LogWarning("AudioSource component missing from this gameobject. Adding one.");
			// let's just add the AudioSource component dynamically
			_audio = gameObject.AddComponent<AudioSource>();
		}

		// determine the player's specified layer
		_playerLayer = this.gameObject.layer;

		// determine the stunned enemy layer number
		_invincibleLayer = LayerMask.NameToLayer(invincibleLayer);

		// make sure collision are off between the enemyLayer and the invincibleLayer
		// which is where the player is placed while invincible
		Physics2D.IgnoreLayerCollision(LayerMask.NameToLayer(enemyLayer), _invincibleLayer, true);

		// determine the platform's specified layer
		_platformLayer = LayerMask.NameToLayer("Platform");
	}

	// this is where most of the player controller magic happens each game event loop
	protected virtual void Update()
	{
		// exit update if player cannot move or game is paused
		if (!playerCanMove || (Time.timeScale == 0f))
			return;

		// determine horizontal velocity change based on the horizontal input
		_vx = CrossPlatformInputManager.GetAxisRaw ("Horizontal");

		// Determine if running based on the horizontal movement
		if (_vx != 0) 
		{
			_isRunning = true;
		} else {
			_isRunning = false;
		}

		// set the running animation state
		_animator.SetBool("Running", _isRunning);

		// get the current vertical velocity from the rigidbody component
		_vy = _rigidbody.velocity.y;

		// Check to see if character is grounded by raycasting from the middle of the player
		// down to the groundCheck position and see if collected with gameobjects on the
		// whatIsGround layer
		_isGrounded = Physics2D.Linecast(_transform.position, groundCheck.position, whatIsGround);  

		// allow double jump after grounded
		if (_isGrounded) {
			_canDoubleJump = true;
		}

		// Set the grounded animation states
		_animator.SetBool("Grounded", _isGrounded);

		if (_isGrounded && CrossPlatformInputManager.GetButtonDown ("Jump")) { // If grounded AND jump button pressed, then allow the player to jump
			DoJump ();
		} else if (_canDoubleJump && CrossPlatformInputManager.GetButtonDown ("Jump")) { // if candoublejump and jump button pressed, then allow player to double jump
			DoJump ();
			// disable double jump after double jumping since you can only really do it once
			_canDoubleJump = false;
		}
	
		// If the player stops jumping mid jump and player is not yet falling
		// then set the vertical velocity to 0 (he will start to fall from gravity)
		if(CrossPlatformInputManager.GetButtonUp("Jump") && _vy>0f)
		{
			_vy = 0f;
		}

		// Change the actual velocity on the rigidbody
		_rigidbody.velocity = new Vector2(_vx * moveSpeed, _vy);

		// if moving up then don't collide with platform layer
		// this allows the player to jump up through things on the platform layer
		// NOTE: requires the platforms to be on a layer named "Platform"
		Physics2D.IgnoreLayerCollision(_playerLayer, _platformLayer, (_vy > 0.0f));
	}

	// Checking to see if the sprite should be flipped
	// this is done in LateUpdate since the Animator may override the localScale
	// this code will flip the player even if the animator is controlling scale
	protected void LateUpdate()
	{
		// get the current scale
		Vector3 localScale = _transform.localScale;

		if (_vx > 0) // moving right so face right
		{
			_facingRight = true;
		} else if (_vx < 0) { // moving left so face left
			_facingRight = false;
		}

		// check to see if scale x is right for the player
		// if not, multiple by -1 which is an easy way to flip a sprite
		if (((_facingRight) && (localScale.x<0)) || ((!_facingRight) && (localScale.x>0))) {
			localScale.x *= -1;
		}

		// update the scale
		_transform.localScale = localScale;
	}

	protected void OnCollisionEnter2D(Collision2D other)
	{
		// if the player collides with a MovingPlatform, then make it a child of that platform
		// so it will go for a ride on the MovingPlatform
		if (other.gameObject.tag=="MovingPlatform")
		{
			this.transform.parent = other.transform;
		}
	}

	protected void OnCollisionStay2D(Collision2D other)
	{
		// if the player collides with a DroppingPlatform
		if (other.gameObject.tag=="DroppingPlatform")
		{
			other.gameObject.GetComponentInParent<DroppingPlatform> ().Tremble ();
		}
	}
		
	protected void OnCollisionExit2D(Collision2D other)
	{
		// if the player exits a collision with a moving platform, then unchild it
		if (other.gameObject.tag=="MovingPlatform")
		{
			this.transform.parent = null;
		}
	}
	#endregion

	#region protected funcs
	// make the player jump
	protected void DoJump() {
		// reset current vertical motion to 0 prior to jump
		_vy = 0f;
		// add a force in the up direction
		_rigidbody.AddForce (new Vector2 (0, jumpForce));
		// play the jump sound
		PlaySound (jumpSFX);
	}

	// do what needs to be done to freeze the player
	protected void FreezeMotion() {
		playerCanMove = false;
		_rigidbody.isKinematic = true;
	}

	// do what needs to be done to unfreeze the player
	protected void UnFreezeMotion() {
		playerCanMove = true;
		_rigidbody.isKinematic = false;
	}

	// play sound through the audiosource on the gameobject
	protected void PlaySound(AudioClip clip)
	{
		_audio.PlayOneShot (clip);
	}

	// setup the player to be invincible
	protected void Hurt ()
	{
		if (_isInvincible == false) {
			_isInvincible = true;

			// switch layer to invincible layer so no collisions with the enemy
			gameObject.layer = _invincibleLayer;
			groundCheck.gameObject.layer = _invincibleLayer;

			// provide the player with feedback that it is hurt
			_animator.SetBool("Invincible", true);

			// Player jumps up a bit when getting hurt
			_rigidbody.AddForce (new Vector2 (0, 200f));

			// start coroutine to recover
			StartCoroutine (Recover ());
		}
	}

	// coroutine to recover the player back to normal
	protected IEnumerator Recover () {
		yield return new WaitForSeconds(invincibleTime); 

		// no longer invincible
		_isInvincible = false;

		// switch layer back to regular layer for regular collisions with enemies
		gameObject.layer = _playerLayer;
		groundCheck.gameObject.layer = _playerLayer;

		// provide the player with feedback
		_animator.SetBool("Invincible", false);
	}
	#endregion

	#region public funcs
	// public function to apply damage to the player
	public void ApplyDamage (int damage) {
		if (playerCanMove == true && _isInvincible == false) {
			GameManager.gm.DealDamage (damage);

			if (GameManager.gm.health <= 0) { // player is now dead, so start dying
				PlaySound (deathSFX);
				StartCoroutine (KillPlayer ());
			} else { // player is hurt, so become invincible for a second
				PlaySound(hurtSFX);
				Hurt ();
			}
		}
	}

	// public function to kill the player when they have a fall death
	public void FallDeath () {
		if (playerCanMove) {
			GameManager.gm.DealDamage (GameManager.gm.maxHealth);	// Make sure it depletes all health
			PlaySound(fallSFX);
			StartCoroutine (KillPlayer ());
		}
	}

	// coroutine to kill the player
	public IEnumerator KillPlayer()
	{
		if (playerCanMove)
		{
			// freeze the player
			FreezeMotion();

			// play the death animation
			_animator.SetTrigger("Death");
			
			// After waiting tell the GameManager to reset the game
			yield return new WaitForSeconds(2.0f);

			if (GameManager.gm) // if the gameManager is available, tell it to reset the game
				GameManager.gm.ResetGame();
			else // otherwise, just reload the current level
				//Application.LoadLevel(Application.loadedLevelName);
				SceneManager.LoadScene (SceneManager.GetActiveScene().buildIndex);
		}
	}

	public void CollectCoin(int amount) {
		PlaySound(coinSFX);

		if (GameManager.gm) // add the points through the game manager, if it is available
			GameManager.gm.AddPoints(amount);
	}

	// public function on victory over the level
	public void Victory() {
		PlaySound(victorySFX);
		FreezeMotion ();
		_animator.SetTrigger("Victory");

		if (GameManager.gm) // do the game manager level compete stuff, if it is available
			GameManager.gm.LevelCompete();
	}

	// public function to respawn the player at the appropriate location
	public void Respawn(Vector3 spawnloc) {
		UnFreezeMotion();
		_transform.parent = null;
		_transform.position = spawnloc;
		_animator.SetTrigger("Respawn");
	}

	// public function to make the player bounce off the enemy's head
	public void EnemyBounce() {
		DoJump ();
	}

	// public function to add an exta life for the player
	public void AddLife() {
		PlaySound(extraLifeSFX);

		if (GameManager.gm) // add extra life through the game manager, if it is available
			GameManager.gm.AddLife();
	}

	// public function to update the spawn location of the player when passing a flag checkpoint
	public void FlagPass(Vector3 position) {
		PlaySound(flagSFX);
		
		if (GameManager.gm) { // update the spawn location through the game manager, if it is available
			position.y += 2; // increase the y axis of the spawn location, so the player will fall down to flag checkpoint
			GameManager.gm.SetSpawnLocation (position);
		}
	}

	// public function to tell whether Sparty is facing right or not
	public bool IsFacingRight () {
		return _facingRight;
	}

	// public function to increase max health by 1 for the player
	public void AddMaxHealth() {
		PlaySound(maxHealthSFX);

		if (GameManager.gm) // increase max health through the game manager, if it is available
			GameManager.gm.AddMaxHealth();
	}

	// public function to add player's health
	public void AddHealth(int recovery) {
		PlaySound(healthPotionSFX);

		if (GameManager.gm) // increase max health through the game manager, if it is available
			GameManager.gm.AddHealth(recovery);
	}
	#endregion
}
