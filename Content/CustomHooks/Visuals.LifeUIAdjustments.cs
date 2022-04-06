﻿using MonoMod.Cil;
using StarlightRiver.Core;
using System;
using Terraria;

namespace StarlightRiver.Content.CustomHooks
{
	class LifeUIAdjustments : HookGroup
	{
		public override SafetyLevel Safety => SafetyLevel.Questionable; //Adjusts a few measurements for the vanilla health UI

		public override void Load()
		{
			if (Main.dedServ)
				return;

			IL.Terraria.Main.DrawInterface_Resources_Life += ShiftText; //PORTTODO: Figure out where this moved to in vanilla
		}

		private void ShiftText(ILContext il)
		{
			ILCursor c = new ILCursor(il);

			for (int k = 0; k < 2; k++)
			{
				c.TryGotoNext(n => n.MatchLdcI4(500));
				c.Index++;
				c.EmitDelegate<Func<int, int>>(stringConcatDelegate);
			}
		}

		private int stringConcatDelegate(int arg)
		{
			Player Player = Main.LocalPlayer;
			var sp = Player.GetModPlayer<BarrierPlayer>();

			if (sp.Barrier <= 0 && sp.MaxBarrier <= 0)
				return arg;

			return arg - (int)(Terraria.GameContent.FontAssets.MouseText.Value.MeasureString($"  {sp.Barrier}/{sp.MaxBarrier}").X / 2) - 6;
		}
	}
}
