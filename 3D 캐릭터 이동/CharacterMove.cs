using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterMove : MonoBehaviour
{
    public float speed = 5f; // �̵��ӵ�
    private Rigidbody characterRigidbody;

    void Start()
    {
        characterRigidbody = GetComponent<Rigidbody>();
    }

    void Update()
    {
        float inputX = Input.GetAxis("Horizontal"); //�¿�
        float inputZ = Input.GetAxis("Vertical");   //�յ� 
        // -1 ~ 1

        Vector3 velocity = new Vector3(inputX, 0, inputZ); // 3����
        velocity *= speed;
        characterRigidbody.velocity = velocity;
    }
}
