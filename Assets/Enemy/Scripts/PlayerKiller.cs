using UnityEngine;

public class PlayerKiller : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent<Player>(out var player))
            player.Kill();
    }
}
