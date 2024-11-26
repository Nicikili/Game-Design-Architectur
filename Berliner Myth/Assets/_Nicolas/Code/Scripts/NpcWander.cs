using UnityEngine;

public class NpcWander : NpcComponent
{
    public Area Area;
    public Area PlayerArea;

    [SerializeField] float maxWaitTime = 3f;
    [SerializeField] float maxWaitTimeRandom = 5f;

    [SerializeField] float maxWanderTime = 5f;
    [SerializeField] float maxWanderTimeRandom = 7f;

    enum Estate
    {
        Wandering,
        Waiting,
        Gathering,
        Listening,
        Following,
        AttackingOthers,
        AttackingPlayer
    }

    [SerializeField] Estate state = Estate.Wandering;
    [SerializeField] private float waitTime = 0f;
    [SerializeField] private float wanderTime = 0f;

    [SerializeField] private float maxDistanceToPlayer = 10f;

   

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
        //if (approval<=0)
        //{
        //    //go aggro against player
        //}
        //else if (approval>0.0f && approval<=100f)
        //} //Standart NPC Behavior
        //else if (approval>100f && approval<200) // NPC Follow Mode
        //{
        //    //follow player
        //}
        //else if (approval>=200f) // NPC Attacking mode
        //{
        //    //attack other npcs
        //}
        //else
        //{
        //    Debug.Log("Where is the approval AHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHH !!!!!!");
        //}

        float distanceToPlayer = Vector3.Distance(transform.position, PlayerArea.transform.position);

        if (distanceToPlayer < maxDistanceToPlayer)
        {
            Debug.DrawLine(transform.position, PlayerArea.transform.position, Color.red);
        }

        if (state == Estate.Waiting) //// Waits around
        {
            waitTime -= Time.deltaTime;
            if (!npc.controller.startSpeech) //If player is not doing a Speech continue by wandering around
            {
                if (waitTime < 0f)
                {
                    ChangeState(Estate.Wandering);
                }
            }
            else if (npc.controller.startSpeech && distanceToPlayer < maxDistanceToPlayer) //Go to the player doing the speech
            {
                ChangeState(Estate.Gathering);
            }
        }
        else if (state == Estate.Listening) //// Listen the Speech
        {
            if (!npc.controller.startSpeech) // If player stops the Speech go back to wandering 4/5 or waiting 1/5
            {
                if (Random.Range(0f, 100.0f) > 20f)
                {
                    ChangeState(Estate.Wandering);
                }
                else
                {
                    ChangeState(Estate.Waiting);
                }
            }
        }
        else if (state == Estate.Wandering) //// Wander around
        {
            wanderTime -= Time.deltaTime;
            if (!npc.controller.startSpeech)
            {
                if (HasArrived() || wanderTime < 0f) // If has arrived at told location or wandered around for too long change to waiting
                {
                    ChangeState(Estate.Waiting);
                }
            }
            else if (npc.controller.startSpeech && distanceToPlayer < maxDistanceToPlayer)
            {
                ChangeState(Estate.Gathering);
            }
        }
        else if (state == Estate.Gathering) //// Go towards the player if near her
        {
            if (HasArrived()) //If arrives at the player position
            {
                ChangeState(Estate.Listening); // change to listening
            }
            if (!npc.controller.startSpeech) //If player stops the speech cancel path going towards the player
            {
                if (Random.Range(0f, 100.0f) > 20f) //Tells npc to go wandering 4/5 or waiting 1/5
                {
                    ChangeState(Estate.Wandering);
                }
                else
                {
                    ChangeState(Estate.Waiting);
                }
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

            wanderTime = maxWanderTime + Random.Range(0f, maxWanderTimeRandom);
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
        else if (state == Estate.Following)
        {
            npc.agent.isStopped = false;
            //Npc goes into follow mode, following the player, set destination to players destination
        }
        else if (state == Estate.AttackingOthers)
        {
            npc.agent.isStopped = false;
            //Violence mode activated, Npc will attack other Npc
            //Set destination towards closest npc
            //punches each npc thats opposite of itself
        }
        else if (state == Estate.AttackingPlayer)
        {
            npc.agent.isStopped = false;

            //Set desintation towards player
            //Goes into Violence mode against player
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

//Reminder - pusher to get a path through people