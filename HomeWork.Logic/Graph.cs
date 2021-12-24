using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace HomeWork5.Logic
{
    public class Graph
    {
        List<Node> nodes = new List<Node>();
        int[,] table { get; set; }
   
        public void AddNode(int id)
        {
            Node node = new Node(id);
            nodes.Add(node);
        }

        public void DeleteNode(int id)
        {
            Node node = FindNode(id);
            nodes.Remove(node);
        }
        public void AddEdge(Node nodeFirst, Node nodeSecond, int cash, string name)
        {
            Edge edge1 = new Edge(nodeFirst, nodeSecond, cash, 0,name);
            nodeFirst.edge.Add(edge1);

            //Edge edge2 = new Edge(nodeSecond, nodeFirst, cash, 0);
            //nodeSecond.edge.Add(edge2);
        }

        public void DeleteEdge(int idFirstNode, int idSecondNode)
        {
            Edge edge = FindEdge(idFirstNode, idSecondNode);
            nodes[idFirstNode - 1].edge.Remove(edge);
        }

        public void ChangePrice(int idFirstNode,int idSecondNode,int newPrice)
        {
            Edge edge = FindEdge(idFirstNode, idSecondNode);
            edge.price = newPrice;
        }

        Edge FindEdge(int idFirstNode, int idSecondNode)
        {
            foreach(Node node in nodes)
            {
               if( node.id == idFirstNode)
               {
                    foreach(Edge edge in node.edge)
                    {
                        if (edge.nodeSecond.id == idSecondNode)
                            return edge;
                    }
               }
            }
            return null;
        }

        public Node FindNode(int id)
        {
            foreach (Node node in nodes)
            {
                if (node.id == id)
                {
                    return node;
                }
            }
            return null;
        }
            public void FindMinFlow(int idStart, int idFinal)
        {
            int[,] rGraph = new int[(int)Math.Sqrt(table.Length), (int)Math.Sqrt(table.Length)];

            int[,] t = transformationGraph();

            MaxFlow.fordFulkerson(table, rGraph, idStart, idFinal);

            setPayment(rGraph);
        }
        public void setPayment(int[,] rGraph)
        {
            for (int i = 0; i < nodes.Count; i++)
            {
                int countEdge = 0;
                for (int j = 0; j < nodes.Count; j++)
                {
                    if (rGraph[i, j] != 0)
                    {
                        nodes[i].edge[countEdge].SetPaid(rGraph[i, j]);
                        countEdge++;
                    }
                }
            }
        }
        public int[,] transformationGraph()
        {
            int[,] table = new int[nodes.Count, nodes.Count];

            for (int i = 0; i < nodes.Count; i++)
            {
                int countEdge = 0;

                for (int j = 0; j < nodes.Count; j++)
                {
                    for (int k = 0; k < nodes[i].edge.Count; k++)
                    {
                        if (nodes[i].edge[k].nodeSecond == nodes[j])
                        {
                            table[i, j] = nodes[i].edge[k].price;
                            countEdge++;
                            break;
                        }
                        else
                        {
                            table[i, j] = 0;
                        }
                    }
                }
            }
            return table;
        }

        public void Save(string file)
        {

            using (FileStream save = new FileStream($"{file}", FileMode.OpenOrCreate))
            {
                string[] lines = new string[nodes.Count];

                for (int i = 0; i < nodes.Count; i++)
                {
                    lines[i] = $"{nodes[i].id}\n";
                    for (int j = 0; j < nodes[i].edge.Count; j++)
                    {
                        lines[i] += $"/{nodes[i].id}->{nodes[i].edge[j].nodeSecond.id};{nodes[i].edge[j].price};{nodes[i].edge[j].paid}|\n" ;
                    }
                    
                    byte[] array = System.Text.Encoding.Default.GetBytes(lines[i]);
                    save.Write(array, 0, array.Length);
                    save.Seek(1,SeekOrigin.End);
                }
            }
        }

        public void Download(string file)
        {
            using (FileStream save = new FileStream($"{file}", FileMode.OpenOrCreate))
            {
                save.Seek(0, SeekOrigin.Begin);
                byte[] output = new byte[save.Length];
                save.Read(output, 0, output.Length);

                string textGraph = Encoding.Default.GetString(output);  // hello house
            }

        }

    }


    public class Node
    {
        public List<Edge> edge = new List<Edge>();
        public int id;
        public Node(int id)
        {
            this.id = id;
        }
    }

    public class Edge
    {
        Node nodeFirst { get; set; }
        public Node nodeSecond { get; set; }
        string name { get; set; }
        public int price { get; set; }
        public int paid { get; set; }
        public Edge(Node nodeFirst, Node nodeSecond, int price, int paid,string name)
        {
            this.nodeFirst = nodeFirst;
            this.nodeSecond = nodeSecond;
            this.price = price;
            this.paid = paid;
            this.name = name;
        }

        public void SetPaid(int paid)
        {
            this.paid = paid;
        }

    }
}
