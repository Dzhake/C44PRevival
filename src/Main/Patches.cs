namespace DuckGame.C44P;

public static class Patches
{
    [AutoPatch(typeof(Duck), nameof(Duck.TryGrab), PatchType.Prefix)]
    public static void Duck_TryGrab_Prefix(Duck __instance)
    {
        if (!FuseTeams.HasTeam(__instance, out var team)) return;

        foreach (Holdable h in Level.CheckCircleAll<Holdable>(new Vec2(__instance.x, __instance.y + 4f), 18f))
            h.canPickUp = FuseTeams.CanPickUp(team, h);
    }
}