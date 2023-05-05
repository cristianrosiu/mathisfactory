using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HouseController : MonoBehaviour
{
    [SerializeField] private int id;
    [SerializeField] private float score;

    private MeshRenderer renderer;
    
    public float Score   
    {
        get { return score; } 
        set { score = value; }  
    }
    public int Id   
    {
        get { return id; } 
        set { id = value; }  
    }

    private void Awake()
    {
        renderer = gameObject.GetComponent<MeshRenderer>();
    }
    
    private void OnCollisionEnter(Collision other)
    {
        if (other.collider.tag.Equals("lava"))
            renderer.material.SetColor("_Color", Color.red);
    }
}
