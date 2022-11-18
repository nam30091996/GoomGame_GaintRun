//-----------------------------------------------------
//            Arbor 3: FSM & BT Graph Editor
//		  Copyright(c) 2014-2020 caitsithware
//-----------------------------------------------------
using UnityEditor;

namespace ArborEditor.Calculators
{
	using Arbor;
	using Arbor.Calculators;

	[CustomEditor(typeof(RandomSelectComponent))]
	internal sealed class RandomSelectComponentInspector : NodeBehaviourEditor
	{
		RandomSelectComponent _Target;
		SerializedProperty _WeightsProperty;
		OutputSlotComponentProperty _OutputProperty;
		ClassConstraintInfo _ConstraintInfo = null;

		private void OnEnable()
		{
			_Target = target as RandomSelectComponent;

			_ConstraintInfo = new ClassConstraintInfo();

			_WeightsProperty = serializedObject.FindProperty("_Weights");
			_OutputProperty = new OutputSlotComponentProperty(serializedObject.FindProperty("_Output"));
		}

		void OnPreValueField(SerializedProperty valueProperty)
		{
			if (valueProperty.serializedObject.targetObject != target)
			{
				return;
			}

			FlexibleComponentProperty flexibleComponentProperty = new FlexibleComponentProperty(valueProperty);

			flexibleComponentProperty.overrideConstraintType = (_ConstraintInfo != null) ? _ConstraintInfo.GetConstraintBaseType() : null;
		}

		public override void OnInspectorGUI()
		{
			serializedObject.Update();

			_ConstraintInfo.baseType = _Target.type;

			EditorGUILayout.PropertyField(_OutputProperty.typeProperty.property);

			WeightListProeprtyEditor.Callback callback = new WeightListProeprtyEditor.Callback();
			callback.onPreValueFieldCallback += OnPreValueField;

			_WeightsProperty.SetStateData(callback);

			EditorGUILayout.PropertyField(_WeightsProperty);

			_WeightsProperty.RemoveStateData<WeightListProeprtyEditor.Callback>();

			DataSlotField slotField = _OutputProperty.dataSlotField;
			if (slotField != null && _ConstraintInfo != null)
			{
				slotField.overrideConstraint = _ConstraintInfo;
			}
			EditorGUILayout.PropertyField(_OutputProperty.property);

			serializedObject.ApplyModifiedProperties();
		}
	}
}