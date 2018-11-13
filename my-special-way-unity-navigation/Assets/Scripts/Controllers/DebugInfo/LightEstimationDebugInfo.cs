#region Usings

using GoogleARCore;

#endregion

namespace Msw.Core.Controllers.DebugInfo
{
	public class LightEstimationDebugInfo : DebugInfoBase
	{
		protected virtual void Update()
		{
			var state = Frame.LightEstimate.State;
			var colorCorrection = Frame.LightEstimate.ColorCorrection;
			var pixelIntensity = Frame.LightEstimate.PixelIntensity;

			DebugInfo.text = $"State {state}\nColorCorrection {colorCorrection}\nPixelIntensity {pixelIntensity}";
		}
	}
}

