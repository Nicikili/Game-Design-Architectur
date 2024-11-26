using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NpcComponent : MonoBehaviour
{
    protected NpcController npc;

    protected virtual void Awake()
    {
        npc = GetComponent<NpcController>();
    }
}
