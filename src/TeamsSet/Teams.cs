using System.Collections.Generic;

namespace DuckGame.C44P;

public static class FuseTeams
{
    public enum FuseTeam {None, CT, T}

    public static Dictionary<Duck, FuseTeam> DuckTeams = new();

    public static void Reset()
    {
        DuckTeams = new();
    }

    public static bool HasTeam(Duck duck, out FuseTeam team)
    {
        team = Team(duck);
        return team is not FuseTeam.None;
    }

    public static FuseTeam Team(Duck duck)
    {
        return DuckTeams.TryGetValue(duck, out var team) ? team : FuseTeam.None;
    }

    public static bool CanPickUp(Duck duck, Holdable t)
    {
        return !DuckTeams.TryGetValue(duck, out var team) || CanPickUp(team, t);
    }

    public static bool CanPickUp(FuseTeam team, Holdable t)
    {
        return t switch
        {
            Holster holster => CanPickUp(team, holster._containedObject),
            Defuser or CTArmor => team is FuseTeam.CT,
            C4 or TArmor => team is FuseTeam.T,
            _ => true
        };
    }
}
