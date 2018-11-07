#region Usings

using UnityEngine;

#endregion

namespace Msw.Core.Controllers.DebugInfo
{
    public class FpsDebugInfo : DebugInfoBase
    {
        // Attach this to a GUIText to make a frames/second indicator.
        //
        // It calculates frames/second over each updateInterval,
        // so the display does not keep changing wildly.
        //
        // It is also fairly accurate at very low FPS counts (<10).
        // We do this not by simply counting frames per interval, but
        // by accumulating FPS for each frame. This way we end up with
        // correct overall FPS even if the interval renders something like
        // 5.5 frames.

        [SerializeField] private float _updateInterval = 0.5F;

        private float _accum  = 0; // FPS accumulated over the interval
        private int   _frames = 0; // Frames drawn over the interval
        private float _timeleft;   // Left time for current interval

        protected virtual void Start()
        {
            _timeleft = _updateInterval;
        }

        protected virtual void Update()
        {
            _timeleft -= Time.deltaTime;
            _accum    += Time.timeScale / Time.deltaTime;
            ++_frames;

            // Interval ended - update GUI text and start new interval
            if (_timeleft <= 0.0)
            {
                // display two fractional digits (f2 format)
                float  fps              = _accum / _frames;
                string stringFormatType = $"{fps:F2} FPS";
                DebugInfo.text = stringFormatType;

                if (fps < 30)
                {
                    DebugInfo.color = Color.yellow;
                }
                else if (fps < 10)
                {
                    DebugInfo.color = Color.red;
                }
                else
                {
                    DebugInfo.color = Color.green;
                }

                _timeleft = _updateInterval;
                _accum    = 0.0F;
                _frames   = 0;
            }
        }
    }
}