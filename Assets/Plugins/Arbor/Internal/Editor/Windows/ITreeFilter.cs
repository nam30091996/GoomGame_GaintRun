//-----------------------------------------------------
//            Arbor 3: FSM & BT Graph Editor
//		  Copyright(c) 2014-2020 caitsithware
//-----------------------------------------------------
namespace ArborEditor
{
	public interface ITreeFilter<T>
	{
		bool useFilter
		{
			get;
		}

		bool openFilter
		{
			get;
			set;
		}

		void OnFilterSettingsGUI();
		bool IsValid(T value);
	}
}