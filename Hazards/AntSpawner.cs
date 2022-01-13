using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Handles the spawning of the ants to minimise cost
/// </summary>
public class AntSpawner : MonoBehaviour
{
    [SerializeField]
    GameObject[] m_antsToSpawn; //The ant objects that will spawn from this spawner

    bool m_active = true;       //Tracks if the spawner is being used

    void OnTriggerEnter( Collider other )
    {
        //Spawn the ant when the player passes through the trigger
        //Only spawn ants at a certain time- ants are only needed when the player has reached that point in the level
        if( other.gameObject.CompareTag( "Player" ) && m_active )
        {
            for( int index = 0; index < m_antsToSpawn.Length; ++index )
            {
                m_antsToSpawn[index].SetActive( true ); //'Spawn' the ants in the array 

                m_active = false; //Spawner becomes inactive once used as it is no longer needed
            }
        }      
    }    
}
