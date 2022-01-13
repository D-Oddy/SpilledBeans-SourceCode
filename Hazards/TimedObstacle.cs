using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Class for toggling obstacle type
/// </summary>
public class TimedObstacle : MonoBehaviour
{
    [SerializeField]
    float m_switchInterval;     //Time in between each particle effect fire

    [SerializeField]
    Renderer m_renderer;        //Renderer used to check visibility

    ParticleSystem m_particles; //Particles emitted by the obstacle
    AudioSource m_soundEffect;  //Sound made by particles

    // Start is called before the first frame update
    void Start()
    {
        //Assign sound and particle effects to object's relevant components
        m_soundEffect = GetComponent<AudioSource>();
        m_particles = GetComponent<ParticleSystem>();

        InvokeRepeating( "ToggleEffects", 0f, m_switchInterval );  //Reapeatedly play the sparks effects after specified intervals
    }

    /// <summary>
    /// Toggles the sound and particle effects on and off
    /// </summary>
    void ToggleEffects()
    {
        if( m_renderer.isVisible )
        {  
            //Play the particle and sound effects once
            m_soundEffect.Play();
            m_particles.Play();
        }        
    }
}
