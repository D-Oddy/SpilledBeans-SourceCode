using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

/// <summary>
/// Class containing all ant AI behaviour
/// </summary>
/// <remarks>
/// Handles ant patroling, detecting beans and stealing beans
/// </remarks>
public class AntAI : MonoBehaviour
{
    enum State { Patrol, Found, Collected, Killed }     //The current state the ant is in- determines what the ant does and when
    State m_currentState;                               //The state the ant is in/ what functions it will run based on the enum
                                         
    NavMeshAgent m_agent;                               //The ant's NavMesh component
                                                  
    [SerializeField]                              
    float m_wayPointRange;                              //Boundary for new waypoint's distance from previous one
    [SerializeField]
    float m_speedRange;                                 //Boundaries for min and max speed
    float   m_lookRadius = 2f;                          //How far away the ant can 'see' beans from
      
    bool    m_nextPointSet;                             //Has the next waypoint been set?

    Vector3 m_wayPoint;                                 //The current point for the ant to travel to

    [Space(10)]
    [SerializeField]
    GameObject m_carriedBean;                           //The bean object shown once the ant has picked one up
    GameObject m_beanTarget;                            //Used to set waypoint to position of a bean

    Animator       m_animator;                          //Ainmator that controls the ant's animations
    AudioSource    m_deathSound;                        //Sound efect for ant dying
    [SerializeField]
    ParticleSystem m_splatter;                          //Blood splatter when the ant is killed

    // Start is called before the first frame update
    void Start()
    {
        m_currentState = State.Patrol;              //Set default state
                                                   
        m_agent = GetComponent<NavMeshAgent>();     //Assign navmesh component
                                                   
        m_animator   = GetComponent<Animator>();    //Assign animator component
        m_deathSound = GetComponent<AudioSource>(); //Assign audio source
    }

    // Update is called once per frame
    void Update()
    {
        //Assign functions to the different states
        switch( m_currentState )
        {
            case State.Patrol:
                Patrol();
                break;
            case State.Found:
                BeanFound();
                break;
            case State.Collected:
                BeanCollected();
                break;
            case State.Killed:
                Killed();
                break;
        }
    }

    #region Collisions

    void OnCollisionEnter( Collision other )
    {
        //Kill the ant if it gets hit by the player
        if( other.gameObject.CompareTag ( "Player" ) )
        {
            m_currentState = State.Killed;      //Change the state to 'killed'
        }
        if( other.gameObject.CompareTag ( "NavObstacle" ) )
        {
            m_nextPointSet = false;             //Give the ant a new waypoint if it hits an obstacle (stops it from getting stuck on objects on the way to its target point)
        }
    }

    /// <summary>
    /// Detects beans within the ant's trigger collider
    /// </summary>
    /// <param name="other">Object inside the trigger</param>
    void OnTriggerStay( Collider other )
    {
        if( other.gameObject.CompareTag ( "Bean" ) )
        {
            if( m_currentState != State.Collected ) //Can't collect a bean if one has akready been collected
            {
                m_beanTarget = other.gameObject;    //Set the waypoint target to the bean that was in the trigger

                m_currentState = State.Found;       //Do functions for when a bean has been founnd
            }
            else
            {
                m_currentState = State.Collected;   //If a bean is already collected, do not change states
            }       
        }
    }

    #endregion Collisions

    #region patrolState
    /// <summary>
    /// Cycles through random waypoints, moving the ant to each one // When reached, find a new one
    /// </summary>
    void Patrol()
    {
        float turnSpeed = 8f;       //Set the speed at which the ant turns to face its new waypoint
        float pointRadius = 0.1f;   //Distance used to check if the point is in a valid area

        Vector3 wayPointDistance = transform.position - m_wayPoint;                         //Determine the distance between the ant and its waypoint
        Quaternion lookRotation = Quaternion.LookRotation(m_wayPoint - transform.position); //Determine the rotation from the ant to the waypoint

        if ( !m_nextPointSet ) //If a valid way point has not yet been found:
        {
            FindWayPoint();    //Find a new awypoint for the ant to move to
        }
        else                   //If a point was successfuly found:
        {
            m_agent.SetDestination( m_wayPoint ); //Set the waypoint that was found to the nav mesh's new destination

            transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, turnSpeed * Time.deltaTime); //Rotate the ant towards its waypoint
        }

        if( wayPointDistance.magnitude < m_agent.stoppingDistance || !Physics.CheckSphere( m_wayPoint, pointRadius, m_agent.areaMask ))  //If waypoint was reached or is out of bounds
        {
            m_nextPointSet = false; //Point has not been set
        }
    }

    /// <summary>
    /// Finds a new random point for the ant to move to
    /// </summary>
    void FindWayPoint()
    {
        //Define a random waypoint position on the x and x axis using waypoint range to determine distance from last point
        float xOffset = Random.Range( -m_wayPointRange, m_wayPointRange );
        float zOffset = Random.Range( -m_wayPointRange, m_wayPointRange );

        //Set this to the position of our waypoint
        m_wayPoint = new Vector3( transform.position.x + xOffset, transform.position.y, transform.position.z + zOffset );

        //Vary speed for each waypoint- to make to movement more random and unpredictable
        float moveSpeed = Random.Range( m_speedRange, ( m_speedRange / 2f ) );

        //Set the new speed each time a new waypoint is given
        m_agent.speed = moveSpeed;

        //Check if the point is in our specified area
        if( Physics.Raycast( m_wayPoint, -transform.up, 1f, m_agent.areaMask ) )
        {
            m_nextPointSet = true; 
        }

        StartCoroutine( SafetyNet() ); //For robustness, ensure the ant doesn't get stuck in one position
    }

    /// <summary>
    /// Allows time to reset the patrol in the event that the ant fails to reach its waypoint
    /// Repeat for extra robustness 
    /// </summary>
    /// <returns></returns>
    IEnumerator SafetyNet()
    {
        float safetyMargin = 5f;                        //How long to wait before forcing a new waypoint

        Vector3 prevPos = transform.position;           //Temporary value to store ant's last position

        yield return new WaitForSeconds( safetyMargin );//Wait for safety margin

        //After margin has passed
        if( transform.position == prevPos )
        {
            //Assign a new waypoint if the ant's position is the same as it was before the safety margin
            m_nextPointSet = false;
        }

        Vector3 prevPos2 = transform.position;           //Temporary value to store ant's last position

        yield return new WaitForSeconds( safetyMargin );//Wait for safety margin

        if( transform.position == prevPos2 )
        {
            //Assign a new waypoint if the ant's position is the same as it was before the safety margin
            m_nextPointSet = false;
        }
    }
    #endregion patrolState

    /// <summary>
    /// Moves ant towards any bean in its vicinity
    /// </summary>
    /// <param name="foundBean">Transform of the bean that was found</param>
    void BeanFound()
    { 
        m_wayPoint = m_beanTarget.transform.position;                        //Make the navmesh's waypoint the position of the bean that was found
                                                                             
        float distance = Vector3.Distance( m_wayPoint, transform.position ); //Find the distance between the ant and the bean

        //If the ant does ont currently have a bean
        if( m_currentState != State.Collected && m_wayPoint != null )
        {
            //Define and set new look rotation to be facing the bean 
            Quaternion lookRotation = Quaternion.LookRotation( m_wayPoint - transform.position );
            transform.rotation = Quaternion.Slerp( transform.rotation, lookRotation, 8f * Time.deltaTime );

            //Distance between ant and bean is within the ant's view distance
            if( distance <= m_lookRadius )
            {
                m_agent.SetDestination( m_wayPoint ); //Move ant towards the bean
            }
            //If the ant reaches the bean
            if( distance <= m_agent.stoppingDistance )
            {
                m_currentState = State.Collected;     //Change state to show the bean has been collected
            }        
        }
        else
        {
            m_currentState = State.Patrol;            //If a bean has already been collected, keep the state as patrol
        }     
    }

    /// <summary>
    /// Gives the ant a bean to carry on its back
    /// </summary>
    /// <param name="stolenBean">The bean being picked up</param>
    void BeanCollected()
    {
        m_beanTarget.SetActive( false ); //Collect the bean 
        m_carriedBean.SetActive( true ); //Make it appear on the ant's back

        Patrol();                        //Keep the state set to collected while the ant has the bean, while still executing the patrol function
    }

    /// <summary>
    /// Handles the ant's 'killed' state
    /// </summary>
    void Killed()
    {
        m_animator.SetTrigger( "die" );     //Trigger the ant's death animation

        m_agent.speed = 0f;                 //Stop the ant from moving
        m_deathSound.Play();                //Play death sound effect
        m_splatter.gameObject.SetActive( true );
        m_splatter.Play();                  //Play blood splatter animation

        enabled = false;                    //Disable this script for added robustness
    }
}