using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EquipController : MonoBehaviour
{
    public GameObject CurrentEquippedTool;

    public void Attack(int attackNum)
    {
        if (CurrentEquippedTool.TryGetComponent<WeaponBase>(out var weapBase))
        {
            weapBase.Attack(attackNum);
        }
    }
}
