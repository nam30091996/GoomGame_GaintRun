using System;
using UnityEngine;
using UnityEngine.EventSystems;

public class GamePlayController : MonoBehaviour, IPointerDownHandler
{
    public PlayerController player;
    public MapController map;

    private bool startGame = false;

    private void Start()
    {
        startGame = false;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (!startGame)
        {
            startGame = true;
            player.StartRun();
            map.StartMove();
        }
    }

    // public void OnPointerUp(PointerEventData eventData)
    // {
    // }
}