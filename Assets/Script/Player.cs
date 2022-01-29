using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Player : MonoBehaviour
{
    public float speed = 1;
    public float rotationSpeed = 1;
    public Transform model;

    private Rigidbody m_rigidbody;
    void Start()
    {
        m_rigidbody = GetComponent<Rigidbody>();
    }

    Vector3 movement;
    private void Update()
    {
        movement = new Vector3(
            Input.GetAxis("Horizontal"),
            0,
            Input.GetAxis("Vertical")
            );

        Quaternion newRot = model.transform.rotation;
        if (movement.magnitude > 0.01f)
        {
            newRot = Quaternion.Euler(0, Mathf.Atan2(movement.x, movement.z) * Mathf.Rad2Deg, 0);
        }
        model.transform.rotation = Quaternion.Lerp(model.transform.rotation, newRot, rotationSpeed * Time.deltaTime);
    }

    void FixedUpdate()
    {
        m_rigidbody.velocity = movement * speed;
    }
}
