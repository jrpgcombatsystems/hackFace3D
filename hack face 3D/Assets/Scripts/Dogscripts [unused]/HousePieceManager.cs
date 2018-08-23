using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HousePieceManager : MonoBehaviour {

    public Object[] housePieces;
    [SerializeField] GameObject housePrefab;
    [SerializeField] GameObject ground;
    [SerializeField] int numberOfHouses = 10;


    private void Awake() {
        housePieces = Resources.LoadAll("Prefabs/House Pieces", typeof(GameObject));
    }


    private void Start() {
        PlaceHouses();
    }


    void PlaceHouses() {
        float groundSize = ground.transform.localScale.x * 10f;

        for (int i = 0; i < numberOfHouses; i++) {
            GameObject newHouse = Instantiate(housePrefab);
            Vector3 newHousePosition = new Vector3(
                Random.Range(-groundSize * 0.5f, groundSize * 0.5f),
                2.5f,
                Random.Range(-groundSize * 0.5f, groundSize * 0.5f)
                );
            newHouse.transform.position = newHousePosition;
        }
    }
}
