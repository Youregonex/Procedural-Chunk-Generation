
public class PlayerChunkInteraction : AgentNetworkBehaviourComponent
{
    public override void DisableComponent() => enabled = false;

    public override void EnableComponent() => enabled = true;

    public override void Initialize() {}
}
