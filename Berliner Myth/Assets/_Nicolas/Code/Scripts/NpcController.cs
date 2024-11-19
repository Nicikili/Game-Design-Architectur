using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
public class NpcController : MonoBehaviour
{
    [SerializeField] public NavMeshAgent agent;
    [SerializeField] public Animator animator;
    [SerializeField] public GameObject player;

    public PlayerController controller;

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
}
