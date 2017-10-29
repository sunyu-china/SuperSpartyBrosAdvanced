using UnityEngine;
using System.Collections;

public class BatLair : MonoBehaviour {

	public GameObject GreenBat;

	EnemyMoveTowards _enemyMoveTowards;
	Animator _animator;

	// Use this for initialization
	void Awake () {
		if (GreenBat == null)
			Debug.LogError (name + ": GreenBat not set!");
		else {
			_enemyMoveTowards = GreenBat.GetComponent<EnemyMoveTowards> ();
			if (_enemyMoveTowards == null)
				Debug.LogError (name + ": GreenBat not correctly set!");
		}

		_animator = GetComponent<Animator>();
		if (_animator==null) // if Animator is missing
			Debug.LogError(name + ": Animator component missing from this gameobject");
	}
	
	public void Trigger (Transform target) {
		// play animation
		_animator.SetTrigger("Trigger");

		_enemyMoveTowards.MoveTarget = target;
		// leave the SetActive method to be called by animator
	}

	// called by animation
	void SetOut () {
		GreenBat.SetActive (true);
	}
}
