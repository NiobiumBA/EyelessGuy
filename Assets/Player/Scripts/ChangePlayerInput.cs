using UnityEngine;

public class ChangePlayerInput : MonoBehaviour
{
    [SerializeField] private PlayerController m_controller;
    [SerializeField] private KeyCode m_changeKey = KeyCode.Escape;

    private bool m_controllerEnable;

    public bool ControllerEnable
    {
        get => m_controllerEnable;
        set
        {
            m_controllerEnable = value;

            m_controller.CanMove = value;
            m_controller.CanRotate = value;
            Cursor.lockState = value ? CursorLockMode.Locked : CursorLockMode.None;
        }
    }

    private void Start()
    {
        ControllerEnable = true;
    }

    private void Update()
    {
        if (Input.GetKeyDown(m_changeKey))
            ControllerEnable = !ControllerEnable;
    }
}
