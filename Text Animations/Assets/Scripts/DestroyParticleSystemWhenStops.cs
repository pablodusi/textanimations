using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyParticleSystemWhenStops : MonoBehaviour
{
    private ParticleSystem particleSystem2;
    
    private void Awake()
    {
        particleSystem2 = GetComponent<ParticleSystem>();
    }

    /*
    public delegate void ParticleSystemStops();
    public event ParticleSystemStops OnParticleSystemStops;
    */

    private void Update()
    {
        /*
        if (particleSystem2.isPlaying)
        {
            
        }*/
    }

}
