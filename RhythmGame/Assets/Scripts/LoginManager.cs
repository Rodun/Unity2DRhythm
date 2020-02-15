using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Firebase.Auth;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class LoginManager : MonoBehaviour
{
    // Firebase인증 기능 객체
    private FirebaseAuth auth;

    // ID, Password Input UI;
    public InputField emailInputField;
    public InputField passwordInputField;

    // Sign in result UI
    public Text messageUI;

    void Start()
    {
        // Firebase Auth Object Initialization
        auth = FirebaseAuth.DefaultInstance;
        messageUI.text = "";
    }

    public void Login()
    {
        string email = emailInputField.text;
        string password = passwordInputField.text;

        auth.SignInWithEmailAndPasswordAsync(email, password).ContinueWith
        (
            task =>
            {
                if(task.IsCompleted && !task.IsCanceled && !task.IsFaulted)
                {
                    Debug.Log("[Success Sign in] ID: " + auth.CurrentUser.UserId);
                    PlayerInformation.auth = auth;
                    SceneManager.LoadScene("SongSelectScene");
                }
                else
                {
                    messageUI.text = "계정을 다시 확인 해주세요.";
                }
            }
        );
    }

    public void GotoJoin()
    {
        SceneManager.LoadScene("JoinScene"); 
    }
}
