#region Usings

using TMPro;
using UnityEngine;

#endregion

namespace Msw.Core.Controllers.DebugInfo
{
	public abstract class DebugInfoBase : MonoBehaviour
	{
		[SerializeField] protected TextMeshProUGUI DebugInfo;
	}
}
