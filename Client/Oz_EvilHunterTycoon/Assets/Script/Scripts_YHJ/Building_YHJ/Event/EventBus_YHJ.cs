using System;
using UnityEngine;

// ★ 전체 시스템 이벤트 허브
public static class EventBus_YHJ
{
    // ★ 건물 상호작용 요청
    public static Action<GameObject, IUnit_YHJ> RequestInteract;

    // ★ 상호작용 결과
    public static Action<IUnit_YHJ, InteractionResult_YHJ> OnInteractionResult;

    // ★ 아이템 제출 요청 (레거시)
    // UI/헌터팀 직접판매 흐름에서 "이 아이템을 가능한 만큼 제출" 의미로 유지합니다.
    // amount 개념이 필요한 새 UI는 아래 RequestSellItem을 우선 사용하세요.
    public static Action<IUnit_YHJ, string> RequestItemFromUnit;

    // ★ 판매 처리 요청 (UI 요청 매입 / 수량 지정 판매용)
    // - itemID: 거래소가 원하는 재료 ID
    // - amount: 이번 제출에서 필요한 수량
    // UI 담당:
    // 1. 거래소 UI에서 부족 재료/수량을 정한다.
    // 2. TradeInteraction_YHJ.SetPurchaseRequest(...) 호출
    // 3. 헌터가 거래소에 오면 TradeInteraction_YHJ가 이 이벤트를 발행한다.
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

    public static Action<IUnit_YHJ, GameObject> RequestProcessUnit;

    public static Action<IUnit_YHJ, string, int> OnItemReceived;

    // ★ 거래소 요청 상태 변경 알림
    // UI 담당:
    // - building: 어떤 거래소 UI를 갱신할지 식별
    // - itemID: 현재 요청 중인 재료 ID (없으면 빈 문자열)
    // - remainingAmount: 남은 요청 수량 (0이면 요청 없음)
    public static Action<GameObject, string, int> OnTradeRequestChanged;

    public static Action<IUnit_YHJ, string> RequestBuyItem;

    public static Action<IUnit_YHJ, string, bool> OnBuyItemResult;

    // ★ 헌터 개인 골드 지불 요청
    // 헌터 담당 스크립트에서 unit을 확인한 뒤 resultCallback(true/false)을 호출해준다.
    public static Action<IUnit_YHJ, int, Action<bool>> RequestSpendUnitGold;

    // ★ 헌터 개인 골드 지불 결과 알림
    public static Action<IUnit_YHJ, int, bool> OnSpendUnitGoldResult;
}
