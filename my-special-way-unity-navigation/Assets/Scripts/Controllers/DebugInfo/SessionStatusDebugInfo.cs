#region Usings

using System;
using GoogleARCore;
using UnityEngine;

#endregion

namespace Msw.Core.Controllers.DebugInfo
{
	public class SessionStatusDebugInfo : DebugInfoBase
	{
		private Color _color;
		
		protected virtual void Update()
		{
			switch (Session.Status)
			{
				case SessionStatus.None:
					_color = Color.red;
					break;
				case SessionStatus.Initializing:
					_color = Color.yellow;
					break;
				case SessionStatus.Tracking:
					_color = Color.green;
					break;
				case SessionStatus.LostTracking:
					_color = Color.red;
					break;
				case SessionStatus.NotTracking:
					_color = Color.red;
					break;
				case SessionStatus.FatalError:
					_color = Color.red;
					break;
				case SessionStatus.ErrorApkNotAvailable:
					_color = Color.red;
					break;
				case SessionStatus.ErrorPermissionNotGranted:
					_color = Color.red;
					break;
				case SessionStatus.ErrorSessionConfigurationNotSupported:
					_color = Color.red;
					break;
				default:
					throw new ArgumentOutOfRangeException();
			}

			DebugInfo.color = _color;
			DebugInfo.text = $"SessionStatus {Session.Status.ToString()}";
		}
	}
}
