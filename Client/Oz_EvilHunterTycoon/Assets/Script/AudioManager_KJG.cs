using UnityEngine;
using UnityEngine.Audio;
using System;
using System.Collections;
using System.Collections.Generic;

public class AudioManager_KJG : MonoBehaviour
{
    public static AudioManager_KJG Instance { get; private set; }

    [Header("Audio Mixer")]
    [SerializeField] private AudioMixer mixer;

    [Header("BGM 설정")]
    [SerializeField] private AudioSource bgmSource;           // BGM 전용 AudioSource (Loop용)

    [Header("SFX 풀링 설정")]
    [SerializeField] private int sfxPoolSize = 15;

    [Header("기본 볼륨 설정")]
    [Range(0f, 1f)] public float masterVolume = 1f;
    [Range(0f, 1f)] public float bgmVolume = 0.8f;
    [Range(0f, 1f)] public float sfxVolume = 1f;

    // SFX 풀링
    private List<AudioSource> sfxPool = new List<AudioSource>();
    private Queue<AudioSource> availableSources = new Queue<AudioSource>();

    // SoundData 관리 (soundId → SoundData_KJG)
    private Dictionary<string, SoundData_KJG> soundDataDict = new Dictionary<string, SoundData_KJG>();

    // ==================== C# Events ====================
    public event Action<string> OnPlaySFX;     // SFX 재생 시 발생
    public event Action<string> OnPlayBGM;     // BGM 변경 시 발생

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        InitializePool();
        LoadSounds();
        LoadVolumeSettings();

        Debug.Log("✅ AudioManager_KJG 초기화 완료");
    }

    // ==================== SFX 풀 초기화 ====================
    private void InitializePool()
    {
        for (int i = 0; i < sfxPoolSize; i++)
        {
            GameObject go = new GameObject($"SFX_Source_{i}");
            go.transform.SetParent(transform);

            AudioSource source = go.AddComponent<AudioSource>();
            source.playOnAwake = false;
            source.outputAudioMixerGroup = mixer != null ? mixer.FindMatchingGroups("SFX")[0] : null;

            sfxPool.Add(source);
            availableSources.Enqueue(source);
        }
    }

    // ==================== SoundData 로드 ====================
    private void LoadSounds()
    {
        soundDataDict.Clear();

        // Resources/Audio/SoundData 폴더에서 모든 SoundData_KJG 로드
        SoundData_KJG[] soundDatas = Resources.LoadAll<SoundData_KJG>("Audio/SoundData");

        foreach (var data in soundDatas)
        {
            if (!string.IsNullOrEmpty(data.soundId))
            {
                soundDataDict[data.soundId] = data;
            }
            else
            {
                Debug.LogWarning($"SoundData_KJG '{data.name}'의 soundId가 비어있습니다.");
            }
        }

        Debug.Log($"[AudioManager] {soundDataDict.Count}개의 SoundData_KJG를 로드했습니다.");
    }

    // ==================== 볼륨 설정 ====================
    private void LoadVolumeSettings()
    {
        masterVolume = PlayerPrefs.GetFloat("MasterVolume", 1f);
        bgmVolume = PlayerPrefs.GetFloat("BGMVolume", 0.8f);
        sfxVolume = PlayerPrefs.GetFloat("SFXVolume", 1f);

        ApplyVolume();
    }

    private void ApplyVolume()
    {
        if (mixer != null)
        {
            mixer.SetFloat("MasterVolume", Mathf.Log10(Mathf.Max(0.0001f, masterVolume)) * 20);
            mixer.SetFloat("BGMVolume", Mathf.Log10(Mathf.Max(0.0001f, bgmVolume)) * 20);
            mixer.SetFloat("SFXVolume", Mathf.Log10(Mathf.Max(0.0001f, sfxVolume)) * 20);
        }
    }

    // ==================== 사운드 재생 ====================
    public void PlaySFX(string soundId)
    {
        if (!soundDataDict.TryGetValue(soundId, out SoundData_KJG data) || data.clip == null)
        {
            Debug.LogWarning($"[AudioManager] SoundData를 찾을 수 없음: {soundId}");
            return;
        }

        if (availableSources.Count == 0)
        {
            Debug.LogWarning("[AudioManager] SFX 풀 부족! 재사용 대기 중...");
            return;
        }

        AudioSource source = availableSources.Dequeue();

        source.clip = data.GetClip();
        source.volume = data.GetVolume() * sfxVolume;
        source.pitch = data.GetPitch();
        source.loop = data.loop;

        if (data.mixerGroup != null)
            source.outputAudioMixerGroup = data.mixerGroup;

        source.Play();

        // 풀 반환 코루틴
        StartCoroutine(ReturnToPool(source, data.clip.length));

        OnPlaySFX?.Invoke(soundId);
    }

    public void PlayBGM(string soundId)
    {
        if (!soundDataDict.TryGetValue(soundId, out SoundData_KJG data) || data.clip == null)
        {
            Debug.LogWarning($"[AudioManager] BGM SoundData를 찾을 수 없음: {soundId}");
            return;
        }

        if (bgmSource != null)
        {
            bgmSource.clip = data.GetClip();
            bgmSource.volume = data.GetVolume() * bgmVolume;
            bgmSource.pitch = data.GetPitch();
            bgmSource.loop = data.loop;

            if (data.mixerGroup != null)
                bgmSource.outputAudioMixerGroup = data.mixerGroup;

            bgmSource.Play();

            OnPlayBGM?.Invoke(soundId);
        }
    }

    // ==================== 풀 반환 ====================
    private IEnumerator ReturnToPool(AudioSource source, float clipLength)
    {
        yield return new WaitForSeconds(clipLength + 0.1f);

        if (source != null)
        {
            source.Stop();
            source.clip = null;
            availableSources.Enqueue(source);
        }
    }

    // ==================== 볼륨 제어 ====================
    public void SetMasterVolume(float value)
    {
        masterVolume = Mathf.Clamp01(value);
        ApplyVolume();
        PlayerPrefs.SetFloat("MasterVolume", masterVolume);
    }

    public void SetBGMVolume(float value)
    {
        bgmVolume = Mathf.Clamp01(value);
        ApplyVolume();
        PlayerPrefs.SetFloat("BGMVolume", bgmVolume);
    }

    public void SetSFXVolume(float value)
    {
        sfxVolume = Mathf.Clamp01(value);
        ApplyVolume();
        PlayerPrefs.SetFloat("SFXVolume", sfxVolume);
    }

    public void StopBGM()
    {
        if (bgmSource != null)
            bgmSource.Stop();
    }

    // ==================== 편의 메서드 ====================
    [ContextMenu("Play Test SFX")]
    private void TestPlaySFX() => PlaySFX("button_click");   // 실제 soundId로 변경해서 테스트
}