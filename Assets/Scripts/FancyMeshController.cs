using System.Collections.Generic;
using UnityEngine;

public class SpherePulseFinal : MonoBehaviour
{
    [Header("Setup")]
    public GameObject nodePrefab;
    private float nodeScale = 0.01f;

    [Header("Pulse Control")]
    private float spawnIntervalMin = 0.05f;
    private float spawnIntervalMax = 0.5f;
    private float activeTime = 2f;
    private float spreadChance = 0.15f;
    private int maxConcurrentSeeds = 50;

    private int maxLife = 24;

    [Header("Visual")]
    private float emissionStrength = 10f;
    private float nodeAlphaScale = 0.35f;
    private float lineAlphaScale = 1f;

    [SerializeField] private Color fullStrengthColor = new Color(0f, 0.6f, 1f, 1f);
    [SerializeField] private Color midStrengthColor = Color.green;
    [SerializeField] private Color lowStrengthColor = new Color(1f, 0f, 1f, 1f);

    private Vector3[] vertices;
    private List<int>[] neighbors;

    private float[] nodeIntensity;
    private float[] nodeTimer;
    private float[] nodeCooldown;
    private float[] nodeActiveTime;
    private GameObject[] nodeObjects;

    private int[] edgeA;
    private int[] edgeB;
    private int[] edgeFrom;
    private int[] edgeTo;
    private LineRenderer[] edgeRenderers;
    private bool[] edgePulseActive;
    private Dictionary<(int, int), int> edgeLookup;
    private Material edgeMaterial;

    private float spawnTimer;
    private float nextSpawnTime;
    private float pulseUpdateTimer;
    private float pulseUpdateInterval = 0.28f;

    struct Pulse
    {
        public int current;
        public int previous;
        public int life;
    }

    private List<Pulse> currentPulses = new List<Pulse>();
    private List<Pulse> nextPulses = new List<Pulse>();

    // ---------------- INIT ----------------

    void Start()
    {
        Mesh mesh = GetComponent<MeshFilter>().mesh;

        vertices = mesh.vertices;

        BuildGraph(mesh);
        SpawnEdges();
        SpawnNodes();

        nodeIntensity = new float[vertices.Length];
        nodeTimer = new float[vertices.Length];
        nodeCooldown = new float[vertices.Length];
        nodeActiveTime = new float[vertices.Length];

        for (int i = 0; i < vertices.Length; i++)
        {
            nodeActiveTime[i] = Random.Range(1f, 2f);
        }

        pulseUpdateTimer = pulseUpdateInterval;
        nextSpawnTime = Random.Range(spawnIntervalMin, spawnIntervalMax);
    }

    void Update()
    {
        HandlePulseSpawning();

        pulseUpdateTimer -= Time.deltaTime;
        if (pulseUpdateTimer <= 0f)
        {
            ProcessPulses();
            pulseUpdateTimer = pulseUpdateInterval;
        }

        UpdateTimers();
        UpdateEdgeTimers();
        UpdateVisuals();
    }

    // ---------------- GRAPH ----------------

    void BuildGraph(Mesh mesh)
    {
        int[] triangles = mesh.triangles;

        HashSet<(int, int)> uniqueEdges = new HashSet<(int, int)>();

        void AddEdge(int a, int b)
        {
            if (a < b) uniqueEdges.Add((a, b));
            else uniqueEdges.Add((b, a));
        }

        for (int i = 0; i < triangles.Length; i += 3)
        {
            int a = triangles[i];
            int b = triangles[i + 1];
            int c = triangles[i + 2];

            AddEdge(a, b);
            AddEdge(b, c);
            AddEdge(c, a);
        }

        neighbors = new List<int>[vertices.Length];

        for (int i = 0; i < vertices.Length; i++)
            neighbors[i] = new List<int>();

        foreach (var (a, b) in uniqueEdges)
        {
            neighbors[a].Add(b);
            neighbors[b].Add(a);
        }

        int edgeCount = uniqueEdges.Count;
        edgeA = new int[edgeCount];
        edgeB = new int[edgeCount];
        edgeFrom = new int[edgeCount];
        edgeTo = new int[edgeCount];
        edgePulseActive = new bool[edgeCount];
        edgeRenderers = new LineRenderer[edgeCount];
        edgeLookup = new Dictionary<(int, int), int>();

        int index = 0;
        foreach (var (a, b) in uniqueEdges)
        {
            edgeA[index] = a;
            edgeB[index] = b;
            edgeLookup[(a, b)] = index;
            edgeLookup[(b, a)] = index;
            edgeFrom[index] = a;
            edgeTo[index] = b;
            index++;
        }
    }

    // ---------------- NODES ----------------

    void SpawnNodes()
    {
        nodeObjects = new GameObject[vertices.Length];

        for (int i = 0; i < vertices.Length; i++)
        {
            GameObject node = Instantiate(nodePrefab, transform);
            node.transform.localPosition = vertices[i];
            node.transform.localScale = Vector3.one * nodeScale;
            node.SetActive(false);

            var r = node.GetComponentInChildren<Renderer>();
            if (r != null)
            {
                var transparentMat = new Material(Shader.Find("Sprites/Default"));
                transparentMat.color = new Color(0f, 1f, 1f, 0f);
                r.material = transparentMat;
            }

            nodeObjects[i] = node;
        }
    }

    void SpawnEdges()
    {
        edgeMaterial = new Material(Shader.Find("Sprites/Default"));
        edgeMaterial.color = new Color(1f, 1f, 1f, 1f);

        for (int i = 0; i < edgeA.Length; i++)
        {
            GameObject edgeObject = new GameObject($"Edge_{edgeA[i]}_{edgeB[i]}");
            edgeObject.transform.SetParent(transform, false);

            var lr = edgeObject.AddComponent<LineRenderer>();
            lr.useWorldSpace = false;
            lr.positionCount = 2;
            lr.SetPosition(0, vertices[edgeA[i]]);
            lr.SetPosition(1, vertices[edgeB[i]]);
            lr.startWidth = lr.endWidth = nodeScale * 0.3f;
            lr.material = edgeMaterial;
            lr.startColor = Color.clear;
            lr.endColor = Color.clear;
            lr.enabled = false;

            edgeRenderers[i] = lr;
        }
    }

    // ---------------- PULSE SPAWN ----------------

    void HandlePulseSpawning()
    {
        spawnTimer += Time.deltaTime;

        if (spawnTimer < nextSpawnTime) return;

        spawnTimer = 0f;
        nextSpawnTime = Random.Range(spawnIntervalMin, spawnIntervalMax);

        if (currentPulses.Count < maxConcurrentSeeds)
        {
            currentPulses.Add(new Pulse
            {
                current = Random.Range(0, vertices.Length),
                previous = -1,
                life = 0
            });
        }
    }

    // ---------------- PULSE SIM ----------------

    void ProcessPulses()
    {
        nextPulses.Clear();

        int activeCount = 0;

        for (int i = 0; i < currentPulses.Count; i++)
        {
            Pulse p = currentPulses[i];

            ActivateNode(p.current);

            var neigh = neighbors[p.current];

            foreach (int n in neigh)
            {
                if (n == p.previous) continue;

                if (Random.value < spreadChance)
                {
                    if (edgeLookup.TryGetValue((p.current, n), out int edgeIndex))
                        ActivateEdge(edgeIndex, p.current, n);

                    nextPulses.Add(new Pulse
                    {
                        current = n,
                        previous = p.current,
                        life = p.life + 1
                    });
                }
            }

            p.life++;

            if (p.life < maxLife)
            {
                nextPulses.Add(p);
            }

            activeCount++;
        }

        var temp = currentPulses;
        currentPulses = nextPulses;
        nextPulses = temp;
        nextPulses.Clear();
    }

    // ---------------- NODE STATE ----------------

    void ActivateNode(int i)
    {
        if (nodeCooldown[i] > 0f || nodeTimer[i] > 0f) return;
        
        nodeIntensity[i] = 1f;
        nodeTimer[i] = nodeActiveTime[i];
    }

    void ActivateEdge(int edgeIndex, int from, int to)
    {
        if (edgePulseActive[edgeIndex]) return;
        if (nodeIntensity[to] > 0f || nodeCooldown[to] > 0f) return;
        
        edgePulseActive[edgeIndex] = true;
        edgeFrom[edgeIndex] = from;
        edgeTo[edgeIndex] = to;
    }

    void UpdateTimers()
    {
        for (int i = 0; i < nodeIntensity.Length; i++)
        {
            if (nodeTimer[i] > 0f)
            {
                nodeTimer[i] -= Time.deltaTime;
                nodeIntensity[i] = Mathf.Max(0f, nodeTimer[i] / nodeActiveTime[i]);
                
                if (nodeTimer[i] <= 0f)
                {
                    nodeCooldown[i] = 0.5f;
                }
            }
            
            if (nodeCooldown[i] > 0f)
            {
                nodeCooldown[i] -= Time.deltaTime;
            }
        }
    }

    void UpdateEdgeTimers()
    {
        for (int i = 0; i < edgePulseActive.Length; i++)
        {
            if (edgePulseActive[i] && nodeIntensity[edgeTo[i]] <= 0f)
                edgePulseActive[i] = false;
        }
    }

    Color GetPulseColor(float normalized)
    {
        normalized = Mathf.Clamp01(normalized);
        if (normalized > 0.5f)
        {
            return Color.Lerp(midStrengthColor, fullStrengthColor, (normalized - 0.5f) * 2f);
        }
        return Color.Lerp(lowStrengthColor, midStrengthColor, normalized * 2f);
    }

    // ---------------- VISUALS ----------------

    void UpdateVisuals()
    {
        for (int i = 0; i < edgeRenderers.Length; i++)
        {
            var lr = edgeRenderers[i];
            if (lr == null) continue;

            if (edgePulseActive[i])
            {
                lr.enabled = true;
                lr.SetPosition(0, vertices[edgeFrom[i]]);
                lr.SetPosition(1, vertices[edgeTo[i]]);
                float alpha = nodeIntensity[edgeTo[i]] * lineAlphaScale;
                Color lineColor = GetPulseColor(nodeIntensity[edgeTo[i]]) * alpha;
                lr.startColor = lineColor;
                lr.endColor = lineColor;
            }
            else
            {
                lr.enabled = false;
            }
        }

        for (int i = 0; i < nodeObjects.Length; i++)
        {
            GameObject node = nodeObjects[i];
            float intensity = nodeIntensity[i];
            bool active = intensity > 0f;

            node.SetActive(active);
            if (!active) continue;

            var r = node.GetComponentInChildren<Renderer>();
            if (r == null) continue;

            Color baseColor = GetPulseColor(intensity);
            baseColor.a = Mathf.Clamp01(intensity * nodeAlphaScale);
            r.material.SetColor("_Color", baseColor);
        }
    }
}