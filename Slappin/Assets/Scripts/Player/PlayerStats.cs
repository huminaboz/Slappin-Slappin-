
public class PlayerStats : Singleton<PlayerStats>
{
    private int currency1;

    
    private void Start()
    {
        UpdateHUD();
    }

    public void AddCurency(int amount)
    {
        currency1 += amount;
        UpdateHUD();
        //todo:: Animate the number to show it went up
    }

    private void UpdateHUD()
    {
        GameplayUIManager.I.currency1.text = currency1.ToString();
    }
    
}
