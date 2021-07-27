using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterMove : MonoBehaviour
{
    public float speed = 5f; // 이동속도
    private Rigidbody characterRigidbody;

    void Start()
    {
        characterRigidbody = GetComponent<Rigidbody>();
    }

    void Update()
    {
        float inputX = Input.GetAxis("Horizontal"); //좌우
        float inputZ = Input.GetAxis("Vertical");   //앞뒤 
        // -1 ~ 1

        Vector3 velocity = new Vector3(inputX, 0, inputZ); // 3차원
        velocity *= speed;
        characterRigidbody.velocity = velocity;
    }
}
