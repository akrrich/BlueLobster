public class UpdateManager
{
    public void UpdateAllGame()
    {
        if (GameManager.Instance.GameState == GameState.Playing)
        {
            GameManager.Instance.OnGameStatePlaying?.Invoke();
        }
    }

    public void FixedUpdateAllGame()
    {
        if (GameManager.Instance.GameState == GameState.Playing)
        {
            GameManager.Instance.OnGameStatePlayingFixedUpdate?.Invoke();
        }
    }
}
