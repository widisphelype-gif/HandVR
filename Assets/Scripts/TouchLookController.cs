using UnityEngine;

namespace HandVR
{
    // Controlo de visão alternativo para aparelhos sem giroscópio.
    // Arraste na tela para olhar ao redor. Não é head tracking real.
    public class TouchLookController : MonoBehaviour
    {
        [SerializeField] private Transform target;
        [SerializeField] private float sensitivity = 0.12f;
        [SerializeField] private float maxPitch = 80f;

        private Vector2 lastPosition;
        private float yaw;
        private float pitch;

        private void Awake()
        {
            if (target == null) target = transform;
            Vector3 angles = target.localEulerAngles;
            yaw = angles.y;
            pitch = angles.x;
        }

        private void Update()
        {
            if (Input.touchCount > 0)
            {
                Touch touch = Input.GetTouch(0);
                if (touch.phase == TouchPhase.Began) lastPosition = touch.position;
                else if (touch.phase == TouchPhase.Moved)
                {
                    Vector2 delta = touch.position - lastPosition;
                    lastPosition = touch.position;
                    ApplyLook(delta);
                }
            }
            else if (Input.GetMouseButtonDown(0))
                lastPosition = Input.mousePosition;
            else if (Input.GetMouseButton(0))
            {
                Vector2 current = Input.mousePosition;
                ApplyLook(current - lastPosition);
                lastPosition = current;
            }
        }

        private void ApplyLook(Vector2 delta)
        {
            yaw += delta.x * sensitivity;
            pitch -= delta.y * sensitivity;
            pitch = Mathf.Clamp(pitch, -maxPitch, maxPitch);
            target.localRotation = Quaternion.Euler(pitch, yaw, 0f);
        }
    }
}
