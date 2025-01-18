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

    [AutoPatch(typeof(Duck), nameof(Duck.Equip), PatchType.Prefix)]
    public static bool Duck_Equip_Prefix(Duck __instance, Equipment e)
    {
        return e is not ChestPlate || (!__instance.HasEquipment(typeof(CTArmor)) && !__instance.HasEquipment(typeof(TArmor)));
    }
}