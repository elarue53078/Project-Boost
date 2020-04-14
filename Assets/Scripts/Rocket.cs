using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Rocket : MonoBehaviour
{
    Rigidbody rocketRB;
    AudioSource rocketAudio;
    [SerializeField] float mainThrust = 100f;
    [SerializeField] float rcsThrust = 100f;
    [SerializeField] AudioClip mainEngine;
    [SerializeField] AudioClip rocketExplosion;
    [SerializeField] AudioClip levelLoad;

    enum State {Alive, Dying, Transcending}
    State state = State.Alive;

    // Start is called before the first frame update
    void Start()
    {
        rocketRB = GetComponent<Rigidbody>();
        rocketAudio = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        if(state == State.Alive)
        {
            RespondToThrustInput();
            RespondToRotateInput();
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if(state != State.Alive) { return; }

        print("collided");
        switch (collision.gameObject.tag)
        {
            case "Friendly":
                //do nothing
                print("Okay");
                break;
            case "Finish":
                StartSuccess();
                break;
            default:
                StartExplosion();
                break;

        }
    }

    private void StartExplosion()
    {
        print("dead");
        state = State.Dying;
        rocketAudio.Stop();
        rocketAudio.PlayOneShot(rocketExplosion);
        Invoke("PlayerDie", 2f); //parameterize time
    }

    private void StartSuccess()
    {
        print("Hit finish");
        state = State.Transcending;
        rocketAudio.Stop();
        rocketAudio.PlayOneShot(levelLoad);
        Invoke("LoadNextScene", 1f); //parameterize time
    }

    private void PlayerDie()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    private void LoadNextScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

    private void RespondToThrustInput()
    {
        float thrustSpeed = mainThrust * Time.deltaTime;

        if (Input.GetKey(KeyCode.Space)) // can thrust while rotating
        {
            ApplyThrust(thrustSpeed);
        }
        else
        {
            rocketAudio.Stop();
        }
    }

    private void ApplyThrust(float thrustSpeed)
    {
        print("Thrusting.");
        rocketRB.AddRelativeForce(Vector3.up * thrustSpeed);
        if (!rocketAudio.isPlaying) // no layering
        {
            rocketAudio.PlayOneShot(mainEngine);
        }
    }

    private void RespondToRotateInput()
    {
        rocketRB.freezeRotation = true;
        float rotationSpeed = rcsThrust * Time.deltaTime;

        if (Input.GetKey(KeyCode.A))
        {
            print("Rotating left");
            transform.Rotate(-Vector3.forward * rotationSpeed);
        }
        else if (Input.GetKey(KeyCode.D))
        {
            print("Rotating right.");
            transform.Rotate(Vector3.forward * rotationSpeed);
        }

        rocketRB.freezeRotation = false;
    }
}
