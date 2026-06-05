using Vintagestory.API.Common;
using Vintagestory.API.Config;
using Vintagestory.API.Server;

namespace CraftingGridHunger.ModContent
{
    public class ModSystemCraftingGridHunger : ModSystem
    {
        ICoreAPI? api;

        public override bool ShouldLoad(EnumAppSide forSide)
        {
            return true;
        }

        public override void Start(ICoreAPI api)
        {
            this.api = api;
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
            StatModifiers allmod = new StatModifiers();

            foreach (var slot in inv)
            {
                if (slot.Empty) continue;
                if (inv.GetSlotId(slot) == 9) continue;
                allmod.hungerrate += 0.1f;
            }

            EntityPlayer entity = player.Entity;
            entity.Stats
                .Set("hungerrate", "craftinggridhunger", allmod.hungerrate, true)
            ;
        }

    }
}
