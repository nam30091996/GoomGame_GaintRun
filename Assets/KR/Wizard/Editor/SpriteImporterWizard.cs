// Creates a simple wizard that lets you create a Light GameObject
// or if the user clicks in "Apply", it will set the color of the currently
// object selected to red

using UnityEditor;
using UnityEngine;
using System.Threading;
using KR;

public class SpriteImporterWizard : ScriptableWizard
{
    //public float range = 500;
    //public Color color = Color.red;
	public Sprite spriteSheet;

    [MenuItem("KR/Sprite Importer")]
    static void CreateWizard()
    {
		ScriptableWizard.DisplayWizard<SpriteImporterWizard>("Sprite Light", "Convert To FBF");
        //If you don't want to use the secondary button simply leave it out:
        //ScriptableWizard.DisplayWizard<WizardCreateLight>("Create Light", "Create");
    }

    void OnWizardCreate()
    {
		//GameObject go = new GameObject("New Light");
		//Light lt = go.AddComponent<Light>();
		//lt.range = range;
		//lt.color = color;

		EditorUtility.DisplayProgressBar("Creating Multiple Sprite", "Extracting sprite", 0);


		var path = AssetDatabase.GetAssetPath(spriteSheet);
		var objs = AssetDatabase.LoadAllAssetsAtPath(path);

		//Sprite[] sprites = Resources.LoadAll<Sprite>(spriteSheet.name);

		float totals = objs.Length;

		for (int i = 0; i < objs.Length; i++){
			var s = objs[i] as Sprite;
                
            if (s != null)
            { // this is a sprite object
				var rect = s.textureRect;
				var pixels = s.texture.GetPixels(rect.x.round(), rect.y.round(), rect.width.round(), rect.height.round());

				int width = s.textureRect.width.round();
				int height = s.textureRect.height.round();
				string outputName = path.Replace(".png","_") + i + ".png"; 
				EditorUtility.DisplayProgressBar("Progressing asset", outputName, i / totals);
				CreateSprite(outputName , width, height, pixels);
                //CreateSprite()



            }
		}

		EditorUtility.ClearProgressBar();

	}
	void CreateSprite(string path, int width, int height, Color [] colors){
		Texture2D tex2D = new Texture2D(width, height);
        Color[] outputColors = new Color[width * height];
        int i = 0;
        for (int y = 0; y < width; y++)
        {
            for (int x = 0; x < height; x++)
            {
				outputColors[i] =   colors[i];
                i++;
            }
        }

        tex2D.SetPixels(colors);
        tex2D.Apply();
        var bytes = tex2D.EncodeToPNG();

		Thread thread = new Thread(() => {
            System.IO.File.WriteAllBytes(path, bytes);

        });

        thread.Start();
        thread.Join();

        AssetDatabase.Refresh();
	}

    void OnWizardUpdate()
    {
        helpString = "Input sprite sheet image.";
    }

    // When the user presses the "Apply" button OnWizardOtherButton is called.
    void OnWizardOtherButton()
    {
        //if (Selection.activeTransform != null)
        //{
        //    Light lt = Selection.activeTransform.GetComponent<Light>();

        //    if (lt != null)
        //    {
        //        lt.color = Color.red;
        //    }
        //}
    }
}