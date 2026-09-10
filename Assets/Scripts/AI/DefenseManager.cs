// DefenseManager.cs
//
using UnityEngine;
using System.Collections.Generic;

// Coordinates every AI defender so they don't all swarm the ball at once.
// Plays the same role for the defending team that PlayerSwitcher plays for the attacking team.
public class DefenseManager : MonoBehaviour {
    [Header("References")]
    public Transform ball;
    public PlayerSwitcher playerSwitcher; // to find attackers to mark/dispossess
    public List<DefenderAI> defenders;

    [Header("Pressing Settings")]
    // how many defenders may actively chase the ball at once (keep this to 2 - 3) 
    public int maxPressers = 2;
    public float pressConsiderRadius = 25f;

    // Update every frame
    void Update() {
	// there is no ball or defenders return
        if (ball == null || defenders == null || defenders.Count == 0)
	    return;
	
	// Otherwise we assign Positions to those existing defenders
        AssignRoles();
    }

    // Assign roles or positions foe all the defenders in the game
    void AssignRoles() {

	// Compute each defender's distance to the ball exactly once, we store the defenders distance from the ball within its defender object.
	// improving efficency and makes it so no redudent calculations need to be completed
	//
	List<(DefenderAI defender, float distance)> withDistances = new List<(DefenderAI, float)>();
	foreach (DefenderAI d in defenders) {
	    withDistances.Add((d, Vector3.Distance(d.transform.position, ball.position)));
	}

	// Sort using the cached distance no recalculating here
	withDistances.Sort((a, b) => a.distance.CompareTo(b.distance));

	// loop will tell which defenders should press the ball and push 
	for (int i = 0; i < withDistances.Count; i++) {
	    DefenderAI d = withDistances[i].defender;
	    float distToBall = withDistances[i].distance;

	    // Check to see if at max pressers, if not or other def closer switch state of defenders closest to ball to pressing
	    if (i < maxPressers && distToBall <= pressConsiderRadius) {
		d.SetState(DefenderAI.AIState.Pressing);
		continue;
	    }

	    // if the defender is not chasing the ball find the closest attacker and mark him
	    // otherwise float around zone in zonal state
	    PlayerController nearestAttacker = FindNearestAttackerTo(d.HomePosition, d.markRadius);
	    if (nearestAttacker != null) {
		d.SetMarkTarget(nearestAttacker);
		d.SetState(DefenderAI.AIState.Marking);
	    } else {
		d.SetState(DefenderAI.AIState.Zonal);
	    }
	}
    }

    // find the nearest attacker to mark
    PlayerController FindNearestAttackerTo(Vector3 point, float radius) {
        PlayerController nearest = null;
        float best = radius;

	// note: might be able to optimize this, this goes through the list of players and measures distances for them all
        foreach (PlayerController pc in playerSwitcher.teamPlayers) {
            float dist = Vector3.Distance(point, pc.transform.position);
            if (dist < best) {
                best = dist;
                nearest = pc;
            }
        }
        return nearest;
    }
}
