using CrossingMachine.Content.Tiles;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CrossingMachine.Content.Items;

public class UIOpenItem : ModItem
{

	public override void SetDefaults() {
		Item.width = 20; // The item texture's width
		Item.height = 20; // The item texture's height
		Item.useStyle = ItemUseStyleID.DrinkLiquid;

		Item.maxStack = Item.CommonMaxStack; // The item's max stack value
		Item.value = Item.buyPrice(silver: 1); // The value of the item in copper coins. Item.buyPrice & Item.sellPrice are helper methods that returns costs in copper coins based on platinum/gold/silver/copper arguments provided to it.
	}

	public override bool? UseItem(Player player)
	{

		Item.DefaultToPlaceableTile(ModContent.TileType<CrossMachine>());
		return base.UseItem(player);
	}

	// Please see Content/ExampleRecipes.cs for a detailed explanation of recipe creation.
	public override void AddRecipes() {
		CreateRecipe(999)
			.AddIngredient(ItemID.DirtBlock, 10)
			.AddTile(TileID.WorkBenches)
			.Register();
	}
}
