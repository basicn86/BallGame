using Godot;
using System;

public partial class CptDetonatorGround : StaticBody3D
{
	[Export]
	MeshInstance3D meshInstance3D;

	[Export]
	ShaderMaterial shader;

	//Note: this line is hard coded, there's no way to update the maxBurnPoints during runtime
	private const int maxBurnPoints = 1024;
	private Vector3[] burnPoints;
	private uint burnPointCount = 0;

	//TODO: don't make this a singleton since we can only have one instance of this class. In the future we may want multiple instances of this class.
	public static CptDetonatorGround? Instance { get; private set; }

	public override void _Ready()
	{
		if (Instance != null) return;
		Instance = this;

		Image img = Image.CreateEmpty(512, 512, true, Image.Format.Rgbaf);
		for (int i = 0; i < 512; i++)
		{
			for (int j = 0; j < 512; j++)
			{
				img.SetPixel(i, j, new Color(0, 0.3f, 0, 1.0f));
			}
		}

		//generate a block for testing
		for (int i = 200; i < 300; i++)
		{
			for (int j = 200; j < 300; j++)
			{
				img.SetPixel(i, j, new Color(0.3f, 0.3f, 0.3f, 1.0f));
			}
		}

		img.GenerateMipmaps();
		ImageTexture imageTexture = new ImageTexture();
		imageTexture.SetImage(img);
		shader.SetShaderParameter("Texture", imageTexture);

		SetupBurnPoints();
	}

	private void SetupBurnPoints()
	{
		burnPoints = new Vector3[maxBurnPoints];
		for (int i = 0; i < maxBurnPoints; i++) burnPoints[i] = new Vector3();
	}
	
	public void AddBurnPoint(Vector3 point)
	{
		if(burnPointCount == maxBurnPoints) return;
		burnPoints[burnPointCount] = point;
		burnPointCount++;
		shader.SetShaderParameter("BurnPoints", burnPoints);
		shader.SetShaderParameter("BurnPointCount", burnPointCount);
	}
}
