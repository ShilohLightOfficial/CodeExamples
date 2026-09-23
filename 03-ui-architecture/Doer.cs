using UnityEngine;

public class Doer : MonoBehaviour
{
    protected IWriteableGameState WriteGameState;
    protected GameManagerScript gameManager;
    protected GameModeFlow activeGameModeFlow;

    public void Initialize(GameManagerScript _gameManager,IWriteableGameState writeableGameState, GameModeFlow ActiveGameModeFlow)
    {
        this.activeGameModeFlow = ActiveGameModeFlow;
        this.gameManager = _gameManager;
        this.WriteGameState = writeableGameState;
    }
}
