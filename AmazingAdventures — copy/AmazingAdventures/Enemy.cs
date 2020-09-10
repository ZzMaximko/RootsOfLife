using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Windows.Forms;

namespace AmazingAdventures
{
    enum EnemyAttackStatus : byte
    {
        just_waiting,
        light_attack,
        strong_attack
    }
    enum ContainItem : byte
    {
        is_no_item,
        is_heart,
        is_coin
    }
    enum EnemyGetMapped : byte
    {
        is_not,
        is_ok
    }
    enum EnemyDirection : byte
    {
        is_down,
        is_up,
        is_left,
        is_right
    }
    enum EnemyStatus : byte
    {
        edle,
        run_down,
        run_up,
        run_left,
        run_right,
        light_attack_down,
        light_attack_up,
        light_attack_left,
        light_attack_right,
        strong_attack_down,
        strong_attack_up,
        strong_attack_left,
        strong_attack_right
    }
    enum EnemyHealth : byte
    {
        HP0 = 0,
        HP25,
        HP50,
        HP75,
        HP100
    }
    class Enemy
    {
        private int picSizeWidth = 63;
        private int picSizeHeight = 63;

        private bool attackHero = false; // производится ли удар по герою
        private Point impulseAttackPos; // текущая позиция импульса
        private bool impulseHimselfWay = false; // анимация движения импульса по карте
        private EnemyAttackStatus attackState; // состояние при котором противник производит или не производит удар по герою
        private int lightAttackDownStep = 0; // 5
        private int lightAttackUpStep = 6; // 9
        private int lightAttackLeftStep = 10; // 16
        private int lightAttackRightStep = 17; // 24

        private int strongAttackDownStep = 0; // 3
        private int strongAttackUpStep = 4; // 6
        private int strongAttackLeftStep = 7; // 11
        private int strongAttackRightStep = 12; // 17


        private bool damaged = false; // нанесен ли противнику урон
        private int leftSideDamagedStep = 50; // 55
        private int rightSideDamagedStep = 56; // 61

        private bool enemyDropedItem = false; // обронил ли противник предмет (после смерти)
        private int dropedHeartItemStep = 56; // 61
        private int enemyGotMapItem = 0; // противник подобрал предмет с карты, если значение равно нулю, то предмета нет
        private ContainItem enemyGotItem; // содержит ли противник предмет типа сердца для восполнения собственного запаса жизней персонажа или монеты

        private int runningDownStep = 0; // 0-3
        private int runningUpStep = 4; // 4-7
        private int runningLeftStep = 8; // 8-11
        private int runningRightStep = 12; // 12-15

        private bool enemyIncoming = false;
        private int incomingStep = 21; // 35

        private bool dyingByWater = false;
        private int dyingByWaterStep = 16; // 20

        private bool dyingBySword = false;
        private int dyingBySwordStep = 36; // 40

        private bool dyingByArrow = false;
        private int dyingByArrowStep = 41; // 49

        private int timeOutMoving = 2;

        private EnemyStatus enemyState;
        private Point enemyLocation;
        private EnemyHealth enemyHitPoints;
        private EnemyDirection eDirection;
        private EnemyDirection eDirectionImpulseBuff;
        private EnemyGetMapped unitStatusReady;

        public EnemyGetMapped UnitStatusReady
        {
            get
            {
                return unitStatusReady;
            }
        }

        public Point EnemyLocation
        {
            get
            {
                return enemyLocation;
            }
        }

        public bool EnemyIncoming
        {
            get
            {
                return enemyIncoming;
            }
            set
            {
                enemyIncoming = value;
            }
        }

        public EnemyHealth HitPoints
        {
            get
            {
                return enemyHitPoints;
            }
            set
            {
                enemyHitPoints = value;
            }
        }

        public bool DyingBySword
        {
            get
            {
                return dyingBySword;
            }
            set
            {
                dyingBySword = value;
            }
        }

        public bool DyingByArrow
        {
            get
            {
                return dyingByArrow;
            }
            set
            {
                dyingByArrow = value;
            }
        }
        public bool DyingByWater
        {
            get
            {
                return dyingByWater;
            }
            set
            {
                dyingByWater = value;
            }
        }

        public int TimeOutMoving
        {
            get
            {
                return timeOutMoving;
            }
            set
            {
                timeOutMoving = value;
            }
        }

        public ContainItem EnemyGotItem
        {
            get
            {
                return enemyGotItem;
            }
        }

        public int EnemyGotMapItem
        {
            get
            {
                return enemyGotMapItem;
            }
        }

        public bool Damaged
        {
            get
            {
                return damaged;
            }
            set
            {
                damaged = value;
            }
        }

        public bool AttackHero
        {
            get
            {
                return attackHero;
            }
            set
            {
                attackHero = value;
            }
        }


        public void SetEnemyLocation(Point enemyLocation)
        {
            this.enemyLocation = enemyLocation;
        }

        public Point EnemyRandomizePosition(int[,] map)
        {
            Random rnd = new Random(DateTime.Now.Millisecond);
            int x, y;
            do
            {
                x = rnd.Next(2, 31);
                y = rnd.Next(2, 18);
            } while (map[x, y] == 143 || map[x, y] == 144 || map[x, y] == 1 || map[x, y] == 79 || map[x, y] == 95);
            return new Point(x, y);
        }
        public EnemyDirection RandomDirection()
        {
            EnemyDirection myDirection;
            Random rand = new Random(DateTime.Now.Millisecond);
            myDirection = (EnemyDirection)rand.Next(0, 4);
            return myDirection;
        }
        private EnemyAttackStatus RandomTypeAttack()
        {
            Random rand = new Random(DateTime.Now.Millisecond);
            return (EnemyAttackStatus)rand.Next(0, 3);
        }
        public void EnemyMove(Graphics myGraph, ImageList imgGame, ImageList imgEnemy, ref int[,] map, Point heroLocation)
        {
            this.eDirection = RandomDirection();
            switch(this.eDirection)
            {
                case EnemyDirection.is_down:
                    {
                        this.enemyState = EnemyStatus.run_down;
                        if(heroLocation != new Point(this.enemyLocation.X, this.enemyLocation.Y + 1) && (map[this.enemyLocation.X, this.enemyLocation.Y + 1] < 145 || map[this.enemyLocation.X, this.enemyLocation.Y + 1] > 148) && map[this.enemyLocation.X, this.enemyLocation.Y + 1] != 143 && map[this.enemyLocation.X, this.enemyLocation.Y + 1] != 144 && map[this.enemyLocation.X, this.enemyLocation.Y + 1] != 1 && map[this.enemyLocation.X, this.enemyLocation.Y + 1] != 79 && map[this.enemyLocation.X, this.enemyLocation.Y + 1] != 95)
                        {
                            if(map[enemyLocation.X, enemyLocation.Y + 1] >= 157 && map[enemyLocation.X, enemyLocation.Y + 1] <= 163)
                            {
                                enemyGotMapItem = map[enemyLocation.X, enemyLocation.Y + 1];
                            }
                            map[this.enemyLocation.X, this.enemyLocation.Y] = 0;
                            imgGame.Draw(myGraph, this.enemyLocation.X * picSizeWidth, this.enemyLocation.Y * picSizeHeight, map[this.enemyLocation.X, this.enemyLocation.Y]);
                            this.enemyLocation = new Point(this.enemyLocation.X, this.enemyLocation.Y + 1);
                            map[this.enemyLocation.X, this.enemyLocation.Y] = 145;
                            imgGame.Draw(myGraph, this.enemyLocation.X * picSizeWidth, this.enemyLocation.Y * picSizeHeight, map[this.enemyLocation.X, this.enemyLocation.Y]);
                        } else
                        {
                            map[this.enemyLocation.X, this.enemyLocation.Y] = 145;
                            imgGame.Draw(myGraph, this.enemyLocation.X * picSizeWidth, this.enemyLocation.Y * picSizeHeight, map[this.enemyLocation.X, this.enemyLocation.Y]);
                        }
                        break;
                    }
                case EnemyDirection.is_up:
                    {
                        this.enemyState = EnemyStatus.run_up;
                        if(heroLocation != new Point(this.enemyLocation.X, this.enemyLocation.Y - 1) && (map[this.enemyLocation.X, this.enemyLocation.Y - 1] < 145 || map[this.enemyLocation.X, this.enemyLocation.Y - 1] > 148) && map[this.enemyLocation.X, this.enemyLocation.Y - 1] != 143 && map[this.enemyLocation.X, this.enemyLocation.Y - 1] != 144 && map[this.enemyLocation.X, this.enemyLocation.Y - 1] != 1 && map[this.enemyLocation.X, this.enemyLocation.Y - 1] != 79 && map[this.enemyLocation.X, this.enemyLocation.Y - 1] != 95)
                        {
                            if(map[enemyLocation.X, enemyLocation.Y - 1] >= 157 && map[enemyLocation.X, enemyLocation.Y - 1] <= 163)
                            {
                                enemyGotMapItem = map[enemyLocation.X, enemyLocation.Y - 1];
                            }
                            map[this.enemyLocation.X, this.enemyLocation.Y] = 0;
                            imgGame.Draw(myGraph, this.enemyLocation.X * picSizeWidth, this.enemyLocation.Y * picSizeHeight, map[this.enemyLocation.X, this.enemyLocation.Y]);
                            this.enemyLocation = new Point(this.enemyLocation.X, enemyLocation.Y - 1);
                            map[this.enemyLocation.X, this.enemyLocation.Y] = 146;
                            imgGame.Draw(myGraph, this.enemyLocation.X * picSizeWidth, this.enemyLocation.Y * picSizeHeight, map[this.enemyLocation.X, this.enemyLocation.Y]);
                        } else
                        {
                            map[this.enemyLocation.X, this.enemyLocation.Y] = 146;
                            imgGame.Draw(myGraph, this.enemyLocation.X * picSizeWidth, this.enemyLocation.Y * picSizeHeight, map[this.enemyLocation.X, this.enemyLocation.Y]);
                        }
                        break;
                    }
                case EnemyDirection.is_left:
                    {
                        this.enemyState = EnemyStatus.run_left;
                        if(heroLocation != new Point(this.enemyLocation.X - 1, this.enemyLocation.Y) && (map[this.enemyLocation.X - 1, this.enemyLocation.Y] < 145 || map[this.enemyLocation.X - 1, this.enemyLocation.Y] > 148) && map[this.enemyLocation.X - 1, this.enemyLocation.Y] != 143 && map[this.enemyLocation.X - 1, this.enemyLocation.Y] != 144 && map[this.enemyLocation.X - 1, this.enemyLocation.Y] != 1 && map[this.enemyLocation.X - 1, this.enemyLocation.Y] != 79 && map[this.enemyLocation.X - 1, this.enemyLocation.Y] != 95)
                        {
                            if(map[enemyLocation.X - 1, enemyLocation.Y] >= 157 && map[enemyLocation.X - 1, enemyLocation.Y] <= 163)
                            {
                                enemyGotMapItem = map[enemyLocation.X - 1, enemyLocation.Y];
                            }
                            map[this.enemyLocation.X, this.enemyLocation.Y] = 0;
                            imgGame.Draw(myGraph, this.enemyLocation.X * picSizeWidth, this.enemyLocation.Y * picSizeHeight, map[this.enemyLocation.X, this.enemyLocation.Y]);
                            this.enemyLocation = new Point(this.enemyLocation.X - 1, this.enemyLocation.Y);
                            map[this.enemyLocation.X, this.enemyLocation.Y] = 147;
                            imgGame.Draw(myGraph, this.enemyLocation.X * picSizeWidth, this.enemyLocation.Y * picSizeHeight, map[this.enemyLocation.X, this.enemyLocation.Y]);
                        } else
                        {
                            map[this.enemyLocation.X, this.enemyLocation.Y] = 147;
                            imgGame.Draw(myGraph, this.enemyLocation.X * picSizeWidth, this.enemyLocation.Y * picSizeHeight, map[this.enemyLocation.X, this.enemyLocation.Y]);
                        }
                        break;
                    }
                case EnemyDirection.is_right:
                    {
                        this.enemyState = EnemyStatus.run_right;
                        if(heroLocation != new Point(this.enemyLocation.X + 1, this.enemyLocation.Y) && (map[this.enemyLocation.X + 1, this.enemyLocation.Y] < 145 || map[this.enemyLocation.X + 1, this.enemyLocation.Y] > 148) && map[this.enemyLocation.X + 1, this.enemyLocation.Y] != 143 && map[this.enemyLocation.X + 1, this.enemyLocation.Y] != 144 && map[this.enemyLocation.X + 1, this.enemyLocation.Y] != 1 && map[this.enemyLocation.X + 1, this.enemyLocation.Y] != 79 && map[this.enemyLocation.X + 1, this.enemyLocation.Y] != 95)
                        {
                            if(map[enemyLocation.X + 1, enemyLocation.Y] >= 157 && map[enemyLocation.X + 1, enemyLocation.Y] <= 163)
                            {
                                enemyGotMapItem = map[enemyLocation.X + 1, enemyLocation.Y];
                            }
                            map[this.enemyLocation.X, this.enemyLocation.Y] = 0;
                            imgGame.Draw(myGraph, this.enemyLocation.X * picSizeWidth, this.enemyLocation.Y * picSizeHeight, map[this.enemyLocation.X, this.enemyLocation.Y]);
                            this.enemyLocation = new Point(this.enemyLocation.X + 1, this.enemyLocation.Y);
                            map[this.enemyLocation.X, this.enemyLocation.Y] = 148;
                            imgGame.Draw(myGraph, this.enemyLocation.X * picSizeWidth, this.enemyLocation.Y * picSizeHeight, map[this.enemyLocation.X, this.enemyLocation.Y]);
                        } else
                        {
                            map[this.enemyLocation.X, this.enemyLocation.Y] = 148;
                            imgGame.Draw(myGraph, this.enemyLocation.X * picSizeWidth, this.enemyLocation.Y * picSizeHeight, map[this.enemyLocation.X, this.enemyLocation.Y]);
                        }
                        break;
                    }
            }
        }
        public void EnemyRunningAnimation(Graphics myGraph, ImageList imgEnemy)
        {
            switch (this.enemyState)
            {
                case EnemyStatus.run_down:
                    {
                        if(this.runningDownStep <= 3)
                        {
                            imgEnemy.Draw(myGraph, this.enemyLocation.X * picSizeWidth, this.enemyLocation.Y * picSizeHeight, this.runningDownStep);
                            this.runningDownStep++;
                        } else
                        {
                            this.runningDownStep = 0;
                            imgEnemy.Draw(myGraph, this.enemyLocation.X * picSizeWidth, this.enemyLocation.Y * picSizeHeight, this.runningDownStep);
                            this.enemyState = EnemyStatus.edle;
                        }
                        break;
                    }
                case EnemyStatus.run_up:
                    {
                        if(this.runningUpStep <= 7)
                        {
                            imgEnemy.Draw(myGraph, this.enemyLocation.X * picSizeWidth, this.enemyLocation.Y * picSizeHeight, this.runningUpStep);
                            this.runningUpStep++;
                        } else
                        {
                            this.runningUpStep = 4;
                            imgEnemy.Draw(myGraph, this.enemyLocation.X * picSizeWidth, this.enemyLocation.Y * picSizeHeight, this.runningUpStep);
                            this.enemyState = EnemyStatus.edle;
                        }
                        break;
                    }
                case EnemyStatus.run_left:
                    {
                        if(this.runningLeftStep <= 11)
                        {
                            imgEnemy.Draw(myGraph, this.enemyLocation.X * picSizeWidth, this.enemyLocation.Y * picSizeHeight, this.runningLeftStep);
                            this.runningLeftStep++;
                        } else
                        {
                            this.runningLeftStep = 8;
                            imgEnemy.Draw(myGraph, this.enemyLocation.X * picSizeWidth, this.enemyLocation.Y * picSizeHeight, this.runningLeftStep);
                            this.enemyState = EnemyStatus.edle;
                        }
                        break;
                    }
                case EnemyStatus.run_right:
                    {
                        if(this.runningRightStep <= 15)
                        {
                            imgEnemy.Draw(myGraph, this.enemyLocation.X * picSizeWidth, this.enemyLocation.Y * picSizeHeight, this.runningRightStep);
                            this.runningRightStep++;
                        } else
                        {
                            this.runningRightStep = 12;
                            imgEnemy.Draw(myGraph, this.enemyLocation.X * picSizeWidth, this.enemyLocation.Y * picSizeHeight, this.runningRightStep);
                            this.enemyState = EnemyStatus.edle;
                        }
                        break;
                    }
            }
        }
        public void EnemyIncomingAnimation(Graphics myGraph, ImageList imgEnemy)
        {
            if (this.enemyIncoming)
            {
                if(this.incomingStep <= 35)
                {
                    imgEnemy.Draw(myGraph, this.enemyLocation.X * picSizeWidth, this.enemyLocation.Y * picSizeHeight, this.incomingStep);
                    this.incomingStep++;
                } else
                {
                    this.incomingStep = 21;
                    this.enemyIncoming = false;
                }
            }
        }
        public void ItemDroped(Graphics myGraph, ImageList imgItems, ref int[,] map)
        {
            if(this.enemyDropedItem)
            {
                if (this.enemyGotMapItem == 0)
                {
                    switch(this.enemyGotItem)
                    {
                        case ContainItem.is_heart:
                            {
                                if (this.dropedHeartItemStep <= 61)
                                {
                                    imgItems.Draw(myGraph, this.enemyLocation.X * picSizeWidth, this.enemyLocation.Y * picSizeHeight, this.dropedHeartItemStep);
                                    this.dropedHeartItemStep++;
                                }
                                else
                                {
                                    this.dropedHeartItemStep = 56;
                                    map[this.enemyLocation.X, this.enemyLocation.Y] = 164;
                                    this.enemyDropedItem = false;
                                }
                                break;
                            }
                        case ContainItem.is_coin:
                            {
                                map[this.enemyLocation.X, this.enemyLocation.Y] = 165;
                                this.enemyDropedItem = false;
                                break;
                            }
                    }
                } else
                {
                    map[this.enemyLocation.X, this.enemyLocation.Y] = this.enemyGotMapItem;
                    this.enemyDropedItem = false;
                }
            }
        }
        public void EnemyDyingByWater(Graphics myGraph, ImageList imgEnemy, ImageList imgGame, ref int[,] map)
        {
            if(this.dyingByWater)
            {
                if(this.dyingByWaterStep <= 20)
                {
                    imgEnemy.Draw(myGraph, this.enemyLocation.X * picSizeWidth, this.enemyLocation.Y * picSizeHeight, this.dyingByWaterStep);
                    this.dyingByWaterStep++;
                } else
                {
                    this.dyingByWaterStep = 16;
                    map[this.enemyLocation.X, this.enemyLocation.Y] = 79;
                    imgGame.Draw(myGraph, this.enemyLocation.X * picSizeWidth, this.enemyLocation.Y * picSizeHeight, map[this.enemyLocation.X, this.enemyLocation.Y]);
                    this.dyingByWater = false;
                    this.unitStatusReady = EnemyGetMapped.is_not;
                }
            }
        }
        public void EnemyDyingBySword(Graphics myGraph, ImageList imgEnemy, ImageList imgGame, ref int[,] map)
        {
            if(this.dyingBySword)
            {
                if(this.dyingBySwordStep <= 40)
                {
                    imgEnemy.Draw(myGraph, this.enemyLocation.X * picSizeWidth, this.enemyLocation.Y * picSizeHeight, this.dyingBySwordStep);
                    this.dyingBySwordStep++;
                } else
                {
                    this.dyingBySwordStep = 36;
                    map[this.enemyLocation.X, this.enemyLocation.Y] = 0;
                    imgGame.Draw(myGraph, this.enemyLocation.X * picSizeWidth, this.enemyLocation.Y * picSizeHeight, map[this.enemyLocation.X, this.enemyLocation.Y]);
                    this.dyingBySword = false;
                    this.enemyDropedItem = true; // выпал предмет
                    this.unitStatusReady = EnemyGetMapped.is_not;
                }
            }
        }
        public void EnemyDyingByArrow(Graphics myGraph, ImageList imgEnemy, ImageList imgGame, ref int[,] map)
        {
            if(this.dyingByArrow)
            {
                if(this.dyingByArrowStep <= 49)
                {
                    imgEnemy.Draw(myGraph, this.enemyLocation.X * picSizeWidth, this.enemyLocation.Y * picSizeHeight, this.dyingByArrowStep);
                    this.dyingByArrowStep++;
                } else
                {
                    this.dyingByArrowStep = 41;
                    map[this.enemyLocation.X, this.enemyLocation.Y] = 0;
                    imgGame.Draw(myGraph, this.enemyLocation.X * picSizeWidth, this.enemyLocation.Y * picSizeHeight, map[this.enemyLocation.X, this.enemyLocation.Y]);
                    this.dyingByArrow = false;
                    this.enemyDropedItem = true; // выпал предмет
                    this.unitStatusReady = EnemyGetMapped.is_not;
                }
            }
        }
        public void TakeDamageByHero(Graphics myGraph, ImageList imgEnemy)
        {
            if (this.damaged)
            {
                switch(eDirection)
                {
                    case EnemyDirection.is_down:
                    case EnemyDirection.is_left:
                        {
                            if(leftSideDamagedStep <= 55)
                            {
                                imgEnemy.Draw(myGraph, this.enemyLocation.X * picSizeWidth, this.enemyLocation.Y * picSizeHeight, leftSideDamagedStep);
                                leftSideDamagedStep++;
                            } else
                            {
                                leftSideDamagedStep = 50;
                                this.damaged = false;
                            }
                            break;
                        }
                    case EnemyDirection.is_up:
                    case EnemyDirection.is_right:
                        {
                            if(rightSideDamagedStep <= 61)
                            {
                                imgEnemy.Draw(myGraph, this.enemyLocation.X * picSizeWidth, this.enemyLocation.Y * picSizeHeight, rightSideDamagedStep);
                                rightSideDamagedStep++;
                            } else
                            {
                                rightSideDamagedStep = 56;
                                this.damaged = false;
                            }
                            break;
                        }
                }
            }
        }
        #region Light_Attack
        private void SearchingHeroForAttack(Point playerPosition)
        {
            if(!attackHero)
            {
                Point possiblePlayerPosition = new Point();
                EnemyStatus enemyStateBuff = EnemyStatus.edle;
                switch(eDirection)
                {
                    case EnemyDirection.is_down:
                        {
                            possiblePlayerPosition = new Point(enemyLocation.X, enemyLocation.Y + 1);
                            enemyStateBuff = EnemyStatus.light_attack_down;
                            break;
                        }
                    case EnemyDirection.is_up:
                        {
                            possiblePlayerPosition = new Point(enemyLocation.X, enemyLocation.Y - 1);
                            enemyStateBuff = EnemyStatus.light_attack_up;
                            break;
                        }
                    case EnemyDirection.is_left:
                        {
                            possiblePlayerPosition = new Point(enemyLocation.X - 1, enemyLocation.Y);
                            enemyStateBuff = EnemyStatus.light_attack_left;
                            break;
                        }
                    case EnemyDirection.is_right:
                        {
                            possiblePlayerPosition = new Point(enemyLocation.X + 1, enemyLocation.Y);
                            enemyStateBuff = EnemyStatus.light_attack_right;
                            break;
                        }
                }
                if(possiblePlayerPosition == playerPosition)
                {
                    enemyState = enemyStateBuff;
                    attackHero = true;
                }
            }
        }
        public void EnemyAttackAnimation(Graphics myGraph, ImageList imgGame, ImageList imgEnemyAttack, int[,] map, Point playerPosition, ref playerStatus playerState)
        {
            SearchingHeroForAttack(playerPosition);
            switch(enemyState)
            {
                case EnemyStatus.light_attack_down:
                    {
                        if (lightAttackDownStep <= 5)
                        {
                            imgEnemyAttack.Draw(myGraph, enemyLocation.X * picSizeWidth, enemyLocation.Y * picSizeHeight, lightAttackDownStep);
                            lightAttackDownStep++;
                        }
                        else
                        {
                            lightAttackDownStep = 0;
                            imgGame.Draw(myGraph, enemyLocation.X * picSizeWidth, enemyLocation.Y * picSizeHeight, map[enemyLocation.X, enemyLocation.Y]);
                            enemyState = EnemyStatus.edle;
                            attackHero = false;
                            playerState = playerStatus.under_light_damage;
                        }
                        break;
                    }
                case EnemyStatus.light_attack_up:
                    {
                        if (lightAttackUpStep <= 9)
                        {
                            imgEnemyAttack.Draw(myGraph, enemyLocation.X * picSizeWidth, enemyLocation.Y * picSizeHeight, lightAttackUpStep);
                            lightAttackUpStep++;
                        }
                        else
                        {
                            lightAttackUpStep = 6;
                            imgGame.Draw(myGraph, enemyLocation.X * picSizeWidth, enemyLocation.Y * picSizeHeight, map[enemyLocation.X, enemyLocation.Y]);
                            enemyState = EnemyStatus.edle;
                            attackHero = false;
                            playerState = playerStatus.under_light_damage;
                        }
                        break;
                    }
                case EnemyStatus.light_attack_left:
                    {
                        if (lightAttackLeftStep <= 16)
                        {
                            imgEnemyAttack.Draw(myGraph, enemyLocation.X * picSizeWidth, enemyLocation.Y * picSizeHeight, lightAttackLeftStep);
                            lightAttackLeftStep++;
                        }
                        else
                        {
                            lightAttackLeftStep = 10;
                            imgGame.Draw(myGraph, enemyLocation.X * picSizeWidth, enemyLocation.Y * picSizeHeight, map[enemyLocation.X, enemyLocation.Y]);
                            enemyState = EnemyStatus.edle;
                            attackHero = false;
                            playerState = playerStatus.under_light_damage;
                        }
                        break;
                    }
                case EnemyStatus.light_attack_right:
                    {
                        if(lightAttackRightStep <= 24)
                        {
                            imgEnemyAttack.Draw(myGraph, enemyLocation.X * picSizeWidth, enemyLocation.Y * picSizeHeight, lightAttackRightStep);
                            lightAttackRightStep++;
                        } else
                        {
                            lightAttackRightStep = 17;
                            imgGame.Draw(myGraph, enemyLocation.X * picSizeWidth, enemyLocation.Y * picSizeHeight, map[enemyLocation.X, enemyLocation.Y]);
                            enemyState = EnemyStatus.edle;
                            attackHero = false;
                            playerState = playerStatus.under_light_damage;
                        }
                        break;
                    }
            }

        }
        #endregion

        #region Strong_Attack
        private void SearchingHeroForStrongAttack(Point playerPosition)
        {
            if(!attackHero && !impulseHimselfWay)
            {
                switch(eDirection)
                {
                    case EnemyDirection.is_down:
                        {
                            for(int i = 2; i <= 6; i++)
                                if(playerPosition == new Point(enemyLocation.X, enemyLocation.Y + i))
                                {
                                    enemyState = EnemyStatus.strong_attack_down;
                                    attackHero = true;
                                }
                            break;
                        }
                    case EnemyDirection.is_up:
                        {
                            for(int i = 2; i <= 6; i++)
                                if(playerPosition == new Point(enemyLocation.X, enemyLocation.Y - i))
                                {
                                    enemyState = EnemyStatus.strong_attack_up;
                                    attackHero = true;
                                }
                            break;
                        }
                    case EnemyDirection.is_left:
                        {
                            for(int i = 2; i <= 6; i++)
                                if(playerPosition == new Point(enemyLocation.X - i, enemyLocation.Y))
                                {
                                    enemyState = EnemyStatus.strong_attack_left;
                                    attackHero = true;
                                }
                            break;
                        }
                    case EnemyDirection.is_right:
                        {
                            for(int i = 2; i <= 6; i++)
                                if(playerPosition == new Point(enemyLocation.X + i, enemyLocation.Y))
                                {
                                    enemyState = EnemyStatus.strong_attack_right;
                                    attackHero = true;
                                }
                            break;
                        }
                }
            }
        }
        private void EnemyAttackAnimationBeforeImpulseStart(Graphics myGraph, ImageList imgEnemyStrongAttack, ImageList imgGame, ref int[,] map)
        {
            switch (enemyState)
            {
                case EnemyStatus.strong_attack_down:
                    {
                        if (strongAttackDownStep <= 3)
                        {
                            imgEnemyStrongAttack.Draw(myGraph, enemyLocation.X * picSizeWidth, enemyLocation.Y * picSizeHeight, strongAttackDownStep);
                            strongAttackDownStep++;
                        }
                        else
                        {
                            strongAttackDownStep = 0;
                            enemyState = EnemyStatus.edle;
                            attackHero = false; // заканчивается анимация удара противником
                            if (map[enemyLocation.X, enemyLocation.Y + 1] == 0 || map[enemyLocation.X, enemyLocation.Y + 1] == 94)
                            {
                                impulseAttackPos = new Point(enemyLocation.X, enemyLocation.Y + 1);
                                map[impulseAttackPos.X, impulseAttackPos.Y] = 166;
                                imgGame.Draw(myGraph, impulseAttackPos.X * picSizeWidth, impulseAttackPos.Y * picSizeHeight, map[impulseAttackPos.X, impulseAttackPos.Y]);
                                impulseHimselfWay = true; // анимация перемещения импульса
                                eDirectionImpulseBuff = EnemyDirection.is_down; // задаем направление импульса
                            }
                        }
                        break;
                    }
                case EnemyStatus.strong_attack_up:
                    {
                        if(strongAttackUpStep <= 6)
                        {
                            imgEnemyStrongAttack.Draw(myGraph, enemyLocation.X * picSizeWidth, enemyLocation.Y * picSizeHeight, strongAttackUpStep);
                            strongAttackUpStep++;
                        } else
                        {
                            strongAttackUpStep = 4;
                            enemyState = EnemyStatus.edle;
                            attackHero = false;
                            if(map[enemyLocation.X, enemyLocation.Y - 1] == 0 || map[enemyLocation.X, enemyLocation.Y - 1] == 94)
                            {
                                impulseAttackPos = new Point(enemyLocation.X, enemyLocation.Y - 1);
                                map[impulseAttackPos.X, impulseAttackPos.Y] = 166;
                                imgGame.Draw(myGraph, impulseAttackPos.X * picSizeWidth, impulseAttackPos.Y * picSizeHeight, map[impulseAttackPos.X, impulseAttackPos.Y]);
                                impulseHimselfWay = true;
                                eDirectionImpulseBuff = EnemyDirection.is_up;
                            }
                        }
                        break;
                    }
                case EnemyStatus.strong_attack_left:
                    {
                        if(strongAttackLeftStep <= 11)
                        {
                            imgEnemyStrongAttack.Draw(myGraph, enemyLocation.X * picSizeWidth, enemyLocation.Y * picSizeHeight, strongAttackLeftStep);
                            strongAttackLeftStep++;
                        } else
                        {
                            strongAttackLeftStep = 7;
                            enemyState = EnemyStatus.edle;
                            attackHero = false;
                            if(map[enemyLocation.X - 1, enemyLocation.Y] == 0 || map[enemyLocation.X - 1, enemyLocation.Y] == 94)
                            {
                                impulseAttackPos = new Point(enemyLocation.X - 1, enemyLocation.Y);
                                map[impulseAttackPos.X, impulseAttackPos.Y] = 166;
                                imgGame.Draw(myGraph, impulseAttackPos.X * picSizeWidth, impulseAttackPos.Y * picSizeHeight, map[impulseAttackPos.X, impulseAttackPos.Y]);
                                impulseHimselfWay = true;
                                eDirectionImpulseBuff = EnemyDirection.is_left;
                            }
                        }
                        break;
                    }
                case EnemyStatus.strong_attack_right:
                    {
                        if(strongAttackRightStep <= 17)
                        {
                            imgEnemyStrongAttack.Draw(myGraph, enemyLocation.X * picSizeWidth, enemyLocation.Y * picSizeHeight, strongAttackRightStep);
                            strongAttackRightStep++;
                        } else
                        {
                            strongAttackRightStep = 12;
                            enemyState = EnemyStatus.edle;
                            attackHero = false;
                            if(map[enemyLocation.X + 1, enemyLocation.Y] == 0 || map[enemyLocation.X + 1, enemyLocation.Y] == 94)
                            {
                                impulseAttackPos = new Point(enemyLocation.X + 1, enemyLocation.Y);
                                map[impulseAttackPos.X, impulseAttackPos.Y] = 166;
                                imgGame.Draw(myGraph, impulseAttackPos.X * picSizeWidth, impulseAttackPos.Y * picSizeHeight, map[impulseAttackPos.X, impulseAttackPos.Y]);
                                impulseHimselfWay = true;
                                eDirectionImpulseBuff = EnemyDirection.is_right;
                            }
                        }
                        break;
                    }
            }
        }
        private void EnemyStrongDamageHero(Point playerPosition, ref playerStatus pState)
        {
            Point possiblePlayerPos = new Point();
            switch(eDirectionImpulseBuff)
            {
                case EnemyDirection.is_down:
                    {
                        possiblePlayerPos = new Point(impulseAttackPos.X, impulseAttackPos.Y + 1);
                        break;
                    }
                case EnemyDirection.is_up:
                    {
                        possiblePlayerPos = new Point(impulseAttackPos.X, impulseAttackPos.Y - 1);
                        break;
                    }
                case EnemyDirection.is_left:
                    {
                        possiblePlayerPos = new Point(impulseAttackPos.X - 1, impulseAttackPos.Y);
                        break;
                    }
                case EnemyDirection.is_right:
                    {
                        possiblePlayerPos = new Point(impulseAttackPos.X + 1, impulseAttackPos.Y);
                        break;
                    }
            }
            if (possiblePlayerPos == playerPosition)
                pState = playerStatus.under_strong_damage;
        }
        private void EnemyImpulseMoving(Graphics myGraph, ImageList imgGame, ref int[,] map, ref playerStatus playerState, Point playerPosition)
        {
            if(impulseHimselfWay)
            {
                switch(eDirectionImpulseBuff)
                {
                    case EnemyDirection.is_down:
                        {
                            if(map[impulseAttackPos.X, impulseAttackPos.Y + 1] == 0 || map[impulseAttackPos.X, impulseAttackPos.Y + 1] == 94)
                            {
                                map[impulseAttackPos.X, impulseAttackPos.Y] = 0;
                                imgGame.Draw(myGraph, impulseAttackPos.X * picSizeWidth, impulseAttackPos.Y * picSizeHeight, map[impulseAttackPos.X, impulseAttackPos.Y]);
                                impulseAttackPos = new Point(impulseAttackPos.X, impulseAttackPos.Y + 1);
                                map[impulseAttackPos.X, impulseAttackPos.Y] = 166;
                                imgGame.Draw(myGraph, impulseAttackPos.X * picSizeWidth, impulseAttackPos.Y * picSizeHeight, map[impulseAttackPos.X, impulseAttackPos.Y]);
                            } else
                            {
                                impulseHimselfWay = false; // завершаем анимацию движения импульса, т.к. он наткнулся на объект отличный от травы (0)
                                map[impulseAttackPos.X, impulseAttackPos.Y] = 0;
                                imgGame.Draw(myGraph, impulseAttackPos.X * picSizeWidth, impulseAttackPos.Y * picSizeHeight, map[impulseAttackPos.X, impulseAttackPos.Y]);
                                EnemyStrongDamageHero(playerPosition, ref playerState); // метод проверяет наносит ли импульс урон по герою
                            }
                            break;
                        }
                    case EnemyDirection.is_up:
                        {
                            if(map[impulseAttackPos.X, impulseAttackPos.Y - 1] == 0 || map[impulseAttackPos.X, impulseAttackPos.Y - 1] == 94)
                            {
                                map[impulseAttackPos.X, impulseAttackPos.Y] = 0;
                                imgGame.Draw(myGraph, impulseAttackPos.X * picSizeWidth, impulseAttackPos.Y * picSizeHeight, map[impulseAttackPos.X, impulseAttackPos.Y]);
                                impulseAttackPos = new Point(impulseAttackPos.X, impulseAttackPos.Y - 1);
                                map[impulseAttackPos.X, impulseAttackPos.Y] = 166;
                                imgGame.Draw(myGraph, impulseAttackPos.X * picSizeWidth, impulseAttackPos.Y * picSizeHeight, map[impulseAttackPos.X, impulseAttackPos.Y]);
                            } else
                            {
                                impulseHimselfWay = false;
                                map[impulseAttackPos.X, impulseAttackPos.Y] = 0;
                                imgGame.Draw(myGraph, impulseAttackPos.X * picSizeWidth, impulseAttackPos.Y * picSizeHeight, map[impulseAttackPos.X, impulseAttackPos.Y]);
                                EnemyStrongDamageHero(playerPosition, ref playerState);
                            }
                            break;
                        }
                    case EnemyDirection.is_left:
                        {
                            if (map[impulseAttackPos.X - 1, impulseAttackPos.Y] == 0 || map[impulseAttackPos.X - 1, impulseAttackPos.Y] == 94)
                            {
                                map[impulseAttackPos.X, impulseAttackPos.Y] = 0;
                                imgGame.Draw(myGraph, impulseAttackPos.X * picSizeWidth, impulseAttackPos.Y * picSizeHeight, map[impulseAttackPos.X, impulseAttackPos.Y]);
                                impulseAttackPos = new Point(impulseAttackPos.X - 1, impulseAttackPos.Y);
                                map[impulseAttackPos.X, impulseAttackPos.Y] = 166;
                                imgGame.Draw(myGraph, impulseAttackPos.X * picSizeWidth, impulseAttackPos.Y * picSizeHeight, map[impulseAttackPos.X, impulseAttackPos.Y]);
                            } else
                            {
                                impulseHimselfWay = false;
                                map[impulseAttackPos.X, impulseAttackPos.Y] = 0;
                                imgGame.Draw(myGraph, impulseAttackPos.X * picSizeWidth, impulseAttackPos.Y * picSizeHeight, map[impulseAttackPos.X, impulseAttackPos.Y]);
                                EnemyStrongDamageHero(playerPosition, ref playerState);
                            }
                            break;
                        }
                    case EnemyDirection.is_right:
                        {
                            if (map[impulseAttackPos.X + 1, impulseAttackPos.Y] == 0 || map[impulseAttackPos.X + 1, impulseAttackPos.Y] == 94)
                            {
                                map[impulseAttackPos.X, impulseAttackPos.Y] = 0;
                                imgGame.Draw(myGraph, impulseAttackPos.X * picSizeWidth, impulseAttackPos.Y * picSizeHeight, map[impulseAttackPos.X, impulseAttackPos.Y]);
                                impulseAttackPos = new Point(impulseAttackPos.X + 1, impulseAttackPos.Y);
                                map[impulseAttackPos.X, impulseAttackPos.Y] = 166;
                                imgGame.Draw(myGraph, impulseAttackPos.X * picSizeWidth, impulseAttackPos.Y * picSizeHeight, map[impulseAttackPos.X, impulseAttackPos.Y]);
                            } else
                            {
                                impulseHimselfWay = false;
                                map[impulseAttackPos.X, impulseAttackPos.Y] = 0;
                                imgGame.Draw(myGraph, impulseAttackPos.X * picSizeWidth, impulseAttackPos.Y * picSizeHeight, map[impulseAttackPos.X, impulseAttackPos.Y]);
                                EnemyStrongDamageHero(playerPosition, ref playerState);
                            }
                            break;
                        }
                }
            }
        }
        public void EnemyStrongAttackAnimation(Point playerPosition, Graphics myGraph, ImageList imgEnemyStrongAttack, ImageList imgGame, ref playerStatus playerState, ref int[,] map)
        {
            EnemyImpulseMoving(myGraph, imgGame, ref map, ref playerState, playerPosition); // находится перед другими методами, чтобы работала отрисовка заряда рядом с врагом перед его запуском в вдижение
            SearchingHeroForStrongAttack(playerPosition);
            EnemyAttackAnimationBeforeImpulseStart(myGraph, imgEnemyStrongAttack, imgGame, ref map);
            //EnemyImpulseMoving(myGraph, imgGame, ref map, ref playerState, playerPosition);
        }
        #endregion

        public void EnemyInitialize(EnemyHealth enemyHitPoints, Point enemyLocation)
        {
            Random rand = new Random(DateTime.Now.Millisecond);
            this.enemyHitPoints = enemyHitPoints;
            this.enemyLocation = enemyLocation;
            this.unitStatusReady = EnemyGetMapped.is_ok;
            this.enemyGotItem = (ContainItem)rand.Next(0, 3);
        }

        public Enemy()
        {
            
        }
    }
}
