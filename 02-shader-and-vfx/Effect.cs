using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D;

//THIS SCRIPT IS A SMALL REUSABLE SCRIPT TO SHOW AND HIDE GROUPS OF PARTICLES AND SWAP MATERIALS ON SPRITES, DESIGNED FOR USE ON FX
public class Effect : MonoBehaviour
{
    static readonly int EnabledHash = Animator.StringToHash("Enabled");

    [SerializeField] public ParticleSystem[] particleSystems;
    [SerializeField] public SpriteRenderer[] spriteRenderersToMaterialSwap;
    [SerializeField] public SpriteShapeRenderer[] spriteShapeRenderersToMaterialSwap;
    [SerializeField] public Animator[] animatorsToSetBoolEnabled;
    [Space]
    [SerializeField] Material fxMaterial;

    Material[] effectedSpriteRenderersOriginalMaterials;
    Material[] effectedSpriteShapeRenderersOriginalMaterials;

    bool desiredAppearance;
    bool hasStarted;

    private void Start()
    {
        hasStarted = true;

        //INITALIZE ARRAY
        effectedSpriteRenderersOriginalMaterials = new Material[spriteRenderersToMaterialSwap.Length];
        effectedSpriteShapeRenderersOriginalMaterials = new Material[spriteShapeRenderersToMaterialSwap.Length];

        //SAVE ALL THE ORIGINAL MATERIALS
        for (int i = 0; i < spriteRenderersToMaterialSwap.Length; i++)
        {
            effectedSpriteRenderersOriginalMaterials[i] = spriteRenderersToMaterialSwap[i].material;
        }

        //SAVE ALL THE ORIGINAL MATERIALS
        for (int i = 0; i < spriteShapeRenderersToMaterialSwap.Length; i++)
        {
            effectedSpriteShapeRenderersOriginalMaterials[i] = spriteShapeRenderersToMaterialSwap[i].materials[1];
        }

        CheckForAnyNullRefs();

        OnStart();
    }

    protected virtual void OnStart()
    {
        //APPLY WHATEVER STATE WAS REQUESTED BEFORE START RAN (DEFAULTS TO OFF)
        SetFXApperence(desiredAppearance);
    }

    public void SetReferences(EffectVisualReferences visualReferences)
    {
        if (visualReferences.spriteRenderersToMaterialChange != null) spriteRenderersToMaterialSwap = visualReferences.spriteRenderersToMaterialChange;
        if (visualReferences.spriteShapeRenderersToMaterialChange != null) spriteShapeRenderersToMaterialSwap = visualReferences.spriteShapeRenderersToMaterialChange;
        if (visualReferences.particleSystemsToToggle != null) particleSystems = visualReferences.particleSystemsToToggle;
        if (visualReferences.animatorsToSetBoolEnabled != null) animatorsToSetBoolEnabled = visualReferences.animatorsToSetBoolEnabled;
    }

    public void SetFXApperence(bool onOrOff)
    {
        desiredAppearance = onOrOff;

        //IF START HASNT SAVED THE ORIGINAL MATERIALS YET, JUST REMEMBER THE STATE AND APPLY IT IN OnStart
        if (!hasStarted) return;

        //ENABLE ALL PARTICLES
        foreach(ParticleSystem ps in particleSystems)
        {
            var emmisionModule = ps.emission;
            emmisionModule.enabled = onOrOff;
        }

        //SWAP ALL MATERIALS
        for(int i = 0; i < spriteRenderersToMaterialSwap.Length; i++)
        {
            if (onOrOff) spriteRenderersToMaterialSwap[i].material = fxMaterial;
            else spriteRenderersToMaterialSwap[i].material = effectedSpriteRenderersOriginalMaterials[i];
        }
        //SWAP ALL MATERIALS
        for (int i = 0; i < spriteShapeRenderersToMaterialSwap.Length; i++)
        {
            // Get the current materials from the SpriteShapeRenderer
            Material[] currentMaterials = spriteShapeRenderersToMaterialSwap[i].materials;

            // Create a new array for the modified materials
            Material[] modifiedMaterials = new Material[currentMaterials.Length];

            // Copy the current materials to the modified materials array
            currentMaterials.CopyTo(modifiedMaterials, 0);

            // Modify the material you want
            modifiedMaterials[1] = onOrOff ? fxMaterial : effectedSpriteShapeRenderersOriginalMaterials[i];

            // Assign the modified materials array back to the SpriteShapeRenderer
            spriteShapeRenderersToMaterialSwap[i].materials = modifiedMaterials;
        }

        foreach (Animator anim in animatorsToSetBoolEnabled)
        {
            anim.SetBool(EnabledHash, onOrOff);
        }

        OnEffectStatusChanged(onOrOff);
    }
    protected virtual void OnEffectStatusChanged(bool onOrOff)
    {
        //CHILDREN CAN INHERIT THIS TO DO SPECIFIC THINGS
    }
    public void TurnOffAndDestroy()
    {
        SetFXApperence(false);
        Destroy(gameObject, 0.3f);
    }
    private void CheckForAnyNullRefs()
    {
        if (particleSystems != null) foreach (ParticleSystem ps in particleSystems) if (ps == null) Debug.LogError("ToggleableEffect has null particleSystem " + name);
        if (spriteRenderersToMaterialSwap != null) foreach (SpriteRenderer ps in spriteRenderersToMaterialSwap) if (ps == null) Debug.LogError("ToggleableEffect has null spriteRenderer " + name);
        if (spriteShapeRenderersToMaterialSwap != null) foreach (SpriteShapeRenderer ps in spriteShapeRenderersToMaterialSwap) if (ps == null) Debug.LogError("ToggleableEffect has null spriteShapeRenderer " + name);
        if (animatorsToSetBoolEnabled != null) foreach (Animator ps in animatorsToSetBoolEnabled) if (ps == null) Debug.LogError("ToggleableEffect has null animator " + name);
        if (spriteRenderersToMaterialSwap.Length > 0 || spriteShapeRenderersToMaterialSwap.Length > 0) if (fxMaterial == null) Debug.LogError("ToggleableEffect has null fxMaterial " + name);
    }
}

[System.Serializable]
public class EffectVisualReferences
{
    public SpriteRenderer[] spriteRenderersToMaterialChange;
    public SpriteShapeRenderer[] spriteShapeRenderersToMaterialChange;
    public ParticleSystem[] particleSystemsToToggle;
    public Animator[] animatorsToSetBoolEnabled;
    public float particlesYShapeSize;
    public float particlesXShapeSize;
    public Transform particlesParent;
}
