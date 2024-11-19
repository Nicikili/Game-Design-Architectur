using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.ProBuilder.MeshOperations;

public class NpcWander : NpcComponent
{
    public Area Area;
    public Area PlayerArea;

    [SerializeField] float maxWaitTime = 3f;
    [SerializeField] float maxWaitTimeRandom = 5f;
   
    float maxWanderTime = 5f;

    enum Estate
    {
        Wandering,
        Waiting,
        Gathering,
        Listening
    }

    [SerializeField] Estate state = Estate.Wandering;
    [SerializeField] private float waitTime = 0f;
    [SerializeField] private float wanderTime = 0f; 

    private void Start()
    {
        if (Random.Range(0f, 100.0f) > 50f)
        {
            ChangeState(Estate.Wandering);
        }
        else
        {
            ChangeState(Estate.Waiting);
        }
    }

    private void Update()
    {
        if (state == Estate.Waiting)
        {
            waitTime -= Time.deltaTime;
            if (!npc.controller.isSpeaking)
            {

                if (waitTime < 0f)
                {
                    ChangeState(Estate.Wandering);
                }
            }
            else
            {
                ChangeState (Estate.Gathering);
            }
        }
        else if (state == Estate.Listening)
        {
            if (!npc.controller.isSpeaking)
            {
                ChangeState (Estate.Wandering);
            }
        }
        else if (state == Estate.Wandering)
        {
            if (!npc.controller.isSpeaking)
            {
                if (HasArrived() || wanderTime < 0f)
                {
                    ChangeState(Estate.Waiting);
                }
            }
            else
            {
                ChangeState(Estate.Gathering);
            }
        }
        else if (state == Estate.Gathering)
        {
            if (HasArrived() || wanderTime < 0f)
            {
                ChangeState(Estate.Listening);
            }
        }
    }

    private void ChangeState(Estate newstate)
    {
        state = newstate;

        if (state == Estate.Wandering)
        {
            npc.agent.isStopped = false;

            SetRandomDestination();

            wanderTime = maxWanderTime;
        }
        else if (state == Estate.Listening)
        {
            npc.agent.isStopped = true; 
        }
        else if (state == Estate.Waiting)
        {
            waitTime = maxWaitTime + Random.Range(0f, maxWaitTimeRandom);

            npc.agent.isStopped = true;
        }
        else if (state == Estate.Gathering)
        {
            npc.agent.isStopped = false;

            SetPlayerDestination();
        }
          
    }

    bool HasArrived()
    {
        return npc.agent.remainingDistance <= npc.agent.stoppingDistance;
    }

    void SetRandomDestination()
    {
        npc.agent.SetDestination(Area.GetRandomPoint());
    }

    void SetPlayerDestination()
    {
        npc.agent.SetDestination(PlayerArea.GetRandomPoint());
    }
}

//Reminder - unstuck over time when not reached destination - do not gather when not speaking - if stop speaking go directly to new waypoint - pusher to get a path through people