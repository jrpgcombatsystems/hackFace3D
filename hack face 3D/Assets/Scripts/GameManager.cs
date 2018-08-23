using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class GameManager : MonoBehaviour {

    public enum GameState { Exploration, Battle, Paused, Menu }
    [HideInInspector] public GameState gameState;

    int currentHP;
    [HideInInspector] public int CurrentHP {
        get { return currentHP; }
        set {
            value = Mathf.Clamp(value, 0, Services.playerStats.maxHP);
            currentHP = value;
        }
    }
    [HideInInspector] public bool dontTakeInput;
    [HideInInspector] Stack<GameObject> corpses = new Stack<GameObject>();

    public void OnEnable() {
        GameEventManager.instance.Subscribe<GameEvents.BattleStarted>(BattleStartedHandler);
        GameEventManager.instance.Subscribe<GameEvents.BattleEnded>(BattleEndedHandler);
    }

    public void OnDisable() {
        GameEventManager.instance.Unsubscribe<GameEvents.BattleStarted>(BattleStartedHandler);
        GameEventManager.instance.Unsubscribe<GameEvents.BattleEnded>(BattleEndedHandler);
    }

    public void Awake() {
        Services.battleManager = GetComponentInChildren<BattleManager>();
        Services.gameManager = this;
        Services.battleChanceManager = GetComponentInChildren<BattleChanceManager>();
        Services.playerController = FindObjectOfType<PlayerController>();
        Services.potionManager = GetComponentInChildren<PotionManager>();
        Services.playerStats = new PlayerStats();
        Services.battleMenuManager = GetComponentInChildren<BattleMenuManager>();
        Services.mainMenuManager = GetComponentInChildren<MainMenuManager>();

        CurrentHP = Services.playerStats.maxHP;
    }

    private void Update() {
        switch (gameState) {
            case GameState.Exploration:
                if (Input.GetKeyDown(KeyCode.X)) {
                    if (dontTakeInput) {
                        dontTakeInput = false;
                        return;
                    }
                    Services.mainMenuManager.Show(true);
                    gameState = GameState.Menu;
                }
                break;
            case GameState.Battle:
                break;
            case GameState.Paused:
                break;
            case GameState.Menu:
                break;
            default:
                break;
        }
    }

    void BattleStartedHandler(GameEvent gameEvent) {
        gameState = GameState.Battle;
    }

    void BattleEndedHandler(GameEvent gameEvent) {
        gameState = GameState.Exploration;
    }

    void GameOver() {
        // One day this function will do something else.
        Debug.Log("Game Over");
    }

    public void AddACorpseToTheCorpsePile(GameObject corpse) {
        corpses.Push(corpse);
    }

    public IEnumerator ResurrectionSequence() {
        // If there are no corpses left, just trigger a game over.
        if (corpses.Count == 0) {
            GameOver();
            yield break;
        }

        // Move player rig to last corpse's position.
        GameObject lastCorpse = corpses.Pop();
        float duration = 1f;
        Services.playerController.transform.DOMove(lastCorpse.transform.position, duration);
        Camera.main.transform.DOLocalMove(Services.playerController.fieldCameraReference.localPosition, duration);
        Camera.main.transform.DOLocalRotate(Services.playerController.fieldCameraReference.localRotation.eulerAngles, duration);
        yield return new WaitForSeconds(duration);

        // Replace corpse with player.
        Services.playerController.mapself.SetActive(true);
        Destroy(lastCorpse);

        gameState = GameState.Exploration;
        Services.playerController.isMovementEnabled = true;

        yield return null;
    }
}
