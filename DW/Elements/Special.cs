﻿using System;


using System.Drawing;
using SdlDotNet.Graphics.Sprites;




namespace DW
{
    [Serializable]
    public class Special
    {
        public Stair stair;
        public int x;
        public int y;
        protected string value = "";
        protected string originalValue = "";
        protected Color color = Color.Gray;
        protected string face = "front";
        protected string name;


        public Special(Stair par1stair)
        {
            stair = par1stair;
        }


        public Special()
        { }


        public string getChar()
        {
            return value;
        }

        public string getValue()
        {
            return value;
        }


        public Color getColor()
        {
            return color;
        }

        public string getName()
        {
            return name;
        }


        public void setPos(Stair par3stair, int par1x, int par2y)
        {
            stair = par3stair;
            x = par1x;
            y = par2y;
        }


        public void setPos(int par1x, int par2y)
        {
            x = par1x;
            y = par2y;
        }


        public int getX()
        {
            return x;
        }


        public int getY()
        {
            return y;
        }


        public Stair getStair()
        {
            return stair;
        }


        public virtual Special clone()
        {
            return (Special)this.MemberwiseClone();
        }


        public virtual bool canPass()
        {
            return false;
        }


        public virtual void interact(Entity par1)
        {


        }


        public virtual Special update()
        {
            return this;
        }

        public void setFace(string par1)
        {
            face = par1;
        }

        public string getFace()
        {
            return face;
        }
    }
}

