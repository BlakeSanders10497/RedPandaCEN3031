using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HoverItem : MonoBehaviour
{
    Renderer m_Renderer;
    private Color original;

    void Start()
    {
        //Fetch the Renderer component of the GameObject
        m_Renderer = GetComponent<Renderer>();
        original = m_Renderer.material.color;
    }

    //Run your mouse over the GameObject to change the Renderer's material color to red
    void OnMouseOver()
    {
        m_Renderer.material.color = Color.grey;
        Debug.Log(this.transform.name);
    }

    //Change the Material's Color back to white when the mouse exits the GameObject
    void OnMouseExit()
    {
        m_Renderer.material.color = original;
    }

    // Update is called once per frame
    //void Update()
    //{

    //}
}