using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPooler : MonoBehaviour
{
    public List<GameObject> Notes;
    private List<List<GameObject>> poolsOfNotes;
    public int noteCount = 10;
    private bool more = true;

    void Start()
    {
        for(int i = 0; i < Notes.Count; ++i)
        {
            poolsOfNotes = new List<List<GameObject>>();
            for (int n = 0; n < noteCount; ++n)
            {
                GameObject obj = Instantiate(Notes[i]);
                obj.SetActive(false);
                poolsOfNotes[i].Add(obj);
            }
        }            
    }

    public GameObject getObject(int noteType)
    {
        foreach(GameObject obj in poolsOfNotes[noteType - 1])
        {
            if(!obj.activeInHierarchy)
            {
                return obj;
            }
            
            if(more)
            {
                GameObject _obj = Instantiate(Notes[noteType]);
                poolsOfNotes[noteType - 1].Add(_obj);
                return _obj;
            }
        }

        return null;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
