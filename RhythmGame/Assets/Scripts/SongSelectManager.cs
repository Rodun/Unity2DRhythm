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
using UnityEngine.Purchasing;
using UnityEngine.Purchasing.Security;
using UnityEngine.Analytics;

public class SongSelectManager : MonoBehaviour, IStoreListener
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

    // 인 앱 결제 관련 변수
    private string productID = "music_3";
    private IStoreController controller; // 인 앤 결제를 위한 컨트롤러 객체입니다.

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
        TextAsset textAsset = textAssets[musicIndex - 1];
        StringReader stringReader = new StringReader(textAsset.text);
                
        musicTitleUI.text = stringReader.ReadLine(); // 첫 번째 줄에 적힌 곡 이름을 읽어서 UI를 업데이트 합니다.
        stringReader.ReadLine(); // 두 번째 줄을 읽기만하고 아무것도 하지 않는다.
        bpmUI.text = "BPM: " + stringReader.ReadLine().Split(' ')[0]; // 세 번째 줄에 첫번째로 적힌 bpm을 읽어온다.

        // 리소스에서 비트(beat) 음악 파일을 불러와 재생합니다.
        AudioClip audioClip = audioClips[musicIndex - 1];
        audioSource.clip = audioClip;
        audioSource.Play();
                
        musicImageUI.sprite = sprites[musicIndex - 1]; // 리소스에서 비트(beat) 이미지 파일을 불러옵니다.

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
        if(controller == null)
        {
            Debug.Log("결제 모듈이 초기화 되지 않았습니다.");
        }
        else
        {
            controller.InitiatePurchase(productID);
        }
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

    Sprite[] sprites;
    AudioClip[] audioClips;
    TextAsset[] textAssets;

    void Start()
    {
        sprites = new Sprite[musicCount];
        audioClips = new AudioClip[musicCount];
        textAssets = new TextAsset[musicCount];

        // 모든 곡의 정보를 미리 읽습니다.
        for(int i = 1; i <=musicCount; ++i)
        {
            sprites[i - 1] = Resources.Load<Sprite>("Beats/" + i.ToString());
            audioClips[i - 1] = Resources.Load<AudioClip>("Beats/" + i.ToString());
            textAssets[i - 1] = Resources.Load<TextAsset>("Beats/" + i.ToString());
        }

        userUI.text = PlayerInformation.auth.CurrentUser.Email + "님 환영합니다.";
        musicIndex = 1;
        UpdateSong(musicIndex);
        InitStore(); // 인 앱 결제 모듈을 초기화합니다.
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
    
    void InitStore()
    {
        // 환경설정 객체를 선언합니다.
        ConfigurationBuilder builder = ConfigurationBuilder.Instance(StandardPurchasingModule.Instance());

        // 설정한 상품 ID를 인 앱 결제 상품으로서 등록합니다.
        builder.AddProduct(productID, ProductType.Consumable, new IDs { { productID, GooglePlay.Name } });
        UnityPurchasing.Initialize(this, builder);
    }

    public void OnInitializeFailed(InitializationFailureReason error)
    {
        Debug.Log("결제 모듈 초기화에 실패했습니다.");
    }

    public PurchaseProcessingResult ProcessPurchase(PurchaseEventArgs e)
    {
        bool success = true;

        // 아래 소스코드는 안드로이드(Android)에서 실행했을 때에만 정상적으로 동작합니다.
        CrossPlatformValidator validator = new CrossPlatformValidator(GooglePlayTangle.Data(),
            AppleTangle.Data(), Application.identifier);

        try
        {
            // 앱 상에서 구매한 물품에 대하여 결제 처리를 진행합니다.
            IPurchaseReceipt[] result = validator.Validate(e.purchasedProduct.receipt);
            for(int i = 0; i < result.Length; i++)
            {
                Analytics.Transaction(productID, e.purchasedProduct.metadata.localizedPrice,
                    e.purchasedProduct.metadata.isoCurrencyCode);
            }
        }        
        catch (IAPSecurityException ex) 
        {
            // 유니티 에디터에서 실행하는 경우 오류가 발생합니다.
            Debug.Log("오류 발생: " + ex.Message);
            success = false;
        }

        if(success)
        {
            Debug.Log("결제 완료");

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
        else
        {
            Debug.Log("결제 실패");
        }

        return PurchaseProcessingResult.Complete;
    }

    public void OnPurchaseFailed(Product i, PurchaseFailureReason p)
    {
        if(!p.Equals(PurchaseFailureReason.UserCancelled))
        {
            Debug.Log("결제 모듈 동작에 실패했습니다.");
        }
        else
        {
            Debug.Log("사용자가 결제를 취소했습니다.");            
        }
    }

    public void OnInitialized(IStoreController controller, IExtensionProvider extensions)
    {
        this.controller = controller;
        Debug.Log("결제 모듈 초기화가 완료되었습니다.");
    }
}
