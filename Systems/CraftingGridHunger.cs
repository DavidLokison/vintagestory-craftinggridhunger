using System;
using Vintagestory.API.Common;
using Vintagestory.API.Config;
using Vintagestory.API.Server;

namespace CraftingGridHunger.ModContent
{
    public class ModSystemCraftingGridHunger : ModSystem
    {
        CraftingGridHungerConfig? config;

        public override bool ShouldLoad(EnumAppSide forSide)
        {
            return forSide == EnumAppSide.Server;
        }

        public override void Start(ICoreAPI api)
        {
            TryLoadConfig(api);
        }

        private void TryLoadConfig(ICoreAPI api) 
        {
            try
            {
                config = api.LoadModConfig<CraftingGridHungerConfig>("CraftingGridHunger.json");
                if (config == null)
                {
                    config = new CraftingGridHungerConfig();
                    api.StoreModConfig<CraftingGridHungerConfig>(config, "CraftingGridHunger.json");
                }
            }
            catch (Exception e)
            {
                Mod.Logger.Error("Could not load config! Loading default settings instead.");
                Mod.Logger.Error(e);
                config = new CraftingGridHungerConfig();
            }
        }

        public override void StartServerSide(ICoreServerAPI api)
        {
            base.StartServerSide(api);

            api.Event.PlayerJoin += Event_PlayerJoin;
        }

        private void Event_PlayerJoin(IServerPlayer byPlayer)
        {
            IInventory inv = byPlayer.InventoryManager.GetOwnInventory(GlobalConstants.craftingInvClassName);
            inv.SlotModified += (slotid) => updateHungerStat(inv, byPlayer);

            updateHungerStat(inv, byPlayer);
        }

        private void updateHungerStat(IInventory inv, IServerPlayer player)
        {
            if (config == null) return;
            StatModifiers allmod = new StatModifiers();

            foreach (var slot in inv)
            {
                if (slot.Empty) continue;
                if (inv.GetSlotId(slot) == 9) continue;
                allmod.hungerrate += config.HungerPenaltyPerOccupiedSlot;
            }

            EntityPlayer entity = player.Entity;
            entity.Stats
                .Set("hungerrate", "craftinggridhunger", allmod.hungerrate, true)
            ;
        }

    }
}
