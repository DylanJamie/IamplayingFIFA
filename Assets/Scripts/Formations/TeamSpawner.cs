// TeamSpawner.cs

using UnityEngine;
using System.Collections.Generic;

public class TeamSpawner : MonoBehaviour {
    [Header("Setup")]
    public Formation formation;
    public GameObject playerPrefab;
    public PlayerSwitcher playerSwitcher;
    public GoalManager goalManager;

    private List<PlayerController> spawnedPlayers = new List<PlayerController>();

    void Awake() {
        SpawnTeam();
    }

    void SpawnTeam() {
        foreach (FormationSlot slot in formation.slots) {
            GameObject go = Instantiate(playerPrefab, slot.spawnPoint.position, slot.spawnPoint.rotation);
            PlayerController pc = go.GetComponent<PlayerController>();

            pc.positionName = slot.positionName;
            pc.startPosition = slot.spawnPoint;
            // pc.role = slot.role;  // once AI behavior differs by role

            spawnedPlayers.Add(pc);
        }

        // need to Wire into the system 
        playerSwitcher.teamPlayers = spawnedPlayers;
        goalManager.players = spawnedPlayers;
    }
}
