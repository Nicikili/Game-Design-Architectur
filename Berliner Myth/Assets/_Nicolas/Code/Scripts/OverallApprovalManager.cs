using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class OverallApprovalManager : MonoBehaviour
{

    private List<NpcController> allNpcs = new List<NpcController>();
    private Dictionary< string, int> groupCounts = new Dictionary< string, int>();
    private Dictionary<string, int> stateCounts = new Dictionary< string, int>();
    private Dictionary<NpcController, float> initialApproval = new Dictionary<NpcController, float>();
    [SerializeField] private TextMeshProUGUI overallApprovalPercentageValueF;
    [SerializeField] private TextMeshProUGUI overallApprovalPercentageValueB;


    private int overallApprovalPercentage = 0;

    private int initialNpcCount = 0;
    private int deadNpcCount = 0;

    private float imbalanceTicker = 0f;

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

        foreach (NpcController npc in allNpcs)
        {
            if (!initialApproval.ContainsKey(npc))
            {
                initialApproval[npc] = npc.Approval;
            }
        }
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

    private void ImbalanceTicker()
    {
        if (groupCounts.Count < 2) return;

        //Find the differnce between the largest and smallest groups

        int maxCount = 0;
        int minCount = int.MaxValue;

        foreach (var group in groupCounts.Values)
        {
            maxCount = Mathf.Max(maxCount, group);
            minCount = Mathf.Min(minCount, group);
        }

        int imbalance = maxCount - minCount;

        //If imbalance exceeds a threshold, start adjusting approval

        if (imbalance > 15) //threshold for imbalance
        {
            float adjustmentRate = imbalance * 0.01f; //scale adjustment for imbalance ticker
            imbalanceTicker += adjustmentRate * Time.deltaTime;

            overallApprovalPercentage = Mathf.Clamp(overallApprovalPercentage + Mathf.RoundToInt(imbalanceTicker), -100, 100);
        }
        else
        {
            imbalanceTicker = 0f;
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

    private float CalculateAverageDeviation()
    {
        if (allNpcs.Count == 0) return 0;

        float totalDeviation = 0f;

        foreach (NpcController npc in allNpcs) 
        {
            if (initialApproval.TryGetValue(npc, out float initial))
            {
                totalDeviation += Mathf.Abs(npc.Approval - initial);
            }
        }

        return totalDeviation / allNpcs.Count;

    }

    private string DetermineEscalationLevel(float averageDeviation)
    {
        if (averageDeviation < 10) return "calm"; // minimal deviation

        if (averageDeviation < 30) return "soft"; // moderate deviation

        if (averageDeviation < 60) return "middle"; // significant deviation

        return "strong";                            // high deviation
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
                SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 2);
            }
            else
            {
                //bad ending
                Debug.Log("GAME OVER BAD");
                SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 3);
            }
            
        }
    }

    private void Update()
    {
        //update overall approval percentage every frame
        UpdateOverallApproval();
        CountNpcsByGroup();
        CountNpcsByState();

        //handle imbalance and win condition
        ImbalanceTicker();
        WinConditionMet();

        // update ui
        overallApprovalPercentageValueF.text = overallApprovalPercentage.ToString("F0");
        overallApprovalPercentageValueB.text = overallApprovalPercentage.ToString("F0");

    }

    private void OnGUI()
    {
        // Define Position and Size
        float labelWidth = 200f;
        float labelHeight = 50f;
        Rect labelRect = new Rect(
            Screen.width - labelWidth * 2 - 10,
            10,
            labelWidth * 2,
            labelHeight * 2 
            );

        // Define Style
        GUIStyle style = new GUIStyle(GUI.skin.label)
        {
            fontSize = 18,
            fontStyle = FontStyle.Bold,
            alignment = TextAnchor.MiddleRight,
            normal = { textColor = Color.yellow }
        };

        GUIStyle BigStyle = new GUIStyle(GUI.skin.label)
        {
            fontSize = 48,
            fontStyle = FontStyle.Bold,
            alignment = TextAnchor.MiddleRight,
            normal = { textColor = Color.yellow }
        };

        // Display the overall percentage
        //GUI.Label(labelRect, $"Approval: {overallApprovalPercentage}%", BigStyle);

        float yOffset = labelHeight + 10;
        foreach (var group in groupCounts)
        {
            Rect groupLabelRect = new Rect(
                Screen.width - labelWidth - 10,
                100 + yOffset,
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
                100 + yOffset,
                labelWidth,
                labelHeight
                );

            GUI.Label(stateLabelRect, $"{state.Key}: {state.Value}", style);
            yOffset += labelHeight;

        }
    }
}
