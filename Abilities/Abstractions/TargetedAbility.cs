public class TargetedAbility : Ability {

	protected override void Start()
	{
		base.Start();
		requiresTarget = true;
	}
}
