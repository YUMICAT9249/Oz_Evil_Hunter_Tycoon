using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IUnit_YHJ
{
    float CurrentHP { get; }
    float MaxHP { get; }

    bool IsDead { get; }

    void Heal(float amount);
    void Revive();
}
