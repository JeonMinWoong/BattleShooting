using Firebase;
using Firebase.Auth;
using Firebase.Extensions;
using Firebase.Database;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class AuthManager : MonoBehaviour
{
    public static AuthManager instance
    {
        get
        {
            // 만약 싱글톤 변수에 아직 오브젝트가 할당되지 않았다면
            if (m_instance == null)
            {
                // 씬에서 GameManager 오브젝트를 찾아 할당
                m_instance = FindObjectOfType<AuthManager>();
            }

            // 싱글톤 오브젝트를 반환
            return m_instance;
        }
    }

    private static AuthManager m_instance;

    public class PlayerInpo
    {
        // 순위 정보를 담는 Rank 클래스
        // Firebase와 동일하게 name, score, timestamp를 가지게 해야함
        public string email = "";
        public string nickName = "";
        public int winCount = 0;
        public int killCount = 0;
        public int ingCount = 0;
        public int losCount = 0;
        public int gold = 0;
        public int medal = 0;
        public int level = 0;
        public float currentExp = 0;
        public float maxExp = 0;
        public int skill = 0;
        public int playerSpPoint;
        public int spCount1_1;
        public int spCount1_2;
        public int spCount1_3;
        public int spCount1_4;
        public int spCount1_5;
        public int spCount1_6;
        public int spCount2_1;
        public int spCount2_2;
        public int spCount2_3;
        public int spCount2_4;
        public int spCount2_5;
        public int spCount2_6;
        public int spCount3_1;
        public int spCount3_2;
        public int spCount3_3;
        public int spCount3_4;
        public int spCount3_5;
        public int spCount3_6;
        public int sP;
        public bool training;
        // JSON 형태로 바꿀 때, 프로퍼티는 지원이 안됨. 프로퍼티로 X

        public PlayerInpo(string _email, string _nickName, int _win, int _kill, int _ing, int _los, int _gold, int _medal, int _level, float _currentExp, float maxExp, int skill, int playerSpPoint,
            int spCount1_1, int spCount1_2, int spCount1_3, int spCount1_4, int spCount1_5, int spCount1_6, int spCount2_1, int spCount2_2, int spCount2_3, int spCount2_4, int spCount2_5,
            int spCount2_6, int spCount3_1, int spCount3_2, int spCount3_3, int spCount3_4, int spCount3_5, int spCount3_6, int sP, bool training)
        {
            // 초기화하기 쉽게 생성자 사용
            this.email = _email;
            this.nickName = _nickName;
            this.winCount = _win;
            this.killCount = _kill;
            this.ingCount = _ing;
            this.losCount = _los;
            this.gold = _gold;
            this.medal = _medal;
            this.level = _level;
            this.currentExp = _currentExp;
            this.maxExp = maxExp;
            this.skill = skill;
            this.playerSpPoint = playerSpPoint;
            this.spCount1_1 = spCount1_1;
            this.spCount1_2 = spCount1_2;
            this.spCount1_3 = spCount1_3;
            this.spCount1_4 = spCount1_4;
            this.spCount1_5 = spCount1_5;
            this.spCount1_6 = spCount1_6;
            this.spCount2_1 = spCount2_1;
            this.spCount2_2 = spCount2_2;
            this.spCount2_3 = spCount2_3;
            this.spCount2_4 = spCount2_4;
            this.spCount2_5 = spCount2_5;
            this.spCount2_6 = spCount2_6;
            this.spCount3_1 = spCount3_1;
            this.spCount3_2 = spCount3_2;
            this.spCount3_3 = spCount3_3;
            this.spCount3_4 = spCount3_4;
            this.spCount3_5 = spCount3_5;
            this.spCount3_6 = spCount3_6;
            this.sP = sP;
            this.training = training;
        }

        public Dictionary<string, object> ToDictionary()
        {
            Dictionary<string, object> dic = new Dictionary<string, object>();
            dic["email"] = this.email;
            dic["nickName"] = this.nickName;
            dic["winCount"] = this.winCount;
            dic["killCount"] = this.killCount;
            dic["ingCount"] = this.ingCount;
            dic["losCount"] = this.losCount;
            dic["gold"] = this.gold;
            dic["medal"] = this.medal;
            dic["level"] = this.level;
            dic["currentExp"] = this.currentExp;
            dic["maxExp"] = this.maxExp;
            dic["skill"] = this.skill;
            dic["playerSpPoint"] = this.playerSpPoint;
            dic["spCount1_1"] = this.spCount1_1;
            dic["spCount1_2"] = this.spCount1_2;
            dic["spCount1_3"] = this.spCount1_3;
            dic["spCount1_4"] = this.spCount1_4;
            dic["spCount1_5"] = this.spCount1_5;
            dic["spCount1_6"] = this.spCount1_6;
            dic["spCount2_1"] = this.spCount2_1;
            dic["spCount2_2"] = this.spCount2_2;
            dic["spCount2_3"] = this.spCount2_3;
            dic["spCount2_4"] = this.spCount2_4;
            dic["spCount2_5"] = this.spCount2_5;
            dic["spCount2_6"] = this.spCount2_6;
            dic["spCount3_1"] = this.spCount3_1;
            dic["spCount3_2"] = this.spCount3_2;
            dic["spCount3_3"] = this.spCount3_3;
            dic["spCount3_4"] = this.spCount3_4;
            dic["spCount3_5"] = this.spCount3_5;
            dic["spCount3_6"] = this.spCount3_6;
            dic["sP"] = this.sP;
            dic["training"] = this.training;
            return dic;
        }

    }

    public bool IsFirebaseReady { get; private set; }
    public bool IsSignInOnProgress { get; private set; }

    public InputField emailField;
    public InputField passwordField;
    public Button signInButton;
    public GameObject errorText;

    public InputField emailInput;
    public InputField passInput;
    public InputField nickNameInput;
    public Text resultText;
    public GameObject signUpWindow;
    public GameObject signUpText;

    public static FirebaseApp firebaseApp;
    public static FirebaseAuth firebaseAuth;

    public FirebaseUser User;
    public string usernickName;
    public int userWin;
    public int userKill;
    public int userIng;
    public int userLos;
    public int userGold;
    public int userMedal;
    public int userLevel;
    public float userCurrentExp;
    public float userMaxExp;
    public int userSkill;
    public int userPlayerSpPoint;
    public int userSpCount1_1;
    public int userSpCount1_2;
    public int userSpCount1_3;
    public int userSpCount1_4;
    public int userSpCount1_5;
    public int userSpCount1_6;
    public int userSpCount2_1;
    public int userSpCount2_2;
    public int userSpCount2_3;
    public int userSpCount2_4;
    public int userSpCount2_5;
    public int userSpCount2_6;
    public int userSpCount3_1;
    public int userSpCount3_2;
    public int userSpCount3_3;
    public int userSpCount3_4;
    public int userSpCount3_5;
    public int userSpCount3_6;
    public int userSp;
    public bool userTraining;




    public DatabaseReference reference = null;

    public void Awake()
    {
        DontDestroyOnLoad(gameObject);

        FirebaseApp.DefaultInstance.Options.DatabaseUrl =
                    new System.Uri("https://battleshooting.firebaseio.com/");

        // 파이어베이스의 메인 참조 얻기
        reference = FirebaseDatabase.DefaultInstance.RootReference;

        Screen.SetResolution(1920, 1080, true);
    }

    public void Start()
    {
        
        if(SceneManager.GetActiveScene().name == "Start")
            signInButton.interactable = false;

        FirebaseApp.CheckAndFixDependenciesAsync().ContinueWithOnMainThread(task =>
        {
            var result = task.Result;

            if (result != DependencyStatus.Available)
            {
                Debug.LogError(result.ToString());
                IsFirebaseReady = false;
            }
            else
            {
                IsFirebaseReady = true;

                firebaseApp = FirebaseApp.DefaultInstance;
                firebaseAuth = FirebaseAuth.DefaultInstance;
            }

            signInButton.interactable = IsFirebaseReady;
        });
    }

    public void SignIn()
    {
        if (!IsFirebaseReady || IsSignInOnProgress) return;

        IsSignInOnProgress = true;
        signInButton.interactable = false;

        firebaseAuth.SignInWithEmailAndPasswordAsync(emailField.text, passwordField.text).ContinueWithOnMainThread(
            task =>
            {
                Debug.Log($"Sign in status : {task.Status}");

                IsSignInOnProgress = false;
                signInButton.interactable = true;

                if (task.IsFaulted)
                {
                    errorText.SetActive(true);
                }
                else if (task.IsCanceled)
                {
                    Debug.LogError("It's canceled");
                }
                else
                {
                    User = task.Result;
                    ReadUserInfos("User");
                    Invoke("SceneMove", 1);
                }
            });
    }

    public void SceneMove()
    {
        SceneManager.LoadScene("Lobby");
    }

    public void SignCreate()
    {
        // 회원가입 버튼은 인풋 필드가 비어있지 않을 때 작동한다.
        if (emailInput.text.Length != 0 && passInput.text.Length != 0 && nickNameInput.text.Length != 0)
        {
            firebaseAuth.CreateUserWithEmailAndPasswordAsync(emailInput.text, passInput.text).ContinueWithOnMainThread(
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
                        User = task.Result;
                        //CreateUserWithPath(new PlayerInpo(emailInput.text, nickNameInput.text)) ;
                        CreateUserWithJson(new PlayerInpo(emailInput.text, nickNameInput.text,0,0,0,0,0,0,1,0,100,1,1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,false));
                        signUpWindow.SetActive(false);
                        signUpText.SetActive(true);

                    }
                });
        }
        else
            resultText.text = "Sign up is failed.";
    }

    public void CreateUserWithJson(PlayerInpo playerInpo)
    {
        string json = JsonUtility.ToJson(playerInpo);
        reference.Child("User").Child(User.UserId).SetRawJsonValueAsync(json);
    }

    public void CreateUserWithPath(PlayerInpo _userInfo)
    {
        //해야됨
        reference.Child("users").Child("Email").SetValueAsync(_userInfo.email);
        reference.Child("users").Child("nickname").SetValueAsync(_userInfo.nickName);
    }

    public void ReadUserInfos(string _dataSet)
    {
        // 특정 데이터셋의 DB 참조 얻기
        DatabaseReference uiReference = FirebaseDatabase.DefaultInstance.GetReference(_dataSet);

        uiReference.GetValueAsync().ContinueWith(
            task =>
            {
                if (task.IsCompleted)
                {
                    DataSnapshot snapshot = task.Result;

                    foreach (DataSnapshot data in snapshot.Children)
                    {
                        // JSON 자체가 딕셔너리 기반
                        if (data.Key == User.UserId)
                        {
                            IDictionary playerInpo = (IDictionary)data.Value;
                            Debug.Log("Email: " + playerInpo["email"] + " / NickName: " + playerInpo["nickName"] + "Win " + playerInpo["winCount"]);
                            usernickName = playerInpo["nickName"].ToString();
                            userWin = int.Parse(playerInpo["winCount"].ToString());
                            userKill = int.Parse(playerInpo["killCount"].ToString());
                            userIng = int.Parse(playerInpo["ingCount"].ToString());
                            userLos = int.Parse(playerInpo["losCount"].ToString());
                            userGold = int.Parse(playerInpo["gold"].ToString());
                            userMedal = int.Parse(playerInpo["medal"].ToString());
                            userLevel = int.Parse(playerInpo["level"].ToString());
                            userCurrentExp = int.Parse(playerInpo["currentExp"].ToString());
                            userMaxExp = int.Parse(playerInpo["maxExp"].ToString());
                            userSkill = int.Parse(playerInpo["skill"].ToString());
                            userPlayerSpPoint = int.Parse(playerInpo["playerSpPoint"].ToString());
                            userSpCount1_1 = int.Parse(playerInpo["spCount1_1"].ToString());
                            userSpCount1_2 = int.Parse(playerInpo["spCount1_2"].ToString());
                            userSpCount1_3 = int.Parse(playerInpo["spCount1_3"].ToString());
                            userSpCount1_4 = int.Parse(playerInpo["spCount1_4"].ToString());
                            userSpCount1_5 = int.Parse(playerInpo["spCount1_5"].ToString());
                            userSpCount1_6 = int.Parse(playerInpo["spCount1_6"].ToString());
                            userSpCount2_1 = int.Parse(playerInpo["spCount2_1"].ToString());
                            userSpCount2_2 = int.Parse(playerInpo["spCount2_2"].ToString());
                            userSpCount2_3 = int.Parse(playerInpo["spCount2_3"].ToString());
                            userSpCount2_4 = int.Parse(playerInpo["spCount2_4"].ToString());
                            userSpCount2_5 = int.Parse(playerInpo["spCount2_5"].ToString());
                            userSpCount2_6 = int.Parse(playerInpo["spCount2_6"].ToString());
                            userSpCount3_1 = int.Parse(playerInpo["spCount3_1"].ToString());
                            userSpCount3_2 = int.Parse(playerInpo["spCount3_2"].ToString());
                            userSpCount3_3 = int.Parse(playerInpo["spCount3_3"].ToString());
                            userSpCount3_4 = int.Parse(playerInpo["spCount3_4"].ToString());
                            userSpCount3_5 = int.Parse(playerInpo["spCount3_5"].ToString());
                            userSpCount3_6 = int.Parse(playerInpo["spCount3_6"].ToString());
                            userSp = int.Parse(playerInpo["sP"].ToString());
                            userTraining = bool.Parse(playerInpo["training"].ToString());
                            break;
                        }
                    }
                }
            });
    }

    public void OnClickUpdateChildren()
    {
        Debug.Log("1");
        DatabaseReference mDatabaseRef = FirebaseDatabase.DefaultInstance.RootReference;
        string userId = User.UserId.ToString();

        Dictionary<string, object> childUpdates = new Dictionary<string, object>();
        childUpdates["/User/" + userId + "/" + "winCount"] = userWin;
        childUpdates["/User/" + userId + "/" + "killCount"] = userKill;
        childUpdates["/User/" + userId + "/" + "ingCount"] = userIng;
        childUpdates["/User/" + userId + "/" + "losCount"] = userLos;
        childUpdates["/User/" + userId + "/" + "gold"] = userGold;
        childUpdates["/User/" + userId + "/" + "medal"] = userMedal;
        childUpdates["/User/" + userId + "/" + "level"] = userLevel;
        childUpdates["/User/" + userId + "/" + "currentExp"] = userCurrentExp;
        childUpdates["/User/" + userId + "/" + "maxExp"] = userMaxExp;
        childUpdates["/User/" + userId + "/" + "skill"] = userSkill;
        childUpdates["/User/" + userId + "/" + "playerSpPoint"] = userPlayerSpPoint;
        childUpdates["/User/" + userId + "/" + "spCount1_1"] = userSpCount1_1;
        childUpdates["/User/" + userId + "/" + "spCount1_2"] = userSpCount1_2;
        childUpdates["/User/" + userId + "/" + "spCount1_3"] = userSpCount1_3;
        childUpdates["/User/" + userId + "/" + "spCount1_4"] = userSpCount1_4;
        childUpdates["/User/" + userId + "/" + "spCount1_5"] = userSpCount1_5;
        childUpdates["/User/" + userId + "/" + "spCount1_6"] = userSpCount1_6;
        childUpdates["/User/" + userId + "/" + "spCount2_1"] = userSpCount2_1;
        childUpdates["/User/" + userId + "/" + "spCount2_2"] = userSpCount2_2;
        childUpdates["/User/" + userId + "/" + "spCount2_3"] = userSpCount2_3;
        childUpdates["/User/" + userId + "/" + "spCount2_4"] = userSpCount2_4;
        childUpdates["/User/" + userId + "/" + "spCount2_5"] = userSpCount2_5;
        childUpdates["/User/" + userId + "/" + "spCount2_6"] = userSpCount2_6;
        childUpdates["/User/" + userId + "/" + "spCount3_1"] = userSpCount3_1;
        childUpdates["/User/" + userId + "/" + "spCount3_2"] = userSpCount3_2;
        childUpdates["/User/" + userId + "/" + "spCount3_3"] = userSpCount3_3;
        childUpdates["/User/" + userId + "/" + "spCount3_4"] = userSpCount3_4;
        childUpdates["/User/" + userId + "/" + "spCount3_5"] = userSpCount3_5;
        childUpdates["/User/" + userId + "/" + "spCount3_6"] = userSpCount3_6;
        childUpdates["/User/" + userId + "/" + "sP"] = userSp;
        childUpdates["/User/" + userId + "/" + "training"] = userTraining;



        mDatabaseRef.UpdateChildrenAsync(childUpdates).ContinueWith(
            task =>
            {
                Debug.Log(string.Format("OnClickUpdateChildren::IsCompleted:{0} IsCanceled:{1} IsFaulted:{2}", task.IsCompleted, task.IsCanceled, task.IsFaulted));
            }
        );
    }
}