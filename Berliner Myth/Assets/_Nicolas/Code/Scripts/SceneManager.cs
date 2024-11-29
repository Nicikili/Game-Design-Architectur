using Unity.AI.Navigation;
using UnityEngine;

public class SceneManager : MonoBehaviour
{
    [SerializeField] private NavMeshSurface navMeshSurface;
    [SerializeField] private float updateInterval = 2f;

    private float timer;

    public void Start()
    {
        if (navMeshSurface == null)
        {
            navMeshSurface = GetComponent<NavMeshSurface>();
        }
    }

    public void Update()
    {
        timer += Time.deltaTime;

        if (timer >= updateInterval)
        {
            UpdateNavMesh();
            timer = 0f;
        }
    }

    public void UpdateNavMesh()
    {
        if (navMeshSurface != null)
        {
            navMeshSurface.UpdateNavMesh(navMeshSurface.navMeshData);
        }
    }
}