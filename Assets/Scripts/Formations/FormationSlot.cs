// FormationSlot.cs

using UnityEngine;

// A single position in a formation — e.g. "Striker" at a specific spot
[System.Serializable]
public class FormationSlot {
    public string positionName;   // "Striker", "Right Wing", "Center Back"
    public Transform spawnPoint;  // where this player starts
    public PlayerRole role;       // used for AI behavior differences later
}

// Broad behavior categories — drives AI positioning logic, not just cosmetics
public enum PlayerRole {
    Attacker,
    Midfielder,
    Defender,
    Goalkeeper
}
