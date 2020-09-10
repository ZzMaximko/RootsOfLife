using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Windows.Forms;

namespace AmazingAdventures
{
    enum charClass : byte
    {
        warrior, wizard, engineer, archer
    }
    enum playerStatus : byte
    {
        edle,
        down,
        left,
        right,
        up,
        edle_motion,
        attack_down,
        attack_up,
        attack_left,
        attack_right,
        get_water_down,
        get_water_up,
        get_water_left,
        get_water_right,
        bow_attack_down,
        bow_attack_up,
        bow_attack_left,
        bow_attack_right,
        under_light_damage,
        under_strong_damage
    }

    class Hero
    {
        #region Поля
        private int picSizeWidth = 63, picSizeHeight = 63; // размер картинки основного массива
        private string playerName;
        private Point playerPos;
        private charClass playerType;
        private playerStatus pState;
        private int playerExperience;
        private int foodPoints;
        private int lightDamageStep = 0;
        private int strongDamageStep = 14; // 29
        private int strongDamageStepContinue = 30; // 31
        #endregion

        #region Свойства
        public string PlayerName
        {
            get
            {
                return playerName;
            }
            set
            {
                playerName = value;
            }
        }

        public Point PlayerPos
        {
            get
            {
                return playerPos;
            }
            set
            {
                playerPos = value;
            }
        }

        public charClass PlayerType
        {
            get
            {
                return playerType;
            }
            set
            {
                playerType = value;
            }
        }
        public playerStatus PState
        {
            get
            {
                return pState;
            }
            set
            {
                pState = value;
            }
        }
        public int PlayerExperience
        {
            get
            {
                return playerExperience;
            }
            set
            {
                playerExperience = value;
            }
        }
        public int FoodPoints
        {
            get
            {
                return foodPoints;
            }
            set
            {
                foodPoints = value;
            }
        }
        public int PlayerLevel { get; set; }
        public int PlayerHealth { get; set; }
        public int EnergyPoints { get; set; }
        public int WoodCount { get; set; } // древесина
        public int StoneCount { get; set; } // камни
        public int CoinCount { get; set; } // монеты
        public int EmptyBottles { get; set; } // пустые бутылки 
        public int WaterBottles { get; set; } // бутылки с водой
        public bool ItemSword { get; set; } // меч
        public bool ItemBow { get; set; } // лук
        public int ShovelCount { get; set; } // лопата
        public bool ShieldActivated { get; set; } // активация щита
        public int KilledEnemies { get; set; } // количество убитых противников
        #endregion

        #region methods
        private void FreezePlayerActions(ref bool T_down, ref bool T_up, ref bool T_left, ref bool T_right, ref bool T_freeze_attack)
        {
            T_down = T_up = T_left = T_right = false;
            T_freeze_attack = true;
        }
        private void UnfreezePlayerActions(ref bool T_down, ref bool T_up, ref bool T_left, ref bool T_right, ref bool T_freeze_attack)
        {
            T_down = T_up = T_left = T_right = true;
            T_freeze_attack = false;
        }
        public void HeroLightDamageAnimation(Graphics myGraph, ImageList imgHeroAttacked, ref bool T_down, ref bool T_up, ref bool T_left, ref bool T_right, ref bool T_freeze_attack)
        {
            if(pState == playerStatus.under_light_damage)
            {
                FreezePlayerActions(ref T_down, ref T_up, ref T_left, ref T_right, ref T_freeze_attack);
                if(PlayerHealth > 1)
                {
                    if(lightDamageStep < 9)
                    {
                        imgHeroAttacked.Draw(myGraph, playerPos.X * picSizeWidth, playerPos.Y * picSizeHeight, lightDamageStep);
                        lightDamageStep++;
                    } else
                    {
                        lightDamageStep = 0;
                        imgHeroAttacked.Draw(myGraph, playerPos.X * picSizeWidth, playerPos.Y * picSizeHeight, 13);
                        pState = playerStatus.edle;
                        PlayerHealth--;
                        UnfreezePlayerActions(ref T_down, ref T_up, ref T_left, ref T_right, ref T_freeze_attack);
                    }
                } else
                {
                    if (PlayerHealth - 1 <= 0)
                    {
                        if (lightDamageStep <= 12)
                        {
                            imgHeroAttacked.Draw(myGraph, playerPos.X * picSizeWidth, playerPos.Y * picSizeHeight, lightDamageStep);
                            lightDamageStep++;
                        }
                        else
                        {
                            lightDamageStep = 0;
                            pState = playerStatus.edle;
                            PlayerHealth = 0;
                            FreezePlayerActions(ref T_down, ref T_up, ref T_left, ref T_right, ref T_freeze_attack);
                        }
                    }
                }
            }
        }
        public void HeroStrongDamageAnimation(Graphics myGraph, ImageList imgHeroAttacked, ref bool T_down, ref bool T_up, ref bool T_left, ref bool T_right, ref bool T_freeze_attack)
        {
            if(pState == playerStatus.under_strong_damage)
            {
                FreezePlayerActions(ref T_down, ref T_up, ref T_left, ref T_right, ref T_freeze_attack);
                if (PlayerHealth > 2)
                {
                    if(strongDamageStep <= 29)
                    {
                        imgHeroAttacked.Draw(myGraph, playerPos.X * picSizeWidth, playerPos.Y * picSizeHeight, strongDamageStep);
                        strongDamageStep++;
                    } else
                    {
                        strongDamageStep = 14;
                        pState = playerStatus.edle;
                        PlayerHealth -= 2;
                        UnfreezePlayerActions(ref T_down, ref T_up, ref T_left, ref T_right, ref T_freeze_attack);
                    }
                } else
                {
                    if (PlayerHealth - 2 <= 0)
                    {
                        if (strongDamageStep <= 25)
                        {
                            imgHeroAttacked.Draw(myGraph, playerPos.X * picSizeWidth, playerPos.Y * picSizeHeight, strongDamageStep);
                            strongDamageStep++;
                        }
                        else
                        {
                            if (strongDamageStepContinue <= 31)
                            {
                                imgHeroAttacked.Draw(myGraph, playerPos.X * picSizeWidth, playerPos.Y * picSizeHeight, strongDamageStepContinue);
                                strongDamageStepContinue++;
                            }
                            else
                            {
                                strongDamageStepContinue = 30;
                                pState = playerStatus.edle;
                                PlayerHealth = 0;
                                strongDamageStep = 14;
                                FreezePlayerActions(ref T_down, ref T_up, ref T_left, ref T_right, ref T_freeze_attack);
                            }
                        }
                    }
                }
            }
        }
        #endregion
    }
}
