using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NpcGenerator : MonoBehaviour
{
    [SerializeField] NpcController NPCPrefab;
    [SerializeField] Area Area;
    [SerializeField] int Count = 15;

    private void Start()
    {
        for (int i = 0; i < Count; i++)
        {
            Vector3 position = Area.GetRandomPoint();
            Quaternion rotation = Quaternion.Euler(0f, Random.Range(0f, 360f), 0f);

            NpcController npc = Instantiate(NPCPrefab, position, rotation);

            npc.GetComponent<NpcWander>().Area = Area;
        }
    }

}
