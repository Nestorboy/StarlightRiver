﻿using StarlightRiver.Core;
using Terraria;
using Terraria.ID;

namespace StarlightRiver.Food.Content.Mains
{
    internal class MonsterSteak : Ingredient
    {
        public MonsterSteak() : base("+3% critical strike chance", 900, IngredientType.Main) { }

        public override bool Autoload(ref string name)
        {
            StarlightNPC.NPCLootEvent += LootMonsterSteak;
            return true;
        }

        public override void BuffEffects(Player player, float multiplier)
        {
            player.meleeCrit += 3;
            player.rangedCrit += 3;
            player.magicCrit += 3;
        }

        private void LootMonsterSteak(NPC npc)
        {
            if (npc.type == NPCID.FaceMonster && Main.rand.Next(4) == 0)
                Item.NewItem(npc.Center, item.type);
        }
    }
}
