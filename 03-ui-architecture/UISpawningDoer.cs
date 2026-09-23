/*
 * UI SPAWNING SERVICE — DESIGN NOTES
 * -----------------------------------
 * Owns instantiation, dependency injection, and lifecycle for every top-level UI
 * panel in Combat Climber via a small IUISpawnable contract, rather than one
 * hardcoded field/method pair per screen (see UISpawnableBase + concrete panels
 * like MainMenuUISpawnable).
 *
 * A couple of calls worth flagging, since they're easy to misread as oversights:
 *
 * - INSTANTIATE/DESTROY, NOT POOLED: these are top-level flow screens (main menu,
 *   mode select, wall editing, try-wall) opened a handful of times per session at
 *   natural transition points, not toggled rapidly like a HUD element. The
 *   GC/instantiation cost here is negligible. I'd reach for pooling on anything
 *   opened and closed at higher frequency — a combo popup, a damage number, a
 *   settings drawer a player might flick open and shut repeatedly.
 *
 * - PER-SCREEN Inject() SIGNATURES, NOT A GENERIC FACTORY: each panel takes
 *   genuinely different dependencies (GameManagerScript, PracticeGameModeFlow,
 *   ModuleInteractionSystem + a couple of Evts). A generic object[]-args factory
 *   would trade away compile-time type safety for flexibility this panel count
 *   doesn't need. I'd revisit that tradeoff if the panel count or the variety of
 *   dependencies grew a lot.
 *
 * - RegisterUISpawnable guards against re-registering an already-open panel type,
 *   so calling Spawn twice on the same panel can't silently orphan the first
 *   instance in the scene.
 */

using System.Collections.Generic;
using UnityEngine;

public enum UIPanelType { MainMenu, GameModeSelect, Joystick, WallEditing, TryWall, Settings, Countdown}

public interface IUISpawnable
{
    public UIPanelType GetUIPanelType();
    public void InitializeCanvas(Camera cameraForCanvas, string sortingLayerName);
    public void EnterPanel();
    public void ClosePanel();
    public GameObject GetObjectToDestroy();
}


public class UISpawningDoer : Doer
{
    [SerializeField] GameObject mainMenuCanvas;
    [SerializeField] GameObject gameModeSelectionCanvas;
    [SerializeField] GameObject joystickCanvas;
    [SerializeField] GameObject moduleSelectionCanvas;
    [SerializeField] GameObject tryWallCanvas;

    Dictionary<UIPanelType, IUISpawnable> currentlySpawnedUIPrefabs = new Dictionary<UIPanelType, IUISpawnable>();

    Camera cachedCamera;
    string cachedUISortingLayerName = "UI";

    private void Awake()
    {
        cachedCamera = Camera.main;
    }

    public void SpawnMainMenuCanvas(GameManagerScript gameManagerScript)
    {
        MainMenuUISpawnable mainMenu = Instantiate(mainMenuCanvas, null).GetComponent<MainMenuUISpawnable>();
        mainMenu.Inject(gameManagerScript);
        RegisterUISpawnable(mainMenu);

    }
    public void SpawnGameModeSelectionCanvas(GameManagerScript gameManagerScript)
    {
        GameModeSelectionUISpawnable gameModeSelection = Instantiate(gameModeSelectionCanvas, null).GetComponent<GameModeSelectionUISpawnable>();
        gameModeSelection.Inject(gameManagerScript);
        RegisterUISpawnable(gameModeSelection);

    }
    public void SpawnTryWallCanvas(PracticeGameModeFlow practiceGameModeFlow)
    {
        TryWallUISpawnable tryWall = Instantiate(tryWallCanvas, null).GetComponent<TryWallUISpawnable>();
        tryWall.Inject(practiceGameModeFlow);
        RegisterUISpawnable(tryWall);

    }
    public void SpawnJoystickCanvas(PlayerInputDoer playerInputDoer)
    {
        JoystickInput joystick = Instantiate(joystickCanvas, null).GetComponent<JoystickInput>();
        joystick.Inject(playerInputDoer);
        RegisterUISpawnable(joystick);

    }

    public void SpawnModuleSelectionCanvas(ModuleInteractionSystem moduleInteractionSystem, Evt startTryingWallPressedEvt, Evt exitPracticeModeBtnPressed)
    {
        ModuleSelectionCanvasScript moduleSelection = Instantiate(moduleSelectionCanvas, null).GetComponent<ModuleSelectionCanvasScript>();
        moduleSelection.Inject(moduleInteractionSystem, startTryingWallPressedEvt,exitPracticeModeBtnPressed);
        RegisterUISpawnable(moduleSelection);
    }

    public void RegisterUISpawnable(IUISpawnable uiSpawnable)
    {
        UIPanelType panelType = uiSpawnable.GetUIPanelType();

        if (currentlySpawnedUIPrefabs.ContainsKey(panelType) && currentlySpawnedUIPrefabs[panelType] != null)
        {
            Destroy(uiSpawnable.GetObjectToDestroy());
            return;
        }

        currentlySpawnedUIPrefabs[panelType] = uiSpawnable;

        uiSpawnable.InitializeCanvas(cachedCamera, cachedUISortingLayerName);
        uiSpawnable.EnterPanel();
    }

    public void GenericCloseUIElement(UIPanelType uiPanelType)
    {
        if (!currentlySpawnedUIPrefabs.ContainsKey(uiPanelType))
        {
            Debug.LogError("ERROR: Ui panel " + uiPanelType.ToString() + " isn't yet spawned");
            return;
        }

        currentlySpawnedUIPrefabs[uiPanelType].ClosePanel();
        Destroy(currentlySpawnedUIPrefabs[uiPanelType].GetObjectToDestroy());
    }
}
