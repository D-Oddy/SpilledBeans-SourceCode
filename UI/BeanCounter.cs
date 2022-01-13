using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;

/// <summary>
/// Class for the UI bar representing beans
/// </summary>
public class BeanCounter : MonoBehaviour
{
    [SerializeField]
    BeanManager m_beanManager;              //Reference to the player's bean manager

    TMPro.TextMeshProUGUI m_beansText;      //UI text to display the amount of beans

    Animator m_textAnim;                    //Animator that controls text ui animations
    [SerializeField]
    Animator    m_imageAnim;                //Animator for glow image object
    AudioSource m_soundEffect;              //Audio source for the bean pickup sound

    void Start()
    {
        //Assign these variables to the corresponding components on this object
        m_beansText   = GetComponent<TMPro.TextMeshProUGUI>();
        m_textAnim    = GetComponent<Animator>();
        m_soundEffect = GetComponent<AudioSource>(); 
    }

    void Update()
    {
        //Match the value of the bean counter text to the player's current beans integer
        m_beansText.text = m_beanManager.GetBeanCount().ToString() + "/" + m_beanManager.GetBeanMax();
    }

    public void Lose()
    {
        //Play visual and audio effects for losing a bean
        m_textAnim.Play( "PulseDown" );
        m_soundEffect.Play();
    }

    public void Gain()
    {
        //Play visual and audio effects for gaining a bean
        m_textAnim.Play( "PulseUp" );
        m_imageAnim.Play( "PulseUp" );
        m_soundEffect.Play();
    }
}