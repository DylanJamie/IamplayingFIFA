// ScoreboardUI.cs

using UnityEngine;
using TMPro; // swap to UnityEngine.UI.Text if you're not using TextMeshPro

public class ScoreboardUI : MonoBehaviour {
    [Header("References")]
    public GoalManager goalManager;

    [Header("Text Fields")]
    public TextMeshProUGUI homeScoreText;
    public TextMeshProUGUI awayScoreText; // placeholder "0" until defenders can score
    public TextMeshProUGUI timerText;

    private float matchTime = 0f;

    void Update() {
        matchTime += Time.deltaTime;
        UpdateTimerDisplay();
    }

    // Call this whenever the score changes
    public void UpdateScoreDisplay() {
        if (homeScoreText != null)
            homeScoreText.text = goalManager.score.ToString();

        if (awayScoreText != null)
            awayScoreText.text = "0";
    }

    void UpdateTimerDisplay() {
        int minutes = Mathf.FloorToInt(matchTime / 60f);
        int seconds = Mathf.FloorToInt(matchTime % 60f);
        if (timerText != null)
            timerText.text = $"{minutes:00}:{seconds:00}";
    }
}
