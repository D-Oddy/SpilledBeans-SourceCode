using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Class for playing different sound effects based on what object the player hits
/// </summary>
public class PlayerSounds : MonoBehaviour
{
    [SerializeField]
    bool m_playOnStart = false; //Should a sound play as soon as the object is active in the scene?

    AudioSource m_source;

    [SerializeField]
    AudioClip[] m_startClips;
    [SerializeField]
    AudioClip m_damage;
    [SerializeField]
    AudioClip m_hardHit;
    [SerializeField]
    AudioClip m_softHit;

    // Start is called before the first frame update
    void Start()
    {
        m_source = GetComponent<AudioSource>();

        if( m_playOnStart )
        {
            m_source.clip = m_startClips[Random.Range( 0, m_startClips.Length )];

            m_source.PlayOneShot( m_source.clip );
        }
    }

    void OnCollisionEnter( Collision other )
    {
        switch( other.gameObject.tag )
        {
            case "HardSurface":
                m_source.clip = m_hardHit;              //Set new clip
                m_source.PlayOneShot( m_source.clip );  //Play new clip
                m_source.clip = m_damage;               //Reset back to old clip
                break;
            case "SoftSurface":
                m_source.clip = m_softHit;              //Set new clip
                m_source.PlayOneShot( m_source.clip );  //Play new clip
                m_source.clip = m_damage;               //Reset back to old clip
                break;
        }
    }

    void OnCollisionExit()
    {
        m_source.clip = m_damage;
    }
}
