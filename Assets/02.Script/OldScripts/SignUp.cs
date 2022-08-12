using UnityEngine;
using UnityEngine.UI;

using Firebase;
using Firebase.Database;
using Firebase.Unity.Editor;

public class SignUp : MonoBehaviour
{
    class PlayerInpo
    {
        // 순위 정보를 담는 Rank 클래스
        // Firebase와 동일하게 name, score, timestamp를 가지게 해야함
        public string email;
        public string nickName;
        // JSON 형태로 바꿀 때, 프로퍼티는 지원이 안됨. 프로퍼티로 X

        public PlayerInpo(string email, string nickName)
        {
            // 초기화하기 쉽게 생성자 사용
            this.email = email;
            this.nickName = nickName;
        }
    }

    public DatabaseReference reference { get; set; }
    // 라이브러리를 통해 불러온 FirebaseDatabase 관련객체를 선언해서 사용

    public InputField emailInput;
    public InputField passInput;
    public InputField nickNameInput;
    public Text resultText;

    // Use this for initialization
    void Awake()
    {
        // 인증을 관리할 객체를 초기화 한다.
        auth = Firebase.Auth.FirebaseAuth.DefaultInstance;
    }

    void Start()
    {
        reference = FirebaseDatabase.DefaultInstance.RootReference;
        // 데이터베이스 경로를 설정해 인스턴스를 초기화
        // Database의 특정지점을 가리킬 수 있는데, 그 중 RootReference를 가리킴
    }

    // 인증을 관리할 객체
    Firebase.Auth.FirebaseAuth auth;

    // 회원가입 버튼을 눌렀을 때 작동할 함수
    public void SignCreate()
    {
        // 회원가입 버튼은 인풋 필드가 비어있지 않을 때 작동한다.
        if (emailInput.text.Length != 0 && passInput.text.Length != 0 && nickNameInput.text.Length != 0)
        {
            auth.CreateUserWithEmailAndPasswordAsync(emailInput.text, passInput.text).ContinueWith(
                task =>
                {
                    if (task.IsFaulted)
                    {
                        resultText.text = "Sign up is failed.";
                    }
                    else if (task.IsCanceled)
                    {
                        resultText.text = "Sign up is failed.";
                    }
                    else
                    {
                        writePlayerInpo(emailInput.text,nickNameInput.text);
                        resultText.text = "Sign up is complete.";

                    }
                });
        }
        else
            resultText.text = "Sign up is failed.";
    }

    public void writePlayerInpo(string email, string nickName )
    {
        PlayerInpo playerInpo = new PlayerInpo(email, nickName);
        string json = JsonUtility.ToJson(playerInpo);
        // 데이터를 json형태로 반환

        reference.Child("PlayerInpo").Child(email).SetRawJsonValueAsync(json);
        // 생성된 키의 자식으로 json데이터를 삽입
    }
}