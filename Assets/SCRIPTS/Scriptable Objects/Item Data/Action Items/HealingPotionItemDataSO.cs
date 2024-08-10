using UnityEngine;

[CreateAssetMenu(menuName = "Item Data/Action Item Data/Healing Potion")]
public class HealingPotionItemDataSO : ActionItemDataSO
{
    [field: SerializeField] public int HealAmount { get; private set; }

    public override void Use(AgentCoreBase user)
    {
        base.Use(user);

        user.GetAgentComponent<AgentHealthSystem>().TakeHealing(HealAmount);
    }
}
