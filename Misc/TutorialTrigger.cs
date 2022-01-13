using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// Handles objects that trigger tutorial popups
/// </summary>
public class TutorialTrigger : MonoBehaviour
{
    [SerializeField]
    GameObject m_panel;                     //The panel object that displays the tutorial

    [SerializeField]
    TMPro.TextMeshProUGUI m_textObject;     //The text object on the panel used to display the text
    [SerializeField]
    string m_textContent;                   //The content of the text

    [SerializeField]
    InputAction m_closeAction;              //The input action required to close the tutorial

    [SerializeField]
    UIAnimator  m_uIAnimator;               //The panel's ui animator component

    [SerializeField]
    bool m_requiresInput;                   //Does the tutorial need input in order to disappear
    bool m_active = true;                   //Is the tutorial currently displayed

    // Update is called once per frame
    void Update()
    {
        m_closeAction.performed += ctx => StartCoroutine( Close() );

        if( !m_active )
        {
            m_uIAnimator.Exit();  //Move the panel out of frame if it is no longer active
        }
    }

    /// <summary>
    /// Shows tutorial popup when the player enters a certain area
    /// </summary>
    /// <param name="other">Object colliding with the trigger</param>
    void OnTriggerEnter( Collider other )
    {
        if( other.gameObject.CompareTag ( "Player" ) && m_active == true ) //When the player walks through the trigger area
        {
            if( m_requiresInput )             
            {
                m_closeAction.Enable();         //Start listening for the input action
            }

            m_panel.SetActive( true );          //Start displaying the panel
            StartCoroutine( TypeOutText( m_textContent ) );
        }
    }

    /// <summary>
    /// Simulates animaion that looks like the text is being typed aout to make it feel more dynamic
    /// </summary>
    /// <param name="text"></param>
    /// <returns></returns>
    IEnumerator TypeOutText( string text )
    {
        float charDelay = 0.0005f; //Delay between adding each character (speed of typing)

        m_textObject.text = "";

        foreach( char letter in text.ToCharArray() ) //Add the individual letters from the text one by 
        {
            yield return new WaitForSeconds( charDelay );

            m_textObject.text += letter;

            yield return null;
        }
    }

    /// <summary>
    /// Close popup if the player leaves the trigger box area
    /// </summary>
    /// <param name="other"></param>
    void OnTriggerExit( Collider other )
    {     
        if( other.gameObject.CompareTag ( "Player" ) )
        {
            StartCoroutine( Close() );
        }
    }

    /// <summary>
    /// Close the popup when done with it
    /// </summary>
    /// <returns></returns>
    IEnumerator Close()
    {
        float delay = 1f;                        //Time of delay in seconds

        yield return new WaitForSeconds( delay );//Delay before changeing active status- leaves extra time for the player to read the text

        m_active = false;                        //Stop displaying the panel

        yield return new WaitForSeconds( delay );//Delay before destroying popup object- leaves time to play the exit animation before destroying
                                                
        m_closeAction.Disable();                 //Stop listening for the input action
        m_panel.SetActive( false );              //Set the panel to inactive ready for the next tutorial popup
        Destroy( gameObject );                   //Destroy the trigger object to it can't be activated again
    }
}
