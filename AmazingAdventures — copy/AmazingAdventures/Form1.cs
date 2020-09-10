using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Media;
using System.Windows.Media;
using System.Reflection;
using System.IO;

namespace AmazingAdventures
{
    public partial class Form1 : Form
    {
        internal int picSizeWidth = 63, picSizeHeight = 63; // размер картинки основного массива
        //int picValuesSizeWidth = 20, picValuesSizeHeight = 20; // размер картинки массива символов под объекты на карте
        int arraySize_N = 31;
        int arraySize_M = 18;
        int room_arraySize_N = 31, room_arraySize_M = 18;
        int room_x = 13, room_y = 7; // room 3x3
        bool nearRoom = false;
        internal int[,] map; //основная карта

        int totalCountEnemiesOnMap = 0; // количество врагов всего в игре
        int countEnemies = 0; // количество врагов на карте в данный момент
        Enemy[] enemyCollection; // коллекция врагов в игре
        int countdownEnemy = 100; // обратный отсчет времени до появления нового врага, 100 шагов таймера или 20 секунд
        bool launchCountdownTimer = true; // запуск обратного отсчета
        bool enemyMapping = false; // отрисовка нового врага на карте
        bool allEnemiesDied = true; // все ли враги ликвидированы
        //int timeOutMoving = 2; // таймаут после появления
        int currentlyEnemiesDied = 0; // убито врагов на данный момент

        int[,] room_map; // карта домика
        bool room = false;
        Point playerRoomPos;
        int roomUpStep = 9;
        int roomDownStep = 7;
        int roomLeftStep = 11;
        int roomRightStep = 13;

        int[,] river_arr; //массив реки
        int treesCount = 0; //счетчик деревьев
        Graphics graph; // поверхность отрисовки
        int edle_step = 10, down_step = 6, up_step = 12, left_step = 16, right_step = 20; //счетчики анимации движения персонажа
        int attackStep_down = 24, attackStep_up = 28, attackStep_left = 32, attackStep_right = 36; //счетчики пробега анимации атаки персонажа
        int stumping_step = 65; //счетчик пробега анимации превращения дерева в пень
        int count = 0; // счетчик для анимации в покое
        int treeCountdownDamaged = 0; //счетчик шагов таймера отображения индикатора здоровья дерева
        bool treesStump = false; //общее для всех деревьев состояние, произошла ли вырубка какого-либо дерева
        Point treeStumpState; //дерево получившее статус пень на текущий момент
        bool treesDamaged = false; //общее для всех деревьев состояние, произошел ли удар по дереву вообще
        bool arrow_up_t, arrow_down_t, arrow_left_t, arrow_right_t, freeze_actions; //триггеры для отключения кнопок управления персонажем на время анимации
        #region Фоновая музыка в игре
        SoundPlayer mainMenuThemeSound = new System.Media.SoundPlayer();
        SoundPlayer gameThemeSound = new System.Media.SoundPlayer();
        SoundPlayer apartmentSound = new System.Media.SoundPlayer();
        #endregion

        // переменные для работы с элементом "камень"
        Rock[] myRocks; // array of stone
        int rockCountdownDamaged = 0; //счетчик шагов таймера отображения индикатора здоровья камня
        bool rockDestroyingStatus = false; //общее для всех камней состояние, произошло ли разрушение какого-либо камня
        Point rockDestroyingStatePos; //камень получивший статус разрушен на текущий момент получаем его расположение
        bool rockDamaged = false; //общее для всех камней состояние, произошел ли удар по камню вообще
        int rockDestroyingStep = 19; // счетчик анимации разрушения камня
        Stone[] myStones;
        int stone_step = 8; // 18
        int stonePlayerBuff = 0; // буфер камней при подбирании предмета
        //-------------------------------------------

        MediaPlayer soundAttackSwordEnemy = new MediaPlayer(); // звук удара мечем по врагу
        MediaPlayer soundThrowStone = new MediaPlayer(); // звук броска камня
        MediaPlayer soundDestroyStone = new MediaPlayer(); // звук разрушения камня
        MediaPlayer soundPickUpCoin = new MediaPlayer(); // звук поднятия монеты и других предметов на карте, крме основных

        // звуки кнопок меню
        MediaPlayer btnClickSound = new MediaPlayer();
        //------------------
        MediaPlayer soundPickupItem = new MediaPlayer(); // звук при поднимании предмета игроком
        MediaPlayer soundSwordAttack_air = new MediaPlayer(); // звук удара мечем в воздухе
        MediaPlayer soundSwordAttack_tree = new MediaPlayer(); // звук удара мечем по дереву
        Hero player = new Hero();
        Tree[] myTrees;

        Wood[] myWoods;
        int countWoodElements = 1000; // возможное количество элементов древесины на карте
        int woodArrayStep = 0; // счетчик элементов массива с древесиной
        int wood_one_step = 81, wood_two_step = 89, wood_more_step = 93; // счетчики анимации древесины расположенной на карте
        bool woodFirstDroped = false;
        bool woodWasFound = false;
        int woodPlayerBuff = 0; // временный буфер для хранения текущего количества подобранного дерева

        // элементы стартового меню
        Image startMenuForm = Properties.Resources.start_menu;
        bool status_start_menu = true;
        //------------------------

        // элементы главного меню
        Image newGameBtn;
        Point newGameBtnPos;
        Image loadGameBtn;
        Point loadGameBtnPos;
        Image authorGameBtn;
        Point authorGameBtnPos;
        Image exitGameBtn;
        Point exitGameBtnPos;
        bool mainMenuActivated = false;
        bool anyBtnsClicked = false;
        //------------------------

        // элементы меню новой игры
        Image newGameMenu = Properties.Resources.new_game_menu;
        Image startBtn;
        Point startBtnPos;
        Image backBtn;
        Point backBtnPos;
        TextBox textBoxProfileName;
        bool newGameMenuActivated = false;
        bool anyBtnsClicked_newGameMenu = false;
        Image attantion_msg = Properties.Resources.attantion_new_game_name;
        //------------------------

        // элементы меню об авторе
        Image authorGameMenu = Properties.Resources.author_menu;
        Point authorGameMenuPos;
        Image authorBackBtn;
        Point authorBackBtnPos;
        bool anyBtnsClicked_author_menu = false;
        bool authorGameMenuActivated = false;
        //------------------------

        // триггеры для отработки всплывающих окон сообщений
        Image imgBackgroundMsg = Properties.Resources.message_canvas; // фон для сообщений
        Point imgBackgroundMsgPos; // расположение на форме
        bool msgGameControls = false;
        int arrow_up_step = 96, arrow_down_step = 102, arrow_left_step = 108, arrow_right_step = 114; // счетчики для отрисовки подсказок управления стрелками
        //--------------------------------------------------

        // элементы игрового процесса
        bool GameplayActivated = false;
        Point FoodPointsPos;
        Size FoodPointsFormSize = new Size(244, 42);
        int gameFoodPoints = 30;
        Stack<int> valScore = new Stack<int>();
        int useFoodPointStep = 0;
        //---------------------------
        Point EnergyPointsPos;
        Size EnergyPointsFormSize = new Size(564, 42);
        int gameEnergyPoints = 50;
        //---------------------------
        Point LevelPointsPos;
        Size LevelPointsFormSize = new Size(894, 42);
        int gameLevelPoints = 75;
        int levelStep = 1;
        //---------------------------
        // логические переменные для индикации значений еды и энерегии на персонаже
        bool foodHeroActive = false;
        bool energyHeroActive = false;
        //---------------------------

        // активаци анимации, когда герой умирает от голода
        bool playerDyingWithoutFood = false;
        int DyingNoFoodStep = 9; // 12
        //-------------------------------------------------

        // отрисовка монет на интерфейсе игрока
        Image imageCoinGUI = Properties.Resources.coin_gui_form;
        Point imageCoinGUIPos = new Point(0, 42);
        //-------------------------------------

        // элементы отрисовки сообщения
        MediaPlayer msgSound = new MediaPlayer();
        Image msgCollectBtn;
        Point msgCollectBtnPos;
        Image msgBackBtn;
        Point msgBackBtnPos;
        bool messageActivated = false;
        bool anyBtnsClicked_message = false;
        string typeMessageTrigger = ""; // триггер для распознавания на какой предмет наступил герой, чтобы его собрать в инвентарь "wood", "stone" и тд.
        //-----------------------------

        // элементы отрисовки инвентаря
        MediaPlayer inventorySound = new MediaPlayer(); // звук вызова инвентаря
        Image imgBackgroundInventory = Properties.Resources.menu_inventory; // фоновое меню инвентаря
        Point imgBGInventoryPos; // положение инвенторя на форме
        Image inventoryBackBtn; // кнопка "назад" инвентаря
        Point inventoryBackBtnPos; // положение кнопки в инвентаре
        Image inventoryBuildHouseBtn; // кнопка "построить дом"
        Point inventoryBuildHouseBtnPos; //  положение кнопки постройки дома в инвентаре
        Image inventoryCoinsCount; // изображение формы для монет
        Point inventoryCoinsCountPos; // положение формы для монет в инвентаре
        bool inventoryActivated = false; // вызван ли инвентарь
        bool anyBtnsClicked_inventory = false; // нажата ли кнопка в инвентаре
        Inventory myBag = new Inventory();
        int swordFocusingStep = 7;
        int bowFocusingStep = 11;
        int woodFocusingStep = 15;
        int stoneFocusingStep = 19;
        int emptyBottleFocusingStep = 23;
        int waterBottleFocusingStep = 27;
        int shovelFocusingStep = 32;
        Image imgInventoryText = Properties.Resources.inventory_background_text; // фон для текста
        Point imgInventoryTextPos; // положение фона для отрисовки и обновления текста в форме инвентаря
        //-----------------------------

        // переменные для объекта бутылки
        Bottle[] myBottles;
        int bottle_empty_step = 128;
        int bottlePlayerBuff = 0; // буфер текущего количества подобранных бутылок
        //-------------------------------

        // сообщение для взаимодействия с предметами
        //Image interactionBackgroundIMG = Properties.Resources.interaction_background;
        //Point interactionBackgroundIMGPos = new Point(960, 0);
        Point msgInteractionPos = new Point(1152, 15);
        bool nearRiver = false; // персонаж находится рядом с рекой
        playerStatus pStatusBuff; // временная переменная для хранения состояния персонажа
        int getWaterDownStep = 0;
        int getWaterUpStep = 6;
        int getWaterLeftStep = 12;
        int getWaterRightStep = 18;
        int get_water_count = 0;
        // использование бутылк  с водой для полива пенька
        bool nearTree = false; // персонаж находится лицом к дереву
        Point possibleStumpPositionForWatering = Point.Empty; // возможная позиция пенька для испльзования бутылки с водой
        bool treeGrowingActivated = false; // триггер анимации выращивания дерева
        bool treeStumpWateringActivated = false; // триггер анимации полива пенька
        int wateringStumpStep = 0; // 5
        int growingTreeStep = 49; // 65
        WaterStatus stumpWaterStatusBuff;
        Point growingTreePosition;

        // элементы интерфейса для выбора инструмента в руки персонажу
        InterfaceItems myInterface = new InterfaceItems();
        Point myInterfacePos = new Point(920, 0);
        int interfaceSwordFocusStep = 3;
        int interfaceBowFocusStep = 7;
        int interfaceShovelFocusStep = 11;
        //------------------------------------------------------------

        // анимация использованя лопаты
        bool shovelUseActivated = false;
        int shovelUseStepDown = 0; // 2
        int shovelUseStepUp = 3; // 5
        int shovelUseStepLeft = 6; // 8
        int shovelUseStepRight = 9; // 11
        Point locationStumpBuffer; // временный буфер хранения расположения пенька при использовании лопаты
        //-----------------------------

        MediaPlayer soundBuilding = new MediaPlayer();

        int timeMinutesBuffer; // переменная для хранения времени, после которого появляется предмет на карте 
        bool building_mode = false; // режим постройки

        // элементы отрисовки объекта "камень" при атаке
        Point stoneAttackPos;
        stoneAttackStatus stoneAttackState;
        bool stoneAttackDestroyed = false;
        int stone_attack_destroyed_step = 19; // 22
        bool stoneIsFLying = false;
        //----------------------------------------------

        #region Элементы отрисовки объекта "стрела" при атаке
        Point arrowPositionBuff;
        Point arrowAttackPos;
        bool arrowAttackDestroyed = false;
        bool arrowIsFlying = false;
        int arrow_attack_destroying_step = 9; // 12
        HeroDirection pDirectionArrowBuff; // буфер напрвления героя для летящей стрелы
        //счетчики шага анимации движения героя при запуске стрелы
        int bowAttackDownStep = 0; // 2
        int bowAttackUpStep = 0; // 2
        int bowAttackLeftStep = 3; // 5
        int bowAttackRightStep = 6; // 8
        #endregion
        MediaPlayer soundBowPull = new MediaPlayer();
        MediaPlayer soundBowSwishing = new MediaPlayer();

        Point swordAttackEnemyPositionBuff; // буфер для хранения позиции врага при ударе, в каком он положении относительно героя

        MapItem ItemsGenerator = new MapItem(); // создаем генератор предметов на карте
        int countdownItemSpawnMap = 200; // счетчик появления предмета на карте
        bool bonusEnergyActivated = false; // bonus energy
        int bonusMaxEnergyCount = 50; // максимальное количество бонусных очков энергии
        int bonusCornTimeOut = 360; // 6 шагов таймера = 1 секунде, 360 = 1 минута (увеличение энергии на 50)
        int energyRestoreCounter = 3; // шаг восполнения энергии

        bool bonusSpeedActivated = false; // bonus freeze enemies actions
        int countSpeedEnemyMoving = 2; // максимальная скорость противника
        int bonusIncreaseSpeedEnemy = 4; // величина "уменьшения" скорости противника
        int bonusPepperTimeOut = 180; // 180 = 30 секунд (замедление вдвое скорости перемещения врагов)

        bool bonusPowerActivated = false; // bonus attack power
        int bonusTomatoTimeOut = 120; // 120 = 20 секунд (бонус меч - откидывает врага на клетку, как и другое оружие)
        //HeroDirection bonusPower_AttackType;


        // gameover элементы
        Image imgBackgroundGameOver = Properties.Resources.menu_background_gameover;
        Point imgBackgroundGameOverPos;
        Image imgMainMenuBtn;
        Point imgMainMenuBtnPos;
        bool anyBtnsClicked_GameOverMenu = false;
        bool gameoverMenuActivated = false;
        //-------------------

        // win game элементы
        Image imgBackgroundWinner = Properties.Resources.menu_background_winner;
        Point imgBackgroundWinnerPos;
        bool anyBtnsClicked_WinnerMenu = false;
        bool winnerMenuActivated = false;
        //------------------

        // pause menu
        Image imgBackgroundPauseMenu = Properties.Resources.menu_background_pause;
        Point imgBackgroundPauseMenuPos;
        bool anyBtnsClicked_PauseMenu = false;
        bool pauseMenuActivated = false;
        //------------------

        int timeOutEndGame = 50; // счетчик времени после законченной игры
        playerStatus playerStateForEnemyAttackBuff;

        public enum stoneAttackStatus : byte
        {
            is_good,
            is_bad
        }

        public enum HeroDirection : byte
        {
            is_down,
            is_up,
            is_left,
            is_right
        }

        HeroDirection playerDirection; // направление героя
        HeroDirection pDirectionBuff; // временная переменная для хранения направления героя

        public enum Symbols : byte
        {
            zero = 0,
            one = 1,
            two = 2,
            three = 3,
            four = 4,
            five = 5,
            six = 6,
            seven = 7,
            eight = 8,
            nine = 9,
            Upper_A = 10, Upper_B = 11, Upper_C = 12, Upper_D = 13, Upper_E = 14, Upper_F = 15, Upper_G = 16, Upper_H = 17, Upper_I = 18, Upper_J = 19,
            Upper_K = 20, Upper_L = 21, Upper_M = 22, Upper_N = 23, Upper_O = 24, Upper_P = 25, Upper_Q = 26, Upper_R = 27, Upper_S = 28, Upper_T = 29,
            Upper_U = 30, Upper_V = 31, Upper_W = 32, Upper_X = 33, Upper_Y = 34, Upper_Z = 35,
            Lower_A = 36, Lower_B, Lower_C, Lower_D, Lower_E, Lower_F, Lower_G, Lower_H, Lower_I, Lower_J,
            Lower_K, Lower_L, Lower_M, Lower_N, Lower_O, Lower_P, Lower_Q, Lower_R, Lower_S, Lower_T,
            Lower_U, Lower_V, Lower_W, Lower_X, Lower_Y, Lower_Z,
            symbol_attention, symbol_colon, symbol_continue, symbol_hyphen, symbol_inverted_commas, symbol_point, symbol_question, symbol_white_space
        }

        public void GetSymbolsArray(string text, ref Symbols[] arraySymbols)
        {
            for (int i = 0; i < text.Length; i++)
            {
                switch (text[i])
                {
                    case '0':
                        arraySymbols[i] = Symbols.zero; break;
                    case '1':
                        arraySymbols[i] = Symbols.one; break;
                    case '2':
                        arraySymbols[i] = Symbols.two; break;
                    case '3':
                        arraySymbols[i] = Symbols.three; break;
                    case '4':
                        arraySymbols[i] = Symbols.four; break;
                    case '5':
                        arraySymbols[i] = Symbols.five; break;
                    case '6':
                        arraySymbols[i] = Symbols.six; break;
                    case '7':
                        arraySymbols[i] = Symbols.seven; break;
                    case '8':
                        arraySymbols[i] = Symbols.eight; break;
                    case '9':
                        arraySymbols[i] = Symbols.nine; break;
                    case 'A':
                        arraySymbols[i] = Symbols.Upper_A; break;
                    case 'B':
                        arraySymbols[i] = Symbols.Upper_B; break;
                    case 'C':
                        arraySymbols[i] = Symbols.Upper_C; break;
                    case 'D':
                        arraySymbols[i] = Symbols.Upper_D; break;
                    case 'E':
                        arraySymbols[i] = Symbols.Upper_E; break;
                    case 'F':
                        arraySymbols[i] = Symbols.Upper_F; break;
                    case 'G':
                        arraySymbols[i] = Symbols.Upper_G; break;
                    case 'H':
                        arraySymbols[i] = Symbols.Upper_H; break;
                    case 'I':
                        arraySymbols[i] = Symbols.Upper_I; break;
                    case 'J':
                        arraySymbols[i] = Symbols.Upper_J; break;
                    case 'K':
                        arraySymbols[i] = Symbols.Upper_K; break;
                    case 'L':
                        arraySymbols[i] = Symbols.Upper_L; break;
                    case 'M':
                        arraySymbols[i] = Symbols.Upper_M; break;
                    case 'N':
                        arraySymbols[i] = Symbols.Upper_N; break;
                    case 'O':
                        arraySymbols[i] = Symbols.Upper_O; break;
                    case 'P':
                        arraySymbols[i] = Symbols.Upper_P; break;
                    case 'Q':
                        arraySymbols[i] = Symbols.Upper_Q; break;
                    case 'R':
                        arraySymbols[i] = Symbols.Upper_R; break;
                    case 'S':
                        arraySymbols[i] = Symbols.Upper_S; break;
                    case 'T':
                        arraySymbols[i] = Symbols.Upper_T; break;
                    case 'U':
                        arraySymbols[i] = Symbols.Upper_U; break;
                    case 'V':
                        arraySymbols[i] = Symbols.Upper_V; break;
                    case 'W':
                        arraySymbols[i] = Symbols.Upper_W; break;
                    case 'X':
                        arraySymbols[i] = Symbols.Upper_X; break;
                    case 'Y':
                        arraySymbols[i] = Symbols.Upper_Y; break;
                    case 'Z':
                        arraySymbols[i] = Symbols.Upper_Z; break;
                    case 'a':
                        arraySymbols[i] = Symbols.Lower_A; break;
                    case 'b':
                        arraySymbols[i] = Symbols.Lower_B; break;
                    case 'c':
                        arraySymbols[i] = Symbols.Lower_C; break;
                    case 'd':
                        arraySymbols[i] = Symbols.Lower_D; break;
                    case 'e':
                        arraySymbols[i] = Symbols.Lower_E; break;
                    case 'f':
                        arraySymbols[i] = Symbols.Lower_F; break;
                    case 'g':
                        arraySymbols[i] = Symbols.Lower_G; break;
                    case 'h':
                        arraySymbols[i] = Symbols.Lower_H; break;
                    case 'i':
                        arraySymbols[i] = Symbols.Lower_I; break;
                    case 'j':
                        arraySymbols[i] = Symbols.Lower_J; break;
                    case 'k':
                        arraySymbols[i] = Symbols.Lower_K; break;
                    case 'l':
                        arraySymbols[i] = Symbols.Lower_L; break;
                    case 'm':
                        arraySymbols[i] = Symbols.Lower_M; break;
                    case 'n':
                        arraySymbols[i] = Symbols.Lower_N; break;
                    case 'o':
                        arraySymbols[i] = Symbols.Lower_O; break;
                    case 'p':
                        arraySymbols[i] = Symbols.Lower_P; break;
                    case 'q':
                        arraySymbols[i] = Symbols.Lower_Q; break;
                    case 'r':
                        arraySymbols[i] = Symbols.Lower_R; break;
                    case 's':
                        arraySymbols[i] = Symbols.Lower_S; break;
                    case 't':
                        arraySymbols[i] = Symbols.Lower_T; break;
                    case 'u':
                        arraySymbols[i] = Symbols.Lower_U; break;
                    case 'v':
                        arraySymbols[i] = Symbols.Lower_V; break;
                    case 'w':
                        arraySymbols[i] = Symbols.Lower_W; break;
                    case 'x':
                        arraySymbols[i] = Symbols.Lower_X; break;
                    case 'y':
                        arraySymbols[i] = Symbols.Lower_Y; break;
                    case 'z':
                        arraySymbols[i] = Symbols.Lower_Z; break;
                    case '!':
                        arraySymbols[i] = Symbols.symbol_attention; break;
                    case ':':
                        arraySymbols[i] = Symbols.symbol_colon; break;
                    case '>':
                        arraySymbols[i] = Symbols.symbol_continue; break;
                    case '-':
                        arraySymbols[i] = Symbols.symbol_hyphen; break;
                    case '"':
                        arraySymbols[i] = Symbols.symbol_inverted_commas; break;
                    case '.':
                        arraySymbols[i] = Symbols.symbol_point; break;
                    case '?':
                        arraySymbols[i] = Symbols.symbol_question; break;
                    case ' ':
                        arraySymbols[i] = Symbols.symbol_white_space; break;
                }
            }
        }

        public void PaintTextInventory(string text)
        {
            Symbols[] arraySymbols = new Symbols[text.Length];
            int iter = 0;
            int msg_raws_height = 15;
            GetSymbolsArray(text, ref arraySymbols);
            foreach (Symbols symElement in arraySymbols)
            {
                imageListSymbols.Draw(this.CreateGraphics(), imgInventoryTextPos.X + 10 + iter, imgInventoryTextPos.Y + msg_raws_height, (byte)symElement);
                if (iter < imgBackgroundInventory.Width + 100 && symElement != Symbols.symbol_colon)
                {
                    iter += 14;
                }
                else
                {
                    iter = 0;
                    msg_raws_height += 18;
                }
            }
        }

        public void PaintTextEverywhere(string text, Point textPosition, int sizeWidth)
        {
            Symbols[] arraySymbols = new Symbols[text.Length];
            int interval = 0;
            int stringRaws = 0;
            GetSymbolsArray(text, ref arraySymbols);
            foreach (Symbols symElement in arraySymbols)
            {
                imageListSymbols.Draw(this.CreateGraphics(), textPosition.X + interval, textPosition.Y + stringRaws, (byte)symElement);
                if (interval < (textPosition.X + sizeWidth))
                {
                    interval += 14;
                } else
                {
                    interval = 0;
                    stringRaws += 18;
                }
            }
        }

        //public void PaintInteraction() => graph.DrawImage(interactionBackgroundIMG, interactionBackgroundIMGPos);

        public void PaintTextInteraction(string text)
        {
            Symbols[] arraySymbols = new Symbols[text.Length];
            int iter = 0;
            GetSymbolsArray(text, ref arraySymbols);
            foreach(Symbols symElement in arraySymbols)
            {
                imageListSymbols.Draw(this.CreateGraphics(), msgInteractionPos.X + iter, msgInteractionPos.Y, (byte)symElement);
                iter += 14;
            }
        }

        public void ClearTextInteraction()
        {
            int j = 0;
            for(int i = (msgInteractionPos.X / 64); i < arraySize_N; i++)
            {
                imageListGame.Draw(this.CreateGraphics(), i * picSizeWidth, j * picSizeHeight, river_arr[i, j]);
            }
        }

        public void ClearInterfaceItemBack()
        {
            int j = 0;
            for (int i = (myInterfacePos.X / 64); i < (msgInteractionPos.X / 64); i++)
                imageListGame.Draw(this.CreateGraphics(), i * picSizeWidth, j * picSizeHeight, river_arr[i, j]);
        }


        public void PaintMessage(string msg)
        {
            Symbols[] arraySymbols = new Symbols[msg.Length];
            int iter = 0;
            int msg_raws_height = 25;
            imgBackgroundMsgPos = new Point(ClientSize.Width / 3, ClientSize.Height - 150);
            GetSymbolsArray(msg, ref arraySymbols);
            graph.DrawImage(imgBackgroundMsg, imgBackgroundMsgPos);
            foreach (Symbols symElement in arraySymbols)
            {
                imageListSymbols.Draw(this.CreateGraphics(), (imgBackgroundMsgPos.X + 16) + iter, imgBackgroundMsgPos.Y + msg_raws_height, (byte)symElement);
                if (iter < 350 && symElement != Symbols.symbol_colon)
                {
                    iter += 14;
                }
                else
                {
                    iter = 0;
                    msg_raws_height += 16;
                }
            }
        }
        // отрисовка кнопок на поле сообщения
        public void PaintMessage_Buttons()
        {
            #region drawing_collect_btn
            msgCollectBtn = Properties.Resources.collect_word; // добавляем внедренный ресурс в переменную класса Image
            msgCollectBtnPos = new Point(imgBackgroundMsgPos.X + 30, imgBackgroundMsgPos.Y + 70); // задаем точку расположения кнопки на карте
            graph.DrawImage(msgCollectBtn, msgCollectBtnPos); // отрисовка кнопки на карте
            #endregion
            #region drawing_collect_back_btn
            msgBackBtn = Properties.Resources.back_word;
            msgBackBtnPos = new Point(imgBackgroundMsgPos.X + 270, imgBackgroundMsgPos.Y + 70);
            graph.DrawImage(msgBackBtn, msgBackBtnPos);
            #endregion
        }

        public void Clear_Message()
        {
            for (int i = imgBackgroundMsgPos.X / picSizeWidth; i < arraySize_N; i++)
                for (int j = imgBackgroundMsgPos.Y / picSizeHeight; j < arraySize_M; j++)
                {
                    if (map[i, j] == 79) imageListGame.Draw(this.CreateGraphics(), i * picSizeWidth, j * picSizeHeight, river_arr[i, j]);
                    else
                        imageListGame.Draw(this.CreateGraphics(), i * picSizeWidth, j * picSizeHeight, map[i, j]);
                }
            foreach(Tree treeElement in myTrees)
            {
                if (treeElement.HitPoints == healthStatus.HP0_stump && treeElement.ShovelUsed == ShovelStatus.is_not)
                    if (treeElement.WateringStumpState == WaterStatus.none)
                        imageListGame.Draw(this.CreateGraphics(), treeElement.TreeLocation.X * picSizeWidth, treeElement.TreeLocation.Y * picSizeHeight, (byte)treeElement.HitPoints);
                    else
                        imageListGame.Draw(this.CreateGraphics(), treeElement.TreeLocation.X * picSizeWidth, treeElement.TreeLocation.Y * picSizeHeight, (byte)treeElement.WateringStumpState);
            }
        }

        public void CheckResDirectory()
        {
            if (!Directory.Exists(Application.StartupPath + @"\ResGame"))
            {
                Directory.CreateDirectory(Application.StartupPath + @"\ResGame");
            }
        }

        public Uri GetRelativeUriRes(string fileName)
        {
            Uri relativeUri = new Uri(fileName, System.UriKind.Relative);
            return relativeUri;
        }

        public string GetResourceSoundToFileTMP(string resourceName)
        {
            CheckResDirectory();
            Assembly executingAssembly = Assembly.GetExecutingAssembly();
            System.IO.Stream resourceStream = executingAssembly.GetManifestResourceStream(Application.ProductName + ".Resources." + resourceName);
            string fileName = @"ResGame\tmp_" + resourceName;
            System.IO.Stream fileStream = System.IO.File.OpenWrite(fileName);
            resourceStream.CopyTo(fileStream);
            fileStream.Flush();
            fileStream.Close();
            return fileName;
        }

        public void Initial_Sounds()
        {
            mainMenuThemeSound.Stream = Properties.Resources._333800__foolboymedia__sunday_morning_in_the_great_hall;
            gameThemeSound.Stream = Properties.Resources._257997__foolboymedia__shanty_town;
            apartmentSound.Stream = Properties.Resources.sky_loop;
            // звук нажатия кнопки в меню и сообщениях
            btnClickSound.Open(GetRelativeUriRes(GetResourceSoundToFileTMP("sound_btn_clicked.wav")));
            // звук сообщения
            msgSound.Open(GetRelativeUriRes(GetResourceSoundToFileTMP("264828__cmdrobot__text-message-or-videogame-jump.wav")));
            #region sound_sword_attack_in_air
            soundSwordAttack_air.Open(GetRelativeUriRes(GetResourceSoundToFileTMP("air_sword.wav")));
            soundSwordAttack_air.Volume = 0.2;
            #endregion
            #region sound_sword_attack_tree
            soundSwordAttack_tree.Open(GetRelativeUriRes(GetResourceSoundToFileTMP("tree_sword.wav")));
            soundSwordAttack_tree.Volume = 0.2;
            #endregion
            #region sound_inventory
            inventorySound.Open(GetRelativeUriRes(GetResourceSoundToFileTMP("467605__triqystudio__closebag.wav")));
            #endregion
            #region sound_pickup_item
            soundPickupItem.Open(GetRelativeUriRes(GetResourceSoundToFileTMP("pickup_item.wav")));
            #endregion
            #region sound_building
            soundBuilding.Open(GetRelativeUriRes(GetResourceSoundToFileTMP("build_construction.wav")));
            #endregion
            #region sound_attack_enemy
            soundAttackSwordEnemy.Open(GetRelativeUriRes(GetResourceSoundToFileTMP("sword_attack_enemies.wav")));
            #endregion
            #region sound_throw_and_destroy_stone
            soundThrowStone.Open(GetRelativeUriRes(GetResourceSoundToFileTMP("throw_stone.wav")));
            soundDestroyStone.Open(GetRelativeUriRes(GetResourceSoundToFileTMP("stone_crashed.wav")));
            #endregion
            #region bow_sounds
            soundBowPull.Open(GetRelativeUriRes(GetResourceSoundToFileTMP("bow_pull.wav")));
            soundBowSwishing.Open(GetRelativeUriRes(GetResourceSoundToFileTMP("bow_swishing.wav")));
            #endregion
            #region sound_pickup_coin
            soundPickUpCoin.Open(GetRelativeUriRes(GetResourceSoundToFileTMP("pickup_coin.wav")));
            #endregion
        }

        public void Initial_GUI()
        {
            player.PlayerHealth = 3;
            player.FoodPoints = gameFoodPoints;
            FoodPointsPos = new Point(132, 10);
            player.EnergyPoints = gameEnergyPoints;
            EnergyPointsPos = new Point(244, 10);
            player.PlayerExperience = 0;
            LevelPointsPos = new Point(574, 10);
            player.PlayerLevel = 1;
        }

        public void Paint_GUIHitPoints()
        {
            imgUI.Draw(this.CreateGraphics(), 10, 10, player.PlayerHealth);
        }

        public void Paint_GUICoins()
        {
            graph.DrawImage(imageCoinGUI, imageCoinGUIPos);
            DrawValuesInventoryForCoins(player.CoinCount, imageCoinGUIPos, 6);
        }

        public void Paint_GUI()
        {
            Paint_GUIHitPoints();
            Paint_GUIFoodPoints();
            Paint_GUIEnergyPoints();
            Paint_GUILevelPoints();
            Paint_GUICoins();
        }

        public void Initial_River()
        {
            river_arr[0, 0] = 75;
            river_arr[arraySize_N - 1, 0] = 70;
            river_arr[0, arraySize_M - 1] = 78;
            river_arr[arraySize_N - 1, arraySize_M - 1] = 73;
            for (int i = 1; i < arraySize_N - 1; i++)
            {
                river_arr[i, 0] = 67;
            }
            for (int j = 1; j < arraySize_M - 1; j++)
            {
                river_arr[0, j] = 76;
            }
            for (int i = 1; i < arraySize_N - 1; i++)
            {
                river_arr[i, arraySize_M - 1] = 68;
            }
            for (int j = 1; j < arraySize_M - 1; j++)
            {
                river_arr[arraySize_N - 1, j] = 71;
            }
            for (int i = 1; i < arraySize_N - 1; i++)
                for (int j = 1; j < arraySize_M - 1; j++)
                {
                    if (map[i, j] == 79)
                    {
                        if (map[i - 1, j] != 79 && map[i, j - 1] != 79)
                        {
                            river_arr[i, j] = 72; // левый верхний угол
                        }
                        if (map[i + 1, j] != 79 && map[i, j - 1] != 79)
                        {
                            river_arr[i, j] = 77; // правый верхний угол
                        }
                        if (map[i - 1, j] != 79 && map[i, j + 1] != 79)
                        {
                            river_arr[i, j] = 69; // левый нижний угол
                        }
                        if (map[i + 1, j] != 79 && map[i, j + 1] != 79)
                        {
                            river_arr[i, j] = 74; // правый нижний угол
                        }
                        if (map[i - 1, j] != 79 && map[i, j - 1] == 79 && map[i, j + 1] == 79)
                        {
                            river_arr[i, j] = 71; // левая середина
                        }
                        if (map[i + 1, j] != 79 && map[i, j - 1] == 79 && map[i, j + 1] == 79)
                        {
                            river_arr[i, j] = 76; // правая середина
                        }
                        if (map[i, j - 1] != 79 && map[i - 1, j] == 79 && map[i + 1, j] == 79)
                        {
                            river_arr[i, j] = 68; // верхняя середина
                        }
                        if (map[i, j + 1] != 79 && map[i - 1, j] == 79 && map[i + 1, j] == 79)
                        {
                            river_arr[i, j] = 67; // нижняя середина
                        }
                        if (map[i - 1, j] == 79 && map[i, j + 1] == 79 && map[i - 1, j + 1] != 79)
                        {
                            river_arr[i, j] = 70; // левый нижний край
                        }
                        if (map[i + 1, j] == 79 && map[i, j - 1] == 79 && map[i + 1, j - 1] != 79)
                        {
                            river_arr[i, j] = 78; // правый верхний край
                        }
                        if (map[i, j - 1] == 79 && map[i - 1, j] == 79 && map[i - 1, j - 1] != 79)
                        {
                            river_arr[i, j] = 73; // левый верхний край
                        }
                        if (map[i + 1, j] == 79 && map[i, j + 1] == 79 && map[i + 1, j + 1] != 79)
                        {
                            river_arr[i, j] = 75; // правый нижний край
                        }
                        if (map[i + 1, j] == 79 && map[i - 1, j] == 79 && map[i, j - 1] == 79 && map[i, j + 1] == 79 && map[i + 1, j - 1] == 79 && map[i + 1, j + 1] == 79 && map[i - 1, j - 1] == 79 && map[i - 1, j + 1] == 79)
                        {
                            river_arr[i, j] = 66; // спрайт расположенный внутри реки, центр
                        }
                    }
                }
        }

        public void PaintRiver()
        {
            for (int i = 0; i < arraySize_N; i++)
                for (int j = 0; j < arraySize_M; j++)
                {
                    if (map[i, j] == 79) imageListGame.Draw(this.CreateGraphics(), i * picSizeWidth, j * picSizeHeight, river_arr[i, j]);
                }
        }

        public void PaintMap()
        {
            for (int i = 0; i < arraySize_N; i++)
                for (int j = 0; j < arraySize_M; j++)
                {
                    imageListGame.Draw(this.CreateGraphics(), i * picSizeWidth, j * picSizeHeight, map[i, j]);
                }
        }

        public void ResetMap()
        {
            for (int i = 0; i < arraySize_N; i++)
                for (int j = 0; j < arraySize_M; j++)
                    map[i, j] = 0;
        }

        public void RandomMapping()
        {
            int x, y;
            Random rand = new Random(DateTime.Now.Millisecond);
            //заполнение массива карты травой и деревьями случаным образом
            for (int i = 0; i < 30; i++)
                for (int j = 0; j < 16; j++)
                {
                    x = rand.Next(0, 30);
                    y = rand.Next(5, 16);
                    map[x, y] = rand.Next(0, 2);
                }
            //заполнение массива карты камнем случайным образом
            for (int i = 0; i < 30; i++)
                for (int j = 0; j < 16; j++)
                {
                    x = rand.Next(15, 20);
                    y = rand.Next(10, 15);
                    map[x, y] = rand.Next(94, 96);
                }
            //заполнение массива карты значениями для отрисовки реки
            map[20, 8] = 79;
            map[21, 8] = 79;
            //----------------
            map[20, 9] = 79;
            map[21, 9] = 79;
            map[23, 9] = 79;
            map[24, 9] = 79;
            //----------------
            map[20, 10] = 79;
            map[21, 10] = 79;
            map[22, 10] = 79;
            map[23, 10] = 79;
            map[24, 10] = 79;
            //----------------
            map[19, 11] = 79;
            map[20, 11] = 79;
            map[21, 11] = 79;
            map[22, 11] = 79;
            map[23, 11] = 79;
            map[24, 11] = 79;
            map[25, 11] = 79;
            //----------------
            map[19, 12] = 79;
            map[20, 12] = 79;
            map[21, 12] = 79;
            map[22, 12] = 79;
            map[23, 12] = 79;
            map[24, 12] = 79;
            map[25, 12] = 79;
            //----------------
            map[19, 13] = 79;
            map[20, 13] = 79;
            map[22, 13] = 79;
            map[23, 13] = 79;
            map[24, 13] = 79;
            //----------------
            map[22, 14] = 79;
            map[23, 14] = 79;
            //конец заполнения массива с рекой

            map[player.PlayerPos.X, player.PlayerPos.Y] = 2; //начальное положение персонажа
            map[10, 11] = 80;

            // границы карты
            x = 0;
            for (int j = 0; j < arraySize_M; j++)
            {
                map[x, j] = 79;
            }
            for (int i = 0; i < arraySize_N; i++)
            {
                map[i, x] = 79;
            }
            x = arraySize_N - 1;
            for (int j = 0; j < arraySize_M; j++)
            {
                map[x, j] = 79;
            }
            x = arraySize_M - 1;
            for (int i = 0; i < arraySize_N; i++)
            {
                map[i, x] = 79;
            }
            //--------------
        }

        private void menu_timer_Tick(object sender, EventArgs e)
        {
            Menu_MouseMove();
        }

        public void ValueToNumbers(int val)
        {
            int a;
            while (val != 0)
            {
                a = val % 10;
                val /= 10;
                valScore.Push(a);
            }
        }

        public void LevelUp()
        {
            player.PlayerExperience = 0;
            gameLevelPoints += (10 + levelStep);
            levelStep++;
            player.PlayerLevel++;
        }

        public void Paint_GUILevelPoints()
        {
            Paint_GUILevelBar();
            Paint_GUILevelPointsValues();
        }

        public void Paint_GUILevelBar()
        {
            int x;
            x = (player.PlayerExperience * 100) / gameLevelPoints;
            imgUI_EnergyPoints.Draw(this.CreateGraphics(), LevelPointsPos.X, LevelPointsPos.Y, x + 102);
        }

        public void Paint_GUILevelPointsValues()
        {
            int b = 0;
            imgListValues.Draw(this.CreateGraphics(), (LevelPointsPos.X + (302 / 3)) + b, LevelPointsPos.Y + 5, 18); // drawing 'L'
            b += 14;
            imgListValues.Draw(this.CreateGraphics(), (LevelPointsPos.X + (302 / 3)) + b, LevelPointsPos.Y + 5, 19); // drawing 'V'
            b += 14;
            imgListValues.Draw(this.CreateGraphics(), (LevelPointsPos.X + (302 / 3)) + b, LevelPointsPos.Y + 5, 18); // drawing 'L'
            b += 16;
            ValueToNumbers(player.PlayerLevel);
            while (valScore.Count != 0)
            {
                imgListValues.Draw(this.CreateGraphics(), (LevelPointsPos.X + (302 / 3)) + b, LevelPointsPos.Y + 5, valScore.Pop());
                b += 14;
            }
            b += 14; // пробел
            imgListValues.Draw(this.CreateGraphics(), (LevelPointsPos.X + (302 / 3)) + b, LevelPointsPos.Y + 5, 16); // drawing 'X'
            b += 14;
            imgListValues.Draw(this.CreateGraphics(), (LevelPointsPos.X + (302 / 3)) + b, LevelPointsPos.Y + 5, 17); // drawing 'P'
            b += 16;
            ValueToNumbers(player.PlayerExperience);
            while (valScore.Count != 0)
            {
                imgListValues.Draw(this.CreateGraphics(), (LevelPointsPos.X + (302 / 3)) + b, LevelPointsPos.Y + 5, valScore.Pop());
                b += 14;
            }
            imgListValues.Draw(this.CreateGraphics(), (LevelPointsPos.X + (302 / 3)) + b, LevelPointsPos.Y + 5, 11); // drawing '/'
            b += 12;
            ValueToNumbers(gameLevelPoints);
            while (valScore.Count != 0)
            {
                imgListValues.Draw(this.CreateGraphics(), (LevelPointsPos.X + (302 / 3)) + b, LevelPointsPos.Y + 5, valScore.Pop());
                b += 14;
            }
        }

        public void Paint_GUIEnergyBar()
        {
            int x;
            x = (player.EnergyPoints * 100) / gameEnergyPoints;
            imgUI_EnergyPoints.Draw(this.CreateGraphics(), EnergyPointsPos.X, EnergyPointsPos.Y, x);
        }

        public void Paint_GUIEnergyPointsValues()
        {
            int b = 0;
            imgListValues.Draw(this.CreateGraphics(), (EnergyPointsPos.X + (302 / 3)) + b, EnergyPointsPos.Y + 5, 15); // drawing 'E'
            b += 14;
            imgListValues.Draw(this.CreateGraphics(), (EnergyPointsPos.X + (302 / 3)) + b, EnergyPointsPos.Y + 5, 17); // drawing 'P'
            b += 16;
            ValueToNumbers(player.EnergyPoints);
            while (valScore.Count != 0)
            {
                imgListValues.Draw(this.CreateGraphics(), (EnergyPointsPos.X + (302 / 3)) + b, EnergyPointsPos.Y + 5, valScore.Pop());
                b += 14;
            }
            imgListValues.Draw(this.CreateGraphics(), (EnergyPointsPos.X + (302 / 3)) + b, EnergyPointsPos.Y + 5, 11); // '/'
            b += 12;
            ValueToNumbers(gameEnergyPoints);
            while (valScore.Count != 0)
            {
                imgListValues.Draw(this.CreateGraphics(), (EnergyPointsPos.X + (302 / 3)) + b, EnergyPointsPos.Y + 5, valScore.Pop());
                b += 14;
            }
        }

        public void Paint_GUIEnergyPoints()
        {
            Paint_GUIEnergyBar();
            Paint_GUIEnergyPointsValues();
        }

        public void Paint_GUIFoodsBar()
        {
            int x;
            x = (player.FoodPoints * 100) / gameFoodPoints;
            imgUI.Draw(this.CreateGraphics(), FoodPointsPos.X, FoodPointsPos.Y, x + 4);
        }

        public void Paint_GUIFoodPointsValues()
        {
            int b = 0;
            ValueToNumbers(player.FoodPoints);
            while (valScore.Count != 0)
            {
                imgListValues.Draw(this.CreateGraphics(), (FoodPointsPos.X + (112 / 5)) + b, FoodPointsPos.Y + 5, valScore.Pop());
                b += 14;
            }
            imgListValues.Draw(this.CreateGraphics(), (FoodPointsPos.X + (112 / 5)) + (b - 3), FoodPointsPos.Y + 5, 11); // drawing '/'
            b += 10;
            ValueToNumbers(gameFoodPoints);
            while (valScore.Count != 0)
            {
                imgListValues.Draw(this.CreateGraphics(), (FoodPointsPos.X + (112 / 5)) + b, FoodPointsPos.Y + 5, valScore.Pop());
                b += 14;
            }
        }

        public void Paint_GUIFoodPoints()
        {
            Paint_GUIFoodsBar();
            Paint_GUIFoodPointsValues();
        }

        public void Initial_Wood()
        {
            myWoods = new Wood[countWoodElements]; // объявляем массив древесины с прогнозируемым количеством материала на карте
            for (int i = 0; i < countWoodElements; i++)
            {
                myWoods[i] = new Wood(countStatus.wood_empty, new Point(-1, -1), true, 0);
            }
        }

        public void DrawValuesAnimateItem(string str, Point itemLocation, int b, int h = 0)
        {
            for (int i = str.Length - 1; i >= 0; i--)
            {
                int x = int.Parse(str[i].ToString());
                imgListValues.Draw(this.CreateGraphics(), (itemLocation.X * picSizeWidth) + b, (itemLocation.Y * picSizeHeight) + 44 - h, x);
                if (i == 0) b -= 10; else b -= 15;
            }
            imgListValues.Draw(this.CreateGraphics(), (itemLocation.X * picSizeWidth) + b, (itemLocation.Y * picSizeHeight) + 44 - h, 10); // drawing 'x'
        }

        public void WoodAnimation()
        {
            string woodCountStr = "";
            int b = 44; // шаг отрисовки
            if (myWoods != null)
            {
                if (wood_one_step <= 88)
                {
                    foreach (Wood woodElement in myWoods)
                    {
                        if (woodElement.CountState == countStatus.wood_one && woodElement.WoodLocation != player.PlayerPos)
                        {
                            imageListGame.Draw(this.CreateGraphics(), woodElement.WoodLocation.X * picSizeWidth, woodElement.WoodLocation.Y * picSizeHeight, wood_one_step);
                            woodCountStr = woodElement.ContainWood.ToString();
                            DrawValuesAnimateItem(woodCountStr, woodElement.WoodLocation, b);
                            b = 44;
                        } else if (woodElement.CountState == countStatus.wood_one && woodElement.WoodLocation == player.PlayerPos)
                        {
                            switch (map[player.PlayerPos.X, player.PlayerPos.Y]) // проверяем положение персонажа на карте и отрисовываем соответствующие картинки
                            {
                                case 2: // герой стоит лицом вниз
                                    {
                                        imageListGame.Draw(this.CreateGraphics(), player.PlayerPos.X * picSizeWidth, player.PlayerPos.Y * picSizeHeight, 120);
                                        break;
                                    }
                                case 3: // герой стоит лицом вверх
                                    {
                                        imageListGame.Draw(this.CreateGraphics(), player.PlayerPos.X * picSizeWidth, player.PlayerPos.Y * picSizeHeight, 121);
                                        break;
                                    }
                                case 4: // герой стоит лицом влево
                                    {
                                        imageListGame.Draw(this.CreateGraphics(), player.PlayerPos.X * picSizeWidth, player.PlayerPos.Y * picSizeHeight, 122);
                                        break;
                                    }
                                case 5: // герой стоит лицом вправо
                                    {
                                        imageListGame.Draw(this.CreateGraphics(), player.PlayerPos.X * picSizeWidth, player.PlayerPos.Y * picSizeHeight, 123);
                                        break;
                                    }
                            }
                        }
                    }
                    wood_one_step++;
                } else
                {
                    wood_one_step = 81;
                }

                if (wood_two_step <= 92)
                {
                    foreach (Wood woodElement in myWoods)
                    {
                        if (woodElement.CountState == countStatus.wood_more_three && woodElement.WoodLocation != player.PlayerPos)
                        {
                            imageListGame.Draw(this.CreateGraphics(), woodElement.WoodLocation.X * picSizeWidth, woodElement.WoodLocation.Y * picSizeHeight, wood_two_step);
                            woodCountStr = woodElement.ContainWood.ToString();
                            DrawValuesAnimateItem(woodCountStr, woodElement.WoodLocation, b);
                            b = 44;
                        } else if (woodElement.CountState == countStatus.wood_more_three && woodElement.WoodLocation == player.PlayerPos)
                        {
                            switch (map[player.PlayerPos.X, player.PlayerPos.Y]) // проверяем положение персонажа на карте и отрисовываем соответствующие картинки
                            {
                                case 2: // герой стоит лицом вниз
                                    {
                                        imageListGame.Draw(this.CreateGraphics(), player.PlayerPos.X * picSizeWidth, player.PlayerPos.Y * picSizeHeight, 124);
                                        break;
                                    }
                                case 3: // герой стоит лицом вверх
                                    {
                                        imageListGame.Draw(this.CreateGraphics(), player.PlayerPos.X * picSizeWidth, player.PlayerPos.Y * picSizeHeight, 125);
                                        break;
                                    }
                                case 4: // герой стоит лицом влево
                                    {
                                        imageListGame.Draw(this.CreateGraphics(), player.PlayerPos.X * picSizeWidth, player.PlayerPos.Y * picSizeHeight, 126);
                                        break;
                                    }
                                case 5: // герой стоит лицом вправо
                                    {
                                        imageListGame.Draw(this.CreateGraphics(), player.PlayerPos.X * picSizeWidth, player.PlayerPos.Y * picSizeHeight, 127);
                                        break;
                                    }
                            }
                        }
                    }
                    wood_two_step++;
                } else
                {
                    wood_two_step = 89;
                }
            }
        }

        public void Initial_Hearts()
        {
            /// TODO: do hearts on map
        }

        public void Initial_Bottles()
        {
            int bottleStep = -1;
            int bottlesCount = 0;
            foreach(int mapElement in map)
            {
                if(mapElement == 80)
                {
                    bottlesCount++; // подсчитываем сколько бутылок присутствует на карте
                }
            }
            myBottles = new Bottle[bottlesCount]; // объявляем массив бутылок размером равным количеству бутылок на карте
            for(int i = 0; i < arraySize_N; i++)
                for(int j = 0; j < arraySize_M; j++)
                {
                    if(map[i, j] == 80)
                    {
                        bottleStep++;
                        myBottles[bottleStep] = new Bottle(bottleStatus.bottle_empty, new Point(i, j)); // создаем каждую бутылку как объект класса Bottle
                    }
                }
        }

        public void BottleAnimation()
        {
            int b = 44;
            int h = 10;
            string bottleCountStr = "";
            if(myBottles != null)
            {
                if(bottle_empty_step <= 137)
                {
                    foreach(Bottle bottleElement in myBottles)
                    {
                        if(bottleElement.BottleLocation != player.PlayerPos && bottleElement.BottleState == bottleStatus.bottle_empty)
                        {
                            imageListGame.Draw(this.CreateGraphics(), bottleElement.BottleLocation.X * picSizeWidth, bottleElement.BottleLocation.Y * picSizeHeight, bottle_empty_step);
                            bottleCountStr = bottleElement.ContainBottle.ToString();
                            DrawValuesAnimateItem(bottleCountStr, bottleElement.BottleLocation, b, h);
                            b = 44;
                        }
                    }
                    bottle_empty_step++;
                } else
                {
                    bottle_empty_step = 128;
                }
            }
        }

        public void Initial_Trees()
        {
            treesCount = 0;
            int treeStep = -1;
            foreach (int mapElement in map)
            {
                if (mapElement == 1) treesCount++; // подсчитываем сколько деревьев вообще присутствует на карте
            }
            myTrees = new Tree[treesCount]; // объявляем массив деревьев размером с количество деревьев на карте
            for (int i = 0; i < arraySize_N; i++)
                for (int j = 0; j < arraySize_M; j++)
                {
                    if (map[i, j] == 1) // элемент с позицией i, j равен 1 (значение дерева)
                    {
                        treeStep++;
                        myTrees[treeStep] = new Tree(new Point(i, j), healthStatus.HP100); // создаем каждое дерево как объект класса Tree, в конструкторе задаем позицию дерева и его здоровье
                    }
                }
        }

        public void Initial_Rocks_and_Stones()
        {
            int rocksCount = 0;
            int rockStep = -1;
            foreach(int mapElement in map)
            {
                if (mapElement == 95) rocksCount++;
            }
            myRocks = new Rock[rocksCount]; // объявляем массив камней размером с количество камней на карте
            myStones = new Stone[rocksCount];
            for(int i = 0; i < arraySize_N; i++)
                for(int j = 0; j < arraySize_M; j++)
                {
                    if(map[i, j] == 95)
                    {
                        rockStep++;
                        myRocks[rockStep] = new Rock(rockHealthStatus.HP100, new Point(i, j)); // создаем каждый камень как объект класса Rock, в конструкторе задаем позицию камня и его здоровье
                        myStones[rockStep] = new Stone(stoneStatus.is_rock, new Point(i, j)); // создаем каждый камушек как объект класса Stone, в конструкторе задаем позицию камушка и его здоровье
                    }
                }
        }

        public void StoneAnimation()
        {
            int b = 44;
            int h = 10;
            string stoneCountStr = "";
            if(myStones != null)
            {
                if(stone_step <= 18)
                {
                    foreach(Stone stoneElement in myStones)
                    {
                        if(stoneElement.StoneLocation != player.PlayerPos && stoneElement.StoneState == stoneStatus.is_stone)
                        {
                            imgListStone.Draw(this.CreateGraphics(), stoneElement.StoneLocation.X * picSizeWidth, stoneElement.StoneLocation.Y * picSizeHeight, stone_step);
                            stoneCountStr = stoneElement.ContainStone.ToString();
                            DrawValuesAnimateItem(stoneCountStr, stoneElement.StoneLocation, b, h);
                        }
                    }
                    stone_step++;
                } else
                {
                    stone_step = 8;
                }
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            //......some shit
            Assembly executAssembly = Assembly.GetExecutingAssembly();
            System.IO.Stream resourceStream = executAssembly.GetManifestResourceStream(Application.ProductName + ".Resources.cursorGauntlet_bronze.cur");
            string fileName = "tmp_game_cursor.cur";
            System.IO.Stream fileStream = System.IO.File.OpenWrite(fileName);
            resourceStream.CopyTo(fileStream);
            fileStream.Flush();
            fileStream.Close();
            Initial_Sounds(); // подгружаем музыкальное сопровождение
            mainMenuThemeSound.PlayLooping(); // воспроизводим циклически фоновую музыку для главного меню
        }

        public void MessageAnimation()
        {
            // сообщение в начале новой игры с подсказкой как управлять игроком
            if (msgGameControls)
            {
                // стрелка вверх подсказка движения
                if (arrow_up_step <= 101)
                {
                    imageListGame.Draw(this.CreateGraphics(), player.PlayerPos.X * picSizeWidth, (player.PlayerPos.Y - 1) * picSizeHeight, arrow_up_step);
                    arrow_up_step++;
                } else
                {
                    arrow_up_step = 96;
                }
                // стрелка вниз подсказка движения
                if (arrow_down_step <= 107)
                {
                    imageListGame.Draw(this.CreateGraphics(), player.PlayerPos.X * picSizeWidth, (player.PlayerPos.Y + 1) * picSizeHeight, arrow_down_step);
                    arrow_down_step++;
                } else
                {
                    arrow_down_step = 102;
                }
                // стрелка влево подсказка движения
                if (arrow_left_step <= 113)
                {
                    imageListGame.Draw(this.CreateGraphics(), (player.PlayerPos.X - 1) * picSizeWidth, player.PlayerPos.Y * picSizeHeight, arrow_left_step);
                    arrow_left_step++;
                } else
                {
                    arrow_left_step = 108;
                }
                // стрелка вправо подсказка движения
                if (arrow_right_step <= 119)
                {
                    imageListGame.Draw(this.CreateGraphics(), (player.PlayerPos.X + 1) * picSizeWidth, player.PlayerPos.Y * picSizeHeight, arrow_right_step);
                    arrow_right_step++;
                } else
                {
                    arrow_right_step = 114;
                }
            }
        }

        private void Form1_Paint(object sender, PaintEventArgs e)
        {
            if (status_start_menu) // проврка первого запуска игры
            {
                PaintStartMenu(); // отрисовка стартового меню
                menu_timer.Enabled = true; // включаем таймер для меню
            }
        }

        public void Initial_Enemies()
        {
            int possibleCountEnemies = 21;
            enemyCollection = new Enemy[possibleCountEnemies];
            for(int i = 0; i < enemyCollection.Length; i++)
            {
                enemyCollection[i] = new Enemy();
            }
            countEnemies = 0;
            countdownEnemy = 100;
            //totalCountEnemiesOnMap = possibleCountEnemies;
        }

        public void Prepare_Collections_Map()
        {
            map = new int[arraySize_N, arraySize_M];
            river_arr = new int[arraySize_N, arraySize_M];
            room_map = new int[room_arraySize_N, room_arraySize_M];
        }

        public void MappingRoom()
        {
            for(int i = room_x; i < room_x + 4; i++)
                for(int j = room_y; j < room_y + 4; j++)
                {
                    room_map[i, j] = 1;
                }
            room_map[room_x, room_y] = 2;
            playerRoomPos = new Point(14, 9);
            room_map[playerRoomPos.X, playerRoomPos.Y] = 4;
        }

        public void PaintRoom()
        {
            for(int i = 0; i < room_arraySize_N; i++)
                for(int j = 0; j < room_arraySize_M; j++)
                {
                    imgListRoom.Draw(this.CreateGraphics(), i * picSizeWidth, j * picSizeHeight, room_map[i, j]);
                }
        }

        public void Initial_Map()
        {
            launchCountdownTimer = true; // запуск обратного отсчета
            enemyMapping = false; // отрисовка нового врага на карте
            allEnemiesDied = true; // все ли враги ликвидированы
            currentlyEnemiesDied = 0; // убито врагов на данный момент
            player.KilledEnemies = 0; // убито врагов героем

            Initial_Enemies();
            freeze_actions = true; // заблокировать движения игрока, истина ибо в начале игры персонаж недвижим
            arrow_up_t = arrow_down_t = arrow_left_t = arrow_right_t = false; //задаем значение истины, чтобы воспользоваться кнопками
            down_step++; up_step++; left_step++; right_step++; ////наращиваем заранее на 1, чтобы не отрисовывать персонажа в покое дважды
            myInterface.GetItemsPosition(myInterfacePos); // задаем позицию элементам интерфейса
            player.PState = playerStatus.edle;
            Prepare_Collections_Map();
            player.PlayerPos = new Point(5, 5);
            #region Наличие предметов в сумке на старте игры
            player.WoodCount = 0;
            player.StoneCount = 0;
            player.CoinCount = 0;
            player.EmptyBottles = 0;
            player.WaterBottles = 0;
            player.ItemSword = true;
            player.ItemBow = true;
            player.ShovelCount = 0;
            #endregion
            //MappingRoom();
            RandomMapping();
            Initial_River();
            PaintMap();
            PaintRiver();
            Initial_Trees();
            Initial_Rocks_and_Stones();
            Initial_Wood();
            Initial_Bottles();
            //GUI
            Initial_GUI();
            Paint_GUI();
            //----
        }

        public void PaintStartMenu()
        {
            Prepare_Collections_Map();
            PaintMap();
            graph = this.CreateGraphics();
            graph.DrawImage(startMenuForm, new Point(this.Width / 8, this.Height / 4));
        }

        public void PaintMenu()
        {
            PaintMap();
            #region drawing_menu_form
            Image mainMenu = Properties.Resources.main_menu;
            graph.DrawImage(mainMenu, new Point(this.Width / 8, this.Height / 4));
            #endregion

            #region button_new_game
            #region заметка!!!
            /*newGameLbl = new Label();
            newGameLbl.Left = (ClientSize.Width / 5) + 510;
            newGameLbl.Top = (ClientSize.Height / 4) + 200;
            newGameLbl.ImageList = imgListMenuCtrls;
            newGameLbl.ImageIndex = 0;
            newGameLbl.Parent = this.Parent;
            newGameLbl.BackColor = Color.Transparent;
            newGameLbl.MouseClick += new MouseEventHandler(LabelNewGame_MouseMove);
            this.Controls.Add(newGameLbl);*/
            #endregion
            newGameBtn = Properties.Resources.new_game_word;
            newGameBtnPos = new Point((mainMenu.Width + 200), (mainMenu.Height / 2) + 300);
            graph.DrawImage(newGameBtn, newGameBtnPos);
            #endregion

            #region button_load_game
            loadGameBtn = Properties.Resources.load_game_word;
            loadGameBtnPos = new Point((mainMenu.Width + 200), (mainMenu.Height / 2) + 350);
            graph.DrawImage(loadGameBtn, loadGameBtnPos);
            #endregion

            #region button_author_game
            authorGameBtn = Properties.Resources.author_word;
            authorGameBtnPos = new Point((mainMenu.Width + 200), (mainMenu.Height / 2) + 400);
            graph.DrawImage(authorGameBtn, authorGameBtnPos);
            #endregion

            #region button_exit_game
            exitGameBtn = Properties.Resources.exit_word;
            exitGameBtnPos = new Point((mainMenu.Width + 200), (mainMenu.Height / 2) + 450);
            graph.DrawImage(exitGameBtn, exitGameBtnPos);
            #endregion
        }

        public void PaintMenuAuthor()
        {
            PaintMap();
            #region drawing_menu_form
            authorGameMenuPos = new Point(this.Width / 8, this.Height / 4);
            graph.DrawImage(authorGameMenu, authorGameMenuPos);
            #endregion

            #region drawing_button_back
            authorBackBtn = Properties.Resources.back_word;
            authorBackBtnPos = new Point(authorGameMenuPos.X + (authorGameMenu.Width - 180), authorGameMenuPos.Y + 450);
            #endregion
        }

        public void PaintMenuPause()
        {
            #region drawing_menu_pause_form
            imgBackgroundPauseMenuPos = new Point(this.Width / 8, this.Height / 4);
            graph.DrawImage(imgBackgroundPauseMenu, imgBackgroundPauseMenuPos);
            #endregion

            #region drawing_button_going_to_main_menu
            imgMainMenuBtn = Properties.Resources.main_menu_word;
            imgMainMenuBtnPos = new Point(imgBackgroundPauseMenuPos.X + (imgBackgroundPauseMenu.Width - 210), imgBackgroundPauseMenuPos.Y + 450);
            graph.DrawImage(imgMainMenuBtn, imgMainMenuBtnPos);
            #endregion

            anyBtnsClicked_PauseMenu = false;
        }

        public void PaintMenuGameOver()
        {
            mainMenuThemeSound.PlayLooping();
            ResetMap();
            PaintMap();
            #region drawing_form_gameover
            imgBackgroundGameOverPos = new Point(this.Width / 8, this.Height / 4);
            graph.DrawImage(imgBackgroundGameOver, imgBackgroundGameOverPos);
            #endregion

            #region drawing_name_player_profile
            Point namePos = new Point(imgBackgroundGameOverPos.X + 200, imgBackgroundGameOverPos.Y + 260);
            PaintTextEverywhere("Player: " + player.PlayerName.ToString(), namePos, imgBackgroundGameOver.Width);
            #endregion

            #region drawing_count_enemies_hero_killed
            Point textPos = new Point(imgBackgroundGameOverPos.X + 200, imgBackgroundGameOverPos.Y + 340);
            PaintTextEverywhere("Count enemies whos you killed", textPos, imgBackgroundGameOver.Width);
            Point valuesPos = new Point(textPos.X + (imgBackgroundGameOver.Width / 2) - 5, textPos.Y - 48);
            DrawValuesInventoryItem(player.KilledEnemies, valuesPos, 5);
            #endregion

            #region drawing_count_coins
            Image formCountCoin = Properties.Resources.inventory_coins_form;
            Point coinFormLocate = new Point(imgBackgroundGameOverPos.X + 900, imgBackgroundGameOverPos.Y + 319);
            graph.DrawImage(formCountCoin, coinFormLocate);
            DrawValuesInventoryForCoins(player.CoinCount, coinFormLocate, 42);
            #endregion

            #region drawing_status_player
            Point statusPos = new Point(imgBackgroundGameOverPos.X + 620, imgBackgroundGameOverPos.Y + 520);
            if(player.CoinCount >= 0 && player.CoinCount <= 5)
            {
                PaintTextEverywhere("Status: bummer", statusPos, imgBackgroundGameOver.Width);
            } else if(player.CoinCount > 5 && player.CoinCount <= 10)
            {
                PaintTextEverywhere("Status: Garbage hero", statusPos, imgBackgroundGameOver.Width);
            } else if(player.CoinCount > 10 && player.CoinCount <= 15)
            {
                PaintTextEverywhere("Status: Poor man", statusPos, imgBackgroundGameOver.Width);
            } else if(player.CoinCount > 15 && player.CoinCount <= 20)
            {
                PaintTextEverywhere("Status: Definitely not bad person", statusPos, imgBackgroundGameOver.Width);
            }
            #endregion

            #region drawing_button_return_to_main_menu
            imgMainMenuBtn = Properties.Resources.main_menu_word;
            imgMainMenuBtnPos = new Point(imgBackgroundGameOverPos.X + (imgBackgroundGameOver.Width - 210), imgBackgroundGameOverPos.Y + 450);
            graph.DrawImage(imgMainMenuBtn, imgMainMenuBtnPos);
            #endregion

            anyBtnsClicked_GameOverMenu = false; // перед созданием формы и отрисовки окна проигрыша указываем, что все кнопки еще не были нажаты на этой форме
        }

        public void PaintMenuWinner()
        {
            mainMenuThemeSound.PlayLooping();
            ResetMap();
            PaintMap();
            #region drawing_form_gameover
            imgBackgroundWinnerPos = new Point(this.Width / 8, this.Height / 4);
            graph.DrawImage(imgBackgroundWinner, imgBackgroundWinnerPos);
            #endregion

            #region drawing_name_player_profile
            Point namePos = new Point(imgBackgroundWinnerPos.X + 200, imgBackgroundWinnerPos.Y + 260);
            PaintTextEverywhere("Player: " + player.PlayerName.ToString(), namePos, imgBackgroundWinner.Width);
            #endregion

            #region drawing_count_enemies_hero_killed
            Point textPos = new Point(imgBackgroundWinnerPos.X + 200, imgBackgroundWinnerPos.Y + 340);
            PaintTextEverywhere("Count enemies whos you killed", textPos, imgBackgroundWinner.Width);
            Point valuesPos = new Point(textPos.X + (imgBackgroundWinner.Width / 2) - 5, textPos.Y - 48);
            DrawValuesInventoryItem(player.KilledEnemies, valuesPos, 5);
            #endregion

            #region drawing_count_coins
            Image formCountCoin = Properties.Resources.inventory_coins_form;
            Point coinFormLocate = new Point(imgBackgroundWinnerPos.X + 900, imgBackgroundWinnerPos.Y + 319);
            graph.DrawImage(formCountCoin, coinFormLocate);
            DrawValuesInventoryForCoins(player.CoinCount, coinFormLocate, 42);
            #endregion

            #region drawing_status_player
            Point statusPos = new Point(imgBackgroundWinnerPos.X + 620, imgBackgroundWinnerPos.Y + 520);
            if (player.CoinCount >= 0 && player.CoinCount <= 5)
            {
                PaintTextEverywhere("Status: bummer", statusPos, imgBackgroundWinner.Width);
            }
            else if (player.CoinCount > 5 && player.CoinCount <= 10)
            {
                PaintTextEverywhere("Status: Garbage hero", statusPos, imgBackgroundWinner.Width);
            }
            else if (player.CoinCount > 10 && player.CoinCount <= 15)
            {
                PaintTextEverywhere("Status: Poor man", statusPos, imgBackgroundWinner.Width);
            }
            else if (player.CoinCount > 15 && player.CoinCount <= 20)
            {
                PaintTextEverywhere("Status: Definitely not bad person", statusPos, imgBackgroundWinner.Width);
            }
            #endregion

            #region drawing_button_return_to_main_menu
            imgMainMenuBtn = Properties.Resources.main_menu_word;
            imgMainMenuBtnPos = new Point(imgBackgroundWinnerPos.X + (imgBackgroundWinner.Width - 210), imgBackgroundWinnerPos.Y + 450);
            graph.DrawImage(imgMainMenuBtn, imgMainMenuBtnPos);
            #endregion

            anyBtnsClicked_WinnerMenu = false; // перед созданием формы и отрисовки окна проигрыша указываем, что все кнопки еще не были нажаты на этой форме
        }

        public void ArrowDamagedEnemy()
        {
            #region стрела попадает в противника
            switch(pDirectionArrowBuff)
            {
                case HeroDirection.is_down:
                    {
                        arrowPositionBuff = new Point(arrowAttackPos.X, arrowAttackPos.Y + 1);
                        break;
                    }
                case HeroDirection.is_up:
                    {
                        arrowPositionBuff = new Point(arrowAttackPos.X, arrowAttackPos.Y - 1);
                        break;
                    }
                case HeroDirection.is_left:
                    {
                        arrowPositionBuff = new Point(arrowAttackPos.X - 1, arrowAttackPos.Y);
                        break;
                    }
                case HeroDirection.is_right:
                    {
                        arrowPositionBuff = new Point(arrowAttackPos.X + 1, arrowAttackPos.Y);
                        break;
                    }
            }
            foreach (Enemy enemyElement in enemyCollection)
            {
                if (enemyElement.UnitStatusReady == EnemyGetMapped.is_ok)
                {
                    if (enemyElement.EnemyLocation == arrowPositionBuff && enemyElement.HitPoints != EnemyHealth.HP0)
                    {
                        if (enemyElement.HitPoints - 1 != EnemyHealth.HP0)
                        {
                            soundAttackSwordEnemy.Play();
                            enemyElement.Damaged = true; // отрисовка анимации противника при попадании в него стрелы
                            enemyElement.HitPoints--;
                            soundAttackSwordEnemy.Position = TimeSpan.MinValue;
                        }
                        else
                        {
                            enemyElement.HitPoints = EnemyHealth.HP0;
                            player.KilledEnemies++;
                            currentlyEnemiesDied++;
                            if (currentlyEnemiesDied == countEnemies)
                            {
                                allEnemiesDied = true;
                                launchCountdownTimer = true;
                                currentlyEnemiesDied = 0;
                            }
                            enemyElement.DyingByArrow = true;
                        }
                    }
                }
            }
            #endregion
        }

        public void Menu_MouseMove()
        {
            #region pause_menu
            if(pauseMenuActivated)
            {
                // событие наведения и нажатия кнопки "главное меню"
                if (imgMainMenuBtn != null && Cursor.Position.X >= imgMainMenuBtnPos.X && Cursor.Position.Y >= imgMainMenuBtnPos.Y && Cursor.Position.X <= (imgMainMenuBtnPos.X + imgMainMenuBtn.Width) && Cursor.Position.Y <= (imgMainMenuBtnPos.Y + imgMainMenuBtn.Height))
                {
                    imgMainMenuBtn = Properties.Resources.main_menu_word_active;
                    graph.DrawImage(imgMainMenuBtn, imgMainMenuBtnPos);
                    if(Form1.MouseButtons == MouseButtons.Left)
                    {
                        btnClickSound.Play();
                        mainMenuThemeSound.PlayLooping();
                        GameplayActivated = false;
                        pauseMenuActivated = false;
                        anyBtnsClicked_PauseMenu = true;
                        anyBtnsClicked = false;
                        anyBtnsClicked_newGameMenu = false;
                        mainMenuActivated = true;
                        ResetMap(); // обнуляем массив при переходе в основное меню
                        PaintMenu();
                        btnClickSound.Position = TimeSpan.MinValue;
                    }
                } else
                {
                    if(imgMainMenuBtn != null && anyBtnsClicked_PauseMenu == false)
                    {
                        imgMainMenuBtn = Properties.Resources.main_menu_word;
                        graph.DrawImage(imgMainMenuBtn, imgMainMenuBtnPos);
                    }
                }
            }
            #endregion

            #region menu_winner
            if (winnerMenuActivated)
            {
                // событие наведения и нажатия кнопки "главное меню"
                if (imgMainMenuBtn != null && Cursor.Position.X >= imgMainMenuBtnPos.X && Cursor.Position.Y >= imgMainMenuBtnPos.Y && Cursor.Position.X <= (imgMainMenuBtnPos.X + imgMainMenuBtn.Width) && Cursor.Position.Y <= (imgMainMenuBtnPos.Y + imgMainMenuBtn.Height))
                {
                    imgMainMenuBtn = Properties.Resources.main_menu_word_active;
                    graph.DrawImage(imgMainMenuBtn, imgMainMenuBtnPos);
                    if (Form1.MouseButtons == MouseButtons.Left)
                    {
                        btnClickSound.Play();
                        anyBtnsClicked_WinnerMenu = true;
                        anyBtnsClicked = false;
                        winnerMenuActivated = false;
                        mainMenuActivated = true;
                        PaintMenu();
                        anyBtnsClicked_newGameMenu = false;
                        btnClickSound.Position = TimeSpan.MinValue;
                    }
                }
                else
                {
                    if (imgMainMenuBtn != null && anyBtnsClicked_WinnerMenu == false)
                    {
                        imgMainMenuBtn = Properties.Resources.main_menu_word;
                        graph.DrawImage(imgMainMenuBtn, imgMainMenuBtnPos);
                    }
                }
            }
            #endregion

            #region menu_game_over
            if (gameoverMenuActivated)
            {
                // событие наведения и нажатия кнопки "главное меню"
                if(imgMainMenuBtn != null && Cursor.Position.X >= imgMainMenuBtnPos.X && Cursor.Position.Y >= imgMainMenuBtnPos.Y && Cursor.Position.X <= (imgMainMenuBtnPos.X + imgMainMenuBtn.Width) && Cursor.Position.Y <= (imgMainMenuBtnPos.Y + imgMainMenuBtn.Height))
                {
                    imgMainMenuBtn = Properties.Resources.main_menu_word_active;
                    graph.DrawImage(imgMainMenuBtn, imgMainMenuBtnPos);
                    if(Form1.MouseButtons == MouseButtons.Left)
                    {
                        btnClickSound.Play();
                        anyBtnsClicked_GameOverMenu = true;
                        anyBtnsClicked = false;
                        gameoverMenuActivated = false;
                        mainMenuActivated = true;
                        PaintMenu();
                        anyBtnsClicked_newGameMenu = false;
                        btnClickSound.Position = TimeSpan.MinValue;
                    }
                } else
                {
                    if(imgMainMenuBtn != null && anyBtnsClicked_GameOverMenu == false)
                    {
                        imgMainMenuBtn = Properties.Resources.main_menu_word;
                        graph.DrawImage(imgMainMenuBtn, imgMainMenuBtnPos);
                    }
                }
            }
            #endregion

            #region author_menu_activated
            if(authorGameMenuActivated)
            {
                // событие наведение и нажатие на кнопку "Назад"
                if (authorBackBtn != null && Cursor.Position.X >= authorBackBtnPos.X && Cursor.Position.Y >= authorBackBtnPos.Y && Cursor.Position.X <= (authorBackBtnPos.X + authorBackBtn.Width) && Cursor.Position.Y <= (authorBackBtnPos.Y + authorBackBtn.Height))
                {
                    authorBackBtn = Properties.Resources.back_word_active;
                    graph.DrawImage(authorBackBtn, authorBackBtnPos);
                    if (Form1.MouseButtons == MouseButtons.Left)
                    {
                        btnClickSound.Play();
                        anyBtnsClicked_author_menu = true;
                        anyBtnsClicked = false;
                        authorGameMenuActivated = false;
                        mainMenuActivated = true;
                        PaintMenu();
                        btnClickSound.Position = TimeSpan.MinValue;
                    }
                }
                else
                {
                    if (authorBackBtn != null && anyBtnsClicked_author_menu == false)
                    {
                        authorBackBtn = Properties.Resources.back_word;
                        graph.DrawImage(authorBackBtn, authorBackBtnPos);
                    }
                }
            }
            #endregion

            #region main_menu_activated
            if (mainMenuActivated)
            {
                // событие наведение и нажатие на кнопку "новая игра"
                if (newGameBtn != null && Cursor.Position.X >= newGameBtnPos.X && Cursor.Position.Y >= newGameBtnPos.Y && Cursor.Position.X <= (newGameBtnPos.X + newGameBtn.Width) && Cursor.Position.Y <= (newGameBtnPos.Y + newGameBtn.Height))
                {
                    newGameBtn = Properties.Resources.new_game_word_active;
                    graph.DrawImage(newGameBtn, newGameBtnPos);
                    if (Form1.MouseButtons == MouseButtons.Left)
                    {
                        //MessageBox.Show("NEW GAME");
                        btnClickSound.Play();
                        mainMenuActivated = false; // отключаем событие на кнопках главного меню
                        PaintMenuNewGame(); // отрисовываем меню новой игры
                        newGameMenuActivated = true; // активируем "обработчик" события наведения и нажатия на кнопки меню новой игры
                        anyBtnsClicked = true; // какой-то из пунктов меню был активирован
                        btnClickSound.Position = TimeSpan.MinValue;
                    }
                }
                else
                {
                    if (newGameBtn != null && anyBtnsClicked == false)
                    {
                        newGameBtn = Properties.Resources.new_game_word;
                        graph.DrawImage(newGameBtn, newGameBtnPos);
                    }
                }

                // событие наведение и нажатие на кнопку "Загрузить игру"
                if (loadGameBtn != null && Cursor.Position.X >= loadGameBtnPos.X && Cursor.Position.Y >= loadGameBtnPos.Y && Cursor.Position.X <= (loadGameBtnPos.X + loadGameBtn.Width) && Cursor.Position.Y <= (loadGameBtnPos.Y + loadGameBtn.Height))
                {
                    loadGameBtn = Properties.Resources.load_game_word_active;
                    graph.DrawImage(loadGameBtn, loadGameBtnPos);
                    if (Form1.MouseButtons == MouseButtons.Left)
                    {
                        btnClickSound.Play();
                        MessageBox.Show("Load GAME");
                        btnClickSound.Position = TimeSpan.MinValue;
                    }
                }
                else
                {
                    if (loadGameBtn != null && anyBtnsClicked == false)
                    {
                        loadGameBtn = Properties.Resources.load_game_word;
                        graph.DrawImage(loadGameBtn, loadGameBtnPos);
                    }
                }

                // событие наведения и нажатия на кнопку "Автор"
                if (authorGameBtn != null && Cursor.Position.X >= authorGameBtnPos.X && Cursor.Position.Y >= authorGameBtnPos.Y && Cursor.Position.X <= (authorGameBtnPos.X + authorGameBtn.Width) && Cursor.Position.Y <= (authorGameBtnPos.Y + authorGameBtn.Height))
                {
                    authorGameBtn = Properties.Resources.author_word_active;
                    graph.DrawImage(authorGameBtn, authorGameBtnPos);
                    if (Form1.MouseButtons == MouseButtons.Left)
                    {
                        btnClickSound.Play();
                        anyBtnsClicked = true;
                        anyBtnsClicked_author_menu = false;
                        mainMenuActivated = false;
                        authorGameMenuActivated = true;
                        PaintMenuAuthor();
                        btnClickSound.Position = TimeSpan.MinValue;
                    }
                } else
                {
                    if (authorGameBtn != null && anyBtnsClicked == false)
                    {
                        authorGameBtn = Properties.Resources.author_word;
                        graph.DrawImage(authorGameBtn, authorGameBtnPos);
                    }
                }

                // событие наведение и нажатие на кнопку "Выход"
                if (exitGameBtn != null && Cursor.Position.X >= exitGameBtnPos.X && Cursor.Position.Y >= exitGameBtnPos.Y && Cursor.Position.X <= (exitGameBtnPos.X + exitGameBtn.Width) && Cursor.Position.Y <= (exitGameBtnPos.Y + exitGameBtn.Height))
                {
                    exitGameBtn = Properties.Resources.exit_word_active;
                    graph.DrawImage(exitGameBtn, exitGameBtnPos);
                    if (Form1.MouseButtons == MouseButtons.Left)
                    {
                        btnClickSound.Play();
                        this.Close();
                        btnClickSound.Position = TimeSpan.MinValue;
                    }
                }
                else
                {
                    if (exitGameBtn != null && anyBtnsClicked == false)
                    {
                        exitGameBtn = Properties.Resources.exit_word;
                        graph.DrawImage(exitGameBtn, exitGameBtnPos);
                    }
                }
            }
            #endregion

            #region new_game_menu_activated
            if (newGameMenuActivated)
            {
                // событие наведение и нажатие на кнопку "Старт"
                if (startBtn != null && Cursor.Position.X >= startBtnPos.X && Cursor.Position.Y >= startBtnPos.Y && Cursor.Position.X <= (startBtnPos.X + startBtn.Width) && Cursor.Position.Y <= (startBtnPos.Y + startBtn.Height))
                {
                    startBtn = Properties.Resources.start_word_active;
                    graph.DrawImage(startBtn, startBtnPos);
                    if (Form1.MouseButtons == MouseButtons.Left)
                    {
                        if (textBoxProfileName.Text != "")
                        {
                            player.PlayerName = textBoxProfileName.Text; // присваиваем полю класса Hero playerName текст из компонента textbox
                            this.Controls.Remove(textBoxProfileName); // удаляем компонент с формы
                            textBoxProfileName.Dispose(); // очищаем ресурсы памяти после компонента
                            timer_moving.Enabled = true; // запускаем общий игровой таймер
                            newGameMenuActivated = false; // отключаем событие на кнопках меню новой игры
                            Initial_Map(); // отрисовка основной карты
                            anyBtnsClicked_newGameMenu = true; // какой-то из пунктов меню новой игры был активирован
                            PaintMessage("AUTHOR:\"Use the arrow keys to control the hero...\" >"); // отрисовать сообщение с подсказкой
                            msgGameControls = true; // стрелки-подсказки
                            GameplayActivated = true;
                            gameThemeSound.PlayLooping(); // воспроизводим циклично фоновую музыку для геймплея
                        } else
                        {
                            graph.DrawImage(attantion_msg, new Point((newGameMenu.Width / 2) + 440, newGameMenu.Height + 220));
                        }
                    }
                }
                else
                {
                    if (startBtn != null && anyBtnsClicked_newGameMenu == false)
                    {
                        startBtn = Properties.Resources.start_word;
                        graph.DrawImage(startBtn, startBtnPos);
                    }
                }

                // событие наведение и нажатие на кнопку "Назад"
                if (backBtn != null && Cursor.Position.X >= backBtnPos.X && Cursor.Position.Y >= backBtnPos.Y && Cursor.Position.X <= (backBtnPos.X + backBtn.Width) && Cursor.Position.Y <= (backBtnPos.Y + backBtn.Height))
                {
                    backBtn = Properties.Resources.back_word_active;
                    graph.DrawImage(backBtn, backBtnPos);
                    if (Form1.MouseButtons == MouseButtons.Left)
                    {
                        this.Controls.Remove(textBoxProfileName); // удаляем компонент с формы
                        textBoxProfileName.Dispose(); // очищаем ресурсы памяти после компонента
                        newGameMenuActivated = false; // отключаем событие на кнопках меню новой игры
                        PaintMenu(); // отрисовываем главное меню
                        mainMenuActivated = true; // активируем "обработчик" события наведения и нажатия на кнопки главного меню
                        anyBtnsClicked_newGameMenu = false; // какой-то из пунктов меню был активирован
                        anyBtnsClicked = false; // очищаем статус нажатых кнопок на главном меню
                    }
                }
                else
                {
                    if (backBtn != null && anyBtnsClicked_newGameMenu == false)
                    {
                        backBtn = Properties.Resources.back_word;
                        graph.DrawImage(backBtn, backBtnPos);
                    }
                }
            }
            #endregion

            #region Gameplay_Activated
            if (GameplayActivated && building_mode == false && room == false && messageActivated == false && inventoryActivated == false && pauseMenuActivated == false)
            {
                if (player.PlayerHealth == 0)
                {
                    timer_moving.Enabled = false;
                    if (timeOutEndGame > 0)
                    {
                        timeOutEndGame--;
                    }
                    else
                    {
                        timeOutEndGame = 50;
                        GameplayActivated = false;
                        //timer_moving.Enabled = false;
                        gameoverMenuActivated = true;
                        PaintMenuGameOver();
                        //....
                    }
                }
                
                if (player.PlayerHealth != 0)
                {
                    HeroDyingByNOFOOD(); // герой умирает от голода

                    // анимация выращивания деревьев
                    TreeGrowingByWater();

                    ShovelUseAnimation(); // анимация использования лопаты относитльно пенька

                    // Shield was activated
                    if (player.ShieldActivated)
                    {
                        if (player.EnergyPoints > 0)
                        {
                            player.EnergyPoints--;
                        }
                        else
                        {
                            player.ShieldActivated = false;
                        }
                    }

                    // создание предмета карты
                    if (countdownItemSpawnMap >= 0)
                    {
                        countdownItemSpawnMap--;
                    }
                    else
                    {
                        ItemsGenerator.SetItemPositionOnMap(ref map);
                        countdownItemSpawnMap = 200;
                    }

                    ItemsGenerator.ItemAnimation(this.CreateGraphics(), imgListItems, map, arraySize_N, arraySize_M); // анимация предметов карты

                    HeroAttackAnimation(); // анимация атаки героя
                    if (enemyCollection != null)
                    {
                        foreach (Enemy enemyElement in enemyCollection)
                        {
                            if (enemyElement.UnitStatusReady == EnemyGetMapped.is_ok)
                            {
                                if (enemyElement.DyingByArrow) // анимация убийства противника стрелой
                                {
                                    enemyElement.EnemyDyingByArrow(this.CreateGraphics(), imgListEnemy, imageListGame, ref map);
                                }
                                enemyElement.EnemyDyingByWater(this.CreateGraphics(), imgListEnemy, imageListGame, ref map); // анимация врага тонущего в воде

                                enemyElement.TakeDamageByHero(this.CreateGraphics(), imgListEnemy); // анимация удара по противнику (условие внутри)
                                enemyElement.EnemyAttackAnimation(this.CreateGraphics(), imageListGame, imgListEnemyAttack, map, player.PlayerPos, ref playerStateForEnemyAttackBuff); // анимация удара врагом по герою
                                enemyElement.EnemyStrongAttackAnimation(player.PlayerPos, this.CreateGraphics(), imgListEnemyStrongAttack, imageListGame, ref playerStateForEnemyAttackBuff, ref map); // анимация сильного импульсного удара врагом по герою
                            }
                            enemyElement.ItemDroped(this.CreateGraphics(), imgListItems, ref map); // анимация выпавшего предмета из противника
                        }
                    }
                    if (playerStateForEnemyAttackBuff != playerStatus.edle && player.ShieldActivated == false) // условие, если буфер статуса игрока отличен от статуса "в покое" и герой без щита, то присвоим новый статус герою
                    {
                        player.PState = playerStateForEnemyAttackBuff; // передаем значение из буфера члену класса "герой" со статусом игрока
                        playerStateForEnemyAttackBuff = playerStatus.edle; // очищаем статус буфера (чтобы не зациклить выполнение анимации удара по герою)
                    }
                    else
                    {
                        playerStateForEnemyAttackBuff = playerStatus.edle;
                    }
                    // проверяем статус уже конкретно героя, если анимация еще не выполнена до конца и удар не завершен, то запрещаем игроку передвигаться
                    player.HeroLightDamageAnimation(this.CreateGraphics(), imgHeroUnderDamage, ref arrow_down_t, ref arrow_up_t, ref arrow_left_t, ref arrow_right_t, ref freeze_actions); // анимация ближней ататки по герою
                    player.HeroStrongDamageAnimation(this.CreateGraphics(), imgHeroUnderDamage, ref arrow_down_t, ref arrow_up_t, ref arrow_left_t, ref arrow_right_t, ref freeze_actions); // анимация дальней атаки по герою
                    Paint_GUIHitPoints(); // отрисовка индикатора здоровья
                    Paint_GUICoins(); // отрисовка индикатора монет

                    #region Отрисовка полета стрелы выпущенной героем
                    if (arrowIsFlying)
                    {
                        switch (pDirectionArrowBuff)
                        {
                            case HeroDirection.is_down:
                                {
                                    if (map[arrowAttackPos.X, arrowAttackPos.Y + 1] == 0 || map[arrowAttackPos.X, arrowAttackPos.Y + 1] == 94)
                                    {
                                        map[arrowAttackPos.X, arrowAttackPos.Y] = 0;
                                        imageListGame.Draw(this.CreateGraphics(), arrowAttackPos.X * picSizeWidth, arrowAttackPos.Y * picSizeHeight, map[arrowAttackPos.X, arrowAttackPos.Y]);
                                        arrowAttackPos = new Point(arrowAttackPos.X, arrowAttackPos.Y + 1);
                                        map[arrowAttackPos.X, arrowAttackPos.Y] = 153;
                                        imageListGame.Draw(this.CreateGraphics(), arrowAttackPos.X * picSizeWidth, arrowAttackPos.Y * picSizeHeight, map[arrowAttackPos.X, arrowAttackPos.Y]);
                                    }
                                    else
                                    {
                                        arrowIsFlying = false; // стрела наткнулась на ячейку содержащую отличное значение от нуля (0 - трава), "выключаем" отрисовку полета
                                        arrowAttackDestroyed = true; // начинаем отрисовку анимации разрушения стрелы об предмет
                                        ArrowDamagedEnemy(); // регистрируем попадание стрелы в противника
                                    }
                                    break;
                                }
                            case HeroDirection.is_up:
                                {
                                    if (map[arrowAttackPos.X, arrowAttackPos.Y - 1] == 0 || map[arrowAttackPos.X, arrowAttackPos.Y - 1] == 94)
                                    {
                                        map[arrowAttackPos.X, arrowAttackPos.Y] = 0;
                                        imageListGame.Draw(this.CreateGraphics(), arrowAttackPos.X * picSizeWidth, arrowAttackPos.Y * picSizeHeight, map[arrowAttackPos.X, arrowAttackPos.Y]);
                                        arrowAttackPos = new Point(arrowAttackPos.X, arrowAttackPos.Y - 1);
                                        map[arrowAttackPos.X, arrowAttackPos.Y] = 154;
                                        imageListGame.Draw(this.CreateGraphics(), arrowAttackPos.X * picSizeWidth, arrowAttackPos.Y * picSizeHeight, map[arrowAttackPos.X, arrowAttackPos.Y]);
                                    }
                                    else
                                    {
                                        arrowIsFlying = false; // стрела наткнулась на ячейку содержащую отличное значение от нуля (0 - трава), "выключаем" отрисовку полета
                                        arrowAttackDestroyed = true; // начинаем отрисовку анимации разрушения стрелы об предмет
                                        ArrowDamagedEnemy(); // регистрируем попадание стрелы в противника
                                    }
                                    break;
                                }
                            case HeroDirection.is_left:
                                {
                                    if (map[arrowAttackPos.X - 1, arrowAttackPos.Y] == 0 || map[arrowAttackPos.X - 1, arrowAttackPos.Y] == 94)
                                    {
                                        map[arrowAttackPos.X, arrowAttackPos.Y] = 0;
                                        imageListGame.Draw(this.CreateGraphics(), arrowAttackPos.X * picSizeWidth, arrowAttackPos.Y * picSizeHeight, map[arrowAttackPos.X, arrowAttackPos.Y]);
                                        arrowAttackPos = new Point(arrowAttackPos.X - 1, arrowAttackPos.Y);
                                        map[arrowAttackPos.X, arrowAttackPos.Y] = 155;
                                        imageListGame.Draw(this.CreateGraphics(), arrowAttackPos.X * picSizeWidth, arrowAttackPos.Y * picSizeHeight, map[arrowAttackPos.X, arrowAttackPos.Y]);
                                    }
                                    else
                                    {
                                        arrowIsFlying = false; // стрела наткнулась на ячейку содержащую отличное значение от нуля (0 - трава), "выключаем" отрисовку полета
                                        arrowAttackDestroyed = true; // начинаем отрисовку анимации разрушения стрелы об предмет
                                        ArrowDamagedEnemy(); // регистрируем попадание стрелы в противника
                                    }
                                    break;
                                }
                            case HeroDirection.is_right:
                                {
                                    if (map[arrowAttackPos.X + 1, arrowAttackPos.Y] == 0 || map[arrowAttackPos.X + 1, arrowAttackPos.Y] == 94)
                                    {
                                        map[arrowAttackPos.X, arrowAttackPos.Y] = 0;
                                        imageListGame.Draw(this.CreateGraphics(), arrowAttackPos.X * picSizeWidth, arrowAttackPos.Y * picSizeHeight, map[arrowAttackPos.X, arrowAttackPos.Y]);
                                        arrowAttackPos = new Point(arrowAttackPos.X + 1, arrowAttackPos.Y);
                                        map[arrowAttackPos.X, arrowAttackPos.Y] = 156;
                                        imageListGame.Draw(this.CreateGraphics(), arrowAttackPos.X * picSizeWidth, arrowAttackPos.Y * picSizeHeight, map[arrowAttackPos.X, arrowAttackPos.Y]);
                                    }
                                    else
                                    {
                                        arrowIsFlying = false; // стрела наткнулась на ячейку содержащую отличное значение от нуля (0 - трава), "выключаем" отрисовку полета
                                        arrowAttackDestroyed = true; // начинаем отрисовку анимации разрушения стрелы об предмет
                                        ArrowDamagedEnemy(); // регистрируем попадание стрелы в противника
                                    }
                                    break;
                                }
                        }
                    }
                    // Разрушение стрелы об предмет
                    if (arrowAttackDestroyed)
                    {
                        if (arrow_attack_destroying_step <= 12)
                        {
                            imgListBow.Draw(this.CreateGraphics(), arrowAttackPos.X * picSizeWidth, arrowAttackPos.Y * picSizeHeight, arrow_attack_destroying_step);
                            arrow_attack_destroying_step++;
                        }
                        else
                        {
                            arrow_attack_destroying_step = 9;
                            map[arrowAttackPos.X, arrowAttackPos.Y] = 0;
                            imageListGame.Draw(this.CreateGraphics(), arrowAttackPos.X * picSizeWidth, arrowAttackPos.Y * picSizeHeight, map[arrowAttackPos.X, arrowAttackPos.Y]);
                            arrowAttackDestroyed = false;
                        }
                    }
                    #endregion

                    HeroBowAttackAnimation(); // анимация атаки из лука, вызываем после анимации полета стрелы, чтобы пропстить шаг таймера и отрисовать стрелу рядом с героем

                    // анимация сруба дерева
                    if (treesStump)
                    {
                        if (stumping_step >= 49)
                        {
                            imageListGame.Draw(this.CreateGraphics(), treeStumpState.X * picSizeWidth, treeStumpState.Y * picSizeHeight, stumping_step);
                            stumping_step--;
                        }
                        else
                        {
                            stumping_step = 65;
                            treesStump = false;
                            imageListGame.Draw(this.CreateGraphics(), treeStumpState.X * picSizeWidth, treeStumpState.Y * picSizeHeight, (byte)healthStatus.HP0_stump);
                        }
                    }
                    //----------------------------------------


                    //----Отрисовка полета камня кинутого персонажем--------------------------
                    if (stoneIsFLying)
                    {
                        switch (pDirectionBuff)
                        {
                            case HeroDirection.is_down:
                                {
                                    if (map[stoneAttackPos.X, stoneAttackPos.Y + 1] == 0 || map[stoneAttackPos.X, stoneAttackPos.Y + 1] == 94)
                                    {
                                        map[stoneAttackPos.X, stoneAttackPos.Y] = 0;
                                        imageListGame.Draw(this.CreateGraphics(), stoneAttackPos.X * picSizeWidth, stoneAttackPos.Y * picSizeHeight, map[stoneAttackPos.X, stoneAttackPos.Y]);
                                        stoneAttackPos = new Point(stoneAttackPos.X, stoneAttackPos.Y + 1);
                                        map[stoneAttackPos.X, stoneAttackPos.Y] = 142;
                                        imageListGame.Draw(this.CreateGraphics(), stoneAttackPos.X * picSizeWidth, stoneAttackPos.Y * picSizeHeight, map[stoneAttackPos.X, stoneAttackPos.Y]);
                                    }
                                    else
                                    {
                                        stoneAttackDestroyed = true;
                                        stoneIsFLying = false;
                                    }
                                    break;
                                }
                            case HeroDirection.is_up:
                                {
                                    if (map[stoneAttackPos.X, stoneAttackPos.Y - 1] == 0 || map[stoneAttackPos.X, stoneAttackPos.Y - 1] == 94)
                                    {
                                        map[stoneAttackPos.X, stoneAttackPos.Y] = 0;
                                        imageListGame.Draw(this.CreateGraphics(), stoneAttackPos.X * picSizeWidth, stoneAttackPos.Y * picSizeHeight, map[stoneAttackPos.X, stoneAttackPos.Y]);
                                        stoneAttackPos = new Point(stoneAttackPos.X, stoneAttackPos.Y - 1);
                                        map[stoneAttackPos.X, stoneAttackPos.Y] = 142;
                                        imageListGame.Draw(this.CreateGraphics(), stoneAttackPos.X * picSizeWidth, stoneAttackPos.Y * picSizeHeight, map[stoneAttackPos.X, stoneAttackPos.Y]);
                                    }
                                    else
                                    {
                                        stoneAttackDestroyed = true;
                                        stoneIsFLying = false;
                                    }
                                    break;
                                }
                            case HeroDirection.is_left:
                                {
                                    if (map[stoneAttackPos.X - 1, stoneAttackPos.Y] == 0 || map[stoneAttackPos.X - 1, stoneAttackPos.Y] == 94)
                                    {
                                        map[stoneAttackPos.X, stoneAttackPos.Y] = 0;
                                        imageListGame.Draw(this.CreateGraphics(), stoneAttackPos.X * picSizeWidth, stoneAttackPos.Y * picSizeHeight, map[stoneAttackPos.X, stoneAttackPos.Y]);
                                        stoneAttackPos = new Point(stoneAttackPos.X - 1, stoneAttackPos.Y);
                                        map[stoneAttackPos.X, stoneAttackPos.Y] = 142;
                                        imageListGame.Draw(this.CreateGraphics(), stoneAttackPos.X * picSizeWidth, stoneAttackPos.Y * picSizeHeight, map[stoneAttackPos.X, stoneAttackPos.Y]);
                                    }
                                    else
                                    {
                                        stoneAttackDestroyed = true;
                                        stoneIsFLying = false;
                                    }
                                    break;
                                }
                            case HeroDirection.is_right:
                                {
                                    if (map[stoneAttackPos.X + 1, stoneAttackPos.Y] == 0 || map[stoneAttackPos.X + 1, stoneAttackPos.Y] == 94)
                                    {
                                        map[stoneAttackPos.X, stoneAttackPos.Y] = 0;
                                        imageListGame.Draw(this.CreateGraphics(), stoneAttackPos.X * picSizeWidth, stoneAttackPos.Y * picSizeHeight, map[stoneAttackPos.X, stoneAttackPos.Y]);
                                        stoneAttackPos = new Point(stoneAttackPos.X + 1, stoneAttackPos.Y);
                                        map[stoneAttackPos.X, stoneAttackPos.Y] = 142;
                                        imageListGame.Draw(this.CreateGraphics(), stoneAttackPos.X * picSizeWidth, stoneAttackPos.Y * picSizeHeight, map[stoneAttackPos.X, stoneAttackPos.Y]);
                                    }
                                    else
                                    {
                                        stoneAttackDestroyed = true;
                                        stoneIsFLying = false;
                                    }
                                    break;
                                }
                        }
                    }
                    if (stoneAttackDestroyed)
                    {
                        soundDestroyStone.Play();
                        if (stone_attack_destroyed_step <= 22)
                        {
                            imgListStone.Draw(this.CreateGraphics(), stoneAttackPos.X * picSizeWidth, stoneAttackPos.Y * picSizeHeight, stone_attack_destroyed_step);
                            stone_attack_destroyed_step++;
                        }
                        else
                        {
                            soundDestroyStone.Position = TimeSpan.MinValue;
                            stone_attack_destroyed_step = 19;
                            map[stoneAttackPos.X, stoneAttackPos.Y] = 0;
                            imageListGame.Draw(this.CreateGraphics(), stoneAttackPos.X * picSizeWidth, stoneAttackPos.Y * picSizeHeight, map[stoneAttackPos.X, stoneAttackPos.Y]);
                            stoneAttackDestroyed = false;
                        }
                    }
                    //------------------------------------------------------------------------

                    //----Отрисовка элементов интерфейса для выбора инструмента персонажем----
                    if (myInterface.ItemsPosition != null)
                    {
                        int inter_w = 17;
                        int inter_h = 12; // т.к. по умолчанию в методе стоит +44 по высоте, то отнимаем 52-20 = 32, => 44-32=12
                        string Key = myInterface.GetItemsKey(player.ItemSword, player.ItemBow, player.ShovelCount, player.WoodCount);
                        if (Key != "")
                        {
                            for (int i = 0; i < Key.Length; i++)
                            {
                                switch (Key[i].ToString())
                                {
                                    case "s":
                                        {
                                            if (Cursor.Position.X >= myInterface.ItemsPosition[i].X && Cursor.Position.Y >= myInterface.ItemsPosition[i].Y && Cursor.Position.X <= (myInterface.ItemsPosition[i].X + 52) && Cursor.Position.Y <= (myInterface.ItemsPosition[i].Y + 52))
                                            {
                                                if (Form1.MouseButtons == MouseButtons.Left)
                                                {
                                                    myInterface.CurrentItem = itemTag.tag_sword;
                                                }
                                            }
                                            else
                                            {
                                                imgListInterface.Draw(this.CreateGraphics(), myInterface.ItemsPosition[i], 0);
                                            }
                                            break;
                                        }
                                    case "h":
                                        {
                                            if (Cursor.Position.X >= myInterface.ItemsPosition[i].X && Cursor.Position.Y >= myInterface.ItemsPosition[i].Y && Cursor.Position.X <= (myInterface.ItemsPosition[i].X + 52) && Cursor.Position.Y <= (myInterface.ItemsPosition[i].Y + 52))
                                            {
                                                if (Form1.MouseButtons == MouseButtons.Left)
                                                {
                                                    myInterface.CurrentItem = itemTag.tag_bow;
                                                }
                                            }
                                            else
                                            {
                                                imgListInterface.Draw(this.CreateGraphics(), myInterface.ItemsPosition[i], 1);
                                            }
                                            break;
                                        }
                                    case "p":
                                        {
                                            if (Cursor.Position.X >= myInterface.ItemsPosition[i].X && Cursor.Position.Y >= myInterface.ItemsPosition[i].Y && Cursor.Position.X <= (myInterface.ItemsPosition[i].X + 52) && Cursor.Position.Y <= (myInterface.ItemsPosition[i].Y + 52))
                                            {
                                                if (Form1.MouseButtons == MouseButtons.Left)
                                                {
                                                    myInterface.CurrentItem = itemTag.tag_shovel;
                                                }
                                            }
                                            else
                                            {
                                                imgListInterface.Draw(this.CreateGraphics(), myInterface.ItemsPosition[i], 2);
                                                DrawValuesInventoryItem(player.ShovelCount, myInterface.ItemsPosition[i], inter_w, inter_h);
                                            }
                                            break;
                                        }
                                    case "a":
                                        {
                                            imgListInterface.Draw(this.CreateGraphics(), myInterface.ItemsPosition[i], 15);
                                            DrawValuesInventoryItem(player.WoodCount, myInterface.ItemsPosition[i], inter_w, inter_h);
                                            break;
                                        }
                                }
                            }
                        }
                    }

                    if (myInterface.ItemsPosition != null)
                    {
                        int inter_w = 17;
                        int inter_h = 12; // т.к. по умолчанию в методе стоит +44 по высоте, то отнимаем 52-20 = 32, => 44-32=12
                        string Key = myInterface.GetItemsKey(player.ItemSword, player.ItemBow, player.ShovelCount, player.WoodCount);
                        if (Key != "")
                        {
                            for (int i = 0; i < Key.Length; i++)
                            {
                                switch (Key[i].ToString())
                                {
                                    case "s":
                                        {
                                            if (myInterface.CurrentItem == itemTag.tag_sword)
                                            {
                                                if (interfaceSwordFocusStep <= 6)
                                                {
                                                    imgListInterface.Draw(this.CreateGraphics(), myInterface.ItemsPosition[i], interfaceSwordFocusStep);
                                                    interfaceSwordFocusStep++;
                                                }
                                                else
                                                {
                                                    interfaceSwordFocusStep = 3;
                                                    imgListInterface.Draw(this.CreateGraphics(), myInterface.ItemsPosition[i], interfaceSwordFocusStep);
                                                }
                                            }
                                            break;
                                        }
                                    case "h":
                                        {
                                            if (myInterface.CurrentItem == itemTag.tag_bow)
                                            {
                                                if (interfaceBowFocusStep <= 10)
                                                {
                                                    imgListInterface.Draw(this.CreateGraphics(), myInterface.ItemsPosition[i], interfaceBowFocusStep);
                                                    interfaceBowFocusStep++;
                                                }
                                                else
                                                {
                                                    interfaceBowFocusStep = 7;
                                                    imgListInterface.Draw(this.CreateGraphics(), myInterface.ItemsPosition[i], interfaceBowFocusStep);
                                                }
                                            }
                                            break;
                                        }
                                    case "p":
                                        {
                                            if (myInterface.CurrentItem == itemTag.tag_shovel)
                                            {
                                                if (interfaceShovelFocusStep <= 14)
                                                {
                                                    imgListInterface.Draw(this.CreateGraphics(), myInterface.ItemsPosition[i], interfaceShovelFocusStep);
                                                    DrawValuesInventoryItem(player.ShovelCount, myInterface.ItemsPosition[i], inter_w, inter_h);
                                                    interfaceShovelFocusStep++;
                                                }
                                                else
                                                {
                                                    interfaceShovelFocusStep = 11;
                                                    imgListInterface.Draw(this.CreateGraphics(), myInterface.ItemsPosition[i], interfaceShovelFocusStep);
                                                    DrawValuesInventoryItem(player.ShovelCount, myInterface.ItemsPosition[i], inter_w, inter_h);
                                                }
                                            }
                                            break;
                                        }
                                }
                            }
                        }
                    }
                    //------------------------------------------------------------------------

                    //-------Отрисовка сообщения о возможном взаимодействии с предметом-------
                    switch (playerDirection)
                    {
                        case HeroDirection.is_down:
                            {
                                pStatusBuff = playerStatus.get_water_down;
                                if (map[player.PlayerPos.X, player.PlayerPos.Y + 1] == 79)
                                {
                                    nearRiver = true;
                                    PaintTextInteraction("Press -E- to fill a bottle with water");
                                }
                                else
                                {
                                    nearRiver = false;
                                    ClearTextInteraction();
                                }
                                break;
                            }
                        case HeroDirection.is_up:
                            {
                                pStatusBuff = playerStatus.get_water_up;
                                if (map[player.PlayerPos.X, player.PlayerPos.Y - 1] == 79)
                                {
                                    nearRiver = true;
                                    PaintTextInteraction("Press -E- to fill a bottle with water");
                                }
                                else
                                {
                                    nearRiver = false;
                                    ClearTextInteraction();
                                }
                                break;
                            }
                        case HeroDirection.is_left:
                            {
                                pStatusBuff = playerStatus.get_water_left;
                                if (map[player.PlayerPos.X - 1, player.PlayerPos.Y] == 79)
                                {
                                    nearRiver = true;
                                    PaintTextInteraction("Press -E- to fill a bottle with water");
                                }
                                else
                                {
                                    nearRiver = false;
                                    ClearTextInteraction();
                                }
                                break;
                            }
                        case HeroDirection.is_right:
                            {
                                pStatusBuff = playerStatus.get_water_right;
                                if (map[player.PlayerPos.X + 1, player.PlayerPos.Y] == 79)
                                {
                                    nearRiver = true;
                                    PaintTextInteraction("Press -E- to fill a bottle with water");
                                }
                                else
                                {
                                    nearRiver = false;
                                    ClearTextInteraction();
                                }
                                break;
                            }
                    }
                    // Проверка находится ли герой перед входом в дом
                    if (playerDirection == HeroDirection.is_up)
                    {
                        if (map[player.PlayerPos.X, player.PlayerPos.Y - 1] == 144)
                        {
                            nearRoom = true;
                            PaintTextInteraction("Press -E- to enter the house");
                        }
                        else
                        {
                            nearRoom = false;
                            //ClearTextInteraction();
                        }
                    }
                    else
                    {
                        nearRoom = false;
                    }
                    // Проверка находится ли герой перед деревом
                    switch (playerDirection)
                    {
                        case HeroDirection.is_down:
                            {
                                if (map[player.PlayerPos.X, player.PlayerPos.Y + 1] == 1)
                                {
                                    possibleStumpPositionForWatering = new Point(player.PlayerPos.X, player.PlayerPos.Y + 1);
                                    nearTree = true;
                                    PaintTextInteraction("Press -E- to use bottle with water");
                                }
                                else
                                {
                                    nearTree = false;
                                }
                                break;
                            }
                        case HeroDirection.is_up:
                            {
                                if (map[player.PlayerPos.X, player.PlayerPos.Y - 1] == 1)
                                {
                                    possibleStumpPositionForWatering = new Point(player.PlayerPos.X, player.PlayerPos.Y - 1);
                                    nearTree = true;
                                    PaintTextInteraction("Press -E- to use bottle with water");
                                }
                                else
                                {
                                    nearTree = false;
                                }
                                break;
                            }
                        case HeroDirection.is_left:
                            {
                                if (map[player.PlayerPos.X - 1, player.PlayerPos.Y] == 1)
                                {
                                    possibleStumpPositionForWatering = new Point(player.PlayerPos.X - 1, player.PlayerPos.Y);
                                    nearTree = true;
                                    PaintTextInteraction("Press -E- to use bottle with water");
                                }
                                else
                                {
                                    nearTree = false;
                                }
                                break;
                            }
                        case HeroDirection.is_right:
                            {
                                if (map[player.PlayerPos.X + 1, player.PlayerPos.Y] == 1)
                                {
                                    possibleStumpPositionForWatering = new Point(player.PlayerPos.X + 1, player.PlayerPos.Y);
                                    nearTree = true;
                                    PaintTextInteraction("Press -E- to use bottle with water");
                                }
                                else
                                {
                                    nearTree = false;
                                }
                                break;
                            }
                    }
                    //------------------------------------------------------------------------

                    //-------------------Отрисовка индикации пищи на персонаже----------------
                    if (foodHeroActive)
                    {
                        int s = 0;
                        ValueToNumbers(player.FoodPoints);
                        while (valScore.Count != 0)
                        {
                            imgHeroValues.Draw(this.CreateGraphics(), (player.PlayerPos.X * picSizeWidth) + s, (player.PlayerPos.Y * picSizeHeight) + (picSizeHeight - 16), valScore.Pop());
                            s += 14;
                        }
                    }
                    //-------------------------------------------------------------------------

                    //-------------------Отрисовка индикации энергии на персонаже--------------
                    if (energyHeroActive)
                    {
                        int s = 0;
                        ValueToNumbers(player.EnergyPoints);
                        while (valScore.Count != 0)
                        {
                            imgHeroValues.Draw(this.CreateGraphics(), (player.PlayerPos.X * picSizeWidth) + s, (player.PlayerPos.Y * picSizeHeight) + (picSizeHeight - 16), valScore.Pop() + 10);
                            s += 14;
                        }
                    }
                    //-------------------------------------------------------------------------

                    // вычисление процента пищи при наведении мышки на элемент
                    if (Cursor.Position.X >= FoodPointsPos.X && Cursor.Position.Y >= FoodPointsPos.Y && Cursor.Position.X <= FoodPointsFormSize.Width && Cursor.Position.Y <= FoodPointsFormSize.Height)
                    {
                        //----------------------Индикация зачений пищи на персонаже----------------------------
                        if (Form1.MouseButtons == MouseButtons.Left)
                        {
                            if (foodHeroActive == false) foodHeroActive = true; else foodHeroActive = false;
                            if (foodHeroActive == true) energyHeroActive = false;
                        }
                        //-------------------------------------------------------------------------------------
                        Paint_GUIFoodsBar();
                        int x, b = 0;
                        x = (player.FoodPoints * 100) / gameFoodPoints; // находим процент от общего количества очков пищи
                        ValueToNumbers(x); // разбиваем на цифры
                        while (valScore.Count != 0) // считываем посимвольно число из стека и отрисовываем
                        {
                            imgListValues.Draw(this.CreateGraphics(), (FoodPointsPos.X + (112 / 5)) + b, FoodPointsPos.Y + 5, valScore.Pop());
                            b += 14;
                        }
                        imgListValues.Draw(this.CreateGraphics(), (FoodPointsPos.X + (112 / 5)) + b, FoodPointsPos.Y + 5, 13);
                    }
                    else
                    {
                        Paint_GUIFoodPoints();
                    }
                    // вычисление процента энергии при наведении мышки на элемент
                    if (Cursor.Position.X >= EnergyPointsPos.X && Cursor.Position.Y >= EnergyPointsPos.Y && Cursor.Position.X <= EnergyPointsFormSize.Width && Cursor.Position.Y <= EnergyPointsFormSize.Height)
                    {
                        //----------------------Индикация значений энергии на персонаже------------------------
                        if (Form1.MouseButtons == MouseButtons.Left)
                        {
                            if (energyHeroActive == false) energyHeroActive = true; else energyHeroActive = false;
                            if (energyHeroActive == true) foodHeroActive = false;
                        }
                        //-------------------------------------------------------------------------------------

                        Paint_GUIEnergyBar();
                        int y, a = 0;
                        y = (player.EnergyPoints * 100) / gameEnergyPoints; // находим процент от общего количества очков энергии
                        ValueToNumbers(y); // разбиваем на цифры
                        while (valScore.Count != 0)
                        {
                            imgListValues.Draw(this.CreateGraphics(), (EnergyPointsPos.X + (320 / 3)) + a, EnergyPointsPos.Y + 5, valScore.Pop());
                            a += 14;
                        }
                        imgListValues.Draw(this.CreateGraphics(), (EnergyPointsPos.X + (320 / 3)) + a, EnergyPointsPos.Y + 5, 13); // drawing '%'
                    }
                    else
                    {
                        Paint_GUIEnergyPoints();
                    }
                    // вычисление процента очков уровня при наведении мышки на элемент
                    if (Cursor.Position.X >= LevelPointsPos.X && Cursor.Position.Y >= LevelPointsPos.Y && Cursor.Position.X <= LevelPointsFormSize.Width && Cursor.Position.Y <= LevelPointsFormSize.Height)
                    {
                        Paint_GUILevelBar();
                        int x, y = 0;
                        x = (player.PlayerExperience * 100) / gameLevelPoints; // находим процент от общего количества очков уровня
                        ValueToNumbers(x); // разбиваем на цифры
                        while (valScore.Count != 0)
                        {
                            imgListValues.Draw(this.CreateGraphics(), (LevelPointsPos.X + (320 / 3)) + y, LevelPointsPos.Y + 5, valScore.Pop());
                            y += 14;
                        }
                        imgListValues.Draw(this.CreateGraphics(), (LevelPointsPos.X + (320 / 3)) + y, LevelPointsPos.Y + 5, 13); // drawing '%'
                    }
                    else
                    {
                        Paint_GUILevelPoints();
                    }
                }

                if (player.KilledEnemies + 1 == 21)
                {
                    if (timeOutEndGame > 0)
                    {
                        timeOutEndGame--;
                    }
                    else
                    {
                        timeOutEndGame = 50;
                        arrow_down_t = arrow_up_t = arrow_left_t = arrow_right_t = false;
                        freeze_actions = true;
                        GameplayActivated = false;
                        timer_moving.Enabled = false;
                        winnerMenuActivated = true;
                        PaintMenuWinner();
                        //....
                    }
                }
            }
            #endregion

            #region Message_Activated
            if (messageActivated)
            {
                timer_moving.Enabled = false;
                // кнопка "подобрать" предмет
                if (msgCollectBtn != null && Cursor.Position.X >= msgCollectBtnPos.X && Cursor.Position.Y >= msgCollectBtnPos.Y && Cursor.Position.X <= (msgCollectBtnPos.X + msgCollectBtn.Width) && Cursor.Position.Y <= (msgCollectBtnPos.Y + msgCollectBtn.Height))
                {
                    msgCollectBtn = Properties.Resources.collect_word_active;
                    graph.DrawImage(msgCollectBtn, msgCollectBtnPos);
                    if (Form1.MouseButtons == MouseButtons.Left)
                    {
                        btnClickSound.Play();
                        timer_moving.Enabled = true;
                        switch(typeMessageTrigger)
                        {
                            case "wood":
                                {
                                    player.WoodCount += woodPlayerBuff; // игрок подбирает древесину, заносим из временного хранилища количество древесины в общий счетчик дерева
                                    foreach (Wood woodElement in myWoods)
                                    {
                                        if (player.PlayerPos == woodElement.WoodLocation)
                                        {
                                            woodElement.ContainWood = 0; // обнуляем счетчик дерева в ячейке
                                            woodElement.CountState = countStatus.wood_empty; // здааем значение древесины пустое
                                        }
                                    }
                                    break;
                                }
                            case "bottle":
                                {
                                    player.EmptyBottles += bottlePlayerBuff; // игрок подбирает пустую бутылку, заносим из временного хранилища количество бутылок в общий счетчик бутылок
                                    foreach(Bottle bottleElement in myBottles)
                                    {
                                        if(player.PlayerPos == bottleElement.BottleLocation)
                                        {
                                            map[bottleElement.BottleLocation.X, bottleElement.BottleLocation.Y] = 0; // заменяем ячейку с рисунком бутылки на рисунок с травой, убирая таким образом бутылку с общей карты
                                            bottleElement.BottleState = bottleStatus.bottle_none; // присваиваем стату бутылке "не существует"
                                        }
                                    }
                                    break;
                                }
                            case "stone":
                                {
                                    player.StoneCount += stonePlayerBuff; // игрок подбирает камень, заносим из временного хранилища количество камней в общий счетчик
                                    foreach(Stone stoneElement in myStones)
                                    {
                                        if(stoneElement.StoneLocation == player.PlayerPos)
                                        {
                                            //map[stoneElement.StoneLocation.X, stoneElement.StoneLocation.Y] = 0; // заменяем ячейку с рисунком камнем на рисунок с травой, убирая таким образом камень с общей карты
                                            stoneElement.StoneState = stoneStatus.is_none; // присваиваем стату камня "не существует"
                                        }
                                    }
                                    break;
                                }
                        }
                        arrow_down_t = arrow_up_t = arrow_right_t = arrow_left_t = true; // активировать движения персонажа
                        freeze_actions = false; // активировать атаку героя
                        messageActivated = false; // завершаем выполнение оброботчика событий сообщения
                        anyBtnsClicked_message = true; // одна из кнопок сообщения нажата, следовательно отключаем выполнение следующих кнопок
                        Clear_Message(); // "очищаем" поле с формой сообщения, рефрешим текстуры
                        //MessageBox.Show(player.WoodCount.ToString());
                        btnClickSound.Position = TimeSpan.MinValue;
                    }

                } else if (msgCollectBtn != null && anyBtnsClicked_message == false)
                {
                    msgCollectBtn = Properties.Resources.collect_word;
                    graph.DrawImage(msgCollectBtn, msgCollectBtnPos);
                }
                // кнопка "назад" к игровому процессу
                if (msgBackBtn != null && Cursor.Position.X >= msgBackBtnPos.X && Cursor.Position.Y >= msgBackBtnPos.Y && Cursor.Position.X <= (msgBackBtnPos.X + msgBackBtn.Width) && Cursor.Position.Y <= (msgBackBtnPos.Y + msgBackBtn.Height))
                {
                    msgBackBtn = Properties.Resources.back_word_active;
                    graph.DrawImage(msgBackBtn, msgBackBtnPos);
                    // событие нажатия кнопки "назад" в сообщении
                    if (Form1.MouseButtons == MouseButtons.Left)
                    {
                        btnClickSound.Play();
                        timer_moving.Enabled = true;
                        woodPlayerBuff = 0; // обнуляем временное хранилище количества древесины на которой стоит персонаж
                        arrow_down_t = arrow_up_t = arrow_left_t = arrow_right_t = true; // активировать движения персонажа
                        freeze_actions = false; // активировать атаку героя
                        messageActivated = false; // завершаем выполнение оброботчика событий сообщения
                        anyBtnsClicked_message = true; // одна из кнопок сообщения нажата, следовательно отключаем выполнение следующих кнопок
                        Clear_Message(); // "очищаем" поле с формой сообщения, рефрешим текстуры
                        btnClickSound.Position = TimeSpan.MinValue;
                    }
                } else if (msgBackBtn != null && anyBtnsClicked_message == false)
                {
                    msgBackBtn = Properties.Resources.back_word;
                    graph.DrawImage(msgBackBtn, msgBackBtnPos);
                }
            }
            #endregion
            #region Inventory_Activated
            if(inventoryActivated)
            {
                timer_moving.Enabled = false;
                // отрисовка анимации фокуса на предметах при наведении указателя мыши
                bool mouseMoveItem = false;
                if(myBag.ItemsPos != null)
                {
                    for(int i = 0; i < myBag.Key.Length; i++)
                    {
                        switch(myBag.Key[i].ToString())
                        {
                            case "s":
                                {
                                    if(Cursor.Position.X >= myBag.ItemsPos[i].X && Cursor.Position.Y >= myBag.ItemsPos[i].Y && Cursor.Position.X <= (myBag.ItemsPos[i].X + 64) && Cursor.Position.Y <= (myBag.ItemsPos[i].Y + 64))
                                    {
                                        mouseMoveItem = true;
                                        if(swordFocusingStep <= 10)
                                        {
                                            imgListInventory.Draw(this.CreateGraphics(), myBag.ItemsPos[i], swordFocusingStep);
                                            swordFocusingStep++;
                                        } else
                                        {
                                            swordFocusingStep = 7;
                                            imgListInventory.Draw(this.CreateGraphics(), myBag.ItemsPos[i], swordFocusingStep);
                                        }
                                        PaintTextInventory("Short sword. More suitable for cutting trees and cutting salad.");
                                    } else
                                    {
                                        imgListInventory.Draw(this.CreateGraphics(), myBag.ItemsPos[i], 1);
                                        mouseMoveItem = false;
                                        //graph.DrawImage(imgInventoryText, imgInventoryTextPos);
                                    }
                                    break;
                                }
                            case "h":
                                {
                                    if(Cursor.Position.X >= myBag.ItemsPos[i].X && Cursor.Position.Y >= myBag.ItemsPos[i].Y && Cursor.Position.X <= (myBag.ItemsPos[i].X + 64) && Cursor.Position.Y <= (myBag.ItemsPos[i].Y + 64))
                                    {
                                        mouseMoveItem = true;
                                        if(bowFocusingStep <= 14)
                                        {
                                            imgListInventory.Draw(this.CreateGraphics(), myBag.ItemsPos[i], bowFocusingStep);
                                            bowFocusingStep++;
                                        } else
                                        {
                                            bowFocusingStep = 11;
                                            imgListInventory.Draw(this.CreateGraphics(), myBag.ItemsPos[i], bowFocusingStep);
                                        }
                                        PaintTextInventory("This is a bow. Extremely useful thing. Able to protect against almost any trouble.");
                                    } else
                                    {
                                        imgListInventory.Draw(this.CreateGraphics(), myBag.ItemsPos[i], 2);
                                    }
                                    break;
                                }
                            case "p":
                                {
                                    if(Cursor.Position.X >= myBag.ItemsPos[i].X && Cursor.Position.Y >= myBag.ItemsPos[i].Y && Cursor.Position.X <= (myBag.ItemsPos[i].X + 64) && Cursor.Position.Y <= (myBag.ItemsPos[i].Y + 64))
                                    {
                                        mouseMoveItem = true;
                                        if(shovelFocusingStep <= 35)
                                        {
                                            imgListInventory.Draw(this.CreateGraphics(), myBag.ItemsPos[i], shovelFocusingStep);
                                            DrawValuesInventoryItem(player.ShovelCount, myBag.ItemsPos[i]);
                                            shovelFocusingStep++;
                                        } else
                                        {
                                            shovelFocusingStep = 32;
                                            imgListInventory.Draw(this.CreateGraphics(), myBag.ItemsPos[i], shovelFocusingStep);
                                            DrawValuesInventoryItem(player.ShovelCount, myBag.ItemsPos[i]);
                                        }
                                        PaintTextInventory("Shovel. Use it to clear your way big time.");
                                    } else
                                    {
                                        imgListInventory.Draw(this.CreateGraphics(), myBag.ItemsPos[i], 31);
                                        DrawValuesInventoryItem(player.ShovelCount, myBag.ItemsPos[i]);
                                    }
                                    break;
                                }
                            case "w":
                                {
                                    if(Cursor.Position.X >= myBag.ItemsPos[i].X && Cursor.Position.Y >= myBag.ItemsPos[i].Y && Cursor.Position.X <= (myBag.ItemsPos[i].X + 64) && Cursor.Position.Y <= (myBag.ItemsPos[i].Y + 64))
                                    {
                                        mouseMoveItem = true;
                                        if(woodFocusingStep <= 18)
                                        {
                                            imgListInventory.Draw(this.CreateGraphics(), myBag.ItemsPos[i], woodFocusingStep);
                                            DrawValuesInventoryItem(player.WoodCount, myBag.ItemsPos[i]);
                                            woodFocusingStep++;
                                        } else
                                        {
                                            woodFocusingStep = 15;
                                            imgListInventory.Draw(this.CreateGraphics(), myBag.ItemsPos[i], woodFocusingStep);
                                            DrawValuesInventoryItem(player.WoodCount, myBag.ItemsPos[i]);
                                        }
                                        PaintTextInventory("Wood. Using a wood you can build some useful structures that will save your life and help you get other items.");
                                    } else
                                    {
                                        imgListInventory.Draw(this.CreateGraphics(), myBag.ItemsPos[i], 3);
                                        DrawValuesInventoryItem(player.WoodCount, myBag.ItemsPos[i]);
                                    }
                                    break;
                                }
                            case "r":
                                {
                                    if(Cursor.Position.X >= myBag.ItemsPos[i].X && Cursor.Position.Y >= myBag.ItemsPos[i].Y && Cursor.Position.X <= (myBag.ItemsPos[i].X + 64) && Cursor.Position.Y <= (myBag.ItemsPos[i].Y + 64))
                                    {
                                        mouseMoveItem = true;
                                        if(stoneFocusingStep <= 22)
                                        {
                                            imgListInventory.Draw(this.CreateGraphics(), myBag.ItemsPos[i], stoneFocusingStep);
                                            DrawValuesInventoryItem(player.StoneCount, myBag.ItemsPos[i]);
                                            stoneFocusingStep++;
                                        } else
                                        {
                                            stoneFocusingStep = 19;
                                            imgListInventory.Draw(this.CreateGraphics(), myBag.ItemsPos[i], stoneFocusingStep);
                                            DrawValuesInventoryItem(player.StoneCount, myBag.ItemsPos[i]);
                                        }
                                        PaintTextInventory("Stone. With the help of stone you can defend yourself or use it in the construction of something.");
                                    } else
                                    {
                                        imgListInventory.Draw(this.CreateGraphics(), myBag.ItemsPos[i], 4);
                                        DrawValuesInventoryItem(player.StoneCount, myBag.ItemsPos[i]);
                                    }
                                    break;
                                }
                            case "e":
                                {
                                    if(Cursor.Position.X >= myBag.ItemsPos[i].X && Cursor.Position.Y >= myBag.ItemsPos[i].Y && Cursor.Position.X <= (myBag.ItemsPos[i].X + 64) && Cursor.Position.Y <= (myBag.ItemsPos[i].Y + 64))
                                    {
                                        mouseMoveItem = true;
                                        if(emptyBottleFocusingStep <= 26)
                                        {
                                            imgListInventory.Draw(this.CreateGraphics(), myBag.ItemsPos[i], emptyBottleFocusingStep);
                                            DrawValuesInventoryItem(player.EmptyBottles, myBag.ItemsPos[i]);
                                            emptyBottleFocusingStep++;
                                        } else
                                        {
                                            emptyBottleFocusingStep = 23;
                                            imgListInventory.Draw(this.CreateGraphics(), myBag.ItemsPos[i], emptyBottleFocusingStep);
                                            DrawValuesInventoryItem(player.EmptyBottles, myBag.ItemsPos[i]);
                                        }
                                        PaintTextInventory("Just empty bottle.");
                                    } else
                                    {
                                        imgListInventory.Draw(this.CreateGraphics(), myBag.ItemsPos[i], 5);
                                        DrawValuesInventoryItem(player.EmptyBottles, myBag.ItemsPos[i]);
                                    }
                                    break;
                                }
                            case "b":
                                {
                                    if (Cursor.Position.X >= myBag.ItemsPos[i].X && Cursor.Position.Y >= myBag.ItemsPos[i].Y && Cursor.Position.X <= (myBag.ItemsPos[i].X + 64) && Cursor.Position.Y <= (myBag.ItemsPos[i].Y + 64))
                                    {
                                        mouseMoveItem = true;
                                        if(waterBottleFocusingStep <= 30)
                                        {
                                            imgListInventory.Draw(this.CreateGraphics(), myBag.ItemsPos[i], waterBottleFocusingStep);
                                            DrawValuesInventoryItem(player.WaterBottles, myBag.ItemsPos[i]);
                                            waterBottleFocusingStep++;
                                        } else
                                        {
                                            waterBottleFocusingStep = 27;
                                            imgListInventory.Draw(this.CreateGraphics(), myBag.ItemsPos[i], waterBottleFocusingStep);
                                            DrawValuesInventoryItem(player.WaterBottles, myBag.ItemsPos[i]);
                                        }
                                        PaintTextInventory("Bottle with water. You can use this to restore your food points or do something else.");
                                    } else
                                    {
                                        imgListInventory.Draw(this.CreateGraphics(), myBag.ItemsPos[i], 6);
                                        DrawValuesInventoryItem(player.WaterBottles, myBag.ItemsPos[i]);
                                    }
                                    break;
                                }
                        }
                    }
                }
                if (mouseMoveItem == false) graph.DrawImage(imgInventoryText, imgInventoryTextPos);
                // кнопка "построить дом" инвентаря
                if (inventoryBuildHouseBtn != null && Cursor.Position.X >= inventoryBuildHouseBtnPos.X && Cursor.Position.Y >= inventoryBuildHouseBtnPos.Y && Cursor.Position.X <= (inventoryBuildHouseBtnPos.X + inventoryBuildHouseBtn.Width) && Cursor.Position.Y <= (inventoryBuildHouseBtnPos.Y + inventoryBuildHouseBtn.Height))
                {
                    inventoryBuildHouseBtn = Properties.Resources.inventoryBuildHouse_active;
                    graph.DrawImage(inventoryBuildHouseBtn, inventoryBuildHouseBtnPos);
                    // событие нажатия кнопки
                    if (Form1.MouseButtons == MouseButtons.Left)
                    {
                        btnClickSound.Play();
                        building_mode = true; // входим в режим постройки
                        inventoryActivated = false; // завершаем выполнение оброботчика событий инвентаря
                        anyBtnsClicked_inventory = true; // одна из кнопок инвентаря нажата, следовательно отключаем выполнение следующих кнопок
                        ClearInventory(); //"очищаем" поле с формой инвентаря, рефрешим текстуры
                        btnClickSound.Position = TimeSpan.MinValue;
                    }
                } else if (inventoryBuildHouseBtn != null && anyBtnsClicked_inventory == false)
                {
                    inventoryBuildHouseBtn = Properties.Resources.inventoryBuildHouse;
                    graph.DrawImage(inventoryBuildHouseBtn, inventoryBuildHouseBtnPos);
                }
                // кнопка "назад" вернуться к игровому процессу
                if (inventoryBackBtn != null && Cursor.Position.X >= inventoryBackBtnPos.X && Cursor.Position.Y >= inventoryBackBtnPos.Y && Cursor.Position.X <= (inventoryBackBtnPos.X + inventoryBackBtn.Width) && Cursor.Position.Y <= (inventoryBackBtnPos.Y + inventoryBackBtn.Height))
                {
                    inventoryBackBtn = Properties.Resources.back_word_active;
                    graph.DrawImage(inventoryBackBtn, inventoryBackBtnPos);
                    // событие нажатия кнопки "назад" в сообщении
                    if(Form1.MouseButtons == MouseButtons.Left)
                    {
                        btnClickSound.Play();
                        timer_moving.Enabled = true;
                        myBag.Key = ""; // очищаем ключ инвентаря
                        arrow_down_t = arrow_up_t = arrow_left_t = arrow_right_t = true; // активировать движения персонажа
                        freeze_actions = false; // активировать атаку героя
                        inventoryActivated = false; // завершаем выполнение оброботчика событий инвентаря
                        anyBtnsClicked_inventory = true; // одна из кнопок инвентаря нажата, следовательно отключаем выполнение следующих кнопок
                        ClearInventory(); //"очищаем" поле с формой инвентаря, рефрешим текстуры
                        btnClickSound.Position = TimeSpan.MinValue;
                    }
                }
                else if(inventoryBackBtn != null && anyBtnsClicked_inventory == false)
                {
                    inventoryBackBtn = Properties.Resources.back_word;
                    graph.DrawImage(inventoryBackBtn, inventoryBackBtnPos);
                }
            }
            #endregion
            #region Building_Mode
            if (building_mode)
            {
                timer_moving.Enabled = false;
                for(int i = 0; i < arraySize_N; i++)
                    for(int j = 0; j < arraySize_M; j++)
                    {
                        if(Cursor.Position.X >= (i * picSizeWidth) && Cursor.Position.Y >= (j * picSizeHeight) && Cursor.Position.X <= ((i * picSizeWidth) + picSizeWidth) && Cursor.Position.Y <= ((j * picSizeHeight) + picSizeHeight))
                        {
                            BuildingHouseSearching(i, j);
                            BuildingHouseSearching(i, j - 1);
                            if(Form1.MouseButtons == MouseButtons.Left)
                            {
                                if ((map[i, j] == 0 && map[i, j - 1] == 0) || (map[i, j] == 94 && map[i, j - 1] == 94) || (map[i, j] == 94 && map[i, j - 1] == 0) || (map[i, j] == 0 && map[i, j - 1] == 94))
                                {
                                    soundBuilding.Play();
                                    BuildingConstruction(i, j);
                                    building_mode = false;
                                    PaintAllMap();
                                    Paint_GUIHitPoints();
                                    myBag.Key = ""; // очищаем ключ инвентаря
                                    arrow_down_t = arrow_up_t = arrow_left_t = arrow_right_t = true; // активировать движения персонажа
                                    freeze_actions = false; // активировать атаку героя
                                    timer_moving.Enabled = true;
                                    soundBuilding.Position = TimeSpan.MinValue;
                                }
                            }
                        } else if(building_mode)
                        {
                            if(map[i, j] == 79)
                            {
                                imageListGame.Draw(this.CreateGraphics(), i * picSizeWidth, j * picSizeHeight, river_arr[i, j]);
                            } else
                            {
                                imageListGame.Draw(this.CreateGraphics(), i * picSizeWidth, j * picSizeHeight, map[i, j]);
                            }
                        }
                    }
            }
            #endregion
        }

        public void BuildingHouseSearching(int i, int j)
        {
            if(i >= 0 && i <= arraySize_N && j >= 0 && j <= arraySize_M)
            {
                switch (map[i, j])
                {
                    case 0:
                        {
                            imgForBuilding.Draw(this.CreateGraphics(), i * picSizeWidth, j * picSizeHeight, 13);
                            break;
                        }
                    case 94:
                        {
                            imgForBuilding.Draw(this.CreateGraphics(), i * picSizeWidth, j * picSizeHeight, 13);
                            break;
                        }
                    case 1:
                        {
                            imgForBuilding.Draw(this.CreateGraphics(), i * picSizeWidth, j * picSizeHeight, 15);
                            break;
                        }
                    case 95:
                        {
                            imgForBuilding.Draw(this.CreateGraphics(), i * picSizeWidth, j * picSizeHeight, 14);
                            break;
                        }
                    case 79:
                        {
                            imgForBuilding.Draw(this.CreateGraphics(), i * picSizeWidth, j * picSizeHeight, river_arr[i, j] - 66);
                            break;
                        }
                }
            }
        }

        public void BuildingConstruction(int i, int j)
        {
            if(i >= 0 && i <= arraySize_N && j >= 0 && j <= arraySize_M)
            {
                //if((map[i, j] == 0 && map[i, j - 1] == 0) || (map[i, j] == 94 && map[i, j - 1] == 94))
                //{
                    map[i, j - 1] = 143;
                    map[i, j] = 144;
                    imageListGame.Draw(this.CreateGraphics(), i * picSizeWidth, (j - 1) * picSizeHeight, 143);
                    imageListGame.Draw(this.CreateGraphics(), i * picSizeWidth, j * picSizeHeight, 144);
                //}
            }
        }

        public void PaintAllMap()
        {
            for (int i = 0; i < arraySize_N; i++)
                for (int j = 0; j < arraySize_M; j++)
                {
                    // отрисовываем элементы реки
                    if (map[i, j] == 79) imageListGame.Draw(this.CreateGraphics(), i * picSizeWidth, j * picSizeHeight, river_arr[i, j]);
                    else
                        imageListGame.Draw(this.CreateGraphics(), i * picSizeWidth, j * picSizeHeight, map[i, j]);
                }
            // отрисовываем "срубленные" деревья пеньками
            foreach (Tree treeElement in myTrees)
            {
                if (treeElement.HitPoints == healthStatus.HP0_stump && treeElement.ShovelUsed == ShovelStatus.is_not)
                    if (treeElement.WateringStumpState == WaterStatus.none) // проверка был ли пень полит водой из бутылки
                        imageListGame.Draw(this.CreateGraphics(), treeElement.TreeLocation.X * picSizeWidth, treeElement.TreeLocation.Y * picSizeHeight, (byte)treeElement.HitPoints);
                    else
                        imageListGame.Draw(this.CreateGraphics(), treeElement.TreeLocation.X * picSizeWidth, treeElement.TreeLocation.Y * picSizeHeight, (byte)treeElement.WateringStumpState);
            }
        }

        public void PaintMenuNewGame()
        {
            textBoxProfileName = new TextBox(); // создаем текстовое поле для ввода имени профиля
            textBoxProfileName.Left = (ClientSize.Width / 2) - 60; // задаем расположение
            textBoxProfileName.Top = (ClientSize.Height / 3) + 90;
            this.Controls.Add(textBoxProfileName); // добавляем текстовое поле на форму
            PaintMap(); // отрисовываем фон картой (на данный момент массив заполнен нулями и рисуем фон одной травой)
            #region drawing_new_game_form
            graph.DrawImage(newGameMenu, new Point(this.Width / 8, this.Height / 4));
            #endregion
            #region drawing_button_back
            backBtn = Properties.Resources.back_word;
            backBtnPos = new Point(newGameMenu.Width / 2 + 100, newGameMenu.Height + 320);
            graph.DrawImage(backBtn, backBtnPos);
            #endregion
            #region drawing_button_start
            startBtn = Properties.Resources.start_word;
            startBtnPos = new Point(newGameMenu.Width + 280, newGameMenu.Height + 320);
            graph.DrawImage(startBtn, startBtnPos);
            #endregion
        }

        public void GetKeyInventory()
        {
            if (player.ItemSword) myBag.Key += "s"; // код меча
            if (player.ItemBow) myBag.Key += "h"; // код лука
            if (player.ShovelCount > 0) myBag.Key += "p"; // код лопаты
            if (player.WoodCount > 0) myBag.Key += "w"; // код древесины
            if (player.StoneCount > 0) myBag.Key += "r"; // код камня
            if (player.EmptyBottles > 0) myBag.Key += "e"; // код пустой бутылки
            if (player.WaterBottles > 0) myBag.Key += "b"; // код бутылки с водой
        }

        public void DrawValuesInventoryItem(int value, Point itemPosition, int w = 0, int h = 0) // h-height and w-width for draw values on interface item
        {
            int b = 24;
            string str = value.ToString();
            imgListValues.Draw(this.CreateGraphics(), itemPosition.X + b - w, itemPosition.Y + 44 - h, 10); // draw 'x'
            b += 10;
            for (int k = 0; k < str.Length; k++)
            {
                imgListValues.Draw(this.CreateGraphics(), itemPosition.X + b - w, itemPosition.Y + 44 - h, int.Parse(str[k].ToString())); // draw values
                b += 15;
            }
        }

        public void DrawValuesInventoryForCoins(int value, Point coinItemPosition, int sizeValue)
        {
            string str = value.ToString();
            int b = sizeValue;
            for(int i = 0; i < str.Length; i++)
            {
                imgCoinValues.Draw(this.CreateGraphics(), coinItemPosition.X + b, coinItemPosition.Y + 10, int.Parse(str[i].ToString()));
                b += 9;
            }
        }

        public void PaintInventoryItems()
        {
            for(int i = 0; i < myBag.Key.Length; i++)
            {
                switch(myBag.Key[i].ToString())
                {
                    case "s":
                        imgListInventory.Draw(this.CreateGraphics(), myBag.ItemsPos[i], 1);
                        break;
                    case "h":
                        imgListInventory.Draw(this.CreateGraphics(), myBag.ItemsPos[i], 2);
                        break;
                    case "p":
                        imgListInventory.Draw(this.CreateGraphics(), myBag.ItemsPos[i], 31);
                        DrawValuesInventoryItem(player.ShovelCount, myBag.ItemsPos[i]);
                        break;
                    case "w":
                        {
                            imgListInventory.Draw(this.CreateGraphics(), myBag.ItemsPos[i], 3);
                            DrawValuesInventoryItem(player.WoodCount, myBag.ItemsPos[i]);
                            break;
                        }
                    case "r":
                        imgListInventory.Draw(this.CreateGraphics(), myBag.ItemsPos[i], 4);
                        break;
                    case "e":
                        {
                            imgListInventory.Draw(this.CreateGraphics(), myBag.ItemsPos[i], 5);
                            DrawValuesInventoryItem(player.EmptyBottles, myBag.ItemsPos[i]);
                            break;
                        }
                    case "b":
                        {
                            imgListInventory.Draw(this.CreateGraphics(), myBag.ItemsPos[i], 6);
                            DrawValuesInventoryItem(player.WaterBottles, myBag.ItemsPos[i]);
                            break;
                        }
                }
            }
        }

        public void PaintInventory()
        {
            #region drawing_inventory_form
            int x, y;
            if (player.PlayerPos.X * picSizeWidth < ClientSize.Width / 2)
            {
                x = ClientSize.Width - 600;
                y = 450;
                imgBGInventoryPos = new Point(x, y);
            }
            else if (player.PlayerPos.X * picSizeWidth > ClientSize.Width / 2)
            {
                x = 200;
                y = 450;
                imgBGInventoryPos = new Point(x, y);
            }
            graph.DrawImage(imgBackgroundInventory, imgBGInventoryPos);
            #endregion
            #region initial_position_background_text_inventory
            imgInventoryTextPos = new Point(imgBGInventoryPos.X + 20, imgBGInventoryPos.Y + 150);
            #endregion
            #region drawing_items
            myBag.SetItemsPosition(imgBGInventoryPos); // задаем координаты наших элементов инвентаря
            GetKeyInventory();
            if (myBag.Key == "")
            {
                foreach (Point itemBag in myBag.ItemsPos)
                {
                    imgListInventory.Draw(this.CreateGraphics(), itemBag, 0);
                }
            }
            else
            {
                foreach(Point itemBag in myBag.ItemsPos)
                {
                    imgListInventory.Draw(this.CreateGraphics(), itemBag, 0);
                }
                PaintInventoryItems();
            }
            #endregion
            #region drawing_button_build_house
            if (player.WoodCount >= 100)
            {
                inventoryBuildHouseBtn = Properties.Resources.inventoryBuildHouse;
                inventoryBuildHouseBtnPos = new Point(imgBGInventoryPos.X + 100, imgBGInventoryPos.Y + 360);
                graph.DrawImage(inventoryBuildHouseBtn, inventoryBuildHouseBtnPos);
            }
            #endregion
            #region drawing_image_coins
            inventoryCoinsCount = Properties.Resources.inventory_coins_form;
            inventoryCoinsCountPos = new Point(imgBGInventoryPos.X + 350, imgBGInventoryPos.Y + 360);
            graph.DrawImage(inventoryCoinsCount, inventoryCoinsCountPos);
            DrawValuesInventoryForCoins(player.CoinCount, inventoryCoinsCountPos, 42);
            #endregion
            #region drawing_button_back
            inventoryBackBtn = Properties.Resources.back_word;
            inventoryBackBtnPos = new Point(imgBGInventoryPos.X + 200, imgBGInventoryPos.Y + 460);
            graph.DrawImage(inventoryBackBtn, inventoryBackBtnPos);
            #endregion
        }

        public void ClearInventory()
        {
            for (int i = imgBGInventoryPos.X / picSizeWidth; i < arraySize_N; i++)
                for (int j = imgBGInventoryPos.Y / picSizeHeight; j < arraySize_M; j++)
                {
                    // отрисовываем элементы реки
                    if (map[i, j] == 79) imageListGame.Draw(this.CreateGraphics(), i * picSizeWidth, j * picSizeHeight, river_arr[i, j]);
                    else
                        imageListGame.Draw(this.CreateGraphics(), i * picSizeWidth, j * picSizeHeight, map[i, j]); // отрисовываем заново часть карты где располагалось окно инвентаря
                }
            // отрисовываем "срубленные" деревья пеньками
            foreach(Tree treeElement in myTrees)
            {
                if (treeElement.HitPoints == healthStatus.HP0_stump && treeElement.ShovelUsed == ShovelStatus.is_not)
                    if(treeElement.WateringStumpState == WaterStatus.none)
                    imageListGame.Draw(this.CreateGraphics(), treeElement.TreeLocation.X * picSizeWidth, treeElement.TreeLocation.Y * picSizeHeight, (byte)treeElement.HitPoints);
                else
                    imageListGame.Draw(this.CreateGraphics(), treeElement.TreeLocation.X * picSizeWidth, treeElement.TreeLocation.Y * picSizeHeight, (byte)treeElement.WateringStumpState);
            }
        }

        public Form1()
        {
            InitializeComponent();
        }

        public void DrawingItemMessage(string type)
        {
            switch (type)
            {
                case "wood":
                    {
                        #region drawing_message_wood
                        foreach (Wood woodElement in myWoods)
                        {
                            if (woodElement.WoodLocation == player.PlayerPos && woodElement.CountState != countStatus.wood_empty)
                            {
                                msgSound.Play(); // звук сообщения
                                anyBtnsClicked_message = false; // задаем значение ложь т.к. кнопки на форме сообщения еще не были нажаты
                                arrow_down_t = arrow_up_t = arrow_left_t = arrow_right_t = false; // ограничить движения персонажа
                                freeze_actions = true; // ограничить атаку персонажа на время сообщения
                                PaintMessage("Do you want to pick up an item? " + woodElement.ContainWood + "xWood"); // рисуем рамку и текст сообщения
                                PaintMessage_Buttons(); // рисуем кнопки для действия игрока
                                typeMessageTrigger = type;
                                messageActivated = true; // включаем обработку события на кнопки сообщения
                                woodPlayerBuff = woodElement.ContainWood; // заносим количество древесины на которые наступил персонаж
                                msgSound.Position = TimeSpan.MinValue; // задаем начальное значение для интервала аудиофайла, для следующего воспроизведения
                            }
                        }
                        #endregion
                        break;
                    }
                case "bottle":
                    {
                        #region draw_message_bottle
                        foreach(Bottle bottleElement in myBottles)
                        {
                            if(bottleElement.BottleLocation == player.PlayerPos && bottleElement.BottleState != bottleStatus.bottle_none)
                            {
                                msgSound.Play();
                                anyBtnsClicked_message = false; // задаем значение ложь т.к. кнопки на форме сообщения еще не были нажаты
                                arrow_down_t = arrow_up_t = arrow_left_t = arrow_right_t = false; // ограничить движения персонажа
                                freeze_actions = true; // ограничить атаку персонажа на время сообщения
                                PaintMessage("Do you want to pick up an item? " + bottleElement.ContainBottle + "xBottle"); // рисуем рамку и текст сообщения
                                PaintMessage_Buttons(); // рисуем кнопки для действия игрока
                                typeMessageTrigger = type; /* узнаем какого типа было сообщение, чтобы передать в функцию сбора предметов,
                                                              в зависимости от этого параметра мы узнаем какого типа предмет мы будем собирать игроку*/
                                messageActivated = true; // включаем обработку события на кнопки сообщения
                                bottlePlayerBuff = bottleElement.ContainBottle; // заносим количество бутылок на которые наступил персонаж в временную переменную
                                msgSound.Position = TimeSpan.MinValue;
                            }
                        }
                        #endregion
                        break;
                    }
                case "stone":
                    {
                        #region draw_message_stone
                        foreach(Stone stoneElement in myStones)
                        {
                            if(stoneElement.StoneLocation == player.PlayerPos && stoneElement.StoneState != stoneStatus.is_rock && stoneElement.StoneState != stoneStatus.is_none)
                            {
                                msgSound.Play();
                                anyBtnsClicked_message = false; // задаем значение ложь т.к. кнопки на форме сообщения еще не были нажаты
                                arrow_down_t = arrow_up_t = arrow_left_t = arrow_right_t = false; // ограничить движения персонажа
                                freeze_actions = true; // ограничить атаку персонажа на время сообщения
                                PaintMessage("Do you want to pick up an item? " + stoneElement.ContainStone + "xStone"); // рисуем рамку и текст сообщения
                                PaintMessage_Buttons(); // рисуем кнопки для действия игрока
                                typeMessageTrigger = type; /* узнаем какого типа было сообщение, чтобы передать в функцию сбора предметов,
                                                              в зависимости от этого параметра мы узнаем какого типа предмет мы будем собирать игроку*/
                                messageActivated = true; // включаем обработку события на кнопки сообщения
                                stonePlayerBuff = stoneElement.ContainStone; // заносим количество камней на которые наступил персонаж в временную переменную
                                msgSound.Position = TimeSpan.MinValue;
                            }
                        }
                        #endregion
                        break;
                    }
            }

        }

        public void PaintPlayerWithItemUp(itemTag currentItemPerson)
        {
            switch (currentItemPerson)
            {
                case itemTag.tag_none:
                case itemTag.tag_sword:
                    {
                        map[player.PlayerPos.X, player.PlayerPos.Y] = 3;
                        break;
                    }
                case itemTag.tag_shovel:
                    {
                        map[player.PlayerPos.X, player.PlayerPos.Y] = 139;
                        break;
                    }
                case itemTag.tag_bow:
                    {
                        map[player.PlayerPos.X, player.PlayerPos.Y] = 150;
                        break;
                    }
            }
        }

        public void PaintPlayerWithItemDown(itemTag currentItemPerson)
        {
            switch(currentItemPerson)
            {
                case itemTag.tag_none:
                case itemTag.tag_sword:
                    {
                        map[player.PlayerPos.X, player.PlayerPos.Y] = 2;
                        break;
                    }
                case itemTag.tag_shovel:
                    {
                        map[player.PlayerPos.X, player.PlayerPos.Y] = 138;
                        break;
                    }
                case itemTag.tag_bow:
                    {
                        map[player.PlayerPos.X, player.PlayerPos.Y] = 149;
                        break;
                    }
            }
        }

        public void PaintPlayerWithItemLeft(itemTag currentItemPerson)
        {
            switch(currentItemPerson)
            {
                case itemTag.tag_none:
                case itemTag.tag_sword:
                    {
                        map[player.PlayerPos.X, player.PlayerPos.Y] = 4;
                        break;
                    }
                case itemTag.tag_shovel:
                    {
                        map[player.PlayerPos.X, player.PlayerPos.Y] = 140;
                        break;
                    }
                case itemTag.tag_bow:
                    {
                        map[player.PlayerPos.X, player.PlayerPos.Y] = 151;
                        break;
                    }
            }
        }

        public void PaintPlayerWithItemRight(itemTag currentItemPerson)
        {
            switch(currentItemPerson)
            {
                case itemTag.tag_none:
                case itemTag.tag_sword:
                    {
                        map[player.PlayerPos.X, player.PlayerPos.Y] = 5;
                        break;
                    }
                case itemTag.tag_shovel:
                    {
                        map[player.PlayerPos.X, player.PlayerPos.Y] = 141;
                        break;
                    }
                case itemTag.tag_bow:
                    {
                        map[player.PlayerPos.X, player.PlayerPos.Y] = 152;
                        break;
                    }
            }
        }

        public void AttackStoneDirection()
        {
            switch(playerDirection)
            {
                case HeroDirection.is_down:
                    {
                        if(map[player.PlayerPos.X, player.PlayerPos.Y + 1] == 0 || map[player.PlayerPos.X, player.PlayerPos.Y + 1] == 94)
                        {
                            stoneAttackPos = new Point(player.PlayerPos.X, player.PlayerPos.Y + 1);
                            map[stoneAttackPos.X, stoneAttackPos.Y] = 142;
                            imageListGame.Draw(this.CreateGraphics(), stoneAttackPos.X * picSizeWidth, stoneAttackPos.Y * picSizeHeight, map[stoneAttackPos.X, stoneAttackPos.Y]);
                            stoneIsFLying = true;
                        }
                        break;
                    }
                case HeroDirection.is_up:
                    {
                        if(map[player.PlayerPos.X, player.PlayerPos.Y - 1] == 0 || map[player.PlayerPos.X, player.PlayerPos.Y - 1] == 94)
                        {
                            stoneAttackPos = new Point(player.PlayerPos.X, player.PlayerPos.Y - 1);
                            map[stoneAttackPos.X, stoneAttackPos.Y] = 142;
                            imageListGame.Draw(this.CreateGraphics(), stoneAttackPos.X * picSizeWidth, stoneAttackPos.Y * picSizeHeight, map[stoneAttackPos.X, stoneAttackPos.Y]);
                            stoneIsFLying = true;
                        }
                        break;
                    }
                case HeroDirection.is_left:
                    {
                        if(map[player.PlayerPos.X - 1, player.PlayerPos.Y] == 0 || map[player.PlayerPos.X - 1, player.PlayerPos.Y] == 94)
                        {
                            stoneAttackPos = new Point(player.PlayerPos.X - 1, player.PlayerPos.Y);
                            map[stoneAttackPos.X, stoneAttackPos.Y] = 142;
                            imageListGame.Draw(this.CreateGraphics(), stoneAttackPos.X * picSizeWidth, stoneAttackPos.Y * picSizeHeight, map[stoneAttackPos.X, stoneAttackPos.Y]);
                            stoneIsFLying = true;
                        }
                        break;
                    }
                case HeroDirection.is_right:
                    {
                        if(map[player.PlayerPos.X + 1, player.PlayerPos.Y] == 0 || map[player.PlayerPos.X + 1, player.PlayerPos.Y] == 94)
                        {
                            stoneAttackPos = new Point(player.PlayerPos.X + 1, player.PlayerPos.Y);
                            map[stoneAttackPos.X, stoneAttackPos.Y] = 142;
                            imageListGame.Draw(this.CreateGraphics(), stoneAttackPos.X * picSizeWidth, stoneAttackPos.Y * picSizeHeight, map[stoneAttackPos.X, stoneAttackPos.Y]);
                            stoneIsFLying = true;
                        }
                        break;
                    }
            }
        }

        public void AttackArrowDirection()
        {
            switch (playerDirection)
            {
                case HeroDirection.is_down:
                    {
                        player.PState = playerStatus.bow_attack_down;
                        break;
                    }
                case HeroDirection.is_up:
                    {
                        player.PState = playerStatus.bow_attack_up;
                        break;
                    }
                case HeroDirection.is_left:
                    {
                        player.PState = playerStatus.bow_attack_left;
                        break;
                    }
                case HeroDirection.is_right:
                    {
                        player.PState = playerStatus.bow_attack_right;
                        break;
                    }
            }
        }

        public void HeroDyingByNOFOOD()
        {
            if(playerDyingWithoutFood)
            {
                if(DyingNoFoodStep <= 12)
                {
                    imgHeroUnderDamage.Draw(this.CreateGraphics(), player.PlayerPos.X * picSizeWidth, player.PlayerPos.Y * picSizeHeight, DyingNoFoodStep);
                    DyingNoFoodStep++;
                } else
                {
                    DyingNoFoodStep = 9;
                    playerDyingWithoutFood = false;
                    player.PlayerHealth = 0;
                }
            }
        }

        public void UsingFoodPoints()
        {
            if (useFoodPointStep <= 8) useFoodPointStep++;
            else // герой делает 10 шагов и происходит вычитание 1 очка еды
            {
                useFoodPointStep = 0;
                if(player.FoodPoints - 1 > 0)
                {
                    player.FoodPoints--;
                } else
                {
                    if(player.PlayerHealth - 1 > 0)
                    {
                        player.PlayerHealth--;
                        Paint_GUIHitPoints();
                        player.FoodPoints = gameFoodPoints / 3;
                    } else
                    {
                        imgUI.Draw(this.CreateGraphics(), 10, 10, 0);
                        arrow_down_t = arrow_up_t = arrow_left_t = arrow_right_t = false;
                        freeze_actions = true;
                        playerDyingWithoutFood = true;
                    }
                }
            }
        }

        public void SwordDamagedEnemy()
        {
            #region Попадание мечем по противнику
            switch(playerDirection)
            {
                case HeroDirection.is_down:
                    {
                        swordAttackEnemyPositionBuff = new Point(player.PlayerPos.X, player.PlayerPos.Y + 1);
                        break;
                    }
                case HeroDirection.is_up:
                    {
                        swordAttackEnemyPositionBuff = new Point(player.PlayerPos.X, player.PlayerPos.Y - 1);
                        break;
                    }
                case HeroDirection.is_left:
                    {
                        swordAttackEnemyPositionBuff = new Point(player.PlayerPos.X - 1, player.PlayerPos.Y);
                        break;
                    }
                case HeroDirection.is_right:
                    {
                        swordAttackEnemyPositionBuff = new Point(player.PlayerPos.X + 1, player.PlayerPos.Y);
                        break;
                    }
            }
            foreach (Enemy enemyElement in enemyCollection)
            {
                if (enemyElement.UnitStatusReady == EnemyGetMapped.is_ok)
                {
                    if (enemyElement.EnemyLocation == swordAttackEnemyPositionBuff && enemyElement.HitPoints != EnemyHealth.HP0)
                    {
                        if (enemyElement.HitPoints - 1 != EnemyHealth.HP0)
                        {
                            soundAttackSwordEnemy.Play();
                            enemyElement.Damaged = true; // удар по противнику
                            enemyElement.HitPoints--;
                            if(bonusPowerActivated) // бонус силы атаки
                            {
                                switch(playerDirection)
                                {
                                    case HeroDirection.is_down:
                                        {
                                            if(map[enemyElement.EnemyLocation.X, enemyElement.EnemyLocation.Y + 1] == 0 || map[enemyElement.EnemyLocation.X, enemyElement.EnemyLocation.Y + 1] == 94)
                                            {
                                                map[enemyElement.EnemyLocation.X, enemyElement.EnemyLocation.Y] = 0;
                                                imageListGame.Draw(this.CreateGraphics(), enemyElement.EnemyLocation.X * picSizeWidth, enemyElement.EnemyLocation.Y * picSizeHeight, map[enemyElement.EnemyLocation.X, enemyElement.EnemyLocation.Y]);
                                                enemyElement.SetEnemyLocation(new Point(enemyElement.EnemyLocation.X, enemyElement.EnemyLocation.Y + 1));
                                            }
                                            /*if(map[enemyElement.EnemyLocation.X, enemyElement.EnemyLocation.Y + 1] == 79)
                                            {
                                                map[enemyElement.EnemyLocation.X, enemyElement.EnemyLocation.Y] = 0;
                                                imageListGame.Draw(this.CreateGraphics(), enemyElement.EnemyLocation.X * picSizeWidth, enemyElement.EnemyLocation.Y * picSizeHeight, map[enemyElement.EnemyLocation.X, enemyElement.EnemyLocation.Y]);
                                                enemyElement.SetEnemyLocation(new Point(enemyElement.EnemyLocation.X, enemyElement.EnemyLocation.Y + 1));
                                                enemyElement.DyingByWater = true; // активация анимации тонущего врага
                                            }*/
                                            break;
                                        }
                                    case HeroDirection.is_up:
                                        {
                                            if(map[enemyElement.EnemyLocation.X, enemyElement.EnemyLocation.Y - 1] == 0 || map[enemyElement.EnemyLocation.X, enemyElement.EnemyLocation.Y - 1] == 94)
                                            {
                                                map[enemyElement.EnemyLocation.X, enemyElement.EnemyLocation.Y] = 0;
                                                imageListGame.Draw(this.CreateGraphics(), enemyElement.EnemyLocation.X * picSizeWidth, enemyElement.EnemyLocation.Y * picSizeHeight, map[enemyElement.EnemyLocation.X, enemyElement.EnemyLocation.Y]);
                                                enemyElement.SetEnemyLocation(new Point(enemyElement.EnemyLocation.X, enemyElement.EnemyLocation.Y - 1));
                                            }
                                            /*if(map[enemyElement.EnemyLocation.X, enemyElement.EnemyLocation.Y - 1] == 79)
                                            {
                                                map[enemyElement.EnemyLocation.X, enemyElement.EnemyLocation.Y] = 0;
                                                imageListGame.Draw(this.CreateGraphics(), enemyElement.EnemyLocation.X * picSizeWidth, enemyElement.EnemyLocation.Y * picSizeHeight, map[enemyElement.EnemyLocation.X, enemyElement.EnemyLocation.Y]);
                                                enemyElement.SetEnemyLocation(new Point(enemyElement.EnemyLocation.X, enemyElement.EnemyLocation.Y - 1));
                                                enemyElement.DyingByWater = true; // активация анимации тонущего врага
                                            }*/
                                            break;
                                        }
                                    case HeroDirection.is_left:
                                        {
                                            if (map[enemyElement.EnemyLocation.X - 1, enemyElement.EnemyLocation.Y] == 0 || map[enemyElement.EnemyLocation.X - 1, enemyElement.EnemyLocation.Y] == 94)
                                            {
                                                map[enemyElement.EnemyLocation.X, enemyElement.EnemyLocation.Y] = 0;
                                                imageListGame.Draw(this.CreateGraphics(), enemyElement.EnemyLocation.X * picSizeWidth, enemyElement.EnemyLocation.Y * picSizeHeight, map[enemyElement.EnemyLocation.X, enemyElement.EnemyLocation.Y]);
                                                enemyElement.SetEnemyLocation(new Point(enemyElement.EnemyLocation.X - 1, enemyElement.EnemyLocation.Y));
                                            }
                                            /*if (map[enemyElement.EnemyLocation.X - 1, enemyElement.EnemyLocation.Y] == 79)
                                            {
                                                map[enemyElement.EnemyLocation.X, enemyElement.EnemyLocation.Y] = 0;
                                                imageListGame.Draw(this.CreateGraphics(), enemyElement.EnemyLocation.X * picSizeWidth, enemyElement.EnemyLocation.Y * picSizeHeight, map[enemyElement.EnemyLocation.X, enemyElement.EnemyLocation.Y]);
                                                enemyElement.SetEnemyLocation(new Point(enemyElement.EnemyLocation.X - 1, enemyElement.EnemyLocation.Y));
                                                enemyElement.DyingByWater = true; // активация анимации тонущего врага
                                            }*/
                                            break;
                                        }
                                    case HeroDirection.is_right:
                                        {
                                            if (map[enemyElement.EnemyLocation.X + 1, enemyElement.EnemyLocation.Y] == 0 || map[enemyElement.EnemyLocation.X + 1, enemyElement.EnemyLocation.Y] == 94)
                                            {
                                                map[enemyElement.EnemyLocation.X, enemyElement.EnemyLocation.Y] = 0;
                                                imageListGame.Draw(this.CreateGraphics(), enemyElement.EnemyLocation.X * picSizeWidth, enemyElement.EnemyLocation.Y * picSizeHeight, map[enemyElement.EnemyLocation.X, enemyElement.EnemyLocation.Y]);
                                                enemyElement.SetEnemyLocation(new Point(enemyElement.EnemyLocation.X + 1, enemyElement.EnemyLocation.Y));
                                            }
                                            /*if (map[enemyElement.EnemyLocation.X + 1, enemyElement.EnemyLocation.Y] == 79)
                                            {
                                                map[enemyElement.EnemyLocation.X, enemyElement.EnemyLocation.Y] = 0;
                                                imageListGame.Draw(this.CreateGraphics(), enemyElement.EnemyLocation.X * picSizeWidth, enemyElement.EnemyLocation.Y * picSizeHeight, map[enemyElement.EnemyLocation.X, enemyElement.EnemyLocation.Y]);
                                                enemyElement.SetEnemyLocation(new Point(enemyElement.EnemyLocation.X + 1, enemyElement.EnemyLocation.Y));
                                                enemyElement.DyingByWater = true; // активация анимации тонущего врага
                                            }*/
                                            break;
                                        }
                                }
                            }
                            soundAttackSwordEnemy.Position = TimeSpan.MinValue;
                        }
                        else
                        {
                            enemyElement.HitPoints = EnemyHealth.HP0;
                            currentlyEnemiesDied++;
                            player.KilledEnemies++;
                            if (currentlyEnemiesDied == countEnemies)
                            {
                                allEnemiesDied = true;
                                launchCountdownTimer = true;
                                currentlyEnemiesDied = 0;
                            }
                            enemyElement.DyingBySword = true;
                        }
                    }
                }
            }
            #endregion
        }

        public void HeroTakeMapItem()
        {
            Point possiblePositionMapItemBuff = new Point(-1, -1);
            switch(playerDirection)
            {
                case HeroDirection.is_down:
                    {
                        possiblePositionMapItemBuff = new Point(player.PlayerPos.X, player.PlayerPos.Y + 1);
                        break;
                    }
                case HeroDirection.is_up:
                    {
                        possiblePositionMapItemBuff = new Point(player.PlayerPos.X, player.PlayerPos.Y - 1);
                        break;
                    }
                case HeroDirection.is_left:
                    {
                        possiblePositionMapItemBuff = new Point(player.PlayerPos.X - 1, player.PlayerPos.Y);
                        break;
                    }
                case HeroDirection.is_right:
                    {
                        possiblePositionMapItemBuff = new Point(player.PlayerPos.X + 1, player.PlayerPos.Y);
                        break;
                    }
            }
            switch(map[possiblePositionMapItemBuff.X, possiblePositionMapItemBuff.Y])
            {
                case 157:
                    {
                        soundPickUpCoin.Play();
                        if(player.FoodPoints + 5 >= gameFoodPoints)
                        {
                            player.FoodPoints = gameFoodPoints;
                        } else
                        {
                            player.FoodPoints += 5;
                        }
                        soundPickUpCoin.Position = TimeSpan.MinValue;
                        break;
                    }
                case 158:
                    {
                        soundPickUpCoin.Play();
                        if(player.FoodPoints + 10 >= gameFoodPoints)
                        {
                            player.FoodPoints = gameFoodPoints;
                        } else
                        {
                            player.FoodPoints += 10;
                        }
                        soundPickUpCoin.Position = TimeSpan.MinValue;
                        break;
                    }
                case 159:
                    {
                        soundPickUpCoin.Play();
                        if(player.FoodPoints + 2 >= gameFoodPoints)
                        {
                            player.FoodPoints = gameFoodPoints;
                        } else
                        {
                            player.FoodPoints += 2;
                        }
                        if (!bonusEnergyActivated)
                        {
                            gameEnergyPoints += bonusMaxEnergyCount;
                            bonusEnergyActivated = true;
                        }
                        soundPickUpCoin.Position = TimeSpan.MinValue;
                        break;
                    }
                case 160:
                    {
                        soundPickUpCoin.Play();
                        if(player.FoodPoints + 2 >= gameFoodPoints)
                        {
                            player.FoodPoints = gameFoodPoints;
                        } else
                        {
                            player.FoodPoints += 2;
                        }
                        if (!bonusSpeedActivated)
                        {
                            countSpeedEnemyMoving += bonusIncreaseSpeedEnemy;
                            bonusSpeedActivated = true;
                        }
                        soundPickUpCoin.Position = TimeSpan.MinValue;
                        break;
                    }
                case 161:
                    {
                        soundPickUpCoin.Play();
                        if(player.FoodPoints + 3 >= gameFoodPoints)
                        {
                            player.FoodPoints = gameFoodPoints;
                        } else
                        {
                            player.FoodPoints += 3;
                        }
                        if(!bonusPowerActivated)
                        {
                            bonusPowerActivated = true;
                        }
                        soundPickUpCoin.Position = TimeSpan.MinValue;
                        break;
                    }
                case 162:
                    {
                        soundPickUpCoin.Play();
                        if(player.FoodPoints + 10 >= gameFoodPoints)
                        {
                            player.FoodPoints = gameFoodPoints;
                        } else
                        {
                            player.FoodPoints += 10;
                        }
                        if(player.EnergyPoints + 10 >= gameEnergyPoints)
                        {
                            player.EnergyPoints = gameEnergyPoints;
                        } else
                        {
                            player.EnergyPoints += 10;
                        }
                        soundPickUpCoin.Position = TimeSpan.MinValue;
                        break;
                    }
                case 163:
                    {
                        soundPickUpCoin.Play();
                        player.ShovelCount++;
                        soundPickUpCoin.Position = TimeSpan.MinValue;
                        break;
                    }
                case 164:
                    {
                        soundPickUpCoin.Play();
                        if(player.PlayerHealth + 1 > 3)
                        {
                            player.PlayerHealth = 3;
                        } else
                        {
                            player.PlayerHealth++;
                        }
                        soundPickUpCoin.Position = TimeSpan.MinValue;
                        break;
                    }
                case 165:
                    {
                        soundPickUpCoin.Play();
                        player.CoinCount++;
                        soundPickUpCoin.Position = TimeSpan.MinValue;
                        break;
                    }
            }
        }

        public void ShovelUseAnimation()
        {
            if(shovelUseActivated)
            {
                switch(playerDirection)
                {
                    case HeroDirection.is_down:
                        {
                            if(shovelUseStepDown <= 2)
                            {
                                imgHeroUseShovel.Draw(this.CreateGraphics(), player.PlayerPos.X * picSizeWidth, player.PlayerPos.Y * picSizeHeight, shovelUseStepDown);
                                shovelUseStepDown++;
                            } else
                            {
                                shovelUseStepDown = 0;
                                imageListGame.Draw(this.CreateGraphics(), player.PlayerPos.X * picSizeWidth, player.PlayerPos.Y * picSizeHeight, map[player.PlayerPos.X, player.PlayerPos.Y]);
                                arrow_down_t = arrow_up_t = arrow_left_t = arrow_right_t = true;
                                freeze_actions = false;
                                map[locationStumpBuffer.X, locationStumpBuffer.Y] = 0;
                                imageListGame.Draw(this.CreateGraphics(), locationStumpBuffer.X * picSizeWidth, locationStumpBuffer.Y * picSizeHeight, map[locationStumpBuffer.X, locationStumpBuffer.Y]);
                                shovelUseActivated = false;
                            }
                            break;
                        }
                    case HeroDirection.is_up:
                        {
                            if(shovelUseStepUp <= 5)
                            {
                                imgHeroUseShovel.Draw(this.CreateGraphics(), player.PlayerPos.X * picSizeWidth, player.PlayerPos.Y * picSizeHeight, shovelUseStepUp);
                                shovelUseStepUp++;
                            } else
                            {
                                shovelUseStepUp = 3;
                                imageListGame.Draw(this.CreateGraphics(), player.PlayerPos.X * picSizeWidth, player.PlayerPos.Y * picSizeHeight, map[player.PlayerPos.X, player.PlayerPos.Y]);
                                arrow_down_t = arrow_up_t = arrow_left_t = arrow_right_t = true;
                                freeze_actions = false;
                                map[locationStumpBuffer.X, locationStumpBuffer.Y] = 0;
                                imageListGame.Draw(this.CreateGraphics(), locationStumpBuffer.X * picSizeWidth, locationStumpBuffer.Y * picSizeHeight, map[locationStumpBuffer.X, locationStumpBuffer.Y]);
                                shovelUseActivated = false;
                            }
                            break;
                        }
                    case HeroDirection.is_left:
                        {
                            if (shovelUseStepLeft <= 8)
                            {
                                imgHeroUseShovel.Draw(this.CreateGraphics(), player.PlayerPos.X * picSizeWidth, player.PlayerPos.Y * picSizeHeight, shovelUseStepLeft);
                                shovelUseStepLeft++;
                            }
                            else
                            {
                                shovelUseStepLeft = 6;
                                imageListGame.Draw(this.CreateGraphics(), player.PlayerPos.X * picSizeWidth, player.PlayerPos.Y * picSizeHeight, map[player.PlayerPos.X, player.PlayerPos.Y]);
                                arrow_down_t = arrow_up_t = arrow_left_t = arrow_right_t = true;
                                freeze_actions = false;
                                map[locationStumpBuffer.X, locationStumpBuffer.Y] = 0;
                                imageListGame.Draw(this.CreateGraphics(), locationStumpBuffer.X * picSizeWidth, locationStumpBuffer.Y * picSizeHeight, map[locationStumpBuffer.X, locationStumpBuffer.Y]);
                                shovelUseActivated = false;
                            }
                            break;
                        }
                    case HeroDirection.is_right:
                        {
                            if(shovelUseStepRight <= 11)
                            {
                                imgHeroUseShovel.Draw(this.CreateGraphics(), player.PlayerPos.X * picSizeWidth, player.PlayerPos.Y * picSizeHeight, shovelUseStepRight);
                                shovelUseStepRight++;
                            } else
                            {
                                shovelUseStepRight = 9;
                                imageListGame.Draw(this.CreateGraphics(), player.PlayerPos.X * picSizeWidth, player.PlayerPos.Y * picSizeHeight, map[player.PlayerPos.X, player.PlayerPos.Y]);
                                arrow_down_t = arrow_up_t = arrow_left_t = arrow_right_t = true;
                                freeze_actions = false;
                                map[locationStumpBuffer.X, locationStumpBuffer.Y] = 0;
                                imageListGame.Draw(this.CreateGraphics(), locationStumpBuffer.X * picSizeWidth, locationStumpBuffer.Y * picSizeHeight, map[locationStumpBuffer.X, locationStumpBuffer.Y]);
                                shovelUseActivated = false;
                            }
                            break;
                        }
                }
            }
        }

        public void TreeGrowingByWater()
        {
            if (treeGrowingActivated)
            {
                if (growingTreeStep <= 65)
                {
                    imageListGame.Draw(this.CreateGraphics(), growingTreePosition.X * picSizeWidth, growingTreePosition.Y * picSizeHeight, growingTreeStep);
                    growingTreeStep++;
                }
                else
                {
                    growingTreeStep = 49;
                    treeGrowingActivated = false;
                }
            }
        }

        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.T && GameplayActivated)
            {
                if (myInterface.CurrentItem <= itemTag.tag_bow)
                {
                    myInterface.CurrentItem++;
                }
                else
                {
                    myInterface.CurrentItem = itemTag.tag_none;
                }
            }
            // activate shield
            if (e.KeyCode == Keys.F && !freeze_actions)
            {
                if (!player.ShieldActivated)
                {
                    if (player.EnergyPoints > 0)
                    {
                        player.ShieldActivated = true; // активация щита
                    }
                } else
                {
                    player.ShieldActivated = false; // деактивация щита
                    map[player.PlayerPos.X, player.PlayerPos.Y] = 2;
                    imageListGame.Draw(this.CreateGraphics(), player.PlayerPos.X * picSizeWidth, player.PlayerPos.Y * picSizeHeight, map[player.PlayerPos.X, player.PlayerPos.Y]);
                }
            }
            if(e.KeyCode == Keys.E && !freeze_actions && nearTree)
            {
                if (player.WaterBottles >= 1)
                {
                    foreach (Tree treeElement in myTrees)
                    {
                        if (treeElement.TreeLocation == possibleStumpPositionForWatering && treeElement.HitPoints == healthStatus.HP0_stump)
                        {
                            arrow_down_t = arrow_up_t = arrow_left_t = arrow_right_t = false;
                            freeze_actions = true;
                            if (treeElement.WateringStumpState + 1 < WaterStatus.high)
                            {
                                treeStumpWateringActivated = true;
                                treeElement.WateringStumpState++;
                                stumpWaterStatusBuff = treeElement.WateringStumpState;
                            }
                            else if (treeElement.WateringStumpState + 1 == WaterStatus.high)
                            {
                                treeElement.WateringStumpState = WaterStatus.high;
                                treeElement.HitPoints = healthStatus.HP100;
                                stumpWaterStatusBuff = treeElement.WateringStumpState;
                                treeStumpWateringActivated = true; // запуск анимации с водой
                            }
                        }
                    }
                }
            }
            if(e.KeyCode == Keys.E && !freeze_actions && nearRoom)
            {
                freeze_actions = true;
                room = true;
                MappingRoom();
                PaintRoom();
                apartmentSound.PlayLooping();
            }
            if(e.KeyCode == Keys.Escape && building_mode)
            {
                building_mode = false;
                PaintAllMap();
                Paint_GUIHitPoints();
                myBag.Key = ""; // очищаем ключ инвентаря
                arrow_down_t = arrow_up_t = arrow_left_t = arrow_right_t = true; // активировать движения персонажа
                freeze_actions = false; // активировать атаку героя
                timer_moving.Enabled = true;
            }
            if(e.KeyCode == Keys.D && !freeze_actions && !stoneIsFLying && !stoneAttackDestroyed)
            {
                if(player.StoneCount > 0)
                {
                    soundThrowStone.Play();
                    pDirectionBuff = playerDirection;
                    AttackStoneDirection();
                    player.StoneCount--;
                    soundThrowStone.Position = TimeSpan.MinValue;
                }
            }
            if(e.KeyCode == Keys.Enter && status_start_menu)
            {
                PaintMenu();
                mainMenuActivated = true;
                status_start_menu = false;
                Cursor.Current = new Cursor("tmp_game_cursor.cur");
            }
            if(e.KeyCode == Keys.Enter && msgGameControls)
            {
                msgGameControls = false;
                PaintMap();
                PaintRiver();
                Paint_GUI();
                arrow_down_t = arrow_up_t = arrow_left_t = arrow_right_t = true;
                freeze_actions = false;
            }
            /*if (e.KeyCode == Keys.Q)
                Initial_Map();*/

            // вызов паузы во время игры
            if(e.KeyCode == Keys.Escape && GameplayActivated && !building_mode)
            {
                if (!pauseMenuActivated)
                {
                    PaintMenuPause(); // рисуем окно меню паузы
                    pauseMenuActivated = true; // активация события кнопок на форме меню паузы
                    timer_moving.Enabled = false; // выключаем таймер на движение ботов и некую анимацию на карте
                    arrow_down_t = arrow_up_t = arrow_left_t = arrow_right_t = false; // герой не двигается
                    freeze_actions = true; // герой не может взаимодействовать с окружением
                } else
                {
                    pauseMenuActivated = false; // деактивация события кнопок на форме меню паузы
                    PaintAllMap(); // перерисовываем всю карту заново
                    timer_moving.Enabled = true; // включаем таймер на движение ботов и некую анимацию на карте
                    arrow_down_t = arrow_up_t = arrow_left_t = arrow_right_t = true; // включаем передвижение героя
                    freeze_actions = false; // герой может взаимодействовать с окружением
                }
            }

            // вызов формы инвентаря
            if(e.KeyCode == Keys.I && GameplayActivated && !inventoryActivated && !building_mode)
            {
                inventorySound.Play();
                anyBtnsClicked_inventory = false; // задаем значение ложь т.к. кнопки на форме инвентаря еще не были нажаты
                PaintInventory();
                inventoryActivated = true;
                arrow_down_t = arrow_up_t = arrow_left_t = arrow_right_t = false; // ограничить движения персонажа
                freeze_actions = true; // ограничить атаку героя
                inventorySound.Position = TimeSpan.MinValue;
            }

            if(e.KeyCode == Keys.E && !freeze_actions && nearRiver)
            {
                if(player.EmptyBottles > 0)
                {
                    player.PState = pStatusBuff;
                    arrow_down_t = arrow_up_t = arrow_left_t = arrow_right_t = false;
                    freeze_actions = true;
                    get_water_count = 0;
                }
            }

            // герой передвигается вверх
            if (e.KeyCode == Keys.Up && arrow_up_t)
            {
                player.PState = playerStatus.up;
                playerDirection = HeroDirection.is_up;
                if (!room)
                {
                    if ((map[player.PlayerPos.X, player.PlayerPos.Y - 1] < 145 || map[player.PlayerPos.X, player.PlayerPos.Y - 1] > 148) && map[player.PlayerPos.X, player.PlayerPos.Y - 1] != 143 && map[player.PlayerPos.X, player.PlayerPos.Y - 1] != 144 && map[player.PlayerPos.X, player.PlayerPos.Y - 1] != 1 && map[player.PlayerPos.X, player.PlayerPos.Y - 1] != 79 && map[player.PlayerPos.X, player.PlayerPos.Y - 1] != 95)
                    {
                        HeroTakeMapItem(); // проверка поднял ли герой один из предметов генератора
                        map[player.PlayerPos.X, player.PlayerPos.Y] = 0;
                        imageListGame.Draw(this.CreateGraphics(), player.PlayerPos.X * picSizeWidth, player.PlayerPos.Y * picSizeHeight, map[player.PlayerPos.X, player.PlayerPos.Y]);
                        player.PlayerPos = new Point(player.PlayerPos.X, player.PlayerPos.Y - 1);
                        PaintPlayerWithItemUp(myInterface.CurrentItem);
                        imageListGame.Draw(this.CreateGraphics(), player.PlayerPos.X * picSizeWidth, player.PlayerPos.Y * picSizeHeight, map[player.PlayerPos.X, player.PlayerPos.Y]);
                        UsingFoodPoints();
                        DrawingItemMessage("wood"); // проверяем не наступил ли персонаж на древесину, тогда всплывает окно подсказки
                        DrawingItemMessage("bottle"); // проверяем не наступил ли персонаж на бутылку, тогда всплывает окно подсказки
                        DrawingItemMessage("stone"); // проверяем не наступил ли персонаж на камень, тогда всплывает окно подсказки
                    }
                    else
                    {
                        //происходит отрисовка персонажа, который пытается пройти вверх, но упирается в объект(имитация)
                        //map[player.PlayerPos.X, player.PlayerPos.Y] = 3;
                        PaintPlayerWithItemUp(myInterface.CurrentItem);
                        imageListGame.Draw(this.CreateGraphics(), player.PlayerPos.X * picSizeWidth, player.PlayerPos.Y * picSizeHeight, map[player.PlayerPos.X, player.PlayerPos.Y]);
                    }
                } else if(room)
                {
                    if(room_map[playerRoomPos.X, playerRoomPos.Y - 1] != 0 && room_map[playerRoomPos.X, playerRoomPos.Y - 1] != 2)
                    {
                        room_map[playerRoomPos.X, playerRoomPos.Y] = 1;
                        imgListRoom.Draw(this.CreateGraphics(), playerRoomPos.X * picSizeWidth, playerRoomPos.Y * picSizeHeight, room_map[playerRoomPos.X, playerRoomPos.Y]);
                        playerRoomPos = new Point(playerRoomPos.X, playerRoomPos.Y - 1);
                        room_map[playerRoomPos.X, playerRoomPos.Y] = 4;
                        imgListRoom.Draw(this.CreateGraphics(), playerRoomPos.X * picSizeWidth, playerRoomPos.Y * picSizeHeight, room_map[playerRoomPos.X, playerRoomPos.Y]);
                    } else
                    {
                        room_map[playerRoomPos.X, playerRoomPos.Y] = 4;
                        imgListRoom.Draw(this.CreateGraphics(), playerRoomPos.X * picSizeWidth, playerRoomPos.Y * picSizeHeight, room_map[playerRoomPos.X, playerRoomPos.Y]);
                    }
                }
            }
            // герой передвигается вниз
            if (e.KeyCode == Keys.Down && arrow_down_t)
            {
                player.PState = playerStatus.down;
                playerDirection = HeroDirection.is_down;
                if (!room)
                {
                    if ((map[player.PlayerPos.X, player.PlayerPos.Y + 1] < 145 || map[player.PlayerPos.X, player.PlayerPos.Y + 1] > 148) && map[player.PlayerPos.X, player.PlayerPos.Y + 1] != 143 && map[player.PlayerPos.X, player.PlayerPos.Y + 1] != 144 && map[player.PlayerPos.X, player.PlayerPos.Y + 1] != 1 && map[player.PlayerPos.X, player.PlayerPos.Y + 1] != 79 && map[player.PlayerPos.X, player.PlayerPos.Y + 1] != 95)
                    {
                        HeroTakeMapItem(); // проверка поднял ли герой один из предметов генератора
                        map[player.PlayerPos.X, player.PlayerPos.Y] = 0;
                        imageListGame.Draw(this.CreateGraphics(), player.PlayerPos.X * picSizeWidth, player.PlayerPos.Y * picSizeHeight, map[player.PlayerPos.X, player.PlayerPos.Y]);
                        player.PlayerPos = new Point(player.PlayerPos.X, player.PlayerPos.Y + 1);
                        //map[player.PlayerPos.X, player.PlayerPos.Y] = 2;
                        PaintPlayerWithItemDown(myInterface.CurrentItem);
                        imageListGame.Draw(this.CreateGraphics(), player.PlayerPos.X * picSizeWidth, player.PlayerPos.Y * picSizeHeight, map[player.PlayerPos.X, player.PlayerPos.Y]);
                        UsingFoodPoints();
                        DrawingItemMessage("wood"); // проверяем не наступил ли персонаж на древесину, тогда всплывает окно подсказки
                        DrawingItemMessage("bottle"); // проверяем не наступил ли персонаж на бутылку, тогда всплывает окно подсказки
                        DrawingItemMessage("stone"); // проверяем не наступил ли персонаж на камень, тогда всплывает окно подсказки
                    }
                    else
                    {
                        //происходит отрисовка персонажа, который пытается пройти вниз, но упирается в объект(имитация)
                        //map[player.PlayerPos.X, player.PlayerPos.Y] = 2;
                        PaintPlayerWithItemDown(myInterface.CurrentItem);
                        imageListGame.Draw(this.CreateGraphics(), player.PlayerPos.X * picSizeWidth, player.PlayerPos.Y * picSizeHeight, map[player.PlayerPos.X, player.PlayerPos.Y]);
                    }
                } else if(room)
                {
                    if(room_map[playerRoomPos.X, playerRoomPos.Y + 1] != 0 && room_map[playerRoomPos.X, playerRoomPos.Y + 1] != 2)
                    {
                        room_map[playerRoomPos.X, playerRoomPos.Y] = 1;
                        imgListRoom.Draw(this.CreateGraphics(), playerRoomPos.X * picSizeWidth, playerRoomPos.Y * picSizeHeight, room_map[playerRoomPos.X, playerRoomPos.Y]);
                        playerRoomPos = new Point(playerRoomPos.X, playerRoomPos.Y + 1);
                        room_map[playerRoomPos.X, playerRoomPos.Y] = 3;
                        imgListRoom.Draw(this.CreateGraphics(), playerRoomPos.X * picSizeWidth, playerRoomPos.Y * picSizeHeight, room_map[playerRoomPos.X, playerRoomPos.Y]);
                    } else
                    {
                        room = false;
                        PaintAllMap();
                        Paint_GUIHitPoints();
                        gameThemeSound.PlayLooping();
                        freeze_actions = false;
                    }
                }
            }
            // герой передвигается влево
            if (e.KeyCode == Keys.Left && arrow_left_t)
            {
                player.PState = playerStatus.left;
                playerDirection = HeroDirection.is_left;
                if (!room)
                {
                    if ((map[player.PlayerPos.X - 1, player.PlayerPos.Y] < 145 || map[player.PlayerPos.X - 1, player.PlayerPos.Y] > 148) && map[player.PlayerPos.X - 1, player.PlayerPos.Y] != 143 && map[player.PlayerPos.X - 1, player.PlayerPos.Y] != 144 && map[player.PlayerPos.X - 1, player.PlayerPos.Y] != 1 && map[player.PlayerPos.X - 1, player.PlayerPos.Y] != 79 && map[player.PlayerPos.X - 1, player.PlayerPos.Y] != 95)
                    {
                        HeroTakeMapItem(); // проверка поднял ли герой один из предметов генератора
                        map[player.PlayerPos.X, player.PlayerPos.Y] = 0;
                        imageListGame.Draw(this.CreateGraphics(), player.PlayerPos.X * picSizeWidth, player.PlayerPos.Y * picSizeHeight, map[player.PlayerPos.X, player.PlayerPos.Y]);
                        player.PlayerPos = new Point(player.PlayerPos.X - 1, player.PlayerPos.Y);
                        //map[player.PlayerPos.X, player.PlayerPos.Y] = 4;
                        PaintPlayerWithItemLeft(myInterface.CurrentItem);
                        imageListGame.Draw(this.CreateGraphics(), player.PlayerPos.X * picSizeWidth, player.PlayerPos.Y * picSizeHeight, map[player.PlayerPos.X, player.PlayerPos.Y]);
                        UsingFoodPoints();
                        DrawingItemMessage("wood"); // проверяем не наступил ли персонаж на древесину, тогда всплывает окно подсказки
                        DrawingItemMessage("bottle"); // проверяем не наступил ли персонаж на бутылку, тогда всплывает окно подсказки
                        DrawingItemMessage("stone"); // проверяем не наступил ли персонаж на камень, тогда всплывает окно подсказки
                    }
                    else
                    {
                        //происходит отрисовка персонажа, который пытается пройти влево, но упирается в объект(имитация)
                        //map[player.PlayerPos.X, player.PlayerPos.Y] = 4;
                        PaintPlayerWithItemLeft(myInterface.CurrentItem);
                        imageListGame.Draw(this.CreateGraphics(), player.PlayerPos.X * picSizeWidth, player.PlayerPos.Y * picSizeHeight, map[player.PlayerPos.X, player.PlayerPos.Y]);
                    }
                } else
                {
                    if (room_map[playerRoomPos.X - 1, playerRoomPos.Y] != 0 && room_map[playerRoomPos.X - 1, playerRoomPos.Y] != 2)
                    {
                        room_map[playerRoomPos.X, playerRoomPos.Y] = 1;
                        imgListRoom.Draw(this.CreateGraphics(), playerRoomPos.X * picSizeWidth, playerRoomPos.Y * picSizeHeight, room_map[playerRoomPos.X, playerRoomPos.Y]);
                        playerRoomPos = new Point(playerRoomPos.X - 1, playerRoomPos.Y);
                        room_map[playerRoomPos.X, playerRoomPos.Y] = 5;
                        imgListRoom.Draw(this.CreateGraphics(), playerRoomPos.X * picSizeWidth, playerRoomPos.Y * picSizeHeight, room_map[playerRoomPos.X, playerRoomPos.Y]);
                    } else
                    {
                        room_map[playerRoomPos.X, playerRoomPos.Y] = 5;
                        imgListRoom.Draw(this.CreateGraphics(), playerRoomPos.X * picSizeWidth, playerRoomPos.Y * picSizeHeight, room_map[playerRoomPos.X, playerRoomPos.Y]);
                    }
                }
            }
            // герой передвигается вправо
            if (e.KeyCode == Keys.Right && arrow_right_t)
            {
                player.PState = playerStatus.right;
                playerDirection = HeroDirection.is_right;
                if (!room)
                {
                    if ((map[player.PlayerPos.X + 1, player.PlayerPos.Y] < 145 || map[player.PlayerPos.X + 1, player.PlayerPos.Y] > 148) && map[player.PlayerPos.X + 1, player.PlayerPos.Y] != 143 && map[player.PlayerPos.X + 1, player.PlayerPos.Y] != 144 && map[player.PlayerPos.X + 1, player.PlayerPos.Y] != 1 && map[player.PlayerPos.X + 1, player.PlayerPos.Y] != 79 && map[player.PlayerPos.X + 1, player.PlayerPos.Y] != 95)
                    {
                        HeroTakeMapItem(); // проверка поднял ли герой один из предметов генератора
                        map[player.PlayerPos.X, player.PlayerPos.Y] = 0;
                        imageListGame.Draw(this.CreateGraphics(), player.PlayerPos.X * picSizeWidth, player.PlayerPos.Y * picSizeHeight, map[player.PlayerPos.X, player.PlayerPos.Y]);
                        player.PlayerPos = new Point(player.PlayerPos.X + 1, player.PlayerPos.Y);
                        //map[player.PlayerPos.X, player.PlayerPos.Y] = 5;
                        PaintPlayerWithItemRight(myInterface.CurrentItem);
                        imageListGame.Draw(this.CreateGraphics(), player.PlayerPos.X * picSizeWidth, player.PlayerPos.Y * picSizeHeight, map[player.PlayerPos.X, player.PlayerPos.Y]);
                        UsingFoodPoints();
                        DrawingItemMessage("wood"); // проверяем не наступил ли персонаж на древесину, тогда всплывает окно подсказки
                        DrawingItemMessage("bottle"); // проверяем не наступил ли персонаж на бутылку, тогда всплывает окно подсказки
                        DrawingItemMessage("stone"); // проверяем не наступил ли персонаж на камень, тогда всплывает окно подсказки
                    }
                    else
                    {
                        //происходит отрисовка персонажа, который пытается пройти вправо, но упирается в объект(имитация)
                        //map[player.PlayerPos.X, player.PlayerPos.Y] = 5;
                        PaintPlayerWithItemRight(myInterface.CurrentItem);
                        imageListGame.Draw(this.CreateGraphics(), player.PlayerPos.X * picSizeWidth, player.PlayerPos.Y * picSizeHeight, map[player.PlayerPos.X, player.PlayerPos.Y]);
                    }
                } else
                {
                    if (room_map[playerRoomPos.X + 1, playerRoomPos.Y] != 0 && room_map[playerRoomPos.X + 1, playerRoomPos.Y] != 2)
                    {
                        room_map[playerRoomPos.X, playerRoomPos.Y] = 1;
                        imgListRoom.Draw(this.CreateGraphics(), playerRoomPos.X * picSizeWidth, playerRoomPos.Y * picSizeHeight, room_map[playerRoomPos.X, playerRoomPos.Y]);
                        playerRoomPos = new Point(playerRoomPos.X + 1, playerRoomPos.Y);
                        room_map[playerRoomPos.X, playerRoomPos.Y] = 6;
                        imgListRoom.Draw(this.CreateGraphics(), playerRoomPos.X * picSizeWidth, playerRoomPos.Y * picSizeHeight, room_map[playerRoomPos.X, playerRoomPos.Y]);
                    } else
                    {
                        room_map[playerRoomPos.X, playerRoomPos.Y] = 6;
                        imgListRoom.Draw(this.CreateGraphics(), playerRoomPos.X * picSizeWidth, playerRoomPos.Y * picSizeHeight, room_map[playerRoomPos.X, playerRoomPos.Y]);
                    }
                }
            }

            //использование лука
            if (e.KeyCode == Keys.S && !freeze_actions && myInterface.CurrentItem == itemTag.tag_bow && !arrowIsFlying && !arrowAttackDestroyed && !player.ShieldActivated)
            {
                soundBowPull.Play();
                pDirectionArrowBuff = playerDirection;
                AttackArrowDirection();
                soundBowPull.Position = TimeSpan.MinValue;
            }

            //использование лопаты
            if(e.KeyCode == Keys.S && !freeze_actions && myInterface.CurrentItem == itemTag.tag_shovel && !shovelUseActivated)
            {
                if (player.ShovelCount >= 5)
                {
                    Point possiblePositionStump = Point.Empty;
                    switch (playerDirection)
                    {
                        case HeroDirection.is_down:
                            {
                                possiblePositionStump = new Point(player.PlayerPos.X, player.PlayerPos.Y + 1);
                                break;
                            }
                        case HeroDirection.is_up:
                            {
                                possiblePositionStump = new Point(player.PlayerPos.X, player.PlayerPos.Y - 1);
                                break;
                            }
                        case HeroDirection.is_left:
                            {
                                possiblePositionStump = new Point(player.PlayerPos.X - 1, player.PlayerPos.Y);
                                break;
                            }
                        case HeroDirection.is_right:
                            {
                                possiblePositionStump = new Point(player.PlayerPos.X + 1, player.PlayerPos.Y);
                                break;
                            }
                    }
                    foreach (Tree treeElement in myTrees)
                    {
                        if (treeElement.TreeLocation == possiblePositionStump && treeElement.HitPoints == healthStatus.HP0_stump)
                        {
                            locationStumpBuffer = treeElement.TreeLocation;
                            treeElement.ShovelUsed = ShovelStatus.is_yes;
                            shovelUseActivated = true;
                            arrow_down_t = arrow_up_t = arrow_left_t = arrow_right_t = false;
                            freeze_actions = true;
                            player.ShovelCount -= 5;
                        }
                    }
                }
            }

            //атака мечом
            if (e.KeyCode == Keys.S && !freeze_actions && myInterface.CurrentItem == itemTag.tag_sword && !player.ShieldActivated)
            {
                bool attackTree = false; // удар не производится по дереву
                //атаковать вниз
                if (map[player.PlayerPos.X, player.PlayerPos.Y] == 2 || map[player.PlayerPos.X, player.PlayerPos.Y] == 6 || map[player.PlayerPos.X, player.PlayerPos.Y] == 10)
                {
                    player.PState = playerStatus.attack_down;
                    //атака мечом по дереву
                    if (map[player.PlayerPos.X, player.PlayerPos.Y + 1] == 1)
                    {
                        attackTree = true;
                        foreach (Tree treeElement in myTrees)
                        {
                            if (treeElement.TreeLocation == new Point(player.PlayerPos.X, player.PlayerPos.Y + 1) && treeElement.HitPoints != healthStatus.HP0_stump)
                            {
                                if (treeElement.HitPoints - 1 != healthStatus.HP0_stump)
                                {
                                    #region wood_droping
                                    // создание и отрисовка древесины рядом с деревом
                                    foreach (Wood woodElement in myWoods)
                                    {
                                        if (woodElement.WoodLocation == new Point(player.PlayerPos.X, player.PlayerPos.Y) && woodElement.Grabbed == false)
                                        {
                                            woodWasFound = true;
                                            woodElement.ContainWood++;
                                            if (woodElement.ContainWood <= 3) woodElement.CountState = countStatus.wood_one;
                                            else woodElement.CountState = countStatus.wood_more_three;
                                            //MessageBox.Show(Convert.ToString(woodElement.ContainWood));
                                        }
                                        else woodFirstDroped = true;
                                    }
                                    // если объекта древесины нет на карте, то создаем его заново
                                    if (!(woodFirstDroped && woodWasFound) && woodArrayStep < countWoodElements)
                                    {
                                        myWoods[woodArrayStep].WoodLocation = new Point(player.PlayerPos.X, player.PlayerPos.Y);
                                        myWoods[woodArrayStep].CountState = countStatus.wood_one;
                                        myWoods[woodArrayStep].ContainWood = 1;
                                        myWoods[woodArrayStep].Grabbed = false;
                                        woodArrayStep++;
                                        woodFirstDroped = false;
                                    }
                                    woodWasFound = false;
                                    //-----------------------------------------------
                                    #endregion

                                    treeElement.HitPoints--;
                                    imageListGame.Draw(this.CreateGraphics(), treeElement.TreeLocation.X * picSizeWidth, treeElement.TreeLocation.Y * picSizeHeight, (byte)treeElement.HitPoints);
                                    treeElement.Damaged = true;
                                    treesDamaged = true;
                                }
                                else
                                {
                                    treesStump = true;
                                    treeElement.HitPoints = healthStatus.HP0_stump;
                                    treeStumpState = treeElement.TreeLocation;
                                    treeElement.Damaged = false;
                                    treesDamaged = false;
                                }
                            }
                        }
                    }
                    else attackTree = false;
                    #region sword_attack
                    //атака мечом по камню
                    if (map[player.PlayerPos.X, player.PlayerPos.Y + 1] == 95)
                    {
                        foreach (Rock rockElement in myRocks)
                        {
                            if (rockElement.RockLocation == new Point(player.PlayerPos.X, player.PlayerPos.Y + 1) && rockElement.HitPoints != rockHealthStatus.HP0)
                            {
                                if (rockElement.HitPoints - 1 != rockHealthStatus.HP0)
                                {
                                    rockElement.HitPoints--; // отнимаем очки жизней
                                    imgListStone.Draw(this.CreateGraphics(), rockElement.RockLocation.X * picSizeWidth, rockElement.RockLocation.Y * picSizeHeight, (int)rockElement.HitPoints);
                                    rockElement.Damaged = true; // ставим флаг на той скале по которой был произведен удар
                                    rockDamaged = true; // произошел удар по какой-то скале на карте
                                }
                                else
                                {
                                    rockDestroyingStatus = true; // если хитпоинты скалы равны нулю, то инициируем анимацию разрушения
                                    rockElement.HitPoints = rockHealthStatus.HP0;
                                    rockDestroyingStatePos = rockElement.RockLocation; // запоминаем расположение разрушенной скалы
                                    rockElement.Damaged = false;
                                    rockDamaged = false;
                                }
                            }
                        }
                    }
                    #endregion
                    #region sword_attack_enemy
                    SwordDamagedEnemy(); // атака мечом по противнику
                    #endregion
                }
                //атаковать вверх
                if (map[player.PlayerPos.X, player.PlayerPos.Y] == 3 || map[player.PlayerPos.X, player.PlayerPos.Y] == 12)
                {
                    player.PState = playerStatus.attack_up;
                    //атака мечом по дереву
                    if (map[player.PlayerPos.X, player.PlayerPos.Y - 1] == 1)
                    {
                        attackTree = true;
                        foreach (Tree treeElement in myTrees)
                        {
                            if (treeElement.TreeLocation == new Point(player.PlayerPos.X, player.PlayerPos.Y - 1) && treeElement.HitPoints != healthStatus.HP0_stump)
                            {
                                if (treeElement.HitPoints - 1 != healthStatus.HP0_stump)
                                {
                                    #region wood_droping
                                    // создание и отрисовка древесины рядом с деревом
                                    foreach (Wood woodElement in myWoods)
                                    {
                                        if (woodElement.WoodLocation == new Point(player.PlayerPos.X, player.PlayerPos.Y) && woodElement.Grabbed == false)
                                        {
                                            woodWasFound = true;
                                            woodElement.ContainWood++;
                                            if (woodElement.ContainWood <= 3) woodElement.CountState = countStatus.wood_one;
                                            else woodElement.CountState = countStatus.wood_more_three;
                                            //MessageBox.Show(Convert.ToString(woodElement.ContainWood));
                                        }
                                        else woodFirstDroped = true;
                                    }
                                    // если объекта древесины нет на карте, то создаем его заново
                                    if (!(woodFirstDroped && woodWasFound) && woodArrayStep < countWoodElements)
                                    {
                                        myWoods[woodArrayStep].WoodLocation = new Point(player.PlayerPos.X, player.PlayerPos.Y);
                                        myWoods[woodArrayStep].CountState = countStatus.wood_one;
                                        myWoods[woodArrayStep].ContainWood = 1;
                                        myWoods[woodArrayStep].Grabbed = false;
                                        woodArrayStep++;
                                        woodFirstDroped = false;
                                    }
                                    woodWasFound = false;
                                    //-----------------------------------------------
                                    #endregion

                                    treeElement.HitPoints--;
                                    imageListGame.Draw(this.CreateGraphics(), treeElement.TreeLocation.X * picSizeWidth, treeElement.TreeLocation.Y * picSizeHeight, (byte)treeElement.HitPoints);
                                    treeElement.Damaged = true;
                                    treesDamaged = true;
                                }
                                else
                                {
                                    treesStump = true;
                                    treeElement.HitPoints = healthStatus.HP0_stump;
                                    treeStumpState = treeElement.TreeLocation;
                                    treeElement.Damaged = false;
                                    treesDamaged = false;
                                }
                            }
                        }
                    }
                    else attackTree = false;
                    #region sword_attack
                    //атака мечом по камню
                    if (map[player.PlayerPos.X, player.PlayerPos.Y - 1] == 95)
                    {
                        foreach (Rock rockElement in myRocks)
                        {
                            if (rockElement.RockLocation == new Point(player.PlayerPos.X, player.PlayerPos.Y - 1) && rockElement.HitPoints != rockHealthStatus.HP0)
                            {
                                if (rockElement.HitPoints - 1 != rockHealthStatus.HP0)
                                {
                                    rockElement.HitPoints--; // отнимаем очки жизней
                                    imgListStone.Draw(this.CreateGraphics(), rockElement.RockLocation.X * picSizeWidth, rockElement.RockLocation.Y * picSizeHeight, (int)rockElement.HitPoints);
                                    rockElement.Damaged = true; // ставим флаг на той скале по которой был произведен удар
                                    rockDamaged = true; // произошел удар по какой-то скале на карте
                                }
                                else
                                {
                                    rockDestroyingStatus = true; // если хитпоинты скалы равны нулю, то инициируем анимацию разрушения
                                    rockElement.HitPoints = rockHealthStatus.HP0;
                                    rockDestroyingStatePos = rockElement.RockLocation; // запоминаем расположение разрушенной скалы
                                    rockElement.Damaged = false;
                                    rockDamaged = false;
                                }
                            }
                        }
                    }
                    #endregion
                    #region sword_attack_enemy
                    SwordDamagedEnemy(); // атака мечом по противнику
                    #endregion
                }
                //атаковать влево, когда спрайт с расположением персонажа и индексом равным 4 или 16
                if (map[player.PlayerPos.X, player.PlayerPos.Y] == 4 || map[player.PlayerPos.X, player.PlayerPos.Y] == 16)
                {
                    player.PState = playerStatus.attack_left;
                    //атака мечом по дереву
                    if (map[player.PlayerPos.X - 1, player.PlayerPos.Y] == 1)
                    {
                        attackTree = true;
                        foreach (Tree treeElement in myTrees)
                        {
                            if (treeElement.TreeLocation == new Point(player.PlayerPos.X - 1, player.PlayerPos.Y) && treeElement.HitPoints != healthStatus.HP0_stump)
                            {
                                if (treeElement.HitPoints - 1 != healthStatus.HP0_stump)
                                {
                                    #region wood_droping
                                    // создание и отрисовка древесины рядом с деревом
                                    foreach (Wood woodElement in myWoods)
                                    {
                                        if (woodElement.WoodLocation == new Point(player.PlayerPos.X, player.PlayerPos.Y) && woodElement.Grabbed == false)
                                        {
                                            woodWasFound = true;
                                            woodElement.ContainWood++;
                                            if (woodElement.ContainWood <= 3) woodElement.CountState = countStatus.wood_one;
                                            else woodElement.CountState = countStatus.wood_more_three;
                                            //MessageBox.Show(Convert.ToString(woodElement.ContainWood));
                                        }
                                        else woodFirstDroped = true;
                                    }
                                    // если объекта древесины нет на карте, то создаем его заново
                                    if (!(woodFirstDroped && woodWasFound) && woodArrayStep < countWoodElements)
                                    {
                                        myWoods[woodArrayStep].WoodLocation = new Point(player.PlayerPos.X, player.PlayerPos.Y);
                                        myWoods[woodArrayStep].CountState = countStatus.wood_one;
                                        myWoods[woodArrayStep].ContainWood = 1;
                                        myWoods[woodArrayStep].Grabbed = false;
                                        woodArrayStep++;
                                        woodFirstDroped = false;
                                    }
                                    woodWasFound = false;
                                    //-----------------------------------------------
                                    #endregion

                                    treeElement.HitPoints--;
                                    imageListGame.Draw(this.CreateGraphics(), treeElement.TreeLocation.X * picSizeWidth, treeElement.TreeLocation.Y * picSizeHeight, (byte)treeElement.HitPoints);
                                    treeElement.Damaged = true;
                                    treesDamaged = true;
                                }
                                else
                                {
                                    treesStump = true;
                                    treeElement.HitPoints = healthStatus.HP0_stump;
                                    treeStumpState = treeElement.TreeLocation;
                                    treeElement.Damaged = false;
                                    treesDamaged = false;
                                }
                            }
                        }
                    }
                    else attackTree = false;
                    #region sword_attack
                    //атака мечом по камню
                    if(map[player.PlayerPos.X - 1, player.PlayerPos.Y] == 95)
                    {
                        foreach(Rock rockElement in myRocks)
                        {
                            if(rockElement.RockLocation == new Point(player.PlayerPos.X - 1, player.PlayerPos.Y) && rockElement.HitPoints != rockHealthStatus.HP0)
                            {
                                if(rockElement.HitPoints - 1 != rockHealthStatus.HP0)
                                {
                                    rockElement.HitPoints--; // отнимаем очки жизней
                                    imgListStone.Draw(this.CreateGraphics(), rockElement.RockLocation.X * picSizeWidth, rockElement.RockLocation.Y * picSizeHeight, (int)rockElement.HitPoints);
                                    rockElement.Damaged = true; // ставим флаг на той скале по которой был произведен удар
                                    rockDamaged = true; // произошел удар по какой-то скале на карте
                                } else
                                {
                                    rockDestroyingStatus = true; // если хитпоинты скалы равны нулю, то инициируем анимацию разрушения
                                    rockElement.HitPoints = rockHealthStatus.HP0;
                                    rockDestroyingStatePos = rockElement.RockLocation; // запоминаем расположение разрушенной скалы
                                    rockElement.Damaged = false;
                                    rockDamaged = false;
                                }
                            }
                        }
                    }
                    #endregion
                    #region sword_attack_enemy
                    SwordDamagedEnemy(); // атака мечом по противнику
                    #endregion
                }
                //атаковать вправо, когда спрайт с расположением персонажа и индексом равным 5 или 20
                if (map[player.PlayerPos.X, player.PlayerPos.Y] == 5 || map[player.PlayerPos.X, player.PlayerPos.Y] == 20)
                {
                    player.PState = playerStatus.attack_right;
                    //атака мечом по дереву
                    if (map[player.PlayerPos.X + 1, player.PlayerPos.Y] == 1)
                    {
                        attackTree = true;
                        foreach (Tree treeElement in myTrees)
                        {
                            if (treeElement.TreeLocation == new Point(player.PlayerPos.X + 1, player.PlayerPos.Y) && treeElement.HitPoints != healthStatus.HP0_stump)
                            {
                                if (treeElement.HitPoints - 1 != healthStatus.HP0_stump)
                                {
                                    #region wood_droping
                                    // создание и отрисовка древесины рядом с деревом
                                    foreach (Wood woodElement in myWoods)
                                    {
                                        if (woodElement.WoodLocation == new Point(player.PlayerPos.X, player.PlayerPos.Y) && woodElement.Grabbed == false)
                                        {
                                            woodWasFound = true;
                                            woodElement.ContainWood++;
                                            if (woodElement.ContainWood <= 3) woodElement.CountState = countStatus.wood_one;
                                            else woodElement.CountState = countStatus.wood_more_three;
                                            //MessageBox.Show(Convert.ToString(woodElement.ContainWood));
                                        }
                                        else woodFirstDroped = true;
                                    }
                                    // если объекта древесины нет на карте, то создаем его заново
                                    if (!(woodFirstDroped && woodWasFound) && woodArrayStep < countWoodElements)
                                    {
                                        myWoods[woodArrayStep].WoodLocation = new Point(player.PlayerPos.X, player.PlayerPos.Y);
                                        myWoods[woodArrayStep].CountState = countStatus.wood_one;
                                        myWoods[woodArrayStep].ContainWood = 1;
                                        myWoods[woodArrayStep].Grabbed = false;
                                        woodArrayStep++;
                                        woodFirstDroped = false;
                                    }
                                    woodWasFound = false;
                                    //-----------------------------------------------
                                    #endregion

                                    treeElement.HitPoints--;
                                    imageListGame.Draw(this.CreateGraphics(), treeElement.TreeLocation.X * picSizeWidth, treeElement.TreeLocation.Y * picSizeHeight, (byte)treeElement.HitPoints);
                                    treeElement.Damaged = true;
                                    treesDamaged = true;
                                }
                                else
                                {
                                    treesStump = true;
                                    treeElement.HitPoints = healthStatus.HP0_stump;
                                    treeStumpState = treeElement.TreeLocation;
                                    treeElement.Damaged = false;
                                    treesDamaged = false;
                                }
                            }
                        }
                    }
                    else attackTree = false;
                    #region sword_attack
                    //атака мечом по камню
                    if (map[player.PlayerPos.X + 1, player.PlayerPos.Y] == 95)
                    {
                        foreach (Rock rockElement in myRocks)
                        {
                            if (rockElement.RockLocation == new Point(player.PlayerPos.X + 1, player.PlayerPos.Y) && rockElement.HitPoints != rockHealthStatus.HP0)
                            {
                                if (rockElement.HitPoints - 1 != rockHealthStatus.HP0)
                                {
                                    rockElement.HitPoints--; // отнимаем очки жизней
                                    imgListStone.Draw(this.CreateGraphics(), rockElement.RockLocation.X * picSizeWidth, rockElement.RockLocation.Y * picSizeHeight, (int)rockElement.HitPoints);
                                    rockElement.Damaged = true; // ставим флаг на той скале по которой был произведен удар
                                    rockDamaged = true; // произошел удар по какой-то скале на карте
                                }
                                else
                                {
                                    rockDestroyingStatus = true; // если хитпоинты скалы равны нулю, то инициируем анимацию разрушения
                                    rockElement.HitPoints = rockHealthStatus.HP0;
                                    rockDestroyingStatePos = rockElement.RockLocation; // запоминаем расположение разрушенной скалы
                                    rockElement.Damaged = false;
                                    rockDamaged = false;
                                }
                            }
                        }
                    }
                    #endregion
                    #region sword_attack_enemy
                    SwordDamagedEnemy(); // атака мечом по противнику
                    #endregion
                }
                if (attackTree == false)
                {
                    soundSwordAttack_air.Play();
                    soundSwordAttack_air.Position = TimeSpan.MinValue; // задаем начальное значение для интервала аудиофайла, для следующего воспроизведения
                } else
                {
                    soundSwordAttack_tree.Play();
                    soundSwordAttack_tree.Position = TimeSpan.MinValue;
                }
            }
        }

        public void IsStoneCheck(Point stonePosition)
        {
            for(int i = 0; i < myRocks.Length; i++)
            {
                if(myRocks[i].RockLocation == stonePosition)
                {
                    map[myRocks[i].RockLocation.X, myRocks[i].RockLocation.Y] = 0;
                    myStones[i].StoneState = stoneStatus.is_stone;
                }
            }
        }

        private void Form1_KeyPress(object sender, KeyPressEventArgs e)
        {
            //....
            //MessageBox.Show(e.KeyChar.ToString() == "f"? "F epta": "nothing");
            /*if (e.KeyChar.ToString() == "s")
            {
                player.PState = playerStatus.down;
                if (map[player.PlayerPos.X, player.PlayerPos.Y + 1] != 1)
                {
                    map[player.PlayerPos.X, player.PlayerPos.Y] = 0;
                    imageListGame.Draw(this.CreateGraphics(), player.PlayerPos.X * 32, player.PlayerPos.Y * 32, map[player.PlayerPos.X, player.PlayerPos.Y]);
                    player.PlayerPos = new Point(player.PlayerPos.X, player.PlayerPos.Y + 1);
                    map[player.PlayerPos.X, player.PlayerPos.Y] = 2;
                    imageListGame.Draw(this.CreateGraphics(), player.PlayerPos.X * 32, player.PlayerPos.Y * 32, map[player.PlayerPos.X, player.PlayerPos.Y]);
                }
                else
                {
                    //происходит отрисовка персонажа, который пытается пройти вниз, но упирается в объект(имитация)
                    map[player.PlayerPos.X, player.PlayerPos.Y] = 2;
                    imageListGame.Draw(this.CreateGraphics(), player.PlayerPos.X * 32, player.PlayerPos.Y * 32, map[player.PlayerPos.X, player.PlayerPos.Y]);
                }
            }
            else
            {
                player.PState = playerStatus.edle;
            }*/
        }

        public void HeroBowAttackAnimation() // обработка анимации натяжения тетевы лука персонажем
        {
            #region Анимация запуска героем стрелы, направление "вниз"
            if (player.PState == playerStatus.bow_attack_down)
            {
                if (bowAttackDownStep <= 2)
                {
                    imgListBow.Draw(this.CreateGraphics(), player.PlayerPos.X * picSizeWidth, player.PlayerPos.Y * picSizeHeight, bowAttackDownStep);
                    bowAttackDownStep++;
                }
                else
                {
                    bowAttackDownStep = 0;
                    imageListGame.Draw(this.CreateGraphics(), player.PlayerPos.X * picSizeWidth, player.PlayerPos.Y * picSizeHeight, map[player.PlayerPos.X, player.PlayerPos.Y]);
                    player.PState = playerStatus.edle;
                    if ((map[player.PlayerPos.X, player.PlayerPos.Y + 1] == 0 || map[player.PlayerPos.X, player.PlayerPos.Y + 1] == 94) && player.WoodCount > 0)
                    {
                        soundBowSwishing.Play();
                        arrowAttackPos = new Point(player.PlayerPos.X, player.PlayerPos.Y + 1);
                        map[arrowAttackPos.X, arrowAttackPos.Y] = 153;
                        imageListGame.Draw(this.CreateGraphics(), arrowAttackPos.X * picSizeWidth, arrowAttackPos.Y * picSizeHeight, map[arrowAttackPos.X, arrowAttackPos.Y]);
                        player.WoodCount--; // отнимаем одну единицу древесины
                        arrowIsFlying = true; // дальше включается анимация полета стрелы
                        soundBowSwishing.Position = TimeSpan.MinValue;
                    }
                }
            }
            #endregion
            #region Анимация запуска героем стрелы, направление "вверх" (как таковой анимации в этом направлении нет)
            if (player.PState == playerStatus.bow_attack_up)
            {
                if (bowAttackUpStep <= 2)
                {
                    bowAttackUpStep++;
                }
                else
                {
                    bowAttackUpStep = 0;
                    player.PState = playerStatus.edle;
                    if ((map[player.PlayerPos.X, player.PlayerPos.Y - 1] == 0 || map[player.PlayerPos.X, player.PlayerPos.Y - 1] == 94) && player.WoodCount > 0)
                    {
                        soundBowSwishing.Play();
                        arrowAttackPos = new Point(player.PlayerPos.X, player.PlayerPos.Y - 1);
                        map[arrowAttackPos.X, arrowAttackPos.Y] = 154;
                        imageListGame.Draw(this.CreateGraphics(), arrowAttackPos.X * picSizeWidth, arrowAttackPos.Y * picSizeHeight, map[arrowAttackPos.X, arrowAttackPos.Y]);
                        player.WoodCount--;
                        arrowIsFlying = true; // дальше включается анимация полета стрелы
                        soundBowSwishing.Position = TimeSpan.MinValue;
                    }
                }
            }
            #endregion
            #region Анимация запуска героем стрелы, направление "влево"
            if (player.PState == playerStatus.bow_attack_left)
            {
                if (bowAttackLeftStep <= 5)
                {
                    imgListBow.Draw(this.CreateGraphics(), player.PlayerPos.X * picSizeWidth, player.PlayerPos.Y * picSizeHeight, bowAttackLeftStep);
                    bowAttackLeftStep++;
                }
                else
                {
                    bowAttackLeftStep = 3;
                    imageListGame.Draw(this.CreateGraphics(), player.PlayerPos.X * picSizeWidth, player.PlayerPos.Y * picSizeHeight, map[player.PlayerPos.X, player.PlayerPos.Y]);
                    player.PState = playerStatus.edle;
                    if ((map[player.PlayerPos.X - 1, player.PlayerPos.Y] == 0 || map[player.PlayerPos.X - 1, player.PlayerPos.Y] == 94) && player.WoodCount > 0)
                    {
                        soundBowSwishing.Play();
                        arrowAttackPos = new Point(player.PlayerPos.X - 1, player.PlayerPos.Y);
                        map[arrowAttackPos.X, arrowAttackPos.Y] = 155;
                        imageListGame.Draw(this.CreateGraphics(), arrowAttackPos.X * picSizeWidth, arrowAttackPos.Y * picSizeHeight, map[arrowAttackPos.X, arrowAttackPos.Y]);
                        player.WoodCount--;
                        arrowIsFlying = true; // дальше включается анимация полета стрелы
                        soundBowSwishing.Position = TimeSpan.MinValue;
                    }
                }
            }
            #endregion
            #region Анимация запуска героем стрелы, направление "вправо"
            if (player.PState == playerStatus.bow_attack_right)
            {
                if (bowAttackRightStep <= 8)
                {
                    imgListBow.Draw(this.CreateGraphics(), player.PlayerPos.X * picSizeWidth, player.PlayerPos.Y * picSizeHeight, bowAttackRightStep);
                    bowAttackRightStep++;
                }
                else
                {
                    bowAttackRightStep = 6;
                    imageListGame.Draw(this.CreateGraphics(), player.PlayerPos.X * picSizeWidth, player.PlayerPos.Y * picSizeHeight, map[player.PlayerPos.X, player.PlayerPos.Y]);
                    player.PState = playerStatus.edle;
                    if ((map[player.PlayerPos.X + 1, player.PlayerPos.Y] == 0 || map[player.PlayerPos.X + 1, player.PlayerPos.Y] == 94) && player.WoodCount > 0)
                    {
                        soundBowSwishing.Play();
                        arrowAttackPos = new Point(player.PlayerPos.X + 1, player.PlayerPos.Y);
                        map[arrowAttackPos.X, arrowAttackPos.Y] = 156;
                        imageListGame.Draw(this.CreateGraphics(), arrowAttackPos.X * picSizeWidth, arrowAttackPos.Y * picSizeHeight, map[arrowAttackPos.X, arrowAttackPos.Y]);
                        player.WoodCount--;
                        arrowIsFlying = true; // дальше включается анимация полета стрелы
                        soundBowSwishing.Position = TimeSpan.MinValue;
                    }
                }
            }
            #endregion
        }

        public void HeroAttackAnimation()
        {
            //анимация удара мечом лицом вверх: происходит один раз, после нажатия кнопки S,
            //если положения героя на карте равно индексу картинки 3 или 12 из компонента imageList (картинки персонажа с направлением вверх)
            if (player.PState == playerStatus.attack_up)
            {
                count = 0; // сбрасываем счетчик анимации в покое (персонаж моргает)
                if (attackStep_up <= 31)
                {
                    imageListGame.Draw(this.CreateGraphics(), player.PlayerPos.X * picSizeWidth, player.PlayerPos.Y * picSizeHeight, attackStep_up);
                    attackStep_up++;
                }
                else
                {
                    attackStep_up = 28;
                    imageListGame.Draw(this.CreateGraphics(), player.PlayerPos.X * picSizeWidth, player.PlayerPos.Y * picSizeHeight, 3); //индекс картинки равен 3, поскольку задается положение лицом вверх в покое
                    player.PState = playerStatus.edle; // присваиваем персонажу статус "в покое"
                }
            }

            //анимация удара мечом лицом вниз: происходит один раз, после нажатия кнопки S
            //если положения героя на карте равно индексу картинки 2, 6, или 10 из компонента imageList (картинки персонажа с направлением вниз)
            if (player.PState == playerStatus.attack_down)
            {
                count = 0; // сбрасываем счетчик анимации в покое (персонаж моргает)
                if (attackStep_down <= 27)
                {
                    imageListGame.Draw(this.CreateGraphics(), player.PlayerPos.X * picSizeWidth, player.PlayerPos.Y * picSizeHeight, attackStep_down);
                    attackStep_down++;
                }
                else
                {
                    attackStep_down = 24;
                    imageListGame.Draw(this.CreateGraphics(), player.PlayerPos.X * picSizeWidth, player.PlayerPos.Y * picSizeHeight, 2); //индекс картинки равен 2, поскольку задается положение лицом вниз в покое
                    player.PState = playerStatus.edle;
                }
            }

            //анимация удара мечом лицом влево: происходит один раз, после нажатия кнопки S,
            //если положения героя на карте равно индексу картинки 4 или 16 из компонента imageList (картинки персонажа с направлением влево)
            if (player.PState == playerStatus.attack_left)
            {
                count = 0; // сбрасываем счетчик анимации в покое (персонаж моргает)
                if (attackStep_left <= 35)
                {
                    imageListGame.Draw(this.CreateGraphics(), player.PlayerPos.X * picSizeWidth, player.PlayerPos.Y * picSizeHeight, attackStep_left);
                    attackStep_left++;
                }
                else
                {
                    attackStep_left = 32;
                    imageListGame.Draw(this.CreateGraphics(), player.PlayerPos.X * picSizeWidth, player.PlayerPos.Y * picSizeHeight, 4); //индекс картинки равен 4, поскольку задается положение лицом влево в покое
                    player.PState = playerStatus.edle;
                }
            }

            //анимация удара мечом лицом вправо: происходит один раз, после нажатия кнопки S,
            //если положения героя на карте равно индексу картинки 5 или 20 из компонента imageList (картинки персонажа с направлением вправо)
            if (player.PState == playerStatus.attack_right)
            {
                count = 0; // сбрасываем счетчик анимации в покое (персонаж моргает)
                if (attackStep_right <= 39)
                {
                    imageListGame.Draw(this.CreateGraphics(), player.PlayerPos.X * picSizeWidth, player.PlayerPos.Y * picSizeHeight, attackStep_right);
                    attackStep_right++;
                }
                else
                {
                    attackStep_right = 36;
                    imageListGame.Draw(this.CreateGraphics(), player.PlayerPos.X * picSizeWidth, player.PlayerPos.Y * picSizeHeight, 5); //индекс картинки равен 5, поскольку задается положение лицом вправо в покое
                    player.PState = playerStatus.edle;
                }
            }
        }

        public void HeroShieldActivated()
        {
            if(player.ShieldActivated)
            {
                switch(playerDirection)
                {
                    case HeroDirection.is_down:
                        {
                            map[player.PlayerPos.X, player.PlayerPos.Y] = 167;
                            break;
                        }
                    case HeroDirection.is_up:
                        {
                            map[player.PlayerPos.X, player.PlayerPos.Y] = 168;
                            break;
                        }
                    case HeroDirection.is_left:
                        {
                            map[player.PlayerPos.X, player.PlayerPos.Y] = 169;
                            break;
                        }
                    case HeroDirection.is_right:
                        {
                            map[player.PlayerPos.X, player.PlayerPos.Y] = 170;
                            break;
                        }
                }
                imageListGame.Draw(this.CreateGraphics(), player.PlayerPos.X * picSizeWidth, player.PlayerPos.Y * picSizeHeight, map[player.PlayerPos.X, player.PlayerPos.Y]);
            }
        }

        private void timer_moving_Tick(object sender, EventArgs e)
        {/*
            if (step < 10 && map != null)
            {
                for (int i = 0; i < 50; i++)
                    for (int j = 0; j < 50; j++)
                    {
                        if(map[i, j] == 1)
                            imageListGame.Draw(this.CreateGraphics(), i * 32, j * 32, step);
                    }
                step++;
            }
            else step = 7;*/

            // Бонус энергии
            if(bonusEnergyActivated)
            {
                if(bonusCornTimeOut > 0)
                {
                    bonusCornTimeOut--;
                } else
                {
                    bonusCornTimeOut = 360;
                    gameEnergyPoints -= bonusMaxEnergyCount;
                    if(player.EnergyPoints > gameEnergyPoints)
                    {
                        player.EnergyPoints = gameEnergyPoints;
                    }
                    bonusEnergyActivated = false;
                }
            }
            // Бонус уменьшение скорости противника
            if(bonusSpeedActivated)
            {
                if(bonusPepperTimeOut > 0)
                {
                    bonusPepperTimeOut--;
                } else
                {
                    bonusPepperTimeOut = 180;
                    countSpeedEnemyMoving -= bonusIncreaseSpeedEnemy;
                    bonusSpeedActivated = false;
                }
            }
            // Бонус сила атаки
            if(bonusPowerActivated)
            {
                if(bonusTomatoTimeOut > 0)
                {
                    bonusTomatoTimeOut--;
                } else
                {
                    bonusTomatoTimeOut = 120;
                    bonusPowerActivated = false;
                }
            }

            if (room == false)
            {
                // автопополнение очков энергии
                if (player.ShieldActivated == false && player.EnergyPoints < gameEnergyPoints)
                {
                    if (energyRestoreCounter > 0)
                    {
                        energyRestoreCounter--;
                    } else
                    {
                        energyRestoreCounter = 3;
                        player.EnergyPoints++;
                    }
                }

                HeroShieldActivated();
                MessageAnimation(); // анимация сообщений по ходу игры
                WoodAnimation(); // анимация древесины на карте
                BottleAnimation(); // анимация бутылок на карте
                StoneAnimation(); // анимация камней на карте
                if (launchCountdownTimer)
                {
                    if (countdownEnemy > 0) // начало обратного отсчета появления врага на карте
                    {
                        countdownEnemy--;
                    }
                    else
                    {
                        countdownEnemy = 100;
                        launchCountdownTimer = false;
                        if (allEnemiesDied) // если враги были убиты в одной волне и волна не последняя, то инициируем действие добавления врага на карту
                        {
                            enemyMapping = true;
                            countEnemies++;
                            allEnemiesDied = false;
                        }
                    }
                }
                // добавление врага на карте
                if(enemyMapping)
                {
                    for(int i = 0; i < countEnemies; i++)
                    {
                        enemyCollection[i].EnemyInitialize(EnemyHealth.HP100, enemyCollection[i].EnemyRandomizePosition(map));
                        enemyCollection[i].EnemyIncoming = true;
                    }
                    enemyMapping = false;
                }
                if(enemyCollection != null)
                {
                    foreach(Enemy enemyElement in enemyCollection)
                    {
                        if (enemyElement.UnitStatusReady == EnemyGetMapped.is_ok)
                        {
                            if (enemyElement.EnemyIncoming)
                            {
                                enemyElement.EnemyIncomingAnimation(this.CreateGraphics(), imgListEnemy);
                            }
                            else if (enemyElement.DyingBySword == false && enemyElement.DyingByArrow == false && enemyElement.Damaged == false && enemyElement.AttackHero == false && enemyElement.DyingByWater == false)
                            {
                                if (enemyElement.TimeOutMoving > 0) enemyElement.TimeOutMoving--;
                                else
                                {
                                    enemyElement.TimeOutMoving = countSpeedEnemyMoving;
                                    enemyElement.EnemyMove(this.CreateGraphics(), imageListGame, imgListEnemy, ref map, player.PlayerPos);
                                }
                                enemyElement.EnemyRunningAnimation(this.CreateGraphics(), imgListEnemy);
                            }
                            if (enemyElement.DyingBySword)
                            {
                                enemyElement.EnemyDyingBySword(this.CreateGraphics(), imgListEnemy, imageListGame, ref map);
                            }
                        }
                    }
                }
            }

            // анимация разрушения камня
            if(rockDestroyingStatus)
            {
                if(rockDestroyingStep <= 22)
                {
                    imgListStone.Draw(this.CreateGraphics(), rockDestroyingStatePos.X * picSizeWidth, rockDestroyingStatePos.Y * picSizeHeight, rockDestroyingStep);
                    rockDestroyingStep++;
                } else
                {
                    rockDestroyingStep = 19;
                    rockDestroyingStatus = false;
                    IsStoneCheck(rockDestroyingStatePos);
                }
            }

            // анимация сруба дерева
            /*if (treesStump)
            {
                if(stumping_step >= 49)
                {
                    imageListGame.Draw(this.CreateGraphics(), treeStumpState.X * picSizeWidth, treeStumpState.Y * picSizeHeight, stumping_step);
                    stumping_step--;
                } else
                {
                    stumping_step = 65;
                    treesStump = false;
                    imageListGame.Draw(this.CreateGraphics(), treeStumpState.X * picSizeWidth, treeStumpState.Y * picSizeHeight, (byte)healthStatus.HP0_stump);
                }
            }*/
            //----------------------------------------

            //Исчезновение индикатора здоровья скалы, через 8 шагов таймера
            if(rockDamaged)
            {
                if (rockCountdownDamaged < 8) rockCountdownDamaged++; else
                {
                    for(int i = 0; i < myRocks.Length; i++)
                    {
                        if(myRocks[i].Damaged)
                        {
                            imageListGame.Draw(this.CreateGraphics(), myRocks[i].RockLocation.X * picSizeWidth, myRocks[i].RockLocation.Y * picSizeHeight, map[myRocks[i].RockLocation.X, myRocks[i].RockLocation.Y]); //отрисовываем обычную скалу с индексом равным 95 без индикатора
                            myRocks[i].Damaged = false;
                        }
                    }
                    rockDamaged = false;
                    rockCountdownDamaged = 0;
                }
            }
            //----------------------------------------


            //Исчезновение индикатора здоровья дерева, через 8 шагов таймера
            if (treesDamaged)
            {
                if(treeCountdownDamaged < 8) treeCountdownDamaged++; else
                {
                    for(int i = 0; i < myTrees.Length; i++)
                    {
                        if(myTrees[i].Damaged)
                        {
                            imageListGame.Draw(this.CreateGraphics(), myTrees[i].TreeLocation.X * picSizeWidth, myTrees[i].TreeLocation.Y * picSizeHeight, map[myTrees[i].TreeLocation.X, myTrees[i].TreeLocation.Y]); //отрисовываем обычное дерево с индексом равным 1
                            myTrees[i].Damaged = false;
                        }
                    }
                    treeCountdownDamaged = 0;
                    treesDamaged = false;
                }
            }
            //-------------------------------------------

            //анимация использования бутылки с водой относительно дерева со статусом "пень"
            if(treeStumpWateringActivated)
            {
                if(wateringStumpStep <= 5)
                {
                    imgWateringStump.Draw(this.CreateGraphics(), possibleStumpPositionForWatering.X * picSizeWidth, possibleStumpPositionForWatering.Y * picSizeHeight, wateringStumpStep);
                    wateringStumpStep++;
                } else
                {
                    wateringStumpStep = 0;
                    treeStumpWateringActivated = false;
                    soundPickupItem.Play();
                    imageListGame.Draw(this.CreateGraphics(), possibleStumpPositionForWatering.X * picSizeWidth, possibleStumpPositionForWatering.Y * picSizeHeight, (byte)stumpWaterStatusBuff);
                    // опустошаем бутылку
                    player.WaterBottles--;
                    player.EmptyBottles++;
                    arrow_down_t = arrow_up_t = arrow_left_t = arrow_right_t = true;
                    freeze_actions = false;
                    if(stumpWaterStatusBuff == WaterStatus.high)
                    {
                        treeGrowingActivated = true; // запускаем анимацию выращивания дерева
                        growingTreePosition = possibleStumpPositionForWatering;
                    }
                    soundPickupItem.Position = TimeSpan.MinValue;
                }
            }
            //---------------------------------------------------

            //анимация наполнения бутылки у реки, когда герой стоит лицом вниз (анимация выполняется 3 раза, перед тем как наполнить полностью бутылку)
            if(player.PState == playerStatus.get_water_down)
            {
                count = 0; // сбрасываем счетчик анимации в покое (персонаж моргает)
                if (get_water_count < 3)
                {
                    if (getWaterDownStep <= 5)
                    {
                        imgListHeroWater.Draw(this.CreateGraphics(), player.PlayerPos.X * picSizeWidth, player.PlayerPos.Y * picSizeHeight, getWaterDownStep);
                        getWaterDownStep++;
                    }
                    else
                    {
                        getWaterDownStep = 0;
                        imgListHeroWater.Draw(this.CreateGraphics(), player.PlayerPos.X * picSizeWidth, player.PlayerPos.Y * picSizeHeight, getWaterDownStep);
                        get_water_count++;
                    }
                } else
                {
                    soundPickupItem.Play();
                    get_water_count = 0;
                    imageListGame.Draw(this.CreateGraphics(), player.PlayerPos.X * picSizeWidth, player.PlayerPos.Y * picSizeHeight, 2); // анимация заканчивается и рисуется персонаж в покое направленный вниз
                    player.PState = playerStatus.edle;
                    player.EmptyBottles--;
                    player.WaterBottles++;
                    arrow_down_t = arrow_up_t = arrow_left_t = arrow_right_t = true;
                    freeze_actions = false;
                    soundPickupItem.Position = TimeSpan.MinValue;
                }
            }
            //----------------------------------

            //анимация наполнения бутылки у реки, когда герой стоит лицом вверх (анимация выполняется 3 раза, перед тем как наполнить полностью бутылку)
            if(player.PState == playerStatus.get_water_up)
            {
                count = 0; // сбрасываем счетчик анимации в покое (персонаж моргает)
                if(get_water_count < 3)
                {
                    if(getWaterUpStep <= 11)
                    {
                        imgListHeroWater.Draw(this.CreateGraphics(), player.PlayerPos.X * picSizeWidth, player.PlayerPos.Y * picSizeHeight, getWaterUpStep);
                        getWaterUpStep++;
                    } else
                    {
                        getWaterUpStep = 6;
                        imgListHeroWater.Draw(this.CreateGraphics(), player.PlayerPos.X * picSizeWidth, player.PlayerPos.Y * picSizeHeight, getWaterUpStep);
                        get_water_count++;
                    }
                } else
                {
                    soundPickupItem.Play();
                    get_water_count = 0;
                    imageListGame.Draw(this.CreateGraphics(), player.PlayerPos.X * picSizeWidth, player.PlayerPos.Y * picSizeHeight, 3); // анимация заканчивается и рисуется персонаж в покое направленный вверх
                    player.PState = playerStatus.edle;
                    player.EmptyBottles--;
                    player.WaterBottles++;
                    arrow_down_t = arrow_up_t = arrow_left_t = arrow_right_t = true;
                    freeze_actions = false;
                    soundPickupItem.Position = TimeSpan.MinValue;
                }
            }
            //----------------------------------

            //анимация наполнения бутылки у реки, когда герой стоит лицом влево (анимация выполняется 3 раза, перед тем как наполнить полностью бутылку)
            if(player.PState == playerStatus.get_water_left)
            {
                count = 0; // сбрасываем счетчик анимации в покое (персонаж моргает)
                if(get_water_count < 3)
                {
                    if(getWaterLeftStep <= 17)
                    {
                        imgListHeroWater.Draw(this.CreateGraphics(), player.PlayerPos.X * picSizeWidth, player.PlayerPos.Y * picSizeHeight, getWaterLeftStep);
                        getWaterLeftStep++;
                    } else
                    {
                        getWaterLeftStep = 12;
                        imgListHeroWater.Draw(this.CreateGraphics(), player.PlayerPos.X * picSizeWidth, player.PlayerPos.Y * picSizeHeight, getWaterLeftStep);
                        get_water_count++;
                    }
                } else
                {
                    soundPickupItem.Play();
                    get_water_count = 0;
                    imageListGame.Draw(this.CreateGraphics(), player.PlayerPos.X * picSizeWidth, player.PlayerPos.Y * picSizeHeight, 4);
                    player.PState = playerStatus.edle;
                    player.EmptyBottles--;
                    player.WaterBottles++;
                    arrow_down_t = arrow_up_t = arrow_left_t = arrow_right_t = true;
                    freeze_actions = false;
                    soundPickupItem.Position = TimeSpan.MinValue;
                }
            }
            //----------------------------------

            //анимация наполнения бутылки у реки, когда герой стоит лицом вправо (анимация выполняется 3 раза, перед тем как наполнить полностью бутылку)
            if(player.PState == playerStatus.get_water_right)
            {
                count = 0; // сбрасываем счетчик анимации в покое (персонаж моргает)
                if(get_water_count < 3)
                {
                    if(getWaterRightStep <= 23)
                    {
                        imgListHeroWater.Draw(this.CreateGraphics(), player.PlayerPos.X * picSizeWidth, player.PlayerPos.Y * picSizeHeight, getWaterRightStep);
                        getWaterRightStep++;
                    } else
                    {
                        getWaterRightStep = 18;
                        imgListHeroWater.Draw(this.CreateGraphics(), player.PlayerPos.X * picSizeWidth, player.PlayerPos.Y * picSizeHeight, getWaterRightStep);
                        get_water_count++;
                    }
                } else
                {
                    soundPickupItem.Play();
                    get_water_count = 0;
                    imageListGame.Draw(this.CreateGraphics(), player.PlayerPos.X * picSizeWidth, player.PlayerPos.Y * picSizeHeight, 5);
                    player.PState = playerStatus.edle;
                    player.EmptyBottles--;
                    player.WaterBottles++;
                    arrow_down_t = arrow_up_t = arrow_left_t = arrow_right_t = true;
                    freeze_actions = false;
                    soundPickupItem.Position = TimeSpan.MinValue;
                }
            }
            //----------------------------------


            //анимация движения вниз: проигрывается один раз, после одного нажатия кнопки ArrowDown
            if (player.PState == playerStatus.down)
                {
                if (room == false)
                {
                    if (down_step <= 9)
                    {
                        imageListGame.Draw(this.CreateGraphics(), player.PlayerPos.X * picSizeWidth, player.PlayerPos.Y * picSizeHeight, down_step);
                        down_step++;
                    }
                    else
                    {
                        down_step = 6;
                        imageListGame.Draw(this.CreateGraphics(), player.PlayerPos.X * picSizeWidth, player.PlayerPos.Y * picSizeHeight, map[player.PlayerPos.X, player.PlayerPos.Y]);
                        player.PState = playerStatus.edle; //меняем статус положения героя, когда анимация закончится, с статуса движения вниз на статус покой
                    }
                } else
                {
                    if (roomDownStep <= 8)
                    {
                        imgListRoom.Draw(this.CreateGraphics(), playerRoomPos.X * picSizeWidth, playerRoomPos.Y * picSizeHeight, roomDownStep);
                        roomDownStep++;
                    } else
                    {
                        roomDownStep = 7;
                        imgListRoom.Draw(this.CreateGraphics(), playerRoomPos.X * picSizeWidth, playerRoomPos.Y * picSizeHeight, 3);
                        player.PState = playerStatus.edle;
                    }
                }
                }

            //анимация движения вверх: проигрывается один раз, после однократного нажатия кнопки ArrowUp
            if(player.PState == playerStatus.up)
            {
                if (room == false)
                {
                    if (up_step <= 15)
                    {
                        imageListGame.Draw(this.CreateGraphics(), player.PlayerPos.X * picSizeWidth, player.PlayerPos.Y * picSizeHeight, up_step);
                        up_step++;
                    }
                    else
                    {
                        up_step = 12;
                        imageListGame.Draw(this.CreateGraphics(), player.PlayerPos.X * picSizeWidth, player.PlayerPos.Y * picSizeHeight, map[player.PlayerPos.X, player.PlayerPos.Y]);
                        player.PState = playerStatus.edle;
                    }
                } else
                {
                    if (roomUpStep <= 10)
                    {
                        imgListRoom.Draw(this.CreateGraphics(), playerRoomPos.X * picSizeWidth, playerRoomPos.Y * picSizeHeight, roomUpStep);
                        roomUpStep++;
                    } else
                    {
                        roomUpStep = 9;
                        imgListRoom.Draw(this.CreateGraphics(), playerRoomPos.X * picSizeWidth, playerRoomPos.Y * picSizeHeight, 4);
                        player.PState = playerStatus.edle;
                    }
                }
            }

            //анимация движения влево: проигрывается один раз, после однократного нажатия кнопки ArrowLeft
            if(player.PState == playerStatus.left)
            {
                if (room == false)
                {
                    if (left_step <= 19)
                    {
                        imageListGame.Draw(this.CreateGraphics(), player.PlayerPos.X * picSizeWidth, player.PlayerPos.Y * picSizeHeight, left_step);
                        left_step++;
                    }
                    else
                    {
                        left_step = 16;
                        imageListGame.Draw(this.CreateGraphics(), player.PlayerPos.X * picSizeWidth, player.PlayerPos.Y * picSizeHeight, map[player.PlayerPos.X, player.PlayerPos.Y]);
                        player.PState = playerStatus.edle;
                    }
                } else
                {
                    if (roomLeftStep <= 12)
                    {
                        imgListRoom.Draw(this.CreateGraphics(), playerRoomPos.X * picSizeWidth, playerRoomPos.Y * picSizeHeight, roomLeftStep);
                        roomLeftStep++;
                    }
                    else
                    {
                        roomLeftStep = 11;
                        imgListRoom.Draw(this.CreateGraphics(), playerRoomPos.X * picSizeWidth, playerRoomPos.Y * picSizeHeight, 5);
                        player.PState = playerStatus.edle;
                    }
                }
            }

            //анимация движения влево: проигрывается один раз, после одного нажатия кнопки ArrowRight
            if(player.PState == playerStatus.right)
            {
                if (room == false)
                {
                    if (right_step <= 23)
                    {
                        imageListGame.Draw(this.CreateGraphics(), player.PlayerPos.X * picSizeWidth, player.PlayerPos.Y * picSizeHeight, right_step);
                        right_step++;
                    }
                    else
                    {
                        right_step = 20;
                        imageListGame.Draw(this.CreateGraphics(), player.PlayerPos.X * picSizeWidth, player.PlayerPos.Y * picSizeHeight, map[player.PlayerPos.X, player.PlayerPos.Y]);
                        player.PState = playerStatus.edle;
                    }
                } else
                {
                    if (roomRightStep <= 14)
                    {
                        imgListRoom.Draw(this.CreateGraphics(), playerRoomPos.X * picSizeWidth, playerRoomPos.Y * picSizeHeight, roomRightStep);
                        roomRightStep++;
                    }
                    else
                    {
                        roomRightStep = 13;
                        imgListRoom.Draw(this.CreateGraphics(), playerRoomPos.X * picSizeWidth, playerRoomPos.Y * picSizeHeight, 6);
                        player.PState = playerStatus.edle;
                    }
                }
            }

            //анимация в покое: происходит, если значение массива в течение 14-ти шагов таймера, с координатами игрока, равно одному из 4 значений:
            // 2 - герой стоит лицом вниз
            // 3 - герой стоит лицом вверх
            // 4 - герой стоит лицом влево
            // 5 - герой стоит лицом вправо
            // Если в промежутке 0 до 14 происходит движение, то счетчик count обнуляется, иначе наращивается на 1 (идея)
            // Реализация: Если счетчик count <= 14, то происходит условие, если элемент массива с индексами в виде координат героя равен значениям 2, 3, 4, 5 в течение 14 "секунд",
            // то значение счетчика count инкрементируем, иначе обнуляем.
            // Иначе, если count = 14, присваиваем статусу положения героя на карте PState значение playerStatus.edle_motion (движение в покое)
            if (count <= 20)
            {
                if (map != null)
                {
                    if (map[player.PlayerPos.X, player.PlayerPos.Y] == 2) count++; else count = 0;
                }
            }
            else player.PState = playerStatus.edle_motion;

            //анимация движения в покое: происходит, когда статус положения героя на карте PState равен playerStatus.edle_motion
            if(player.PState == playerStatus.edle_motion)
            {
                if(edle_step <= 11)
                {
                    imageListGame.Draw(this.CreateGraphics(), player.PlayerPos.X * picSizeWidth, player.PlayerPos.Y * picSizeHeight, edle_step);
                    edle_step++;
                }
                else
                {
                    edle_step = 10;
                    imageListGame.Draw(this.CreateGraphics(), player.PlayerPos.X * picSizeWidth, player.PlayerPos.Y * picSizeHeight, edle_step);
                    count = 0;
                    player.PState = playerStatus.edle;
                }
            }
        }
    }
}
