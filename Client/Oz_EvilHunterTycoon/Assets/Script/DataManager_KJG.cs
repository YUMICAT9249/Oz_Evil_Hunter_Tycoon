using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class DataManager_KJG : MonoBehaviour
{
    public static DataManager_KJG Instance { get; private set; }


  //  [Header("====데이터 테이블===")]


   // private Dictionary<string, DropGroup> dropGroupsCache = new();

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        Initialize();
    }

    public void Initialize()
    {

    }
}
