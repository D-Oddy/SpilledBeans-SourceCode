using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class PickUp : MonoBehaviour
{
    Vector3 m_startPos;

    [SerializeField]
    float m_frequency;     //How fast the object moves up and down 
    [SerializeField]
    float m_magnitude;     //How far the object moves up and down     
    [SerializeField]
    float m_spinSpeed;     //How fast the object spins around

    // Start is called before the first frame update
    void Start()
    {
        m_startPos = transform.position; //The original position of the object
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = m_startPos + transform.forward * Mathf.Sin( Time.time * m_frequency ) * m_magnitude; //Oscilate the object up and down

        transform.Rotate( 0, 0, m_spinSpeed * Time.deltaTime ); //Time compensated object rotation by spin speeed
    }
}
