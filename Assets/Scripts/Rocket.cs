using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rocket : MonoBehaviour
{
    Rigidbody rocketRB;
    AudioSource rocketThrust;
    [SerializeField] float mainThrust = 100f;
    [SerializeField] float rcsThrust = 100f;

    // Start is called before the first frame update
    void Start()
    {
        rocketRB = GetComponent<Rigidbody>();
        rocketThrust = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        Thrust();
        Rotate();
    }

    private void Thrust()
    {
        float thrustSpeed = mainThrust * Time.deltaTime;

        if (Input.GetKey(KeyCode.Space)) // can thrust while rotating
        {
            print("Thrusting.");
            rocketRB.AddRelativeForce(Vector3.up * thrustSpeed);
            if (!rocketThrust.isPlaying) // no layering
            {
                rocketThrust.Play();
            }
        }
        else
        {
            rocketThrust.Stop();
        }
    }

    private void Rotate()
    {
        rocketRB.freezeRotation = true;
        float rotationSpeed = rcsThrust * Time.deltaTime;

        if (Input.GetKey(KeyCode.A))
        {
            print("Rotating left");
            transform.Rotate(Vector3.forward * rotationSpeed);
        }
        else if (Input.GetKey(KeyCode.D))
        {
            print("Rotating right.");
            transform.Rotate(-Vector3.forward * rotationSpeed);
        }

        rocketRB.freezeRotation = false;
    }
}
