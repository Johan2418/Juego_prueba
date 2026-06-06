using UnityEngine;

public class SecretBossCameraFollow : MonoBehaviour
{
    [Header("Target")]
    public Transform target;
    
    [Header("Settings")]
    public Vector3 offset = new Vector3(0, 0, -10);
    [Range(0.01f, 10f)]
    public float smoothSpeed = 5f;

    private void Start()
    {
        if (target == null)
        {
            GameObject player = GameObject.FindWithTag("Player");
            if (player != null)
            {
                target = player.transform;
            }
        }
    }

    private void LateUpdate()
    {
        if (target == null) return;

        Vector3 desiredPosition = target.position + offset;
        // Maintain the offset's Z specifically if the target is in 2D space (Z=0 usually)
        desiredPosition.z = offset.z;

        Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed * Time.deltaTime);
        transform.position = smoothedPosition;
    }
}
