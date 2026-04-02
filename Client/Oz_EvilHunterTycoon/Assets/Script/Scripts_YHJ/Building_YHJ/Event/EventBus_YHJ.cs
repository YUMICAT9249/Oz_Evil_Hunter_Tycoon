using System;
using UnityEngine;

// ★ 전체 시스템 이벤트 허브
public static class EventBus_YHJ
{
    // ★ 건물 상호작용 요청
    public static Action<GameObject, IUnit_YHJ> RequestInteract;

    // ★ 상호작용 결과
    public static Action<IUnit_YHJ, InteractionResult_YHJ> OnInteractionResult;

    // ★ 아이템 제출 요청
    public static Action<IUnit_YHJ, string> RequestItemFromUnit;

    // ★ 판매 처리 요청
    public static Action<IUnit_YHJ, string, int> RequestSellItem;

    // ★ 현재 인구 요청
    public static Action RequestPopulation;

    // ★ 인구 응답
    public static Action<int, int> OnPopulationResult;

    // ★ 헌터 생성 요청
    public static Action RequestSpawnHunter;

    // ★ 기능: 스킬 UI 열기 요청
    // - 특정 유닛 기준 UI 표시
    // - 현재는 수신자 없음 (UI 연결 예정)
    public static Action<IUnit_YHJ> RequestOpenSkillUI;

    // ★ 기능: 보스 UI 열기 요청
    public static Action RequestOpenBossUI;
}