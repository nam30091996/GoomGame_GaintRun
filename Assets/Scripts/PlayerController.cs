using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public GamePlayController gamePlayController;
    public Animator anim;
    // public ParticleSystem trailFx, windFx;
    public TMP_Text txtLevel;
    public List<GameObject> listChar;

    private int level;
    private float sideLerpSpeed = 10;
    // private Rigidbody _rigidbody;
    private ItemId currentItemId;

    private bool screenTouch = false;
    private float startTouch = 0, startX = 0;

    [HideInInspector] public int hp, dame;

    private void Start()
    {
        // _rigidbody = GetComponent<Rigidbody>();
        level = 0;
        screenTouch = false;
        startTouch = 0;
        startX = 0;
        currentItemId = ItemId.Item1;
    }

    private void Update()
    {
        if (!gamePlayController.running) return;
        if (Input.GetMouseButton(0) && !screenTouch)
        {
            screenTouch = true;
            startTouch = Camera.main.ScreenToViewportPoint(Input.mousePosition).x;
            startX = transform.position.x;
        }
        else if (Input.GetMouseButton(0))
        {
            MoveSideways();
        }
        else
        {
            screenTouch = false;
        }
    }

    private void MoveSideways()
    {
        float change = (Camera.main.ScreenToViewportPoint(Input.mousePosition).x - startTouch) * 12;
        if (startX + change > 6 || startX + change < -6) return;
        transform.position = Vector3.Lerp(transform.position,
            new Vector3(startX + change, transform.position.y, transform.position.z), sideLerpSpeed * Time.deltaTime);
    }

    public void StartRun()
    {
        anim.Play("run");
        // trailFx.Play();
        // windFx.Play();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag.Equals("Item"))
        {
            bool eat = other.GetComponent<ItemController>().id == currentItemId;

            other.gameObject.SetActive(false);
            if (eat) level++;
            else level--;
            if (level == -1) level = 0;
            txtLevel.text = level.ToString();
            transform.DOScale(Vector3.one + (level - 1) * new Vector3(0.05f, 0.05f, 0.05f), 0.1f);
        }
        else if (other.tag.Equals("ChangeChar"))
        {
            foreach (GameObject o in listChar)
            {
                o.SetActive(false);
            }

            currentItemId = other.GetComponent<ChangeChar>().id;
            switch (currentItemId)
            {
                case ItemId.Item1:
                    listChar[0].SetActive(true);
                    break;
                case ItemId.Item2:
                    listChar[1].SetActive(true);
                    break;
                case ItemId.Item3:
                    listChar[2].SetActive(true);
                    break;
            }
            other.gameObject.SetActive(false);
        } else if (other.tag.Equals("Boss"))
        {
            // startRun = false;
            hp = GameUtils.GetHp(level);
            dame = GameUtils.GetDame(level);
            txtLevel.transform.parent.gameObject.SetActive(false);
            gamePlayController.running = false;
            Signals.Get<OnMeetBoss>().Dispatch();

            Camera.main.transform.DOMove(new Vector3(8.84f, 8.23f, other.transform.position.z), 1f);
            Camera.main.transform.DORotateQuaternion(Quaternion.Euler(16f, -90, 0), 1f);
            transform.DOMove(new Vector3(0, transform.position.y, other.transform.position.z - 3f), 1f)
                .OnComplete(() =>
                {
                    anim.Play("idle_fight");
                    gamePlayController.fighting = true;
                    gamePlayController.StartFighting();
                });
        }
    }
}