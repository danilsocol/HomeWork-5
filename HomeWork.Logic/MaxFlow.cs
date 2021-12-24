using System;
using System.Collections.Generic;
using System.Text;

namespace HomeWork5.Logic
{
	public class MaxFlow
	{
		static int V;
		public static int fordFulkerson(int[,] graph, int[,] rGraph, int idStart, int idFinal)
		{
			V = (int)Math.Sqrt(graph.Length);

			int i, j;

			Array.Copy(graph, rGraph, graph.Length);

			int[] parent = new int[V];

			int max_flow = 0;

			while (bfs(rGraph, idStart, idFinal, parent))
			{
				List<int> route = new List<int>();

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

                max_flow += path_flow;

				// ShowPath();

			}

			return max_flow;
		}

		static bool bfs(int[,] rGraph, int idStart, int idFinal, int[] parent)
		{

			bool[] visited = new bool[V];
			for (int i = 0; i < V; ++i)
				visited[i] = false;

			List<int> queue = new List<int>();
			queue.Add(idStart);
			visited[idStart] = true;
			parent[idStart] = -1;

			while (queue.Count != 0)
			{
				int i = queue[0];
				queue.RemoveAt(0);

				for (int j = 0; j < V; j++)
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


