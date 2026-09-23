using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D;
using Sirenix.OdinInspector;

public class TogglableEffect : Effect
{
    [SerializeField] bool startEnabled;

    protected override void OnStart()
    {
        SetFXApperence(startEnabled);
    }
    [HorizontalGroup("Split", 0.5f)]
    [Button("Enable Effect", ButtonSizes.Large), GUIColor(0.4f, 0.8f, 1)]
    public void EnableEffect() => SetFXApperence(true);

    [HorizontalGroup("Split", 0.5f)]
    [Button("Disable Effect", ButtonSizes.Large), GUIColor(0.8f, 0.3f, 0.6f)]
    public void DisableEffect() => SetFXApperence(false);
    public void SetEffect(bool onOrOff) {
        if (onOrOff) EnableEffect();
        else DisableEffect();
    }
}
