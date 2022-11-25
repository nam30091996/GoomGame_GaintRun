using TMPro;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public Animator anim;
    public ParticleSystem trailFx, windFx;
    public TMP_Text txtLevel;

    private int level;
    private bool startRun;
    private float forwardSpeed = 3f, sideLerpSpeed = 5;
    private Rigidbody _rigidbody;

    private void Start()
    {
        _rigidbody = GetComponent<Rigidbody>();
        level = 1;
    }

    private void Update()
    {
        // if (startRun)
        // {
        //     MoveForward();
        // }

        if (Input.GetMouseButton(0))
        {
            MoveSideways();
        }
    }

    private void MoveForward()
    {
        
        _rigidbody.velocity = Vector3.forward * forwardSpeed;
    }

    private void MoveSideways()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, 100))
        {
            if (hit.point.x > transform.position.x + 0.05f)
                anim.transform.rotation = Quaternion.Lerp(anim.transform.rotation, Quaternion.Euler(0, 30, 0),
                    sideLerpSpeed * Time.deltaTime);
            else if (hit.point.x < transform.position.x - 0.05f)
                anim.transform.rotation = Quaternion.Lerp(anim.transform.rotation, Quaternion.Euler(0, -30, 0),
                    sideLerpSpeed * Time.deltaTime);
            else
                anim.transform.rotation = Quaternion.Lerp(anim.transform.rotation, Quaternion.Euler(0, 0, 0),
                    sideLerpSpeed * Time.deltaTime);

            transform.position = Vector3.Lerp(transform.position,
                new Vector3(hit.point.x, transform.position.y, transform.position.z), sideLerpSpeed * Time.deltaTime);
        }
    }

    public void StartRun()
    {
        startRun = true;
        anim.Play("run");
        trailFx.Play();
        windFx.Play();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag.Equals("diamond"))
        {
            other.gameObject.SetActive(false);
            transform.localScale += new Vector3(0.05f, 0.05f, 0.05f);
            level++;
            txtLevel.text = "lv " + level;
        }
    }
}