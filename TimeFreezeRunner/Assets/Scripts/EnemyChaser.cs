using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class EnemyChaser : MonoBehaviour
{
    [Header("Refs")]
    public Transform player;
    Rigidbody2D rb;

    [Header("Tuning")]
    public float speed = 3.2f;          
    public float steerLerp = 0.20f;     
    public float avoidRayDist = 1.0f;   
    public LayerMask obstacleMask;

    [Header("Visual")]
    public GameObject pauseIcon;     

    Vector2 v;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Start()
    {
        if (!player) player = FindObjectOfType<PlayerController>()?.transform;
    }

    void FixedUpdate()
    {
        if (GameManager.I != null && !GameManager.I.IsPlaying)
    {
        rb.velocity = Vector2.zero;
        if (pauseIcon) pauseIcon.SetActive(true);
        return;
    }
        bool active = GameManager.I != null ? GameManager.I.IsPlayerMoving : true;


        if (pauseIcon) pauseIcon.SetActive(!active);

        if (!active)
        {
            rb.velocity = Vector2.zero;
            return;
        }

        if (!player) return;

        Vector2 toPlayer = (Vector2)player.position - rb.position;
        if (toPlayer.sqrMagnitude < 0.0001f) return;

        Vector2 dir = toPlayer.normalized;

        var hit = Physics2D.Raycast(rb.position, dir, avoidRayDist, obstacleMask);
        if (hit.collider)
        {
            Vector2 perp = new Vector2(-dir.y, dir.x);
            Vector2 dirA = (dir + perp * 0.6f).normalized;
            Vector2 dirB = (dir - perp * 0.6f).normalized;

            bool hitA = Physics2D.Raycast(rb.position, dirA, avoidRayDist, obstacleMask);
            dir = (!hitA) ? dirA : dirB;
        }

        Vector2 desired = dir * speed;
        v = Vector2.Lerp(v, desired, steerLerp);
        rb.velocity = v;
    }

    public void SetFrozenVisual(bool frozen)
    {
        if (pauseIcon) pauseIcon.SetActive(frozen);
    }

    void OnCollisionEnter2D(Collision2D c)
    {
        if (c.rigidbody && c.rigidbody.GetComponent<PlayerController>())
        {
            Debug.Log("Enemy hit Player");
            GameManager.I?.OnPlayerCaught();

        }
    }
}
