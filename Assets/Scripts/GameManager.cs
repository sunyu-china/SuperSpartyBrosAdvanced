using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using UnityEngine.UI; // include UI namespace so can reference UI elements

public class GameManager : MonoBehaviour {

	// static reference to game manager so can be called from other scripts directly (not just through gameobject component)
	public static GameManager gm;

	// levels to move to on victory and lose, and start menu
	public string levelAfterVictory;
	public string levelAfterGameOver;
	public string startMenu;

	// game performance
	public int score = 0;
	public int highscore = 0;
	public int startLives = 3;
	public int lives = 3;
	public int startHealth = 5;
	public int health = 5;
	public int maxHealth = 5;


	// UI elements to control
	public Text UIScore;
	public Text UIHighScore;
	public Text UILevel;
	public Text UIExtraLives;
	public GameObject[] UIHealth;
	public GameObject UIGamePaused;

	// private variables
	CharacterController2D _player;
	Vector3 _spawnLocation;

	// set things up here
	void Awake () {
		// setup reference to game manager
		if (gm == null)
			gm = this.GetComponent<GameManager>();

		// setup all the variables, the UI, and provide errors if things not setup properly.
		setupDefaults();
	}

	// game loop
	void Update() {
		// if ESC pressed then pause the game
		if (Input.GetKeyDown(KeyCode.Escape)) {
			if (Time.timeScale > 0f) {
				Pause (); 
			} else {
				Resume (); 
			}
		}
	}

	void Pause ()
	{
		UIGamePaused.SetActive (true);
		// this brings up the pause UI
		Time.timeScale = 0f;
		// this pauses the game action
	}

	// setup all the variables, the UI, and provide errors if things not setup properly.
	void setupDefaults() {
		// setup reference to player
		if (_player == null)
			_player = GameObject.FindObjectOfType<CharacterController2D> ();
		
		if (_player==null)
			Debug.LogError("Player not found in Game Manager");
		
		// get initial _spawnLocation based on initial position of player
		_spawnLocation = _player.transform.position;

		// if levels not specified, default to current level
		if (levelAfterVictory=="") {
			Debug.LogWarning("levelAfterVictory not specified, defaulted to current level");
			//levelAfterVictory = Application.loadedLevelName;
			levelAfterVictory = SceneManager.GetActiveScene().name;
		}
		
		if (levelAfterGameOver=="") {
			Debug.LogWarning("levelAfterGameOver not specified, defaulted to current level");
			//levelAfterGameOver = Application.loadedLevelName;
			levelAfterGameOver = SceneManager.GetActiveScene().name;
		}

		if (startMenu=="") {
			Debug.LogError("startMenu not specified!");
		}

		// friendly error messages
		if (UIScore==null)
			Debug.LogError ("Need to set UIScore on Game Manager.");
		
		if (UIHighScore==null)
			Debug.LogError ("Need to set UIHighScore on Game Manager.");
		
		if (UILevel==null)
			Debug.LogError ("Need to set UILevel on Game Manager.");

		if (UIExtraLives==null)
			Debug.LogError ("Need to set UIExtraLives on Game Manager.");
		
		if (UIGamePaused==null)
			Debug.LogError ("Need to set UIGamePaused on Game Manager.");
		
		// get stored player prefs
		refreshPlayerState();

		// get the UI ready for the game
		refreshGUI();
	}

	// get stored Player Prefs if they exist, otherwise go with defaults set on gameObject
	void refreshPlayerState() {
		lives = PlayerPrefManager.GetLives();

		// special case if lives <= 0 then must be testing in editor, so reset the player prefs
		if (lives <= 0) {
			PlayerPrefManager.ResetPlayerState(startLives, startHealth, false);
			lives = PlayerPrefManager.GetLives();
		}

		maxHealth = PlayerPrefManager.GetMaxHealth ();
		RestoreHealth ();	// set health to maxHealth

		score = PlayerPrefManager.GetScore();
		highscore = PlayerPrefManager.GetHighscore();

		// save that this level has been accessed so the MainMenu can enable it
		PlayerPrefManager.UnlockLevel();
	}

	// refresh all the GUI elements
	void refreshGUI() {
		// set the text elements of the UI
		UIScore.text = "Score: "+score.ToString();
		UIHighScore.text = "Highscore: "+highscore.ToString ();
		//UILevel.text = Application.loadedLevelName; // obsolete method
		UILevel.text = SceneManager.GetActiveScene().name;
		UIExtraLives.text = "x " + (lives-1);
		
		// display health and max health in UI
		for(int i=0;i<UIHealth.Length;i++) {
			if (i < maxHealth) { // turn on the appropriate number of health indicators in the UI based on the max health
				UIHealth[i].SetActive(true);

				if (i < health) {
					UIHealth [i].GetComponent<Image> ().color = Color.white;
				} else {
					UIHealth [i].GetComponent<Image> ().color = Color.black;
				}
			} else {
				UIHealth[i].SetActive(false);
			}
		}
	}

	// public function to add points and update the gui and highscore player prefs accordingly
	public void AddPoints(int amount)
	{
		// increase score
		score+=amount;

		// update UI
		UIScore.text = "Score: "+score.ToString();

		// if score>highscore then update the highscore UI too
		if (score>highscore) {
			highscore = score;
			UIHighScore.text = "Highscore: "+score.ToString();
		}
	}

	// public function to remove player life and reset game accordingly
	public void ResetGame() {
		// remove life and update GUI
		lives--;
		refreshGUI();

		if (lives<=0) { // no more lives
			// save the current player prefs before going to GameOver
			PlayerPrefManager.SavePlayerState(score, highscore, lives, maxHealth);

			// load the gameOver screen
			//Application.LoadLevel (levelAfterGameOver); // obsolete method
			SceneManager.LoadScene(levelAfterGameOver);
		} else {
			BossFlag bossFlag = GameObject.FindObjectOfType<BossFlag> ();
			if (bossFlag != null) {	// if boss exists, reset it
				bossFlag.Reset();
			}

			// tell the player to respawn
			_player.Respawn(_spawnLocation);

			// restore to max health
			RestoreHealth();
		}
	}

	// public function for level complete
	public void LevelCompete() {
		// save the current player prefs before moving to the next level
		PlayerPrefManager.SavePlayerState(score, highscore, lives, maxHealth);

		// use a coroutine to allow the player to get fanfare before moving to next level
		StartCoroutine(LoadNextLevel());
	}

	// load the nextLevel after delay
	IEnumerator LoadNextLevel() {
		yield return new WaitForSeconds(3.5f); 
		//Application.LoadLevel (levelAfterVictory); // obsolete method
		SceneManager.LoadScene(levelAfterVictory);
	}

	// public function to add an extra player life
	public void AddLife() {
		// add life and update GUI
		lives++;
		refreshGUI ();
	}

	// public function to deal damage to player's health
	public void DealDamage (int damage) {
		// reduce health and update GUI
		health -= damage;
		refreshGUI ();
	}

	// public function to add player's health
	public void AddHealth (int recovery) {
		// add health and update GUI
		health = Mathf.Clamp(health + recovery, 0, maxHealth);
		refreshGUI ();
	}

	// public function to restore player's max health
	public void RestoreHealth () {
		// restore health and update GUI
		health = maxHealth;
		refreshGUI ();
	}

	// public function to increase player's max health by one
	public void AddMaxHealth() {
		// add max health and update GUI
		maxHealth++;
		refreshGUI ();
	}

	// public function to (re)set the spawn location
	public void SetSpawnLocation(Vector3 position) {
		_spawnLocation = position;
	}

	// public function to resume the game from pause
	public void Resume ()
	{
		Time.timeScale = 1f;
		// this unpauses the game action (ie. back to normal)
		UIGamePaused.SetActive (false);
		// remove the pause UI
	}

	// public function to return to the start menu
	public void BackToStartMenu () {
		// need to resume from pause first
		Resume();

		// initialize player state
		PlayerPrefManager.ResetPlayerState(startLives, startHealth, false);

		// load the scene
		SceneManager.LoadScene(startMenu);
	}
}
