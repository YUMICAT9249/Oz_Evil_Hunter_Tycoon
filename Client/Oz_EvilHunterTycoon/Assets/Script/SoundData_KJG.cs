using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

[CreateAssetMenu(fileName = "SoundData", menuName = "Audio/SoundData")]
public class SoundData : ScriptableObject
{
    public string soundId;          // 고유 키(string으로 간단하게 호출하기위함)
    public AudioClip clip;          //오디오 클립 끌어다 놓기
    public AudioMixerGroup mixerGroup;  // BGM(배경음악) / SFX(효과음) / UI(버튼,알림) 중 하나 끌어다 놓기

    [Range(0f, 1f)] public float volume = 1f;
    [Range(0.5f, 2f)] public float pitch = 1f;  //재생속도,음높이 1=정상,0.5=느리고 낮음,2=빠르고 높음
    public bool loop = false;                   //반복 재생할지 여부

    // 같은 사운드가 여러개날때 단조로움을 방지하기위한 스크립트(효과음용)
    [Header("Random Pitch Variation (SFX용)")]
    public bool randomizePitch = false;                         //randomizePitch = true → pitch ± pitchRandomRange 범위에서 랜덤
    [Range(0f, 0.5f)] public float pitchRandomRange = 0.1f;     //필요하면 사용
}
