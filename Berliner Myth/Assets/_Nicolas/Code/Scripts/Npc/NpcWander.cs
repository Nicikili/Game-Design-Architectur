using System.IO.Pipes;
using UnityEngine;

public class NpcWander : NpcComponent
{
    public Area Area;
    public SpeechArea PlayerArea;

    [SerializeField] float maxWaitTime = 3f;
    [SerializeField] float maxWaitTimeRandom = 5f;

    [SerializeField] float maxWanderTime = 5f;
    [SerializeField] float maxWanderTimeRandom = 7f;

    [SerializeField] float maxAggroTime = 5f;
    [SerializeField] float maxAggroTimeRandom = 7f;

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
    [SerializeField] private float aggroTime = 0f;

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

    public string GetCurrentState()
    {
        return state.ToString();
    }

    private void Update()
    {

        float distanceToPlayer = Vector3.Distance(transform.position, PlayerArea.transform.position);

        if (distanceToPlayer < maxDistanceToPlayer)
        {
            Debug.DrawLine(transform.position, PlayerArea.transform.position, Color.red);
        }


        if (npc.Approval <= 0)
        {
            aggroTime -= Time.deltaTime;

            ChangeState(Estate.AttackingPlayer);

            if (aggroTime <= 0f)
            {
                npc.AdjustApproval(Random.Range(50f, 100f));
            }
        }
        else if (npc.Approval > 0.0f && npc.Approval <= 150f)
        {
            #region Npc Standart Behavior
            if (state == Estate.Waiting) //// Waits around
            {
                waitTime -= Time.deltaTime;
                if (npc.controller.startSpeech) // if player is doing a speech
                {
                    if (distanceToPlayer < maxDistanceToPlayer) // if close enough to the player
                    {
                        ChangeState(Estate.Gathering);
                    }
                }

                if (waitTime < 0f) // switch back to wandering after waitTime has passed
                {
                    ChangeState(Estate.Wandering);
                }

            }
            else if (state == Estate.Listening) //// Listen the Speech
            {
                if (npc.controller.startSpeech) //if player is doing a speech
                {
                    if (npc.controller.activeSpeechGroup == npc.GroupName) // if player speech group match to npc group (blueSpeech = blueGroup)
                    {

                        npc.AdjustApproval(Time.deltaTime * 5f); //Gain Approval over Time

                    }
                    else // if they do no match
                    {
                        npc.AdjustApproval(-Time.deltaTime * 5f); //Lose approval over Time
                    }
                }
                else //if player is not doing a speech
                {
                    if (Random.Range(0f, 100.0f) > 20f) // If player stops the Speech go back to wandering 4/5 or waiting 1/5
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
                if (npc.controller.startSpeech) // if player is doing a speech
                {
                    if (distanceToPlayer < maxDistanceToPlayer) // if close enough to the player
                    {
                        ChangeState(Estate.Gathering);
                    }
                }

                if (HasArrived() || wanderTime < 0f) // If has arrived at told location or wandered around for too long change to waiting
                {
                    ChangeState(Estate.Waiting);
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
            #endregion
        }

        else if (npc.Approval > 150f && npc.Approval < 200f)
        {
            #region NPC Follow Mode
            if (state == Estate.Listening)
            {
                if (npc.controller.startSpeech) // if player is doing  a speech
                {
                    if (npc.controller.activeSpeechGroup == npc.GroupName) // if player speech group match to npc group
                    {

                        npc.AdjustApproval(Time.deltaTime * 5f); //Gain Approval over Time

                    }
                    else
                    {
                        npc.AdjustApproval(-Time.deltaTime * 5f); //Lose approval over Time
                    }
                }
                else // if player is not doing a speech
                {
                    ChangeState(Estate.Following);
                }
            }
            else if (state == Estate.Following)
            {
                if (npc.controller.startSpeech) // if player is doing a speech
                {
                    ChangeState(Estate.Gathering);
                }
                else // if player is not doing a speech
                {
                    ChangeState(Estate.Following);
                }

            }
            else if (state == Estate.Gathering)
            {
                if (HasArrived()) //If arrives at the player position
                {
                    ChangeState(Estate.Listening); // change to listening
                }

            }
            #endregion
        }

        else if (npc.Approval >= 200f) // NPC Attacking mode
        {
            aggroTime -= Time.deltaTime;

            ChangeState(Estate.AttackingOthers);

            if (aggroTime < 0f)
            {
                npc.AdjustApproval(-Random.Range(50f, 100f));

                ChangeState(Estate.Waiting);
            }
        }
        else
        {
            Debug.Log("Where is the approval AHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHH !!!!!!");
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

            SetSpeechDestination();
        }
        else if (state == Estate.Following)
        {
            npc.agent.isStopped = false;
            SetPlayerDestination();
        }
        else if (state == Estate.AttackingOthers)
        {
            npc.agent.isStopped = false;

            aggroTime = maxAggroTime + Random.Range(0f, maxAggroTimeRandom);

            HandleAttackOther();
        }
        else if (state == Estate.AttackingPlayer)
        {
            npc.agent.isStopped = false;

            aggroTime = maxAggroTime + Random.Range(0f, maxAggroTimeRandom);

            HandleAttackPlayer();
        }

    }


    bool HasArrived() //check if has arrived at destination point
    {
        return npc.agent.remainingDistance <= npc.agent.stoppingDistance;
    }

    void SetRandomDestination() //set random destination point inside the move area
    {
        npc.agent.SetDestination(Area.GetRandomPoint());
    }

    void SetSpeechDestination() //set random destination position inside the speech area
    {
        npc.agent.SetDestination(PlayerArea.GetRandomPoint());
    }

    void SetPlayerDestination() //set destination towards the player
    {
        npc.agent.SetDestination(npc.player.transform.position);
    }
    private NpcController FindClosestNpc()
    {
        NpcController[] allNpcs = FindObjectsOfType<NpcController>();
        NpcController closestNpc = null;
        float shortesDistance = Mathf.Infinity;

        foreach (NpcController npcController in allNpcs)
        {
            if (npcController == npc || npcController.GroupName == npc.GroupName) continue;

            float distance = Vector3.Distance(transform.position, npcController.transform.position);

            if (distance < shortesDistance)
            {
                shortesDistance = distance;
                closestNpc = npcController;
            }
        }
        if (closestNpc != null)
        {
            Debug.DrawLine(transform.position, closestNpc.transform.position);
           
        }
        return closestNpc;
    }

    void HandleAttackOther()
    {
        NpcController targetNpc = FindClosestNpc();

        if (targetNpc != null)
        {
            npc.agent.SetDestination(targetNpc.transform.position);

            if (Vector3.Distance(npc.agent.transform.position, targetNpc.transform.position) <=2f)
            {
                targetNpc.TakenDamage(Time.deltaTime * 5);

            }           
        }
    }
    void HandleAttackPlayer()
    {

        npc.agent.SetDestination(npc.player.transform.position);

        if (Vector3.Distance(npc.agent.transform.position, npc.player.transform.position) <= 2f)
        {
            Debug.Log("Attacking Player");
        }

    }

}

//Reminder - pusher to get a path through people

//Reminder - Update NavMesh to get colliders always fresh

//TODO - Player attack and Npc Attack