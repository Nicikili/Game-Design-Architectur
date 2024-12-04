using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OverallApprovalManager : MonoBehaviour
{

    private List<NpcController> allNpcs = new List<NpcController>();
    private int overallApprovalPercentage = 0;

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

    private void Update()
    {
        //update overall approval percentage every frame
        UpdateOverallApproval();
    }

    private void OnGUI()
    {
        // Define Position and Size
        float labelWidth = 200f;
        float lableHeight = 50f;
        Rect labelRect = new Rect(
            Screen.width - labelWidth - 10,
            10,
            labelWidth,
            lableHeight 
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
    }

}
