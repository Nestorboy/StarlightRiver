﻿using StarlightRiver.Content.Items.BuriedArtifacts;
using Terraria.DataStructures;
using Terraria.ID;

namespace StarlightRiver.Content.Items.Geomancer
{
	public class SparklingBullet : ModItem
	{
		public override string Texture => AssetDirectory.GeomancerItem + Name;

		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Sparkling Bullet");
			Tooltip.SetDefault("Regenerates on a successful hit");
		}

		public override void SetDefaults()
		{
			Item.width = Item.height = 10;

			Item.value = Item.sellPrice(copper: 10);
			Item.rare = ItemRarityID.Orange;

			Item.maxStack = 20;
			Item.damage = 20;
			Item.knockBack = 1.5f;

			Item.ammo = AmmoID.Bullet;
			Item.consumable = true;

			Item.DamageType = DamageClass.Ranged;
			Item.shoot = ModContent.ProjectileType<SparklingBulletProj>();
			Item.shootSpeed = 0;
		}

		public override void AddRecipes()
		{
			Recipe recipe = CreateRecipe(20);
			recipe.AddIngredient(ModContent.ItemType<ExoticGeodeArtifactItem>(), 1);
			recipe.AddTile(TileID.Anvils);
			recipe.Register();
		}

		public override void OnConsumedAsAmmo(Item weapon, Player player)
		{
			player.GetModPlayer<SparklingBulletPlayer>().bulletsConsumed++;
		}
	}

	internal class SparklingBulletProj : ModProjectile
	{
		public override string Texture => AssetDirectory.GeomancerItem + Name;

		private Player player => Main.player[Projectile.owner];

		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Sparkling Bullet");
			ProjectileID.Sets.TrailCacheLength[Projectile.type] = 6;
			ProjectileID.Sets.TrailingMode[Projectile.type] = 0;
		}

		public override void SetDefaults()
		{
			Projectile.width = Projectile.height = 8;

			Projectile.DamageType = DamageClass.Ranged;
			Projectile.friendly = true;

			Projectile.timeLeft = 640;
			Projectile.penetrate = 1;

			Projectile.aiStyle = 1;
			AIType = ProjectileID.Bullet;
		}

		public override void OnSpawn(IEntitySource source)
		{
			Projectile.velocity *= 0.9f;
		}

		public override void AI()
		{
			Projectile.rotation = Projectile.velocity.ToRotation() + 1.57f;
			Projectile.velocity.Y += 0.1f;

			if (Main.rand.NextBool(10))
				Dust.NewDustPerfect(Projectile.Center, ModContent.DustType<Dusts.ArtifactSparkles.GeodeArtifactSparkleFast>(), Main.rand.NextVector2Circular(0.5f, 0.5f), 0, default, Main.rand.NextFloat(0.85f, 1.15f) * Projectile.scale);
		}

		public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
		{
			if (player.GetModPlayer<SparklingBulletPlayer>().bulletsConsumed > 0)
			{
				player.GetModPlayer<SparklingBulletPlayer>().bulletsConsumed--;
				Item.NewItem(new EntitySource_DropAsItem(Projectile), player.Center, ModContent.ItemType<SparklingBullet>(), 1);
			}
		}

		public override void Kill(int timeLeft)
		{
			for (int i = 0; i < 7; i++)
				Dust.NewDustPerfect(Projectile.Center, ModContent.DustType<Dusts.ArtifactSparkles.GeodeArtifactSparkle>(), Main.rand.NextVector2Circular(2.5f, 2.5f), 0, default, Main.rand.NextFloat(0.85f, 1.15f) * Projectile.scale);
		}
	}

	public class SparklingBulletPlayer : ModPlayer
	{
		public int bulletsConsumed = 0;
	}
}