using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

/// <summary>
/// Script for creating UI transitions using DOTween
/// </summary>
public class UIAnimator : MonoBehaviour
{
    [SerializeField]
    RectTransform m_uiElement;    //The desired UI object to be affected 

    public Vector2 m_enterPos;    //Position to slide the ui to when entering the frame
    public Vector2 m_exitPos;     //Position to slide to when exiting the frame
   

    public float m_duration;      //How long it takes to slide from one position to another

    void OnEnable()
    {
        m_uiElement.DOAnchorPos( new Vector2( m_enterPos.x, m_enterPos.y ), m_duration );
    }

    void OnDisable()
    {
        m_uiElement.DOAnchorPos( new Vector2( m_exitPos.x, m_exitPos.y ), m_duration );
    }

    public void Exit()
    {
        m_uiElement.DOAnchorPos( new Vector2( m_exitPos.x, m_exitPos.y ), m_duration );
    }
}
