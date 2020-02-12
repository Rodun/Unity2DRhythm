using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using System.IO;
using UnityEngine.SceneManagement;

public class GameResultManager : MonoBehaviour
{
    public Text musicTitleUI;
    public Text scoreUI;
    public Text maxComboUI;
    public Image RankUI;

    void Start()
    {
        maxComboUI.text = "최대 콤보: " + PlayerInformation.maxCombo.ToString();
        scoreUI.text = "점수: " + (int)PlayerInformation.score;
        musicTitleUI.text = PlayerInformation.musicTitle;

        // 리소스에서 비트(beat) 텍스트 파일을 불러옵니다.
        TextAsset textAsset = Resources.Load<TextAsset>("Beats/" + PlayerInformation.selectedMusic);
        StringReader stringReader = new StringReader(textAsset.text);

        // 첫 번째 줄과 두 번째 줄을 무시합니다.
        stringReader.ReadLine();
        stringReader.ReadLine();

        // 세 번째 줄에 적힌 비트 정보(S, A, B)랭크 점수를 읽습니다.
        string beatInformation = stringReader.ReadLine();
        int scoreS = Convert.ToInt32(beatInformation.Split(' ')[3]);
        int scoreA = Convert.ToInt32(beatInformation.Split(' ')[4]);
        int scoreB = Convert.ToInt32(beatInformation.Split(' ')[5]);

        // 성적에 맞는 랭크 이미지를 불러옵니다.
        if (PlayerInformation.score >= scoreS)
            RankUI.sprite = Resources.Load<Sprite>("Sprites/Rank S");
        else if (PlayerInformation.score >= scoreA)
            RankUI.sprite = Resources.Load<Sprite>("Sprites/Rank A");
        else if (PlayerInformation.score >= scoreB)
            RankUI.sprite = Resources.Load<Sprite>("Sprites/Rank B");
        else
            RankUI.sprite = Resources.Load<Sprite>("Sprites/Rank C");
    }

    public void Replay()
    {
        SceneManager.LoadScene("SongSelectScene");
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
