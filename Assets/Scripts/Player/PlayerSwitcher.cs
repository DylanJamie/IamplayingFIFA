// PlayerSwitcher

// Imports For unity
using UnityEngine;
using UnityEngine.InputSystem;

// Owns which PlayerController is currently being controlled by the user and forwards the input system to one player only
public class PlayerSwitcher : MonoBehaviour {
    [Header("Players")]
    public PlayerController player_1;
    public PlayerController player_2;

    private PlayerController activePlayer;

    // public pointer to active player
    public PlayerController ActivePlayer => activePlayer;
    
    // Start funct
    void Start() {
        // Wire each player to know about the other, and about this switcher
        player_1.teammate = player_2;
        player_2.teammate = player_1;
        player_1.playerSwitcher = this;
        player_2.playerSwitcher = this;

        SetActive(player_1);
    }

    // helper function to switch and set the player controlling the ball
    void SetActive(PlayerController newActive) {
        activePlayer = newActive;

        player_1.isAIControlled = (player_1 != activePlayer);
        player_2.isAIControlled = (player_2 != activePlayer);
    }
    
    // Called by PlayerController.PassBall() once the pass has been kicked
    public void SwitchControlTo(PlayerController newActive) {
        SetActive(newActive);
    }

    // On move this will switch and make a call back
    public void OnMove(InputAction.CallbackContext context) {
        activePlayer.ProcessMove(context.ReadValue<Vector2>());
    }

    // if the player is sprinting
    public void OnSprint(InputAction.CallbackContext context) {
        if (context.performed) {
	    activePlayer.Sprinting(true);
	}
	
        if (context.canceled) {
	    activePlayer.Sprinting(false);
	}
    }

    public void OnShoot(InputAction.CallbackContext context) {
        if (context.started) {
	    activePlayer.StartCharging();
	}
	
        if (context.canceled) {
	    activePlayer.ReleaseShot();
	}
    }

    public void OnPass(InputAction.CallbackContext context) {
        if (context.performed) {
	    activePlayer.PassBall();
	}
    }
}
