﻿using System;

using System.Drawing;

namespace DW
{
    [Serializable]
    class Biome
    {
        protected Stair stair;
        protected Room[] rooms;

        protected Random rand = new Random();
        public int x;
        public int y;
        public int width;
        public int height;

        protected Entity[] entities = new Entity[]
        {

        };


        //<summary>
        //Créer le biome normal
        //</summary>
        public Biome()
        {
        }

        //<summary>
        //applique les paramètre correspondant aux biome
        //</summary>
        //<param name="par1stair">Etage du donjon dans lequel est situé le biome</param>
        //<param name="par2x">position x du biome dans l'étage</param>
        //<param name="par3y">position y du biome dans l'étage</param>
        //<param name="par4width">largeur du biome dans l'étage</param>
        //<param name="par5height">hauteur du biome dans l'étage</param>
        public void set(Stair par1stair, int par2x, int par3y, int par4width, int par5height)
        {
            stair = par1stair;
            x = par2x;
            y = par3y;
            width = par4width;
            height = par5height;
        }

        //<summary>
        //Créer le biome normal
        //</summary>
        //<param name="par1stair">Etage du donjon dans lequel est situé le biome</param>
        //<param name="par2x">position x du biome dans l'étage</param>
        //<param name="par3y">position y du biome dans l'étage</param>
        //<param name="par4width">largeur du biome dans l'étage</param>
        //<param name="par5height">hauteur du biome dans l'étage</param>
        public Biome(Stair par1stair,int par2x,int par3y,int par4width,int par5height)
        {
            stair = par1stair;
            x = par2x;
            y = par3y;
            width = par4width;
            height = par5height;
        }

        //<summary>
        //vérifie si le biome contient le poitn situé aux coordonnées spécifiées
        //</summary>
        //<param name="par1x">position x du point à vérifier</param>
        //<param name="par2y">position y du point à vérifier</param>
        public bool contains(int par1x, int par2y)
        {
            if (par1x >= x && par1x <= x + width && par2y >= y && par2y <= y + height)
                return true;
            return false;
        }

        //<summary>
        //affecte les salles situées à l'intérieur du Biome
        //</summary>
        //<param name="par1">Tableau contenant la liste des salles contenues dans le biome</param>
        public void setRooms(Room[] par1)
        {
            rooms = par1;
        }

        public void genCaseInZone(int par1case,Room par2room)
        {
            int tried = 0;
            int xp = rand.Next(1, par2room.width - 1);
            int yp = rand.Next(1, par2room.height - 1);
            while (par2room.map[xp, yp] != 1)
            {
                if (tried >= 500)
                    return;
                xp = rand.Next(1, par2room.width - 1);
                yp = rand.Next(1, par2room.height - 1);
                tried += 1;
            }
            tried = 0;
            par2room.set(par1case, xp, yp);
            stair.set(par1case, par2room.x + xp, par2room.y + yp);
            for (int i = 0; i < rand.Next(5, 15); i++)
            {
                xp = rand.Next(1, par2room.width - 1);
                yp = rand.Next(1, par2room.height - 1);
                while (!(par2room.map[xp - 1, yp] == par1case || par2room.map[xp + 1, yp] == par1case || par2room.map[xp, yp - 1] == par1case || par2room.map[xp, yp + 1] == par1case) || par2room.map[xp, yp] != 1)
                {
                    if (tried >= 500)
                        return;
                    xp = rand.Next(1, par2room.width - 1);
                    yp = rand.Next(1, par2room.height - 1);
                    tried += 1;
                }
                par2room.set(par1case, xp, yp);
                stair.set(par1case, par2room.x + xp, par2room.y + yp);
            }
        }

        //<summary>
        //retourne la hauteur du biome
        //</summary>
        public int getH()
        {
            return height;
        }

        //<summary>
        //retourne la largeur du biome
        //</summary>
        public int getW()
        {
            return width;
        }

        //<summary>
        //retourne la position x du biome
        //</summary>
        public int getX()
        {
            return x;
        }

        //<summary>
        //retourne la position y du biome
        //</summary>
        public int getY()
        {
            return y;
        }

        //<summary>
        //applique les spécificité du biome aux diverses salles de ce dernier
        //</summary>
        public virtual void apply()
        {
           
        }

        //<summary>
        //applique les spécificité du biome aux diverses salles de ce dernier
        //</summary>
        public virtual Entity[] applyEntities(int par1)
        {
            return null;
        }

        //<summary>
        //retourne une copie indépendante de l'objet courant
        //</summary>
        public Biome clone()
        {
            return (Biome)this.MemberwiseClone();
        }

        //<summary>
        //applique de l'eau dans la salle passée en argument
        //</summary>
        //<param name="par1room">salle à affecter</param>
        protected void applyWater(Room par1room)
        {

            int x=rand.Next(1,par1room.width-1);
            int y=rand.Next(1,par1room.height-1);
            int w = rand.Next(4, 10);
            int h = rand.Next(4, 10);
            for (int i = x; i < w; i++)
            {
                for (int u = y; u < h; u++)
                {
                    try
                    {
                        if (stair.map[i + par1room.x, u + par1room.y] != 2 && stair.map[i + par1room.x, u + par1room.y] != 0)
                        {
                            stair.set(100, i + par1room.x, u + par1room.y);
                            par1room.set(100, i, u);
                        }
                    }
                    catch (Exception)
                    { }
                }
            }
        }




    }
}
