using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Character : MonoBehaviour
{
    private int health;
    private int damage;
    private int energy;
    private int gold;

    #region Properties
    public int Health
    {
        get { return health; }
        set { health = value; }
    }

    public int Damage
    {
        get { return damage; }
        set { damage = value; }
    }

    public int Energy
    {
        get { return energy; }
        set { energy = value; }
    }

    public int Gold
    {
        get { return gold; }
        set { gold = value; }
    }

    #endregion

    #region General Methods
    public void TakeDamage(int damage)
    {
        Health -= damage;

        if (Health <= 0)
        {
            Die();
        }
    }

    public int AddGold(int gold)
    {
        return gold;
    }

    public virtual bool Die()
    {
        return true;
    }
    #endregion
}
