using System.Collections.Generic;
using UnityEngine;

// ★ 건물 대기열 시스템
public class BuildingQueue_YHJ : MonoBehaviour
{
    private Queue<IUnit_YHJ> queue =
        new Queue<IUnit_YHJ>();

    // 대기열 추가
    public void Enqueue(IUnit_YHJ unit)
    {
        if (unit == null)
        {
            Debug.LogWarning("[Queue] null 유닛 들어옴");
            return;
        }

        if (queue.Contains(unit))
        {
            Debug.Log("[Queue] 이미 존재하는 유닛");
            return;
        }

        queue.Enqueue(unit);

        Debug.Log($"[Queue] 추가됨 / 현재 인원: {queue.Count}");
    }

    // 대기열 처리
    public IUnit_YHJ Dequeue()
    {
        if (TryDequeue(out var unit))
            return unit;

        return null;
    }

    // 대기 인원 수
    public int Count => queue.Count;

    // 비어있는지
    public bool IsEmpty()
    {
        return queue.Count == 0;
    }
    public bool Contains(IUnit_YHJ unit)
    {
        return queue.Contains(unit);
    }

    public bool TryDequeue(out IUnit_YHJ unit)
    {
        if (queue.Count == 0)
        {
            unit = null;
            return false;
        }

        unit = queue.Dequeue();

        Debug.Log($"[Queue] 처리됨 / 남은 인원: {queue.Count}");

        return true;
    }

    public IUnit_YHJ Peek()
    {
        if (queue.Count == 0)
            return null;

        return queue.Peek();
    }
    public void Remove(IUnit_YHJ unit)
    {
        if (!queue.Contains(unit))
            return;

        var tempQueue = new Queue<IUnit_YHJ>();

        while (queue.Count > 0)
        {
            var current = queue.Dequeue();

            if (current != unit)
                tempQueue.Enqueue(current);
        }

        queue = tempQueue;

        Debug.Log("[Queue] 특정 유닛 제거 완료");
    }
}