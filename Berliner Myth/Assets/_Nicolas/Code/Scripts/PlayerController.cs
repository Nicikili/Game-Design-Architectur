using UnityEngine;
using UnityEngine.AI;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

[SelectionBase]
public class PlayerController : MonoBehaviour
{
    [SerializeField] private NavMeshAgent agent;
    [SerializeField] private ParticleSystem clickEffect;
    [SerializeField] private InputActionReference move;
    [SerializeField] private InputActionReference speak;
    [SerializeField] private InputActionReference speakForBlue;
    [SerializeField] private InputActionReference speakForRed;

    [SerializeField] private GameObject red;
    [SerializeField] private GameObject blue;

    [SerializeField] private Animator animator;

    private float lookRotationSpeed = 8f;

    public bool isSpeaking = false;
    public bool startSpeech = false;

    private void Awake()
    {
        animator = GetComponent<Animator>();
        agent = gameObject.GetComponent<NavMeshAgent>();
    }
    private void Update()
    {
        FaceTarget();
    }

    #region Manage Subscription
    private void OnEnable()
    {
        move.action.performed += OnMovePerformed;
        move.action.Enable();

        speak.action.performed += OnSpeechPerformed;
        speak.action.Enable();

        speakForBlue.action.performed += BlueSpeechPerformed;
        speakForBlue.action.canceled += BlueSpeechCanceled;
        speakForBlue.action.Enable();

        speakForRed.action.performed += RedSpeechPerformed;
        speakForRed.action.canceled += RedSpeechCanceled;
        speakForRed.action.Enable();
    }

    private void OnDisable()
    {
        move.action.performed -= OnMovePerformed;
        move.action.Disable();

        speak.action.performed -= OnSpeechPerformed;
        speak.action.Disable();

        speakForBlue.action.performed -= BlueSpeechPerformed;
        speakForBlue.action.canceled -= BlueSpeechCanceled;
        speakForBlue.action.Disable();

        speakForRed.action.performed -= RedSpeechPerformed;
        speakForRed.action.canceled -= RedSpeechCanceled;
        speakForRed.action.Disable();
    }
    #endregion

    #region Move
    private void OnMovePerformed(InputAction.CallbackContext ctx)
    {
        if (!isSpeaking) //Check if the Player is Speaking, aka has clicked on Space
        {

            Move(); // Call Move() when the left mouse button is clicked

        }
    }

    private void Move()
    {
        RaycastHit hit;
        if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit, Mathf.Infinity))
        {
            if (hit.collider.CompareTag("Ground"))
            {
                NavMeshPath path = new NavMeshPath();
                agent.CalculatePath(hit.point, path);

                // Visualize the path using lines (DEBUG)
                LineRenderer lineRenderer = GetComponent<LineRenderer>();
                lineRenderer.positionCount = path.corners.Length;
                lineRenderer.SetPositions(path.corners);

                agent.SetDestination(hit.point);
                agent.stoppingDistance = 0;
                if (clickEffect != null)
                {
                    ParticleSystem instantiatedEffect = Instantiate(clickEffect, hit.point + new Vector3(0, 0.1f, 0), clickEffect.transform.rotation);
                    Destroy(instantiatedEffect.gameObject, instantiatedEffect.main.duration);
                }
            }
        }
    }

    private void FaceTarget()
    {

        if (isSpeaking || !agent.hasPath)
            return;

        if (agent.hasPath)
        {
            Vector3 direction = (agent.steeringTarget - transform.position).normalized;
            Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));
            transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * lookRotationSpeed);
        }
        else if (agent.velocity != Vector3.zero)
        {
            Vector3 direction = (agent.destination - transform.position).normalized;
            Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));
            transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * lookRotationSpeed);
        }
    }
    #endregion

    #region Speech

    private void OnSpeechPerformed(InputAction.CallbackContext ctx)
    {
        if (isSpeaking)
        {
            StopSpeak();
            StopSpeech();
        }
        else
        {
            Speak();
        }
    }

    private void BlueSpeechPerformed(InputAction.CallbackContext ctx)
    {
        if (isSpeaking)
        {
            BlueSpeech();
        }
    }

    private void BlueSpeechCanceled(InputAction.CallbackContext ctx)
    {
        if (isSpeaking)
        {
            Debug.Log("Stopped Speaking for Blue");
            animator.SetBool("IsSpeaking", false);
            blue.SetActive(false);

            startSpeech =false;
        }
    }

    private void RedSpeechPerformed(InputAction.CallbackContext ctx)
    {
        if (isSpeaking)
        {
            RedSpeech();
        }
    }

    private void RedSpeechCanceled(InputAction.CallbackContext ctx)
    {
        if (isSpeaking)
        {
            Debug.Log("Stopped Speaking for Red");
            animator.SetBool("IsSpeaking", false);
            red.SetActive(false);

            startSpeech = false;
        }
    }

    private void Speak()
    {
        Debug.Log("I am speaking");

        //Disable Player Movement
        isSpeaking = true;
        agent.ResetPath();

        //Lock Rotation
        agent.updateRotation = false;
        Vector3 forwardDirection = transform.forward;   
        transform.rotation = Quaternion.LookRotation(forwardDirection);

        //Start Animation On Box

        animator.SetBool("IsBox", true);


    }

    private void StopSpeak()
    {
        Debug.Log("Stopped speaking");

        //Enable Player Movement
        isSpeaking = false;

        //Unlock Rotation
        agent.updateRotation = true;

        //Stop Animation On Box

        animator.SetBool("IsBox", false);
    }

    private void BlueSpeech()
    {
        Debug.Log("Start Speaking for Blue");

        animator.SetBool("IsSpeaking", true);

        blue.SetActive(true);
        red.SetActive(false);

        startSpeech = true;
    }

    private void RedSpeech()
    {
        Debug.Log("Start Speaking for Red");

        animator.SetBool("IsSpeaking", true);

        red.SetActive(true);
        blue.SetActive(false);

        startSpeech = true;
    }

    private void StopSpeech()
    {
        animator.SetBool("IsSpeaking", false );

        red.SetActive(false);
        blue.SetActive(false);

        startSpeech = false;
    }
    #endregion
}