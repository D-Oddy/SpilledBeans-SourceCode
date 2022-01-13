using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingTexture : MonoBehaviour
{
    [SerializeField]
    float m_moveSpeed = 0.1f;  //How fast the texture moves

    Renderer m_renderer;

    // Start is called before the first frame update
    void Start()
    {
        m_renderer = GetComponent<Renderer>();
    }

    // Update is called once per frame
    void Update()
    {
        //Time compensated texture scrolling
        //For optimisation, texture scrolling only happens when the object can be seen
        if( m_renderer.isVisible )
        {
            float movement = Time.time * m_moveSpeed;
            m_renderer.material.SetTextureOffset( "_MainTex", new Vector2( 0, movement ) ); //Constantly offset the texture
        }      
    }
}
