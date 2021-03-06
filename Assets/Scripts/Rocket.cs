﻿using UnityEngine;
using UnityEngine.SceneManagement;

public class Rocket : MonoBehaviour {

    [SerializeField] float rcsThrust = 250f;
    [SerializeField] float mainThrust = 1250f;
    [SerializeField] float levelLoadDelay = 2f;
    [SerializeField] AudioClip mainEngine;
    [SerializeField] AudioClip rocketExplosion;
    [SerializeField] AudioClip levelCompleted;

    [SerializeField] ParticleSystem mainEngineParticle;
    [SerializeField] ParticleSystem rocketExplosionParticle;
    [SerializeField] ParticleSystem levelCompletedParticle;

    Rigidbody rigidBody;
    AudioSource audioSource;    

    enum State { Alive, Dying, Transcending }
    State state = State.Alive;
    bool isCollisonOn = true;

	// Use this for initialization
	void Start () {
        rigidBody = GetComponent<Rigidbody>();
        audioSource = GetComponent<AudioSource>();

    }
	
	// Update is called once per frame
	void Update () {
        ProcessInput();
    }

    void OnCollisionEnter(Collision collision)
    {
        if (state != State.Alive) { return; }

        switch (collision.gameObject.tag)
        {
            case "Friendly":
                break;
            case "Finish":
                SuccessSequence();
                break;
            case "Enemy":
                if (isCollisonOn)
                    DeathSequence();
                break;
            case "Fuel":
                break;
        }

    }

    private void SuccessSequence()
    {
        state = State.Transcending;

        audioSource.Stop();
        audioSource.PlayOneShot(levelCompleted);

        mainEngineParticle.Stop();
        levelCompletedParticle.Play();

        Invoke("LoadNextScene", levelLoadDelay);
    }

    private void DeathSequence()
    {
        state = State.Dying;

        audioSource.Stop();
        audioSource.PlayOneShot(rocketExplosion);

        mainEngineParticle.Stop();
        rocketExplosionParticle.Play();

        Invoke("LoadCurrentScene", levelLoadDelay);
    }

    private void LoadCurrentScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    private void LoadNextScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1 >= SceneManager.sceneCountInBuildSettings ? 0 : SceneManager.GetActiveScene().buildIndex + 1);
    }

    private void ProcessInput()
    {
        if (state == State.Alive)
        {
            RespondToThrustInput();
            RespondToRotateInput();
        }

        if (Debug.isDebugBuild)
            RespondToDebugKeys();
    }

    private void RespondToDebugKeys()
    {
        if (Input.GetKeyDown(KeyCode.C))
            isCollisonOn = !isCollisonOn;

        if (Input.GetKeyDown(KeyCode.L))
            LoadNextScene();
    }

    private void RespondToThrustInput()
    {
        if (Input.GetKey(KeyCode.Space))
        {
            rigidBody.AddRelativeForce(Vector3.up * mainThrust * Time.deltaTime);
            if (!audioSource.isPlaying)
                audioSource.PlayOneShot(mainEngine);
            mainEngineParticle.Play();               
        }
        else
        {
            audioSource.Stop();
            mainEngineParticle.Stop();
        }
    }

    private void RespondToRotateInput()
    {
        rigidBody.freezeRotation = true;
        float rotationThisFrame = rcsThrust * Time.deltaTime;

        if (Input.GetKey(KeyCode.A))
        {            
            transform.Rotate(Vector3.forward * rotationThisFrame);
        }
        else if (Input.GetKey(KeyCode.D))
        {
            transform.Rotate(Vector3.back * rotationThisFrame);
        }

        rigidBody.freezeRotation = false;
    }
}
