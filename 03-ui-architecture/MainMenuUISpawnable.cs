using UnityEngine;

public class MainMenuUISpawnable : UISpawnableBase
{
    GameManagerScript gameManager;

    public override UIPanelType GetUIPanelType() => UIPanelType.MainMenu;

    public void Inject(GameManagerScript gameManager)
    {
        this.gameManager = gameManager;
    }

    public void PlayBtnPressed()
    {
        gameManager.PlayButtonPressed();
    }


}
