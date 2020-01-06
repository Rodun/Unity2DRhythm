using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance { get; set; }
    public float noteSpeed;

    void Awake()
    {        
        if (instance == null)
            instance = this;
        else if (instance != this)
            Destroy(gameObject);
    }    

    public enum judges
    {
        NONE =0,
        BAD,
        GOOD,
        PERFECT,
        MISS
    }

    void Start()
    {
        
    }

    void Update()
    {
        
    }
}
