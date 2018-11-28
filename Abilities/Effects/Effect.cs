using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Effect : MonoBehaviour
{

	public string effectName;
	public Sprite icon = null;
	public int maxInstancesPerTarget = 1;
	public float duration;
	public GameObject effectPrefab;
	public CharacterStats MyStats { get; set; }
	public CharacterStats TheirStats { get; set; }

	protected float timer;
	protected EffectConfig config;
	protected Dictionary<string, List<Effect>> theirActiveEffects;

	public virtual void SetConfig(EffectConfig effectConfig)
	{
		config = effectConfig;
		effectName = config.name;
		icon = config.icon;
		maxInstancesPerTarget = config.maxInstancesPerTarget;
		duration = config.duration;
		effectPrefab = config.effectPrefab;

	}
	protected virtual void Start()
	{

		theirActiveEffects = TheirStats.activeEffects;

		if (!theirActiveEffects.ContainsKey(effectName))
		{
			theirActiveEffects.Add(effectName, new List<Effect>());
			theirActiveEffects[effectName].Add(this);
		}
		else if (theirActiveEffects[effectName].Count < maxInstancesPerTarget)
		{
			theirActiveEffects[effectName].Add(this);
		}
		else
		{
			LogicBeforeDestroy();
			Destroy(this);
			return;
		}

		timer = 0;
		StartCoroutine(EffectLogic());
	}

	protected virtual void Update()
	{
		timer += Time.deltaTime;
	}

	protected void RemoveAndDestroy()
	{
		theirActiveEffects[effectName].Remove(this);
		if (theirActiveEffects[effectName].Count == 0)
		{
			theirActiveEffects.Remove(effectName);
		}
	
		Destroy(this);
	}

	public void ResetEffectTimer()
	{
		timer = 0;
	}

	protected abstract void LogicBeforeDestroy();

	protected abstract IEnumerator EffectLogic();
}
