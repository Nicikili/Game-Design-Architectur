using DG.Tweening;
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

    //NPC Reaction Bubble
    [SerializeField] private GameObject reactionBubblePrefab; 
    [SerializeField] private Sprite positiveRectionSprite;
    [SerializeField] private Sprite negativeRectionSprite;

    private GameObject reactionBubbleInstance;
    private SpriteRenderer reactionSpriteRenderer;

    //NPC Death

    [SerializeField] private GameObject redDeadBodyPrefab;
    [SerializeField] private GameObject blueDeadBodyPrefab;
    [SerializeField] private GameObject bloodPoolPrefab;

    private bool hasDied = false;

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

        //Instantiate and configure the reaction bubble
        if (reactionBubblePrefab != null)
        {
            reactionBubbleInstance = Instantiate(reactionBubblePrefab, transform);
            reactionBubbleInstance.transform.localPosition = new Vector3(0, 2, 0); // Position above the NPC

            // Reset the local rotation to prevent inheriting NPC's rotation
            reactionBubbleInstance.transform.localRotation = Quaternion.identity;

            reactionSpriteRenderer = reactionBubbleInstance.GetComponent<SpriteRenderer>();
            reactionBubbleInstance.SetActive(false); // Hide initially
        }
        if (reactionBubbleInstance != null && Camera.main != null)
        {
            // Calculate direction to the camera while locking the Y-axis
            Vector3 direction = Camera.main.transform.position - reactionBubbleInstance.transform.position;
            direction.y = 0; // Lock vertical rotation to keep the bubble upright
            reactionBubbleInstance.transform.rotation = Quaternion.LookRotation(direction);
        }

    }

    private void Update()
    {
        if (reactionBubbleInstance != null && Camera.main != null)
        {
            // Calculate direction to the camera while locking the Y-axis
            Vector3 direction = Camera.main.transform.position - reactionBubbleInstance.transform.position;
            direction.y = 0; // Lock vertical rotation to keep the bubble upright
            reactionBubbleInstance.transform.rotation = Quaternion.LookRotation(direction);
        }
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
            AudioManager.instance.PlayOneShot(FMODEvents.instance.VL_NPC_ScreamsInPain, this.transform.position);
            Die();
        }
    }

    private void Die()
    {
        if (hasDied) return; // Exit if the NPC has already died
        hasDied = true;

        GameObject deadBodyPrefab = null;

        if (GroupName == "Red")
        {
            deadBodyPrefab = redDeadBodyPrefab;
        }
        else if (GroupName == "Blue")
        {
            deadBodyPrefab = blueDeadBodyPrefab;
        }

        Quaternion randomRotation = Quaternion.Euler(0, Random.Range(0f, 360f), 0);

        if (deadBodyPrefab != null) 
        {
            Instantiate(deadBodyPrefab, transform.position, randomRotation);
        }

        Vector3 bloodOffset = new Vector3(Random.Range(-0.5f, 0.5f), 0, Random.Range(-0.5f, 0.5f));
        Vector3 bloodPosition = transform.position + bloodOffset;

        if (bloodPoolPrefab != null)
        {
            GameObject bloodPool = Instantiate(bloodPoolPrefab, bloodPosition, Quaternion.identity);
            bloodPool.transform.localScale = Vector3.zero;

            bloodPool.transform.DOScale(new Vector3(1f, 1f, 1f), 2f)
                .SetEase(Ease.OutQuad);
        }

        Destroy(gameObject);
    }

    public void ShowPositivReaction()
    {
        ShowReaction(positiveRectionSprite);
        AudioManager.instance.PlayOneShot(FMODEvents.instance.VL_NPC_Approves, this.transform.position);
    }

    public void ShowNegativReaction()
    {
       ShowReaction(negativeRectionSprite);
       AudioManager.instance.PlayOneShot(FMODEvents.instance.VL_NPC_Disapproves, this.transform.position);
    }

    private void ShowReaction(Sprite reactionSprite)
    {
        if (reactionSpriteRenderer)
        {
            reactionSpriteRenderer.sprite = reactionSprite;
            reactionBubbleInstance.SetActive(true);

            StartCoroutine(HideReactionBubble());
        }
    }

    private IEnumerator HideReactionBubble()
    {
        yield return new WaitForSeconds(2f); //Display for 2 seconds
        if (reactionBubbleInstance != null)
        {
            reactionBubbleInstance.SetActive(false);
        }
    }

    private void OnGUI() //Display above each NPC affiliation and Aprroval Count
    {
        Vector3 screenPosition = mainCamera.WorldToScreenPoint(transform.position + Vector3.up * 1.5f);

        if(screenPosition.z > 0) // only renders if NPC is in front of the camera
        {
            float labelWidth = 200f;
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

            //GUI.Label(rect, $"{GroupName}: {Approval:F0}\nState: {npcState}\nHit: {CurrentHealth}", style );
        }
    }
}
