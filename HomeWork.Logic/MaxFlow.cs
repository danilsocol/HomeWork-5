using System;
using System.Collections.Generic;
using System.Text;

namespace HomeWork5.Logic
{
	public class MaxFlow
	{
		public static int fordFulkerson(int[,] graph, int[,] rGraph, int idStart, int idFinal, int[] parent, ref List<int> route)
		{
			int i, j;

			route = new List<int>();

			int path_flow = int.MaxValue;
			for (j = idFinal; j != idStart; j = parent[j]) // находим минимальную пропускную способноть 
			{
				i = parent[j];
				route.Add(j);
				path_flow
					= Math.Min(path_flow, rGraph[i, j]);
			}
			route.Add(idStart);

			for (j = idFinal; j != idStart; j = parent[j]) // переворот
			{
				i = parent[j];
				rGraph[i, j] -= path_flow;
				rGraph[j, i] += path_flow;
			}

			return path_flow;


		}

		public static bool bfs(int[,] rGraph, int idStart, int idFinal, int[] parent, int countNode)
		{

			bool[] visited = new bool[countNode];
			for (int i = 0; i < countNode; ++i)
				visited[i] = false;

			List<int> queue = new List<int>();
			queue.Add(idStart);
			visited[idStart] = true;
			parent[idStart] = -1;

			while (queue.Count != 0)
			{
				int i = queue[0];
				queue.RemoveAt(0);

				for (int j = 0; j < countNode; j++)
				{
					if (visited[j] == false
						&& rGraph[i, j] > 0)
					{
						if (j == idFinal)
						{
							parent[j] = i;
							return true;
						}
						queue.Add(j);
						parent[j] = i;
						visited[j] = true;
					}
				}
			}

			return false;
		}
	}
}


