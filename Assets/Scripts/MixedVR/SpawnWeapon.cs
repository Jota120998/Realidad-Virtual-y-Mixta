using UnityEngine;

public class SpawnWeapon : MonoBehaviour
{
    public Transform xrOrigin; // XR Camra del jugador
    public float distance = 2f;
    public float height = -0.3f;

    public bool lookAtPlayer = true;

    void Start()
    {
        MoveWeapon();
    }

    public void MoveWeapon()
    {
       
        Vector3 forwardDir = xrOrigin.forward;
    forwardDir.y = 0;
    forwardDir.Normalize();

    transform.position =
        xrOrigin.position +
        forwardDir * distance +
        Vector3.up * height;

    if (lookAtPlayer)
    {
        transform.LookAt(xrOrigin);
    }
    else
    {
        Vector3 right = forwardDir;
        Vector3 forward = Vector3.up;

        Vector3 up = Vector3.Cross(forward, right);

        transform.rotation = Quaternion.LookRotation(forward, up);
    }

    }
}   