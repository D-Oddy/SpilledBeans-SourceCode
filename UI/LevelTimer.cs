using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// Class holding everything relating to the countdown timer
/// </summary>
public class LevelTimer : MonoBehaviour
{
    #region Variables

    [Header("Script References")]
    [SerializeField]
    GameStateManager m_gameState;           //Reference to game state manager
    [SerializeField]
    PlayerMovement   m_movement;            //Reference to player movement 

    [Header("Text Objects")]
    TMPro.TextMeshProUGUI m_timerText;      //UI text to display the timer value
    [Space (8)]
    [SerializeField]
    TMPro.TextMeshProUGUI m_timeUpText;     //Text displayed when time runs out
    [SerializeField]
    TMPro.TextMeshProUGUI m_countDownText;  //UI text to display the start countdown


    [Space(10)]
    [SerializeField]
    float m_levelTime;                      //Level's time limit at the start of the scene
    float m_currentTime;                    //Current time remaining in one float  
    float m_elapsedTime;                    //Current time elapsed in one float

    bool  m_active  = false;                //Is the timer currently counting?
    bool  m_lowTime = false;                //Is the time running low?

    [Header("Visuals and Audio")]
    [SerializeField]
    AudioSource m_gameMusic;                //Reference to the game's background music
    [SerializeField]
    AudioSource m_lowTimeSound;             //Audio source for the sound made when time is running low
    AudioSource m_bonusSound;               //Audio source for the sound made when adding time
    Animator    m_textAnim;                 //Animator for this text object (level timer)
    [SerializeField]
    Animator    m_bonusTextAnim;            //Animator for bonus text object

    #endregion Variables

    void Start()
    {
        m_timerText = GetComponent<TMPro.TextMeshProUGUI>();

        m_currentTime = m_levelTime;

        m_bonusSound = GetComponent<AudioSource>();
        m_textAnim = GetComponent<Animator>();

        StartCoroutine( CountDown() ); //Introduce the level with a countdown on screen
    }

    #region Update
    // Update is called once per frame
    void Update()
    {  
        if( m_active )
        {
            m_elapsedTime = m_levelTime - m_currentTime;  //Count up the time that has passed

            if( m_currentTime > 1f )//If the timer is on more than 1
            {
                m_currentTime -= Time.deltaTime;                       //Count down the time remaining

                int minutes = Mathf.FloorToInt( m_currentTime / 60f ); //Calculate minutes
                int seconds = Mathf.FloorToInt( m_currentTime % 60f ); //Calculate seconds
                m_timerText.text = minutes.ToString( "0" ) + ":" + seconds.ToString( "00" ); //Convert time to strings and set to timer text
            }
            else                    //If the timer reaches 0 or less
            {
                m_active = false;   //Stop the timer
                m_gameMusic.Stop(); //Stop the music
                m_timeUpText.text = ( "Time's Up!" );

                StartCoroutine( m_gameState.GameOver() ); //Call game state manager's game over function
            }

            TimeAlerts(); //Alert the player when the timer reaches certain values
        }
        if( GameStateManager.m_gameOver ) 
        {
            m_active = false; //Stop the timer from counting down when the player has failed and the game over screen is showing
        }
    }
    #endregion Update

    void TimeAlerts()
    {
        if( m_currentTime < 31 && !m_lowTime ) //When there are 30 seconds left
        {
            //To inndicate to the player that time is running out:
            m_gameMusic.pitch = 1.2f;                              //Increase speed and pitch of the music
            m_textAnim.Play( "Pulse3" );                           //Pulse the size of the timer text 3 times
            m_lowTimeSound.Play();                                 //Play a sound effect
            m_lowTime = true;                                      
        }

        if( m_currentTime < 16 )               //When there are 15 seconds left
        {
            //To indicate to the player that time is very low:
            m_timerText.DOColor( new Color( 1f, 0.3f, 0f ), 0.5f );//Change the colour of the timer text
            m_textAnim.Play( "PulseLoop" );                        //Have it pulse in size constantly
        }
    }

    //Getters
    public float GetLevelTimeValue() { return m_levelTime; }

    public float GetTimerValue() { return m_currentTime; }

    public float GetElapsedValue() { return m_elapsedTime; }
    //Getters

    /// <summary>
    /// Counts down from 3 at the beginning of a level
    /// </summary>
    /// <returns></returns>
    IEnumerator CountDown()
    {       
        float countDownValue = 3;                     //Vaule of the introduction countdown
                                                         
        yield return new WaitForSeconds( 1f );        //Delay before starting countdown

        m_countDownText.gameObject.SetActive( true ); //Show countdown text object

        while( countDownValue > 0 )
        {
            m_countDownText.text = countDownValue.ToString( "0" ); //Display value on the text object being used

            yield return new WaitForSeconds( 1f ); //After 1 second

            countDownValue--;                      //Reduce vaule by 1
        }

        //Once the countdown reaches 0:
        m_countDownText.text = "GO!";              //Display the text as "GO!"                                                  
        m_active = true;                           //Start the timer                                                
        m_movement.enabled = true;                 //Enable player movement

        yield return new WaitForSeconds( 1f );     //Delay to animate the 'go' text before destroying it when it is off screen

        Destroy( m_countDownText.gameObject );     //Destroy the countdown when it is no longer needed
    }

    /// <summary>
    /// Adds ten seconds to the player's remaining time
    /// </summary>
    public void AddSeconds()//See 'PickUpHandler'
    {
        float timeToAdd = 10f;                     //How much time gets added

        m_currentTime = m_currentTime + timeToAdd; //Add the bonus time to the current timer value
        //Indicate to the player that a bonus has been added:
        m_bonusTextAnim.Play( "Bonus" ); //Play text animation
        m_bonusSound.Play();                //Play sound effect
    }
}