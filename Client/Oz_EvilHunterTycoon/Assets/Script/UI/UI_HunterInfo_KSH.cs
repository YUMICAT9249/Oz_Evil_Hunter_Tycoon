using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_HunterInfo_KSH : MonoBehaviour
{
    public TMP_Text hunterLevel; // 헌터 레벨
    public TMP_Text hunterJob; // 헌터 직업
    public TMP_Text hunterTeir; // 헌터 등급
    public TMP_Text hunterName; // 헌터 이름
    public TMP_Text hunterGold; // 헌터 소지 골드

    public TMP_Text hunterHp; // 헌터 체력
    public Image hpBar; // 체력바
   

    public TMP_Text hunterExp; // 헌터 경험치

    public TMP_Text hunterAttackDmg; // 헌터 공격력
    public TMP_Text hunterDef; // 헌터 방어력
    public TMP_Text hunterCritical; // 헌터 치명타
    public TMP_Text hunterDodge; // 헌터 회피력
    public TMP_Text hunterAttackSpeed; // 헌터 공격속도

    public TMP_Text hunter_hunger; // 헌터 허기
    public TMP_Text hunter_mood; // 헌터 기분

    public void ShowHunterInfo(HunterData_PJS hunterData)
    {

        hunterName.text = hunterData._hunterNameList;

        hunterExp.text = $"{hunterData._currentExp} / {hunterData._maxExp}"; // 경험치

        hunterHp.text = $"체력 {hunterData._currentHP} / {hunterData._maxHP}"; // 체력
        TextColorSet(hunterHp, hunterData._hpScore);
        hpBar.color = new Color32(255, 165, 0, 255);   // 주황색
        hpBar.transform.localScale = BarController(hunterData._currentHP, hunterData._maxHP);

        //hunterGold.text = hunterData._gold.ToString(); // 소지 골드

        hunterTeir.text = $"{hunterData._hunterRank}";
        hunterLevel.text = $"Lv.{hunterData._currentLevel.ToString()}"; // 현재 레벨
        hunterJob.text = $"{(HunterJop)hunterData._hunterJop}";

        //hunter_hunger = hunterData.hunger

        hunterAttackDmg.text = hunterData._damage.ToString(); // 공격력
        TextColorSet(hunterAttackDmg, hunterData._damageScore); 

        hunterDef.text = hunterData._defence.ToString(); // 방어력
        TextColorSet(hunterAttackDmg, hunterData._defenceScore);

        hunterCritical.text = hunterData._criticalChance.ToString(); // 치명타
        TextColorSet(hunterAttackDmg, hunterData._criticalChanceScore);

        hunterAttackSpeed.text = hunterData._attackCooldown.ToString("F2"); // 공격속도
        TextColorSet(hunterAttackDmg, hunterData._attackCooldownScore);

        hunterDodge.text = hunterData._dodgeChance.ToString(); // 회피력
        TextColorSet(hunterAttackDmg, hunterData._dodgeChanceScore);

    }
    public Vector3 BarController(float min, float max)
    {
        float ratio = min / max;
        Vector3 localScale = new Vector3(ratio, 1f, 1f);
        return localScale;
    }

    public void TextColorSet(TMP_Text text, int tier)
    {
        if (tier == 0)
        {
            text.color = UnityEngine.Color.white;
        }
        else if(tier == 1)
        {
            text.color = new Color32(135, 206, 235, 255); // 하늘색
        }
        else if (tier == 2)
        {
            text.color = new Color32(255, 165, 0, 255);   // 주황색
        }
        else if (tier == 3)
        {
            text.color = new Color32(128, 0, 128, 255);   // 보라색
        }
        // 등급에 따라 색 변경
    }
}
