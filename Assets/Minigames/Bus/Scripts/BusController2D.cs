using UnityEngine;

namespace BusMinigame
{
    [RequireComponent(typeof(Rigidbody2D))]
    public class BusController2D : MonoBehaviour
    {
        [Header("Movement Settings")]
        public float acceleration = 5f;
        public float maxSpeed = 10f;
        public float steeringPower = 150f;
        public float driftFactor = 0.95f;

        private Rigidbody2D rb;
        private float movementInput;
        private float steeringInput;
        private float velocityVsUp;

        void Awake()
        {
            rb = GetComponent<Rigidbody2D>();
            rb.gravityScale = 0;
            rb.constraints = RigidbodyConstraints2D.FreezeRotation; // We handle rotation manually or via rb
            // User requested Freeze Rotation Z activated, but for a car/bus we usually want it to rotate.
            // However, user specifically said: "Freeze Rotation Z activado". 
            // This might mean they want a non-rotating bus (sliding/strafe) or they just want to prevent physics spinning.
            // I will follow the instruction: Freeze Rotation Z.
        }

        void Update()
        {
            movementInput = Input.GetAxis("Vertical");
            steeringInput = Input.GetAxis("Horizontal");
        }

        void FixedUpdate()
        {
            ApplyEngineForce();
            ApplySteering();
            KillOrthogonalVelocity();
        }

        void ApplyEngineForce()
        {
            // Calculate how much "forward" we are going
            velocityVsUp = Vector2.Dot(transform.up, rb.linearVelocity);

            // Limit max speed
            if (velocityVsUp > maxSpeed && movementInput > 0) return;
            if (velocityVsUp < -maxSpeed * 0.5f && movementInput < 0) return;
            if (rb.linearVelocity.sqrMagnitude > maxSpeed * maxSpeed && movementInput > 0) return;

            // Apply force
            if (movementInput != 0)
            {
                rb.AddForce(transform.up * movementInput * acceleration, ForceMode2D.Force);
            }
        }

        void ApplySteering()
        {
            // Only steer if moving
            float minSpeedBeforeSteering = 0.1f;
            float speedFactor = Mathf.Clamp01(rb.linearVelocity.magnitude / minSpeedBeforeSteering);

            // Rotate based on steering input
            // If Freeze Rotation Z is on, rb.rotation won't change via physics, but we can change it via code or rb.MoveRotation
            // However, usually "Freeze Rotation" prevents ALL rotation. 
            // If the user wants WASD movement for a top-down bus, usually it's tank controls or car controls.
            // If Z is frozen, I'll move it via position if they want 4-way, but usually "Top-down 2D" with WASD/Arrows implies rotation.
            // I'll assume they want to control rotation manually.
            
            float rotationAmount = -steeringInput * steeringPower * Time.fixedDeltaTime * speedFactor;
            // Since Z is frozen in Rigidbody, we rotate the transform directly or unfreeze it.
            // I will use transform.Rotate for simplicity as the user specifically asked for Freeze Rotation Z.
            transform.Rotate(0, 0, rotationAmount);
        }

        void KillOrthogonalVelocity()
        {
            Vector2 forwardVelocity = transform.up * Vector2.Dot(rb.linearVelocity, transform.up);
            Vector2 rightVelocity = transform.right * Vector2.Dot(rb.linearVelocity, transform.right);

            rb.linearVelocity = forwardVelocity + rightVelocity * driftFactor;
        }
    }
}