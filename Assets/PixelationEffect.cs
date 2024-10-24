using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

[System.Serializable]
[PostProcess(typeof(PixelationRenderer), PostProcessEvent.AfterStack, "Custom/Pixelation")]
public sealed class Pixelation : PostProcessEffectSettings
{
    [Range(1f, 128f), Tooltip("The pixel size for the pixelation effect.")]
    public FloatParameter pixelSize = new FloatParameter { value = 8f };
}

public sealed class PixelationRenderer : PostProcessEffectRenderer<Pixelation>
{
    public override void Render(PostProcessRenderContext context)
    {
        PropertySheet sheet = context.propertySheets.Get("Custom/Pixelate");
        sheet.properties.SetFloat("_PixelSize", settings.pixelSize);
        context.command.BlitFullscreenTriangle(context.source, context.destination, sheet, 0);
    }
}
