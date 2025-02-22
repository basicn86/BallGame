using Godot;
using System;
using System.Collections.Generic;
using System.Diagnostics;

public partial class CptDetonatorGround : StaticBody3D
{
	[Export]
	MeshInstance3D meshInstance3D;

	[Export]
	ShaderMaterial shader;

	//TODO: don't make this a singleton since we can only have one instance of this class. In the future we may want multiple instances of this class.
	public static CptDetonatorGround? Instance { get; private set; }

	Vector3[,] TextureCoords = new Vector3[width, width];

	Vector3[] vertices;
	Vector2[] uvs;
	int[] indices;

	const int width = 512; //texture is 1:1, height is same as width

	private Image img;
	private ImageTexture imageTexture;

	public override void _Ready()
	{
		if (Instance != null) QueueFree();
		Instance = this;

		Mesh mesh = meshInstance3D.Mesh;
		if (mesh == null) QueueFree();

		vertices = (Vector3[])mesh.SurfaceGetArrays(0)[(int)Mesh.ArrayType.Vertex];
		uvs = (Vector2[])mesh.SurfaceGetArrays(0)[(int)Mesh.ArrayType.TexUV];
		indices = (int[])mesh.SurfaceGetArrays(0)[(int)Mesh.ArrayType.Index];

		img = Image.CreateEmpty(width, width, true, Image.Format.Rf);

		for (int i = 0; i < width; i++)
		{
			for (int j = 0; j < width; j++)
			{
				img.SetPixel(i, j, new Color(1.0f, 0.0f, 0.0f, 0.0f));
			}
		}
		img.GenerateMipmaps();
		imageTexture = new ImageTexture();
		imageTexture.SetImage(img);
		shader.SetShaderParameter("Mask", imageTexture);

		for (int i = 0; i < width; i++)
		{
			for (int j = 0; j < width; j++)
			{
				Vector3 worldPos = GetWorldPositionFromUV(new Vector2((float)i / (float)width, (float)j / (float)width));
				TextureCoords[i, j] = worldPos;
			}
		}
	}

	public override void _Process(double delta)
	{
		//this is for debug.
		if (Input.IsActionJustPressed("debugf2")) {
			Stopwatch sw = new Stopwatch();
			sw.Start();
			
			AddBurnPoint(new Vector3(150, 0, 100));

			sw.Stop();
			GD.Print("Time: " + sw.ElapsedMilliseconds);
		}
	}
	
	public void AddBurnPoint(Vector3 point)
	{
		for (int i = 0; i < width; i++)
		{
			for (int j = 0; j < width; j++)
			{
				Vector3 worldPos = TextureCoords[i, j];
				if (worldPos.IsZeroApprox()) continue;
				if (worldPos.DistanceSquaredTo(point) < 9f)
				{
					img.SetPixel(i, j, new Color(0.0f, 0.0f, 0.0f, 0.0f));
				}
			}
		}
		img.GenerateMipmaps();
		imageTexture.Update(img);
		shader.SetShaderParameter("Mask", imageTexture);
	}

	public Vector3 GetBarycentrics(Vector2 p, Vector2 a, Vector2 b, Vector2 c)
	{
		Vector2 v0 = b - a;
		Vector2 v1 = c - a;
		Vector2 v2 = p - a;

		float d00 = v0.Dot(v0);
		float d01 = v0.Dot(v1);
		float d11 = v1.Dot(v1);
		float d20 = v2.Dot(v0);
		float d21 = v2.Dot(v1);

		float denom = d00 * d11 - d01 * d01;
		if (Mathf.Abs(denom) < 0.0001f)
			return new Vector3(-1, -1, -1); // Degenerate triangle

		float v = (d11 * d20 - d01 * d21) / denom;
		float w = (d00 * d21 - d01 * d20) / denom;
		float u = 1.0f - v - w;
		return new Vector3(u, v, w);
	}

	// Checks if barycentrics represent a point inside the triangle.
	public bool IsInsideTriangle(Vector3 bary)
	{
		return bary.X >= 0 && bary.Y >= 0 && bary.Z >= 0;
	}

	Vector3 GetWorldPositionFromUV(Vector2 uv)
	{
		for(int i = 0; i < indices.Length; i += 3)
		{
			int index0 = indices[i];
			int index1 = indices[i + 1];
			int index2 = indices[i + 2];

			Vector3 v0 = vertices[index0];
			Vector3 v1 = vertices[index1];
			Vector3 v2 = vertices[index2];

			Vector3 bary = GetBarycentrics(uv, uvs[index0], uvs[index1], uvs[index2]);
			if (IsInsideTriangle(bary))
			{
				Vector3 localPos = vertices[index0] * bary.X + vertices[index1] * bary.Y + vertices[index2] * bary.Z;
				Vector3 worldPos = meshInstance3D.ToGlobal(localPos);
				return worldPos;
			}
		}

		return new Vector3();
	}
}
