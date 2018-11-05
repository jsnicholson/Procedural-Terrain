using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour {

    public float speed = 10.0f;
    public float sprintSpeed = 20.0f;

    void FixedUpdate() {
        MovePlayer();
    }

    void MovePlayer(){
        if (Input.GetKey(KeyCode.LeftShift)) {
            speed = sprintSpeed;
        }
        if (Input.GetKey(KeyCode.W)) {
            transform.position = transform.position + Camera.main.transform.forward * speed * Time.deltaTime;
        }
        if (Input.GetKey(KeyCode.S)) {
            transform.position = transform.position + -(Camera.main.transform.forward) * speed * Time.deltaTime;
        }
        if (Input.GetKey(KeyCode.A)) {
            transform.position = transform.position + -(Camera.main.transform.right) * speed * Time.deltaTime;
        }
        if (Input.GetKey(KeyCode.D)) {
            transform.position = transform.position + Camera.main.transform.right * speed * Time.deltaTime;
        }
        if (Input.GetKey(KeyCode.Space)) {
            transform.position = transform.position + Vector3.up * speed * Time.deltaTime;
        }
        if (Input.GetKey(KeyCode.LeftControl)) {
            transform.position = transform.position - Vector3.up * speed * Time.deltaTime;
        }
    }
}
