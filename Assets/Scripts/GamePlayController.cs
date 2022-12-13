using System.Collections;
using DG.Tweening;
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class GamePlayController : MonoBehaviour, IPointerDownHandler
{
    public PlayerController player;
    public MapController map;

    public GameObject fightCanvas;
    public Slider playerHp, bossHp;

    [HideInInspector] public bool startGame = false, running = false, fighting = false;

    private void Start()
    {
        startGame = false;
        running = false;
        fighting = false;
    }


    public void OnPointerDown(PointerEventData eventData)
    {
        if (!startGame)
        {
            startGame = true;
            running = true;
            player.StartRun();
            map.StartMove();
            player.GetComponent<Rigidbody>().isKinematic = false;
        }

        if (fighting)
        {
            playerAttack = true;
        }
    }

    private bool playerAttack = false;
    private float bossAttackPercent = 15;

    public void StartFighting()
    {
        fightCanvas.SetActive(true);
        playerAttack = false;
        bossAttackPercent = 15;

        playerHp.maxValue = player.hp;
        playerHp.value = player.hp;

        bossHp.maxValue = map.boss.hp;
        bossHp.value = map.boss.hp;

        StartCoroutine(CheckAttackPlayerOrBoss());
    }

    private IEnumerator CheckAttackPlayerOrBoss()
    {
        while (player.hp > 0 && map.boss.hp > 0)
        {
            yield return new WaitForSeconds(1f);
            if (Random.Range(0f, 100f) <= bossAttackPercent) BossAttack();
            else if (playerAttack) PlayerAttack();
        }
    }

    private void BossAttack()
    {
        map.boss.anim.Play("attack");
        Invoke(nameof(DecreasePlayerHpBar), 0.5f);
        playerAttack = false;
        bossAttackPercent = 15;
    }

    private void DecreasePlayerHpBar()
    {
        player.hp -= map.boss.dame;
        DOTween.To(() => playerHp.value, x => playerHp.value = x, player.hp, 0.25f);
    }

    private void PlayerAttack()
    {
        player.anim.Play("attack");
        // bossHp.value -= 10;
        Invoke(nameof(DecreaseBossHpBar), 0.5f);
        playerAttack = false;
        bossAttackPercent += 15;
    }

    private void DecreaseBossHpBar()
    {
        map.boss.hp -= player.dame;
        DOTween.To(() => bossHp.value, x => bossHp.value = x, map.boss.hp, 0.25f);
    }
}