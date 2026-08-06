// PlayerUI.cs

// imports
using UnityEngine;
using TMPro;

// Player UI
public class PlayerUI : MonoBehaviour {
    [Header("References")]
    public PlayerSwitcher playerSwitcher;
    public TMP_Text playerNameText;

    private PlayerController lastActive;

    // Uses the active player name to display as Text
    void Update() {
        PlayerController active = playerSwitcher.ActivePlayer;

        if (active != lastActive) {
            if (active != null) {
                playerNameText.text = active.positionName;
            }
            lastActive = active;
        }
    }
}
