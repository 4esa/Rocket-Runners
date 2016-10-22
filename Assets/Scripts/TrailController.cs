using UnityEngine;
using System.Collections;

public class TrailController : MonoBehaviour {

    public ParticleSystem[] ParticleSystems;

    public void Thrust()
    {
        foreach (ParticleSystem childParticleSystem in ParticleSystems)
        {
            if (!childParticleSystem.isPlaying)
            {
                childParticleSystem.Play();
            }
        }
    }

    public void ReleaseThrust()
    {
        foreach (ParticleSystem childParticleSystem in ParticleSystems)
        {
            if (childParticleSystem.isPlaying)
            {
                childParticleSystem.Stop();
            }
        }
    }
}
