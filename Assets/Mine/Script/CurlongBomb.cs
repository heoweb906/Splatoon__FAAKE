using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CurlongBomb : MonoBehaviour
{
    public ParticleSystem colliderParticle;


    // Start is called before the first frame update
    void Awake()
    {
        colliderParticle.Play();

        Invoke("ParticleStop", 0.65f);
        Destroy(gameObject, 1f);
    }


    private void ParticleStop()
    {
        colliderParticle.Stop();
    }



}
