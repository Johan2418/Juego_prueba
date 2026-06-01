using UnityEngine;

public class CameraFollow2D : MonoBehaviour
{
    [SerializeField] private Transform target;
    [SerializeField] private Vector3 offset = new Vector3(0f, 0f, -10f);
    [SerializeField] private float smoothSpeed = 10f;

    private void Start()
    {
        SnapToTarget();
    }

    private void LateUpdate()
    {
        if (target == null)
        {
            return;
        }

        Vector3 targetPosition = target.position + offset;
        targetPosition.z = offset.z;

        if (smoothSpeed <= 0f)
        {
            transform.position = targetPosition;
            return;
        }

        transform.position = Vector3.Lerp(transform.position, targetPosition, smoothSpeed * Time.deltaTime);
    }

    public void SnapToTarget()
    {
        if (target == null)
        {
            return;
        }

        Vector3 targetPosition = target.position + offset;
        targetPosition.z = offset.z;
        transform.position = targetPosition;
    }

    private void OnValidate()
    {
        smoothSpeed = Mathf.Max(0f, smoothSpeed);
    }
}
