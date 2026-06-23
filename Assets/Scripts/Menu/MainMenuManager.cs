// MainMenuManager.cs

// this will manages the buttons for the start of the game
using UnityEngine;
using UnityEngine.SceneManagement;

// Class for main menu
public class MainMenuManager : MonoBehaviour {
    // Headers
    [Header("Panels")]
    public GameObject difficultyPanel;
    public GameObject settingsPanel;

    // Called by the start button
    public void StartGame() {
	// Match the schene name of the game
	SceneManager.LoadScene("SampleScene");
    }

    // When hitting the Difficulty panel
    public void OpenDifficulty() {
	difficultyPanel.SetActive(true);
    }

    // Hit the settings button
    public void OpenSettings() {
	settingsPanel.SetActive(true);
    }

    // Quit the game
    public void QuitGame() {
	Application.Quit();
    }
}
