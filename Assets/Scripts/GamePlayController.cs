using UnityEngine;
using UnityEngine.EventSystems;

public class GamePlayController : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    public PlayerController player;
    public MapController map;

    public void OnPointerDown(PointerEventData eventData)
    {
        player.StartRun();
        map.StartMove();
    }

    public void OnPointerUp(PointerEventData eventData)
    {
    }
}