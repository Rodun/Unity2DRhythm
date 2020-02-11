using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameResultManager : MonoBehaviour
{
    public Text musicTitleUI;
    public Text scoreUI;
    public Text maxComboUI;

    void Start()
    {
        maxComboUI.text = PlayerInformation.maxCombo.ToString();
        scoreUI.text = PlayerInformation.score.ToString();
        musicTitleUI.text = PlayerInformation.musicTitle;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
