using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapDogPlacer : MonoBehaviour {

    Transform player;
    [SerializeField] GameObject dogPrefab;
    float spawnDistanceMin = 2.5f;
    float spawnDistanceMax = 10f;


    private void Start() {
        player = FindObjectOfType<PlayerController>().transform;
    }


    public void PlaceDog() {
        if (Random.value >= 0.2f) return;

        GameObject newDog = Instantiate(dogPrefab);

        Vector3 newPosition = player.position;
        float modifier = Random.Range(spawnDistanceMin, spawnDistanceMax);
        if (Random.value >= 0.5f) { modifier *= -1f; }
        newPosition.x += modifier;
        modifier = Random.Range(spawnDistanceMin, spawnDistanceMax);
        if (Random.value >= 0.5f) { modifier *= -1f; }
        newPosition.z += modifier;
        newDog.transform.position = newPosition;
    }
}
