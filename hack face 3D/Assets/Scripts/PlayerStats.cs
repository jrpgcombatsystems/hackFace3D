using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStats {

    public int maxHP = 3284;
    public int strength = 78;   // Determines base damage.
    public int defense = 66;    // Modifies base damage on self.
    public int level = 20;
    public float luck = 0.25f;  // Used to determine chance of critical hits.
    public float accuracy = 0.85f;  // Used to determine chance of missing.

    public PlayerStats() { }
}
