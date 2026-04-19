using UnityEngine;

public class WireEnergyController : MonoBehaviour
{
    public MeshRenderer targetRenderer;

    public int pulseCount = 14;

    Vector4[] pulses;         // xyz = position, w = intensity
    Vector3[] velocities;     // CPU simulation
    Vector4[] velocityArray;  // GPU upload

    Material mat;

    void Start()
    {
        mat = targetRenderer.material;

        pulses = new Vector4[pulseCount];
        velocities = new Vector3[pulseCount];
        velocityArray = new Vector4[pulseCount];

        for (int i = 0; i < pulseCount; i++)
            Spawn(i);

        mat.SetInt("_PulseCount", pulseCount);
    }

    void Update()
    {
        float dt = Time.deltaTime;
        float t = Time.time;

        // global drift (slow, stable)
        Vector3 globalFlow = new Vector3(
            Mathf.PerlinNoise(t * 0.05f, 0.1f),
            Mathf.PerlinNoise(t * 0.05f + 10f, 0.1f),
            Mathf.PerlinNoise(t * 0.05f + 20f, 0.1f)
        );

        globalFlow = (globalFlow * 2f) - Vector3.one;
        globalFlow = globalFlow.normalized;

        for (int i = 0; i < pulseCount; i++)
        {
            Vector4 p = pulses[i];
            Vector3 pos = new Vector3(p.x, p.y, p.z);

            Vector3 vel = velocities[i];

            float phase = i * 17.3f;

            // local flow field
            Vector3 local = new Vector3(
                Mathf.PerlinNoise(phase, t * 0.15f),
                Mathf.PerlinNoise(phase + 11.7f, t * 0.15f),
                Mathf.PerlinNoise(phase + 41.3f, t * 0.15f)
            );

            local = (local * 2f) - Vector3.one;
            local = local.normalized;

            Vector3 dir = (local * 0.6f + globalFlow * 0.4f).normalized;

            // inertia motion (critical for stability)
            vel = Vector3.Lerp(vel, dir, dt * 2.0f);
            vel *= 0.98f;

            velocities[i] = vel;

            pos += vel * 0.8f * dt;

            // wrap (stable, no snapping)
            if (pos.magnitude > 1.5f)
                pos = -pos.normalized * 1.05f;

            // energy (smooth, non-binary)
            float energy = Mathf.PerlinNoise(
                phase + Mathf.Sin(t * 0.37f) * 10.0f,
                t * 0.11f + phase * 0.13f
            );
            float target = Mathf.Lerp(0.3f, 0.9f, energy);

            p.w = Mathf.Lerp(p.w, target, dt * 1.5f);

            pulses[i] = new Vector4(pos.x, pos.y, pos.z, p.w);

            velocityArray[i] = new Vector4(vel.x, vel.y, vel.z, 0);
        }

        targetRenderer.material.SetVectorArray("_Pulses", pulses);
        targetRenderer.material.SetVectorArray("_Velocities", velocityArray);
    }

    void Spawn(int i)
    {
        Vector3 p = Random.onUnitSphere;

        pulses[i] = new Vector4(p.x, p.y, p.z, Random.Range(0.6f, 1.0f));
        velocities[i] = Random.onUnitSphere * 0.2f;
    }
}