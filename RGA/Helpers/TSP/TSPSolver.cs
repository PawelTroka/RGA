using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RGA.Helpers
{
    public abstract class TspSolver
    {
        protected class Node
        {
            public int Vertex { get; set; }
            public Node[] ChildNodes { get; set; }
            public bool Selected { get; set; }
        }

        #region Member Variables

        protected readonly double[,] _adjacencyMatrix;
        protected readonly IEnumerable<int> _vertices;

        #endregion

        #region Constructor

        protected TspSolver(IEnumerable<int> vertices, double[,] matrix)
        {
            _vertices = vertices;
            _adjacencyMatrix = matrix;
        }

        #endregion

        #region Public Methods

        public abstract IEnumerable<int> Solve(out double cost);

        #endregion


    }
}