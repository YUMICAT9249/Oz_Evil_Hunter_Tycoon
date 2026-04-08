using UnityEngine;

public class BuildingInteractionReceiver_YHJ : MonoBehaviour
{
    private IBuildingInteraction_YHJ interaction;

    void Awake()
    {
        interaction = GetComponent<IBuildingInteraction_YHJ>();
    }

    void OnEnable()
    {
        EventBus_YHJ.RequestInteract += OnRequestInteract;
    }

    void OnDisable()
    {
        EventBus_YHJ.RequestInteract -= OnRequestInteract;
    }

    private void OnRequestInteract(GameObject target, IUnit_YHJ unit)
    {
        // ⭐ 이 건물이 아닌 경우 무시
        if (target != gameObject)
            return;

        if (interaction == null)
        {
            Debug.LogWarning("Interaction 없음");
            return;
        }

        if (interaction.CanInteract(unit))
        {
            interaction.Interact(unit);
        }
        else
        {
            Debug.Log("조건 불충족");
        }
    }
}