using System.Xml;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform target;        // The target the camera will follow and look at
    public Vector3 offset = new Vector3(-0.2f, 0.2f, 0f); // Camera offset from the target
    public float rotate_camera = -0.2f;
    public float followSpeed = 1f;  // Speed at which camera moves
    public float lookSpeed = 1f;    // Speed at which camera rotates to look at the target


    private Quaternion CAMrotation;

    private void Start()
    {
        CAMrotation = new Quaternion(rotate_camera, 0f, 0f, 1f);
    }
    void LateUpdate()
    {
        if (target == null) return;

        // Smoothly move the camera to the target position + offset
        Vector3 desiredPosition = target.position + offset;
        transform.position = Vector3.Lerp(transform.position, desiredPosition, followSpeed * Time.deltaTime);       

        // Smoothly rotate to look at the target
        Quaternion desiredRotation = Quaternion.LookRotation(target.position - transform.position);
        desiredRotation *= CAMrotation;
        transform.rotation = Quaternion.Slerp(transform.rotation, desiredRotation, lookSpeed * Time.deltaTime);
    }
}
