using UnityEngine;
using UnityEngine.SceneManagement;

public class Rocket : MonoBehaviour
{
    Rigidbody rigidBody;
    AudioSource audioSource;
    [SerializeField] float rcsThrust = 100f;
    [SerializeField] float mainThrust = 100f;

    [SerializeField] float levelLoadDelay = 1f;

    [SerializeField] AudioClip mainEngine;
    [SerializeField] AudioClip deathNoise;
    [SerializeField] AudioClip finishNoise;

    [SerializeField] ParticleSystem mainEngineParticles;
    [SerializeField] ParticleSystem finishParticles;
    [SerializeField] ParticleSystem deathParticles;

    enum State { Alive, Dying, Transcending };
    State state = State.Alive;

    bool collisionsEnabled = true;

    // Start is called before the first frame update
    void Start()
    {
        rigidBody = GetComponent<Rigidbody>();
        audioSource = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        if (state == State.Alive)
        {
            RespondToThrust();
            Rotate();
        }
        if (Debug.isDebugBuild)
        {
            RespondToDebugKeys();
        }
    }

    private void RespondToDebugKeys()
    {
        if (Input.GetKeyDown(KeyCode.L)) {
            LoadNextScene();
        }
        else if (Input.GetKeyDown(KeyCode.C)) {
            collisionsEnabled = !collisionsEnabled;
        }
    }

    private void Rotate()
    {
        float rotationSpeed = rcsThrust * Time.deltaTime;

        if (Input.GetKey(KeyCode.A) && !Input.GetKey(KeyCode.D))
        {
            rigidBody.freezeRotation = true;
            transform.Rotate(Vector3.forward * rotationSpeed);
            rigidBody.freezeRotation = false;
        }
        else if (!Input.GetKey(KeyCode.A) && Input.GetKey(KeyCode.D))
        {
            rigidBody.freezeRotation = true;
            transform.Rotate(-Vector3.forward * rotationSpeed);
            rigidBody.freezeRotation = false;
        }

    }

    private void RespondToThrust()
    {
        if (Input.GetKey(KeyCode.Space))
        {
            Thrust();
        }
        else
        {
            audioSource.Stop();
            if (mainEngineParticles.isPlaying)
            {
                mainEngineParticles.Stop();
            }
        }
    }

    private void Thrust()
    {
        float thrustPerFrame = mainThrust * Time.deltaTime;
        rigidBody.AddRelativeForce(Vector3.up * thrustPerFrame);
        if (!audioSource.isPlaying)
        {
            audioSource.PlayOneShot(mainEngine);
            mainEngineParticles.Play();
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        if (state != State.Alive || !collisionsEnabled) {return;}

        switch (collision.gameObject.tag) {
            case "Friendly":
                break;
            case "Finish":
                state = State.Transcending;
                audioSource.Stop();
                audioSource.PlayOneShot(finishNoise);
                finishParticles.Play();
                Invoke("LoadNextScene", levelLoadDelay);
                break;
            default:
                state = State.Dying;
                audioSource.Stop();
                audioSource.PlayOneShot(deathNoise);
                deathParticles.Play();
                Invoke("LoadFirstLevel", levelLoadDelay);
                break;
        }
    }

    private void LoadFirstLevel()
    {
        SceneManager.LoadScene(0);
    }

    private void LoadNextScene()
    {
        int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
        if (currentSceneIndex == SceneManager.sceneCountInBuildSettings - 1)
        {
            currentSceneIndex = -1;
        }
        SceneManager.LoadScene(currentSceneIndex + 1); 
    }
}
