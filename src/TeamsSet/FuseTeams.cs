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

    public static List<Duck> DucksNotInTeam(FuseTeam team)
    {
        List<Duck> result = new();
        foreach (Duck d in Level.current.things[typeof(Duck)])
            if (Team(d) != team) result.Add(d);

        return result;
    }

    public static List<Duck> DucksInTeam(FuseTeam team)
    {
        List<Duck> result = new();
        foreach (Duck d in Level.current.things[typeof(Duck)])
            if (Team(d) == team) result.Add(d);

        return result;
    }

    public static bool HasTeam(Duck duck, out FuseTeam team)
    {
        team = Team(duck);
        return team is not FuseTeam.None;
    }

    public static FuseTeam Team(Duck? duck)
    {
        if (duck is null) return FuseTeam.None;
        return DuckTeams.TryGetValue(duck, out var team) ? team : FuseTeam.None;
    }

    public static bool CanPickUp(Duck duck, Holdable t)
    {
        return !DuckTeams.TryGetValue(duck, out var team) || (CanPickUp(team, t) && t.canPickUp);
    }

    public static bool CanPickUp(FuseTeam team, Holdable t)
    {
        return t switch
        {
            Defuser or CTArmor => team is FuseTeam.CT && t.canPickUp,
            C4 or TArmor => team is FuseTeam.T && t.canPickUp,
            _ => t.canPickUp
        };
    }
}
