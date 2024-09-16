using UnityEngine;

public class Player : MonoBehaviour
{
    public delegate void Die();
    public event Die OnDead;

    public void Kill()
    {
        OnDead.Invoke();
    }
}
