using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Handles the functions for when the player reaches the end of a level
/// </summary>
public class Goal : MonoBehaviour
{
    [SerializeField]
    GameStateManager m_gameState;     //Reference to the game state manager
    [SerializeField]
    LevelData        m_levelData;     //Reference to the level's data i.e. high scores, time limits, collectibles

    GameObject  m_beacon;             //Light beacon object to indicate that this is the goal
    [SerializeField]
    Animator    m_playerAnim;         //The player object's animator controler
    AudioSource m_successMusic;       //Success sound

    // Start is called before the first frame update
    void Start()
    {
        m_beacon = this.gameObject.transform.GetChild( 0 ).gameObject; //Beacon object is this object's child

        m_successMusic = GetComponent<AudioSource>();                  //Assign sound effect
       
        PlayerPrefs.SetInt( "menuCamera", 1 );                         //Change the menu camera- go back to level selection instead of main menu screen once a level is complete
    }

    void OnTriggerEnter( Collider other )
    {
        if( other.CompareTag ("Player") )        //When the player enters the goal area
        {
            //Update the level the player has reached to the current one they just completed. This will unlock the next level
            if( m_levelData.GetLevelNumber() + 1 > PlayerPrefs.GetInt( "levelReached" ) && m_levelData.GetLevelNumber() < 4 )
            {
                PlayerPrefs.SetInt( "levelReached", m_levelData.GetLevelNumber() + 1 );              
            }

            m_gameState.LevelComplete();         //Change the level state to complete
                                                 
            m_playerAnim.enabled = true;         //Activate the player's animator 
            m_playerAnim.applyRootMotion = false;
            m_playerAnim.SetTrigger( "Goal" );   //Makes the player face the front for the end screen

            other.gameObject.transform.position = new Vector3( transform.position.x - 0.7f, transform.position.y + 1.1f, transform.position.z );//Move the player into position to pour the beans into the pan

            m_beacon.SetActive( false );         //Remove the light beacon                                             
            m_successMusic.Play();               //Play the success sound
        }
    }
}
