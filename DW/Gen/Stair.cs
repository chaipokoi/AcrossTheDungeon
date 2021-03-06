﻿using System;

using System.Drawing;


namespace DW
{
    [Serializable]
    public class Stair
    {
        private Random rand = new Random();
        private Room[] rooms;
        private Biome[] biomes;
        private Biome[] biomesList = new Biome[]
        {
            //new Biome(),
            new BiomeDungeon(),
            new BiomeGarden(),
            new BiomeCave(),
            new BiomeVillage(),
        };
        public int[,] map;
        private Special[,] special;
        private Point[] endlink;
        public int width;
        public int height;
        private int doors = 0;
        private Entity[] entities;

        public Stair()
        {
            width = rand.Next(50, 100);
            height = rand.Next(50, 100);
            map = new int[width + 1, height + 1];
            entities = new Entity[rand.Next(10, 40)];
            special = new Special[width + 1, height + 1];
            rooms = new Room[(int)(width * height / 100 + 1)];
            endlink = new Point[rooms.Length];
            genRooms();
            copyRooms();
            genLink();
            cleanMap();
            applyBiome();
            genDoors();
            brushMap();
            genEntities();
        }

        public Stair(int par1width, int par2height, int[,] par3map, Entity[] par4entities, Special[,] par5special)
        {
            width = par1width;
            height = par2height;
            map = par3map;
            entities = par4entities;
            special = par5special;
        }

        public void update()
        {
            for (int i = 0; i < entities.Length; i++)
            {
                if (entities[i] != null)
                {
                    if (entities[i].update(entities) == true)
                        entities[i] = null;
                }
            }
            for (int i = 0; i < width; i++)
            {
                for (int u = 0; u < height; u++)
                {
                    if (special[i, u] != null)
                        special[i, u] = special[i, u].update();
                }
            }
            for (int i = 0; i < biomes.Length; i++)
            {
                if (biomes[i] != null && biomes[i] is BiomeVillage)
                {
                    ((BiomeVillage)biomes[i]).update();
                }
            }
        }

        //<summary>
        //Augmente le gestionnaire de temps de toutes les entités d'une unité.
        //</summary>
        public void time()
        {
            for (int i = 0; i < entities.Length; i++)
            {
                if (entities[i] != null)
                {
                    if (!(entities[i] is Player))
                        entities[i].time();

                }
            }
        }

        //<summary>
        //Génere les salles
        //</summary>
        private void genRooms()
        {
            Console.WriteLine("Génération des salles.");
            int nb = 0;
            int t = 0;
            while (nb < rooms.Length)
            {
                if (t <= 500)
                {
                    rooms[nb] = new Room(this);
                    if (rooms[nb].check(rooms) == false)
                    {
                        rooms[nb] = null;
                        t = t + 1;
                    }
                    else
                    {
                        nb += 1;
                        t = 0;
                    }
                }
                else
                    break;
            }
        }

        //<summary>
        //Relis les salles entre elles 
        //</summary>
        private void genLink()
        {
            Console.WriteLine("Génération des couloirs.");
            int i = 0;
            int ended = 0;
            Room old = null;
            while(ended<rooms.Length)
            {
                if (rooms[i] != null)
                {
                    Point door = rooms[i].createDoor();
                            int r=-1;
                            for (int e = 0; e < rooms.Length; e++)
                            {
                                if (rooms[e] != null && !rooms[e].Equals(rooms[i]) && (old==null || !old.Equals(rooms[e])) && !rooms[e].ended)
                                {
                                    int de = Math.Abs(rooms[i].x - rooms[e].x) + Math.Abs(rooms[i].y - rooms[e].y);
                                    int dr = 9999999;
                                    if (r >= 0)
                                        dr = Math.Abs(rooms[r].x - rooms[i].x) + Math.Abs(rooms[r].y - rooms[i].y);
                                    if (r == -1 || de < dr)
                                    {
                                        r = e;
                                    }
                                }
                            }
                            old = rooms[i];
                            rooms[i].ended = true;
                            if (r <= -1)
                                    return;
                            if (!rooms[i].Equals(rooms[r]) && rooms[r] != null)
                            {
                                Point o = rooms[r].createDoor();
                                    pathfinding(new Point(door.X + rooms[i].getX(), door.Y + rooms[i].getY()), new Point(o.X + rooms[r].getX(), o.Y + rooms[r].getY()));
                                    Console.WriteLine("Room id " + i + " reliée à Room id " + r);
                                    i = r;
                                    ended += 1;
                            }
                            else
                            {
                                Console.WriteLine("Impossible de relier Room id " + i + " à Room id " + r + " : salle inexistante ou égale à l'orignale.");
                                i = rand.Next(0, rooms.Length);
                                int f = 0;
                                while (rooms[i] == null || rooms[i].ended)
                                {
                                    i = rand.Next(0, rooms.Length);
                                    f += 1;
                                    if (f < 200)
                                        return;
                                }
                            }
                        }
                        else
                            continue;
                    }
        }

        //<summary>
        //Applique la map local des salles a la map générale de l'etage.
        //</summary>
        private void copyRooms()
        {
            for (int i = 0; i < rooms.Length; i++)
            {
                if (rooms[i] != null)
                {
                    int[,] m = rooms[i].getMap();
                    for (int u = 0; u < rooms[i].getW() + 1; u++)
                    {
                        for (int o = 0; o < rooms[i].getH() + 1; o++)
                        {
                            try
                            {
                                map[rooms[i].getX() + u, rooms[i].getY() + o] = m[u, o];
                            }
                            catch (IndexOutOfRangeException)
                            { }
                        }
                    }
                }
            }
        }

        private void applyBiome()
        {
            Console.WriteLine("Génération et Application des biomes.");
            biomes = new Biome[4];
            int xb;
            int yb;
            int wb;
            int hb;
            xb = 0;
            yb = 0;
            wb = rand.Next((int)(width * 0.025), (int)(width * 0.050));
            hb = rand.Next((int)(height * 0.025), (int)(height * 0.050));
            biomes[0] = biomesList[rand.Next(0, biomesList.Length)].clone();
            biomes[0].set(this, xb, yb, wb, hb);
            xb = wb;
            yb = 0;
            wb = width - xb;
            hb = rand.Next((int)(height * 0.025), (int)(height * 0.050));
            biomes[1] = biomesList[rand.Next(0, biomesList.Length)].clone();
            biomes[1].set(this, xb, yb, wb, hb);
            xb = 0;
            yb = biomes[0].getH();
            wb = biomes[0].getW();
            hb = height - yb;
            biomes[2] = biomesList[rand.Next(0, biomesList.Length)].clone();
            biomes[2].set(this, xb, yb, wb, hb);
            xb = biomes[1].getX();
            yb = biomes[1].getH();
            wb = width - xb;
            hb = height - yb;
            biomes[3] = biomesList[rand.Next(0, biomesList.Length)].clone();
            biomes[3].set(this, xb, yb, wb, hb);
            for (int i = 0; i < biomes.Length; i++)
            {
                if (biomes[i] != null)
                {
                    Room[] r = new Room[rooms.Length];
                    for (int u = 0; u < rooms.Length; u++)
                    {
                        if (rooms[u] != null && biomes[i].contains(rooms[u].getX(), rooms[u].getY()) == true)
                        {
                            r[u] = rooms[u];
                            Console.WriteLine("Room id " + u + " dans Biome id " + i + ".");
                        }
                    }
                    biomes[i].setRooms(r);
                    if (!(biomes[i] is BiomeVillage))
                    {
                        Console.WriteLine("Application...");
                        biomes[i].apply();
                    }

                }
            }


        }

        private void genEntities()
        {
            Console.WriteLine("Génération et Placement des entités.");
            for (int i = 0; i < biomes.Length; i++)
            {
                if (biomes[i] != null)
                {
                    int r = rand.Next(0, 20);
                    Entity[] res = biomes[i].applyEntities(r);
                    if (res != null)
                    {
                        Console.WriteLine("génération de " + r + " entités");
                        for (int o = 0; o < res.Length; o++)
                        {
                            if (res[o] != null)
                            {
                                if (putEntity(res[o]) == true)
                                    Console.WriteLine("Placement de " + res[o].getName() + " à " + res[o].getX() + "/" + res[o].getY());
                            }
                        }
                    }
                }
            }
            entities[entities.Length - 1] = null;
            entities[entities.Length - 2] = null;

        }

        private void cleanMap()
        {
            Console.WriteLine("Correction des imperfections.");
            for (int i = 0; i < width; i++)
            {
                for (int u = 0; u < height; u++)
                {

                    if ((i == width - 1 || u == height - 1 || i == 0 || u == 0) && map[i, u] != 0)
                    {
                        map[i, u] = 2;
                        // Console.WriteLine("Dépassement des limites de la map.");
                    }
                    else if ((map[i, u] == 1 || map[i, u] == 3))
                    {
                        if (i - 1 >= 0 && map[i - 1, u] == 0)
                            map[i - 1, u] = 2;
                        if (i + 1 <= width && map[i + 1, u] == 0)
                            map[i + 1, u] = 2;
                        if (u - 1 >= 0 && map[i, u - 1] == 0)
                            map[i, u - 1] = 2;
                        if (u + 1 <= height && map[i, u + 1] == 0)
                            map[i, u + 1] = 2;
                        //Console.WriteLine("Ajout de murs pour border les couloirs.");
                    }
                    if (map[i, u] == 0)
                    {
                        if (u - 1 >= 0 && i + 1 <= width && map[i, u - 1] == 2 && map[i + 1, u] == 2)
                            map[i, u] = 2;
                        if (u + 1 <= height && i + 1 <= width && map[i, u + 1] == 2 && map[i + 1, u] == 2)
                            map[i, u] = 2;
                        if (u + 1 <= height && i - 1 >= 0 && map[i, u + 1] == 2 && map[i - 1, u] == 2)
                            map[i, u] = 2;
                        if (u - 1 >= 0 && i - 1 >= 0 && map[i, u - 1] == 2 && map[i - 1, u] == 2)
                            map[i, u] = 2;
                    }
                }
            }

        }

        private void brushMap()
        {
            for (int i = 0; i < width; i++)
            {
                for (int o = 0; o < height; o++)
                {
                    try
                    {

                        /*Clean Water side*/
                        if (map[i, o] != 2 && map[i, o] != 100 && i - 1 >= 0 && map[i - 1, o] == 100)
                            map[i, o] = 1;
                        if (map[i, o] != 2 && map[i, o] != 100 && i + 1 <= width && map[i + 1, o] == 100)
                            map[i, o] = 1;
                        if (map[i, o] != 2 && map[i, o] != 100 && o - 1 >= 0 && map[i, o - 1] == 100)
                            map[i, o] = 1;
                        if (map[i, o] != 2 && map[i, o] != 100 && o + 1 <= height && map[i, o + 1] == 100)
                            map[i, o] = 1;
                        if (map[i, o] != 2 && map[i, o] != 100 && i - 1 >= 0 && o + 1 <= height && map[i - 1, o + 1] == 100)
                            map[i, o] = 1;
                        if (map[i, o] != 2 && map[i, o] != 100 && i - 1 >= 0 && o - 1 >= 0 && map[i - 1, o - 1] == 100)
                            map[i, o] = 1;
                        if (map[i, o] != 2 && map[i, o] != 100 && i + 1 <= width && o + 1 <= height && map[i + 1, o + 1] == 100)
                            map[i, o] = 1;
                        if (map[i, o] != 2 && map[i, o] != 100 && i + 1 <= width && o - 1 >= 0 && map[i + 1, o - 1] == 100)
                            map[i, o] = 1;
                        /*Clean Lava Side*/
                        if (map[i, o] != 101 && map[i + 1, o] == 101)
                            map[i, o] = 1;
                        if (map[i, o] != 101 && map[i - 1, o] == 101)
                            map[i, o] = 1;
                        if (map[i, o] != 101 && map[i, o + 1] == 101)
                            map[i, o] = 1;
                        if (map[i, o] != 101 && map[i, o - 1] == 101)
                            map[i, o] = 1;
                        if (map[i, o] != 101 && map[i + 1, o + 1] == 101)
                            map[i, o] = 1;
                        if (map[i, o] != 101 && map[i + 1, o - 1] == 101)
                            map[i, o] = 1;
                        if (map[i, o] != 101 && map[i - 1, o + 1] == 101)
                            map[i, o] = 1;
                        if (map[i, o] != 101 && map[i - 1, o - 1] == 101)
                            map[i, o] = 1;
                        /*water win against lava*/
                        if (map[i, o] == 101 && map[i + 2, o] == 100)
                            map[i, o] = 1;
                        if (map[i, o] == 101 && map[i - 2, o] == 100)
                            map[i, o] = 1;
                        if (map[i, o] == 101 && map[i, o + 2] == 100)
                            map[i, o] = 1;
                        if (map[i, o] == 101 && map[i, o - 2] == 100)
                            map[i, o] = 1;
                        if (map[i, o] == 101 && map[i + 2, o + 2] == 100)
                            map[i, o] = 1;
                        if (map[i, o] == 101 && map[i + 2, o - 2] == 100)
                            map[i, o] = 1;
                        if (map[i, o] == 101 && map[i - 2, o + 2] == 100)
                            map[i, o] = 1;
                        if (map[i, o] == 101 && map[i - 2, o - 2] == 100)
                            map[i, o] = 1;
                        /*Remove useless wall*/
                        if ((map[i + 1, o] == 1 && map[i - 1, o] == 1) || (map[i, o + 1] == 1 && map[i, o - 1] == 1))
                        {
                            map[i, o] = 1;
                        }

                    }
                    catch (Exception)
                    { }
                }
            }
            /*Apply Villages*/
            for (int i = 0; i < biomes.Length; i++)
            {
                if (biomes[i] != null && biomes[i] is BiomeVillage)
                {
                    biomes[i].apply();
                }
            }
        }

        //<summary>
        //Place des portes au niveau de certains couloirs
        //</summary>
        public void genDoors()
        {
            for (int i = 0; i < rooms.Length; i++)
            {
                if(rooms[i] != null)
                {
                    for (int u = 0; u < rooms[i].doors.Length; u++)
                    {
                        if (rooms[i].doors[u] != Point.Empty)
                        {
                            if (rand.Next(0, 100) <= 70)
                            {
                                int x=rooms[i].doors[u].X + rooms[i].x;
                                int y=rooms[i].doors[u].Y + rooms[i].y;
                                try
                                {
                                    if ((map[x + 1, y] == 1 && map[x - 1, y] == 1) || (map[x, y + 1] == 1 && map[x, y - 1] == 1))
                                    {
                                        map[rooms[i].doors[u].X + rooms[i].x, rooms[i].doors[u].Y + rooms[i].y] = 1;
                                        Door d = new Door(map, rooms[i].doors[u].X + rooms[i].x, rooms[i].doors[u].Y + rooms[i].y);
                                        d.setPos(this, rooms[i].doors[u].X + rooms[i].x, rooms[i].doors[u].Y + rooms[i].y);
                                        setSpecial(d, rooms[i].doors[u].X + rooms[i].x, rooms[i].doors[u].Y + rooms[i].y);
                                        Console.WriteLine("Door spawned at " + d.x + ":" + d.y);
                                    }
                                }
                                catch (Exception)
                                { }
                            }
                        }
                    }
                }
            }
        }

        private void pathfinding(Point a, Point b)
        {
            try
            {
                endlink[doors] = new Point(a.X, a.Y);
                doors += 1;
                int t = 0;
                while (a.X != b.X || a.Y != b.Y)
                {
                    if (t >= 500)
                        break;
                    t += 1;
                    if (a.X <= width && a.X >= 0 && a.Y <= height && a.Y >= 0)
                    {

                        int poids = -1;
                        Point goodnode = new Point();
                        Point[] nodes = new Point[4];
                        if (a.X - 1 <= width && a.X - 1 >= 0 && a.Y <= height && a.Y >= 0)
                            nodes[0] = new Point(a.X - 1, a.Y);
                        else
                            nodes[0] = new Point(-1, -1);
                        if (a.X + 1 <= width && a.X + 1 >= 0 && a.Y <= height && a.Y >= 0)
                            nodes[1] = new Point(a.X + 1, a.Y);
                        else
                            nodes[1] = new Point(-1, -1);
                        if (a.X <= width && a.X >= 0 && a.Y - 1 <= height && a.Y - 1 >= 0)
                            nodes[2] = new Point(a.X, a.Y - 1);
                        else
                            nodes[2] = new Point(-1, -1);
                        if (a.X <= width && a.X >= 0 && a.Y + 1 <= height && a.Y + 1 >= 0)
                            nodes[3] = new Point(a.X, a.Y + 1);
                        else
                            nodes[3] = new Point(-1, -1);
                        for (int p = 0; p < 4; p++)
                        {
                            if (nodes[p].X != -1 && nodes[p].Y != -1)
                            {
                                Point node = nodes[p];
                                int h = Math.Abs(b.X - node.X) + Math.Abs(b.Y - node.Y);
                                if (poids == -1 || h < poids)
                                {
                                    poids = h;
                                    goodnode = node;
                                }
                            }
                        }
                        a.X = goodnode.X;
                        a.Y = goodnode.Y;
                        map[a.X, a.Y] = 1;
                        if ((a.Y == b.Y && (a.X + 1 == b.X || a.X - 1 == b.X)) || (a.X == b.X && (a.Y + 1 == b.Y || a.Y - 1 == b.Y)))
                            break;

                    }
                }
                endlink[doors] = new Point(b.X, b.Y);
                doors += 1;
            }
            catch(Exception)
            {

            }

        }

        public int getW()
        {
            return width;
        }

        public int getH()
        {
            return height;
        }

        public int[,] getMap()
        {
            return map;
        }

        public Special[,] getSpecial()
        {
            return special;
        }

        public void set(int par1case, int par2x, int par3y)
        {
            try
            {
                map[par2x, par3y] = par1case;
            }
            catch (Exception) { }
        }

        public void setSpecial(Special par1case, int par2x, int par3y)
        {
            try
            {
                special[par2x, par3y] = par1case;
            }
            catch (Exception) { }
        }

        //<summary>
        //Retourne la liste d'entités de l'etage
        //</summary>
        public Entity[] getEntities()
        {
            return entities;
        }

        //<summary>
        //Ajoute une entité a la liste d'entitiés de l'etage
        //</summary>
        public bool putEntity(Entity par1)
        {
            for (int i = 0; i < entities.Length; i++)
            {
                if (entities[i] == null)
                {
                    entities[i] = par1;
                    return true;
                }
            }
            return false;
        }

        //<summary>
        //Retourne les coordonnées d'un espace libre sour forme de Point
        //</summary>
        public Point getFreeSpecialCase()
        {
            int xp = rand.Next(1, width - 1);
            int yp = rand.Next(1, height - 1);
            while (map[xp, yp] != 1 || special[xp, yp] != null)
            {
                xp = rand.Next(1, width - 1);
                yp = rand.Next(1, height - 1);
            }
            return new Point(xp, yp);
        }

        public Point getFreeSpecialCase(Rectangle rec)
        {
            int xp = rand.Next(rec.X, rec.X + rec.Width);
            int yp = rand.Next(rec.Y, rec.Y + rec.Height);
            int i = 0;
            while (map[xp, yp] != 1 || special[xp, yp] != null)
            {
                xp = rand.Next(1, width - 1);
                yp = rand.Next(1, height - 1);
                i += 1;
                if (i > 500)
                    return new Point(-1, -1);
            }
            return new Point(xp, yp);
        }

        public void setMap(int[,] par1)
        {
            map = par1;
        }

        public void setSpecial(Special[,] par1)
        {
            special = par1;
        }

        public void removeSpecial(int par1x, int par2y)
        {
            special[par1x, par2y] = null;
        }

        //<summary>
        //Fait apparaitre un item sur le sol de la map
        //</summary>
        //<param name="par1item">L'item a faire apparaitre</param>
        //<param name="par2x">La position x du point a laquelle l'item doit apparaitre</param>
        //<param name="par3y">La position y du point a laquelle l'item doit apparaitre</param>
        public bool spawnItem(Item par1item, int par2x, int par3y)
        {

            ItemOnMap i = new ItemOnMap(par1item, this, par2x, par3y);
            if (special[par2x, par3y] == null)
            {
                i.setPos(par2x, par3y);
                special[par2x, par3y] = i;
            }
            else if (special[par2x + 1, par3y] == null && map[par2x + 1, par3y] == 1)
            {
                i.setPos(par2x + 1, par3y);
                special[par2x + 1, par3y] = i;
            }
            else if (special[par2x - 1, par3y] == null && map[par2x - 1, par3y] == 1)
            {
                i.setPos(par2x - 1, par3y);
                special[par2x - 1, par3y] = i;
            }
            else if (special[par2x, par3y - 1] == null && map[par2x, par3y - 1] == 1)
            {
                i.setPos(par2x, par3y - 1);
                special[par2x, par3y - 1] = i;
            }
            else if (special[par3y, par3y + 1] == null && map[par2x, par3y + 1] == 1)
            {
                i.setPos(par2x, par3y + 1);
                special[par3y, par3y + 1] = i;
            }
            else return false;

            return true;
        }

        //<summary>
        //Retourne l'entitée située aux coordonnées du point passé en paramètre.
        //</summary>
        public Entity getEntityAt(Point par1)
        {
            for (int i = 0; i < entities.Length; i++)
            {
                if (entities[i] != null && entities[i].getX() == par1.X && entities[i].getY() == par1.Y)
                    return entities[i];
            }
            return null;
        }


    }
}