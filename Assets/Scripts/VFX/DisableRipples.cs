using UnityEngine;

public class WaterParticleController : MonoBehaviour
{
    public ParticleSystem waterParticles;
    public Collider waterCollider;

    private void OnTriggerEnter(Collider other)
    {
        if (other == waterCollider)
        {
            // Touching water again → start particles
            if (!waterParticles.isPlaying)
                waterParticles.Play();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other == waterCollider)
        {
            // Leaving water → stop particles
            waterParticles.Stop(true, ParticleSystemStopBehavior.StopEmitting);
        }
    }
}