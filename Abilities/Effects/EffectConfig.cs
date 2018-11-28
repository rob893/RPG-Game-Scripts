using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class EffectConfig : ScriptableObject
{
	[Header("Effect General")]
	[TextArea]
	public string description;
	public Sprite icon = null;
	public int maxInstancesPerTarget = 1;
	public float duration;
	public GameObject effectPrefab;


	protected Effect behaviour;

	public abstract Effect GetBehaviourComponent(GameObject objectToAttachTo);

	public Effect AttachEffectTo(GameObject objectToattachTo)
	{
		Effect behaviourComponent = GetBehaviourComponent(objectToattachTo);
		behaviourComponent.SetConfig(this);
		behaviour = behaviourComponent;
		return behaviourComponent;
	}
}
