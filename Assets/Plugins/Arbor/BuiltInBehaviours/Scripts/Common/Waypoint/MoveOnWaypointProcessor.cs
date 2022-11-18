//-----------------------------------------------------
//            Arbor 3: FSM & BT Graph Editor
//		  Copyright(c) 2014-2020 caitsithware
//-----------------------------------------------------
using UnityEngine;

namespace Arbor
{
	public sealed class MoveOnWaypointProcessor
	{
		private MoveWaypointType _MoveType;
		private WaypointTimeType _TimeType;
		private float _Speed;
		public Vector3 currentPosition
		{
			get;
			private set;
		}

		private WaypointProcessor _Processor = new WaypointProcessor();

		public bool isDone
		{
			get;
			private set;
		}

		public bool isPlaying
		{
			get;
			private set;
		}

		private float _BeginTime = 0f;
		private float _Time = 0;
		private Vector3 _PrevPoint = Vector3.zero;
		private Vector3 _NextPoint = Vector3.zero;
		private Vector3 _Offset = Vector3.zero;

		public void Play(Waypoint waypoint, Vector3 position, Vector3 offset, MoveWaypointType moveType, WaypointTimeType timeType, float speed, bool clearDestPoint)
		{
			_Processor.Initialize(waypoint, clearDestPoint);
			_MoveType = moveType;
			_TimeType = timeType;
			_Speed = speed;
			_Offset = offset;

			currentPosition = position;

			isDone = false;
			isPlaying = _Processor.isValid && _Speed > 0.0f;

			GotoNextPoint(false);
		}

		public void Play(Waypoint waypoint, Vector3 position, MoveWaypointType moveType, WaypointTimeType timeType, float speed, bool clearDestPoint)
		{
			Play(waypoint, position, Vector3.zero, moveType, timeType, speed, clearDestPoint);
		}

		void GotoNextPoint(bool moveDone)
		{
			if (!isPlaying)
			{
				return;
			}

			if (moveDone)
			{
				_Processor.Next(_MoveType);
				if (_Processor.isDone)
				{
					isDone = true;
					isPlaying = false;
					return;
				}
			}

			Transform nextPoint = _Processor.nextPoint;

			_PrevPoint = currentPosition;
			_NextPoint = nextPoint.position + _Offset;
			_BeginTime = WaypointTimeUtility.CurrentTime(_TimeType);

			_Time = _Speed > 0f ? Vector3.Distance(_PrevPoint, _NextPoint) / _Speed : 0f;
		}

		public void Update()
		{
			if (!isPlaying)
			{
				return;
			}

			float currentTime = WaypointTimeUtility.CurrentTime(_TimeType);
			float t = _Time > 0.0f ? Mathf.Clamp01((currentTime - _BeginTime) / _Time) : 0f;
			currentPosition = Vector3.Lerp(_PrevPoint, _NextPoint, t);

			if (Mathf.Approximately(_Time, 0f) || Mathf.Approximately(t, 1f))
			{
				GotoNextPoint(true);
			}
		}
	}
}