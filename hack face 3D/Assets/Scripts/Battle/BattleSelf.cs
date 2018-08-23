using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class BattleSelf : MonoBehaviour {

    [SerializeField] TMP_Text hpText;
    public Transform victoryCameraTransform;
    [SerializeField] GameObject bodyPrefab;

    GameObject myBody;
    int hp;
    public int HP {
        get { return hp; }
        set {
            value = Mathf.Clamp(value, 0, Services.playerStats.maxHP);
            hpText.text = value.ToString() + "/" + Services.playerStats.maxHP;
            hp = value;
        }
    }
    public Animator m_Animator { get { return GetComponentInChildren<Animator>(); } }

    public void SpawnBody() {
        myBody = Instantiate(bodyPrefab, transform);
        myBody.transform.localPosition = Vector3.zero;
    }

    public void DestroyBody() {
        if (myBody == null) { return; }
        Destroy(myBody);
        myBody = null;
    }

    public void Die(bool spawnCorpse) {
        StartCoroutine(DeathSequence(spawnCorpse));
    }

    IEnumerator DeathSequence(bool spawnCorpse) {
        m_Animator.SetTrigger("Death Trigger");

        yield return new WaitForSeconds(1f);

        // Instantiate a corpse
        if (spawnCorpse) {
            GameObject corpse = Instantiate(Services.battleManager.corpsePrefab);
            corpse.transform.position = transform.position;
            corpse.transform.rotation = transform.rotation;
            Services.gameManager.AddACorpseToTheCorpsePile(corpse);
        }

        yield return null;
    }
}
