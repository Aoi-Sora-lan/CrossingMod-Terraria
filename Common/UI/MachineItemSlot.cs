using System;
using CrossingMachine.Common.LoggerSink;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Serilog;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.UI;
using Terraria.UI.Chat;

namespace CrossingMachine.Common.UI;

public class MachineItemSlot : UIElement
{
    private Item[] _itemArray;
    private int _itemIndex;
    private readonly bool _isOutput;
    private int _itemSlotContext = 3;
    private bool _isActivate = true;

    public void SetActivate(bool activate)
    {
        _isActivate = activate;
    }
    
    public MachineItemSlot(Item[] itemArray, int itemIndex, bool isOutput)
    {
        _itemArray = itemArray;
        _itemIndex = itemIndex;
        _isOutput = isOutput;
        Width = new StyleDimension(48f, 0f);
        Height = new StyleDimension(48f, 0f);
    }

    private void HandleItemSlotLogic()
    {
        if (base.IsMouseHovering)
        {
            Main.LocalPlayer.mouseInterface = true;
            if (_isOutput)
            {
                HandleOutputOnly(_itemArray, 3,_itemIndex);;
            }
            else
            {
                Item inv = _itemArray[_itemIndex];
                ItemSlot.OverrideHover(ref inv, _itemSlotContext);
                ItemSlot.LeftClick(ref inv, _itemSlotContext);
                ItemSlot.RightClick(ref inv, _itemSlotContext);
                ItemSlot.MouseHover(ref inv, _itemSlotContext);
                _itemArray[_itemIndex] = inv;
            }
        }
        
    }
    
    
      public static void HandleOutputOnly(Item[] inv, int context = 100, int slot = 0)
    {
        // 只调用部分处理方法，限制功能
        OverrideHoverOutputOnly(inv, context, slot);
        
        // 允许左键点击取出物品
        if (Main.mouseLeft && Main.mouseLeftRelease)
        {
            LeftClickOutputOnly(inv, context, slot);
        }
        
        // 允许右键点击取出部分物品
        if (Main.mouseRight && Main.mouseRightRelease)
        {
            RightClickOutputOnly(inv, context, slot);
        }
        
        // 正常显示悬停信息
        //MouseHover(inv, context, slot);
    }
    
    private static void OverrideHoverOutputOnly(Item[] inv, int context, int slot)
    {
        Item item = inv[slot];
        
        // 禁用所有放入物品的光标提示
        if (item.IsAir)
        {
            // 空槽位，不显示任何特殊光标
            Main.cursorOverride = CursorOverrideID.DefaultCursor;
            return;
        }
        
        // 有物品的槽位，只允许取出的操作
        if (Main.mouseItem.IsAir)
        {
            // 鼠标空时显示可取出的光标
            Main.cursorOverride = CursorOverrideID.DefaultCursor;
        }
        else
        {
            // 鼠标有物品时显示禁止光标
            Main.cursorOverride = CursorOverrideID.DefaultCursor;
        }
    }
    
    private static void LeftClickOutputOnly(Item[] inv, int context, int slot)
    {
        Item item = inv[slot];
        
        // 只有鼠标没有物品时才能取出
        if (Main.mouseItem.IsAir && !item.IsAir)
        {
            // 交换物品
            Utils.Swap(ref inv[slot], ref Main.mouseItem);
            // 播放声音
            SoundEngine.PlaySound(SoundID.Grab);
            // 刷新合成表
            if (Main.mouseItem.type > 0 || inv[slot].type > 0)
            {
                Recipe.FindRecipes();
            }
        }
    }
    
    private static void RightClickOutputOnly(Item[] inv, int context, int slot)
    {
        Item item = inv[slot];
        
        // 只有鼠标没有物品时才能取出部分堆叠
        if (Main.mouseItem.IsAir && !item.IsAir && item.stack > 1)
        {
            // 计算取出的数量（一半或一个）
            int transferAmount = (int)Math.Ceiling(item.stack / 2.0);
            
            // 将部分物品转移到鼠标
            Main.mouseItem = item.Clone();
            Main.mouseItem.stack = transferAmount;
            item.stack -= transferAmount;
            
            // 如果槽位物品堆叠为0，则清空
            if (item.stack <= 0)
            {
                inv[slot] = new Item();
            }
            
            // 播放声音
            SoundEngine.PlaySound(SoundID.MenuTick);
            
            // 刷新合成表
            Recipe.FindRecipes();
        }
        else if (Main.mouseItem.IsAir && !item.IsAir && item.stack == 1)
        {
            // 单个物品直接交换
            LeftClickOutputOnly(inv, context, slot);
        }
    }
    
    protected override void DrawSelf(SpriteBatch spriteBatch)
    {
        if(_isActivate) HandleItemSlotLogic();
        Color color = Color.White;
        Item item = _itemArray[_itemIndex];
        Vector2 position = GetDimensions().Center() + new Vector2(36f, 36f) * -0.5f;
        if (item.type > 0 && item.stack > 0)
        {
            ItemSlot.DrawItemIcon(item, 3, spriteBatch, position+Vector2.One*18f, 36/48f, 32f, color);
            if (item.stack > 1)
            {
                try
                {
                    ChatManager.DrawColorCodedStringWithShadow(spriteBatch, FontAssets.ItemStack.Value,
                        item.stack.ToString(), position + new Vector2(8f, 20f) * 1, color, 0f, Vector2.Zero,
                        new Vector2(36/48f), -1f, 1);
                }
                catch (Exception e)
                {
                    Log.Information("ChatManager Error");
                }
            }
                
        }
    }

    public void SetSlots(Item[] items)
    {
        _itemArray = items;
    }
}