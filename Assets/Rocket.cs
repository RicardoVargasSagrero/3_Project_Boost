using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Rocket : MonoBehaviour
{
    //In this program we will proces input
    
    //With the [SerializeField] we're able to see the variable in the Unity Inspector
    [SerializeField] float rcsThrust = 100f;
    [SerializeField] float mainThrust = 100f;
    [SerializeField] float levelLoadDelay = 2f;
    //Audio resourses
    [SerializeField] AudioClip mainEngine;
    [SerializeField] AudioClip death;
    [SerializeField] AudioClip success;
    //Particle System Resource
    [SerializeField] ParticleSystem mainEngineParticles;
    [SerializeField] ParticleSystem deathParticles;
    [SerializeField] ParticleSystem successParticles;

    Rigidbody rigidBody;
    AudioSource audioSource;

    //States of the game
    enum State { Alive,Dying,Transcending};
    State state = State.Alive;
    bool collisionAreEnabled = true;
    // Start is called before the first frame update
    void Start()
    {
        /*the <> arrows will ther the method to access only to the RigidBody Component */
        rigidBody = GetComponent<Rigidbody>();//Provided by Unity
        audioSource = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        // todo Stop the death sound
        if(state == State.Alive)
        {
            RespondToThrustInput();
            RespondToRotateInput();
        }
        else if (state == State.Dying)
        {
            
        }
        else if (state == State.Transcending)
        {
            //Do nothing
        }
        if (Debug.isDebugBuild) {
            RespondToDebugKeys();
        }
    }

    private void RespondToDebugKeys()
    {
        if (Input.GetKeyDown(KeyCode.L))
        {
            LoadNextLevel();
        }   
        else if (Input.GetKeyDown(KeyCode.C))
        {
            collisionAreEnabled = !collisionAreEnabled;
        }
        
    }

    void OnCollisionEnter(Collision collision)
    {
        if(state != State.Alive || !collisionAreEnabled) { return; }
        switch (collision.gameObject.tag)
        {
            case "Friendly":
                break;
            case "Finish":
                if(state != State.Transcending)
                    StartSuccessSequence();
                break;
            default: //What ever is untagged it will be the default option
                if (state != State.Dying)
                {
                    StartDeathSequence();
                    const float tau = Mathf.PI * 2f;
                    print(Mathf.Sin(tau / 4f));
                }
                break;
        } 
    }
    private void StartDeathSequence()
    {
        state = State.Dying;
        print("Obstacle");
        audioSource.Stop();
        audioSource.PlayOneShot(death);
        deathParticles.Play();
        Invoke("LoadFirstLevel", levelLoadDelay);
    }
    private void StartSuccessSequence()
    {
        state = State.Transcending;
        audioSource.Stop();
        audioSource.PlayOneShot(success);
        successParticles.Play();
        print("Hit finish");
        Invoke("LoadNextLevel", levelLoadDelay);

    }
    private void LoadNextLevel()
    {
        /*This variable loads the Index of the Scene*/
        int currentScenceIndex = SceneManager.GetActiveScene().buildIndex;
        int nextSceneLoader = currentScenceIndex + 1;
        if (nextSceneLoader == SceneManager.sceneCountInBuildSettings)
        {
            SceneManager.LoadScene(0);
            return;
        }
        SceneManager.LoadScene(nextSceneLoader);

    }
    private void LoadFirstLevel()
    {
        SceneManager.LoadScene(0);
    }
    private void RespondToThrustInput()
    {
        if (Input.GetKey(KeyCode.Space))
        {
            ApplyThrust();
        }
        else
         {
            audioSource.Stop();
            mainEngineParticles.Stop();
        }
    }
    private void ApplyThrust()
    {
        rigidBody.AddRelativeForce(Vector3.up * mainThrust * Time.deltaTime);
        if (!audioSource.isPlaying)
            audioSource.PlayOneShot(mainEngine);
        mainEngineParticles.Play();
    }
    private void RespondToRotateInput()
    {
        
        float rotationThisFrame = rcsThrust * Time.deltaTime;
        if (Input.GetKey(KeyCode.LeftArrow))
        {
            rigidBody.freezeRotation = true; //Take manual control of rotation
            transform.Rotate(Vector3.forward * rotationThisFrame);
            rigidBody.freezeRotation = false; //resume physicss control of rotation
        }
        else if (Input.GetKey(KeyCode.RightArrow))
        {
            rigidBody.freezeRotation = true;
            transform.Rotate(-Vector3.forward * rotationThisFrame);
            rigidBody.freezeRotation = false;
        }
        else
        {
            //print("Nothing is being pressed");
        }
        
    }

    
}
