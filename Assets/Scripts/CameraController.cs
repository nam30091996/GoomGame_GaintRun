using System;
using DG.Tweening;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public Transform target;

    public float smoothSpeed = 0.125f;
    private Vector3 offset;
    private bool meetBoss;

    private void Awake()
    {
        Signals.Get<OnMeetBoss>().AddListener(OnMeetBoss);
    }

    private void OnDestroy()
    {
        Signals.Get<OnMeetBoss>().RemoveListener(OnMeetBoss);
    }
    
    private void Start()
    {
        meetBoss = false;
        offset = transform.position - target.position;
    }

    private void LateUpdate()
    {
        if (meetBoss) return;
        Vector3 desiredPosition = target.position + offset;
        Vector3 smoothedPosition = Vector3.Lerp(transform.position, 
            new Vector3(transform.position.x, desiredPosition.y, desiredPosition.z), smoothSpeed);
        transform.position = smoothedPosition;
    }

    private void OnMeetBoss()
    {
        meetBoss = true;
    }
}