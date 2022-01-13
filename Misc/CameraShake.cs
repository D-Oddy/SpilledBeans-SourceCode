using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class CameraShake : MonoBehaviour
{
    CinemachineImpulseSource m_source;

    void Awake()
    {
        //Assign the impulse source to the component on this game object
        m_source = GetComponent<CinemachineImpulseSource>();
    }

    public void ShakeCamera()
    {
        //Shake the screen
        m_source.GenerateImpulse();
    } 
}
