using HOPL.Interpreter.Exceptions;
using System.Collections.Generic;

namespace HOPL.Interpreter.Exploration
{
    public class DependencyGraph
	{
		public enum Color { WHITE, GRAY, BLACK }

		public class GraphNode
		{
			public Dependency Node { get; set; }
			public List<GraphNode> Dependencies { get; set; } = new List<GraphNode>();
			public Color Color { get; set; }

			public GraphNode(Dependency node)
			{
				Node = node;
			}

			public override int GetHashCode()
			{
				return Node.GetHashCode();
			}

			public override bool Equals(object obj)
			{
				return Node.Equals(obj);
			}

			public override string ToString()
			{
				return Node.ToString();
			}
		}

		public Dictionary<Dependency, GraphNode> Graph { get; protected set; } =
			new Dictionary<Dependency, GraphNode>();

		public DependencyGraph(IDictionary<Dependency, HashSet<Dependency>> dependencies)
		{
			foreach (Dependency dep in dependencies.Keys)
			{
				GraphNode node = new GraphNode(dep);
				Graph.Add(dep, node);
			}

			foreach (KeyValuePair<Dependency, HashSet<Dependency>> kvp in dependencies)
			{
				Dependency dep = kvp.Key;
				HashSet<Dependency> deps = kvp.Value;

				GraphNode node = Graph[dep];
				foreach (Dependency child in deps)
					if (Graph.ContainsKey(child))
						node.Dependencies.Add(Graph[child]);
			}
		}

		public Queue<Dependency> EvaluationOrder()
		{
			Queue<Dependency> order = new Queue<Dependency>();

			foreach (GraphNode node in Graph.Values)
				node.Color = Color.WHITE;

			foreach (GraphNode node in Graph.Values)
				if (node.Color == Color.WHITE)
					DepthFirstVisit(node, order);

			return order;
		}

		private void DepthFirstVisit(GraphNode node, Queue<Dependency> order, bool passedVariable = false)
		{
            passedVariable = passedVariable || node.Node.Type == DependencyType.VARIABLE;

			node.Color = Color.GRAY;

			if (node.Node.Type == DependencyType.VARIABLE && node.Node.ContainsAwait)
				throw new DependencyContainsAwaitException(node.ToString());

			foreach (GraphNode child in node.Dependencies)
			{
				if (passedVariable && child.Node.ContainsAwait)
					throw new DependencyContainsAwaitException(child.ToString());

				if (child.Color == Color.WHITE)
					DepthFirstVisit(child, order, passedVariable);
				else if (child.Color == Color.GRAY && child.Node.Type == DependencyType.VARIABLE)
					throw new RecursiveVariableDependencyException(child.ToString());
			}

			if (node.Node.Type == DependencyType.VARIABLE)
				order.Enqueue(node.Node);

			node.Color = Color.BLACK;
		}
	}
}
