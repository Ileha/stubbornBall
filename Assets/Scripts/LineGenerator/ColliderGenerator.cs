using System;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;

namespace Generator {
	public class Edge {
		public int firstIndex;
		public int secondIndex;

		public Edge() { }

		public Edge(Edge other) {
			this.firstIndex = other.firstIndex;
			this.secondIndex = other.secondIndex;
		}

		public Edge(int firstIndex, int secondIndex) {
			this.firstIndex = firstIndex;
			this.secondIndex = secondIndex;
		}

		public void Set(int firstIndex, int secondIndex) {
			this.firstIndex = firstIndex;
			this.secondIndex = secondIndex;
		}

		public void Set(Edge other) {
			this.firstIndex = other.firstIndex;
			this.secondIndex = other.secondIndex;
		}

		public override string ToString() {
			return string.Format("[{0}-{1}]", firstIndex, secondIndex);
		}

		public override int GetHashCode() {
			return Math.Min(firstIndex, secondIndex) * 7 + Math.Max(firstIndex, secondIndex);
		}

		public override bool Equals(object other) {
			if (other == null) {
				return false;
			}

			if (object.ReferenceEquals(this, other)) {
				return true;
			}

			if (!(other is Edge)) {
				return false;
			}
			Edge compare = other as Edge;

			return (firstIndex == compare.firstIndex && secondIndex == compare.secondIndex) || 
				(firstIndex == compare.secondIndex && secondIndex == compare.firstIndex);
		}
	}

	public class ColliderGenerator {
		public static void Generate(Mesh mesh, PolygonCollider2D collider) {
			Vector3[] vertices = mesh.vertices;
			int[] triangles = mesh.triangles;

			HashSet<Edge> edges = new HashSet<Edge>();

			for (int i = 0; i < triangles.Length; i += 3) {
				AddToEdges(edges, new Edge(triangles[i], triangles[i + 1]));
                AddToEdges(edges, new Edge(triangles[i+1], triangles[i + 2]));
                AddToEdges(edges, new Edge(triangles[i + 2], triangles[i]));
			}


			int maxFirstIndex = 0;
			foreach (Edge edge in edges) {
				if (edge.firstIndex > maxFirstIndex) {
					maxFirstIndex = edge.firstIndex;
				}
			}
			Edge[] sortedByFirstIndex = new Edge[maxFirstIndex+1];
			List<List<Vector2>> result = new List<List<Vector2>>();

			foreach (Edge edge in edges) {
				sortedByFirstIndex[edge.firstIndex] = edge;
			}

			Edge tempFirst = null;
			Edge temp = null;

			while (edges.Count > 0) {
				List<Vector2> subResult = new List<Vector2>();
				result.Add(subResult);

				tempFirst = edges.First();
				temp = tempFirst;
				subResult.Add(vertices[tempFirst.firstIndex]);
				edges.Remove(temp);

				do {
					temp = sortedByFirstIndex[temp.secondIndex];
					subResult.Add(vertices[temp.firstIndex]);
					edges.Remove(temp);
				} while (tempFirst.firstIndex != temp.secondIndex);
			}

			collider.pathCount = result.Count;
			for (int i = 0; i < collider.pathCount; i++) {
				collider.SetPath(i, result[i].ToArray());
			}
		}

		private static void AddToEdges(HashSet<Edge> edges, Edge edge) {
			if (!edges.Add(edge)) {
				edges.Remove(edge);
			}
		}

	}
}
