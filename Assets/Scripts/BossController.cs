using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossController : MonoBehaviour
{
    public Animator anim;

    private void Start()
    {
        anim.Play("idle_fight");
    }
}
