using UnityEngine;
using System.Collections;

public class FlyingSwords : MonoBehaviour {

	public enum Status {FLYOUT, FLYBACK}
	public Status status;

	[Tooltip("moving speed in the x-axis")]
	public static float speed = 10f;

	private SpartySwordmanController sparty;

	// Use this for initialization
	void Awake () {
		status = Status.FLYOUT;

		sparty = GameObject.FindObjectOfType<SpartySwordmanController> ();
		if (sparty == null) {
			Debug.LogError(name + ": Can not find Sparty!");
		}
	}

	// Update is called once per frame
	void Update () {
		// when swords flying back to sparty
		if (status == Status.FLYBACK) {
			FlyBack ();
		}
	}

	void FlyBack ()
	{
		// move to sparty's position
		Vector3 current = transform.position;
		Vector3 target = sparty.transform.position;
		float step = speed * Time.deltaTime;
		transform.position = Vector3.MoveTowards (current, target, step);

		// move complete
		if (Vector3.Distance(transform.position, target) <= step) {
			// tell sparty swords retrive completes
			sparty.Retrieved ();
			// destroy this
			Destroy (gameObject);
		}
	}

	void OnTriggerEnter2D (Collider2D collider) {
		Enemy enemy = collider.GetComponent<Enemy> ();
		if (enemy == null) {			// collider is not an enemy
			Breakable breakable = collider.GetComponent<Breakable> ();
			if (breakable != null) {	// destroy breakable objects
				breakable.Break ();

				// flying swords strike through "breakthrough" breakables
				if (collider.tag == "Breakthrough")
					return;
			}

			Interactive interactive = collider.GetComponent<Interactive>();
			if (interactive != null) {	// trigger interactive objects
				interactive.Trigger();
			}

			// flying swords rebound when hitting other stuff
			StartFlyBack ();
		} else {						// collider is an enemy
			if (collider.tag == "Boss") {	// for boss, call stunned to deal with damage
				enemy.Stunned();
			} else {						// for normal enemies, destory
				enemy.Die ();
			}

			// flying swords strike through enemies, so no flyback
		}
	}

	public void StartFlyBack () {
		if (status == Status.FLYOUT) {
			// clear velocity
			GetComponent<Rigidbody2D> ().velocity = Vector2.zero;
			// set status flag
			status = Status.FLYBACK;
		}
	}
}
