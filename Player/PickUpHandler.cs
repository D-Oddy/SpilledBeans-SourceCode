using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Handles picking up collectables and calling the relevant functions for each of them
/// </summary>
public class PickUpHandler : MonoBehaviour
{
    BeanManager     m_beanManager;  //Reference to player's bean manager script
    PlayerLives     m_lives;        //Reference to player's lives script
    [SerializeField]
    LevelTimer      m_timer;        //Reference to the level timer

    // Start is called before the first frame update
    void Start()
    {
        //Assign components that are on the player object
        m_beanManager = GetComponent<BeanManager>();
        m_lives       = GetComponent<PlayerLives>();
    }

    /// <summary>
    /// Call relevant functions when each pickup variant is collected
    /// </summary>
    /// <param name="pickUp">The pickup object the player collided with</param>
    void OnTriggerEnter( Collider pickUp )
    {
        //Call relevant function based on the tag of the object the player collided with
        switch ( pickUp.tag )
        {
            case "Bean":     //Player gains a bean
                m_beanManager.BeansUp();
                Destroy( pickUp.gameObject );
                break;
            case "Life":     //Player gains a life
                StartCoroutine( m_lives.GainLife( pickUp.gameObject ) );
                break;
            case "TimeBonus"://Player gains additional time
                m_timer.AddSeconds();
                Destroy( pickUp.gameObject );
                break;
        }
    }
}