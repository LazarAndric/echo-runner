using UnityEngine;

public class CameraHandler : MonoBehaviour
{
    [Header("Target")]
    public Transform target;          // koga pratimo

    [Header("Offset (treće lice)")]
    public Vector3 cameraOffset = new Vector3(0f, 8f, -10f);

    [Header("Dead zone (buffer) u world jedinicama")]
    public float deadZoneWidth = 4f;   // levo-desno od fokusa
    public float deadZoneDepth = 4f;   // napred-nazad od fokusa

    private Vector3 _focusPoint;       // tačka koju kamera gleda

    void Start()
    {
        if (target != null)
        {
            _focusPoint = target.position;
        }
        else
        {
            _focusPoint = transform.position - cameraOffset;
        }
    }

    void LateUpdate()
    {
        if (target == null)
            return;

        Vector3 targetPos = target.position;
        Vector3 fp = _focusPoint;

        float halfW = deadZoneWidth * 0.5f;
        float halfD = deadZoneDepth * 0.5f;

        // RAČUNAMO RAZLIKU U XZ RAVNI
        float dx = targetPos.x - fp.x;
        float dz = targetPos.z - fp.z;

        // Ako je target izašao iz dead zone po X, pomeramo fokus TAČNO do ivice
        if (dx > halfW)
            fp.x = targetPos.x - halfW;
        else if (dx < -halfW)
            fp.x = targetPos.x + halfW;

        // Ako je target izašao iz dead zone po Z, isto to
        if (dz > halfD)
            fp.z = targetPos.z - halfD;
        else if (dz < -halfD)
            fp.z = targetPos.z + halfD;

        _focusPoint = fp;

        // POZICIJA KAMERE = FOKUS + OFFSET
        Vector3 camPos = _focusPoint + cameraOffset;
        transform.position = camPos;

        // KAMERA GLEDA U FOKUSNU TAČKU
        transform.LookAt(_focusPoint);
    }

    public void SetTarget(Transform newTarget, bool snapInstantly = false)
    {
        target = newTarget;
        if (target == null) return;

        if (snapInstantly)
        {
            _focusPoint = target.position;
            transform.position = _focusPoint + cameraOffset;
            transform.LookAt(_focusPoint);
        }
        else
        {
            // bez specijalne logike – dead zone će prirodno "uhvatiti" novi target
            _focusPoint = target.position;
        }
    }
}
