﻿using System;

using System.Drawing;


namespace DW
{
    [Serializable]
    class BiomeDungeon : Biome
    {
        /* valeurs des différents id ajoutés lors de l'application de ca Biome
         * piège
         * porte
         * coffre
         */
        private object[] specialCase = new object[]
        {
            3,
            new Chest(),
            new Pot(),
            new Rock(),
        };



        //<summary>
        //Créer le biome Donjon
        //</summary>
        public BiomeDungeon() :base()
        {

        }

        //<summary>
        //Créer le biome Donjon
        //</summary>
        //<param name="par1stair">Etage du donjon dans lequel est situé le biome</param>
        //<param name="par2x">position x du biome dans l'étage</param>
        //<param name="par3y">position y du biome dans l'étage</param>
        //<param name="par4width">largeur du biome dans l'étage</param>
        //<param name="par5height">hauteur du biome dans l'étage</param>
        public BiomeDungeon(Stair par1stair, int par2x, int par3y, int par4width, int par5height)
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

                }
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
            for (int i = 1; i < w-1; i++)
            {
                for (int u = 1; u < h-1; u++)
                {
                    if (par2room.map[i,u]==1 && rand.Next(0, 100) == 1)
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
                                s.setPos(stair,par2room.x + i, par2room.y + u);
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
                                s.setPos(stair,par2room.x + xp, par2room.y + yp);
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
