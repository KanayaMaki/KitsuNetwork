using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct BufferProperty<Type> {
	
	public Type Set(Type v)
	{
		value = v;
		isChanged = true;
		return value;
	}
	public Type Get() {
		return value;
	}

	public bool IsChanged()
	{
		return isChanged;
	}
	public void SetNotChanged()
	{
		isChanged = false;
	}

	bool isChanged;

	[SerializeField]
	Type value;
}
