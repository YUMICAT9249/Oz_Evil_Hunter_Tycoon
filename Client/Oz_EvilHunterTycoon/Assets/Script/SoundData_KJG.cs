using UnityEngine;
using UnityEngine.Audio;

[CreateAssetMenu(fileName = "SoundData_KJG", menuName = "Audio/Sound Data_KJG")]
public class SoundData_KJG : ScriptableObject
{
    [Header("기본 정보")]
    [Tooltip("사운드를 호출할 때 사용할 고유 ID (예: button_click, enemy_hit)")]
    public string soundId;

    [Tooltip("재생할 오디오 클립")]
    public AudioClip clip;

    [Header("오디오 설정")]
    [Tooltip("이 사운드가 속할 Mixer Group (BGM / SFX / UI 등)")]
    public AudioMixerGroup mixerGroup;

    [Range(0f, 1f)]
    [Tooltip("기본 볼륨")]
    public float volume = 1f;

    [Range(0.5f, 2f)]
    [Tooltip("기본 피치 (1 = 정상 속도)")]
    public float pitch = 1f;

    [Tooltip("루프 재생 여부 (BGM은 true, 대부분 SFX는 false)")]
    public bool loop = false;

    [Header("랜덤 변동 (단조로움 방지 - SFX 추천)")]
    [Tooltip("피치 랜덤화 여부")]
    public bool randomizePitch = false;

    [Range(0f, 0.5f)]
    [Tooltip("피치 변동 범위 (± 값)")]
    public float pitchRandomRange = 0.1f;

    [Tooltip("볼륨도 약간 랜덤하게 하고 싶을 때")]
    public bool randomizeVolume = false;

    [Range(0f, 0.2f)]
    public float volumeRandomRange = 0.05f;

    // ==================== 유틸리티 메서드 ====================
    public AudioClip GetClip() => clip;

    public float GetVolume()
    {
        float finalVolume = volume;
        if (randomizeVolume)
            finalVolume += Random.Range(-volumeRandomRange, volumeRandomRange);
        return Mathf.Clamp01(finalVolume);
    }

    public float GetPitch()
    {
        float finalPitch = pitch;
        if (randomizePitch)
            finalPitch += Random.Range(-pitchRandomRange, pitchRandomRange);
        return Mathf.Clamp(finalPitch, 0.5f, 2f);
    }
}