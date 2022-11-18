using UnityEngine;
using UnityEngine.EventSystems;

public class GamePlayController : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    public PlayerController player;

    public void OnPointerDown(PointerEventData eventData)
    {
        player.startRun = true;
        player.anim.Play("run");
    }

    public void OnPointerUp(PointerEventData eventData)
    {
    }
}