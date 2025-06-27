using UnityEngine;
using UnityEngine.UI; // UI 요소를 사용하기 위해 필요
using TMPro; // TextMeshPro InputField, Text를 사용하는 경우
using Firebase; // Firebase 초기화를 위해 필요
using Firebase.Auth; // Firebase Authentication 기능을 사용하기 위해 필요
using System.Threading.Tasks; // 비동기 작업을 위해 필요

public class Sign : MonoBehaviour
{
    // Firebase 인증 객체
    private FirebaseAuth auth;
    private FirebaseUser user; // 현재 로그인된 사용자 정보

    // UI 요소들을 연결할 변수 (Inspector에서 드래그앤드롭으로 연결)
    [Header("UI References")]
    public TMP_InputField emailInputField;
    public TMP_InputField passwordInputField;
    public Button registerButton;
    public Button loginButton;
    public Button logoutButton; // 로그아웃 버튼 (선택 사항)
    public TMP_Text resultText;
    public TMP_Text userEmailText; // 로그인된 사용자 이메일 표시 (선택 사항)

    void Awake()
    {
        InitializeFirebase();
    }

    void Start()
    {
        // 버튼에 클릭 이벤트 리스너 추가
        registerButton.onClick.AddListener(OnRegisterButtonClicked);
        loginButton.onClick.AddListener(OnLoginButtonClicked);
        logoutButton?.onClick.AddListener(OnLogoutButtonClicked); // 널 체크 후 추가
        
        // 초기 UI 상태 업데이트
        UpdateUIForAuthState(auth.CurrentUser);
    }

    // Firebase 초기화
    private void InitializeFirebase()
    {
        // Firebase SDK의 모든 필수 구성 요소가 있는지 확인하고 없으면 수정
        FirebaseApp.CheckAndFixDependenciesAsync().ContinueWith(task =>
        {
            var dependencyStatus = task.Result;
            if (dependencyStatus == DependencyStatus.Available)
            {
                // FirebaseApp을 초기화합니다.
                auth = FirebaseAuth.DefaultInstance;
                
                // 인증 상태 변경 리스너를 설정합니다.
                // 사용자가 로그인하거나 로그아웃할 때마다 이 이벤트가 발생합니다.
                auth.StateChanged += OnAuthStateChanged;
                OnAuthStateChanged(this, null); // 초기 상태 확인

                resultText.text = "Firebase 초기화 성공";
                Debug.Log("Firebase 초기화 성공!");
            }
            else
            {
                Debug.LogError($"Firebase 초기화 실패: {dependencyStatus}");
                resultText.text = $"Firebase 초기화 실패: {dependencyStatus}";
            }
        });
    }

    // 인증 상태 변경 시 호출되는 콜백 함수
    void OnAuthStateChanged(object sender, System.EventArgs eventArgs)
    {
        if (auth.CurrentUser != user) // 현재 사용자 정보가 변경되었는지 확인
        {
            bool signedIn = auth.CurrentUser != null;
            if (signedIn)
            {
                user = auth.CurrentUser;
                Debug.Log($"사용자 로그인: {user.Email} (UID: {user.UserId})");
                resultText.text = $"로그인 성공: {user.Email}";
            }
            else
            {
                Debug.Log("사용자 로그아웃.");
                resultText.text = "로그아웃됨";
                user = null;
            }
            UpdateUIForAuthState(user);
        }
    }

    // UI 상태 업데이트 (로그인 여부에 따라 버튼 활성화/비활성화 등)
    private void UpdateUIForAuthState(FirebaseUser currentUser)
    {
        bool loggedIn = currentUser != null;

        emailInputField.interactable = !loggedIn;
        passwordInputField.interactable = !loggedIn;
        registerButton.interactable = !loggedIn;
        loginButton.interactable = !loggedIn;
        logoutButton.interactable = loggedIn;

        if (loggedIn)
        {
            userEmailText.text = $"로그인됨: {currentUser.Email}";
        }
        else
        {
            userEmailText.text = "로그인 필요";
        }
    }

    // 회원가입 버튼 클릭 시 호출
    public void OnRegisterButtonClicked()
    {
        string email = emailInputField.text;
        string password = passwordInputField.text;

        if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
        {
            resultText.text = "이메일과 비밀번호를 입력해주세요.";
            return;
        }

        RegisterUser(email, password);
    }

    // 로그인 버튼 클릭 시 호출
    public void OnLoginButtonClicked()
    {
        string email = emailInputField.text;
        string password = passwordInputField.text;

        if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
        {
            resultText.text = "이메일과 비밀번호를 입력해주세요.";
            return;
        }

        LoginUser(email, password);
    }

    // 로그아웃 버튼 클릭 시 호출
    public void OnLogoutButtonClicked()
    {
        if (auth.CurrentUser != null)
        {
            auth.SignOut();
            resultText.text = "로그아웃되었습니다.";
            Debug.Log("User signed out.");
        }
    }

    // 새로운 사용자 회원가입
    private void RegisterUser(string email, string password)
    {
        resultText.text = "회원가입 중...";
        auth.CreateUserWithEmailAndPasswordAsync(email, password).ContinueWith(task =>
        {
            // 메인 스레드에서 UI 업데이트를 위해 TaskScheduler.FromCurrentSynchronizationContext() 사용
            // 또는 Unity의 Coroutine을 사용하여 비동기 결과를 처리할 수 있습니다.
            if (task.IsCanceled)
            {
                Debug.LogError("회원가입이 취소되었습니다.");
                resultText.text = "회원가입 취소";
                return;
            }
            if (task.IsFaulted)
            {
                Debug.LogError($"회원가입 실패: {task.Exception}");
                resultText.text = "회원가입 실패";
                // FirebaseException을 파싱하여 구체적인 에러 메시지를 표시할 수 있습니다.
                FirebaseException firebaseEx = task.Exception.GetBaseException() as FirebaseException;
                AuthError errorCode = (AuthError)firebaseEx.ErrorCode;
                string errorMessage = "회원가입 실패: ";
                switch (errorCode)
                {
                    case AuthError.MissingEmail: errorMessage += "이메일을 입력하세요."; break;
                    case AuthError.InvalidEmail: errorMessage += "유효하지 않은 이메일 형식입니다."; break;
                    case AuthError.WeakPassword: errorMessage += "비밀번호는 6자리 이상이어야 합니다."; break;
                    case AuthError.EmailAlreadyInUse: errorMessage += "이미 등록된 이메일입니다."; break;
                    default: errorMessage += firebaseEx.Message; break;
                }
                resultText.text = errorMessage;
                return;
            }

            // 회원가입 성공
            Firebase.Auth.FirebaseUser newUser = task.Result;
            Debug.LogFormat("사용자 회원가입 성공: {0} (UID: {1})", newUser.Email, newUser.UserId);
            resultText.text = $"회원가입 성공: {newUser.Email}";
            // 로그인 상태는 OnAuthStateChanged 콜백에서 처리됩니다.
        });
    }

    // 기존 사용자 로그인
    private void LoginUser(string email, string password)
    {
        resultText.text = "로그인 중...";
        auth.SignInWithEmailAndPasswordAsync(email, password).ContinueWith(task =>
        {
            if (task.IsCanceled)
            {
                Debug.LogError("로그인이 취소되었습니다.");
                resultText.text = "로그인 취소";
                return;
            }
            if (task.IsFaulted)
            {
                Debug.LogError($"로그인 실패: {task.Exception}");
                resultText.text = "로그인 실패";
                // FirebaseException을 파싱하여 구체적인 에러 메시지를 표시할 수 있습니다.
                FirebaseException firebaseEx = task.Exception.GetBaseException() as FirebaseException;
                AuthError errorCode = (AuthError)firebaseEx.ErrorCode;
                string errorMessage = "로그인 실패: ";
                switch (errorCode)
                {
                    case AuthError.InvalidEmail: errorMessage += "유효하지 않은 이메일입니다."; break;
                    case AuthError.WrongPassword: errorMessage += "잘못된 비밀번호입니다."; break;
                    case AuthError.UserNotFound: errorMessage += "등록되지 않은 사용자입니다."; break;
                    case AuthError.UserDisabled: errorMessage += "비활성화된 계정입니다."; break;
                    default: errorMessage += firebaseEx.Message; break;
                }
                resultText.text = errorMessage;
                return;
            }

            // 로그인 성공
            Firebase.Auth.FirebaseUser loggedInUser = task.Result;
            Debug.LogFormat("사용자 로그인 성공: {0} (UID: {1})", loggedInUser.Email, loggedInUser.UserId);
            resultText.text = $"로그인 성공: {loggedInUser.Email}";
            // 로그인 상태는 OnAuthStateChanged 콜백에서 처리됩니다.
        });
    }

    // 스크립트가 파괴될 때 이벤트 리스너 제거
    void OnDestroy()
    {
        if (auth != null)
        {
            auth.StateChanged -= OnAuthStateChanged;
        }
        registerButton.onClick.RemoveListener(OnRegisterButtonClicked);
        loginButton.onClick.RemoveListener(OnLoginButtonClicked);
        logoutButton?.onClick.RemoveListener(OnLogoutButtonClicked);
    }
}
