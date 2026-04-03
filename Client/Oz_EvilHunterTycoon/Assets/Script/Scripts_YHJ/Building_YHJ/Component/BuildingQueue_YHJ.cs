using System.Collections.Generic;
using UnityEngine;

// ★ 건물 대기열 시스템
public class BuildingQueue_YHJ : MonoBehaviour
{
    private Queue<IUnit_YHJ> queue =
        new Queue<IUnit_YHJ>();

    // ★ 대기열 추가
    public void Enqueue(IUnit_YHJ unit)
    {
        queue.Enqueue(unit);

        Debug.Log($"[Queue] 추가됨 / 현재 인원: {queue.Count}");
    }

    // ★ 대기열 처리
    public IUnit_YHJ Dequeue()
    {
        if (queue.Count == 0)
            return null;

        var unit = queue.Dequeue();

        Debug.Log($"[Queue] 처리됨 / 남은 인원: {queue.Count}");

        return unit;
    }

    // ★ 대기 인원 수
    public int Count => queue.Count;

    // ★ 비어있는지
    public bool IsEmpty()
    {
        return queue.Count == 0;
    }
}