using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;
using Firebase;
using Firebase.Database;
using Firebase.Unity.Editor;

public class NoteController : MonoBehaviour
{
    class Note
    {
        public int noteType { get; set; }
        public int order { get; set; }
        public Note(int noteType, int order)
        {
            this.noteType = noteType;
            this.order = order;
        }
    }

    public GameObject[] Notes;

    private ObjectPooler noteObjectPooler;

    private List<Note> notes = new List<Note>();

    private float x, z, StartY = 8.0f;

    void MakeNote(Note note)
    {
        GameObject obj = noteObjectPooler.getObject(note.noteType);
        x = obj.transform.position.x;
        z = obj.transform.position.z;
        obj.transform.position = new Vector3(x, StartY, z);
        obj.GetComponent<NoteBehavior>().Initialize();
        obj.SetActive(true);
    }

    private string musicTitle;
    private string musicArtist;
    private int bpm;
    private int divider;
    private float startingPoint;
    private float beatCount;
    private float beatInterval;

    IEnumerator AwaitMakeNote(Note note)
    {
        int noteType = note.noteType;
        int order = note.order;
        yield return new WaitForSeconds( startingPoint + (order * beatInterval) );
        MakeNote(note);
    }

    void Start()
    {
        noteObjectPooler = gameObject.GetComponent<ObjectPooler>();
        
        //리소스에서 beat 텍스트 파일을 불러옵니다.
        TextAsset textAsset = Resources.Load<TextAsset>("Beats/" + PlayerInformation.selectedMusic);
        StringReader reader = new StringReader(textAsset.text);
                
        musicTitle = reader.ReadLine(); // 첫 번째 줄에 적힌 곡 이름을 읽습니다.
        musicArtist = reader.ReadLine(); // 아티스트 정보 읽기

        // 비트 정보 읽기(BPM, Dvider, StartTime)
        string beatInformation = reader.ReadLine(); 
        bpm = Convert.ToInt32(beatInformation.Split(' ')[0]);
        divider = Convert.ToInt32(beatInformation.Split(' ')[1]);
        startingPoint = (float)Convert.ToDouble(beatInformation.Split(' ')[2]);
                
        beatCount = (float)bpm / divider; // 1초마다 떨어지는 비트 개수
        beatInterval = 1 / beatCount; // 비트가 떨어지는 간격 시간

        // 각 비트들이 떨어지는 위치 및 시간 정보를 읽습니다.
        string line;
        while((line = reader.ReadLine()) != null)
        {
            Note note = new Note
            (
                Convert.ToInt32(line.Split(' ')[0]) + 1, 
                Convert.ToInt32(line.Split(' ')[1])
            );
            notes.Add(note);
        }

        //모든 노트를 정해진 시간에 출발하도록 설정
        for (int i = 0; i < notes.Count; i++)
        {
            StartCoroutine(AwaitMakeNote(notes[i]));
        }

        // 마지막 노트를 기준으로 게임종료 함수를 불러온다.
        StartCoroutine(AwaitGameResult(notes[notes.Count - 1].order));
        
    }

    IEnumerator AwaitGameResult(int order)
    {
        yield return new WaitForSeconds(startingPoint + (order * beatInterval) + 8.0f);
        GameResult();
    }

    void GameResult()
    {
        PlayerInformation.maxCombo = GameManager.instance.maxCombo;
        PlayerInformation.score = GameManager.instance.score;
        PlayerInformation.musicTitle = musicTitle;
        PlayerInformation.musicArtist = musicArtist;
        AddRank();
        SceneManager.LoadScene("GameResultScene");
    }

    // 순위 정보를 담는 Rank 클래스 : 클래스를 Json으로 변환하여 저장
    class Rank
    {
        public string email;
        public int score;
        public double timestamp;

        public Rank(string _email, int _score, double _timestamp)
        {
            this.email = _email;
            this.score = _score;
            this.timestamp = _timestamp;
        }
    }

    void AddRank()
    {
        // 데이터베이스에 접속 설정하기
        DatabaseReference reference;
        FirebaseApp.DefaultInstance.SetEditorDatabaseUrl("https://rhythmgame-tutorial.firebaseio.com/");
        reference = FirebaseDatabase.DefaultInstance.RootReference;

        // 삽입할 데이터 준비
        DateTime now = DateTime.Now.ToLocalTime();
        TimeSpan span = (now - new DateTime(1970, 1, 1, 0, 0, 0).ToLocalTime());
        int timestamp = (int)span.TotalSeconds;
        Rank rank = new Rank(PlayerInformation.auth.CurrentUser.Email, (int)PlayerInformation.score, timestamp);
        string json = JsonUtility.ToJson(rank);

        // 랭킹 점수 데이터 삽입하기
        reference.Child("ranks").Child(PlayerInformation.selectedMusic).Child(PlayerInformation.auth.CurrentUser.UserId).SetRawJsonValueAsync(json);            
    }



    // Update is called once per frame
    void Update()
    {
        
    }
}
