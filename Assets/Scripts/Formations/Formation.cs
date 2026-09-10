// Formation.cs

using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "Formation", menuName = "FIFA/Formation")]
public class Formation : ScriptableObject {
    public string formationName; // "3v3", "4-4-2", etc.
    public List<FormationSlot> slots;
}
