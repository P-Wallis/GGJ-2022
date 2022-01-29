using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Player : MonoBehaviour
{
    public enum Weapon { FIRE, ICE }

    public float speed = 1;
    public float rotationSpeed = 1;
    public Transform model;

    public ParticleSystem fireParticles, iceParticles;
    public Transform raycastPoint;
    public Transform cubeCreatePoint;

    public float maxFireDistance;

    private Camera m_camera;
    private Rigidbody m_rigidbody;
    private Plane m_groundPlane;
    private LayerMask iceLayerMask;
    private Weapon currentWeapon = Weapon.FIRE;
    void Start()
    {
        m_rigidbody = GetComponent<Rigidbody>();
        m_camera = Camera.main;
        m_groundPlane = new Plane(Vector3.up, 0);
        iceLayerMask = LayerMask.GetMask("Ice");
    }


    Vector3 movement;
    private void Update()
    {
        // Weapons
        switch (currentWeapon)
        {
            case Weapon.FIRE:
                if (Input.GetMouseButtonDown(0))
                {
                    fireParticles.Play();
                }

                if (Input.GetMouseButton(0))
                {
                    Burn();
                }

                if (Input.GetMouseButtonUp(0))
                {
                    fireParticles.Stop();
                }
                break;
            case Weapon.ICE:
                if (Input.GetMouseButtonDown(0))
                {
                    iceParticles.Play();
                }

                if (Input.GetMouseButton(0))
                {
                    Map._.CreateCubeAtWorldPosition(cubeCreatePoint.position);
                }

                if (Input.GetMouseButtonUp(0))
                {
                    iceParticles.Stop();
                }
                break;
        }

        // Switch Weapon
        if(Input.GetKeyDown(KeyCode.Space))
        {
            if (fireParticles.isPlaying)
                fireParticles.Stop();
            if (iceParticles.isPlaying)
                iceParticles.Stop();

            currentWeapon = currentWeapon == Weapon.FIRE ? Weapon.ICE : Weapon.FIRE;

            if (Input.GetMouseButton(0))
            {
                switch(currentWeapon)
                {
                    case Weapon.FIRE:
                        fireParticles.Play();
                        break;
                    case Weapon.ICE:
                        iceParticles.Play();
                        break;
                }
            }
        }


        // Rotation
        Vector3 cursorPoint = ScreenToGroundPoint(Input.mousePosition);

        Quaternion newRot = Quaternion.Euler(0, Mathf.Atan2(cursorPoint.x - transform.position.x, cursorPoint.z - transform.position.z) * Mathf.Rad2Deg, 0);
        model.transform.rotation = newRot; //Quaternion.Lerp(model.transform.rotation, newRot, rotationSpeed * Time.deltaTime);


        // Movement
        movement = new Vector3(
            Input.GetAxisRaw("Horizontal"),
            0,
            Input.GetAxisRaw("Vertical")
            ).normalized;
    }

    void FixedUpdate()
    {
        m_rigidbody.velocity = movement * speed;
    }

    private Vector3 ScreenToGroundPoint(Vector2 screenPos)
    {
        Ray ray = m_camera.ScreenPointToRay(screenPos);

        float enter = 0.0f;
        if (m_groundPlane.Raycast(ray, out enter))
        {
            return ray.GetPoint(enter);
        }
        return Vector3.zero;
    }

    void Burn()
    {
        RaycastHit hit;
        if(Physics.Raycast(raycastPoint.position, raycastPoint.forward, out hit, maxFireDistance, iceLayerMask))
        {
            GameObject hitObject = hit.collider != null ? hit.collider.gameObject : null;
            IceCube iceCube = hitObject !=null? hitObject.GetComponent<IceCube>() : null;

            if(iceCube !=null)
            {
                iceCube.Burn();
            }
        }
    }
}
