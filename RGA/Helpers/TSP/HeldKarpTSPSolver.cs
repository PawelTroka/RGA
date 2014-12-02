using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RGA.Helpers
{
    public class HeldKarpTSPSolver : TspSolver //based on http://www.codeproject.com/Articles/762581/Held-Karp-algorithm-implementation-in-Csharp
    {
        public HeldKarpTSPSolver(IEnumerable<int> vertices, double[,] matrix) :base(vertices,matrix)
        {
        }

        public override IEnumerable<int> Solve(out double cost)
        {
            var startVertex = _vertices.First();
            var endVertex = startVertex;
            var set = new HashSet<int>(_vertices);
            set.Remove(startVertex);

            var root = new Node();
            cost = GetMinimumCostRoute(startVertex, set, root);
            int lastVertex;
            var tree = TraverseTree(root, startVertex,out lastVertex);

           // cost += _adjacencyMatrix[lastVertex, startVertex];
            return tree;
        }

        #region Private Methods

        private double GetMinimumCostRoute(int startVertex, HashSet<int> set, Node root)
        {
            if (!set.Any())
            {
                return _adjacencyMatrix[startVertex, 0];
            }

            double minCostTotal = double.MaxValue;
            int i = 0;
            int selectedIdx = i;
            root.ChildNodes = new Node[set.Count()];

            foreach (var destinationVertex in set)
            {
                root.ChildNodes[i] = new Node { Vertex = destinationVertex };

                double currentVertexCost = _adjacencyMatrix[startVertex, destinationVertex];

                var newSet = new HashSet<int>(set);
                newSet.Remove(destinationVertex);
                double costFromHere = GetMinimumCostRoute(destinationVertex, newSet, root.ChildNodes[i]);
                double newC = currentVertexCost + costFromHere;

                if (minCostTotal > newC)
                {
                    minCostTotal = newC;
                    selectedIdx = i;
                }

                i++;
            }

            root.ChildNodes[selectedIdx].Selected = true;

            return minCostTotal;

        }

        private IEnumerable<int> TraverseTree(Node root, int startint, out int lastInt)
        {
            var q = new Queue<int>();
            q.Enqueue(startint);
            TraverseTreeUtil(root, q);
            lastInt = q.Last();
            q.Enqueue(startint);
            return q;
        }

        private void TraverseTreeUtil(Node root, Queue<int> vertices)
        {
            if (root.ChildNodes == null)
            {
                return;
            }

            foreach (var child in root.ChildNodes)
            {
                if (child.Selected)
                {
                    vertices.Enqueue(child.Vertex);
                    TraverseTreeUtil(child, vertices);
                }
            }
        }

        #endregion
    }
}