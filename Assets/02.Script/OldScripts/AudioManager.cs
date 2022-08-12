using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public class AudioManager : Singleton<AudioManager>
{
    public AudioSource audioPrefab;     //AudioSource만 붙어있는 빈 게임오브젝트 프리팝
    public AudioClip[] clips;           //모든 오디오클립들 가지고 있음
    public AudioSource audioSource;
    [SerializeField]
    string sceneName;

    private void Start()
    {
        DontDestroyOnLoad(this);
        audioSource = GetComponent<AudioSource>();
    }

    private void Update()
    {
        if (sceneName != SceneManager.GetActiveScene().name)
        {
            sceneName = SceneManager.GetActiveScene().name;
            BGMCheck();
        }
    }

    void BGMCheck()
    {
        audioSource.clip = Resources.Load<AudioClip>("BGM/" + sceneName);
        audioSource.Play();
    }

    //오디오클립을 재생하는 함수
    public void PlaySound(AudioClip clip, Vector3 pos, float pitch = 1f)
    {
        //AudioSource 프리팝을 생성, 값을 전해줌
        AudioSource audio = Instantiate(audioPrefab, transform.position, Quaternion.identity,this.transform);

        audio.transform.position = pos;

        audio.clip = clip;
        audio.pitch = pitch;
        audio.Play();

        Destroy(audio.gameObject, clip.length);     //오디오 재생 이후 게임오브젝트를 삭제해라

    }

    //이름으로 오디오클립 찾아서 재생
    public void PlaySound(string clipName, Vector3 pos, float pitch = 1f)
    {

        foreach (AudioClip ac in clips)      //clips를 순환하며
            if (ac.name == clipName)         //clipName과 같은 clip이 있으면
                PlaySound(ac, pos, pitch);         //그 클립을 재생하라..고 넘겨줌

    }

    //이 중 랜덤한 사운드를 골라서 재생
    public void PlaySound(string[] clipNames, Vector3 pos, float pitch = 1f)
    {
        int r = Random.Range(0, clipNames.Length);

        PlaySound(clipNames[r], pos, pitch);

    }

}
