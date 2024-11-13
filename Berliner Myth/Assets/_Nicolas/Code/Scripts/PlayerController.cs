using UnityEngine;
using UnityEngine.AI;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private NavMeshAgent agent;
    [SerializeField] private ParticleSystem clickEffect;
    [SerializeField] private InputActionReference move;
    [SerializeField] private InputActionReference speak;
    [SerializeField] private InputActionReference speakForBlue;
    [SerializeField] private InputActionReference speakForRed;

    private float lookRotationSpeed = 8f;

    private void Awake()
    {
        agent = gameObject.GetComponent<NavMeshAgent>();
    }

    private void OnEnable()
    {
        move.action.performed += OnMovePerformed;
        move.action.Enable();

        speak.action.performed += OnSpeechPerformed;
    }

    private void OnDisable()
    {
        move.action.performed -= OnMovePerformed;
        move.action.Disable();
    }

    private void Update()
    {
        FaceTarget();
    }

    #region Move
    private void OnMovePerformed(InputAction.CallbackContext ctx)
    {
        Move(); // Call Move() when the left mouse button is clicked
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
        //disable movement
        //switch player prefab to prefab of player on box

        //if released switch back to normal player 
    }

    #endregion
}