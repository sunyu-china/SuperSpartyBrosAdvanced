using UnityEngine;
using System.Collections;

// this becomes the base class for enemy's chasing projectiles
public class TaiJiBallRed : FlyingSpear {

	// different default speed
	public static new float speed = 3f;

	protected Transform _playerTransform;
	protected Vector3 _playerPos;

	void Awake () {
		// get player
		_playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
		if (_playerTransform == null) {
			Debug.LogError (name + ": can not find Player!");
		}
	}

	void Update () {
		// move towards player's position
		Vector3 current = transform.position;
		Vector3 target = _playerTransform.position;
		float step = speed * Time.deltaTime;
		transform.position = Vector3.MoveTowards (current, target, step);
	}
}
