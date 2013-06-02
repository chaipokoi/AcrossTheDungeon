﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DW
{
    [Serializable]
    class Bat : Entity
    {
        public Bat()
        {
            name = "Chauve-Souris";
            life = 50;
            lifeTmp = life;
            force = 2;
            endurance = 2;
            volonte = 0;
            agilite = 7;
            regime = "carnivore";
            value = "V";
            peur = -10;
            originalValue = value;
            turn();
        }

        public override bool update(Entity[] par1)
        {
            others = par1;
            frame += 1;
            if (isSleeping == true)
            {
                if (frame <= 20)
                    value = "Z";
                else
                    value = originalValue;
                if (frame >= 40)
                    frame = 0;
            }
            else
            {
                if (frame <= 20)
                    value = "v";
                else
                    value = originalValue;
                if (frame >= 40)
                    frame = 0;
            }
            if (lifeTmp <= 0)
                dead = true;
            return dead;
        }

 

    }
}