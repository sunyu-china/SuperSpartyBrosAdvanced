using UnityEngine;
using System.Collections;
using UnityEngine.UI;

[RequireComponent (typeof(Image))]
public class HealthBar : MonoBehaviour {

	public Sprite[] HealthSprites;

	private Image _image;
	private int _baseHealth;

	// Use this for initialization
	void Awake () {
		_image = GetComponent<Image> ();
		if (_image == null)
			Debug.LogError (name + ": can not find Image!");

		_baseHealth = HealthSprites.Length;
		if (_baseHealth <= 0)
			Debug.LogError (name + ": HealthSprites not set!");
	}

	// called after disable/reenable
	void OnEnable () {
		ChangeSprite (_baseHealth);
	}
	
	void ChangeSprite (int i) {
		if (i >= 1 && i <= _baseHealth) {	// valid num
			// need to change i to array index
			_image.sprite = HealthSprites[i-1];
		} else {
			Debug.LogError (name + ": invalid parameter i = " + i + " when calling ChangeSprite()!");
		}
	}

	public void UpdateHealthBar (float percentage) {
		int currentHealth = Mathf.RoundToInt(_baseHealth * percentage);

		ChangeSprite (currentHealth);
	}
}
