//-----------------------------------------------------
//            Arbor 3: FSM & BT Graph Editor
//		  Copyright(c) 2014-2020 caitsithware
//-----------------------------------------------------
using UnityEngine;
using System.Collections.Generic;

namespace ArborEditor
{
	[System.Reflection.Obfuscation(Exclude = true)]
	[System.Serializable]
	internal sealed class TransformInfoCache
	{
		public Object target = null;
		public Vector3 position = Vector3.zero;
		public Vector3 scale = Vector3.one;

		public override string ToString()
		{
			return string.Format("{0} : {1} , {2}", target, position, scale);
		}
	}

	[System.Reflection.Obfuscation(Exclude = true)]
	[System.Serializable]
	internal sealed class TransformCache : ISerializationCallbackReceiver
	{
		[System.Reflection.Obfuscation(Exclude = true)]
		[SerializeField]
		private List<TransformInfoCache> _Transforms = new List<TransformInfoCache>();

		private Dictionary<Object, TransformInfoCache> _DicTransforms = new Dictionary<Object, TransformInfoCache>();

		public bool HasTransform(Object target)
		{
			return (object)target != null && _DicTransforms.ContainsKey(target);
		}

		private TransformInfoCache GetTransform(Object target)
		{
			TransformInfoCache transform = null;
			if ((object)target != null && _DicTransforms.TryGetValue(target, out transform))
			{
				return transform;
			}
			return null;
		}

		public Vector3 GetPosition(Object target)
		{
			TransformInfoCache transform = GetTransform(target);
			if (transform != null)
			{
				return transform.position;
			}
			return Vector3.zero;
		}

		public Vector3 GetScale(Object target)
		{
			TransformInfoCache transform = GetTransform(target);
			if (transform != null)
			{
				return transform.scale;
			}
			return Vector3.one;
		}

		private TransformInfoCache GetOrCreateTransform(Object target)
		{
			if (target == null)
			{
				return null;
			}

			TransformInfoCache transform = GetTransform(target);
			if (transform == null)
			{
				transform = new TransformInfoCache();
				transform.target = target;
				_Transforms.Add(transform);
				_DicTransforms.Add(target, transform);
			}

			return transform;
		}

		public void SetPosition(Object target, Vector3 position)
		{
			TransformInfoCache transform = GetOrCreateTransform(target);
			if (transform != null)
			{
				transform.position = position;
			}
		}

		public void SetScale(Object target, Vector3 scale)
		{
			TransformInfoCache transform = GetOrCreateTransform(target);
			if (transform != null)
			{
				transform.scale = scale;
			}
		}

		public void Clear()
		{
			_Transforms.Clear();
			_DicTransforms.Clear();
		}

		void ISerializationCallbackReceiver.OnAfterDeserialize()
		{
			if (_DicTransforms != null)
			{
				_DicTransforms.Clear();
			}
			else
			{
				_DicTransforms = new Dictionary<Object, TransformInfoCache>();
			}

			foreach (TransformInfoCache transform in _Transforms)
			{
				if (transform.target != null)
				{
					_DicTransforms.Add(transform.target, transform);
				}
			}
		}

		void ISerializationCallbackReceiver.OnBeforeSerialize()
		{
			_Transforms.Clear();
			foreach (var pair in _DicTransforms)
			{
				_Transforms.Add(pair.Value);
			}
		}
	}
}