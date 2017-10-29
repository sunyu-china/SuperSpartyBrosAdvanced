using UnityEngine;
using System.Collections;

[RequireComponent (typeof(Animator))]
public class Star : MonoBehaviour {

	public float meanChangeTime = 5f;

	private float changePerSecond;

	// Use this for initialization
	void Awake () {
		changePerSecond = 1 / meanChangeTime;
	}
	
	// Update is called once per frame
	void Update () {
		if (Time.deltaTime > meanChangeTime) {
			Debug.LogWarning (name + "Change rate capped by frame rate!");
		}

		float threshold = changePerSecond * Time.deltaTime;
		if (Random.value < threshold)
			GetComponent<Animator> ().SetTrigger ("Change");
	}
}
