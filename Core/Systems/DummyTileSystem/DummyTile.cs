﻿using StarlightRiver.Content.Packets;
using System.Collections.Generic;
using Terraria.DataStructures;

namespace StarlightRiver.Core.Systems.DummyTileSystem
{
	public abstract class DummyTile : ModTile
	{
		public readonly static Dictionary<Point16, Projectile> dummies = new();

		public virtual int DummyType { get; }

		public Projectile Dummy(int i, int j)
		{
			return GetDummy(i, j, DummyType);
		}

		public static Projectile GetDummy(int i, int j, int type)
		{
			var key = new Point16(i, j);

			if (dummies.TryGetValue(key, out Projectile dummy))
			{
				if (dummy.type == type && dummy.active)
					return dummy;
			}

			if (NonDictSearch(i, j, type))
			{
				if (dummies.TryGetValue(key, out Projectile dummy2))
				{
					if (dummy2.type == type && dummy2.active)
						return dummy2;
				}
			}

			return null;
		}

		public static Projectile GetDummy<T>(int i, int j) where T : Dummy
		{
			var key = new Point16(i, j);

			if (dummies.TryGetValue(key, out Projectile dummy))
			{
				if (dummy.ModProjectile is T && dummy.active)
					return dummy;
			}

			if (NonDictSearch<T>(i, j))
			{
				if (dummies.TryGetValue(key, out Projectile dummy2))
				{
					if (dummy2.ModProjectile is T && dummy2.active)
						return dummy2;
				}
			}

			return null;
		}

		public static bool DummyExists(int i, int j, int type)
		{
			if (GetDummy(i, j, type) != null)
				return true;

			return NonDictSearch(i, j, type);
		}

		public static bool DummyExists<T>(int i, int j) where T : Dummy
		{
			if (GetDummy<T>(i, j) != null)
				return true;

			return NonDictSearch<T>(i, j);
		}


		/// <summary>
		/// searches through projectiles for a dummy that's not found in the dictionary, and adds it to the dictionary if found
		/// primarily for multiplayer since its more likely to fail to be attached to a key there
		/// </summary>
		/// <param name="i"></param>
		/// <param name="j"></param>
		/// <param name="type"></param>
		/// <returns></returns>
		private static bool NonDictSearch(int i, int j, int type)
		{
			for (int k = 0; k < Main.maxProjectiles; k++)
			{
				Projectile proj = Main.projectile[k];

				if (proj.active && proj.type == type && (proj.position / 16).ToPoint16() == new Point16(i, j))
				{
					var key = new Point16(i, j);
					dummies[key] = proj;
					return true;
				}
			}

			return false;
		}

		/// <summary>
		/// searches through projectiles for a dummy that's not found in the dictionary, and adds it to the dictionary if found
		/// primarily for multiplayer since its more likely to fail to be attached to a key there
		/// </summary>
		/// <param name="i"></param>
		/// <param name="j"></param>
		/// <param name="type"></param>
		/// <returns></returns>
		private static bool NonDictSearch<T>(int i, int j) where T : Dummy
		{
			for (int k = 0; k < Main.maxProjectiles; k++)
			{
				Projectile proj = Main.projectile[k];

				if (proj.active && proj.ModProjectile is T && (proj.position / 16).ToPoint16() == new Point16(i, j))
				{
					var key = new Point16(i, j);
					dummies[key] = proj;
					return true;
				}
			}

			return false;
		}

		public virtual void PostSpawnDummy(Projectile dummy) { }

		public virtual void SafeNearbyEffects(int i, int j, bool closer) { }

		public virtual bool SpawnConditions(int i, int j)
		{
			Tile tile = Main.tile[i, j];
			return tile.TileFrameX == 0 && tile.TileFrameY == 0;
		}

		public sealed override void NearbyEffects(int i, int j, bool closer)
		{
			if (!Main.tileFrameImportant[Type] || SpawnConditions(i, j))
			{
				int type = DummyType;//cache type here so you dont grab the it from a dict every single iteration
				Projectile dummy = Dummy(i, j);

				if (dummy is null || !dummy.active)
				{
					if (Main.netMode == Terraria.ID.NetmodeID.MultiplayerClient)
					{
						var packet = new SpawnDummy(Main.myPlayer, type, i, j);
						packet.Send(-1, -1, false);
						return;
					}

					var p = new Projectile();
					p.SetDefaults(type);

					Vector2 spawnPos = new Vector2(i, j) * 16 + p.Size / 2;
					int n = Projectile.NewProjectile(new EntitySource_WorldEvent(), spawnPos, Vector2.Zero, type, 1, 0);

					var key = new Point16(i, j);
					dummies[key] = Main.projectile[n];

					PostSpawnDummy(Main.projectile[n]);
				}
			}

			SafeNearbyEffects(i, j, closer);
		}
	}
}