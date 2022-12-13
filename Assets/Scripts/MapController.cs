using System;
using UnityEngine;

public class MapController : MonoBehaviour
{
    public BossController boss;
    
    private bool move = false;
    private Rigidbody _rigidbody;
    private float forwardSpeed = 10f;

    private void Awake()
    {
        Signals.Get<OnMeetBoss>().AddListener(Stop);
    }

    private void OnDestroy()
    {
        Signals.Get<OnMeetBoss>().RemoveListener(Stop);
    }

    private void Start()
    {
        _rigidbody = GetComponent<Rigidbody>();
    }

    private void Update()
    {
        if (move)
        {
            _rigidbody.velocity = Vector3.back * forwardSpeed;
        }
    }

    public void StartMove()
    {
        move = true;
    }

    private void Stop()
    {
        move = false;
        _rigidbody.velocity = Vector3.zero;
    }
}
