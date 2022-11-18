//-----------------------------------------------------
//            Arbor 3: FSM & BT Graph Editor
//		  Copyright(c) 2014-2020 caitsithware
//-----------------------------------------------------
using UnityEngine;
#if UNITY_5_5_OR_NEWER
using UnityEngine.AI;
#endif
using UnityEditor;

using Arbor;

namespace ArborEditor
{
	internal static class AgentCreator
	{
		[MenuItem("GameObject/Arbor/AgentController", false, 20)]
		static void CreateAgent(MenuCommand menuCommand)
		{
			GameObject gameObject = new GameObject("Agent", typeof(NavMeshAgent), typeof(AgentController));
			GameObjectUtility.SetParentAndAlign(gameObject, menuCommand.context as GameObject);
			Undo.RegisterCreatedObjectUndo(gameObject, "Create Agent");
			Selection.activeGameObject = gameObject;
		}
	}
}
