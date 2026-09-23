using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class VFXService : MonoBehaviour
{
    static readonly int LifetimeID = Shader.PropertyToID("_lifetime");

    [SerializeField] GameObject landingVFXPrefab;

    [SerializeField] Effect spawnedChargeUpVFX;
    [SerializeField] GameObject centralWhiteFlashVFX;
    [SerializeField] private ScriptableRendererFeature[] shockwaveFullScreenRenFeature;
    [SerializeField] private Material fullScreenShockwaveMat;

    private void Awake()
    {
        foreach (ScriptableRendererFeature feature in shockwaveFullScreenRenFeature) feature.SetActive(false);
    }

    public void EnableInwardsLineParticles()
    {
        spawnedChargeUpVFX.SetFXApperence(true);
    }

    public void DisableOutwardsLineParticles()
    {
        spawnedChargeUpVFX.SetFXApperence(false);
    }
    [ContextMenu("Play Successful Explosion")]
    public void PlaySuccessExplosion()
    {
        foreach (ScriptableRendererFeature feature in shockwaveFullScreenRenFeature) feature.SetActive(true);
        GameObject spawnedLandingVFX = Instantiate(landingVFXPrefab, Vector3.zero, Quaternion.identity);
        Destroy(spawnedLandingVFX, 3);
        StartCoroutine(ExplosionRoutine());
    }

    private IEnumerator ExplosionRoutine()
    {
        Time.timeScale = 0;
        yield return new WaitForSecondsRealtime(0.1f);
        Time.timeScale = 1;
        float duration = 0.3f;
        float elapsed = 0.0f;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            fullScreenShockwaveMat.SetFloat(LifetimeID, elapsed / duration);
            yield return new WaitForEndOfFrame();
        }
        foreach (ScriptableRendererFeature feature in shockwaveFullScreenRenFeature) feature.SetActive(false);
    }

    public void PlayCentralWhiteFlash()
    {
        GameObject spawned = Instantiate(centralWhiteFlashVFX, Vector3.zero, Quaternion.identity);
        Destroy(spawned, 3);
    }
    public void PlayFailFizzle() { }

    public void ShutDown() { }

    public void Bootup() { }

}
