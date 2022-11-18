//-----------------------------------------------------
//            Arbor 3: FSM & BT Graph Editor
//		  Copyright(c) 2014-2020 caitsithware
//-----------------------------------------------------

namespace ArborEditor
{
	using Arbor;
	public static class ClassTypeConstraintEditorUtility
	{
		public static readonly ClassUnityObjectAttribute unityObject = new ClassUnityObjectAttribute();
		public static readonly ClassAssetObjectAttribute asset = new ClassAssetObjectAttribute();
		public static readonly ClassComponentAttribute component = new ClassComponentAttribute();
		public static readonly ClassScriptableObjectAttribute scriptableObject = new ClassScriptableObjectAttribute();
		public static readonly ClassEnumFieldConstraint enumField = new ClassEnumFieldConstraint();
		public static readonly ClassEnumFlagsConstraint enumFlags = new ClassEnumFlagsConstraint();
	}
}