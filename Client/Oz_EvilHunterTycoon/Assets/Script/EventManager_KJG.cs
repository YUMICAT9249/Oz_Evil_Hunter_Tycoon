using System.Collections;
using System.Collections.Generic;
using System.Net.NetworkInformation;
using UnityEngine;
using UnityEngine.Events;

public class EventManager_KJG : MonoBehaviour
{
    // 싱글톤
    public static EventManager_KJG Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        // 필요하면 여기서 초기화 로직 추가
        Debug.Log("Event 초기화 완료");
    }

    // 화폐 관련
    public static readonly UnityEvent<double> OnGoldChanged = new UnityEvent<double>();
    public static readonly UnityEvent<long> OnExpChanged = new UnityEvent<long>();
    public static readonly UnityEvent<int> OnCashChanged = new UnityEvent<int>();


}