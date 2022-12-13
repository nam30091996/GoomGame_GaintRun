using System;
using UnityEngine;

public class BossController : MonoBehaviour
{
    public Animator anim;
    public int level;
    [HideInInspector] public int hp, dame;

    private void Awake()
    {
        Signals.Get<OnMeetBoss>().AddListener(OnPlayerMeetBoss);
    }

    private void OnDestroy()
    {
        Signals.Get<OnMeetBoss>().RemoveListener(OnPlayerMeetBoss);
    }

    private void Start()
    {
        hp = GameUtils.GetHp(level);
        dame = GameUtils.GetDame(level);
        float scale = 1 + level * 0.05f;
        transform.localScale = new Vector3(scale / 35f, scale * 4f, scale / 35f);
    }

    private void OnPlayerMeetBoss()
    {
        anim.Play("idle_fight");
    }
}