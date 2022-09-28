using GameData.Common;
using GameData.Domains;
using GameData.Domains.Character;
using GameData.Domains.Map;
using GameData.Domains.Organization;
using HarmonyLib;
using HarmonyLib.Tools;
using System;
using System.Collections.Generic;
using TaiwuModdingLib.Core.Plugin;

namespace Taiwu_AddInfected
{
    [PluginConfig("自动恢复魔者", "ReachingFoul", "1.0.0")]
    public class AddInfectedPlugin : TaiwuRemakePlugin
    {
        private Harmony harmony;
        public override void Dispose()
        {
            if (this.harmony == null)
                return;
            this.harmony.UnpatchSelf();
        }

        public virtual void onModSettingUpdate() { }

        public override void Initialize()
        {
            this.harmony = Harmony.CreateAndPatchAll(typeof(AddInfectedPlugin), (string)null);
            HarmonyFileLog.Enabled = true;
        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(MapDomain), "Move", new Type[] { typeof(DataContext), typeof(short), typeof(bool) })]
        public static void MapDomain_AddInfectedPlugin_Prefix(MapDomain __instance, short destBlockId, ref DataContext context)
        {
            GameData.Domains.Character.Character taiwu = DomainManager.Taiwu.GetTaiwu();
            Location newLocation = new Location(taiwu.GetLocation().AreaId, destBlockId);
            MapBlockData mapBlock = __instance.GetBlock(newLocation);
            if (mapBlock.InfectedCharacterSet != null)
            {
                HashSet<int> infectedChars = mapBlock.InfectedCharacterSet;
                foreach (int charId in infectedChars)
                {
                    Character infectedChar = DomainManager.Character.GetElement_Objects(charId);
                    infectedChar.AddFeature(context, 216, true);
                    infectedChar.SetXiangshuInfection(0, context);

                    if (newLocation.AreaId >= 135)
                    {
                        OrganizationInfo defaultOrg = new OrganizationInfo(0, 0, true, -1);
                        infectedChar.SetOrganizationInfo(defaultOrg, context);
                    }

                    sbyte stateIdByAreaId = DomainManager.Map.GetStateIdByAreaId(newLocation.AreaId);
                    short randomSettlementId = DomainManager.Map.GetRandomSettlementId(stateIdByAreaId, context.Random, false);
                    Settlement settlement = DomainManager.Organization.GetSettlement(randomSettlementId);
                    OrganizationInfo organizationInfo4 = new OrganizationInfo(settlement.GetOrgTemplateId(), 0, true, randomSettlementId);
                    infectedChar.SetOrganizationInfo(organizationInfo4, context);
                    mapBlock.AddCharacter(charId);
                }
                mapBlock.InfectedCharacterSet.Clear();
            }
        }
    }
}