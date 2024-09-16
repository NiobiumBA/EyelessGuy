using UnityEngine;

public class PauseInputChanger : MonoBehaviour
{
    [SerializeField] private GameObject m_menuUI;
    [SerializeField] private KeyCode m_pauseKey = KeyCode.Escape;

    private bool m_isPaused = false;

    public bool IsPaused
    {
        get => m_isPaused;
        set
        {
            m_isPaused = value;

            GamePause.IsPaused = m_isPaused;
            Cursor.lockState = m_isPaused ? CursorLockMode.None : CursorLockMode.Locked;
            m_menuUI.SetActive(m_isPaused);
            AudioMixerSystem.SetVolume(AudioMixerSystem.AudioGroup.GameLogic, m_isPaused ? 0 : 1);
        }
    }

    private void Start()
    {
        IsPaused = false;
    }

    private void Update()
    {
        if (Input.GetKeyUp(m_pauseKey))
        {
            IsPaused = !IsPaused;
        }
    }
}
