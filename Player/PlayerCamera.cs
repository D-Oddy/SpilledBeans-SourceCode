using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class PlayerCamera : MonoBehaviour
{
    Animator m_anim; //Animator to control the camera

    // Start is called before the first frame update
    void Start()
    {
        m_anim = GetComponent<Animator>();

        StartCoroutine( Intro() );
    }

    public void Zoom()
    {
        m_anim.Play( "ZoomIn" );  //Play animation to zoom the camera in on the player
    }

    IEnumerator Intro()
    {
        m_anim.Play( "Introduction" );         //Play introduction camera angle animation

        yield return new WaitForSeconds( 3f ); //Delay to allow animation to play

        GameStateManager.m_paused = false;     //Start the gameplay
    }
}
