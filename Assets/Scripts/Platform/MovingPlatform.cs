using UnityEngine;
using System.Collections;

public class MovingPlatform : MonoBehaviour {

	public GameObject platform; // reference to the platform to move

	public GameObject[] myWaypoints; // array of all the waypoints

	[Range(0.0f, 10.0f)] // create a slider in the editor and set limits on moveSpeed
	public float moveSpeed = 5f; // platform move speed
	public float waitAtWaypointTime = 1f; // how long to wait at a waypoint before _moving to next waypoint

	public bool loop = true; // should it loop through the waypoints

	// protected variables

	protected Transform _transform;
	protected int _myWaypointIndex = 0;		// used as index for My_Waypoints
	protected float _moveTime;
	protected bool _moving = true;

	// Use this for initialization
	protected virtual void Awake () {
		_transform = platform.transform;
		_moveTime = 0f;
		StartMoving ();
	}
	
	// game loop
	protected void Update () {
		// if beyond _moveTime, then start moving
		if (Time.time >= _moveTime) {
			Movement();
		}
	}

	protected void Movement() {
		// if there isn't anything in My_Waypoints
		if ((myWaypoints.Length != 0) && (_moving == true)) {

			// move towards waypoint
			_transform.position = Vector3.MoveTowards(_transform.position, myWaypoints[_myWaypointIndex].transform.position, moveSpeed * Time.deltaTime);

			// if the enemy is close enough to waypoint, make it's new target the next waypoint
			if(Vector3.Distance(myWaypoints[_myWaypointIndex].transform.position, _transform.position) <= 0) {
				_myWaypointIndex++;
				_moveTime = Time.time + waitAtWaypointTime;
			}
			
			// reset waypoint back to 0 for looping, otherwise flag not moving for not looping
			if(_myWaypointIndex >= myWaypoints.Length) {
				if (loop)
					_myWaypointIndex = 0;
				else
					StopMoving ();
			}
		}
	}

	public virtual void StopMoving () {
		_moving = false;
	}

	public virtual void StartMoving () {
		_moving = true;
	}
}
