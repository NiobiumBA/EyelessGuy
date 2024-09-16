using UnityEngine;

public class LoseUIActivator : MonoBehaviour
{
    [SerializeField] private Player m_player;
    [SerializeField] private GameObject m_loseUI;

    private void OnEnable()
    {
        m_player.OnDead += ActivateUI;
    }

    private void OnDisable()
    {
        m_player.OnDead -= ActivateUI;
    }

    private void ActivateUI()
    {
        m_loseUI.SetActive(true);
    }
}
