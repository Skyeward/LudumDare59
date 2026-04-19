using System.Collections.Generic;
using UnityEngine;

public class SpherePulseOptimized : MonoBehaviour
{
    [Header("Setup")]
    public Mesh nodeMesh;
    public Material nodeMaterial;

    [Header("Pulse")]
    public float spreadChance = 0.15f;
    public int maxLife = 24;
    public int maxConcurrentSeeds = 50;

    [Header("Timing")]
    public float spawnMin = 0.05f;
    public float spawnMax = 0.5f;
    public float pulseInterval = 0.25f;

    [Header("Visual")]
    public float nodeScale = 0.02f;

    public Color fullColor = new(0f, 0.6f, 1f, 1f);
    public Color midColor = Color.green;
    public Color lowColor = Color.magenta;

    struct Edge
    {
        public int a;
        public int b;
    }

    struct Pulse
    {
        public int current;
        public int previous;
        public int life;
    }

    Vector3[] vertices;
    List<int>[] neighbors;
    List<(int to, int edgeIndex)>[] neighborEdges;

    List<Edge> edges = new();

    float[] intensity;
    float[] timer;
    float[] cooldown;
    float[] lifeTime;

    bool[] edgeActive;

    List<Pulse> current = new();
    List<Pulse> next = new();

    float spawnTimer;
    float nextSpawn;
    float pulseTimer;

    // ---------------- RENDER BUFFERS ----------------

    Matrix4x4[] matrices;
    Vector4[] colors;

    Vector3[] edgeVerts;
    int[] edgeIndices;
    Color[] edgeColors;

    Mesh edgeMesh;

    MaterialPropertyBlock mpb;

    // ---------------- INIT ----------------

    void Start()
    {
        var mesh = GetComponent<MeshFilter>().mesh;
        vertices = mesh.vertices;

        BuildGraph(mesh);

        int vCount = vertices.Length;
        int eCount = edges.Count;

        intensity = new float[vCount];
        timer = new float[vCount];
        cooldown = new float[vCount];
        lifeTime = new float[vCount];

        edgeActive = new bool[eCount];

        matrices = new Matrix4x4[vCount];
        colors = new Vector4[vCount];

        edgeVerts = new Vector3[eCount * 2];
        edgeIndices = new int[eCount * 2];
        edgeColors = new Color[eCount * 2];

        for (int i = 0; i < eCount; i++)
        {
            edgeIndices[i * 2] = i * 2;
            edgeIndices[i * 2 + 1] = i * 2 + 1;
        }

        edgeMesh = new Mesh();

        mpb = new MaterialPropertyBlock();

        nextSpawn = Random.Range(spawnMin, spawnMax);
        pulseTimer = pulseInterval;
    }

    // ---------------- GRAPH ----------------

    void BuildGraph(Mesh mesh)
    {
        var tris = mesh.triangles;
        var set = new HashSet<(int, int)>();

        void Add(int a, int b)
        {
            if (a < b) set.Add((a, b));
            else set.Add((b, a));
        }

        for (int i = 0; i < tris.Length; i += 3)
        {
            Add(tris[i], tris[i + 1]);
            Add(tris[i + 1], tris[i + 2]);
            Add(tris[i + 2], tris[i]);
        }

        neighbors = new List<int>[vertices.Length];
        neighborEdges = new List<(int, int)>[vertices.Length];

        for (int i = 0; i < vertices.Length; i++)
        {
            neighbors[i] = new List<int>();
            neighborEdges[i] = new List<(int, int)>();
        }

        int index = 0;

        foreach (var (a, b) in set)
        {
            edges.Add(new Edge { a = a, b = b });

            neighbors[a].Add(b);
            neighbors[b].Add(a);

            neighborEdges[a].Add((b, index));
            neighborEdges[b].Add((a, index));

            index++;
        }
    }

    // ---------------- UPDATE ----------------

    void Update()
    {
        HandleSpawn();

        pulseTimer -= Time.deltaTime;
        if (pulseTimer <= 0f)
        {
            ProcessPulses();
            pulseTimer = pulseInterval;
        }

        UpdateNodes();
        UpdateEdges();

        RenderNodes();
        RenderEdges();
    }

    // ---------------- PULSES ----------------

    void HandleSpawn()
    {
        spawnTimer += Time.deltaTime;

        if (spawnTimer < nextSpawn) return;

        spawnTimer = 0f;
        nextSpawn = Random.Range(spawnMin, spawnMax);

        if (current.Count < maxConcurrentSeeds)
        {
            current.Add(new Pulse
            {
                current = Random.Range(0, vertices.Length),
                previous = -1,
                life = 0
            });
        }
    }

    void ProcessPulses()
    {
        next.Clear();

        foreach (var p in current)
        {
            ActivateNode(p.current);

            foreach (var (n, edgeIndex) in neighborEdges[p.current])
            {
                if (n == p.previous) continue;

                if (Random.value < spreadChance)
                {
                    edgeActive[edgeIndex] = true;

                    next.Add(new Pulse
                    {
                        current = n,
                        previous = p.current,
                        life = p.life + 1
                    });
                }
            }

            if (p.life < maxLife)
                next.Add(p);
        }

        (current, next) = (next, current);
    }

    void ActivateNode(int i)
    {
        if (cooldown[i] > 0f || timer[i] > 0f) return;

        intensity[i] = 1f;
        timer[i] = 1f;
    }

    // ---------------- UPDATES ----------------

    void UpdateNodes()
    {
        for (int i = 0; i < intensity.Length; i++)
        {
            if (timer[i] > 0f)
            {
                timer[i] -= Time.deltaTime;
                intensity[i] = timer[i];
            }
            else
            {
                intensity[i] = 0f;
            }

            if (cooldown[i] > 0f)
                cooldown[i] -= Time.deltaTime;
        }
    }

    void UpdateEdges()
    {
        for (int i = 0; i < edges.Count; i++)
        {
            var e = edges[i];

            if (intensity[e.a] <= 0f && intensity[e.b] <= 0f)
                edgeActive[i] = false;
        }
    }

    // ---------------- RENDER ----------------

    void RenderNodes()
    {
        for (int i = 0; i < vertices.Length; i++)
        {
            matrices[i] = Matrix4x4.TRS(
                transform.TransformPoint(vertices[i]),
                Quaternion.identity,
                Vector3.one * nodeScale
            );

            colors[i] = GetColor(intensity[i]);
        }

        mpb.SetVectorArray("_Color", colors);

        Graphics.DrawMeshInstanced(
            nodeMesh,
            0,
            nodeMaterial,
            matrices,
            vertices.Length,
            mpb
        );
    }

    void RenderEdges()
    {
        int count = 0;

        for (int i = 0; i < edges.Count; i++)
        {
            if (!edgeActive[i]) continue;

            var e = edges[i];

            edgeVerts[count * 2] = vertices[e.a];
            edgeVerts[count * 2 + 1] = vertices[e.b];

            Color c = GetColor(intensity[e.b]);

            edgeColors[count * 2] = c;
            edgeColors[count * 2 + 1] = c;

            count++;
        }

        edgeMesh.Clear();
        edgeMesh.vertices = edgeVerts;
        edgeMesh.colors = edgeColors;
        edgeMesh.SetIndices(edgeIndices, MeshTopology.Lines, 0);

        Graphics.DrawMesh(edgeMesh, transform.localToWorldMatrix, nodeMaterial, 0);
    }

    Color GetColor(float t)
    {
        t = Mathf.Clamp01(t);

        if (t > 0.5f)
            return Color.Lerp(midColor, fullColor, (t - 0.5f) * 2f);

        return Color.Lerp(lowColor, midColor, t * 2f);
    }
}