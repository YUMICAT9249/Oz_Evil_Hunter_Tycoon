using UnityEngine;

/// <summary>
/// IWorldObject_KJG - 맵 위 모든 오브젝트가 구현해야 하는 인터페이스
/// 
/// 역할:
/// - Monster, Hunter, Building 등 모든 맵 오브젝트가 공통으로 가져야 하는 규격
/// - MapManager_KJG가 모든 오브젝트를 동일하게 다룰 수 있게 해줌
/// - 클릭, HP Bar, UI 표시 등에 필요한 공통 정보를 제공
/// </summary>
public interface IWorldObject_KJG
{
    GameObject GameObject { get; }           // 실제 GameObject 참조
    string ObjectType { get; }               // "Monster", "Hunter", "Building" 등 타입
    string DisplayName { get; }              // 화면에 표시될 이름
    float CurrentHp { get; }
    float MaxHp { get; }

    void OnClicked();                        // 클릭되었을 때 호출
    void OnHealthChanged(float current, float max);   // HP가 변했을 때 호출
}