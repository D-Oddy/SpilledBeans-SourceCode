using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

/// <summary>
/// Class containing all functions that relate to moving the player object
/// </summary>
/// <remarks>
/// Can make the player move on 3 dimensions, jump, ground pound and sprint 
/// </remarks>
public class PlayerMovement : MonoBehaviour
{
    #region variables

    Rigidbody m_rb;                                 //The rigidbody component on the player game object

    Quaternion m_target;                            //Player's look direction
    
    [Header("Speed Settings")]
    [SerializeField]
    float m_walkSpeed = 3.5f;                       //Speed of player when walking
    [SerializeField]
    float m_moveSpeed;                              //Movement speed of the player
    float m_sprintSpeed;                            //Speed of player when sprinting
    [SerializeField]
    float m_sprintMultiplier;                       //Number to multiply walk speed by to get sprint speed
    [SerializeField]
    float m_acceleration;                           //Speed at which player moves from walking to sprinting and vice/verca

    [Header("Forces")]
    [SerializeField]
    float m_jumpForce;                              //Force added when the player jumps
    [SerializeField]
    float m_poundForce = 10f;                       //Force added when the player ground pounds
    [SerializeField]
    float m_weightForce = -8.0f;                    //Value of player's simulated weight
    float m_fallVelocity;                           //Player's velocity
    [SerializeField]
    float m_maxVelocity = 1;                        //Velocity threshold before killing the player

    float m_groundDrag = 8f;                        //Drag when on the ground
    float m_airDrag = 0.6f;                         //Drag when in the air
    float m_groundMultiplier = 10f;                 //Speed multipler when on the ground       
    float m_airMultiplier = 2f;                     //Speed multipler when in the air

    [Header("Grounding")]
    [SerializeField]
    Transform          m_groundCheck;               //Position to check for ground from
    [SerializeField]
    float              m_groundDistance;            //Distance required to be considered grounded
    bool               m_isGrounded;                //Is the player currently on solid ground

    [Header("Visual and Audio Effects")]
    [SerializeField]
    CameraShake        m_cameraShake;               //Camera shake component
    [SerializeField]
    ParticleSystem     m_jumpParticles;             //Dust particle effect for jumping
    AudioSource        m_source;                    //Player object's audio source component
    [SerializeField]
    AudioClip          m_jumpClip;                  //Audio clip played on jump

    #endregion variables

    // Start is called before the first frame update
    void Start()
    {
        m_moveSpeed = m_walkSpeed;

        m_rb = GetComponent<Rigidbody>();
        m_source = GetComponent<AudioSource>();
    }

    #region updates
    // Update is called once per frame
    void Update()
    {
        if( !GameStateManager.m_paused && !GameStateManager.m_gameOver )
        {
            if( Input.GetButtonDown( "Jump" ) && m_isGrounded )
            {
                Jump();       //Jump when specified button is pressed and player is on the ground
            }
            if( Input.GetButtonDown( "Pound" ) && !m_isGrounded )
            {
                GroundPound();//Ground pound when speciifed button is pressed and player is airborne
            }

            SprintControls();
            DragControls();
        }
    }

    // FixedUpdate is called on a reliable timer, independent of the frame rate
    void FixedUpdate()
    {
        if( !GameStateManager.m_paused  )
        {
            //More precise than raycast- can be edited to match player size
            m_isGrounded = Physics.CheckSphere( m_groundCheck.position, m_groundDistance, LayerMask.GetMask( "Ground", "AntGround", "Obstacle" ) );
           
            Vector3 gravity = new Vector3( 0, m_weightForce, 0 );  //Define player's downward gravity force      
            m_rb.AddForce( gravity * m_rb.mass * 5 );              //Add this force to the player's rigidbody

            if( !GameStateManager.m_gameOver )
            {
                MovePlayer();
            }
        }
        if( GameStateManager.m_levelComplete )
        {
            //Freeze the player in position when they  complete the level
            m_rb.constraints = RigidbodyConstraints.FreezePosition;
        }
    }

    #endregion updates

    //Getters and setters
    public float GetVelocity() { return m_fallVelocity; }

    public float GetMaxVelocity() { return m_maxVelocity; }
   
    public void SetVelocity( float velocity ) { m_fallVelocity = velocity; }
    //Getters and setters

    /// <summary>
    /// Moves the player on the x or z axis
    /// </summary>
    /// <remarks>
    /// Adds a force in movement direction, makes player face the direction they move in
    /// </remarks>
    void MovePlayer()
    {
        float m_horizMovement = Input.GetAxisRaw( "Horizontal" ); 
        float m_vertMovement  = Input.GetAxisRaw( "Vertical" );

        Vector3 moveDir = new Vector3( m_horizMovement, 0.0f, m_vertMovement );

        if( m_isGrounded )
        {
            //Use ground speed multiplier when on the ground
            m_rb.AddForce( moveDir.normalized * m_moveSpeed * m_groundMultiplier, ForceMode.Acceleration ); 
        }
        else
        {
            //Use air speed multiplier when in the air
            m_rb.AddForce( moveDir.normalized * m_moveSpeed * m_airMultiplier, ForceMode.Acceleration );
            m_fallVelocity += Time.deltaTime; //Add velocity for the time the player stays falling
        }

        //Define target for player look rotation
        Quaternion target = Quaternion.LookRotation( moveDir.normalized );

        if( moveDir.magnitude != 0f )
        {
            //Face movement direction while moving
            m_rb.MoveRotation( target );
            m_target = Quaternion.LookRotation( moveDir.normalized );
        }
    }

    void Jump()
    {
        m_rb.AddForce( transform.up * m_jumpForce * 2, ForceMode.Impulse );  //Add an upward force to the player's rigidody

        m_jumpParticles.Play();                 //Play dust particle effect
        m_source.clip = m_jumpClip;             //Set jump sound effect clip
        m_source.PlayOneShot( m_source.clip );  //Play jump sound effect clip
    }

    void GroundPound()
    {
       
        m_rb.AddForce( -transform.up * m_poundForce * 2, ForceMode.Impulse );  //Add an downward force to the player's rigidody

        m_cameraShake.ShakeCamera(); //Shake the camera for visual effect
    }

    /// <summary>
    /// Handles player's blend between sprinting and walking
    /// </summary>
    void SprintControls()
    {
        //Calculate sprint speed based on current walking speed. This makes player movement more scaleable
        m_sprintSpeed = m_walkSpeed * m_sprintMultiplier;

        if( Input.GetAxisRaw( "Sprint" ) > 0.1 && m_isGrounded ) //If sprint axis button is being held and palyer is on the ground
        {
            //Gradually increase movement speed from current walk speed to the sprint speed we just calculated
            m_moveSpeed = Mathf.Lerp( m_moveSpeed, m_sprintSpeed, m_acceleration * Time.deltaTime );
        }
        else
        {
            //Gradually decrease movement speed back to its original value from before pressing sprint
            m_moveSpeed = Mathf.Lerp( m_moveSpeed, m_walkSpeed, m_acceleration * Time.deltaTime );
        }
    }

    /// <summary>
    /// Alter drag based on whether the player is grounded
    /// </summary>
    /// <remarks>
    /// Done to simulate more drag/friction when on ground than in the air
    /// </remarks>
    void DragControls()
    {
        if( m_isGrounded )
        {          
            m_rb.drag = m_groundDrag; //Set drag for moving on the ground
        }
        else
        {            
            m_rb.drag = m_airDrag;    //Set drag for moving gthrough the air 
        }
    }
}