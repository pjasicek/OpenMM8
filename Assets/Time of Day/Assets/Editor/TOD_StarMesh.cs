using UnityEngine;
using UnityEditor;
using System;
using System.IO;
using System.Collections.Generic;

public static class TOD_StarMesh
{
	[MenuItem("Assets/Create/Star Mesh", true)]
	private static bool Validate()
	{
		return Selection.activeObject != null && Selection.activeObject.GetType() == typeof(TextAsset);
	}

	[MenuItem("Assets/Create/Star Mesh")]
	private static void Create()
	{
		var separators = new char[] { ' ', '\t' };
		var textAsset = Selection.activeObject as TextAsset;
		var path = AssetDatabase.GetAssetPath(textAsset).Replace(".txt", "");

		var names = new string[] { "_LOD2", "_LOD1", "_LOD0" };
		var cutoff = new float[] { 4.5f, 5.5f, float.MaxValue };

		for (int i = 0; i < names.Length; i++)
		{
			var vertices = new List<Vector3>();
			var normals = new List<Vector3>();
			var tangents = new List<Vector4>();
			var triangles = new List<int>();
			var uv = new List<Vector2>();
			var colors = new List<Color>();

			using (var reader = new StringReader(textAsset.text))
			{
				while (reader.Peek() != -1)
				{
					var line = reader.ReadLine();
					var values = line.Split(separators, StringSplitOptions.RemoveEmptyEntries);

					if (values.Length != 3) continue;

					if (values[0].StartsWith("#")) continue;

					float rasc = float.Parse(values[0]);
					float decl = float.Parse(values[1]);
					float mag  = float.Parse(values[2]);

					if (mag > cutoff[i]) continue;

					var center = Carthesian(rasc, decl);
					var normal = -center;
					var tangent = Tangent(normal);

					float m = Mathf.Pow(2.512f, 1 - mag); 
					var color = new Color(1, 1, 1, m);

					int idx = vertices.Count;

					vertices.Add(center);
					vertices.Add(center);
					vertices.Add(center);
					vertices.Add(center);

					normals.Add(normal);
					normals.Add(normal);
					normals.Add(normal);
					normals.Add(normal);

					tangents.Add(tangent);
					tangents.Add(tangent);
					tangents.Add(tangent);
					tangents.Add(tangent);

					colors.Add(color);
					colors.Add(color);
					colors.Add(color);
					colors.Add(color);

					uv.Add(new Vector2(0, 0));
					uv.Add(new Vector2(1, 0));
					uv.Add(new Vector2(0, 1));
					uv.Add(new Vector2(1, 1));

					triangles.Add(idx);
					triangles.Add(idx+1);
					triangles.Add(idx+2);

					triangles.Add(idx+3);
					triangles.Add(idx+2);
					triangles.Add(idx+1);
				}
			}

			var name = path + names[i] + ".asset";

			var mesh = AssetDatabase.LoadMainAssetAtPath(name) as Mesh;

			if (!mesh)
			{
				mesh = new Mesh() {
					vertices = vertices.ToArray(),
					colors = colors.ToArray(),
					uv = uv.ToArray(),
					normals = normals.ToArray(),
					tangents = tangents.ToArray(),
					triangles = triangles.ToArray(),
				};

				AssetDatabase.CreateAsset(mesh, name);
			}
			else
			{
				mesh.Clear();

				mesh.vertices = vertices.ToArray();
				mesh.colors = colors.ToArray();
				mesh.uv = uv.ToArray();
				mesh.normals = normals.ToArray();
				mesh.tangents = tangents.ToArray();
				mesh.triangles = triangles.ToArray();

				AssetDatabase.SaveAssets();
			}
		}
	}

	private static Vector4 Tangent(Vector3 normal)
	{
		Vector3 t1 = Vector3.Cross(normal, Vector3.forward);
		Vector3 t2 = Vector3.Cross(normal, Vector3.up);

		if (t1.magnitude > t2.magnitude)
		{
			return new Vector4(t1.x, t1.y, t1.z, 1);
		}
		else
		{
			return new Vector4(t2.x, t2.y, t2.z, 1);
		}
	}

	private static Vector3 Carthesian(float rasc, float decl)
	{
		float rasc_rad = Mathf.Deg2Rad * rasc;
		float decl_rad = Mathf.Deg2Rad * decl;

		float x = -Mathf.Cos(decl_rad) * Mathf.Sin(rasc_rad);
		float z =  Mathf.Cos(decl_rad) * Mathf.Cos(rasc_rad);
		float y =  Mathf.Sin(decl_rad);

		return new Vector3(x, y, z);
	}
}
