using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectSaver : MonoBehaviour {

	public void UnsetParent()
	{
		transform.parent = null;
	}
}
