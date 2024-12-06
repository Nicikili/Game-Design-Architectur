using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.AI;

[SelectionBase]
public class NpcController : MonoBehaviour
{

    [SerializeField] public NavMeshAgent agent;
    [SerializeField] public Animator animator;
    [SerializeField] public GameObject player;

    public PlayerController controller;

    public NpcController LastAttacker {  get; private set; }    

    public string GroupName { get; private set; } 
    public Color GroupColor { get; private set; }

    public float Approval { get; private set; } = 100f; // Neutral starting value (range 0 - 200)

    public float CurrentHealth { get; set; }

    [SerializeField] private float maxHealth = 50f;

    private Camera mainCamera;

    private NpcWander wanderComponent;

    public float CurrentSpeed
    {
        get { return agent.velocity.magnitude; }
    }

    private void Awake()
    {
        animator = GetComponent<Animator>();
        agent = GetComponent<NavMeshAgent>();
        controller = player.GetComponent<PlayerController>();
        mainCamera = Camera.main;
        wanderComponent = GetComponent<NpcWander>();
    }

    private void Start()
    {
        CurrentHealth = maxHealth;
    }

    public void SetGroup(string groupName, Color groupColor)
    {
        GroupName = groupName;
        GroupColor = groupColor;

        Renderer renderer = GetComponentInChildren<Renderer>();
        if (renderer != null)
        {
            renderer.material.color = groupColor;
        }
    }

    public void SwitchGroup(string newGroupName)
    {
        NpcGroupManager groupManager = FindObjectOfType<NpcGroupManager>();

        if (groupManager != null)
        {
            NpcGroupManager.Group currentGroup = groupManager.GetGroupByName(GroupName);
            if (currentGroup != null)
            {
                currentGroup.members.Remove(this);
            }

            NpcGroupManager.Group newGroup = groupManager.GetGroupByName(newGroupName);
            if (newGroup != null)
            {
                newGroup.members.Add(this);
                SetGroup(newGroupName, newGroup.groupColor);
            }
            else
            {
                GroupName = newGroupName;
            }

        }
    }


    public void AdjustApproval(float amount)
    {
        Approval = Mathf.Clamp(Approval + amount, 0f, 200f); // Keep Approval between 0 and 200
    }

    public void TakenDamage(float damage, NpcController attacker)
    {
        CurrentHealth = Mathf.Max(CurrentHealth - damage, 0);

        if (attacker != null)
        {
            LastAttacker = attacker; //store the attacker
        }

        if (CurrentHealth <= 0f)
        {
            Die();
        }
    }

    private void Die()
    {
        Destroy(gameObject);
    }

    private void OnGUI() //Display above each NPC affiliation and Aprroval Count
    {
        Vector3 screenPosition = mainCamera.WorldToScreenPoint(transform.position + Vector3.up * 1.5f);

        if(screenPosition.z > 0) // only renders if NPC is in front of the camera
        {
            float labelWidth = 150f;
            float labelHeight = 50f;

            Rect rect = new Rect(
                screenPosition.x - labelWidth /2,
                Screen.height - screenPosition.y - labelHeight,
                labelWidth,
                labelHeight

                );

            GUIStyle style = new GUIStyle(GUI.skin.label)
            {
                alignment = TextAnchor.MiddleCenter,
                fontSize = 14,
                normal = { textColor = Color.white },
            };

            string npcState = wanderComponent != null ? wanderComponent.GetCurrentState() : "Unknown";

            GUI.Label(rect, $"{GroupName}: {Approval:F0}\nState: {npcState}\nHit: {CurrentHealth}", style );
        }
    }
}
