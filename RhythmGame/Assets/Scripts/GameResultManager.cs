using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using System.IO;
using UnityEngine.SceneManagement;
using Firebase;
using Firebase.Database;
using Firebase.Unity.Editor;

public class GameResultManager : MonoBehaviour
{
    public Text musicTitleUI;
    public Text scoreUI;
    public Text maxComboUI;
    public Image RankUI;

    public Text Rank1UI;
    public Text Rank2UI;
    public Text Rank3UI;

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

        Rank1UI.text = "데이터를 불러 오는중 입니다.";
        Rank2UI.text = "데이터를 불러 오는중 입니다.";
        Rank3UI.text = "데이터를 불러 오는중 입니다.";

        // 데이터베이스에 접근
        DatabaseReference reference;
        FirebaseApp.DefaultInstance.SetEditorDatabaseUrl("https://rhythmgame-tutorial.firebaseio.com/");
        reference = FirebaseDatabase.DefaultInstance.GetReference("ranks").Child(PlayerInformation.selectedMusic);

        // 모든 데이터를 Json형태로 가져옵니다.
        reference.OrderByChild("score").GetValueAsync().ContinueWith // OrderByChild로 내림차순으로 데이터를 가져온다.
        (
            task =>
            {
                // 성공적으로 데이터를 가져왔다면.
                if(task.IsCompleted)
                {
                    List<string> rankList = new List<string>();
                    List<string> emailList = new List<string>();
                    DataSnapshot snapshot = task.Result;

                    // Json데이터의 각 원소에 접근.
                    foreach(DataSnapshot data in snapshot.Children)
                    {
                        IDictionary rank = (IDictionary)data.Value;
                        emailList.Add(rank["email"].ToString());
                        rankList.Add(rank["score"].ToString());
                    }

                    //내림차순으로 정렬된 데이터를 뒤집어준다.
                    emailList.Reverse();
                    rankList.Reverse();

                    // Top 3의 데이터를 출력.
                    Rank1UI.text = "플레이 한 사용자가 없습니다.";
                    Rank2UI.text = "플레이 한 사용자가 없습니다.";
                    Rank3UI.text = "플레이 한 사용자가 없습니다.";
                    List<Text> textList = new List<Text>();
                    textList.Add(Rank1UI);
                    textList.Add(Rank2UI);
                    textList.Add(Rank3UI);
                    for(int i = 0; i < textList.Count; ++i)
                    {
                        textList[i].text = (i + 1) + "위: " + emailList[i] + "님, (" + rankList[i] + "점)";
                    }
                }
            }            
        );

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
