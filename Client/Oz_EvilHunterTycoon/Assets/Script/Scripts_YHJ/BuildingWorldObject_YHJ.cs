using UnityEngine;

public class BuildingWorldObject_YHJ : BaseWorldObject_KJG
{
    protected override void Awake()
    {
        base.Awake();

        // ★ 건물은 HP 없음
        maxHp = 0;
        OnHealthChanged(0, 0);

        displayName = gameObject.name;
    }

    public override void OnClicked()
    {
        base.OnClicked();

        Debug.Log("건물 클릭됨: " + displayName);

        // ★ 여기서 UI 띄우거나 이벤트 보내기
    }

    // ★ HP 이벤트 막기
    public override void OnHealthChanged(float current, float max)
    {
        // 아무것도 안함 (HP바 안띄움)
    }

    // ★ 데미지 무시
    public override void TakeDamage(float damage)
    {
        // 건물은 데미지 없음
    }
}