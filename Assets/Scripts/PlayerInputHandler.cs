using UnityEngine;

public class PlayerInputHandler : MonoBehaviour
{
    Vector3 direction= Vector3.zero;
    bool isDirectionChanged;
    public float power;
    private void Update()
    {
        direction = Vector3.zero;
        if (Input.GetKey(KeyCode.RightArrow))
        {
            isDirectionChanged = true;
            direction.x = 1;
        }
        else if (Input.GetKey(KeyCode.LeftArrow))
        {
            isDirectionChanged = true;
            direction.x = -1;
        }
        if (Input.GetKey(KeyCode.UpArrow))
        {
            isDirectionChanged = true;
            direction.z = 1;
        }
        else if (Input.GetKey(KeyCode.DownArrow))
        {
            isDirectionChanged = true;
            direction.z = -1;
        }
        if (isDirectionChanged)
        {
            Vector3 desire = transform.position + Vector3.Normalize( direction)* power;
            transform.position = Vector3.Lerp(transform.position, desire, Time.deltaTime);
        }
    }
}
