using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DifficultyManager_KJG : MonoBehaviour
{
    public static DifficultyManager_KJG Instance {  get; private set; }

    public int currentDifficultyLevel = 0;

    private readonly string[] difficultyNames =
    {
        "Easy","Nomal","Hard"
    };


}
