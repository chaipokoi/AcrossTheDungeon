﻿using System;


using System.Drawing;

namespace DW
{
    [Serializable]
    class BiomeCave : Biome
    {

        /* valeurs des différents id ajoutés lors de l'application de ca Biome
        * piège
        * porte
        * coffre
        */
        private object[] specialCase = new object[]
        {
            new Rock(),
        };

        private new Entity[] entities = new Entity[]
            {
                new Bat()
            };

        public BiomeCave()
            : base()
        {

        }

        public BiomeCave(Stair par1stair, int par2x, int par3y, int par4width, int par5height)
            : base(par1stair, par2x, par3y, par4width, par5height)
        { }

        //<summary>
        //applique les spécificité du biome aux diverses salles de ce dernier
        //</summary>
        public override void apply()
        {
            for (int i = 0; i < rooms.Length; i++)
            {
                if (rooms[i] != null)
                {
                    applySpecialCase(rooms[i]);
                    if(rand.Next(0,3)==0)
                        genCaseInZone(7, rooms[i]);
                    applyWater(rooms[i]);
                    applyGravel(rooms[i]);
                    applyLava(rooms[i]);
                }
            }
        }

        private void applyLava(Room par1room)
        {
            int x = rand.Next(1, par1room.width - 1);
            int y = rand.Next(1, par1room.height - 1);
            int w = rand.Next(2, 7);
            int h = rand.Next(2, 7);
            for (int i = x; i < w; i++)
            {
                for (int u = y; u < h; u++)
                {
                    try
                    {
                        if (stair.map[i + par1room.x, u + par1room.y] != 2)
                        {
                            stair.set(101, i + par1room.x, u + par1room.y);
                            par1room.set(101, i, u);
                        }
                    }
                    catch(Exception)
                    {}
                }
            }
            /*
            int tried = 0;
            int xp = rand.Next(1, par1room.width - 1);
            int yp = rand.Next(1, par1room.height - 1);
            while (par1room.map[xp, yp] != 1)
            {
                if (tried >= 500)
                    return;
                xp = rand.Next(1, par1room.width - 1);
                yp = rand.Next(1, par1room.height - 1);
                tried += 1;
            }
            tried = 0;
            par1room.set(101, xp, yp);
            stair.set(101, par1room.x + xp, par1room.y + yp);
            for (int i = 0; i < rand.Next(5, 15); i++)
            {
                xp = rand.Next(1, par1room.width - 1);
                yp = rand.Next(1, par1room.height - 1);
                while (!(par1room.map[xp - 1, yp] == 101 || par1room.map[xp + 1, yp] == 101 || par1room.map[xp, yp - 1] == 101 || par1room.map[xp, yp + 1] == 101) || par1room.map[xp, yp] != 1)
                {
                    if (tried >= 500)
                        return;
                    xp = rand.Next(1, par1room.width - 1);
                    yp = rand.Next(1, par1room.height - 1);
                    tried += 1;
                }
                par1room.set(101, xp, yp);
                stair.set(101, par1room.x + xp, par1room.y + yp);
            }*/
        }

        //<summary>
        //Pose des graviers dans la salle.
        //<summary>
        private void applyGravel(Room par2room)
        {
            int u = rand.Next(1, 10);
            int tried = 0;
            for (int i = 0; i < u; i++)
            {
                int xGravel = rand.Next(1, par2room.width - 1);
                int yGravel = rand.Next(1, par2room.height - 1);

                while (par2room.map[xGravel, yGravel] != 1 && par2room.map[xGravel - 1, yGravel] == 101 && par2room.map[xGravel + 1, yGravel] == 101 && par2room.map[xGravel, yGravel - 1] == 101 && par2room.map[xGravel, yGravel+1] == 101)
                {
                    if (tried >= 500)
                        return;
                    tried += 1;
                    xGravel = rand.Next(1, par2room.width - 1);
                    yGravel = rand.Next(1, par2room.height - 1);
                }

                par2room.set(5, xGravel, yGravel);
                stair.set(5, par2room.x + xGravel, par2room.y + yGravel);
            }
        }

        //<summary>
        //génère les entitées inhérantes au biome
        //</summary>
        //<param name="par1">nombre maximal d'entités à générer</param>
        public override Entity[] applyEntities(int par1)
        {
            if (par1 > 0)
            {
                Entity[] res = new Entity[par1];
                for (int i = 0; i < par1; i++)
                {
                    Point p = new Point(-1, -1);
                    p = stair.getFreeSpecialCase(new Rectangle(x,y,width,height));
                    int tried = 0;
                    while (contains(p.X, p.Y) == false)
                    {
                        tried += 1;
                        p = stair.getFreeSpecialCase();
                        if (tried >= 500)
                        {
                            Console.WriteLine("fail");
                            break;
                        }
                    }
                    if (p.X == -1 && p.Y == -1)
                        continue;
                    int ty = rand.Next(0, entities.Length);
                    res[i] = (Entity)(entities[ty].clone());
                    res[i].setPos(stair, p.X, p.Y);
                }
                return res;
            }
            else
            {
                return null;

            }
        }

        //<summary>
        //applique les spécificité du biome à la salle de ce dernier passée en argument
        //</summary>
        //<param name="par2room">salle à affecter</param>
        public void applySpecialCase(Room par2room)
        {
            int w = par2room.width;
            int h = par2room.height;
            for (int i = 1; i < w - 1; i++)
            {
                for (int u = 1; u < h - 1; u++)
                {
                    if (par2room.map[i, u] == 1 && rand.Next(0, 70) == 1)
                    {
                        int r = rand.Next(0, specialCase.Length);
                        try
                        {
                            stair.set((int)specialCase[r], par2room.x + i, par2room.y + u);
                            par2room.set((int)specialCase[r], i, u);
                        }
                        catch (System.InvalidCastException)
                        {
                            if (specialCase[r].GetType() != new Pot().GetType() || !(specialCase[r] is Chest))
                            {
                                Special s = (((Special)specialCase[r]).clone());
                                s.setPos(stair, par2room.x + i, par2room.y + u);
                                stair.setSpecial(s, par2room.x + i, par2room.y + u);
                                par2room.setSpecial(s, i, u);
                            }
                            else
                            {
                                int xp = rand.Next(1, par2room.width);
                                int yp = rand.Next(1, par2room.height);
                                while (par2room.map[xp + 1, yp] != 2 && par2room.map[xp - 1, yp] != 2 && par2room.map[xp, yp + 1] != 2 && par2room.map[xp, yp - 1] != 2)
                                {
                                    xp = rand.Next(1, par2room.width);
                                    yp = rand.Next(1, par2room.height);
                                }
                                Special s = ((Special)specialCase[r]);
                                s = (Special)s.clone();
                                s.setPos(stair, par2room.x + xp, par2room.y + yp);
                                stair.setSpecial(s, par2room.x + xp, par2room.y + yp);
                                par2room.setSpecial(s, xp, yp);
                                stair.getSpecial()[par2room.x + xp, par2room.y + yp].setPos(stair, par2room.x + xp, par2room.y + yp);
                                int nb = rand.Next(1, 6);
                                if (specialCase[r] is Pot)
                                {
                                    for (int p = 0; p < nb; p++)
                                    {
                                        try
                                        {
                                            xp = rand.Next(1, par2room.width);
                                            yp = rand.Next(1, par2room.height);
                                            while ((par2room.map[xp + 1, yp] != 2 && par2room.map[xp - 1, yp] != 2 && par2room.map[xp, yp + 1] != 2 && par2room.map[xp, yp - 1] != 2) && (par2room.getSpecial()[xp + 1, yp].GetType() != specialCase[r].GetType() && par2room.getSpecial()[xp - 1, yp].GetType() != specialCase[r].GetType() && par2room.getSpecial()[xp, yp + 1].GetType() != specialCase[r].GetType() && par2room.getSpecial()[xp, yp - 1].GetType() != specialCase[r].GetType()))
                                            {
                                                xp = rand.Next(1, par2room.width);
                                                yp = rand.Next(1, par2room.height);
                                            }
                                            s = ((Special)specialCase[r]);
                                            s = (Special)s.clone();
                                            s.setPos(stair, par2room.x + xp, par2room.y + yp);
                                            stair.setSpecial(s, par2room.x + xp, par2room.y + yp);
                                            stair.getSpecial()[par2room.x + xp, par2room.y + yp].setPos(stair, par2room.x + xp, par2room.y + yp);
                                            par2room.setSpecial(s, xp, yp);
                                        }
                                        catch (Exception) { }
                                    }
                                }
                            }
                        }
                        catch (System.IndexOutOfRangeException) { }
                    }

                }
            }
        }
    }
}