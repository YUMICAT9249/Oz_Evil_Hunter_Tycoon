using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;

/// <summary>
///  AudioManager_KJG
/// - BGM 자동 전환 (Bootstrap → Intro BGM / Ingame_Scene → Main BGM)
/// - SFX 풀링
/// - SaveLoad 안전 처리
/// </summary>
public class AudioManager_KJG : BaseManager_KJG<AudioManager_KJG>
{
    [Header("Audio Mixer (필수)")]
    [SerializeField] private AudioMixer audioMixer;

    [Header("BGM 전용 AudioSource")]
    [SerializeField] private AudioSource bgmSource;

    [Header("SFX 풀링 설정")]
    [SerializeField] private int sfxPoolSize = 20;

    [Header("기본 볼륨")]
    [Range(0f, 1f)] public float masterVolume = 1f;
    [Range(0f, 1f)] public float bgmVolume = 0.85f;
    [Range(0f, 1f)] public float sfxVolume = 1f;

    private List<AudioSource> sfxPool = new List<AudioSource>();
    private Queue<AudioSource> availableSFX = new Queue<AudioSource>();
    private Dictionary<string, SoundData_KJG> soundDictionary = new Dictionary<string, SoundData_KJG>();

    protected override void Awake()
    {
        base.Awake();

        InitializePool();
        SetupBGMSouce();
        LoadAllSoundData();
        LoadSavedVolumesSafe();

        // 씬 전환 시 BGM 자동 변경
        SceneManager.sceneLoaded += OnSceneLoaded;

        Debug.Log("✅ [AudioManager_KJG] 오디오 시스템 완전 초기화 완료");
    }

    protected override void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    /// <summary>
    /// 씬이 바뀔 때 BGM 자동 전환
    /// </summary>
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == "Ingame_Scene")
        {
            PlayBGM("BGM_InGame");
        }
        else if (scene.name.Contains("Bootstrap") || scene.name.Contains("Title") || scene.name.Contains("Intro"))
        {
            PlayBGM("BGM_Intro");
        }
    }

    private void SetupBGMSouce()
    {
        if (bgmSource != null)
        {
            bgmSource.loop = true;
            bgmSource.playOnAwake = false;
            Debug.Log("[AudioManager] 기존 BGM_Source 사용");
            return;
        }

        // Inspector에 연결 안 되어 있으면 자동 생성
        GameObject bgmObj = new GameObject("BGM_Source (Auto)");
        bgmObj.transform.SetParent(transform);
        bgmSource = bgmObj.AddComponent<AudioSource>();
        bgmSource.loop = true;
        bgmSource.playOnAwake = false;
        Debug.Log("[AudioManager] BGM_Source 자동 생성");
    }

    private void InitializePool()
    {
        for (int i = 0; i < sfxPoolSize; i++)
        {
            GameObject go = new GameObject($"SFX_{i}");
            go.transform.SetParent(transform);
            AudioSource source = go.AddComponent<AudioSource>();
            sfxPool.Add(source);
            availableSFX.Enqueue(source);
        }
    }

    private void LoadAllSoundData()
    {
        soundDictionary.Clear();
        SoundData_KJG[] datas = Resources.LoadAll<SoundData_KJG>("Sounds");
        foreach (var data in datas)
        {
            if (!string.IsNullOrEmpty(data.soundId))
                soundDictionary[data.soundId] = data;
        }
        Debug.Log($"[AudioManager] {soundDictionary.Count}개 사운드 데이터 로드 완료");
    }

    private void LoadSavedVolumesSafe()
    {
        if (audioMixer == null)
        {
            Debug.LogWarning("[AudioManager_KJG] audioMixer가 Inspector에 할당되지 않았습니다.");
            return;
        }

        if (Manager_KJG.SaveLoad != null)
        {
            Debug.Log("[AudioManager] SaveLoad에서 볼륨 로드 준비됨");
        }
        else
        {
            Debug.LogWarning("[AudioManager] SaveLoadManager가 아직 등록되지 않았습니다.");
        }

        ApplyVolumes();
    }

    private void ApplyVolumes()
    {
        if (audioMixer == null) return;

        audioMixer.SetFloat("MasterVolume", Mathf.Log10(masterVolume) * 20);
        audioMixer.SetFloat("BGMVolume", Mathf.Log10(bgmVolume) * 20);
        audioMixer.SetFloat("SFXVolume", Mathf.Log10(sfxVolume) * 20);
    }

    // ====================== Public API ======================
    public void PlaySFX(string soundId)
    {
        if (!soundDictionary.TryGetValue(soundId, out SoundData_KJG data))
        {
            Debug.LogWarning($"[AudioManager] 사운드 ID를 찾을 수 없음: {soundId}");
            return;
        }

        if (availableSFX.Count == 0)
        {
            Debug.LogWarning("[AudioManager] SFX 풀이 부족합니다.");
            return;
        }

        AudioSource source = availableSFX.Dequeue();
        source.clip = data.GetClip();
        source.volume = data.GetVolume();
        source.pitch = data.GetPitch();
        source.outputAudioMixerGroup = data.mixerGroup;
        source.loop = false;
        source.Play();

        StartCoroutine(ReturnToPool(source, source.clip.length));
    }

    private IEnumerator ReturnToPool(AudioSource source, float delay)
    {
        yield return new WaitForSeconds(delay + 0.1f);
        if (source != null)
        {
            source.Stop();
            availableSFX.Enqueue(source);
        }
    }

    public void PlayBGM(string soundId)
    {
        if (!soundDictionary.TryGetValue(soundId, out SoundData_KJG data))
        {
            Debug.LogWarning($"[AudioManager] BGM ID를 찾을 수 없음: {soundId}");
            return;
        }

        if (bgmSource == null) return;

        bgmSource.clip = data.GetClip();
        bgmSource.volume = data.GetVolume();
        bgmSource.pitch = data.GetPitch();
        bgmSource.outputAudioMixerGroup = data.mixerGroup;
        bgmSource.Play();
    }

    public void StopBGM()
    {
        if (bgmSource != null)
            bgmSource.Stop();
    }

    public void SetMasterVolume(float volume)
    {
        masterVolume = Mathf.Clamp01(volume);
        ApplyVolumes();
    }

    public void SetBGMVolume(float volume)
    {
        bgmVolume = Mathf.Clamp01(volume);
        ApplyVolumes();
    }

    public void SetSFXVolume(float volume)
    {
        sfxVolume = Mathf.Clamp01(volume);
        ApplyVolumes();
    }
}