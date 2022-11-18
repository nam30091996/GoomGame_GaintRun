//-----------------------------------------------------
//            Arbor 3: FSM & BT Graph Editor
//		  Copyright(c) 2014-2020 caitsithware
//-----------------------------------------------------
using UnityEngine;
using System.Collections.Generic;

using Arbor;

namespace ArborEditor
{
	[CustomNodeDuplicator(typeof(StateLinkRerouteNode))]
	internal sealed class StateLinkRerouteNodeDuplicator : NodeDuplicator
	{
		protected override Node OnDuplicate()
		{
			ArborFSMInternal stateMachine = targetGraph as ArborFSMInternal;

			StateLinkRerouteNode sourceRerouteNode = sourceNode as StateLinkRerouteNode;
			StateLinkRerouteNode rerouteNode = null;
			if (isClip)
			{
				rerouteNode = stateMachine.CreateStateLinkRerouteNode(Vector2.zero, sourceRerouteNode.nodeID);
			}
			else
			{
				rerouteNode = stateMachine.CreateStateLinkRerouteNode(Vector2.zero);
			}

			if (rerouteNode != null)
			{
				rerouteNode.link = new StateLink(sourceRerouteNode.link);
				rerouteNode.direction = sourceRerouteNode.direction;
			}

			return rerouteNode;
		}

		public override void OnAfterDuplicate(List<NodeDuplicator> duplicators)
		{
			StateLinkRerouteNode sourceRerouteNode = sourceNode as StateLinkRerouteNode;
			StateLinkRerouteNode rerouteNode = destNode as StateLinkRerouteNode;

			if (!isClip)
			{
				bool isSameNodeGraph = Clipboard.IsSameNodeGraph(sourceRerouteNode.nodeGraph, rerouteNode.nodeGraph);
				Clipboard.CheckStateLink(rerouteNode.link, isSameNodeGraph, rerouteNode.nodeGraph, duplicators);
			}
		}
	}
}