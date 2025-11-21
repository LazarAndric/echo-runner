using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class RunnerMovement : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed = 8f;        // maksimalna brzina
    public float acceleration = 30f;    // koliko brzo dostiže moveSpeed

    [Header("Turning")]
    public float turnSpeedDegrees = 360f; // koliko stepeni u sekundi može da se okrene

    [Header("Dash")]
    public float dashSpeed = 20f;
    public float dashDuration = 0.15f;
    public float dashCooldown = 1.0f;

    [Header("Floating visual")]
    public Transform visual;
    public float floatAmplitude = 0.1f;
    public float floatFrequency = 2f;

    private Rigidbody _rb;

    private Vector3 _inputDir;      // sirovi input (šta igrač hoće)
    private Vector3 _moveDir;       // stvarni smer kretanja/tela
    private float _currentSpeed = 0f;

    // Dash stanje
    private bool _isDashing = false;
    private float _dashTimer = 0f;
    private float _dashCooldownTimer = 0f;
    private Vector3 _dashDirection;

    private Vector3 _visualBaseLocalPos;

    void Awake()
    {
        _rb = GetComponent<Rigidbody>();
        _rb.useGravity = false;
        _rb.constraints = RigidbodyConstraints.FreezeRotationX |
                          RigidbodyConstraints.FreezeRotationY |
                          RigidbodyConstraints.FreezeRotationZ;

        if (visual != null)
            _visualBaseLocalPos = visual.localPosition;

        // neka početni smer bude "napred" u world-u
        _moveDir = Vector3.forward;
    }

    void Update()
    {
        // --- INPUT ---
        float h = Input.GetAxisRaw("Horizontal");
        float v = Input.GetAxisRaw("Vertical");

        _inputDir = new Vector3(h, 0f, v);
        if (_inputDir.sqrMagnitude > 1f)
            _inputDir.Normalize();

        // Dash tajmeri
        if (_dashCooldownTimer > 0f)
            _dashCooldownTimer -= Time.deltaTime;

        if (_isDashing)
        {
            _dashTimer -= Time.deltaTime;
            if (_dashTimer <= 0f)
            {
                _isDashing = false;
            }
        }
        else
        {
            // Start dash-a na TAP Space
            if (Input.GetKeyDown(KeyCode.Space) && _dashCooldownTimer <= 0f)
            {
                // Koristi smer kretanja ako ga ima, inače input
                Vector3 dir = _moveDir;
                if (dir.sqrMagnitude < 0.001f)
                    dir = _inputDir;

                if (dir.sqrMagnitude > 0.001f)
                {
                    _dashDirection = dir.normalized;
                    _isDashing = true;
                    _dashTimer = dashDuration;
                    _dashCooldownTimer = dashCooldown;
                }
            }
        }

        // Floating vizuel
        if (visual != null)
        {
            float floatOffset = Mathf.Sin(Time.time * floatFrequency) * floatAmplitude;
            visual.localPosition = _visualBaseLocalPos + new Vector3(0f, floatOffset, 0f);
        }
    }

    void FixedUpdate()
    {
        if (_isDashing)
        {
            // Tokom dash-a direktno setujemo velocity
            Vector3 dashVel = _dashDirection * dashSpeed;
            _rb.linearVelocity = new Vector3(dashVel.x, _rb.linearVelocity.y, dashVel.z);
        }
        else
        {
            // --- NORMALNO KRETANJE BEZ KLIZANJA ---

            if (_inputDir.sqrMagnitude > 0.0001f)
            {
                // Ako je prvi frame ili stojimo, preuzmi input kao smer
                if (_moveDir.sqrMagnitude < 0.0001f)
                    _moveDir = _inputDir;

                // Postepeno okretanje tela ka input smeru
                float maxRadians = turnSpeedDegrees * Mathf.Deg2Rad * Time.fixedDeltaTime;
                _moveDir = Vector3.RotateTowards(_moveDir, _inputDir, maxRadians, 0f);
                _moveDir.y = 0f;
                if (_moveDir.sqrMagnitude > 0.0001f)
                    _moveDir.Normalize();

                // Ubrzavanje od 0 ka moveSpeed
                _currentSpeed = Mathf.MoveTowards(
                    _currentSpeed,
                    moveSpeed,
                    acceleration * Time.fixedDeltaTime
                );
            }
            else
            {
                // Nema inputa → stoji, nema klizanja
                _currentSpeed = 0f;
            }

            Vector3 horizVel = _moveDir * _currentSpeed;
            _rb.linearVelocity = new Vector3(horizVel.x, _rb.linearVelocity.y, horizVel.z);
        }

        // --- ROTACIJA U SMERU KRETANJA ---

        Vector3 horizontalVelocity = new Vector3(_rb.linearVelocity.x, 0f, _rb.linearVelocity.z);

        if (horizontalVelocity.sqrMagnitude > 0.0001f)
        {
            Quaternion targetRot = Quaternion.LookRotation(horizontalVelocity.normalized, Vector3.up);
            transform.rotation = Quaternion.Slerp(
                transform.rotation,
                targetRot,
                Time.deltaTime * 10f
            );
        }
    }
}
