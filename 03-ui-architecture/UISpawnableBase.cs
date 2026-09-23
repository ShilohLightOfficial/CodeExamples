using UnityEngine;
using UnityEngine.UI;

public abstract class UISpawnableBase : MonoBehaviour, IUISpawnable
{
    protected GameModeFlow ActiveGameModeFlow;

    public void InitializeCanvas(Camera cameraForCanvas, string sortingLayerName)
    {
        Canvas canvas = GetComponent<Canvas>();
        canvas.worldCamera = cameraForCanvas;
        canvas.sortingLayerName = sortingLayerName;
    }

    public virtual void ClosePanel()
    {
        gameObject.SetActive(false);
    }

    public virtual void EnterPanel()
    {
        gameObject.SetActive(true);
    }

    public GameObject GetPrefab() => gameObject;


    public abstract UIPanelType GetUIPanelType();

    public GameObject GetObjectToDestroy() => gameObject;
}
