using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FirstButton : MonoBehaviour
{
    Button m_firstButton;

    void OnEnable()
    {
        m_firstButton = GetComponent<Button>();

        m_firstButton.Select();
    }
}
