using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shelf : MonoBehaviour
{
    [SerializeField]
    GameObject m_bracket;   //Game Object in place to hold the shelf up before it collapses

    void OnTriggerEnter( Collider other )
    {
        if( other.gameObject.tag == "Player" ) //When the player hits a certain point on the shelf
        {
            //Remove the bracket causing the shelf to fall down
            Destroy( m_bracket.gameObject );
        } 
    }
}
