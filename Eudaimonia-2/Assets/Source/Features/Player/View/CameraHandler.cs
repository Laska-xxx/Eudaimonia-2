using Core;
using UnityEngine;
using UnityEngine.InputSystem;
using Zenject;

public class CameraHandler : MonoBehaviour
{
    [SerializeField] private float mouseSensitivity = 10f;
    [SerializeField] private Transform playerBody;

    private float _xRotation = 0f;
    private InputManager _inputManager;
    private InputAction _mouseLookAction;

    [Inject]
    private void Init(InputManager inputManager)
    {
        _inputManager = inputManager;

        _mouseLookAction = _inputManager.GameInput.Player.Look;
    }

    private void OnDisable()
    {
        _mouseLookAction = null;
    }

    void Update()
    {
        Vector2 lookVector = _mouseLookAction.ReadValue<Vector2>();

        _xRotation -= lookVector.y * mouseSensitivity;
        _xRotation = Mathf.Clamp(_xRotation, -80f, 50f);

        transform.localRotation = Quaternion.Euler(_xRotation, 0f, 0f);
        playerBody.Rotate(Vector3.up * lookVector.x * mouseSensitivity);
    }
}