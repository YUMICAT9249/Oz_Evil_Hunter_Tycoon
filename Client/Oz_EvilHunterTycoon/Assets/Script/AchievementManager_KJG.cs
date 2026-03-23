using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AchievementManager_KJG : MonoBehaviour
{
    [Serializable]
    public class Achievement
    {
        public string id;              // 고유 키
        public string title;           // 표시용 이름
        public string description;     // "적을 100마리 처치하세요" 업적 내용 설명

        public int current;            // 현재 진행도
        public int target;             // 목표치 (target <= current 면 달성)

        public bool isUnlocked;        // 달성 여부
        public bool isSecret;          // 비밀 업적인가?
        public DateTime? unlockTime;   // 언제 깼는지 (랭킹/기념용)

        // UI나 팝업에 쓸 수 있는 추가 정보
        public Sprite icon;            // 아이콘 붙이기 인스펙터에서 드래그 가능
        public string unlockMessage;   // "업적 달성!"
    }
}


