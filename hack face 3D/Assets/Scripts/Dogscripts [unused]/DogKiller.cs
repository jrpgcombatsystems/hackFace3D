using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DogKiller : MonoBehaviour {

    private void OnTriggerEnter(Collider other) {
        if (other.name.ToLower().Contains("dog"))
        {
            GetComponent<AudioSource>().Play();
            Destroy(other.gameObject);
        }
    }
}
