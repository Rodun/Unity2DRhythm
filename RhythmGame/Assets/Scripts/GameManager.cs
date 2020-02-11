using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager instance { get; set; }
    void Awake()
    {        
        if (instance == null)
            instance = this;
        else if (instance != this)
            Destroy(gameObject);
    }
    
    public float noteSpeed;

    public GameObject scoreUI;
    public float score;
    private Text scoreText;

    public GameObject comboUI;
    private int combo;
    private Text comboText;
    private Animator comboAnimator;
    public int maxCombo;

    public enum judges
    {
        NONE =0,
        BAD,
        GOOD,
        PERFECT,
        MISS
    }
    public GameObject judgeUI;
    private Sprite[] judgeSprite;
    private Image judgementSpriteRenderer;
    private Animator judgementSpriteAnimator;

    public GameObject[] trails;
    private SpriteRenderer[] trailSpriteRenderers;

    // Audio member
    private AudioSource audioSource;
    public string music = "1";

    public bool autoPerfect; // 자동 만점 기능

    // Start Music Function
    void MusicStart()
    {
        // 리소스에서 비트(beat) 음악 파일을 불러와 재생합니다.
        AudioClip audioClip = Resources.Load<AudioClip>("Beats/" + music);
        audioSource = GetComponent<AudioSource>();
        audioSource.clip = audioClip;
        audioSource.Play();
    }

    void Start()
    {
        Invoke("MusicStart", 2); // 게임 시작후 2초뒤에 음악이 실행되도록 한다.

        judgementSpriteRenderer = judgeUI.GetComponent<Image>();
        judgementSpriteAnimator = judgeUI.GetComponent<Animator>();
        scoreText = scoreUI.GetComponent<Text>();
        comboText = comboUI.GetComponent<Text>();
        comboAnimator = comboUI.GetComponent<Animator>();

        // 판정 결과를 보여주는 스프라이트 이미지를 미리 초기화합니다.
        judgeSprite = new Sprite[4];
        judgeSprite[0] = Resources.Load<Sprite>("Sprites/Bad");// Resources폴더에서 가져온다.
        judgeSprite[1] = Resources.Load<Sprite>("Sprites/Good");
        judgeSprite[2] = Resources.Load<Sprite>("Sprites/Miss");
        judgeSprite[3] = Resources.Load<Sprite>("Sprites/Perfect");

        trailSpriteRenderers = new SpriteRenderer[trails.Length];
        for(int i = 0; i < trails.Length; ++i)
        {
            trailSpriteRenderers[i] = trails[i].GetComponent<SpriteRenderer>();
        }
    }

    void Update()
    {
        // 사용자가 입력한 키에 해당하는 라인을 빛나게 처리
        if (Input.GetKey(KeyCode.D)) ShineTrail(0);
        if (Input.GetKey(KeyCode.F)) ShineTrail(1);
        if (Input.GetKey(KeyCode.J)) ShineTrail(2);
        if (Input.GetKey(KeyCode.K)) ShineTrail(3);

        // 한 번 빛나게 된 라인은 반복적으로 다시 어둡게 처리
        for(int i = 0; i < trailSpriteRenderers.Length; ++i)
        {
            Color color = trailSpriteRenderers[i].color;
            color.a -= 0.01f;
            trailSpriteRenderers[i].color = color;
        }
    }

    // 해당하는 라인을 빛나게 처리
    public void ShineTrail(int index)
    {
        Color color = trailSpriteRenderers[index].color;
        color.a = 0.32f;
        trailSpriteRenderers[index].color = color;
    }

    // 노트 판정 이후에 판정 결과를 화면에 보여줍니다.
    void showJudgement()
    {
        // 점수 이미지를 보여줍니다.
        string scoreFormat = "000000";
        scoreText.text = score.ToString(scoreFormat);

        // 판정 이미지를 보여줍니다.
        judgementSpriteAnimator.SetTrigger("Show");

        // 콤보가 2 이상일 때만 콤보 이미지를 보여줍니다.
        if(combo >= 2)
        {
            comboText.text = "COMBO" + combo.ToString();
            comboAnimator.SetTrigger("Show");
        }
        if(maxCombo < combo)
        {
            maxCombo = combo;
        }
    }

    // 노트 판정을 진행합니다.
    public void processJudge(judges judge, int noteType)
    {
        if (judge == judges.NONE)
            return;

        // Miss 판정은 콤보를 종료하고, 점수를 많이 깍습니다.
        if(judge == judges.MISS)
        {
            judgementSpriteRenderer.sprite = judgeSprite[2];
            combo = 0;
            if (score >= 15)
                score -= 15;
        }
        // Bad 판정은 콤보를 종료하고, 점수를 조금 깍습니다.
        else if (judge == judges.BAD)
        {
            judgementSpriteRenderer.sprite = judgeSprite[0];
            combo = 0;
            if (score >= 5)
                score -= 5;
        }
        // Perfect, Good 판정은 콤보 및 점수를 증가
        else
        {
            if (judge == judges.PERFECT)
            {
                judgementSpriteRenderer.sprite = judgeSprite[3];
                score += 20;                
            }
            else if(judge == judges.GOOD)
            {
                judgementSpriteRenderer.sprite = judgeSprite[1];
                score += 15;
            }
            combo += 1;
            score += (float)combo * 0.1f;
        }
        showJudgement();
    }
}
