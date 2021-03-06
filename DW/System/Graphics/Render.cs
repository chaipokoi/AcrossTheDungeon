﻿using System;
using System.IO;
using System.Collections.Generic;


using System.Drawing;
using SdlDotNet.Core;
using SdlDotNet.Graphics;
using SdlDotNet.Graphics.Sprites;


namespace DW
{
    class Render
    {
        /* Index de position dans le fichier Tileset.png
         * vide
         * sol
         * mur
         * piège
         * ?
         * gravier
         * herbe
         * transition sol-herbe
         * sable
         * trou
         */
        public int[] id = new int[]
        {
            3,
            0,
            1,
            14,
            -1,
            13,
            12,
            23,
            24,
        };


        private int frame;
        private int entityFrame = 0;
        public int x;
        public int y;
        private KeyValuePair<string, Surface>[] handledItem = new KeyValuePair<string, Surface>[2];

        /*GUI*/
        private StatUI statUI;
        private InventoryUI inventoryUI;
        private RecipeUI recipeUI;
        public SpellsUI spellsUI;

        /*Managers*/
        public AnimationManager animationManager = new AnimationManager();

        /*RenderMap*/
        private Surface shadow = new Surface(30, 30).Convert(Video.Screen);
        private Surface tileset = new Surface("Data/images/TileSet.png");
        private KeyValuePair<string, Sprite>[] spriteDictionnary = new KeyValuePair<string, Sprite>[500];
        protected AnimationCollection waterAnimation;
        protected AnimatedSprite water;
        protected AnimationCollection lavaAnimation;
        protected AnimatedSprite lava;
        private SdlDotNet.Graphics.Font font = new SdlDotNet.Graphics.Font(Directory.GetCurrentDirectory() + "\\Data\\" + "pixel.ttf", 30);

        public Render(int par1x, int par2y)
        {
            x = par1x;
            y = par2y;
            shadow.AlphaBlending = true;
            setAnimatedTile();
            registerDictionnary();
        }

        //<summary>
        //Associe chaque caractère de texte à un sprite correspondant
        //</summary>
        private void registerDictionnary()
        {
            /**************************************************Entities*/
            /*Player*/
            addToSpriteDictionnary("@", "Data/images/Hero.png");
            /*OtherPlayer*/
            addToSpriteDictionnary("à", "Data/images/Hero.png");
            /*Bat*/
            addToSpriteDictionnary("V", "Data/images/Entity/Bat.png");
            /*Pig*/
            addToSpriteDictionnary("P", "Data/images/Entity/Pig.png");
            /*Pnj*/
            addToSpriteDictionnary("Pnj", "Data/images/Entity/Pnj.png");
            /*************************************************Elements*/
            /*Door*/
            addToSpriteDictionnary("|", "Data/images/Elements/Door.png",false);
            addToSpriteDictionnary("|o", "Data/images/Elements/Door_side.png",false);
            /*Plant*/
            addToSpriteDictionnary(",", "Data/images/Elements/Plant/1.png",false);
            addToSpriteDictionnary("\"", "Data/images/Elements/Plant/2.png",false);
            addToSpriteDictionnary("!", "Data/images/Elements/Plant/3.png",false);
            addToSpriteDictionnary("T", "Data/images/Elements/Plant/4.png",false);
            /*Pot*/
            addToSpriteDictionnary("U", "Data/images/Elements/Pot.png",false);
            /*Rock(sur la map)*/
            addToSpriteDictionnary("rock", "Data/images/Elements/Rock.png",false);
            /*Forge(map)*/
            addToSpriteDictionnary("F/", "Data/images/Elements/Forge.png");
            /*************************************************Items*/
            /*baie*/
            addToSpriteDictionnary("baie", "Data/images/Items/Berry.png",false);
            /*cookie*/
            addToSpriteDictionnary("cookie", "Data/images/Items/Cookie.png",false);
            /*Dust*/
            addToSpriteDictionnary("Poussière fine", "Data/images/Items/Dust.png",false);
            /*PickAxe*/
            addToSpriteDictionnary("Pioche", "Data/images/Items/PickAxe.png",false);
            /*IronMineral*/
            addToSpriteDictionnary("Minerais de fer", "Data/images/Items/IronMineral.png",false);
            /*ObsidianMineral*/
            addToSpriteDictionnary("Minerais d'obsidienne", "Data/images/Items/ObsidianMineral.png",false);
            /*IronIngot*/
            addToSpriteDictionnary("Morceau de fer", "Data/images/Items/IronIngot.png",false);
            /*Obsidian*/
            addToSpriteDictionnary("Obsidienne", "Data/images/Items/Obsidian.png",false);
            /*Axe*/
            addToSpriteDictionnary("Hache", "Data/images/Items/Axe.png",false);
            /*Wood*/
            addToSpriteDictionnary("Bois", "Data/images/Items/Wood.png",false);
            /*Rock*/
            addToSpriteDictionnary("Roche", "Data/images/Items/Rock.png",false);
            /*Bucket*/
            addToSpriteDictionnary("Seau", "Data/images/Items/Bucket.png", false);
            /*BucketLava*/
            addToSpriteDictionnary("Seau de lave", "Data/images/Items/BucketLava.png", false);
            /*BucketWater*/
            addToSpriteDictionnary("Seau d'eau", "Data/images/Items/BucketWater.png", false);
            /*BucketSand*/
            addToSpriteDictionnary("Seau de sable", "Data/images/Items/BucketSand.png", false);
            /*Forge(inventaire)*/
            addToSpriteDictionnary("Forge", "Data/images/Items/Pack.png");
        }

        private void setAnimatedTile()
        {
            waterAnimation = new AnimationCollection();
            SurfaceCollection e = new SurfaceCollection();
            e.Add("Data/images/Water.png", new Size(30, 30));
            waterAnimation.Add(e);
            waterAnimation.Delay = 1200;
            water = new AnimatedSprite(waterAnimation);
            water.Animate = true;

            lavaAnimation = new AnimationCollection();
            e = new SurfaceCollection();
            e.Add("Data/images/Lava.png", new Size(30, 30));
            lavaAnimation.Add(e);
            lavaAnimation.Delay = 1200;
            lava = new AnimatedSprite(lavaAnimation);
            lava.Animate = true;
        }

        public void setUI(Player par1)
        {
            statUI = new StatUI(par1);
            inventoryUI = new InventoryUI(par1.getInventory());
            recipeUI = new RecipeUI(par1, null);
            spellsUI = new SpellsUI(par1,210, 430,new Spell[]{Spell.weaponAttack});
        }

        public void openInventory()
        {
            inventoryUI.open();
        }

        public void openRecipe()
        {
            recipeUI.open();
        }

        public void setRecipe(Special par1)
        {
            recipeUI.setTool(par1);
        }

        public void setInventory(Inventory par1)
        {
            inventoryUI.setInventory(par1);
        }

        public bool isUIOpenned()
        {
            if (inventoryUI.isOpenned() || recipeUI.isOpenned())
                return true;
            else
                return false;
        }

        public StatUI getStatUI()
        {
            return statUI;
        }


        //<summary>
        //associe la case spéciale passé en paramètre au sprite donné
        //</summary>
        //<param name="par1special">La case spéciale</param>
        //<param name="par2path">le chemin du sprite à associer</param>
        public void addToSpriteDictionnary(Special par1special,string par2path)
        {
            for (int i = 0; i < spriteDictionnary.Length; i++)
            {
                if (spriteDictionnary[i].Key == null && File.Exists(par2path))
                {
                    AnimationCollection a = new AnimationCollection();
                    SurfaceCollection e = new SurfaceCollection();
                    e.Add(par2path, new Size(30, 30));
                    a.Add(e);
                    a.Delay = 200;
                    AnimatedSprite s = new AnimatedSprite(a);
                    s.Animate = true;
                    spriteDictionnary[i] = new KeyValuePair<string, Sprite>(par1special.getValue(), (Sprite)s);
                    return;
                }
                if (spriteDictionnary[i].Key == par1special.getValue())
                    return;
            }
        }

        //<summary>
        //associe la case spéciale passé en paramètre au sprite donné
        //</summary>
        //<param name="par1special">Le caractère de la case spéciale</param>
        //<param name="par2path">le chemin du sprite à associer</param>
        public void addToSpriteDictionnary(string par1char, string par2path,bool animated=true)
        {
            for (int i = 0; i < spriteDictionnary.Length; i++)
            {
                if (spriteDictionnary[i].Key == null && File.Exists(par2path))
                {
                    if (animated)
                    {
                        AnimationCollection a = new AnimationCollection();
                        SurfaceCollection e = new SurfaceCollection();
                        e.Add(par2path, new Size(30, 30));
                        a.Add(e);
                        a.Delay = 200;
                        AnimatedSprite s = new AnimatedSprite(a);
                        s.Animate = true;
                        spriteDictionnary[i] = new KeyValuePair<string, Sprite>(par1char, s);
                        return;
                    }
                    else
                        spriteDictionnary[i] = new KeyValuePair<string, Sprite>(par1char, new Sprite(par2path));
                }
                if (spriteDictionnary[i].Key == par1char)
                    return;
            }
        }


        //<summary>
        //met à jour le compteur de frame pour les annimations
        //</summary>
        public void update()
        {
            entityFrame += 1;
            if (entityFrame > 3)
                entityFrame = 0;
            frame += 1;
            if (frame > 40)
                frame = 0;
            if(statUI != null)
                statUI.update();
            if(spellsUI != null)
                spellsUI.update();
            if (inventoryUI != null)
                inventoryUI.update();
            if (recipeUI != null)
                recipeUI.update();
            animationManager.update();
        }

        //<summary>
        //retourne le sprite animé associé à l'objet spécifié precedemment enregistré grace à la méthode registerSprite
        //</summary>
        //<param name="par1">objet associé au sprite animé</param>
        private Sprite getSprite(Special par1)
        {
            for (int i = 0; i < spriteDictionnary.Length; i++)
            {
                if (!spriteDictionnary[i].Equals(null))
                {
                    if (spriteDictionnary[i].Key == par1.getValue())
                    {
                        if (spriteDictionnary[i].Value is AnimatedSprite)
                        {
                            AnimatedSprite e = (AnimatedSprite)spriteDictionnary[i].Value;
                            switch (par1.getFace())
                            {
                                case "front":
                                    if (e.Frame > 3)
                                        e.Frame = 0;
                                    break;
                                case "left":
                                    if (e.Frame < 8)
                                        e.Frame = 8;
                                    if (e.Frame > 11)
                                        e.Frame = 8;
                                    break;
                                case "right":
                                    if (e.Frame < 4)
                                        e.Frame = 4;
                                    if (e.Frame > 7)
                                        e.Frame = 4;
                                    break;
                                case "back":
                                    if (e.Frame < 12)
                                        e.Frame = 12;
                                    if (e.Frame > 15)
                                        e.Frame = 12;
                                    break;
                            }
                            return e;
                        }
                        else
                            return spriteDictionnary[i].Value;
                    }
                }
            }
            return null;
        }

        //<summary>
        //retourne le sprite animé associé à l'objet spécifié precedemment enregistré grace à la méthode registerSprite
        //</summary>
        //<param name="par1">caractère associé au sprite animé</param>
        public Sprite getSprite(string par1)
        {
            for (int i = 0; i < spriteDictionnary.Length; i++)
            {
                if (!spriteDictionnary[i].Equals(null))
                {
                    if (spriteDictionnary[i].Key == par1)
                    {
                        if (spriteDictionnary[i].Value is AnimatedSprite)
                        {
                            AnimatedSprite e = (AnimatedSprite)spriteDictionnary[i].Value;
                            return e;
                        }
                        else
                            return spriteDictionnary[i].Value;
                    }
                }
            }
            return null;
        }


        //<summary>
        //affiche la map et les entitésdans le périmètre de vision de l'entité spécifiée
        //</summary>
        //<param name="par1">l'entité</param>
        public void renderEntityVision(Entity par1)
        {
            if (par1.getStair() != null)
            {
                int[,] par1map = par1.getStair().map;
                Special[,] specials = par1.getStair().getSpecial();
                for (int i = (int)(par1.x - par1.getRange()*2); i < par1.x + par1.getRange()*2; i++)
                {
                    for (int u = (int)(par1.y - par1.getRange()*2); u < par1.y + par1.getRange()*2; u++)
                    {
                        if (u >= 0 && i >= 0 && i <= par1.getStair().width && u <= par1.getStair().height && (Math.Abs(par1.x - i) + Math.Abs(par1.y - u))<9)
                        {
                                if (par1map[i, u] < 100)
                                {
                                    if (par1map[i, u] < id.Length && id[par1map[i, u]] != -1)
                                    {
                                        if (par1map[i, u] == 2)
                                            connectionTile(par1map, i, u);
                                        else
                                            Video.Screen.Blit(tileset, new Point(x + i * 30, y + u * 30), new Rectangle(id[par1map[i, u]] * 30, 0, 30, 30));      
                                    }
                                }
                                else
                                    renderAnimated(par1map, i, u);
                                if (specials[i, u] != null)
                                    renderSpecialAt(specials, i, u);
                            }
                            if (!par1.canSee(i, u))
                            {
                                if (i <= par1.x - par1.getRange() || i >= par1.x + par1.getRange() || u <= par1.y - par1.getRange() || u >= par1.y + par1.getRange())
                                {
                                        shadow.Alpha = (byte)(100 + (Math.Abs(par1.x - i) + Math.Abs(par1.y - u)) * 40);
                                }
                                else
                                    shadow.Alpha = 100;
                                Video.Screen.Blit(shadow, new Point(x + i * 30, y + u * 30));
                            }
                    }
                }
                Entity[] ent = par1.getStair().getEntities();
                for (int i = 0; i < ent.Length; i++)
                {
                    if (ent[i] != null)
                    {
                        int xe = ent[i].x;
                        int ye = ent[i].y;
                        if (par1.canSee(xe, ye))
                        {
                                renderEntityAt(ent[i]);
                        }
                    }
                }
            }
        }

        //<summary>
        //adapte les sprites de terrain en fcontion de leur environnnement direct
        //</summary>
        //<param name="par1map">La map</param>
        //<param name="par2x">La position x de la tuile a afficher</param>
        //<param name="par3y">La position y de la tuile a afficher</param>
        private void connectionTile(int[,] par1map, int par2x,int par3y)
        {
                int idToRender = par1map[par2x, par3y];
                if (idToRender == 2)
                {
                    try
                    {
                        /*Wall*/
                        if (par1map[par2x - 1, par3y] != 2 && par1map[par2x + 1, par3y] != 2 && par1map[par2x, par3y + 1] != 2 && par1map[par2x, par3y + 1] != 0)
                            idToRender = id[2]+1;
                        else if (par1map[par2x, par3y + 1] != 2 && par1map[par2x, par3y + 1] != 0)
                            idToRender = id[2] + 1;
                        else
                            idToRender = id[2] + 2;
                     }
                    catch (Exception)
                    {
                        idToRender = id[2] + 2;
                    }
                    Video.Screen.Blit(tileset, new Point(x + par2x * 30, y + par3y * 30), new Rectangle(idToRender * 30, 0, 30, 30));
                    return;
                }

        }


        //<summary>
        //affiche l'entité spécifiée
        //</summary>
        //<param name="par1">l'entité</param>
        private void renderEntityAt(Entity par1)
        {
            AnimatedSprite e=(AnimatedSprite)getSprite(par1);
            if (e != null)
            {
                Video.Screen.Blit(e, new Point(x + par1.x * 30, y + par1.y * 30));
                if (par1 is Player && ((Player)par1).getItemInHand() != null)
                {
                    Player p = ((Player)par1);
                    int ind = -1;
                    if (p.itemInHandChanged())
                    {
                        for (int i = 0; i < handledItem.Length; i++)
                        {
                            if (handledItem[i].Key == p.getName())
                            {
                                ind = i;
                                break;
                            }
                        }
                        Surface h = new Surface(new Size(30, 30)).Convert(Video.Screen);
                        h.Fill(Color.Fuchsia);
                        if (getSprite(p.getItemInHand().getName()) != null)
                            h.Blit(getSprite(p.getItemInHand().getName()), new Point(0, 0));
                        h = h.CreateScaledSurface(0.53D, false);
                        h.SourceColorKey = Color.Fuchsia;
                        if (ind != -1)
                            handledItem[ind] = new KeyValuePair<string, Surface>(p.getName(), h);
                        else
                        {
                            for (int i = 0; i < handledItem.Length; i++)
                            {
                                if(handledItem[i].Key==null)
                                    handledItem[i] = new KeyValuePair<string, Surface>(p.getName(), h);
                            }
                        }
                    }
                    for (int i = 0; i < handledItem.Length; i++)
                    {
                        if (handledItem[i].Key == p.getName())
                        {
                            Point r;
                            if(p.getFace() != "left" && p.getFace() != "right")
                                r=new Point(x + par1.x * 30 +14, y + par1.y * 30+10);
                            else
                                r=new Point(x + par1.x * 30 +5, y + par1.y * 30 + 10);

                                Video.Screen.Blit(handledItem[i].Value, r);
                            break;
                        }
                    }
                    

                }
            }
            else
                Video.Screen.Blit(font.Render(par1.getChar(), par1.getColor()), new Point(x + par1.x * 30, y + par1.y * 30));
        }


        //<summary>
        //affiche la case contenant un objet spécial situé aux coordonées spécicifiées
        //</summary>
        //<param name="par1map">la liste des objets spéciaux contenant l'objet à afficher</param>
        //<param name="par2x">la position x de l'objet à afficher</param>
        //<param name="par3y">la position y de l'objet à afficher</param>
        private void renderSpecialAt(Special[,] par1map, int par2x, int par3y)
        {
                Sprite e = getSprite(par1map[par2x, par3y]);
                if (e != null)
                    Video.Screen.Blit(e, new Point(x + par2x * 30, y + par3y * 30));
                else
                    Video.Screen.Blit(font.Render((string)par1map[par2x, par3y].getChar(), (Color)par1map[par2x, par3y].getColor()), new Point(x + par2x * 30, y + par3y * 30));
        }


        private void renderAnimated(int[,] par1map, int par2x, int par3y)
        {
            if (par1map[par2x, par3y] == 100)
                Video.Screen.Blit(water, new Point(x + par2x * 30, y + par3y * 30));
            else if(par1map[par2x, par3y] == 101)
                Video.Screen.Blit(lava, new Point(x + par2x * 30, y + par3y * 30));
        }


        //<summary>
        //affiche tout les objets spéciaux de la map
        //</summary>
        //<param name="par1map">la liste des objets spéciaux à afficher</param>
        //<param name="par2width">la largeur de la liste des objets spéciaux à afficher</param>
        //<param name="par3height">la hauteur de la liste des objets spéciaux à afficher</param>
        public void renderSpecial(Special[,] par1map, int par2width, int par3height)
        {
            for (int i = 0; i < par2width; i++)
            {
                for (int u = 0; u < par3height; u++)
                {
                    if (par1map[i, u] != null)
                        Video.Screen.Blit(font.Render((string)par1map[i, u].getChar(), (Color)par1map[i, u].getColor()), new Point(x + i * 30, y + u * 30));
                }
            }
        }


        //<summary>
        //affiche tout les entités de la liste spécifiée
        //</summary>
        //<param name="par1">la liste des objets entités à afficher</param>
        public void renderEntity(Entity[] par1)
        {
            for (int i = 0; i < par1.Length; i++)
            {
                if (par1[i] != null)
                {
                    AnimatedSprite e = (AnimatedSprite)getSprite(par1[i]);
                    if (e != null)
                        Video.Screen.Blit(e, new Point(x + par1[i].x * 30, y + par1[i].y * 30));
                    else
                        Video.Screen.Blit(font.Render(par1[i].getChar(), par1[i].getColor()), new Point(x + par1[i].x * 30, y + par1[i].y * 30));
                }
            }
        }


        //<summary>
        //Déplace la caméra aux coordonées spécifiées
        //</summary>
        //<param name="par1x">la pisition x</param>
        //<param name="par2y">la pisition y</param>
        public void move(int par1x, int par2y)
        {
            x = par1x;
            y = par2y;
        }


        //<summary>
        //retourne la psoition x de la caméra
        //</summary>
        public int getX()
        {
            return x;
        }


        //<summary>
        //retourne la psotion y de la caméra
        //</summary>
        public int getY()
        {
            return y;
        }
    }
}

