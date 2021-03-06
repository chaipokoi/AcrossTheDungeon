﻿using System;
using System.Drawing;
using System.Threading;

using SdlDotNet.Graphics;

namespace DW
{
    class InventoryUI
    {

        private Inventory inventory;
        private Surface background = new Surface("Data/images/GUI/InventoryUI_background.png");
        private Surface slot = new Surface("Data/images/GUI/Slot.png");
        private Surface selector = new Surface(new Size(32, 32));
        private Surface Icon = new Surface(new Size(30, 30));
        private bool opened = false;
        private int index;
        private int xIndex;
        private int yIndex;
        private bool inSelection = false;
        private int SelectionIndex = 0;
        private Item[] contents;

        public InventoryUI(Inventory par1)
        {
            inventory = par1;
            index=0;
            contents = inventory.getContents();
            Icon = Icon.Convert(Video.Screen);
            selector.Fill(Color.FromArgb(0, 255, 0));
        }

        //<summary>
        //paramètre la fenetre d'inventaire pour afficher l'inventaire passé en paramètre
        //</summary>
        //<param name="par1">L'inventaire à affecter à la fentre d'inventaire.</param>
        public void setInventory(Inventory par1)
        {
            inventory = par1;
            contents = inventory.getContents();
        }

        //<summary>
        //si ouvert, affiche l'inventaire à l'écran
        //</summary>
        public void update()
        {
            if (opened)
            {
                Video.Screen.Blit(background, new Point(70, 90));
                new Text("pixel.ttf", 30, 90, 100, "Inventaire",200,200,200).update();
                updateSelector();
                int y = 0;
                int x = 0;
                for (int i = 0; i < inventory.getSize(); i++)
                {
                    if (x == 11)
                    {
                        y += 1;
                        x = 0;
                    }
                    Video.Screen.Blit(slot, new Point(105 + (40 * x), 145 + 40 * y));
                    if (contents[i] != null && DW.render.getSprite(contents[i].getName())!=null )
                        Video.Screen.Blit(DW.render.getSprite(contents[i].getName()), new Point(107 + (40 * x), 147 + 40 * y));
                    x += 1;
                }
                if (inSelection)
                {
                    if (contents[index] != null)
                    {
                        Video.Screen.Blit(Icon, new Point(90, 240));
                        string d = contents[index].getDescription();
                        int part = d.Split("\n".ToCharArray()).Length;
                        if (part <= 1)
                            new Text("pixel.ttf", 20, 90 + 70, 262, contents[index].getDescription()).update();
                        else if (part > 1)
                        {
                            int yu = 262 - (part * 25 / 2) + 25 / 2;
                            for (int i = 0; i < part; i++)
                            {
                                new Text("pixel.ttf", 20, 90 + 70, yu + i * 25, d.Split("\n".ToCharArray())[i]).update();
                            }
                        }
                        new Text("pixel.ttf", 20, 100, 350, "Prendre en Main").update();
                        if(contents[index].getAction() != null)
                            new Text("pixel.ttf", 20, 640/2, 360, contents[index].getAction(),255,255,255,TypePos.Center).update();
                        new Text("pixel.ttf", 20, 490, 350, "Lacher").update();
                        int xt = 90;
                        if (SelectionIndex == 1)
                            xt = 270;
                        else if (SelectionIndex == 2)
                            xt = 390+90;
                        new Text("pixel.ttf", 20, xt, 350, ">").update();
                    }
                    else
                        inSelection = false;
                }
            }
        }

        //<summary>
        //actualise la position des différents sélécteurs affichés à l'ecran.
        //scan les entrée de touche afin de réagir en conséquence.
        //</summary>
        private void updateSelector()
        {
            if (inSelection == false)
            {
                if (DW.input.equals(SdlDotNet.Input.Key.Escape))
                    opened=false;
                else if (DW.input.equals(SdlDotNet.Input.Key.RightArrow) && index < inventory.getSize() - 1)
                {
                    index += 1;
                    xIndex += 1;
                    if (xIndex == 11)
                    {
                        yIndex += 1;
                        xIndex = 0;
                    }
                    Thread.Sleep(50);
                }
                else if (DW.input.equals(SdlDotNet.Input.Key.LeftArrow) && index > 0)
                {
                    index -= 1;
                    xIndex -= 1;
                    if (xIndex == -1)
                    {
                        yIndex -= 1;
                        xIndex = 10;
                    }
                    Thread.Sleep(50);
                }
                else if (DW.input.equals(SdlDotNet.Input.Key.KeypadEnter) || DW.input.equals(SdlDotNet.Input.Key.Return))
                {
                    if (contents[index] != null)
                    {
                        Icon = new Surface(new Size(30, 30)).Convert(Video.Screen);
                        Icon.Fill(Color.Fuchsia);
                        if(DW.render.getSprite(contents[index].getName()) != null)
                            Icon.Blit(DW.render.getSprite(contents[index].getName()), new Point(0, 0));
                        Icon = Icon.CreateScaledSurface(2D, false);
                        Icon.SourceColorKey = Color.Fuchsia;
                        inSelection = true;
                        Thread.Sleep(100);
                    }
                }
            }
            else
            {
                if (DW.input.equals(SdlDotNet.Input.Key.Escape))
                {
                    inSelection = false;
                    Thread.Sleep(100);
                }
                else if (DW.input.equals(SdlDotNet.Input.Key.LeftArrow))
                {
                    if (contents[index].getAction() != null)
                        SelectionIndex -= 1;
                    else
                        SelectionIndex -= 2;
                    Thread.Sleep(100);
                }
                else if (DW.input.equals(SdlDotNet.Input.Key.RightArrow))
                {
                    if(contents[index].getAction() != null)
                        SelectionIndex += 1;
                    else
                        SelectionIndex += 2;
                    Thread.Sleep(100);
                }
                if (SelectionIndex < 0)
                    SelectionIndex = 0;
                else if (SelectionIndex > 2)
                    SelectionIndex = 2;
                else if (DW.input.equals(SdlDotNet.Input.Key.KeypadEnter) || DW.input.equals(SdlDotNet.Input.Key.Return))
                {
                    if (SelectionIndex == 0)
                    {
                        Item i = ((Player)inventory.getOwner()).getItemInHand();
                        ((Player)inventory.getOwner()).setItemInHand(contents[index]);
                        contents[index] = i;
                    }
                    else if(SelectionIndex==2)
                    {
                        inventory.removeItem(index, true);
                        contents = inventory.getContents();
                    }
                    else if (SelectionIndex == 1)
                    {
                        contents[index] = contents[index].interact(((Entity)inventory.getOwner()));
                    }
                    inSelection = false;
                    SelectionIndex = 0;
                    Thread.Sleep(100);
                }
            }
            Video.Screen.Blit(selector, new Point(107 + 40 * xIndex, 147 + 40 * yIndex));
        }

        //<summary>
        //ouvre l'inventaire si fermé. Ferme l'inventaire si ouvert.
        //</summary>
        public void open()
        {
            inSelection = false;
            if (opened == false)
                opened = true;
            else
                opened = false;
            Thread.Sleep(100);
        }

        //<summary>
        //retourne si la fenètre d'inventaire est ouverte ou fermée.
        //</summary>
        public bool isOpenned()
        {
            return opened;
        }
    }
}
