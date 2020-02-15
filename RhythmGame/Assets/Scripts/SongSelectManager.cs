using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.IO;

public class SongSelectManager : MonoBehaviour
{
    public Image musicImageUI;
    public Text musicTitleUI;
    public Text bpmUI;

    private int musicIndex;
    private int musicCount = 3;

    // 회원정보 UI
    public Text userUI;

    private void UpdateSong(int musicIndex)
    {
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
        PlayerInformation.selectedMusic = musicIndex.ToString();
        SceneManager.LoadScene("GameScene");
    }

    public void Logout()
    {
        PlayerInformation.auth.SignOut();
        SceneManager.LoadScene("LoginScene");
    }

}
