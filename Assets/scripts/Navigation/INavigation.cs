using System.Collections.Generic;

namespace Navigation
{
	public interface INavigation
	{

		List<string> GetShortestPath(Dictionary<string, Node> graph, string from, string to);
	}
}
