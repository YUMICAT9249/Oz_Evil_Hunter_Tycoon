using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

[CreateAssetMenu(fileName = "SoundData", menuName = "Audio/SoundData")]
public class SoundData : ScriptableObject
{
    public string soundId;          // 고유 키
    public AudioClip clip;          //오디오 클립 끌어다 놓기
    public AudioMixerGroup mixerGroup;  // BGM / SFX / UI 중 하나 끌어다 놓기

    [Range(0f, 1f)] public float volume = 1f;
    [Range(0.5f, 2f)] public float pitch = 1f;
    public bool loop = false;

    [Header("Random Pitch Variation (SFX용)")]
    public bool randomizePitch = false;
    [Range(0f, 0.5f)] public float pitchRandomRange = 0.1f;
}
