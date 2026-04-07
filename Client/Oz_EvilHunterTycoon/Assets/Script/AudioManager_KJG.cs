using UnityEngine;
using UnityEngine.Audio;
using System;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// AudioManager - 게임 내 모든 사운드를 총괄하는 매니저
/// 
/// 사용 예시:
/// Manager_KJG.Audio.PlaySFX("button_click");     // 버튼 클릭 사운드 재생
/// Manager_KJG.Audio.PlayBGM("main_theme");       // 배경 음악 재생
/// Manager_KJG.Audio.SetMasterVolume(0.8f);       // 전체 볼륨 조절
/// 
/// 특징:
/// - Manager_KJG.Audio.XXX 형태로만 접근 가능 (직접 .Instance 사용 안 함)
/// - SFX는 풀링(Pooling) 방식으로 효율적으로 관리
/// - BGM은 별도 AudioSource로 관리
/// - 볼륨은 PlayerPrefs에 저장되어 게임을 껐다 켜도 유지됨
/// </summary>
public class AudioManager_KJG : BaseManager_KJG<AudioManager_KJG>
{
    [Header("Audio Mixer (Unity Mixer Asset 연결)")]
    [SerializeField] private AudioMixer mixer;               // Master, BGM, SFX 그룹을 제어하는 Mixer

    [Header("BGM 전용 AudioSource")]
    [SerializeField] private AudioSource bgmSource;          // BGM은 Loop되므로 별도 관리

    [Header("SFX 풀링 설정")]
    [SerializeField] private int sfxPoolSize = 15;           // 한 번에 동시에 재생할 수 있는 SFX 개수 (성능 최적화)

    [Header("기본 볼륨 설정")]
    [Range(0f, 1f)] public float masterVolume = 1f;         // 전체 볼륨
    [Range(0f, 1f)] public float bgmVolume = 0.8f;          // BGM 전용 볼륨
    [Range(0f, 1f)] public float sfxVolume = 1f;            // 효과음 전용 볼륨

    // SFX 풀링용 변수
    private List<AudioSource> sfxPool = new List<AudioSource>();
    private Queue<AudioSource> availableSources = new Queue<AudioSource>();

    // SoundData_KJG를 soundId로 빠르게 찾기 위한 딕셔너리
    private Dictionary<string, SoundData_KJG> soundDataDict = new Dictionary<string, SoundData_KJG>();

    protected override void Awake()
    {
        base.Awake();

        InitializePool();      // SFX 풀 미리 만들기
        LoadSounds();          // Resources 폴더에서 모든 SoundData 로드
        LoadVolumeSettings();  // 이전에 저장한 볼륨 불러오기

        Debug.Log("✅ [AudioManager_KJG] 오디오 시스템 초기화 완료");
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

            if (mixer != null)
                source.outputAudioMixerGroup = mixer.FindMatchingGroups("SFX")[0];

            sfxPool.Add(source);
            availableSources.Enqueue(source);
        }
    }

    // ==================== SoundData 로드 ====================
    private void LoadSounds()
    {
        soundDataDict.Clear();

        // Resources/Audio/SoundData 폴더에 있는 모든 SoundData_KJG를 자동으로 로드
        SoundData_KJG[] soundDatas = Resources.LoadAll<SoundData_KJG>("Audio/SoundData");

        foreach (var data in soundDatas)
        {
            if (!string.IsNullOrEmpty(data.soundId))
                soundDataDict[data.soundId] = data;
        }

        Debug.Log($"[AudioManager_KJG] {soundDataDict.Count}개의 SoundData_KJG를 로드했습니다.");
    }

    // ==================== 볼륨 불러오기 ====================
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

    // ==================== 사운드 재생 메서드 ====================
    public void PlaySFX(string soundId)
    {
        if (!soundDataDict.TryGetValue(soundId, out SoundData_KJG data) || data.clip == null)
        {
            Debug.LogWarning($"[AudioManager_KJG] SoundData를 찾을 수 없음: {soundId}");
            return;
        }

        if (availableSources.Count == 0)
        {
            Debug.LogWarning("[AudioManager_KJG] SFX 풀 부족! 재사용 대기 중...");
            return;
        }

        AudioSource source = availableSources.Dequeue();
        source.clip = data.GetClip();
        source.volume = data.GetVolume() * sfxVolume;
        source.pitch = data.GetPitch();
        source.loop = data.loop;
        source.Play();

        StartCoroutine(ReturnToPool(source, data.clip.length));
    }

    public void PlayBGM(string soundId)
    {
        if (!soundDataDict.TryGetValue(soundId, out SoundData_KJG data) || data.clip == null) return;

        if (bgmSource != null)
        {
            bgmSource.clip = data.GetClip();
            bgmSource.volume = data.GetVolume() * bgmVolume;
            bgmSource.pitch = data.GetPitch();
            bgmSource.loop = data.loop;
            bgmSource.Play();
        }
    }

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

    // ==================== 볼륨 조절 메서드 ====================
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
}