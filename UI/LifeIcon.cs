using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Handles aniamtion and audio of lives ui 
/// </summary>
public class LifeIcon : MonoBehaviour
{
    Animator    m_flash;    //Life icon animator (flash lives to transition in and out
    AudioSource m_gained;   //Sound effect to play when the player gains a life

    // Start is called before the first frame update
    void Start()
    {
        //Component assignment
        m_flash  = GetComponent<Animator>();
        m_gained = GetComponent<AudioSource>();
    }

    public void Lose()
    {
        m_flash.SetTrigger( "Lost" );  //Lose life ui animation
    }

    public void Gain()
    {
        m_flash.SetTrigger( "Gained" );//Gain life ui animation
        m_gained.Play();
    }
}