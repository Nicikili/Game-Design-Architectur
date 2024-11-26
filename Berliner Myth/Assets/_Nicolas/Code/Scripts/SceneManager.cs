using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.AI.Navigation;
using System;

public class SceneManager : MonoBehaviour
{
    [SerializeField] private NavMeshSurface navMeshSurface;



    private void Update()
    {
        navMeshSurface.UpdateNavMesh(navMeshSurface.navMeshData);

    }
}
