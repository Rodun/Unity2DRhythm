using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Firebase.Auth;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class JoinManager : MonoBehaviour
{
    // Firebase인증 기능 객체
    private FirebaseAuth auth;

    // ID, Password Input UI;
    public InputField emailInputField;
    public InputField passwordInputField;

    // Sign up result UI
    public Text messageUI;

    void Start()
    {
        // Firebase Auth Object Initialization
        auth = FirebaseAuth.DefaultInstance;
        messageUI.text = "";
    }

    bool InputCheck()
    {
        string email = emailInputField.text;
        string password = passwordInputField.text;

        if(email.Length < 8)
        {
            messageUI.text = "이메일은 8자리 이상으로 구성되어야 합니다.";
            return false;
        }
        else if (password.Length < 8)
        {
            messageUI.text = "비밀번호는 8자리 이상으로 구성되어야 합니다.";
            return false;
        }

        messageUI.text = "";
        return true;
    }

    public void Check()
    {
        InputCheck();
    }

    public void Join()
    {
        if(!InputCheck())
        {
            return;
        }
        string email = emailInputField.text;
        string password = passwordInputField.text;

        // 인증객체를 이용하여 이메일과 비밀번호로 가입을 수행합니다.
        auth.CreateUserWithEmailAndPasswordAsync(email, password).ContinueWith
        (
            task =>
            {
                if(!task.IsCanceled && !task.IsFaulted)
                {
                    messageUI.text = "Success Sign up";
                    SceneManager.LoadScene("LoginScene");
                }
                else
                {
                    messageUI.text = "이미 사용중이거나 형식이 올바르지 않습니다.";
                }
            }
        );
    }

    public void Back()
    {
        SceneManager.LoadScene("LoginScene");
    }
}
