using UnityEngine;
using System.Collections;

public class Snowman : Breakable {

	public GameObject ExtraLifeWizard;

	// Use this for initialization
	void Awake () {
		if (ExtraLifeWizard == null)
			Debug.LogError(name + ": ExtraLifeWizard not set!");
	}
	
	// override the old Break method to handle special interaction
	public override void Break () {
		// Call base func
		base.Break();

		// destory the ExtraLife bonus
		Destroy(ExtraLifeWizard.gameObject);

		Die ();
	}

	// override the old Die method
	protected override void Die () {
		// disable the collider (never collide again), but leave the object (showing wreckage)
		GetComponent<BoxCollider2D> ().enabled = false;
	}
}
