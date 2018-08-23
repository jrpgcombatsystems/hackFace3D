using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class House : MonoBehaviour {

    [SerializeField] float width = 10f;
    [SerializeField] float height = 5f;
    int numberOfPieces = 10;
    float[] potentialRotations = new float[4];


    private void Start() {
        potentialRotations[0] = 0f;
        potentialRotations[1] = 90f;
        potentialRotations[2] = 180f;
        potentialRotations[3] = 270f;

        Generate();
    }


    void Generate() {

        HousePieceManager housePieceManager = FindObjectOfType<HousePieceManager>();

        for (int i = 0; i < numberOfPieces; i++) {
            GameObject newPiece = Instantiate(housePieceManager.housePieces[Random.Range(0, housePieceManager.housePieces.Length - 1)]) as GameObject;

            Vector3 newPosition = transform.position;
            newPosition.x += Random.Range(-width, width);
            newPosition.y += Random.Range(-width, width);
            newPosition.z += Random.Range(-width, width);
            newPiece.transform.position = newPosition;

            Vector3 newRotation = newPiece.transform.rotation.eulerAngles;
            newRotation.x = potentialRotations[Random.Range(0, potentialRotations.Length - 1)];
            newRotation.y = potentialRotations[Random.Range(0, potentialRotations.Length - 1)];
            newRotation.z = potentialRotations[Random.Range(0, potentialRotations.Length - 1)];
            newPiece.transform.rotation = Quaternion.Euler(newRotation);

            newPiece.transform.parent = transform;
        }

        //transform.localScale = Vector3.one * 0.5f;
    }
}
