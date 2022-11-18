using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.Analytics;
using KR;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class UILayout : Editor {

    [MenuItem("GameObject/UI/Layout/Panel/Empty", false, 0)]
    public static void CreateEmptyPanel()
    {
        var rect = CreateNav("[DIV] Empty Panel", 1f, 1f, new Vector2(0.5f, 0.5f), new Vector2(.5f, .5f));
        //var pos = rect.localPosition;
        //var size = rect.sizeDelta;
        rect.anchorMin = Vector2.zero;
        rect.anchorMax = Vector2.one;
        //rect.localPosition = pos;
        //rect.sizeDelta = size;
        rect.offsetMin = Vector2.zero;
        rect.offsetMax = Vector2.zero;

    }

    [MenuItem("GameObject/UI/Layout/Panel/Black", false, 0)]
    public static void CreateBlackPanel()
    {
        var rect = CreateNav("[DIV] Black Panel", 1f, 1f, new Vector2(0.5f, 0.5f), new Vector2(.5f, .5f));
        //var pos = rect.localPosition;
        //var size = rect.sizeDelta;
        rect.anchorMin = Vector2.zero;
        rect.anchorMax = Vector2.one;
        //rect.localPosition = pos;
        //rect.sizeDelta = size;
        rect.offsetMin = Vector2.zero;
        rect.offsetMax = Vector2.zero;
	    var img = 	rect.gameObject.AddComponent<Image>();
		img.color = new Color(0, 0, 0, 0.6f);

    }

	[MenuItem("GameObject/UI/Layout/Center/SM", false, 0)]
    public static void CreateCenterNavSM()
    {
		var rect = CreateNav("[NAV] Center Sm", 2f, 2f, new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f));
		rect.localPosition = Vector3.zero;
		rect.gameObject.AddComponent<Image>();

    }

	[MenuItem("GameObject/UI/Layout/Center/MD", false, 0)]
    public static void CreateCenterNavMD()
    {
		var rect = CreateNav("[NAV] Center MD", 1.5f, 1.5f, new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f));
        rect.localPosition = Vector3.zero;
		rect.gameObject.AddComponent<Image>();
    }

	[MenuItem("GameObject/UI/Layout/Center/LG", false, 0)]
    public static void CreateCenterNavLG()
    {
		var rect = CreateNav("[NAV] Center LG", 1.25f, 1.25f, new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f));
        rect.localPosition = Vector3.zero;
		rect.gameObject.AddComponent<Image>();

    }



	[MenuItem("GameObject/UI/Layout/Top/SM", false, 0)]
	public static void CreateTopNavSM (){
		CreateNav("[NAV] Top Sm", 1, 5 , new Vector2(0.5f, 1f), new Vector2(0.5f, 1f));
	}

	[MenuItem("GameObject/UI/Layout/Top/MD", false, 0)]
    public static void CreateTopNavMD()
    {
		CreateNav("[NAV] Top Md", 1, 3.5f, new Vector2(0.5f, 1f), new Vector2(0.5f, 1f));
    }
	[MenuItem("GameObject/UI/Layout/Top/LG", false, 0)]
    public static void CreateTopNavLG()
    {
		CreateNav("[NAV] Top Lg", 1, 2, new Vector2(0.5f, 1f), new Vector2(0.5f, 1f));
    }

	[MenuItem("GameObject/UI/Layout/Right/SM", false, 0)]
    public static void CreateRightNavSM()
    {
        CreateNav("[NAV] Right Sm", 5, 1, new Vector2(1f, 0.5f), new Vector2(1f, 0.5f));
    }

	[MenuItem("GameObject/UI/Layout/Right/MD", false, 0)]
    public static void CreateRightNavMD()
    {
        CreateNav("[NAV] Right Md", 3.5f, 1, new Vector2(1f, 0.5f), new Vector2(1f, 0.5f));
	}

    [MenuItem("GameObject/UI/Layout/Right/LG", false, 0)]
    public static void CreateRightNavLG()
    {
        CreateNav("[NAV] Right Lg", 2f, 1, new Vector2(1f, 0.5f), new Vector2(1f, 0.5f));
    }


	[MenuItem("GameObject/UI/Layout/Left/SM", false, 0)]
	public static void CreateLeftNavSM()
    {
		CreateNav("[NAV] Left Sm", 5, 1, new Vector2(0f, 0.5f), new Vector2(0f, 0.5f), new Vector2(-1f + (1 / 5f), 0));
    }

	[MenuItem("GameObject/UI/Layout/Left/MD", false, 0)]
	public static void CreateLefttNavMD()
    {
		CreateNav("[NAV] Left Md", 3.5f, 1, new Vector2(0f, 0.5f), new Vector2(0f, 0.5f), new Vector2(-1f + (1 / 3.5f), 0));
    }

	[MenuItem("GameObject/UI/Layout/Left/LG", false, 0)]
	public static void CreateLeftNavLG()
    {
		CreateNav("[NAV] Left Lg", 2f, 1, new Vector2(0f, 0.5f), new Vector2(0f, 0.5f), new Vector2(-1f + (1 / 2f), 0));
    }



    
	[MenuItem("GameObject/UI/Layout/Bottom/SM", false, 0)]
    public static void CreateBottomNavSM()
    {
		CreateNav("[NAV] Bottom Sm", 1, 5, new Vector2(0.5f, 0f), new Vector2(0.5f, 0f), new Vector2(0, -1f +  (1 / 5f)));
    }

    [MenuItem("GameObject/UI/Layout/Bottom/MD", false, 0)]
	public static void CreateBottomNavMD()
    {
		CreateNav("[NAV] Bottom Md", 1, 3.5f, new Vector2(0.5f, 0f), new Vector2(0.5f, 0f), new Vector2(0, -1f + (1 / 3.5f)));
    }
    [MenuItem("GameObject/UI/Layout/Bottom/LG", false, 0)]
	public static void CreateBottomNavLG()
    {
		CreateNav("[NAV] Bottom Lg", 1, 2f, new Vector2(0.5f, 0f), new Vector2(0.5f, 0f), new Vector2(0, -1f + (1 / 2f)));
    }





	public static RectTransform CreateNav(string name, float widthDevide, float heightDevide, Vector2 anchorMin, Vector2 anchorMax, Vector2 offset = default(Vector2)){
		heightDevide = heightDevide.clamp(1);
		var selection = Selection.activeObject as GameObject;
		Canvas parent = null;
		if (selection)
			parent = selection.GetComponent<Canvas>() ??
							  selection.GetComponentInParent<Canvas>() ??
							  selection.GetComponentInChildren<Canvas>(); 
			              

		if (parent == null)
            parent = FindObjectOfType<Canvas>();
            
		if (parent == null)
		{
            
			//create new parent.
			GameObject canvas = new GameObject("UI");
			parent = canvas.AddComponent<Canvas>();
			parent.renderMode = RenderMode.ScreenSpaceOverlay;

			canvas.AddComponent<GraphicRaycaster>();
			var scaler = canvas.AddComponent<CanvasScaler>();
			scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
			scaler.screenMatchMode = CanvasScaler.ScreenMatchMode.Expand;
			scaler.referenceResolution = new Vector2(Screen.width, Screen.height);
			Debug.Log("Created new Canvas.");

			var esy = FindObjectOfType<EventSystem>();
			if (esy == null)
			{
				esy =  new GameObject("EventSystem").AddComponent<EventSystem>();
				esy.gameObject.AddComponent<StandaloneInputModule>();

				Debug.Log("Created new EventSystem.");
				Debug.Log("Created new StandaloneInputModule.");

			}
		}
        
		RectTransform rect = new GameObject(name).AddComponent<RectTransform>();
		var selectionRect = selection?.GetComponent<RectTransform>();
        

		rect.SetParent(selectionRect ? selectionRect : parent.transform);
		var size = parent.GetComponent<RectTransform>().sizeDelta;
		var width = size.x / widthDevide;
		var height = size.y / heightDevide;
        //var x = 
        //var y 
		CreateNavigation(ref rect, (size.x -  width) / 2f, (size.y -  height) / 2f, width, height, anchorMin, anchorMax, offset);
		return rect;
	}
	public static void CreateNavigation(ref RectTransform rect, float x, float y, float width, float height,
	                                    Vector2 anchorMin, Vector2 anchorMax, Vector2 offset){
		rect.sizeDelta = new Vector2(width, height);
		rect.anchorMin = anchorMin;
        rect.anchorMax = anchorMax;
		offset =  new Vector2(offset.x * Screen.width, offset.y * Screen.height);
		Debug.Log("Offset: " + offset);
		rect.localPosition = new Vector2(x, y) + offset;
		Selection.activeObject = rect.gameObject;
	
	}
}
