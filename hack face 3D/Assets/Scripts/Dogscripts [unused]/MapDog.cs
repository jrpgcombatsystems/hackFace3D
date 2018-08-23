using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapDog : MonoBehaviour {

    [SerializeField] Transform dogMesh;
    GameObject player;


    void Start() {
        player = FindObjectOfType<PlayerController>().gameObject;
    }


    private void Update() {
        dogMesh.LookAt(player.transform);
        Vector3 newEulers = new Vector3(
            0f,
            dogMesh.transform.rotation.eulerAngles.y,
            0f
            );
    }
}
