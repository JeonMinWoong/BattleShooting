using Firebase;
using Firebase.Auth;
using Firebase.Extensions;
using Firebase.Database;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class DatabassManager : MonoBehaviour
{
    public static DatabassManager instance
    {
        get
        {
            // 만약 싱글톤 변수에 아직 오브젝트가 할당되지 않았다면
            if (m_instance == null)
            {
                // 씬에서 GameManager 오브젝트를 찾아 할당
                m_instance = FindObjectOfType<DatabassManager>();
            }

            // 싱글톤 오브젝트를 반환
            return m_instance;
        }
    }

    private static DatabassManager m_instance;

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
        // JSON 형태로 바꿀 때, 프로퍼티는 지원이 안됨. 프로퍼티로 X

        public PlayerInpo(string _email, string _nickName, int _win, int _kill, int _ing, int _los, int _gold, int _medal)
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
            return dic;
        }

    }

    public bool IsFirebaseReady { get; private set; }

    public static FirebaseApp firebaseApp;

    public static FirebaseUser User;
    public static string usernickName;
    public static int userWin;
    public static int userKill;
    public static int userIng;
    public static int userLos;
    public static int userGold;
    public static int userMedal;

    public DatabaseReference reference = null;

    public void Start()
    {
        User = AuthManager.instance.User;

        FirebaseApp.DefaultInstance.Options.DatabaseUrl =
                    new System.Uri("https://battleshooting.firebaseio.com/");

        // 파이어베이스의 메인 참조 얻기
        reference = FirebaseDatabase.DefaultInstance.RootReference;
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
                            Debug.Log("Email: " + playerInpo["email"] + " / NickName: " + playerInpo["nickName"]);
                            usernickName = playerInpo["nickName"].ToString();
                            userWin = (int)playerInpo["winCount"];
                            userKill = (int)playerInpo["killCount"];
                            userIng = (int)playerInpo["ingCount"];
                            userLos = (int)playerInpo["losCount"];
                            userGold = (int)playerInpo["gold"];
                            userMedal = (int)playerInpo["medal"];
                        }
                    }
                }
            });
    }

    public void OnClickUpdateChildren()
    {
        DatabaseReference mDatabaseRef = FirebaseDatabase.DefaultInstance.RootReference;
        string userId = User.UserId.ToString();

        Dictionary<string, object> childUpdates = new Dictionary<string, object>();
        childUpdates["/User/" + userId + "/" + "winCount"] = userWin;
        childUpdates["/User/" + userId + "/" + "killCount"] = userKill;
        childUpdates["/User/" + userId + "/" + "ingCount"] = userIng;
        childUpdates["/User/" + userId + "/" + "losCount"] = userLos;
        childUpdates["/User/" + userId + "/" + "gold"] = userGold;
        childUpdates["/User/" + userId + "/" + "medal"] = userMedal;

        mDatabaseRef.UpdateChildrenAsync(childUpdates).ContinueWith(
            task =>
            {
                Debug.Log(string.Format("OnClickUpdateChildren::IsCompleted:{0} IsCanceled:{1} IsFaulted:{2}", task.IsCompleted, task.IsCanceled, task.IsFaulted));
            }
        );
    }
}
