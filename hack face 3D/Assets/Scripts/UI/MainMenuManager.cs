using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MainMenuManager : MonoBehaviour {

    [SerializeField] GameObject mainMenu;
    [SerializeField] Image selectHand;
    [SerializeField] TMP_Text itemSelection;
    [SerializeField] TMP_Text exitSelection;
    [SerializeField] GameObject itemMenu;
    [SerializeField] TMP_Text hpDisplay;
    [SerializeField] TMP_Text levelDisplay;
    [SerializeField] TMP_Text strengthDisplay;
    [SerializeField] TMP_Text defenseDisplay;
    [SerializeField] TMP_Text accuracyDisplay;
    [SerializeField] TMP_Text luckDisplay;

    enum MenuSelection { Item, Exit }
    MenuSelection menuSelectionDontTouch = MenuSelection.Item;
    MenuSelection menuSelection {
        get { return menuSelectionDontTouch; }
        set {
            Vector3 newPosition = selectHand.rectTransform.localPosition;
            newPosition.x = -195f;
            if (value == MenuSelection.Item) { newPosition.y = 26; }
            else if (value == MenuSelection.Exit) { newPosition.y = -37; }
            selectHand.rectTransform.localPosition = newPosition;
            menuSelectionDontTouch = value;
        }
    }

    bool isItemMenuOpen = false;

    private void Update() {
        if (mainMenu.activeInHierarchy) {
            if (!isItemMenuOpen) {
                // Allow player move the cursor in the menu
                if (Input.GetKeyDown(KeyCode.UpArrow)) {
                    menuSelection = (MenuSelection)MyMath.Wrap((int)menuSelection - 1, 0, 2);
                } else if (Input.GetKeyDown(KeyCode.DownArrow)) {
                    menuSelection = (MenuSelection)MyMath.Wrap((int)menuSelection + 1, 0, 2);
                }

                // When the player makes a selection
                if (Input.GetKeyDown(KeyCode.Space)) {
                    switch (menuSelection) {
                        case MenuSelection.Item:
                            OpenItemMenu(true);
                            break;
                        case MenuSelection.Exit:
                            Show(false);
                            break;
                    }
                }

                if (Input.GetKeyDown(KeyCode.X)) {
                    Show(false);
                }
            }

            // If the item screen is open
            else {
                if (Input.GetKeyDown(KeyCode.Space)) {
                    if (Services.gameManager.CurrentHP >= Services.playerStats.maxHP) { return; }
                    if (Services.potionManager.CurrentAmount <= 0) { return; }
                    Services.potionManager.Use();
                }
                else if (Input.GetKeyDown(KeyCode.X)) { OpenItemMenu(false); }
            }
        }
    }

    void OpenItemMenu(bool value) {
        itemMenu.SetActive(value);
        selectHand.enabled = !value;
        isItemMenuOpen = value;
    }

    public void Show(bool value) {
        Services.playerController.isMovementEnabled = !value;
        mainMenu.SetActive(value);
        UpdateStatDisplays();
        Services.gameManager.dontTakeInput = true;
        Services.gameManager.gameState = GameManager.GameState.Exploration;
    }

    public void UpdateStatDisplays() {
        hpDisplay.text = Services.gameManager.CurrentHP.ToString() + "/" + Services.playerStats.maxHP;
        levelDisplay.text = Services.playerStats.level.ToString();
        strengthDisplay.text = Services.playerStats.strength.ToString();
        defenseDisplay.text = Services.playerStats.defense.ToString();
        accuracyDisplay.text = (Services.playerStats.accuracy * 100).ToString();
        luckDisplay.text = (Services.playerStats.luck * 100).ToString();
    }
}
