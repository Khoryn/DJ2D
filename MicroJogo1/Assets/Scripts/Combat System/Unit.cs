using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Unit : MonoBehaviour
{
    public string unitName;

    public int damage;
    public int healAmount;

    public int maxHP;
    public int currentHP;

    public int shieldAmount;

    private void Start()
    {
        currentHP = maxHP;
    }

    public bool TakeDamage(int damage)
    {
        currentHP -= damage;

        if (currentHP <= 0)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    public void Heal(int healAmount)
    {
        currentHP += healAmount;

        if (currentHP > maxHP)
        {

            currentHP = maxHP;
        }
    }
}
