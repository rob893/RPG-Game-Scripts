using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Stat {

	public delegate void OnStatChanged();
	public OnStatChanged onStatChanged;

	[SerializeField] private int baseValue;

	private List<int> modifiers = new List<int>();

	public void AlterBaseValue(int value)
	{
		baseValue += value;

		if (onStatChanged != null)
		{
			onStatChanged.Invoke();
		}
	}

	public void SetBaseValue(int value)
	{
		baseValue = value;

		if (onStatChanged != null)
		{
			onStatChanged.Invoke();
		}
	}

	public int GetValue()
	{
		int finalValue = baseValue;
		modifiers.ForEach(x => finalValue += x);
		return finalValue;
	}

	public void AddModifier(int modifier)
	{
		if(modifier != 0)
		{
			modifiers.Add(modifier);

			if (onStatChanged != null)
			{
				onStatChanged.Invoke();
			}
		}
	}

	public void RemoveModifier(int modifier)
	{
		if (modifier != 0 && modifiers.Contains(modifier))
		{
			modifiers.Remove(modifier);

			if(onStatChanged != null)
			{
				onStatChanged.Invoke();
			}
		}
	}

	public void ClearModifiers()
	{
		modifiers.Clear();

		if (onStatChanged != null)
		{
			onStatChanged.Invoke();
		}
	}
}
