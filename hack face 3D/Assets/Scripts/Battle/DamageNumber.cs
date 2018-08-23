using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using DG.Tweening;

public class DamageNumber : MonoBehaviour {

    TextMeshPro m_TextMeshPro { get { return GetComponent<TextMeshPro>(); } }
    public enum HitType { Normal, Critical, Miss, Heal }

    public void Initialize(int damageValue) {
        StartCoroutine(AppearDisappearSequence(HitType.Normal));
    }

    public void Initialize(int damageValue, HitType hitType) {
        if (hitType == HitType.Miss) { m_TextMeshPro.text = "Miss!"; }
        else { m_TextMeshPro.text = damageValue.ToString(); }
        StartCoroutine(AppearDisappearSequence(hitType));
    }

    IEnumerator AppearDisappearSequence(HitType hitType) {
        float duration = 0.6f;

        // Appear and get big/bounce:
        Color newColor = Color.white;
        if (hitType == HitType.Critical) { newColor = Color.yellow; }
        else if (hitType == HitType.Heal) { newColor = Color.green; }
        m_TextMeshPro.DOColor(newColor, duration);

        Vector3 newPos = transform.position;
        newPos += Vector3.Scale(Vector3.Normalize(Camera.main.transform.position - transform.position), new Vector3(1f, 0f, 0f)) * 0.5f;
        transform.position = newPos;

        float scaleMultiplier = 1.5f;
        if (hitType == HitType.Critical) { scaleMultiplier = 2f; }
        Vector3 newScale = transform.localScale * scaleMultiplier;
        transform.localScale = transform.localScale * 0.25f;
        transform.DOScale(newScale, duration);

        // Bounce up...
        Vector3 bouncePos = transform.position;
        bouncePos.y += 1f;
        transform.DOMove(bouncePos, duration * 0.5f);
        yield return new WaitForSeconds(duration * 0.5f);

        // Then bounce down
        bouncePos.y -= 0.5f;
        transform.DOMove(bouncePos, duration * 0.5f);
        yield return new WaitForSeconds(duration * 0.5f);

        // Pause before fading out:
        yield return new WaitForSeconds(0.75f);

        // Fade out:
        duration = 0.6f;
        m_TextMeshPro.DOColor(new Color(1f, 1f, 1f, 0f), duration);
        yield return new WaitForSeconds(duration);

        Destroy(this.gameObject);
        
        yield return null;
    }
}
