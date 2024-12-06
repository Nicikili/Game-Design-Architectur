using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OverallApprovalManager : MonoBehaviour
{

    private List<NpcController> allNpcs = new List<NpcController>();
    private Dictionary< string, int> groupCounts = new Dictionary< string, int>();
    private Dictionary<string, int> stateCounts = new Dictionary< string, int>();
    private int overallApprovalPercentage = 0;

    private int initialNpcCount = 0;
    private int deadNpcCount = 0;

    private void Start()
    {
        RefreshNpcList();
        initialNpcCount = allNpcs.Count;
    }

    private void RefreshNpcList()
    {
        // always get the lates npc count in the scene
        allNpcs.Clear();
        allNpcs.AddRange(FindObjectsOfType<NpcController>());
    }

    private void UpdateOverallApproval()
    {
        RefreshNpcList();   //refresh the list of npcs

        if (allNpcs.Count == 0)
        {
            overallApprovalPercentage = 0;
            return;
        }

        float totalApproval = 0f;

        //sum up the approval of all npcs
        foreach (NpcController npc in allNpcs)
        {
            totalApproval += npc.Approval;
        }

        //calculate the overall approval percentage
        float neutralApproval = allNpcs.Count * 100f; // Neutral approval is 100 for the npcs
        float maxDeviation = allNpcs.Count * 50f; // Maximum deviation from neutral (from 100 to 50 or 100 to 150) all above goes into excess ?

        overallApprovalPercentage = Mathf.RoundToInt(((totalApproval - neutralApproval) / (maxDeviation)) * 100f);
        overallApprovalPercentage = Mathf.Clamp(overallApprovalPercentage, -100, 100);
    }

    private void CountNpcsByGroup()
    {
        groupCounts.Clear();

        foreach (NpcController npc in allNpcs)
        {
            if(!string.IsNullOrEmpty(npc.GroupName))
            {
                if (!groupCounts.ContainsKey( npc.GroupName ))
                {
                    groupCounts[npc.GroupName] = 0;
                }
                groupCounts[npc.GroupName]++;
            }
        }

        deadNpcCount = initialNpcCount - allNpcs.Count;
    }

    private void CountNpcsByState()
    {
        stateCounts.Clear();

        foreach (NpcController npc in allNpcs)
        {
            NpcWander wanderComponent = npc.GetComponent<NpcWander>();
            if (wanderComponent != null)
            {
                string stateName = wanderComponent.GetCurrentState();
                if (!stateCounts.ContainsKey(stateName))
                {
                    stateCounts[stateName] = 0;
                }
                stateCounts[stateName]++;

            }
        }
    }

    private bool AreGroupsBalanced()
    {
        int totalNpcs = allNpcs.Count;

        if (totalNpcs == 0) return false;

        foreach (var group in groupCounts.Values)
        {
            if (group >= totalNpcs * 0.4f)
                {
                return true;
            }
        }
        return false;   
    }

    private void WinConditionMet()
    {

        bool groupsAreBalanced = AreGroupsBalanced();
        float deadPercentage = (float)deadNpcCount / initialNpcCount * 100f;

        if (overallApprovalPercentage >= 100) //if 100% approval achieved -> get to a win state
        {

            Debug.Log("GameOver Incoming");

            if (groupsAreBalanced && deadPercentage < 25f)
            {
                //neutral ending
                Debug.Log("GAME OVER NEUTRAL");
            }
            else
            {
                //bad ending
                Debug.Log("GAME OVER BAD");
            }
            
        }
    }

    private void Update()
    {
        //update overall approval percentage every frame
        UpdateOverallApproval();
        CountNpcsByGroup();
        CountNpcsByState();

        WinConditionMet();
    }

    private void OnGUI()
    {
        // Define Position and Size
        float labelWidth = 200f;
        float labelHeight = 50f;
        Rect labelRect = new Rect(
            Screen.width - labelWidth - 10,
            10,
            labelWidth,
            labelHeight 
            );

        // Define Style
        GUIStyle style = new GUIStyle(GUI.skin.label)
        {
            fontSize = 18,
            fontStyle = FontStyle.Bold,
            alignment = TextAnchor.MiddleRight,
            normal = { textColor = Color.white }
        };

        // Display the overall percentage
        GUI.Label(labelRect, $"Approval: {overallApprovalPercentage}%", style);

        float yOffset = labelHeight + 10;
        foreach (var group in groupCounts)
        {
            Rect groupLabelRect = new Rect(
                Screen.width - labelWidth - 10,
                10 + yOffset,
                labelWidth,
                labelHeight
                );

            GUI.Label(groupLabelRect, $"{group.Key}: {group.Value}", style);
            yOffset += labelHeight;
        }

        foreach (var state in stateCounts)
        {
            Rect stateLabelRect = new Rect(
                Screen.width - labelWidth - 10,
                10 + yOffset,
                labelWidth,
                labelHeight
                );

            GUI.Label(stateLabelRect, $"{state.Key}: {state.Value}", style);
            yOffset += labelHeight;

        }
    }
}
