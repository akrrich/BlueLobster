public class UpdateManager
{
    public void UpdateGameInMenuState()
    {
        if (GameManager.Instance.GameState == GameState.Menu)
        {
            GameManager.Instance.OnGameStateMenu?.Invoke();
        }
    }

    public void UpdateAGameInPlayingState()
    {
        if (GameManager.Instance.GameState == GameState.Playing)
        {
            GameManager.Instance.OnGameStatePlaying?.Invoke();
        }
    }

    public void FixedUpdateGameInPlayingState()
    {
        if (GameManager.Instance.GameState == GameState.Playing)
        {
            GameManager.Instance.OnGameStatePlayingFixedUpdate?.Invoke();
        }
    }
}
