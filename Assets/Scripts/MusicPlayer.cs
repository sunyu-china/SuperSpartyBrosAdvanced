using UnityEngine;
using System.Collections;

public class MusicPlayer : MonoBehaviour {

	public AudioClip myClip;

	private AudioSource _audio;

	// Use this for initialization
	void Start () {
		if (myClip == null)
			Debug.LogError(name + ": AudioClip is missing!");

		_audio = GetComponent<AudioSource> ();
		if (_audio==null) { // if AudioSource is missing
			Debug.LogWarning(name + ": AudioSource component missing from this gameobject. Adding one.");
			// let's just add the AudioSource component dynamically
			_audio = gameObject.AddComponent<AudioSource>();
		}
	}
	
	// play sound through the audiosource on the gameobject
	public void Play() {
		_audio.PlayOneShot(myClip);
	}
}
