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

    public string GroupName { get; private set; }
    public Color GroupColor { get; private set; }


    public float CurrentSpeed
    {
        get { return agent.velocity.magnitude; }
    }

    private void Awake()
    {
        animator = GetComponent<Animator>();
        agent = GetComponent<NavMeshAgent>();
        controller = player.GetComponent<PlayerController>();
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
}
