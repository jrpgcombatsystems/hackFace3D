using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class BattleManager : MonoBehaviour {

    // Camera references
    [SerializeField] Transform battleCameraPoint;

    // Misc references
    [SerializeField] GameObject battleScene;
    public BattleSelf[] battleSelves;

    // Prefab references
    [SerializeField] GameObject damageNumberPrefab;
    public GameObject corpsePrefab;

    enum BattleState { Intro, AwaitingInput, TakingAction, End }
    BattleState battleState;
    int preBattleHP;
    Vector3 damageNumberOffset = new Vector3(0f, 1f, -0.5f);
    GameObject Mapself { get { return Services.playerController.mapself; } }
    Transform FieldCameraReference { get { return Services.playerController.fieldCameraReference; } }

    private void OnEnable() {
        GameEventManager.instance.Subscribe<GameEvents.BattleStarted>(BattleStartedHandler);
    }

    private void OnDisable() {
        GameEventManager.instance.Unsubscribe<GameEvents.BattleStarted>(BattleStartedHandler);
    }

    private void Update() {
        switch (battleState) {

            case BattleState.Intro:
                break;
                
            case BattleState.AwaitingInput:
                // Wait for BattleMenuManager...

                // See if at least one battleself has died.
                foreach (BattleSelf battleself in battleSelves) {
                    if (battleself.HP <= 0) {
                        battleState = BattleState.End;
                    }
                }

                break;

            case BattleState.TakingAction:
                break;

            case BattleState.End:
                break;
        }
    }

    void BattleStartedHandler(GameEvent gameEvent) {
        foreach(BattleSelf battleSelf in battleSelves) { battleSelf.SpawnBody(); }
        StartCoroutine(BattleIntroSequence());
    }

    void TurnOffBattleScene() {
        TurnOffBattleScene(false, true);
    }

    void TurnOffBattleScene(bool moveCamera, bool setMapselfActive) {
        foreach (BattleSelf battleSelf in battleSelves) { battleSelf.DestroyBody(); }
        battleScene.transform.parent = Services.playerController.transform;
        battleScene.transform.localPosition = Vector3.zero;
        battleScene.SetActive(false);
        Mapself.SetActive(setMapselfActive);
        Services.playerController.isMovementEnabled = setMapselfActive;

        if (moveCamera) {
            Camera.main.transform.position = FieldCameraReference.transform.position;
            Camera.main.transform.rotation = FieldCameraReference.transform.rotation;
        }
    }

    int GetDamageValue(out DamageNumber.HitType hitType) {
        // Get base damage:
        int damage = Services.playerStats.strength + ((Services.playerStats.strength + Services.playerStats.level) / 32) * ((Services.playerStats.strength * Services.playerStats.level) / 32);

        // See if attack is a critical hit
        if (Random.value <= Services.playerStats.luck) { hitType = DamageNumber.HitType.Critical; }
        else { hitType = DamageNumber.HitType.Normal; }

        // Factor in other player's defense. (Skip if attack is critical)
        int tempDefense = Services.playerStats.defense;
        if (hitType == DamageNumber.HitType.Critical) { tempDefense = 0; }
        damage = ((Services.playerStats.strength * (512 - tempDefense) * damage) / (16 * 512));

        // Add critical modifier
        if (hitType == DamageNumber.HitType.Critical) { damage = Mathf.FloorToInt(damage * 1.777f); }

        return damage;
    }

    bool IsAnimationComplete(Animator animator) {
        float animationTime = animator.GetCurrentAnimatorStateInfo(0).normalizedTime;
        if (animationTime >= 1) { return true; }
        else { return false; }
    }

    IEnumerator BattleIntroSequence() {
        // Turn on and off the right thingies
        battleState = BattleState.Intro;
        battleScene.SetActive(true);
        Services.battleMenuManager.ShowMenu(true);
        Services.battleMenuManager.AllowInput(true);
        Mapself.SetActive(false);
        Services.playerController.isMovementEnabled = false;

        foreach (BattleSelf battleSelf in battleSelves) {
            battleSelf.HP = Services.gameManager.CurrentHP;
        }

        // Move camera into position
        float duration = 1.5f;
        Camera.main.transform.DOLocalMove(battleCameraPoint.transform.localPosition, duration);
        Camera.main.transform.DOLocalRotate(battleCameraPoint.transform.localRotation.eulerAngles, duration);

        battleState = BattleState.AwaitingInput;

        yield return null;
    }

    IEnumerator BattleEndSequence() {

        Services.battleMenuManager.AllowInput(false);

        // Determine which instance of the player won the battle.
        BattleSelf winner = null;
        foreach (BattleSelf battleself in battleSelves) {
            if (battleself.HP > 0) { winner = battleself; }
        }

        // If we couldn't find a winner, (ie both battleSelves are dead, trigger a game over)
        if (winner == null) {
            // Add more you know, stuff here later
            TurnOffBattleScene(false, false);
            StartCoroutine(Services.gameManager.ResurrectionSequence());
            yield break;
        } 

        // Hide battle menu
        Services.battleMenuManager.ShowMenu(false);

        // Tween camera to victory camera position.
        float duration = 0.8f;
        Camera.main.transform.DOMove(winner.victoryCameraTransform.position, duration);
        Camera.main.transform.DORotate(winner.victoryCameraTransform.rotation.eulerAngles, duration);
        yield return new WaitForSeconds(duration);

        // Show player victory animation.
        if (winner.gameObject.name.Contains("Right")) { winner.m_Animator.SetTrigger("Right Victory Trigger"); } else { winner.m_Animator.SetTrigger("Left Victory Trigger"); }

        // Give animator a second to update its state.
        yield return new WaitForSeconds(0.5f);

        // Wait for animation to complete.
        yield return new WaitUntil(() => {
            if (IsAnimationComplete(winner.m_Animator)) { return true; } else { return false; }
        });

        // Wait a moment longer.
        yield return new WaitForSeconds(1.5f);

        // Temporarily decouple the main camera from the player rig, and secretly move the rig to be centered on the winning battleself.
        Camera.main.transform.parent = null;
        battleScene.transform.parent = null;

        Services.playerController.transform.position = winner.transform.position;

        // Recouple the camera to the player rig.
        Camera.main.transform.parent = Services.playerController.transform;

        // Move the camera to its field position.
        duration = 0.8f;
        Camera.main.transform.DOMove(FieldCameraReference.transform.position, duration);
        Camera.main.transform.DORotate(FieldCameraReference.transform.rotation.eulerAngles, duration);
        yield return new WaitForSeconds(duration);

        // Update player HP
        Services.gameManager.CurrentHP = winner.HP;

        // Turn off battlescenes and activate field player.
        TurnOffBattleScene();

        GameEventManager.instance.FireEvent(new GameEvents.BattleEnded());
    }

    public IEnumerator AttackSequence() {
        battleState = BattleState.TakingAction;
        Services.battleMenuManager.AllowInput(false);

        foreach (BattleSelf battleSelf in battleSelves) {
            battleSelf.m_Animator.SetTrigger("Attack Trigger");
        }

        yield return new WaitForSeconds(1.5f);

        // Show damage numbers
        int deaths = 0;
        foreach (BattleSelf battleSelf in battleSelves) {   
            // Get damage
            DamageNumber.HitType hitType = DamageNumber.HitType.Normal;
            int damageValue = GetDamageValue(out hitType);

            // Display critical hit effects
            if (hitType == DamageNumber.HitType.Critical) {
                // Add more shit here later
            }

            // Test for miss
            if (Services.playerStats.accuracy <= Random.value) {
                hitType = DamageNumber.HitType.Miss;
                damageValue = 0;
            }

            DamageNumber damageNumber = Instantiate(damageNumberPrefab).GetComponent<DamageNumber>();
            damageNumber.transform.position = battleSelf.transform.position + damageNumberOffset;
            damageNumber.Initialize(damageValue, hitType);
            battleSelf.HP -= damageValue;

            if (battleSelf.HP <= 0) {
                deaths++;
            }
        }

        // If at least one battleSelf has died.
        if (deaths > 0) {
            // If both players die, trigger their death animation without spawning a corpse.
            foreach (BattleSelf battleSelf in battleSelves) {
                if (battleSelf.HP <= 0) {
                    if (deaths == 2) { battleSelf.Die(false); }
                    else { battleSelf.Die(true); }
                }
            }

            yield return new WaitForSeconds(2.5f);

            StartCoroutine(BattleEndSequence());
            yield return null;
        } else {
            battleState = BattleState.AwaitingInput;
            Services.battleMenuManager.AllowInput(true);
        }

        yield return null;
    }

    public IEnumerator UsePotionSequence() {
        battleState = BattleState.TakingAction;
        Services.battleMenuManager.AllowInput(false);

        // I guess play an animation where the player uses a potion or whatever and then you know like sparkly particles etc etc it'll look real cool yeah
        yield return new WaitForSeconds(0.75f);

        // Use the thingy majingy :-)
        Services.potionManager.Use();

        for (int i = 0; i < battleSelves.Length; i++) {
            DamageNumber healNumber = Instantiate(damageNumberPrefab).GetComponent<DamageNumber>();
            healNumber.transform.position = battleSelves[i].transform.position;
            healNumber.transform.position += damageNumberOffset;
            healNumber.Initialize(Services.potionManager.healthValue, DamageNumber.HitType.Heal);
        }

        yield return new WaitForSeconds(0.75f);

        battleState = BattleState.AwaitingInput;
        Services.battleMenuManager.AllowInput(true);

        yield return null;
    }
}
