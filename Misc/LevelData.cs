using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Handles saving the player's data and accessing it afterwards
/// </summary>
public class LevelData : MonoBehaviour
{
    [SerializeField]
    int m_levelNumber;       //The number of the level we want to access the data for (set this to whatever level the object is inside)

    float m_bestBeans;       //Player's personal best amount of beans collected for specified level
    float m_bestTime = 181;  //Player's personal fastes time for specified level

    bool m_levelComplete;    //Keeps track of which levels have been complete so the game knows which ones to unlock

    public int GetLevelNumber() { return m_levelNumber; }   //Used to access the level number from outsude this script

    /// <summary>
    /// Saves the player's highest amount of beans reached (given in 'LevelEnd' script)
    /// </summary>
    /// <param name="beans">the value to save</param>
    public void SaveBeans( float beans )
    {
        PlayerPrefs.SetFloat( "bestBeansLevel" + m_levelNumber.ToString(), beans );

        PlayerPrefs.Save();
    }

    /// <summary>
    /// Saves the player's fastest time (given in 'LevelEnd' script)
    /// </summary>
    /// <param name="time">the value to save</param>
    public void SaveTime( float time )
    {
        PlayerPrefs.SetFloat( "bestTimeLevel" + m_levelNumber.ToString(), time );

        PlayerPrefs.Save();
    }

    /// <summary>
    /// Saves the number of the highest level the player has completed
    /// </summary>
    /// <param name="complete">the value to save</param>
    public void SaveLevelReached( bool complete )
    {
        PlayerPrefs.SetInt( "levelReached" + m_levelNumber.ToString(), ( complete ? 1 : 0 ) );

        PlayerPrefs.Save();
    }

    /// <summary>
    /// Loads the player's highest amount of beans reached 
    /// </summary>
    public float LoadBeans()
    {
        m_bestBeans = PlayerPrefs.GetFloat( "bestBeansLevel" + m_levelNumber.ToString() );

        return m_bestBeans;
    }

    /// <summary>
    /// Loads the player's fastest time 
    /// </summary>
    public float LoadTime()
    {
        m_bestTime = PlayerPrefs.GetFloat( "bestTimeLevel" + m_levelNumber.ToString(), 181 );

        return m_bestTime;
    }

    /// <summary>
    /// Loads the number of the highest level the player has completed
    /// </summary>
    public bool LoadLevelReached()
    {
        m_levelComplete = ( PlayerPrefs.GetInt( "levelReached" + m_levelNumber.ToString() ) != 0 );

        return m_levelComplete;
    }
}