using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour
{
    private const string HorizontalAxisName = "Horizontal";
    private const string VerticalAxisName = "Vertical";
    private const string MouseXAxisName = "Mouse X";
    private const string MouseYAxisName = "Mouse Y";

    [SerializeField] private Transform m_camera;
    [SerializeField] private float m_speed;
    [SerializeField] private float m_sensitivity;
    [SerializeField] private bool m_canMove = true;
    [SerializeField] private bool m_canRotate = true;

    private Vector3 m_eulerAngles;
    private CharacterController m_characterController;

    public Vector3 EulerAngles
    {
        get => m_eulerAngles;
        set
        {
            m_eulerAngles = new Vector3(Mathf.Clamp(value.x, -90, 90), value.y % 360, value.z % 360);
        }
    }

    public float Sensitivity
    {
        get => m_sensitivity;
        set
        {
            if (value == 0)
            {
                m_canRotate = false;
                return;
            }

            m_sensitivity = Mathf.Max(value, 0);
        }
    }

    public float Speed
    {
        get => m_speed;
        set
        {
            if (value == 0)
            {
                m_canMove = false;
                return;
            }

            m_speed = Mathf.Max(value, 0);
        }
    }

    public bool CanRotate { get => m_canRotate; set => m_canRotate = value; }

    public bool CanMove { get => m_canMove; set => m_canMove = value; }

    private void Start()
    {
        m_eulerAngles = transform.localEulerAngles;
        m_characterController = GetComponent<CharacterController>();
    }

    private void Update()
    {
        if (CanMove)
            Movement();
    }

    private void LateUpdate()
    {
        if (CanRotate)
            Rotate();
    }

    private void Movement()
    {
        float vertical = Input.GetAxis(VerticalAxisName);
        float horizontal = Input.GetAxis(HorizontalAxisName);

        Vector3 move = m_speed * Time.deltaTime * GamePause.TimeScale * new Vector3(horizontal, 0, vertical);

        Vector3 worldMove = transform.TransformDirection(move);

        m_characterController.Move(worldMove);
    }

    private void Rotate()
    {
        float deltaMouseX = Input.GetAxis(MouseXAxisName);
        float deltaMouseY = Input.GetAxis(MouseYAxisName);

        EulerAngles += m_sensitivity * GamePause.TimeScale * new Vector3(-deltaMouseY, deltaMouseX, 0);

        transform.localEulerAngles = new Vector3(0, EulerAngles.y, 0);
        m_camera.localEulerAngles = new Vector3(EulerAngles.x, 0, EulerAngles.z);
    }
}
