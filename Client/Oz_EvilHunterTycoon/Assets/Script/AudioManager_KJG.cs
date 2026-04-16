using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

/// <summary>
/// [KJG 실무 최고 수준] AudioManager_KJG
///
/// 사용 예시:
/// Manager_KJG.Audio.PlaySFX("monster_death");
/// Manager_KJG.Audio.PlayBGM("main_theme");
/// Manager_KJG.Audio.SetMasterVolume(0.8f);
/// </summary>
public class AudioManager_KJG : BaseManager_KJG<AudioManager_KJG>
{
    [Header("Audio Mixer")]
    [SerializeField] private AudioMixer audioMixer;

    [Header("BGM 전용")]
    [SerializeField] private AudioSource bgmSource;

    [Header("SFX 풀링 설정")]
    [SerializeField] private int sfxPoolSize = 20;

    [Header("기본 볼륨")]
    [Range(0f, 1f)] public float masterVolume = 1f;
    [Range(0f, 1f)] public float bgmVolume = 0.85f;
    [Range(0f, 1f)] public float sfxVolume = 1f;

    // SFX 풀링
    private List<AudioSource> sfxPool = new List<AudioSource>();
    private Queue<AudioSource> availableSFX = new Queue<AudioSource>();

    // SoundData 빠른 검색용
    private Dictionary<string, SoundData_KJG> soundDictionary = new Dictionary<string, SoundData_KJG>();

    protected override void Awake()
    {
        base.Awake();
        InitializePool();
        LoadAllSoundData();
        LoadSavedVolumes();
        Debug.Log("✅ [AudioManager_KJG] 오디오 시스템 완전 초기화 완료");
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

    private void LoadSavedVolumes()
    {
        if (Manager_KJG.SaveLoad != null)
        {
            // SaveLoadManager에서 볼륨 로드 (나중에 SaveLoad와 연동)
            // 현재는 기본값 사용
        }
        ApplyVolumes();
    }

    private void ApplyVolumes()
    {
        audioMixer.SetFloat("MasterVolume", Mathf.Log10(masterVolume) * 20);
        audioMixer.SetFloat("BGMVolume", Mathf.Log10(bgmVolume) * 20);
        audioMixer.SetFloat("SFXVolume", Mathf.Log10(sfxVolume) * 20);
    }

    // ==================== Public API ====================
    public void PlaySFX(string soundId)
    {
        if (!soundDictionary.TryGetValue(soundId, out SoundData_KJG data))
        {
            Debug.LogWarning($"[AudioManager] 사운드 ID를 찾을 수 없음: {soundId}");
            return;
        }

        if (availableSFX.Count == 0)
        {
            Debug.LogWarning("[AudioManager] SFX 풀이 부족합니다. Pool Size를 늘려주세요.");
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

    public void PlayBGM(string soundId, bool fade = true)
    {
        if (!soundDictionary.TryGetValue(soundId, out SoundData_KJG data)) return;

        bgmSource.clip = data.GetClip();
        bgmSource.loop = true;
        bgmSource.outputAudioMixerGroup = data.mixerGroup;
        bgmSource.volume = data.GetVolume();
        bgmSource.pitch = data.GetPitch();
        bgmSource.Play();
    }

    public void StopBGM()
    {
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