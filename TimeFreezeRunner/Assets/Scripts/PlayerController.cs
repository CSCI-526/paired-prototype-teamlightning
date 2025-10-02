using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerController : MonoBehaviour
{
    [Header("Move")]
    public float moveSpeed = 5f;
    public float inputDeadZone = 0.1f;

    [Header("VFX (optional)")]
    public GameObject freezeHint;

    Rigidbody2D rb;
    Vector2 input;
    public bool isMoving { get; private set; }

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        if (GameManager.I != null && !GameManager.I.IsPlaying)
    {
        input = Vector2.zero;
        isMoving = false;
        if (freezeHint) freezeHint.SetActive(true);
        return;
    }
        input.x = Input.GetAxisRaw("Horizontal");
        input.y = Input.GetAxisRaw("Vertical");
        if (input.sqrMagnitude > 1f) input.Normalize();

        isMoving = input.sqrMagnitude > (inputDeadZone * inputDeadZone);

        if (freezeHint) freezeHint.SetActive(!isMoving); 
    }

    void FixedUpdate()
    {
        if (GameManager.I != null && !GameManager.I.IsPlaying)
    {
        rb.velocity = Vector2.zero;
        return;
    }
        rb.velocity = input * moveSpeed;
    }

    public void OnWin()
{
    rb.velocity = Vector2.zero;
    rb.freezeRotation = true;
    StopAllCoroutines();
    StartCoroutine(WinBounce());
}

public void OnLose()
{
    rb.velocity = Vector2.zero;
}

System.Collections.IEnumerator WinBounce()
{
    Vector3 basePos = transform.position;
    float t = 0f, amp = 0.25f, freq = 3.0f;
    while (true)
    {
        t += Time.deltaTime;
        float y = Mathf.Sin(t * Mathf.PI * 2f * freq) * amp;
        transform.position = new Vector3(basePos.x, basePos.y + y, basePos.z);
        yield return null;
    }
}
}
