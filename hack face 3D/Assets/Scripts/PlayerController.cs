using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour {

    [SerializeField] float moveSpeed = 1f;
    [SerializeField] float bodyRotateSpeed = 1f;

    public GameObject mapself;
    [SerializeField] Transform bodyRoot;
    public Transform fieldCameraReference;

    public bool isMovementEnabled = true;
    
    Vector3 inputDirection = Vector3.zero;
    Vector3 newBodyRotation;

    Rigidbody m_Rigidbody;
    Animator m_Animator;


    private void Start() {
        m_Rigidbody = GetComponent<Rigidbody>();
        m_Animator = GetComponentInChildren<Animator>();
    }


    private void Update() {
        if (!isMovementEnabled) { return; }

        // Get input direction.
        inputDirection = Vector3.zero;
        if (Input.GetKey(KeyCode.LeftArrow) || Input.GetAxisRaw("Horizontal") == -1f) { inputDirection.x = -1f; }
        if (Input.GetKey(KeyCode.RightArrow) || Input.GetAxisRaw("Horizontal") == 1f) { inputDirection.x = 1f; }
        if (Input.GetKey(KeyCode.UpArrow) || Input.GetAxisRaw("Vertical") == 1f) { inputDirection.z = 1f; }
        if (Input.GetKey(KeyCode.DownArrow) || Input.GetAxisRaw("Vertical") == -1f) { inputDirection.z = -1f; }

        if (Input.GetKey(KeyCode.Space) || Input.GetButton("Submit")) { inputDirection = Vector3.zero; }


        if (inputDirection != Vector3.zero) {
            // Start animation
            m_Animator.StopPlayback();

            // Test for random encounter
            Services.battleChanceManager.TestForBattle();

            // Rotate body
            Quaternion towardsRotation = Quaternion.LookRotation(inputDirection, Vector3.up);
            newBodyRotation = Quaternion.RotateTowards(
                Quaternion.Euler(newBodyRotation),
                towardsRotation,
                bodyRotateSpeed * Time.deltaTime)
                .eulerAngles;

            newBodyRotation.x = transform.localRotation.eulerAngles.x;
            newBodyRotation.z = transform.localRotation.eulerAngles.y;

            bodyRoot.localRotation = Quaternion.Euler(newBodyRotation);

            // Move
            m_Rigidbody.MovePosition(transform.position + inputDirection.normalized * moveSpeed * Time.deltaTime);

            // Jitter
            bodyRoot.transform.localPosition += Random.insideUnitSphere * 0.01f * Time.deltaTime;
        } 
        
        else { m_Animator.StartPlayback(); }

    }
}
