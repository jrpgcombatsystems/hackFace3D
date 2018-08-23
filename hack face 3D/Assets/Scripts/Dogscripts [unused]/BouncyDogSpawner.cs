using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BouncyDogSpawner : MonoBehaviour {

    [SerializeField] GameObject bouncyDogPrefab;
    [SerializeField] Collider bottomCollider;

    public void AddDog() {
        Instantiate(bouncyDogPrefab, transform.position, Random.rotation);
    }


    private void Update() {
        if (Input.GetKey(KeyCode.Space) || Input.GetButton("Submit")) {
            if (!GetComponent<AudioSource>().isPlaying) { GetComponent<AudioSource>().Play(); }
            bottomCollider.enabled = false;
        }

        else {
            bottomCollider.enabled = true;
            GetComponent<AudioSource>().Pause();
        }
    }
}
