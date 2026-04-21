using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
public class WireMeshPulseController : MonoBehaviour
{
    private Mesh mesh;

    private Vector3[] vertices;
    private int[] triangles;

    private Dictionary<int, HashSet<int>> adjacency;

    private float[] distances;
    private Vector2[] uv2;

    [Header("Pulse Settings")]
    public int sourceNode = 0;
    public float recomputeInterval = 0.2f;

    private float timer;

    void Start()
    {
        var mf = GetComponent<MeshFilter>();

        mesh = Instantiate(mf.sharedMesh);
        mf.mesh = mesh;

        Debug.Log("RUNTIME MESH INSTANCE ID: " + mesh.GetInstanceID());
        Debug.Log("MESH FILTER MESH ID: " + mf.mesh.GetInstanceID());
    }

    void Update()
    {
        // Press SPACE to trigger a new pulse
        // if (Input.GetKeyDown(KeyCode.Space))
        // {
        //     sourceNode = Random.Range(0, vertices.Length);
        //     ComputeDistances(sourceNode);
        //     ApplyToMesh();
        // }

        // Optional: animate pulse origin over time
        timer += Time.deltaTime;
        if (timer > recomputeInterval)
        {
            timer = 0f;

            // Uncomment for continuous motion:
            // sourceNode = (sourceNode + 1) % vertices.Length;
            // ComputeDistances(sourceNode);
            // ApplyToMesh();
        }
    }

    void BuildAdjacency()
    {
        adjacency = new Dictionary<int, HashSet<int>>();

        void AddEdge(int a, int b)
        {
            if (!adjacency.ContainsKey(a))
                adjacency[a] = new HashSet<int>();

            adjacency[a].Add(b);
        }

        for (int i = 0; i < triangles.Length; i += 3)
        {
            int a = triangles[i];
            int b = triangles[i + 1];
            int c = triangles[i + 2];

            AddEdge(a, b);
            AddEdge(b, a);

            AddEdge(b, c);
            AddEdge(c, b);

            AddEdge(c, a);
            AddEdge(a, c);
        }
    }

    void ComputeDistances(int source)
    {
        for (int i = 0; i < distances.Length; i++)
            distances[i] = float.PositiveInfinity;

        Queue<int> q = new Queue<int>();

        distances[source] = 0f;
        q.Enqueue(source);

        while (q.Count > 0)
        {
            int v = q.Dequeue();

            if (!adjacency.ContainsKey(v))
                continue;

            foreach (int n in adjacency[v])
            {
                float d = distances[v] +
                          Vector3.Distance(vertices[v], vertices[n]);

                if (d < distances[n])
                {
                    distances[n] = d;
                    q.Enqueue(n);
                }
            }
        }

        // normalize for shader stability
        float max = 0f;
        for (int i = 0; i < distances.Length; i++)
            if (distances[i] > max && distances[i] < float.PositiveInfinity)
                max = distances[i];

        if (max <= 0f) max = 1f;

        for (int i = 0; i < distances.Length; i++)
            distances[i] /= max;
    }

    void ApplyToMesh()
    {
        Debug.Log("UV2 sample: " + uv2[0]);
        for (int i = 0; i < uv2.Length; i++)
        {
            uv2[i] = new Vector2(distances[i], 0);
        }

        mesh.uv2 = uv2;
    }
}