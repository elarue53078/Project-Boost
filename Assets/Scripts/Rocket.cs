using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Rocket : MonoBehaviour
{
    Rigidbody rocketRB;
    AudioSource rocketAudio;
    [SerializeField] float mainThrust = 1000f;
    [SerializeField] float rcsThrust = 100f;
    [SerializeField] float levelLoadDelay = 2f;

    [SerializeField] AudioClip mainEngine;
    [SerializeField] AudioClip rocketExplosion;
    [SerializeField] AudioClip levelLoad;

    [SerializeField] ParticleSystem engineExhaust;
    [SerializeField] ParticleSystem debris;
    [SerializeField] ParticleSystem successSparkles;

    enum State {Alive, Dying, Transcending}
    State state = State.Alive;

    bool collisionsEnabled = true;

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
        // if(Debug.isDebugBuild) { RespondToDebug(); }
    }

    private void RespondToDebug()
    {
        if (Input.GetKeyDown(KeyCode.L))
        {
            LoadNextScene();
        }
        if (Input.GetKeyDown(KeyCode.C))
        {
            collisionsEnabled = !collisionsEnabled;
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
        debris.Play();
        Invoke("PlayerDie", levelLoadDelay); //parameterize time
    }

    private void StartSuccess()
    {
        print("Hit finish");
        state = State.Transcending;
        rocketAudio.Stop();
        rocketAudio.PlayOneShot(levelLoad);
        successSparkles.Play();
        Invoke("LoadNextScene", levelLoadDelay); //parameterize time
    }

    private void PlayerDie()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    private void LoadNextScene()
    {
        int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
        int nextSceneIndex = currentSceneIndex + 1 == SceneManager.sceneCountInBuildSettings ? 0 : currentSceneIndex + 1;
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
            StopApplyingThrust();
        }
    }

    private void StopApplyingThrust()
    {
        rocketAudio.Stop();
        engineExhaust.Stop();
    }

    private void ApplyThrust(float thrustSpeed)
    {
        print("Thrusting.");
        rocketRB.AddRelativeForce(Vector3.up * thrustSpeed);
        if (!rocketAudio.isPlaying) // no layering
        {
            rocketAudio.PlayOneShot(mainEngine);
        }
        engineExhaust.Play();
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
