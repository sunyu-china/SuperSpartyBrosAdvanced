using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class MenuButtonLoadLevel : MonoBehaviour {

	public void loadLevel(string leveltoLoad)
	{
		//Application.LoadLevel (leveltoLoad);
		SceneManager.LoadScene(leveltoLoad);
	}
}
