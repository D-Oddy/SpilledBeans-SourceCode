using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Michsky.UI.ModernUIPack;

/// <summary>
/// Keeping track of the player's beans
/// </summary>
/// <remarks>
/// This class handles collecting beans and increasing or decreasing the bean amount
/// </remarks>
public class BeanManager : MonoBehaviour
{
    [SerializeField]
    ProgressBar m_beanCountBar;     //Bar on player representing amount of beans
    [SerializeField]
    BeanCounter m_beanCounter;      //UI for showing amount of beans

    int m_currentBeans;             //Current amount of beans collected
    [Space(10)]
    [SerializeField]
    int m_maxBeans;                 //Maximum number of beans for the current level

    [SerializeField]
    GameObject  m_beanPrefab;       //Prefab bean object to spawn when emptying can

    [Space(10)]
    [SerializeField]
    Transform m_spawnPoint;         //Position to spawn beans from when emptied

    // Start is called before the first frame update
    void Start()
    {
        //Match the max value of the bean counter ui to the player's max beans integer
        m_beanCountBar.maxValue = m_maxBeans;
    }

    // Update is called once per frame
    void Update()
    {
        //Match the current value of the bean counter ui to the player's current beans integer
        m_beanCountBar.currentPercent = m_currentBeans;

        if( GameStateManager.m_gameOver )
        {
            //Spill the beans from the can
            SpillBeans();
        }
        if( GameStateManager.m_levelComplete )
        {
            //Empty the beans from the can after a delay for the animation
            StartCoroutine( EmptyBeans() );
        }
    }

    /// <summary>
    /// Gives access to the current beans value from other scripts
    /// </summary>
    /// <returns></returns>
    public int GetBeanCount() { return m_currentBeans; }

    /// <summary>
    /// Gives access to the max beans value from other scripts
    /// </summary>
    /// <returns></returns>
    public int GetBeanMax() { return m_maxBeans; }

    /// <summary>
    /// Increases the player's beans
    /// </summary>
    public void BeansUp()//See 'PickUpHandler'
    {
        if( m_currentBeans < m_maxBeans ) //If the current aaamount is less than the maximum
        {
            m_currentBeans++;    //Increase the player's current beans 
            m_beanCounter.Gain();//Increase amount of beans in ui counter

            //This statement prevents the player's beans from exceeding their maximum value
            if( m_currentBeans > m_maxBeans )
            {
                m_currentBeans = m_maxBeans;
            }
        }
    }

    /// <summary>
    /// Decreases the player's beans
    /// </summary>
    public void BeansDown()
    {
        if( m_currentBeans > 0 ) //If the current amount is more than 0
        {
            m_currentBeans--;    //Decrease the player's current beans 
            m_beanCounter.Lose();//Decrease amount of beans in ui counter

            //This statement prevents the player's beans from going below 0
            if( m_currentBeans < 0 )
            {
                m_currentBeans = 0;
            }
        }
    }

    /// <summary>
    /// Empty the beans from the can into the pan at the end of the level
    /// </summary>
    /// <returns></returns>
    IEnumerator EmptyBeans()
    {
        float animDelay = 1.5f;     //Delay to leave time for pour animation
        float pourDelay = 0.8f;     //Delatto slow down pouring of beans

        yield return new WaitForSeconds( animDelay );

        for( int index = 0; index < m_currentBeans; ++index )  //Spawn beans until amount collected is reached
        {
            if( m_currentBeans > 0 ) //Stop then beans reaches 0
            {
                BeansDown();         //Reduce current amount of beans

                Instantiate( m_beanPrefab, m_spawnPoint.position, m_spawnPoint.rotation );//Spawn a bean at the fire point position and rotation
                m_beanPrefab.transform.rotation = Random.rotation;                        //Set a random rotation on the bean to add randomness to the pouring

                yield return new WaitForSeconds( pourDelay );   //Add slight delay so that all the beans don't spawn at the exact same time
            }
        }

        yield return null;
    }

    /// <summary>
    /// Spill the beans when the player 'dies'
    /// </summary>
    void SpillBeans()
    {
        for( int index = 0; index < m_currentBeans; ++index )  //Spawn beans until amount collected is reached
        {
            if( m_currentBeans > 0 ) //Stop then beans reaches 0
            {
                BeansDown();         //Reduce current amount of beans 

                Instantiate( m_beanPrefab, m_spawnPoint.position, transform.rotation ); //Spawn a bean at the fire point position and player rotation
            }
        }
    }
}