using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Generator;

public class LineComposer : MonoBehaviour
{
	public Vector3 normal;
	public float thickness;

	private MeshFilter mesh;
	private MeshRenderer render;
	private PolygonCollider2D collider;

	private List<Vector3> line = new List<Vector3>();

	void Awake() {
		mesh = gameObject.AddComponent<MeshFilter>();
		mesh.mesh = new Mesh();
		render = gameObject.AddComponent<MeshRenderer>();

		collider = gameObject.AddComponent<PolygonCollider2D>();
	}

	public void SetMaterial(Material material) {
		render.material = material;
	}

	public void AddPoint(Vector3 point) {
		line.Add(point);
		if (line.Count >= 2) {
			GenerateMesh(mesh.mesh);
			ColliderGenerator.Generate(mesh.mesh, collider);
		}
	}

	public void AddPointInGlobalSpace(Vector3 point) {
		AddPoint(point - transform.position);
	}

	public Vector3 GetLast() {
		return line[line.Count-1];
	}

	//private Vector2[] GenerateColliderPoints() {
	//	Vector2[] result = new Vector2[vertices.Count];
	//	for (int i = 0; i < vertices.Count; i++) {
	//		if (i % 2 == 0) {
	//			result[i/2] = new Vector2(vertices[i].x, vertices[i].y);
	//		}
	//		else {
	//			result[(vertices.Count-1)-(i-1)/2] = new Vector2(vertices[i].x, vertices[i].y);
	//		}
	//	}
	//	return result;
	//}

	private void GenerateMesh(Mesh mesh) {
		List<Vector3> vertices = new List<Vector3>();
		List<int> triangles = new List<int>();

		Vector3 direction = line[1]-line[0];

		#region start
		vertices.Add(line[0]+(Rotate(direction, -90).normalized*thickness));
		vertices.Add(line[0]+(Rotate(direction, 90).normalized*thickness));
		#endregion start

		#region middle
		for (int i = 1; i < line.Count-1; i++) {
			direction = line[i + 1] - line[i - 1];

			vertices.Add(line[i]+(Rotate(direction, -90).normalized* thickness));
			vertices.Add(line[i]+(Rotate(direction, 90).normalized* thickness));
		}
		#endregion middle

		#region last
		direction = line[line.Count - 1] - line[line.Count - 2];

		vertices.Add(line[line.Count - 1]+(Rotate(direction, -90).normalized* thickness));
		vertices.Add(line[line.Count - 1]+(Rotate(direction, 90).normalized* thickness));
		#endregion last

		for (int i = 0; i < line.Count-1; i++) {
			triangles.Add(i*2+1);
			triangles.Add(i*2);
			triangles.Add(i*2+2);

			triangles.Add(i*2+1);
			triangles.Add(i*2+2);
			triangles.Add(i*2+3);
		}

		mesh.vertices = vertices.ToArray();
		mesh.triangles = triangles.ToArray();
	}

	private Vector3 Rotate(Vector3 vec, float angle) {
		return Quaternion.AngleAxis(angle, normal) * vec;
	}

	public static LineComposer GetLine(string name, Vector3 position, float thickness, Material LineMaterial) {
		GameObject instance = new GameObject(name);
		instance.transform.position = position;

		LineComposer Composer = instance.AddComponent<LineComposer>();
		Composer.thickness = thickness;
		Composer.normal = Vector3.back;

		Composer.AddPoint(new Vector3(0, 0, 0));
		Composer.SetMaterial(LineMaterial);

		return Composer;
	}
}