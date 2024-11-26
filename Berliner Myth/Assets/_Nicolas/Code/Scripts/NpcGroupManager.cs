using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NpcGroupManager : MonoBehaviour
{
    [System.Serializable]
    public class Group
    {
        public string groupName;
        public Color groupColor;
        public List <NpcController> members = new List <NpcController> ();
    }

    [SerializeField] private List <Group> groups = new List <Group> ();

    public void AddNpcToGroup(NpcController npc)
    {
        Group targetGroup = null;
        int minCount = int.MaxValue;

        foreach (Group group in groups)
        {
            if (group.members.Count < minCount)
            {
                minCount = group.members.Count;
                targetGroup = group;
            }
        }

        if (targetGroup != null)
        {
            {
                targetGroup.members.Add(npc);
                npc.SetGroup(targetGroup.groupName, targetGroup.groupColor);

            }
        }

    }
}
