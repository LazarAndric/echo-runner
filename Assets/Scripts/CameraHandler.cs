using UnityEngine;

public class CameraHandler : MonoBehaviour
{
    public Transform FollowObject;
    public Vector3 Offset;
    public float power;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Offset= FollowObject.position-transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = Vector3.Lerp(transform.position, FollowObject.position - Offset, Time.deltaTime * power); 
    }
}
