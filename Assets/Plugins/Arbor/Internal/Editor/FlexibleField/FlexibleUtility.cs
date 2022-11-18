//-----------------------------------------------------
//            Arbor 3: FSM & BT Graph Editor
//		  Copyright(c) 2014-2020 caitsithware
//-----------------------------------------------------
using UnityEditor;

using Arbor;

namespace ArborEditor
{
	internal static class FlexibleUtility
	{
		public static void DisconectDataBranch(SerializedProperty slotProperty)
		{
			int branchID = slotProperty.FindPropertyRelative("branchID").intValue;
			NodeBehaviour nodeBehaviour = slotProperty.serializedObject.targetObject as NodeBehaviour;

			NodeGraph nodeGraph = (nodeBehaviour != null) ? nodeBehaviour.nodeGraph : null;
			if (nodeGraph != null)
			{
				DataBranch branch = nodeGraph.GetDataBranchFromID(branchID);
				if (branch != null)
				{
					nodeGraph.DeleteDataBranch(branch);
				}
			}
		}
	}
}