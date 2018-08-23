using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PotionManager : MonoBehaviour {

    [SerializeField] string displayName = "Hi Potion";
    public int healthValue = 900;
    [SerializeField] int startingAmount = 99;

    [SerializeField] TMP_Text itemAmountTextBattle;
    [SerializeField] TMP_Text itemAmoutTextMenu;

    int currentAmount;
    public int CurrentAmount {
        get { return currentAmount; }
        set {
            value = Mathf.Clamp(value, 0, startingAmount);
            itemAmountTextBattle.text = value.ToString();
            itemAmoutTextMenu.text = value.ToString();
            currentAmount = value;
        }
    }

    private void Awake() {
        CurrentAmount = startingAmount;
    }

    public void Use() {
        if (CurrentAmount <= 0) { return; }
        switch (Services.gameManager.gameState) {
            case GameManager.GameState.Exploration | GameManager.GameState.Menu:
                Services.gameManager.CurrentHP += healthValue;
                break;
            case GameManager.GameState.Battle:
                foreach(BattleSelf battleSelf in Services.battleManager.battleSelves) {
                    battleSelf.HP += healthValue;
                }
                break;
            default:
                break;
        }

        Services.mainMenuManager.UpdateStatDisplays();

        CurrentAmount--;
    }
}
