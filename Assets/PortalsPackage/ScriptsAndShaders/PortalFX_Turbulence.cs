using System;
using UnityEngine;

[ExecuteInEditMode]

public class PortalFX_Turbulence : MonoBehaviour
{
    public float TurbulenceStrenght = 1;
    public Vector3 Frequency = new Vector3(1, 1, 1);
    public Vector3 OffsetSpeed = new Vector3(0.5f, 0.5f, 0.5f);
    public Vector3 Amplitude = new Vector3(5, 5, 5);
    public Vector3 GlobalForce;

    private float lastStopTime;
    private Vector3 currentOffset;
    private float deltaTime;
    private ParticleSystem.Particle[] particleArray;
    private ParticleSystem particleSys;
    private float time;

    private void Start()
    {
        particleSys = GetComponent<ParticleSystem>();

        if (particleArray==null || particleArray.Length < particleSys.maxParticles) 
            particleArray = new ParticleSystem.Particle[particleSys.maxParticles];
    }


    private void Update()
    {
        int numParticlesAlive = particleSys.GetParticles(particleArray);
        if (!Application.isPlaying) {
            deltaTime = Time.realtimeSinceStartup - lastStopTime;
            lastStopTime = Time.realtimeSinceStartup;
        }
        else
            deltaTime = Time.deltaTime;
        currentOffset += OffsetSpeed * deltaTime;
       
        
        for (int i = 0; i < numParticlesAlive; i++) {
            var particle = particleArray[i];
            float timeTurbulenceStrength = 1;
            var pos = particle.position;
            pos.x /= Frequency.x;
            pos.y /= Frequency.y;
            pos.z /= Frequency.z;
            var turbulenceVector = new Vector3();
            turbulenceVector.x = ((Mathf.PerlinNoise(pos.z - currentOffset.z, pos.y - currentOffset.y) * 2 - 1) * Amplitude.x + GlobalForce.x) * deltaTime;
            turbulenceVector.y = ((Mathf.PerlinNoise(pos.x - currentOffset.x, pos.z - currentOffset.z) * 2 - 1) * Amplitude.y + GlobalForce.y) * deltaTime;
            turbulenceVector.z = ((Mathf.PerlinNoise(pos.y - currentOffset.y, pos.x - currentOffset.x) * 2 - 1) * Amplitude.z + GlobalForce.z) * deltaTime;

            turbulenceVector *= TurbulenceStrenght * timeTurbulenceStrength;
            particleArray[i].position += turbulenceVector;
		}
        particleSys.SetParticles(particleArray, numParticlesAlive);
    }
}