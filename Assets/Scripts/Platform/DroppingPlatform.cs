using UnityEngine;
using System.Collections;

public class DroppingPlatform : MonoBehaviour {

	public GameObject platformPrefab;	// platform prefab

	public string droppingPlatformTag = "DroppingPlatform";

	public GameObject platformDregPrefab;	// dreg (particle system) prefab

	[Range(0.0f, 10.0f)] // create a slider in the editor and set limits on dropSpeed
	public float dropSpeed = 5f; // platform drop speed
	public float waitBeforeDropTime = 1f; // how long before the platform starts to drop

	public bool respawn = true;	// respawn or not
	public float waitBeforeRespawnTime = 1f;	// how long before the platform respawns

	// private variables

	private GameObject _platform;	// platform instance

	private enum Status {IDLE, TREMBLE, FALL, DESTROYED}
	[SerializeField]
	private Status _status;

	private float _dropTime;	// when to drop
	private float _respawnTime;	// when to respawn

	// Use this for initialization
	void Awake () {
		Initiate ();
	}
	
	// Update is called once per frame
	void Update () {
		if (_status == Status.TREMBLE && Time.time >= _dropTime) {	// when trembling, count down to start falling
			// time to drop now
			_status = Status.FALL;

			Fall ();
		} else if (_status == Status.FALL && _platform == null) {	// when falling, monitor if the platform is destroyed
			// platform destroyed
			_status = Status.DESTROYED;

			// set drop time
			_respawnTime = Time.time + waitBeforeRespawnTime;
		} else if (_status == Status.DESTROYED && respawn == true && Time.time >= _respawnTime) {	// when destroyed and respawn enabled, count down to respawn
			// resapwn the platform
			Respawn();
		}
	}

	// Show it in Dev (Scene) mode
	void OnDrawGizmos () {
		Gizmos.DrawCube (transform.position, new Vector3 (2f, 0.6f, 0));
	}

	// (re)initiate local vars
	void Initiate () {
		if (_platform == null) {
			_platform = Instantiate (platformPrefab, transform.position, Quaternion.identity) as GameObject;
			// need to reset the tag
			_platform.tag = droppingPlatformTag;
			// and the parental relationship
			_platform.transform.parent = transform;
		}

		_status = Status.IDLE;
		_dropTime = 0;
		_respawnTime = 0;
	}

	// make the platform fall down
	void Fall () {
		// IMPORTANT: need to turn off kinematic, otherwise the platform does not collide with other stuff
		_platform.GetComponent<Rigidbody2D> ().isKinematic = false;
		_platform.GetComponent<Rigidbody2D> ().velocity = new Vector2 (0, -dropSpeed);
	}

	// respawn the platform
	void Respawn () {
		Initiate ();
	}

	// public function called when player lands on the platform
	public void Tremble () {
		if (_status == Status.IDLE) {
			_status = Status.TREMBLE;

			// play the particle system effect
			Instantiate(platformDregPrefab, transform.position, Quaternion.Euler(90, 0, 0));

			// set drop time
			_dropTime = Time.time + waitBeforeDropTime;
		}
	}
}
