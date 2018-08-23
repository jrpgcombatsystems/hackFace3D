using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneResetter : MonoBehaviour {

    [SerializeField] float resetTime = 60f;
    float timer;

    private void Update() {

        if (1.0f / Time.deltaTime < 5f && timer >= 2f) { ResetScene(); }

        if (Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.DownArrow) || Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.RightArrow) || Input.GetKeyDown(KeyCode.Space)) {
            timer = 0f;
        }
        
        if (Input.GetAxisRaw("Horizontal") != 0 || Input.GetAxisRaw("Vertical") != 0 || Input.GetButtonDown("Submit")) {
            timer = 0f;
        }

        timer += Time.deltaTime;
        if (timer >= resetTime) {
            ResetScene();
        }
    }

    public void ResetScene() {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
