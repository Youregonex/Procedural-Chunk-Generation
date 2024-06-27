
public interface IHaveFaction
{
    public FactionEnum Faction { get; set; }

    public FactionEnum GetFaction() => Faction;
}
