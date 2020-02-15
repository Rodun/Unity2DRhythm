using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.IO;
using Firebase;
using Firebase.Database;
using Firebase.Unity.Editor;
using System;

public class SongSelectManager : MonoBehaviour
{
    public Text startUI;
    public Text disableAlretUI;
    public Image disablePanelUI;
    public Button purchaseButtonUI;
    private bool disable;

    public Image musicImageUI;
    public Text musicTitleUI;
    public Text bpmUI;

    private int musicIndex;
    private int musicCount = 3;

    // 회원정보 UI
    public Text userUI;

    private void UpdateSong(int musicIndex)
    {
        // 선택곡을 변경하면 초기에는 선택 할 수 없게 변경.
        disable = true;
        disablePanelUI.gameObject.SetActive(true);
        disableAlretUI.text = "데이터를 불러오는 중 입니다.";
        purchaseButtonUI.gameObject.SetActive(true);
        startUI.gameObject.SetActive(false);

        // Audio 초기화
        AudioSource audioSource = GetComponent<AudioSource>();
        audioSource.Stop();

        //리소스에서 비트(Beat) 텍스트 파일을 불러옵니다.
        TextAsset textAsset = Resources.Load<TextAsset>("Beats/" + musicIndex.ToString());
        StringReader stringReader = new StringReader(textAsset.text);
                
        musicTitleUI.text = stringReader.ReadLine(); // 첫 번째 줄에 적힌 곡 이름을 읽어서 UI를 업데이트 합니다.
        stringReader.ReadLine(); // 두 번째 줄을 읽기만하고 아무것도 하지 않는다.
        bpmUI.text = "BPM: " + stringReader.ReadLine().Split(' ')[0]; // 세 번째 줄에 첫번째로 적힌 bpm을 읽어온다.

        // 리소스에서 비트(beat) 음악 파일을 불러와 재생합니다.
        AudioClip audioClip = Resources.Load<AudioClip>("Beats/" + musicIndex.ToString());
        audioSource.clip = audioClip;
        audioSource.Play();
                
        musicImageUI.sprite = Resources.Load<Sprite>("Beats/" + musicIndex.ToString()); // 리소스에서 비트(beat) 이미지 파일을 불러옵니다.

        // 데이터베이스에 접근.
        DatabaseReference reference;
        FirebaseApp.DefaultInstance.SetEditorDatabaseUrl("https://rhythmgame-tutorial.firebaseio.com/");
        reference = FirebaseDatabase.DefaultInstance.GetReference("charges").Child(musicIndex.ToString());

        // 모든 데이터를 Json형태로 가져온다.
        reference.GetValueAsync().ContinueWith
        (
            task =>
            {
                // 성공적으로 데이터를 가져왔을 경우.
                if(task.IsCompleted)
                {
                    DataSnapshot snapshot = task.Result;

                    // 해당 곡이 무료인 경우.
                    if(snapshot == null || !snapshot.Exists)
                    {
                        disable = false;
                        disablePanelUI.gameObject.SetActive(false);
                        disableAlretUI.text = "";
                        purchaseButtonUI.gameObject.SetActive(false);
                        startUI.gameObject.SetActive(true);                        
                    }
                    else
                    {
                        // 해당 곡을 구매했을 경우.
                        if(snapshot.Child(PlayerInformation.auth.CurrentUser.UserId).Exists)
                        {
                            disable = false;
                            disablePanelUI.gameObject.SetActive(false);
                            disableAlretUI.text = "";
                            purchaseButtonUI.gameObject.SetActive(false);
                            startUI.gameObject.SetActive(true);                            
                        }
                        
                        // 사용자가 곡을 구매했는지 확인.
                        if(disable)
                        {
                            disablePanelUI.gameObject.SetActive(true);
                            disableAlretUI.text = "플레이 할 수 없는 곡입니다.";
                            purchaseButtonUI.gameObject.SetActive(true);
                            startUI.gameObject.SetActive(false);
                        }
                    }
                }
            }
        );
    }

    // 구매 정보를 담는 Charge 클래스 정의
    class Charge
    {
        public double timestamp;
        public Charge(double _timestamp)
        {
            this.timestamp = _timestamp;
        }
    }

    public void Purchase()
    {
        // 데이터베이스에 접근.
        DatabaseReference reference = PlayerInformation.GetDatabaseReference();

        // 삽입할 데이터 준비하기
        DateTime now = DateTime.Now.ToLocalTime();
        TimeSpan span = (now - new DateTime(1970, 1, 1, 0, 0, 0).ToLocalTime());
        int timestamp = (int)span.TotalSeconds;
        Charge charge = new Charge(timestamp);
        string json = JsonUtility.ToJson(charge);

        // 랭킹 점수 데이터 삽입하기
        reference.Child("charges").Child(musicIndex.ToString()).Child(PlayerInformation.auth.CurrentUser.UserId).SetRawJsonValueAsync(json);
        UpdateSong(musicIndex);
    }

    public void Right()
    {
        musicIndex = musicIndex + 1;
        if (musicIndex > musicCount)
            musicIndex = 1;
        UpdateSong(musicIndex);
    }

    public void Left()
    {
        musicIndex = musicIndex - 1;
        if (musicIndex <= 0)
            musicIndex = musicCount;
        UpdateSong(musicIndex);
    }

    void Start()
    {
        userUI.text = PlayerInformation.auth.CurrentUser.Email + "님 환영합니다.";
        musicIndex = 1;
        UpdateSong(musicIndex);        
    }

    public void GameStart()
    {
        if (disable) return;
        PlayerInformation.selectedMusic = musicIndex.ToString();
        SceneManager.LoadScene("GameScene");
    }

    public void Logout()
    {
        PlayerInformation.auth.SignOut();
        SceneManager.LoadScene("LoginScene");
    }

}
