using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;


public class NpcWander : NpcComponent
{
    public Area Area;
    public SpeechArea PlayerArea;

    [SerializeField] float maxWaitTime = 3f;
    float maxWaitTimeRandom = 5f;

    [SerializeField] float maxWanderTime = 10f;
    float maxWanderTimeRandom = 10f;

    [SerializeField] float maxAggroTime = 5f;
    float maxAggroTimeRandom = 7f;

    [SerializeField] float maxStuckTime = 3f;
    float maxStuckTimeRandom = 5f;

    [SerializeField] float approvalMultiplier = 6f;


    enum Estate
    {
        Wandering,
        Waiting,
        Gathering,
        Listening,
        Following,
        AttackingOthers,
        AttackingPlayer,
        AttackingAttacking,
        DefendPlayer,
        Convert,
        Rally,
        TemporaryAttackingOthers
    }

    [SerializeField] Estate state = Estate.Wandering;

    [SerializeField] private float waitTime = 0f;
    [SerializeField] private float wanderTime = 0f;
    [SerializeField] private float aggroTime = 0f;
    [SerializeField] private float stuckTime = 0f;


    [SerializeField] private float maxDistanceToPlayer = 10f;

    private NpcController targetNpc = null;

    private bool hasArrived = false;
    public bool hasBeenAggroed = false;


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
            #region Npc NoApproval Behavior

            if (state == Estate.AttackingPlayer)
            {
                aggroTime -= Time.deltaTime;

                HandleAttackPlayer(); //attack Player

                if (aggroTime < 0f) // if aggro Time passed change to random new state
                {
                    float randomValue = Random.Range(0f, 100f);

                    if (randomValue < 33f)
                    {
                        ChangeState(Estate.Wandering);
                    }
                    else if (randomValue < 66f)
                    {
                        ChangeState(Estate.Waiting);
                    }
                    else
                    {
                        ChangeState(Estate.AttackingPlayer);
                    }

                }

                if (IsAttacked())
                {
                    ChangeState(Estate.AttackingAttacking);
                }
            }
            else if (state == Estate.Wandering)
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
                    if (Random.Range(0f, 100f) > 50f)
                    {
                        ChangeState(Estate.AttackingPlayer);
                    }
                    else
                    {
                        ChangeState(Estate.Waiting);
                    }
                }

                if (IsAttacked())
                {
                    ChangeState(Estate.AttackingAttacking);
                }

            }
            else if (state == Estate.Waiting)
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
                    if (Random.Range(0f, 100f) > 50f)
                    {
                        ChangeState(Estate.AttackingPlayer);
                    }
                    else
                    {
                        ChangeState(Estate.Wandering);
                    }
                }

                if (IsAttacked())
                {
                    ChangeState(Estate.AttackingAttacking);
                }

            }
            else if (state == Estate.Gathering)
            {
                stuckTime -= Time.deltaTime;
                if (HasArrived()) //If arrives at the player position
                {
                    ChangeState(Estate.Listening); // change to listening
                }
                else if (!HasArrived() && stuckTime < 0 || !npc.controller.startSpeech)
                {
                    if (Random.Range(0f, 100f) > 50f)
                    {
                        ChangeState(Estate.AttackingPlayer);
                    }
                    else
                    {
                        ChangeState(Estate.Wandering);
                    }
                }

                if (IsAttacked())
                {
                    ChangeState(Estate.AttackingAttacking);
                }
            }
            else if (state == Estate.Listening)
            {
                if (npc.controller.startSpeech) //if player is doing a speech
                {
                    if (npc.controller.activeSpeechGroup == npc.GroupName) // if player speech group match to npc group (blueSpeech = blueGroup)
                    {

                        npc.AdjustApproval(Time.deltaTime * approvalMultiplier); //Gain Approval over Time

                    }
                    else // if they do no match
                    {
                        npc.AdjustApproval(-Time.deltaTime * approvalMultiplier); //Lose approval over Time
                    }
                }
                else //if player is not doing a speech
                {
                    float randomValue = Random.Range(0f, 100f);
                    if (randomValue < 33f) // If player stops the Speech go back to wandering 50%
                    {
                        ChangeState(Estate.Wandering);
                    }
                    else if (randomValue < 66f)
                    {
                        ChangeState(Estate.Waiting);
                    }
                    else
                    {
                        ChangeState(Estate.AttackingPlayer);
                    }
                }

                if (IsAttacked())
                {
                    ChangeState(Estate.AttackingAttacking);
                }

            }
            else if (state == Estate.AttackingAttacking)
            {
                aggroTime -= Time.deltaTime;

                HandleAttackRetaliation();

                if (aggroTime < 0f)
                {
                    float randomValue = Random.Range(0f, 100f);

                    if (randomValue < 33f)
                    {
                        ChangeState(Estate.Wandering);
                    }
                    else if (randomValue < 66f)
                    {
                        ChangeState(Estate.Waiting);
                    }
                    else
                    {
                        ChangeState(Estate.AttackingPlayer);
                    }
                }
            }
            else if (state == Estate.TemporaryAttackingOthers)
            {
                float randomValue = Random.Range(0f, 100f);

                hasBeenAggroed = false;

                if (randomValue < 33f) // If player stops the Speech go back to wandering 50%
                {
                    ChangeState(Estate.Wandering);
                }
                else if (randomValue < 66f)
                {
                    ChangeState(Estate.Waiting);
                }
                else
                {
                    ChangeState(Estate.AttackingPlayer);
                }
            }
            #endregion
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

                if (IsAttacked())
                {
                    ChangeState(Estate.AttackingAttacking);
                }

                if (hasBeenAggroed)
                {
                    ChangeState(Estate.TemporaryAttackingOthers);
                }

            }
            else if (state == Estate.Listening) //// Listen the Speech
            {
                if (npc.controller.startSpeech) //if player is doing a speech
                {
                    if (npc.controller.activeSpeechGroup == npc.GroupName) // if player speech group match to npc group (blueSpeech = blueGroup)
                    {

                        npc.AdjustApproval(Time.deltaTime * approvalMultiplier); //Gain Approval over Time

                    }
                    else // if they do no match
                    {
                        npc.AdjustApproval(-Time.deltaTime * approvalMultiplier); //Lose approval over Time
                    }
                }
                else //if player is not doing a speech
                {
                    float randomValue = Random.Range(0f, 100f);
                    if (randomValue < 50f) // If player stops the Speech go back to wandering 50%
                    {
                        ChangeState(Estate.Wandering);
                    }
                    else if (randomValue < 75f)
                    {
                        ChangeState(Estate.Waiting);
                    }
                    else
                    {
                        ChangeState(Estate.Convert);
                    }
                }

                if (IsAttacked())
                {
                    ChangeState(Estate.AttackingAttacking);
                }

                if (hasBeenAggroed)
                {
                    ChangeState(Estate.TemporaryAttackingOthers);
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

                if (IsAttacked())
                {
                    ChangeState(Estate.AttackingAttacking);
                }

                if (hasBeenAggroed)
                {
                    ChangeState(Estate.TemporaryAttackingOthers);
                }

            }
            else if (state == Estate.Gathering) //// Go towards the player if near her
            {
                stuckTime -= Time.deltaTime;

                if (HasArrived()) //If arrives at the player position
                {
                    ChangeState(Estate.Listening); // change to listening
                }
                else if (!HasArrived() && stuckTime < 0 || !npc.controller.startSpeech)
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

                if (IsAttacked())
                {
                    ChangeState(Estate.AttackingAttacking);
                }

                if (hasBeenAggroed)
                {
                    ChangeState(Estate.TemporaryAttackingOthers);
                }
            }
            else if (state == Estate.AttackingOthers || state == Estate.AttackingPlayer)
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
            else if (state == Estate.Convert)
            {
                GoConvert();

                if (hasArrived)
                {
                    ChangeState(Estate.Wandering);
                    hasArrived = false;
                }

                if (IsAttacked())
                {
                    ChangeState(Estate.AttackingAttacking);
                }

                if (hasBeenAggroed)
                {
                    ChangeState(Estate.TemporaryAttackingOthers);
                }

            }
            else if (state == Estate.AttackingAttacking)
            {
                aggroTime -= Time.deltaTime;

                HandleAttackRetaliation();

                if (aggroTime < 0f)
                {
                    float randomValue = Random.Range(0f, 100f);

                    if (randomValue < 50f)
                    {
                        ChangeState(Estate.Wandering);
                    }
                    else
                    {
                        ChangeState(Estate.Waiting);
                    }
                }
            }
            else if (state == Estate.TemporaryAttackingOthers)
            {
                aggroTime -= Time.deltaTime;

                HandleAttackOther();

                if (aggroTime < 0f)
                {

                    hasBeenAggroed = false;

                    float randomValue = Random.Range(0f, 100f);

                    npc.agent.speed = 3.5f;

                    if (randomValue < 50f)
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

                        npc.AdjustApproval(Time.deltaTime * approvalMultiplier); //Gain Approval over Time

                    }
                    else
                    {
                        npc.AdjustApproval(-Time.deltaTime * approvalMultiplier); //Lose approval over Time
                    }
                }
                else // if player is not doing a speech
                {
                    ChangeState(Estate.Following);
                }

                if (IsAttacked())
                {
                    ChangeState(Estate.AttackingAttacking);
                }

                if (hasBeenAggroed)
                {
                    ChangeState(Estate.TemporaryAttackingOthers);
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

                if (IsAttacked())
                {
                    ChangeState(Estate.AttackingAttacking);
                }

                if (IsPlayerAttacked())
                {
                    ChangeState(Estate.DefendPlayer);
                }

                if (hasBeenAggroed)
                {
                    ChangeState(Estate.TemporaryAttackingOthers);
                }

            }
            else if (state == Estate.Gathering)
            {
                stuckTime -= Time.deltaTime;

                if (HasArrived()) //If arrives at the player position
                {
                    ChangeState(Estate.Listening); // change to listening
                }
                else if (!HasArrived() && stuckTime < 0 || !npc.controller.startSpeech)
                {
                    ChangeState(Estate.Following);
                }

                if (IsAttacked())
                {
                    ChangeState(Estate.AttackingAttacking);
                }

                if (hasBeenAggroed)
                {
                    ChangeState(Estate.TemporaryAttackingOthers);
                }

            }
            else if (state == Estate.AttackingOthers || state == Estate.AttackingPlayer)
            {
                ChangeState(Estate.Following);
            }
            else if (state == Estate.TemporaryAttackingOthers)
            {
                aggroTime -= Time.deltaTime;

                HandleAttackOther();

                if (aggroTime < 0f)
                {

                    hasBeenAggroed = false;

                    npc.agent.speed = 3.5f;

                    ChangeState(Estate.Following);

                }
            }
            else if (state == Estate.AttackingAttacking)
            {
                aggroTime -= Time.deltaTime;

                HandleAttackRetaliation();

                if (aggroTime < 0f)
                {
                    ChangeState(Estate.Following);
                }
            }
            else if (state == Estate.DefendPlayer)
            {
                HandleAttackPlayerRetaliation();

                if (!IsPlayerAttacked())
                {
                    ChangeState(Estate.Following);
                }
            }
            #endregion
        }

        else if (npc.Approval >= 200f)
        {
            #region NPC Aggro Mode
            if (state == Estate.AttackingOthers)
            {
                aggroTime -= Time.deltaTime;

                HandleAttackOther();

                if (aggroTime < 0f)
                {
                    float randomValue = Random.Range(0f, 100f);

                    npc.agent.speed = 3.5f;

                    if (randomValue < 22)
                    {
                        ChangeState(Estate.AttackingOthers);
                    }
                    else if (randomValue < 44)
                    {
                        ChangeState(Estate.Waiting);
                    }
                    else if (randomValue < 66)
                    {
                        ChangeState(Estate.Wandering);
                    }
                    else if (randomValue < 88)
                    {
                        ChangeState(Estate.Rally);
                    }
                    else
                    {
                        ChangeState(Estate.Following);
                    }

                }
            }
            else if (state == Estate.TemporaryAttackingOthers)
            {
                aggroTime -= Time.deltaTime;

                HandleAttackOther();

                if (aggroTime < 0f)
                {
                    float randomValue = Random.Range(0f, 100f);

                    hasBeenAggroed = false;

                    npc.agent.speed = 3.5f;

                    if (randomValue < 22)
                    {
                        ChangeState(Estate.AttackingOthers);
                    }
                    else if (randomValue < 44)
                    {
                        ChangeState(Estate.Waiting);
                    }
                    else if (randomValue < 66)
                    {
                        ChangeState(Estate.Wandering);
                    }
                    else if (randomValue < 88)
                    {
                        ChangeState(Estate.Rally);
                    }
                    else
                    {
                        ChangeState(Estate.Following);
                    }

                }
            }
            else if (state == Estate.Rally)
            {
                aggroTime -= Time.deltaTime;

                HandleRallyOther();

                if (aggroTime < 0f)
                {
                    float randomValue = Random.Range(0f, 100f);

                    npc.agent.speed = 3.5f;

                    if (randomValue < 22)
                    {
                        ChangeState(Estate.AttackingOthers);
                    }
                    else if (randomValue < 44)
                    {
                        ChangeState(Estate.Waiting);
                    }
                    else if (randomValue < 66)
                    {
                        ChangeState(Estate.Wandering);
                    }
                    else if (randomValue < 88)
                    {
                        ChangeState(Estate.Rally);
                    }
                    else
                    {
                        ChangeState(Estate.Following);
                    }

                }
            }
            else if (state == Estate.Wandering)
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
                    float randomValue = Random.Range(0f, 100f);

                    if (randomValue < 50)
                    {
                        ChangeState(Estate.AttackingOthers);
                    }
                    else if (randomValue < 25)
                    {
                        ChangeState(Estate.Waiting);
                    }
                    else
                    {
                        ChangeState(Estate.Rally);
                    }
                }

                if (IsAttacked())
                {
                    ChangeState(Estate.AttackingAttacking);
                }

                if (hasBeenAggroed)
                {
                    ChangeState(Estate.TemporaryAttackingOthers);
                }

            }
            else if (state == Estate.Waiting)
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
                    float randomValue = Random.Range(0f, 100f);

                    if (randomValue < 50)
                    {
                        ChangeState(Estate.AttackingOthers);
                    }
                    else if (randomValue < 25)
                    {
                        ChangeState(Estate.Wandering);
                    }
                    else
                    {
                        ChangeState(Estate.Rally);
                    }
                }

                if (IsAttacked())
                {
                    ChangeState(Estate.AttackingAttacking);
                }

                if (hasBeenAggroed)
                {
                    ChangeState(Estate.TemporaryAttackingOthers);
                }

            }
            else if (state == Estate.Following)
            {
                if (npc.controller.startSpeech)
                {
                    ChangeState(Estate.Gathering);
                }
                else
                {
                    ChangeState(Estate.Following);
                }

                if (IsAttacked())
                {
                    ChangeState(Estate.AttackingAttacking);
                }

                if (IsPlayerAttacked())
                {
                    ChangeState(Estate.DefendPlayer);
                }

                if (hasBeenAggroed)
                {
                    ChangeState(Estate.TemporaryAttackingOthers);
                }
            }
            else if (state == Estate.Gathering)
            {
                stuckTime -= Time.deltaTime;
                if (HasArrived()) //If arrives at the player position
                {
                    ChangeState(Estate.Listening); // change to listening
                }
                else if (!HasArrived() && stuckTime < 0 || !npc.controller.startSpeech)
                {
                    if (Random.Range(0f, 100f) > 45f)
                    {
                        ChangeState(Estate.AttackingOthers);
                    }
                    else
                    {
                        ChangeState(Estate.Wandering);
                    }
                }

                if (IsAttacked())
                {
                    ChangeState(Estate.AttackingAttacking);
                }

                if (hasBeenAggroed)
                {
                    ChangeState(Estate.TemporaryAttackingOthers);
                }
            }
            else if (state == Estate.Listening)
            {
                if (npc.controller.startSpeech) //if player is doing a speech
                {
                    if (npc.controller.activeSpeechGroup == npc.GroupName) // if player speech group match to npc group (blueSpeech = blueGroup)
                    {

                        npc.AdjustApproval(Time.deltaTime * 7f); //Gain Approval over Time

                    }
                    else // if they do no match
                    {
                        npc.AdjustApproval(-Time.deltaTime * 7f); //Lose approval over Time
                    }
                }
                else //if player is not doing a speech
                {
                    float randomValue = Random.Range(0f, 100f);
                    if (randomValue < 45f) // If player stops the Speech go back to wandering 50%
                    {
                        ChangeState(Estate.AttackingOthers);
                    }
                    else if (randomValue < 65f)
                    {
                        ChangeState(Estate.Waiting);
                    }
                    else if (randomValue < 85f)
                    {
                        ChangeState(Estate.Wandering);
                    }
                    else
                    {
                        ChangeState(Estate.Rally);
                    }
                }

                if (IsAttacked())
                {
                    ChangeState(Estate.AttackingAttacking);
                }

                if (hasBeenAggroed)
                {
                    ChangeState(Estate.TemporaryAttackingOthers);
                }

            }
            else if (state == Estate.AttackingAttacking)
            {
                aggroTime -= Time.deltaTime;

                HandleAttackRetaliation();

                if (aggroTime < 0f)
                {
                    float randomValue = Random.Range(0f, 100f);

                    if (randomValue < 22)
                    {
                        ChangeState(Estate.AttackingOthers);
                    }
                    else if (randomValue < 44)
                    {
                        ChangeState(Estate.Waiting);
                    }
                    else if (randomValue < 66)
                    {
                        ChangeState(Estate.Wandering);
                    }
                    else if (randomValue < 88)
                    {
                        ChangeState(Estate.Rally);
                    }
                    else
                    {
                        ChangeState(Estate.Following);
                    }
                }
            }
            else if (state == Estate.DefendPlayer)
            {
                HandleAttackPlayerRetaliation();

                if (!IsPlayerAttacked())
                {
                    ChangeState(Estate.Following);
                }
            }
            #endregion
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

            npc.agent.speed = 3.5f;

            SetRandomDestination();

            wanderTime = maxWanderTime + Random.Range(0f, maxWanderTimeRandom);
        }
        else if (state == Estate.Listening)
        {
            npc.agent.isStopped = true;

            npc.agent.speed = 3.5f;

            FacePlayer();

            if (npc.controller.activeSpeechGroup == npc.GroupName)
            {
                npc.ShowPositivReaction();
            }
            else
            {
                npc.ShowNegativReaction();
            }

        }
        else if (state == Estate.Waiting)
        {
            waitTime = maxWaitTime + Random.Range(0f, maxWaitTimeRandom);

            npc.agent.isStopped = true;

            npc.agent.speed = 3.5f;

        }
        else if (state == Estate.Gathering)
        {
            npc.agent.isStopped = false;

            npc.agent.speed = 3.5f;

            SetSpeechDestination();

            stuckTime = maxStuckTime + Random.Range(0f, maxStuckTimeRandom);
        }
        else if (state == Estate.Following)
        {
            npc.agent.isStopped = false;

            npc.agent.speed = 3.5f;

            SetPlayerDestination();
        }
        else if (state == Estate.AttackingOthers)
        {
            npc.agent.isStopped = false;

            npc.agent.speed = 5f;

            aggroTime = maxAggroTime + Random.Range(0f, maxAggroTimeRandom);

            HandleAttackOther();
        }
        else if (state == Estate.AttackingPlayer)
        {
            npc.agent.isStopped = false;

            npc.agent.speed = 3.5f;

            aggroTime = maxAggroTime + Random.Range(0f, maxAggroTimeRandom);

            HandleAttackPlayer();
        }
        else if (state == Estate.AttackingAttacking)
        {
            npc.agent.isStopped = false;

            aggroTime = maxAggroTime + Random.Range(0f, maxAggroTimeRandom);

            npc.agent.speed = 3.5f;

            HandleAttackRetaliation();
        }
        else if (state == Estate.DefendPlayer)
        {
            npc.agent.isStopped = false;

            npc.agent.speed = 3.5f;

            HandleAttackPlayerRetaliation();

        }
        else if (state == Estate.Convert)
        {
            AudioManager.instance.PlayOneShot(FMODEvents.instance.VL_NPC_GoesToOtherNPC, this.transform.position);
            npc.agent.isStopped = false;

            npc.agent.speed = 3.5f;

        }
        else if (state == Estate.Rally)
        {
            npc.agent.isStopped = false;

            HandleRallyOther();

            aggroTime = (maxAggroTime * 2) + Random.Range(0f, maxAggroTimeRandom);

            npc.agent.speed = 5f;
        }
        else if (state == Estate.TemporaryAttackingOthers)
        {
            npc.agent.isStopped = false;

            aggroTime = maxAggroTime + Random.Range(0f, maxAggroTimeRandom);

            HandleAttackOther();

            npc.agent.speed = 5f;

        }
    }


    bool HasArrived() //check if has arrived at destination point
    {
        return npc.agent.remainingDistance <= npc.agent.stoppingDistance;
    }

    bool IsAttacked()
    {
        if (npc.LastAttacker != null && npc.LastAttacker.GroupName != npc.GroupName) //ensure attacker is from the other group
        {
            //AudioManager.instance.PlayOneShot(FMODEvents.instance.VL_NPC_Disapproves, this.transform.position);
            NpcWander attackerWander = npc.LastAttacker.GetComponent<NpcWander>();

            if (attackerWander != null && attackerWander.state == Estate.AttackingOthers)
            {
                return true;
            }
        }
        return false;
    }
    bool IsPlayerAttacked()
    {
        NpcController[] allNpcs = FindObjectsOfType<NpcController>();

        foreach (NpcController npc in allNpcs)
        {
            NpcWander wanderScript = npc.GetComponent<NpcWander>();
            if (wanderScript != null && wanderScript.state == Estate.AttackingPlayer)
            {
                return true;
            }
        }
        return false;
    }

    void SetRandomDestination() //set random destination point inside the move area
    {
        npc.agent.SetDestination(Area.GetRandomPoint());
    }

    void SetSpeechDestination() //set random destination position inside the speech area
    {
        npc.agent.SetDestination(PlayerArea.GetRandomPoint());
    }

    void FacePlayer()
    {
        if (npc.player != null)
        {
            // Calculate the direction to the player
            Vector3 directionToPlayer = npc.player.transform.position - npc.transform.position;
            directionToPlayer.y = 0; // Lock rotation to the Y-axis

            // Determine the target rotation
            Quaternion targetRotation = Quaternion.LookRotation(directionToPlayer);

            // Use DoTween for smooth rotation
            npc.transform.DORotateQuaternion(targetRotation, 0.5f) // Adjust duration (0.5f) as needed
                .SetEase(Ease.OutCubic); // Add easing for a smoother effect
        }
    }
    void SetPlayerDestination() //set destination towards near the player
    {
        Vector3 directionToPlayer = npc.player.transform.position - transform.position;
        directionToPlayer.y = 0;

        float followDistance = 2.0f;
        Vector3 targetPosition = npc.player.transform.position - directionToPlayer.normalized * followDistance;

        NavMeshHit hit;

        if (NavMesh.SamplePosition(targetPosition, out hit, 3.0f, NavMesh.AllAreas))
        {
            npc.agent.SetDestination(hit.position);
        }
        else
        {
            Debug.Log("Cannot follow Player");
        }
    }
    private NpcController FindClosestNpc()  //search for Npc that's from another group an closest to it
    {
        NpcController[] allNpcs = FindObjectsOfType<NpcController>();
        NpcController closestNpc = null;
        float shortesDistance = Mathf.Infinity; //start from infinit and go down to 0 to find which is the closest

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

    private NpcController FindFurthestNpc() //search for Npc that's from another group and furthest to it
    {
        NpcController[] allNpcs = FindObjectsOfType<NpcController>();
        NpcController furthestNpc = null;
        float furthestDistance = 0f; //start from 0 and go up to infinite to find which is the furthest

        foreach (NpcController npcController in allNpcs)
        {
            if (npcController == npc || npcController.GroupName == npc.GroupName) continue;

            float distance = Vector3.Distance(transform.position, npcController.transform.position);

            if (distance > furthestDistance)
            {
                furthestDistance = distance;
                furthestNpc = npcController;
            }
        }
        if (furthestNpc != null)
        {
            Debug.DrawLine(transform.position, furthestNpc.transform.position);
        }
        return furthestNpc;
    }

    private List<NpcController> FindAttackerNpcs() //search for all Npcs that are from another group and are attacking
    {
        List<NpcController> targets = new List<NpcController>();
        NpcController[] allNpcs = FindObjectsOfType<NpcController>(); //get all npcs in the scene

        foreach (NpcController npcController in allNpcs)
        {
            if (npcController.GroupName != npc.GroupName)
            {
                NpcWander wanderComponent = npcController.GetComponent<NpcWander>();

                if (wanderComponent != null && wanderComponent.state == Estate.AttackingOthers)
                {
                    targets.Add(npcController);
                }
            }
        }

        return targets;
    }

    private List<NpcController> FindAttackingPlayerNpcs()
    {
        List<NpcController> targets = new List<NpcController>();
        NpcController[] allNpcs = FindObjectsOfType<NpcController>(); //get all npcs in the scene

        foreach (NpcController npcController in allNpcs)
        {
            if (npcController.GroupName != npc.GroupName)
            {
                NpcWander wanderComponent = npcController.GetComponent<NpcWander>();

                if (wanderComponent != null && wanderComponent.state == Estate.AttackingPlayer)
                {
                    targets.Add(npcController);
                }
            }
        }

        return targets;
    }

    void HandleAttackOther()
    {
        if (targetNpc == null)
        {
            targetNpc = FindClosestNpc();
        }


        if (targetNpc != null)
        {
            npc.agent.SetDestination(targetNpc.transform.position);

            if (Vector3.Distance(npc.agent.transform.position, targetNpc.transform.position) <= 2f)
            {
                targetNpc.TakenDamage(Time.deltaTime * 5, npc);

            }
        }
    }

    void HandleRallyOther()
    {
        if (targetNpc == null)
        {
            targetNpc = FindFurthestNpc();
        }

        if (targetNpc != null)
        {
            npc.agent.SetDestination(targetNpc.transform.position);

            if (Vector3.Distance(npc.agent.transform.position, targetNpc.transform.position) <= 2f)
            {
                targetNpc.TakenDamage(Time.deltaTime * 5, npc);
            }
        }

        NpcController[] allNpcs = FindObjectsOfType<NpcController>();
        foreach (NpcController nearbyNpc in allNpcs)
        {
            if (nearbyNpc == npc || nearbyNpc.GroupName != npc.GroupName) continue;
            {
                float distance = Vector3.Distance(npc.agent.transform.position, nearbyNpc.transform.position);
                if (distance < 5f)
                {
                    NpcWander wanderComponent = nearbyNpc.GetComponent<NpcWander>();
                    if (wanderComponent != null)
                    {
                        wanderComponent.hasBeenAggroed = true;
                    }
                }
            }
        }
    }
    void HandleAttackPlayer()
    {

        npc.agent.SetDestination(npc.player.transform.position);

        if (Vector3.Distance(npc.agent.transform.position, npc.player.transform.position) <= 2f)
        {
            npc.controller.PlayerTakenDamage(Time.deltaTime * 5f);
        }

    }

    void HandleAttackRetaliation()
    {
        List<NpcController> targets = FindAttackerNpcs();

        if (targets.Count > 0)
        {
            targets.Sort((a, b) =>
            {
                float distanceA = Vector3.Distance(npc.transform.position, a.transform.position);
                float distanceB = Vector3.Distance(npc.transform.position, b.transform.position);
                return distanceA.CompareTo(distanceB);
            });

            npc.agent.SetDestination(targets[0].transform.position);

            StartCoroutine(ApplyDamageAfterDelay(targets[0]));
        }
    }

    void HandleAttackPlayerRetaliation()
    {
        List<NpcController> targets = FindAttackingPlayerNpcs();

        if (targets.Count > 0)
        {
            int randomIndex = Random.Range(0, targets.Count);
            npc.agent.SetDestination(targets[randomIndex].transform.position);

            StartCoroutine(ApplyDamageAfterDelay(targets[randomIndex]));
        }

    }

    private IEnumerator ApplyDamageAfterDelay(NpcController target)
    {
        yield return new WaitForSeconds(Random.Range(2f, 7f));

        if (target != null && Vector3.Distance(npc.agent.transform.position, target.transform.position) <= 2f)
        {
            target.TakenDamage(Time.deltaTime * 4.5f, npc);
        }
    }

    void GoConvert()
    {

        if (targetNpc == null)
        {
            targetNpc = FindFurthestNpc();
        }

        if (targetNpc != null)
        {
            npc.agent.SetDestination(targetNpc.transform.position);

            if (Vector3.Distance(npc.agent.transform.position, targetNpc.transform.position) <= 2f)
            {

                if (npc.GroupName == ("Blue"))
                {
                    targetNpc.SwitchGroup("Blue");
                    hasArrived = true;
                }
                else if (npc.GroupName == ("Red"))
                {
                    targetNpc.SwitchGroup("Red");
                    hasArrived = true;
                }

            }
        }

    }
}

//Reminder - pusher to get a path through people
