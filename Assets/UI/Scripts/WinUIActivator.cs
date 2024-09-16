using UnityEngine;

public class WinUIActivator : MonoBehaviour
{
    [SerializeField] private EscapeMazeChecker m_escapeChecker;
    [SerializeField] private GameObject m_winUI;

    private void OnEnable()
    {
        m_escapeChecker.OnEscapeMaze += UIActivate;
    }

    private void OnDisable()
    {
        m_escapeChecker.OnEscapeMaze -= UIActivate;
    }

    private void UIActivate()
    {
        m_winUI.SetActive(true);
    }
}
