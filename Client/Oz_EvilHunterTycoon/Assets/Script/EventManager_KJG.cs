using UnityEngine;
using UnityEngine.Events;
using System.Collections.Generic;

public class EventManager_KJG : MonoBehaviour
{
    public static EventManager_KJG Instance { get; private set; }

    // ==================== Enum 정의 (EventManager 안에 포함) ====================
    public enum GameEvent
    {
        // 시스템
        GameStart,
        GameOver,
        GamePause,
        GameResume,
        SceneLoaded,

        // 저장 & UI
        RequestSave,
        RefreshUI,           

        // 플레이어
        PlayerDied,
        PlayerLevelUp,
        PlayerHealthChanged,

        // 적/전투
        EnemyDied,
        BossDefeated,

        // 기타
        WaveCleared,
        ScoreChanged,
        ItemCollected
    }

    private Dictionary<GameEvent, UnityEvent> globalEvents = new Dictionary<GameEvent, UnityEvent>();

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
        Debug.Log("✅ EventManager_KJG 초기화 완료 (Enum 포함)");
    }

    // ==================== 글로벌 이벤트 메서드 ====================
    public void AddListener(GameEvent eventType, UnityAction listener)
    {
        if (!globalEvents.ContainsKey(eventType))
            globalEvents[eventType] = new UnityEvent();

        globalEvents[eventType].AddListener(listener);
    }

    public void RemoveListener(GameEvent eventType, UnityAction listener)
    {
        if (globalEvents.TryGetValue(eventType, out var unityEvent))
        {
            unityEvent.RemoveListener(listener);
        }
    }

    public void Invoke(GameEvent eventType)
    {
        if (globalEvents.TryGetValue(eventType, out var unityEvent))
        {
            unityEvent?.Invoke();
        }
        else
        {
            Debug.LogWarning($"[EventManager_KJG] 이벤트 '{eventType}'이(가) 등록되지 않았습니다.");
        }
    }

    public void ClearAll()
    {
        foreach (var evt in globalEvents.Values)
            evt.RemoveAllListeners();
        globalEvents.Clear();
    }
}