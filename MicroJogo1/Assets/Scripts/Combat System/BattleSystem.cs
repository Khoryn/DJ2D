using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum BattleState { IDLE, START, PLAYERTURN, ENEMYTURN, WON, LOST }

public class BattleSystem : MonoBehaviour
{
    public BattleState state;

    [SerializeField]
    private GameObject playerPrefab;
    [SerializeField]
    private List<GameObject> enemyPrefabs = new List<GameObject>();

    private int shieldCounter;

    [SerializeField]
    private GameObject gameUI;

    public Transform playerBattleStation;
    public Transform enemyBattleStation;

    private Unit playerUnit;
    private Unit enemyUnit;

    private GameObject playerGO;
    private GameObject enemyGO;

    [SerializeField]
    private BattleHUD playerHUD;
    [SerializeField]
    private BattleHUD enemyHUD;

    [SerializeField]
    private GameObject shieldEffectPrefab;
    private GameObject shieldGO;
    [SerializeField]
    private GameObject healEffectPrefab;
    [SerializeField]
    private GameObject hitEffectPrefab;

    private int enemyCounter = 0;

    Player player;
    GUIManager guiManager;

    void Start()
    {
        gameUI.SetActive(false);
        player = FindObjectOfType<Player>();
        guiManager = FindObjectOfType<GUIManager>();
    }

    private void Update()
    {
        Win();
    }

    #region Enumerators

    public IEnumerator SetupBattle()
    {
        // Show the battle UI;
        gameUI.SetActive(true);

        // Instantiate the player
        playerGO = Instantiate(playerPrefab, playerBattleStation);
        playerUnit = playerGO.GetComponent<Unit>();

        // Instantiate the enemy
        enemyGO = Instantiate(enemyPrefabs[Random.Range(0, enemyPrefabs.Count)], enemyBattleStation);
        enemyUnit = enemyGO.GetComponent<Unit>();

        // Set both player and enemy HUD
        playerHUD.SetHUD(playerUnit);
        enemyHUD.SetHUD(enemyUnit);

        yield return new WaitForSeconds(0.5f);

        // Set player's turn
        state = BattleState.PLAYERTURN;
        PlayerTurn();
    }

    IEnumerator PlayerAttack()
    {
        // Adjust damage values
        bool isDead = enemyUnit.TakeDamage(Random.Range(playerUnit.damage / 2, playerUnit.damage));
        enemyHUD.SetHP(enemyUnit.currentHP);

        // Instantiate the hit tparticle system
        GameObject enemyHit = Instantiate(hitEffectPrefab, enemyBattleStation);

        if (playerUnit.shieldAmount == 20 && shieldCounter <= 2)
        {
            playerUnit.TakeDamage(5);
            playerHUD.SetHP(playerUnit.currentHP);
            shieldCounter += 1;
        }
        else
        {
            playerUnit.shieldAmount = 0;
            Destroy(shieldGO);
        }

        // Check if the enemy is dead
        if (isDead)
        {
            state = BattleState.WON;
            EndBattle();
            Destroy(shieldGO); // Redundant but just in case
        }
        else
        {
            state = BattleState.ENEMYTURN;
            StartCoroutine(EnemyTurn());
        }
        yield return new WaitForSeconds(1f);
        Destroy(enemyHit);
    }

    IEnumerator EnemyTurn()
    {
        yield return new WaitForSeconds(1f);

        // Adjust damage values
        bool isDead = playerUnit.TakeDamage(enemyUnit.damage - playerUnit.shieldAmount);
        playerHUD.SetHP(playerUnit.currentHP);

        // Instantiate the hit tparticle system
        GameObject playerHit = Instantiate(hitEffectPrefab, playerBattleStation);
       
        // Check if the player is dead, if stament is true then End Game
        if (isDead)
        {
            state = BattleState.LOST;
            EndBattle();
        }
        else
        {
            state = BattleState.PLAYERTURN;
            PlayerTurn();
        }
        yield return new WaitForSeconds(1f);
        Destroy(playerHit);
    }

    IEnumerator PlayerHeal()
    {
        // Adjust healing values
        playerUnit.Heal(playerUnit.healAmount);
        playerHUD.SetHP(playerUnit.currentHP);
        GameObject playerHeal = Instantiate(healEffectPrefab, playerBattleStation);

        yield return new WaitForSeconds(0.2f);

        state = BattleState.ENEMYTURN;
        StartCoroutine(EnemyTurn());
        Destroy(playerHeal);
    }

    IEnumerator PlayerShield()
    {
        // Add magic shield to player
        if (playerUnit.shieldAmount < 20)
        {
            shieldCounter = 0;
            playerUnit.shieldAmount = 20;
            playerHUD.SetHP(playerUnit.currentHP);
            shieldGO = Instantiate(shieldEffectPrefab, playerBattleStation);
        }
        else
        {
            Debug.Log("Shield already active");
        }
        
        yield return new WaitForSeconds(1f);

        state = BattleState.ENEMYTURN;
        StartCoroutine(EnemyTurn());
    }

    #endregion

    #region Button Actions

    public void OnAttackButton()
    {
        if (state != BattleState.PLAYERTURN)
        {
            return;
        }
        else
        {
            StartCoroutine(PlayerAttack());
        }
    }

    public void OnHealButton()
    {
        if (state != BattleState.PLAYERTURN)
        {
            return;
        }
        else
        {
            StartCoroutine(PlayerHeal());
        }
    }

    public void OnShieldButton()
    {
        if (state != BattleState.PLAYERTURN)
        {
            return;
        }
        else
        {
            StartCoroutine(PlayerShield());
        }
    }

    #endregion

    public void PlayerTurn()
    {
        // Show text
    }

    public void Win()
    {
        if (enemyCounter == 3)
        {
            guiManager.WinGame();
            gameUI.SetActive(false);
            player.ResetPlayer();
            enemyCounter = 0;
        }
    }

    public void EndBattle()
    {
        if (state == BattleState.WON)
        {
            gameUI.SetActive(false);
            Destroy(playerGO);
            Destroy(enemyGO);
            player.state = PlayerState.WALKING;
            enemyCounter += 1;
        }
        else if (state == BattleState.LOST)
        {
            guiManager.Death();
            gameUI.SetActive(false);
            player.ResetPlayer();
            Destroy(enemyGO);
            Destroy(playerGO);
            enemyCounter = 0;
        }
    }
}
