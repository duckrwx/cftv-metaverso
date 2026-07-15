using UnityEngine;
#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

namespace SalaSegurancaVR
{
    /// <summary>
    /// Controlador auxiliar de navegacao por teclado e mouse para testar a
    /// cena no Unity Editor quando nao ha headset VR disponivel.
    /// Utiliza o novo Input System (com.unity.inputsystem).
    ///
    /// Controles:
    ///   W / A / S / D  -> mover pela sala
    ///   Mouse          -> olhar ao redor
    ///   Espaco / Ctrl  -> subir / descer
    ///   Shift          -> mover mais rapido
    ///   ESC            -> liberar / travar o cursor
    /// </summary>
    [AddComponentMenu("Sala Seguranca VR/Keyboard Mouse Controller")]
    public class PCPlayerController : MonoBehaviour
    {
        [Header("Movimento")]
        public float moveSpeed = 3.5f;
        public float sprintMultiplier = 2f;

        [Header("Camera")]
        public float mouseSensitivity = 0.12f;
        public Transform cameraTransform;

        private float _pitch;
        private bool _looking = true;

        private void Start()
        {
            if (cameraTransform == null && Camera.main != null)
                cameraTransform = Camera.main.transform;
            LockCursor(true);
        }

        private void Update()
        {
#if ENABLE_INPUT_SYSTEM
            Keyboard kb = Keyboard.current;
            Mouse mouse = Mouse.current;
            if (kb == null) return;

            if (kb.escapeKey.wasPressedThisFrame)
                LockCursor(!_looking);

            // Olhar com o mouse.
            if (_looking && mouse != null)
            {
                Vector2 d = mouse.delta.ReadValue() * mouseSensitivity;
                transform.Rotate(Vector3.up, d.x, Space.World);
                _pitch = Mathf.Clamp(_pitch - d.y, -89f, 89f);
                if (cameraTransform != null)
                    cameraTransform.localRotation = Quaternion.Euler(_pitch, 0f, 0f);
            }

            // Movimento WASD + subir/descer.
            Vector3 dir = Vector3.zero;
            if (kb.wKey.isPressed) dir += transform.forward;
            if (kb.sKey.isPressed) dir -= transform.forward;
            if (kb.dKey.isPressed) dir += transform.right;
            if (kb.aKey.isPressed) dir -= transform.right;
            if (kb.spaceKey.isPressed) dir += Vector3.up;
            if (kb.leftCtrlKey.isPressed) dir -= Vector3.up;

            float speed = moveSpeed * (kb.leftShiftKey.isPressed ? sprintMultiplier : 1f);
            transform.position += dir.normalized * (speed * Time.deltaTime);
#endif
        }

        private void LockCursor(bool locked)
        {
            _looking = locked;
            Cursor.lockState = locked ? CursorLockMode.Locked : CursorLockMode.None;
            Cursor.visible = !locked;
        }
    }
}
