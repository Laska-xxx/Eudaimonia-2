using UnityEngine;
using UnityEngine.Rendering;

[System.Serializable, VolumeComponentMenu("Retro/PS1 Post Process")]
public class PS1VolumeComponent : VolumeComponent, IPostProcessComponent
{
    public ClampedFloatParameter pixelationFactor = new ClampedFloatParameter(0.5f, 0.01f, 1.0f);
    public ClampedFloatParameter ditheringScale = new ClampedFloatParameter(1.0f, 0.1f, 4.0f);

    [Space(10)]
    public BoolParameter enableInterlacing = new BoolParameter(true);
    public ClampedIntParameter interlacingSize = new ClampedIntParameter(2, 1, 10);

    public bool IsActive() => pixelationFactor.value < 1.0f || ditheringScale.value > 0.1f || enableInterlacing.value;
    public bool IsTileCompatible() => false;
}
