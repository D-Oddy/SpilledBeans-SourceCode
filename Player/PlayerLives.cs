using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;

/// <summary>
/// Class for handling player health and game over states
/// </summary>
public class PlayerLives : MonoBehaviour
{
    #region Variables

    [SerializeField]
    GameStateManager m_gameState;           //Reference to game state manager
    PlayerMovement   m_movement;            //Reference to player's movement script

    Rigidbody        m_rb;                  //The rigidbody component on the player game object

    [SerializeField]
    LifeIcon[]       m_lifeIcons;           //Life icons in the player's UI
    [Space(10)]

    int              m_maxLives;            //Maximum number of lives 
    int              m_livesLeft;           //Current lives remaining
                                         
    bool             m_vulnerable = true;   //Can the player currently take damage? 
    Renderer         m_renderer;            //Renderer of this oject

    [Header("Visuals and Audio")]   
    [SerializeField]
    Color m_playerColour;                   //Player material colour

    [SerializeField]
    CameraShake      m_cameraShake;         //Camera shake component   

    [SerializeField]
    Animator         m_fullText;            //Text shown when lives are full
    Animator         m_playerAnim;          //Player's animator component
    AudioSource      m_damageSound;         //Sound played when the player takes damage

    #endregion Variables

    // Start is called before the first frame update
    void Start()
    {
        m_movement = GetComponent<PlayerMovement>();

        m_maxLives     = m_lifeIcons.Length;
        m_livesLeft    = m_maxLives;

        m_rb           = GetComponent<Rigidbody>();
        m_renderer     = GetComponent<Renderer>();

        m_playerColour = m_renderer.material.color;

        m_playerAnim   = GetComponent<Animator>();
        m_damageSound  = GetComponent<AudioSource>();
    }

    #region Collisions

    void OnTriggerEnter( Collider other )
    {      
        if( other.gameObject.CompareTag( "Obstacle" ) && !GameStateManager.m_gameOver )
        {         
            StartCoroutine( LoseLife( other.gameObject ) ); //Player loses a life when inside a non-solid obstacle
        }
    }

    /// <summary>
    /// Handles player collisions relating to health
    /// </summary>
    /// <param name="other">The obstacle we collided with</param>
    void OnCollisionEnter( Collision other )
    {
        if( !GameStateManager.m_gameOver ) //Only when the game is not over
        {
            if(other.gameObject.tag == "Obstacle" )
            {
                //Player loses a life when they hit a solid obstacle
                StartCoroutine( LoseLife( other.gameObject ) );
                m_movement.SetVelocity( 0 ); //Reset velocity- stops obstacle knockback from affecting velocity
            }
            if( other.gameObject.tag == "Floor" )
            {
                //Game is over when the player hits the floor
                Failed();
            }

            if( m_movement.GetVelocity() > m_movement.GetMaxVelocity() ) //Player velocity exceeds maximum before landing
            {
                Failed(); //Call fail function- player fell from too high
            }
            else //Velocity did not exceed the max before landing
            {
                m_movement.SetVelocity( 0 ); //Reset velocity back to 0
            }
        }
    }

    /// <summary>
    /// Detectes collisions with particle based obstacles
    /// </summary>
    /// <param name="particle">The particle system we collided with
    /// </param>
    void OnParticleCollision( GameObject particle )
    {
        if( particle.CompareTag ( "Obstacle" ) && !GameStateManager.m_gameOver )
        {
            StartCoroutine( LoseLife( particle ) ); //Player loses a life when they hit a particle obstacle
            m_movement.SetVelocity( 0 );            //Reset velocity- stops obstacle knockback from affecting velocity
        }
    }

    #endregion Collisions

    #region Damage taking

    /// <summary>
    /// Take away one life from the player in timed intervals
    /// </summary>
    /// <param name="damageDealer">The obstacle that damaged the player
    /// </param>
    /// <remarks>
    /// Prevents the player from rapidly losing heath every frame
    /// </remarks>
    IEnumerator LoseLife( GameObject damageDealer )
    {     
        float damageInterval = 1.5f;          //Time after last damaged when the player can be damaged again
         
        if( m_livesLeft >= 1 && m_vulnerable )//Stops lives going below zero
        {
            m_livesLeft--;                    //Take away one life                                       
            m_lifeIcons[m_livesLeft].Lose();  //Remove the corresponding icon from the UI using the m_lifeIcons array

            //Indications to show that damage has been taken:             
            StartCoroutine( DamageFlash() );  //Flash the player's colour  

            Vector3 knockDir = m_rb.transform.position - damageDealer.transform.position; //Calculate direction of knockback
            KnockBack( knockDir );                                                        //Knock the player back away from the obstacle

            m_cameraShake.ShakeCamera();      //Shake the camera for impact    
                                              
            m_damageSound.Play();             //Play a sound to indicate damage
                                                                                    
            m_vulnerable = false;             //Limits the player to losing one life at a time
        }
        if( m_livesLeft == 0 )
        {
            Failed(); //Player has failed when they have 0 lives left
        }

        yield return new WaitForSeconds( damageInterval ); //Delay between each time player takes damage
                                                           
        m_vulnerable = true;                               //Player can nonw take damage again
    }                                                      

    /// <summary>
    /// Flash the player white to indicate damage
    /// </summary>
    /// <returns></returns>
    IEnumerator DamageFlash()
    {
        float flashDelay = 0.4f;                             //Time between the two flashes

        m_renderer.material.DOColor( Color.white, 0f );      //Flash to white
        m_renderer.material.DOColor( m_playerColour, 0.6f ); //Go back to normal colour 

        yield return new WaitForSeconds( flashDelay );       //Delay before 2nd flash

        m_renderer.material.DOColor( Color.white, 0f );      //Flash to white
        m_renderer.material.DOColor( m_playerColour, 0.6f ); //Go back to normal colour 

        yield return null;
    }

    /// <summary>
    /// Knocks the player based on position of what damaged them
    /// Knocks the player up 
    /// </summary>
    /// <param name="knockDir">Direction the player is knocked in</param>
    void KnockBack( Vector3 knockDir )
    {
        float sideForce = 3.5f;        //Amount of sideways force applied to the player
        float upForce = 5.5f;          //Amount of upward force applied to the player

        m_rb.AddForce( knockDir * sideForce, ForceMode.Impulse );  //Push the player in the calculated direction
        m_rb.AddForce( transform.up * upForce, ForceMode.Impulse );//Push the player up
    }

    #endregion Damage taking

    /// <summary>
    /// Gives the player one additional life
    /// </summary>
    public IEnumerator GainLife( GameObject pickUp )//See 'PickUpHandler'
    {
        if( m_livesLeft < m_maxLives ) //If player's lives are not full
        {         
            m_livesLeft++;             //Add one life
            Destroy( pickUp );

            //Add the corresponding icon from the UI using the m_lifeIcons array
            switch( m_livesLeft )
            {
                case 1:
                    m_lifeIcons[0].Gain();
                    break;
                case 2:
                    m_lifeIcons[1].Gain();
                    break;
                case 3:
                    m_lifeIcons[2].Gain();
                    break;
            }
        }
        else //If player's lives are already full when they try to collect a life
        {
            m_fullText.Play( "TextFade" ); //Text animation to show the player their lives are full
        }

        yield return null;
    }

    /// <summary>
    /// Handles player's fail/death state
    /// </summary>
    void Failed()
    {
        StartCoroutine( m_gameState.GameOver() );                //Call game state manager's game over function

        m_rb.constraints = RigidbodyConstraints.None; //Un-freeze the player's rididbody for a ragdoll effect

        //Enable the animator and play the fail animation
        m_playerAnim.enabled = true;
        m_playerAnim.SetBool( "Failed", true );

        m_rb.AddForce( transform.up * 5, ForceMode.Impulse );     //Add upward force
        m_rb.AddTorque( transform.right * 10, ForceMode.Impulse );//Add torque to spin the player slightly

        m_vulnerable = false;                                     //Player can't take damage again while dead
    }
}
