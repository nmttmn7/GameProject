
public class StatusAbilityAction : GameAction
{
	public AbilityRoot abilityRoot;
	public Ability castedAbility;
	
	public bool skip = false;
	public StatusAbilityAction(AbilityRoot abilityRoot)
	{
		this.abilityRoot = abilityRoot;
	}
}
