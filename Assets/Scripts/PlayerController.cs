using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public Animator anim;
    public ParticleSystem trailFx, windFx;
    public TMP_Text txtLevel;
    public List<GameObject> listChar;

    private int level;
    private bool startRun;
    private float forwardSpeed = 3f, sideLerpSpeed = 5;
    private Rigidbody _rigidbody;
    private ItemId currentItemId;

    private bool screenTouch = false;
    private float startTouch = 0, startX = 0;

    private void Start()
    {
        _rigidbody = GetComponent<Rigidbody>();
        level = 1;
        screenTouch = false;
        startTouch = 0;
        startX = 0;
        currentItemId = ItemId.Item1;
    }

    private void Update()
    {
        if (!startRun) return;
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

    private void MoveForward()
    {
        _rigidbody.velocity = Vector3.forward * forwardSpeed;
    }

    private void MoveSideways()
    {
        float change = (Camera.main.ScreenToViewportPoint(Input.mousePosition).x - startTouch) * 8;
        if (startX + change > 4 || startX + change < -4) return;
        transform.position = Vector3.Lerp(transform.position,
            new Vector3(startX + change, transform.position.y, transform.position.z), sideLerpSpeed * Time.deltaTime);


        // Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        // RaycastHit hit;
        // if (Physics.Raycast(ray, out hit, 100))
        // {
        //     // if (hit.point.x > transform.position.x + 0.05f)
        //     //     anim.transform.rotation = Quaternion.Lerp(anim.transform.rotation, Quaternion.Euler(0, 30, 0),
        //     //         sideLerpSpeed * Time.deltaTime);
        //     // else if (hit.point.x < transform.position.x - 0.05f)
        //     //     anim.transform.rotation = Quaternion.Lerp(anim.transform.rotation, Quaternion.Euler(0, -30, 0),
        //     //         sideLerpSpeed * Time.deltaTime);
        //     // else
        //     //     anim.transform.rotation = Quaternion.Lerp(anim.transform.rotation, Quaternion.Euler(0, 0, 0),
        //     //         sideLerpSpeed * Time.deltaTime);
        //
        //     transform.position = Vector3.Lerp(transform.position,
        //         new Vector3(hit.point.x, transform.position.y, transform.position.z), sideLerpSpeed * Time.deltaTime);
        // }
    }

    public void StartRun()
    {
        startRun = true;
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
            if (level == 0) level = 1;
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
            startRun = false;
            txtLevel.transform.parent.gameObject.SetActive(false);
            Signals.Get<OnMeetBoss>().Dispatch();

            Camera.main.transform.DOMove(new Vector3(8.35f, 5.74f, other.transform.position.z), 1f);
            Camera.main.transform.DORotateQuaternion(Quaternion.Euler(7.25f, -90, 0), 1f);
            transform.DOMove(new Vector3(0, transform.position.y, other.transform.position.z - 3f), 1f)
                .OnComplete(() =>
                {
                    anim.Play("idle_fight");
                });
        }
    }
}