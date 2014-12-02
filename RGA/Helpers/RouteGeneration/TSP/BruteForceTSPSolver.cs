using System.Collections.Generic;
using System.Linq;

namespace RGA.Helpers.TSP
{
    public class BruteForceTSPSolver : TspSolver
    {
        private List<IEnumerable<int>> allRoutes;
        private List<int> bestRoute;
        private double bestRouteCost;

        public BruteForceTSPSolver(IEnumerable<int> vertices, double[,] matrix)
            : base(vertices, matrix)
        {
            bestRouteCost = double.MaxValue;
        }

        public override IEnumerable<int> Solve(out double cost)
        {
            var list = new List<int>(_vertices);
            list.RemoveAt(0);

            allRoutes = new List<IEnumerable<int>>(GetPermutations(list, list.Count));

            double cst = double.MaxValue;
            for (int i = 0; i < allRoutes.Count; i++)
            {
                var routeList = new List<int>(allRoutes[i]);

                routeList.Insert(0, 0);
                routeList.Add(0);

                cst = CalculateCost(routeList);
                if (cst < bestRouteCost)
                {
                    bestRouteCost = cst;
                    bestRoute = routeList;
                }
            }


            cost = bestRouteCost;
            return bestRoute;
        }

        private double CalculateCost(List<int> enumerable)
        {
            double cost = 0.0;

            for (int i = 0; i < enumerable.Count - 1; i++)
            {
                cost += _adjacencyMatrix[enumerable[i], enumerable[i + 1]];
            }
            return cost;
        }


        private static IEnumerable<IEnumerable<T>> GetPermutations<T>(IEnumerable<T> list, int length)
        {
            if (length == 1) return list.Select(t => new[] {t});

            return GetPermutations(list, length - 1)
                .SelectMany(t => list.Where(e => !t.Contains(e)),
                    (t1, t2) => t1.Concat(new[] {t2}));
        }
    }
}