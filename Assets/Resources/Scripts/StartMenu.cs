using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;

public class StartMenu : MonoBehaviour {

	public Button startButton;
	public Button exitButton;
	public Button yesQuit;
	public Button noQuit;
	public Image exitMessage;

	// Use this for initialization
	void Awake() {

		//GetComponent<Canvas>().sortingLayerName = "Foreground";
		startButton = startButton.GetComponent<Button>();
		exitButton = exitButton.GetComponent<Button>();

		exitMessage = exitMessage.GetComponent<Image>();
		exitMessage.gameObject.SetActive (false);

		yesQuit = yesQuit.GetComponent<Button>();
		yesQuit.gameObject.SetActive (false);

		noQuit = noQuit.GetComponent<Button>();
		noQuit.gameObject.SetActive (false);


	}

	void Update() {

	}


	public void ExitPressed() {
		print("Exit Pressed");
		exitMessage.gameObject.SetActive(true);
		yesQuit.gameObject.SetActive (true);
		noQuit.gameObject.SetActive (true);

		startButton.gameObject.SetActive (false);
		exitButton.gameObject.SetActive (false);

	}

	public void NoPressed() {
		print("No pressed");
		exitMessage.gameObject.SetActive(false);
		yesQuit.gameObject.SetActive (false);
		noQuit.gameObject.SetActive (false);

		startButton.gameObject.SetActive (true);
		exitButton.gameObject.SetActive (true);
	}

	public void StartGame() {
		print("start pressed");
		SceneManager.LoadScene("TestRoom");
	}

	public void QuitGame() {
		Application.Quit();
	}




}
