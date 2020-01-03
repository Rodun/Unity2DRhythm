using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NoteBehavior : MonoBehaviour
{
    public int noteType;

    void Start()
    {
        //Debug.Log("Note Speed: " + GameManager.instance.noteSpeed);
    }

    void Update()
    {
        transform.Translate(Vector3.down * GameManager.instance.noteSpeed);
    }
}
