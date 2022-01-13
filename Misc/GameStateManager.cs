using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Handling the game's different states and changing to and from each state 
/// </summary>
public class GameStateManager : MonoBehaviour
{
    public static bool m_paused = false;        //For keeping track of whether the game is paused                                                                            
    public static bool m_gameOver = false;      //For keeping track of whether the player has died                                                                            
    public static bool m_levelComplete = false; //For keeping track of whether the level is complete 

    float m_transitionDelay = 2.0f;//Loading scene delay to allow time for transition animation

    [Header("Canvases")]
    [SerializeField]               //Different UI canvases used in the game
    GameObject m_gameCanvas;       
    [SerializeField]               
    GameObject m_pauseCanvas;      
    [SerializeField]               
    GameObject m_gameOverCanvas;   
    [SerializeField]               
    GameObject m_levelEndCanvas;        
                                   
    [Header("Visuals and Audio")]  
    [Space(6)]                     
    [SerializeField]               
    UIAnimator m_sceneTransition;  //UI animator script from the scene transition image object
                                   
    [SerializeField]               
    AudioSource m_gameMusic;       //The background music playing in the level
    AudioSource m_deathMusic;      //The music to play when the player dies/fails

    // Start is called before the first frame update
    void Start()
    {
        //Set the default visibility of each canvas
        m_gameCanvas.SetActive     ( true );
        m_pauseCanvas.SetActive   ( false );
        m_gameOverCanvas.SetActive( false );
        m_levelEndCanvas.SetActive( false );
        //Assign default bool values
        m_gameOver = false;
        m_levelComplete = false;
        //Assign to the audio source component on this object
        m_deathMusic = GetComponent<AudioSource>(); 
    }

    void Update()
    {
        //When the pause button is pressed
        if( Input.GetButtonDown( "Pause" ) )
        {
            //If the game is in progress
            if( !m_levelComplete && !m_gameOver )
            {
                TogglePause();//Pause the game or resume game if it is already paused
            }
        } 
    }

    public void TogglePause()
    {
        StartCoroutine( PauseHandler() );
    }

    /// <summary>
    /// Handles switching between paused and un-paused
    /// </summary>
    /// <returns></returns>
    IEnumerator PauseHandler()
    {
        float pauseDelay = 0.31f;            //Time to leave for menu animation

        m_paused = !m_paused;                //Switch the pause bool to the opposite of its current state

        if( m_paused )
        {
            m_pauseCanvas.SetActive( true ); //Display the pause menu canvas
            m_gameMusic.Pause();             //Pause the game music
        }
        else
        {
            Time.timeScale = 1f;             //Return to normal time
            m_pauseCanvas.SetActive( false );//Stop displaying the pause menu canvas
            m_gameMusic.Play();              //Resume the game music
        }
        
        yield return new WaitForSeconds( pauseDelay );//Slight delay before pausing to leave time for UI animations

        if( m_paused )
        {
            Time.timeScale = 0f;             //Stop the game's time
        }                                    
        else                                 
        {                                    
            Time.timeScale = 1f;             //Return to normal tim
        }                                    
    }

    public void LevelComplete()
    {
        m_gameCanvas.SetActive( false );     //Hide the game's hud
        m_levelEndCanvas.SetActive( true );  //Show the level complete screen

        m_levelComplete = true;
        m_paused = true;
        m_gameMusic.Stop();                      
    }

    public IEnumerator GameOver()
    {
        float gameOverDelay = 1.5f; //Delay before game over screen to allow time for showing player death animations 

        m_gameOver = true;
        m_gameMusic.Stop();

        yield return new WaitForSeconds( gameOverDelay );

        m_gameOverCanvas.SetActive( true ); //Show the game over screen
        m_deathMusic.Play();
    }

    #region ButtonFunctions

    public void ContinueWrapper()
    {
        StartCoroutine( Continue() );
    }

    IEnumerator Continue()
    {
        SceneTransition(); //Fade to black

        yield return new WaitForSeconds( m_transitionDelay );                  //Loading scene delay to allow time for transition animation

        SceneManager.LoadScene( SceneManager.GetActiveScene().buildIndex + 1 );//Load the next scene in the build index
    }

    public void RetryWrapper()
    {
        StartCoroutine( Retry() );
    }

    IEnumerator Retry()
    {
        SceneTransition(); //Fade to black

        yield return new WaitForSeconds( m_transitionDelay );                  //Loading scene delay to allow time for transition animation

        SceneManager.LoadScene( SceneManager.GetActiveScene().buildIndex );    //Load the current scene number (restart the level from scratch)
    }

    public void MenuWrapper()
    {
        StartCoroutine( Menu() );
    }

    IEnumerator Menu()
    {
        SceneTransition(); //Fade to black

        yield return new WaitForSeconds( m_transitionDelay );                  //Loading scene delay to allow time for transition animation

        SceneManager.LoadScene( 0 );                                           //Load the main menu scene
    }

    #endregion ButtonFunctions

    void SceneTransition()
    {
        Time.timeScale = 1f;      //Ensures time is set back to normal
        m_sceneTransition.Exit(); //Play scene transition
    }
}