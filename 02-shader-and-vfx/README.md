Shader + VFX

SoftLightShadows uses a soft-light blend worked out manually per channel, plus a color-keyed mask so the shadow only renders where it's supposed to. Check out my UI Shader Implementation Demo Reel for this exact shader around the 0:55 mark.

Effect / TogglableEffect was a simple concept to take a common pattern across the project and encapsulate it into a reusable and instantly preview-able component.

VFXService toggles a full-screen URP ScriptableRendererFeature for a shockwave effect, drives its shader property over time, and then turns it off again for performance on mobile.
