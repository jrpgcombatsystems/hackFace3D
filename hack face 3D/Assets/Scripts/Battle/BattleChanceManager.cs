using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleChanceManager : MonoBehaviour {

    [SerializeField] float battleTestIncrement = 0.5f;    // How often we test for a random battle (in seconds)
    [SerializeField] float battlePercentage = 0.1f;   // The likelihood of a battle occuring during each test
 
    float timer;


    // Should be called every frame whenever we want random battles to happen (ie when the character is walking)
    public void TestForBattle() {
        timer += Time.deltaTime;
        if (timer >= battleTestIncrement) {
            if (Random.value <= battlePercentage) {
                GameEventManager.instance.FireEvent(new GameEvents.BattleStarted());
            }
            timer = 0f;
        }
    }
}
