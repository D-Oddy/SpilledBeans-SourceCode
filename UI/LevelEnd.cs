using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

/// <summary>
/// Class containing functions relating to the level end menu
/// </summary>
public class LevelEnd : MonoBehaviour
{
    [SerializeField]
    LevelTimer  m_timer;                   //Reference to the timer script
    [SerializeField]                       
    BeanManager m_beanManager;             //Reference to the player's bean manager script
    [SerializeField]                       
    LevelData m_levelData;                 //Reference to this level's data script   

    [SerializeField]
    GameObject m_levelInfo;                //Image for displaying information about the selected level 

    [Space(10)]
    [SerializeField]
    TMPro.TextMeshProUGUI m_beansText;     //Text to display how many beans were collected on this try
    [SerializeField]
    TMPro.TextMeshProUGUI m_timeText;      //Text to display how long the player took on this try
    [SerializeField]
    TMPro.TextMeshProUGUI m_bestBeansTMP;  //Text showing the player's best amount of beans collected
    [SerializeField]
    TMPro.TextMeshProUGUI m_bestTimeTMP;   //Texts showing the player's best time achieved

    [SerializeField]
    PlayerCamera m_playerCam;              //Script that allows zooming the camera in

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine( DisplayUI() ); 

        m_playerCam.Zoom();  //Zoom in on the player

        m_beansText.text = m_beanManager.GetBeanCount().ToString( "0" ) + "/" + m_beanManager.GetBeanMax().ToString( "00" ); //Set beans value to the the text content

        int minutesElapsed = Mathf.FloorToInt( m_timer.GetElapsedValue() / 60f );                  //Calculate minutes
        int secondsElapsed = Mathf.FloorToInt( m_timer.GetElapsedValue() % 60f );                  //Calculate seconds
        m_timeText.text = minutesElapsed.ToString( "00" ) + ":" + secondsElapsed.ToString( "00" ); //Convert time to strings and set to timer text content

        //Update the player's best values if they are better than the previous ones
        if( m_beanManager.GetBeanCount() > m_levelData.LoadBeans() )
        {
            m_levelData.SaveBeans( m_beanManager.GetBeanCount() );//Save this value over the previous value
        }                                                         
        if( m_timer.GetElapsedValue() <= m_levelData.LoadTime() ) 
        {                                                         
            m_levelData.SaveTime( m_timer.GetElapsedValue() );    //Save this value over the previous value
        }
     
        bool reached = true;
        m_levelData.SaveLevelReached( reached );                  //Update level reached to be the current level thta was just completed

        m_bestBeansTMP.text = "Best: " + m_levelData.LoadBeans().ToString(); //Load the best beans value to be set to the text's content

        int minutesElapsedBest = Mathf.FloorToInt( m_levelData.LoadTime() / 60f );                                        //Calculate minutes
        int secondsElapsedBest = Mathf.FloorToInt( m_levelData.LoadTime() % 60f );                                        //Calculate seconds
        m_bestTimeTMP.text = "Best: " + minutesElapsedBest.ToString( "00" ) + ":" + secondsElapsedBest.ToString( "00" );  //Load the best time value to be set to the text's content
    }

    /// <summary>
    /// Display the level complete screen
    /// </summary>
    /// <returns></returns>
    IEnumerator DisplayUI()
    {    
        yield return new WaitForSeconds( 2f );  //Delay showing the end screen until after the level complete animations

        m_levelInfo.SetActive( true );          //Show the player their results for this level and the menu buttons 
    }
}