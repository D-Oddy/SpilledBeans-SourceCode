using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine.UI;

/// <summary>
/// Class handles the level selection menu functions
/// </summary>
public class LevelButton : MonoBehaviour
{
    [SerializeField]
    MenuManager m_menuManager;            //Reference for accessing the menu manager script
    LevelData   m_levelData;              //Reference to the level's data i.e. high scores, time limits, collectibles

    [SerializeField]
    RectTransform m_levelInfo;            //Image for displaying information about the selected level 

    [Space(8)]
    [SerializeField]
    TMPro.TextMeshProUGUI m_bestBeansTMP; //Text showing the player's high score 
    [SerializeField]
    TMPro.TextMeshProUGUI m_bestTimeTMP;  //Texts showing the player's best time 

    [Space(8)]
    public Button m_levelSelect;          //Button for selecting/viewing a level
    public Button m_levelStart;           //Buttin for loading the selected level

    [Space(8)]
    [SerializeField]
    bool m_isLocked;                      //Is the level locked
    [SerializeField]
    GameObject  m_lockedImage;            //Overlay image to display if the level is locked
    AudioSource m_lockedSound;            //Sound effect to play if the level is locked

    // Start is called before the first frame update
    void Start()
    {
        m_levelData   = GetComponent<LevelData>();
        m_lockedSound = GetComponent<AudioSource>();
       
        if( m_levelData.LoadLevelReached() ) //If this level has been reached (previous level has been completed)
        {
            //Set the text elements to read the correct values
            m_bestBeansTMP.text = "Best: " + m_levelData.LoadBeans().ToString();

            int minutesElapsedBest = Mathf.FloorToInt( m_levelData.LoadTime() / 60f ); //Calculate minutes
            int secondsElapsedBest = Mathf.FloorToInt( m_levelData.LoadTime() % 60f ); //Calculate seconds
            m_bestTimeTMP.text = "Best: " + minutesElapsedBest.ToString( "00" ) + ":" + secondsElapsedBest.ToString( "00" );
        }
    }

    //Getters
    public Button GetButton() { return m_levelSelect; }

    public Button GetButton2() { return m_levelStart; }

    public bool GetLocked() { return m_isLocked; }
    //Getters

    /// <summary>
    /// Zoom in on the selected level and display information about it
    /// </summary>
    public void ShowInfo()
    {
        m_levelInfo.DOAnchorPos( new Vector2( -150f, 0f ), 0.4f );   //Move the level info pannel onto the screen
        m_levelSelect.gameObject.SetActive( false );                 //Level selection button can't be pressed again 
        m_levelStart.Select();                                       //Hover over the button that loads the seleted level
        m_menuManager.SetCamera( m_levelData.GetLevelNumber() + 1 ); //Change camera position
    }

    /// <summary>
    /// Stop displaying information
    /// </summary>
    /// <remarks>
    /// See Back() function in Menu Manager script
    /// </remarks>
    public void HideInfo()
    {    
        m_levelInfo.DOAnchorPos( new Vector2( 750f, 0f ), 0.4f );//Move the level info pannel off the screen
        m_levelSelect.gameObject.SetActive( true );              //Level selection button can now be pressed again 
    }

    /// <summary>
    /// Loads up the selected level if it is currently unlocked
    /// </summary>
    public void StartLevel()
    {
        if( !m_isLocked )
        {
            StartCoroutine( m_menuManager.LoadLevel() ); //See menu manager script
        }
        else
        {
            m_lockedSound.Play(); //Audio to indicate the level is locked
        }
    }

    /// <summary>
    /// Set this level to be locked
    /// </summary>
    public void LockLevel()
    {
        m_isLocked = true;              //Change locked bool
        m_lockedImage.SetActive( true );//Display image to show it is locked
    }
}