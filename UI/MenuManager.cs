using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using Cinemachine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

/// <summary>
/// Class contains functions relating to menu navigation
/// </summary>
public class MenuManager : MonoBehaviour
{
    #region Variables
    [SerializeField]
    InputAction   m_backButton;         //Controller input for going back in the menu

    [SerializeField]
    UIAnimator    m_sceneTransition;    //UI animator for scene transitions
    Animator      m_cameraSwitch;       //Animator for changing the camera position

    [SerializeField]
    RectTransform m_mainMenu;           //Main menu panel (Title, buttons), to be hidden when starting game
    [SerializeField]
    RectTransform m_controlsImage;      //Image to display the controls
    [Space(20)]

    [SerializeField]
    LevelButton[] m_levels;             //Selectable levels
    [SerializeField]
    Button m_playButton;                //Play button in the main menu

    int m_currentCam = 0;               //Current active camera number 
    #endregion Variables

    // Start is called before the first frame update
    void Start()
    {
        m_backButton.performed += _ => Back();

        m_cameraSwitch = GetComponent<Animator>();

        m_currentCam   = PlayerPrefs.GetInt( "menuCamera", 0 );

        m_playButton.Select();
        m_backButton.Enable();

        Cursor.visible = false;

        if( m_currentCam == 1 )
        {
            m_levels[0].GetButton().Select();
        }

        if( PlayerPrefs.GetInt( "levelReached" ) < 2 )
        {
            PlayerPrefs.DeleteAll();
        }

        SetLockedLevels();
    }

    //Update is called once per frame
    void Update()
    {
        //Switch the current camera based on the currentCam variable 
        //Current camera is set in "LevelButton" script in the ShowInfo() function
        switch( m_currentCam )
        {
            case 0:
                MainMenu();
                break;
            case 1:
                LevelsMenu();
                break;
            case 2:
                m_cameraSwitch.Play( "Level 1" );
                break;
            case 3:
                m_cameraSwitch.Play( "Level 2" );
                break;
            case 4:
                m_cameraSwitch.Play( "Level 3" );
                break;
            case 5:
                m_cameraSwitch.Play( "Level 4" );
                break;
            case 6:
                m_cameraSwitch.Play( "Level 5" );
                break;
            case 7:
                m_cameraSwitch.Play( "Level 6" );
                break;
        }
    }

    public void SetCamera( int cameraNumber ) { m_currentCam = cameraNumber; }//Allows other scripts tho change the current camera number

    /// <summary>
    /// Locks all levels that have not yet been reached
    /// </summary>
    void SetLockedLevels()
    {
        int levelReached = PlayerPrefs.GetInt("levelReached", 1);

        for( int index = 0; index < m_levels.Length; ++index )
        {
            if( index + 1 > levelReached )
            {
                m_levels[index].LockLevel(); //Call lock function on each level script
            }
        }
    }

    /// <summary>
    /// Display the main/start menu
    /// </summary>
    void MainMenu()
    {      
        m_cameraSwitch.Play( "Main" );                          //Play camera animation to move to main menu
        m_mainMenu.DOAnchorPos( new Vector2( -220, 0 ), 0.8f ); //Move the main menu on screen
     
        for( int index = 0; index < m_levels.Length; ++index )
        {
            m_levels[index].GetButton().gameObject.SetActive( false );//Stop all level buttons from being pressable when in the main menu
        }
    }

    /// <summary>
    /// Display the level selection menu
    /// </summary>
    void LevelsMenu()
    {       
        m_cameraSwitch.Play( "Levels" );                         //Play camera animation to move to levels menu  
        m_mainMenu.DOAnchorPos( new Vector2( -500f, 0 ), 0.8f ); //Move the main menu off screen

        for( int index = 0; index < m_levels.Length; ++index )
        {
            m_levels[index].GetButton().gameObject.SetActive( true );//Make all level buttons pressable when in the level selection menu 
            m_levels[index].HideInfo();                              //Make sure level info is hidden until a button is pressed
        }
    }

    /// <summary>
    /// Go back to the previous menu screen
    /// </summary>
    public void Back()
    {
        m_controlsImage.DOAnchorPos( new Vector2( -100f, -400f ), 0.8f ); //Move the controls image off screen

        if( m_currentCam > 1 )//If the camera is zoomed in on a level
        {
            m_currentCam = 1; //Go back to the level selection screen camera
            m_levels[0].GetButton().Select();
        }
        else                  //If the camera is on the level select screen
        {
            m_currentCam = 0; //Go back to the main menu camera
            m_playButton.Select();
        }
    }

    /// <summary>
    /// Plays the scene transition animation, waits for a set time and then changes the scene
    /// </summary>
    /// <returns></returns>
    public IEnumerator LoadLevel()
    {
        //Amount of time to allow for the animation to run through
        float duration = 4.0f;

        //Trigger the scene transition
        m_sceneTransition.Exit();
        m_backButton.Disable();

        yield return new WaitForSeconds( duration );

        //Start the level that the camera is currently displaying
        switch( m_currentCam )
        {
            case 2:
                SceneManager.LoadScene( "Level1" );
                break;
            case 3:
                SceneManager.LoadScene( "Level2" );
                break;
            case 4:
                SceneManager.LoadScene( "Level3" );
                break;
        }
    }

    #region MenuButtons

    /// <summary>
    /// Call when the "Play" button is pressed
    /// </summary>
    public void PlayButton()
    {
        m_controlsImage.DOAnchorPos( new Vector2( -100f, -400f ), 0.8f ); //Move the controls image off screen
        m_currentCam = 1;                                                 //Change the camera number to 1, opening the level select menu
        m_levels[0].GetButton().Select();
    }

    /// <summary>
    /// Called when "Controls" button is pressed
    /// </summary>
    public void ShowControls()
    {
        m_controlsImage.DOAnchorPos( new Vector2( 100f, 0f ), 0.8f ); //Display image to show the player the game's controls. 
    }

    /// <summary>
    /// Called when the "Quit" button is pressed
    /// </summary>
    public void Quit()
    {
        //Reset the menu camera and exit the game
        PlayerPrefs.SetInt( "menuCamera", 0 );     
        Application.Quit();
    }

    public void Reset()
    {
        PlayerPrefs.DeleteAll(); //Reset the player's game progress
    }

    #endregion MenuButtons
}
