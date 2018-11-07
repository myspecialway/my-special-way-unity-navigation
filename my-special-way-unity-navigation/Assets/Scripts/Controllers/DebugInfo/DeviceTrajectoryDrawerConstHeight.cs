#region Usings

using GoogleARCore;
using UnityEngine;

#endregion

namespace Msw.Core.Controllers.DebugInfo
{
	public class DeviceTrajectoryDrawerConstHeight : MonoBehaviour
	{
		[SerializeField] private LineRenderer _lineRenderer;
		[SerializeField] private float        _lineWidth;
		[SerializeField] private float        _updateInterval;

		private float _elapsedTime;
		private int   _currentIndex = 0;
		private bool  _isStable;

		protected void Awake()
		{
			_lineRenderer.widthMultiplier = _lineWidth;
		}

		protected void Update()
		{
			// not stable
			if (Session.Status != SessionStatus.Tracking)
			{
				return;
			}

			var currentPosition = Frame.Pose.position;

			currentPosition.y = .0f;
			
			_elapsedTime += Time.deltaTime;

			if (_elapsedTime > _updateInterval)
			{
				_elapsedTime = 0f;
				AddNode(currentPosition);
			}
		}

		private void AddNode(Vector3 position)
		{
			_currentIndex++;
			_lineRenderer.positionCount = _currentIndex;
			_lineRenderer.SetPosition(_currentIndex - 1, position);
		}
	}
}