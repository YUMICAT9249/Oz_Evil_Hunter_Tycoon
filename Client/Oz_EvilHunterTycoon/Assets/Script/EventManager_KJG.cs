using UnityEngine;
using UnityEngine.Events;
using System.Collections.Generic;

/// <summary>
/// 글로벌 이벤트 매니저
/// 
/// 역할:
/// - 게임 전체에서 발생하는 "시스템 레벨" 이벤트만 관리
/// - Save/Load, UI 새로고침, 게임 시작/종료 같은 공통 이벤트만 담당
/// - 각 매니저의 세부 이벤트(예: OnGoldChanged, OnAchievementUnlocked)는 
///   해당 매니저에서 C# event로 직접 관리 (진구님 의도대로)
/// 
/// 사용 방법:
/// EventManager_KJG.Instance.Invoke(EventManager_KJG.GameEvent.RequestSave);
/// EventManager_KJG.Instance.AddListener(EventManager_KJG.GameEvent.RefreshUI, RefreshUIHandler);
/// </summary>
public class EventManager_KJG : BaseManager_KJG<EventManager_KJG>
{
    /// <summary>
    /// 글로벌 시스템 이벤트 목록 (필요한 것만 최소화)
    /// </summary>
    public enum GameEvent
    {
        GameStart,
        GameOver,
        GamePause,
        GameResume,
        SceneLoaded,

        // 저장/로드 관련 (글로벌)
        RequestSave,        // 누군가 "지금 저장해!" 요청
        RefreshUI,          // UI 전체 새로고침 요청

        // 전투/진행 관련 글로벌 이벤트
        EnemyDied,
        BossDefeated,
        WaveCleared,
    }

    private Dictionary<GameEvent, UnityEvent> globalEvents = new Dictionary<GameEvent, UnityEvent>();

    protected override void Awake()
    {
        base.Awake();
        Debug.Log("✅ [EventManager_KJG] 글로벌 이벤트 시스템 초기화 완료");
    }

    // ==================== 이벤트 등록 ====================
    public void AddListener(GameEvent eventType, UnityAction listener)
    {
        if (!globalEvents.ContainsKey(eventType))
            globalEvents[eventType] = new UnityEvent();

        globalEvents[eventType].AddListener(listener);
    }

    public void RemoveListener(GameEvent eventType, UnityAction listener)
    {
        if (globalEvents.TryGetValue(eventType, out var unityEvent))
            unityEvent.RemoveListener(listener);
    }

    // ==================== 이벤트 발생 ====================
    public void Invoke(GameEvent eventType)
    {
        if (globalEvents.TryGetValue(eventType, out var unityEvent))
        {
            unityEvent?.Invoke();
        }
        else
        {
            Debug.LogWarning($"[EventManager_KJG] 이벤트 '{eventType}'이 등록되지 않았습니다.");
        }
    }

    // ==================== 자주 사용하는 편의 메서드 ====================
    public void RequestSave() => Invoke(GameEvent.RequestSave);
    public void RefreshUI() => Invoke(GameEvent.RefreshUI);
}