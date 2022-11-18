using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.ProjectWindowCallback;
using System.IO;
using System.Threading;
using KR;
using System.Linq;
public class SpriteCreator : Editor {

	[MenuItem("Assets/Create/UITools/Transparent", false, 18)]
	public static void CreateTransparent()
	{

		CreateShape("Transparent-", 100, 100, Color.clear);

	}
	[MenuItem("Assets/Create/UITools/Square SM", false, 18)]
    public static void CreateSmallSquare()
    {

		CreateShape("Square-sm-", 100, 100, Color.white);
    }



	[MenuItem("Assets/Create/UITools/Square MD", false, 18)]
    public static void CreateMediumSquare()
    {

        CreateShape("Square-md-", 256, 256, Color.white);
    }
	[MenuItem("Assets/Create/UITools/Square LG", false, 18)]
    public static void CreateLargeSquare()
    {
        CreateShape("Square-lg-", 512, 512, Color.white);
    }


	public static void CreateShape(string name ,int width, int height, Color color){
		var o = AssetDatabase.GetAssetPath(Selection.activeObject);
		var assetName = name  + new KR.DateTime(System.DateTime.Now).TotalSeconds.ToString() + ".png";
        var assetPathAndName = o + "/" + assetName;

		Texture2D tex2D = new Texture2D(width, height);
		Color[] colors = new Color[width * height];
        int i = 0;
		for (int y = 0; y < width; y++)
        {
			for (int x = 0; x < height; x++)
			{
				colors[i] = i == 0 ? Color.white : color;
				i++;
			}
        }

        tex2D.SetPixels(colors);
        tex2D.Apply();
        var bytes = tex2D.EncodeToPNG();

        Thread thread = new Thread(() => {
			System.IO.File.WriteAllBytes(assetPathAndName, bytes);

        });

        thread.Start();
        thread.Join();

        AssetDatabase.Refresh();
	}
}
