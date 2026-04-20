using UnityEngine;
using UnityEngine.Audio;

/// <summary>
/// SoundData_KJG - 사운드 데이터 ScriptableObject
/// Resources/Sounds 폴더에 만들어서 사용하세요.
/// </summary>
[CreateAssetMenu(menuName = "KJG/Sound Data", fileName = "New Sound")]
public class SoundData_KJG : ScriptableObject
{
    [Header("기본 정보")]
    [Tooltip("코드에서 호출할 때 사용할 ID (예: monster_death, button_click)")]
    public string soundId;

    [Tooltip("실제 오디오 클립")]
    public AudioClip clip;

    [Header("오디오 설정")]
    [Tooltip("이 사운드가 속할 Mixer Group (Master / BGM / SFX / UI)")]
    public AudioMixerGroup mixerGroup;

    [Range(0f, 1f)]
    public float volume = 1f;

    [Range(0.5f, 2f)]
    public float pitch = 1f;

    [Tooltip("BGM은 true, 대부분 SFX는 false")]
    public bool loop = false;

    [Header("랜덤 변동 (단조로움 방지)")]
    public bool randomizePitch = true;
    [Range(0f, 0.5f)] public float pitchRandomRange = 0.15f;

    public bool randomizeVolume = false;
    [Range(0f, 0.2f)] public float volumeRandomRange = 0.08f;

    // ==================== 유틸리티 ====================
    public AudioClip GetClip() => clip;

    public float GetVolume()
    {
        float v = volume;
        if (randomizeVolume) v += Random.Range(-volumeRandomRange, volumeRandomRange);
        return Mathf.Clamp01(v);
    }

    public float GetPitch()
    {
        float p = pitch;
        if (randomizePitch) p += Random.Range(-pitchRandomRange, pitchRandomRange);
        return Mathf.Clamp(p, 0.5f, 2f);
    }
}