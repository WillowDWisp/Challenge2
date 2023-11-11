using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace jcsilva.CharacterController {

    [RequireComponent(typeof(Rigidbody))]
    public class PlayerController : MonoBehaviour {

        [Header("Player Settings")]
        [Range(0.5f, 10f)]
        [SerializeField] float movementSpeed = 2f;
        [Range(2f, 50f)]
        [SerializeField] float runningSpeed = 10f;
        [SerializeField] float rotationSpeed = 100f;

        private float rotY;
        private float currentSpeed;

        private Rigidbody rb;

        private void Start() {
            rb = GetComponent<Rigidbody>();
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }

        private void FixedUpdate() {
            float x = Input.GetAxis("Horizontal");
            float y = Input.GetAxis("Vertical");

            Vector3 movement = transform.right * x + transform.forward * y;

            movement = movement.normalized;

            currentSpeed = Input.GetKey(KeyCode.LeftShift) ? runningSpeed : movementSpeed;

            if (movement != Vector3.zero) {
                Vector3 velocity = new Vector3(movement.x, rb.velocity.y, movement.z);
                rb.velocity = velocity * currentSpeed;
            } else {
                rb.velocity = new Vector3(0f, rb.velocity.y, 0f);
            }
        }

        private void Update() {
            rotY += Input.GetAxis("Mouse X") * Time.deltaTime * rotationSpeed;
            transform.rotation = Quaternion.Euler(0f, rotY, 0f);
        }
    }
}