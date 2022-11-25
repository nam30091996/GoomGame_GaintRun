using UnityEngine;

public class MapController : MonoBehaviour
{
    private bool startMove = false;
    private Rigidbody _rigidbody;
    private float forwardSpeed = 10f;

    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody>();
    }

    private void Update()
    {
        if (startMove)
        {
            _rigidbody.velocity = Vector3.back * forwardSpeed;
        }
    }

    public void StartMove()
    {
        startMove = true;
    }
}
