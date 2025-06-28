using System;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Firebase;
using Firebase.Auth;
using Firebase.Extensions;
using TMPro;
using UnityEngine.Events;

public class LoginManager : MonoBehaviour
{
    [Header("Panels")]
    [SerializeField] private GameObject loginPanel;
    [SerializeField] private GameObject registerPanel;
    [SerializeField] private GameObject messegePanel;

    [Header("Login UI")]
    [SerializeField] private TMP_InputField loginEmail;
    [SerializeField] private TMP_InputField loginPassword;
    [SerializeField] private Button loginButton;
    [SerializeField] private Button signUpButtonL;

    [Header("Register UI")]
    [SerializeField] private TMP_InputField regEmail;
    [SerializeField] private TMP_InputField regPassword;
    [SerializeField] private TMP_InputField regConfirmPassword;
    [SerializeField] private Button signUpButtonS;
    [SerializeField] private Button BackButton;
    
    [Header("Messege UI")]
    [SerializeField] private TMP_Text messegeStatus;
    [SerializeField] private Button confirmButton;

    private FirebaseAuth auth;
    private Action confirm;

    private void Awake()
    {
        loginButton.onClick.AddListener(OnLoginClicked);
        signUpButtonS.onClick.AddListener(OnRegisterClicked);
        signUpButtonL.onClick.AddListener(ShowRegister);
        BackButton.onClick.AddListener(ShowLogin);
    }

    private void Start()
    {
        ShowLogin();//기본적으로 처음엔 로그인창 팝업
        FirebaseApp.CheckAndFixDependenciesAsync().ContinueWithOnMainThread(task =>
        {
            if (task.Result == DependencyStatus.Available)
            {
                auth = FirebaseAuth.DefaultInstance;
            }
            else
            {
                Application.Quit();
            }
        });
    }

    public void ShowLogin()//로그인 창 팝업
    {
        loginPanel.SetActive(true);
        registerPanel.SetActive(false);
    }

    public void ShowRegister()//회원가입 창 팝업
    {
        loginPanel.SetActive(false);
        registerPanel.SetActive(true);
    }

    public void Showmessege(string status)
    {
        messegePanel.SetActive(true);
        messegeStatus.text = status;
    }

    public void ChandConfirm(UnityAction confirm)
    {
        confirmButton.onClick.RemoveAllListeners();
        confirmButton.onClick.AddListener(confirm);
    }
    
    

    public void OffMessege()
    {
        messegePanel.SetActive(false);
    }

    public async void GameStart()
    {
        SceneManager.LoadScene(1);
    }
    
    

    private void OnLoginClicked()//로그인 클릭
    {
        string email = loginEmail.text.Trim();
        string password = loginPassword.text;

        if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
        {
            Showmessege("이메일과 비밀번호를 모두 입력하세요.");
            ChandConfirm(OffMessege);
            return;
        }
        
        Showmessege("로그인 중...");
        confirmButton.interactable = false;

        auth.SignInWithEmailAndPasswordAsync(email, password).ContinueWithOnMainThread(task =>
        {
            if (task.IsCanceled || task.IsFaulted)
            {
                string error = task.Exception?.GetBaseException().Message ?? "알 수 없는 오류";
                messegeStatus.text = $"로그인 실패: {error}";
                ChandConfirm(OffMessege);
                confirmButton.interactable = true;
            }
            else
            {
                messegeStatus.text = $"환영합니다, {auth.CurrentUser.Email}";
                confirmButton.interactable = true;
                ChandConfirm(GameStart);
            }
        });
    }

    private void OnRegisterClicked()//회원가입 클릭
    {
        string email = regEmail.text.Trim();
        string password = regPassword.text;
        string confirmPassword = regConfirmPassword.text;

        if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
        {
            Showmessege("이메일과 비밀번호를 모두 입력하세요.");
            ChandConfirm(OffMessege);
            return;
        }

        if (password != confirmPassword)
        {
            Showmessege("비밀번호가 일치하지 않습니다.");
            ChandConfirm(OffMessege);
            return;
        }

        confirmButton.interactable = false;
        Showmessege("회원가입 중...");

        auth.CreateUserWithEmailAndPasswordAsync(email, password)
            .ContinueWithOnMainThread(task =>
        {
            if (task.IsCanceled || task.IsFaulted)
            {
                string error = task.Exception?.GetBaseException().Message ?? "알 수 없는 오류";
                messegeStatus.text = $"가입 실패: {error}";
                ChandConfirm(OffMessege);
                confirmButton.interactable = true;
            }
            else
            {
                messegeStatus.text = $"환영합니다, {auth.CurrentUser.Email}";
                ChandConfirm(GameStart);
                confirmButton.interactable = true;
            }
        });
    }
}
