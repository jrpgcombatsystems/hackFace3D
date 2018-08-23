using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BattleMenuManager : MonoBehaviour {

    // UI references
    [SerializeField] Image battleHand;
    [SerializeField] GameObject battleMenu;
    [SerializeField] GameObject itemMenu;

    bool isInputAllowed = false;

    enum SubMenu { Main, Item }
    SubMenu currentSubMenu = SubMenu.Main;

    enum MainMenuSelection { Attack, Item, Escape }
    MainMenuSelection currentMenuSelectionHandsOffTheMerchandise = MainMenuSelection.Attack;
    MainMenuSelection CurrentMenuSelection {
        get {
            return currentMenuSelectionHandsOffTheMerchandise;
        }

        set {
            Vector3 newPosition = battleHand.rectTransform.localPosition;
            if (value == MainMenuSelection.Attack) { newPosition.y = 36f; } 
            else if (value == MainMenuSelection.Item) { newPosition.y = -1f; } 
            else if (value == MainMenuSelection.Escape) { newPosition.y = -36f; }
            battleHand.rectTransform.localPosition = newPosition;
            currentMenuSelectionHandsOffTheMerchandise = value;
        }
    }

    private void Update() {
        // Menu selection
        if (!isInputAllowed) { return; }

        switch (currentSubMenu) {

            case SubMenu.Main:
                // Allow player move the cursor in the menu
                if (Input.GetKeyDown(KeyCode.UpArrow)) {
                    CurrentMenuSelection = (MainMenuSelection)MyMath.Wrap((int)CurrentMenuSelection - 1, 0, 3);
                } else if (Input.GetKeyDown(KeyCode.DownArrow)) {
                    CurrentMenuSelection = (MainMenuSelection)MyMath.Wrap((int)CurrentMenuSelection + 1, 0, 3);
                }

                // When the player makes a selection
                if (Input.GetKeyDown(KeyCode.Space)) {
                    switch (CurrentMenuSelection) {
                        case MainMenuSelection.Attack:
                            StartCoroutine(Services.battleManager.AttackSequence());
                            break;
                        case MainMenuSelection.Item:
                            OpenItemMenu(true);
                            break;
                        case MainMenuSelection.Escape:
                            break;
                    }
                }
                break;

            case SubMenu.Item:
                if (Input.GetKeyDown(KeyCode.Space)) {
                    if (Services.battleManager.battleSelves[0].HP >= Services.playerStats.maxHP) { return; }
                    if (Services.potionManager.CurrentAmount <= 0) { return; }
                    OpenItemMenu(false);
                    StartCoroutine(Services.battleManager.UsePotionSequence());
                } 
                
                else if (Input.GetKeyDown(KeyCode.X)) {
                    OpenItemMenu(false);
                }
                break;
        }
    }

    void OpenItemMenu(bool value) {
        itemMenu.SetActive(value);
        battleHand.enabled = !value;

        if (value == true) { currentSubMenu = SubMenu.Item; }
        else { currentSubMenu = SubMenu.Main; }
    }

    public void AllowInput(bool value) {
        battleHand.enabled = value;
        isInputAllowed = value;
    }

    public void ShowMenu(bool value) {
        battleMenu.SetActive(value);
    }
}
