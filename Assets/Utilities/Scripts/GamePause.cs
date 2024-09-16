public static class GamePause
{
    private static bool m_paused = false;

    public static bool IsPaused
    {
        get => m_paused;
        set => m_paused = value;
    }

    public static float TimeScale => m_paused ? 0.0f : 1.0f;
}
