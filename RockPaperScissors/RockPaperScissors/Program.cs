using RockPaperScissors;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Security.Cryptography.X509Certificates;

namespace RockPaperScissors
{
    class Program
    {

        static void Main(string[] args)
        {
            Game game = new Game();
            game.RunGame();
        }
    }

    public class Game
    {
        private static List<PlayerData> playerList { get; set; } = new List<PlayerData>();
        private static List<EnemyData> enemyList { get; set; } = new List<EnemyData>();
        public CreateManger createManger { get; } = new CreateManger();
        private Dictionary<int, IMenuOption> menuOption;
        readonly Random random = new Random();
        public bool IsLosser { get; set; } = false;
        public bool IsWinner { get; set; } = false;
        public bool createdPlayer { get; set; } = false;
        public bool duelOver { get; set; } = false;
        public bool usingMainMenu { get; set; } = true;

        public Game()
        {
            string[] hand = { "Rock", "Paper", "Scissors" };
            menuOption = new Dictionary<int, IMenuOption>()
            {
                { 0, new ICreatePlayerOption() },
                {1, new IStartGameOption() },
                {2, new ICreateEnemyOption() },
                {3, new IShowWinRank() },
                {4, new IExitOption() }
            };


        }

        public void RunGame()
        {


            while (true)
            {
                while (usingMainMenu)
                {
                    Console.WriteLine("歡迎來到遊戲世界！");
                    usingMainMenu = UsingMainMenu();
                }

            }
        }

        bool UsingMainMenu()
        {
            while (true)
            {
                //Console.Clear();
                Console.WriteLine("請選擇功能：\n0.建立角色\n1.開始遊戲\n2.建立敵人\n3.查詢排名\n4.離開遊戲");
                switch (int.Parse(Console.ReadLine()))
                {
                    case 0:
                        if (!createdPlayer)
                        {
                            menuOption[0].Execute(this);
                        }
                        else
                        { Console.WriteLine("角色已建立"); }
                        break;
                    case 1:
                        if (!createdPlayer)
                        {
                            Console.WriteLine("請先建立角色！");
                            break;
                        }
                        if (enemyList.Count == 0)
                        {
                            Console.WriteLine("請先建立敵人！");
                            break;
                        }
                        menuOption[1].Execute(this);
                        break;
                    case 2:
                        if (!createdPlayer)
                        { Console.WriteLine("需要先建立角色！"); break; }
                        else
                            menuOption[2].Execute(this);
                        break;
                    case 3:
                        if (!createdPlayer)
                        { Console.WriteLine("需要先建立角色！"); break; }
                        menuOption[3].Execute(this);
                        break;
                    case 4:
                        menuOption[4].Execute(this);
                        break;
                    default:
                        Console.WriteLine("輸入錯誤，請重新輸入！");
                        break;
                }
            }
        }

        public void GameIntro()
        {
            bool leaveOption = false;
            while (!leaveOption)
            {
                Console.WriteLine("請選擇遊戲模式：\n1.單人模式\n2.多人模式\n3.離開選單");
                int mode = int.Parse(Console.ReadLine());
                switch (mode)
                {
                    case 1:
                        duelOver = SinglePlayMode(playerList.First(), enemyList[random.Next(0, enemyList.Count)], duelOver);
                        break;
                    case 2:
                        List<EnemyData> enemyPlayer = new List<EnemyData>();
                        Console.WriteLine("請輸入同時猜拳人數(至少3人)：");
                        if (int.TryParse(Console.ReadLine(), out int playerCount) && playerCount > 3)
                        {
                            if (playerCount - 1 > enemyList.Count)
                            { 
                                Console.WriteLine("對手數量不足！");
                                break;
                            }

                            var randomEnemy = enemyList
                                .OrderBy(x => Guid.NewGuid())
                                .Take(playerCount - 1)
                                .ToList();

                            enemyPlayer.AddRange(randomEnemy);

                            Console.WriteLine("參賽者：");
                            foreach (var enemies in enemyPlayer)
                            {
                                Console.WriteLine(enemies.ChrName);
                            }
                        }
                        duelOver = multiPlayMode(playerList.First(), enemyPlayer, duelOver);
                        break;
                    case 3:
                        leaveOption = true;
                        break;
                    default:
                        Console.WriteLine("輸入錯誤，請重新輸入！");
                        break;
                }
            }
        }

        bool SinglePlayMode(PlayerData player, EnemyData enemy, bool duelOver)
        {
            Console.WriteLine($"{player.ChrName}對上{enemy.ChrName}。");
            while (!duelOver)
            {
                bool playerValidHand = false;

                Console.WriteLine("請輸入出招：\n0.剪刀、1.石頭、2.布");
                int playerHand = int.Parse(Console.ReadLine());
                int enemyHand = Utility.RandomNumber(0, 2);
                duelOver = Role.SingleDuel(playerHand, new int[] { enemyHand }, player, enemy, IsWinner);
            }
            return false;
        }

        bool multiPlayMode(PlayerData player, List<EnemyData> enemies, bool duelOver)
        {

            while (!duelOver)
            {

                duelOver = Role.MultiDuel(player, enemies, duelOver);

            }
            return false;

        }


        public void ShowWinRank()
        {
            List<ChrData> allRankList = new List<ChrData>();
            Console.WriteLine("＝＝以下是排名＝＝");
            for (int i = 0; i < playerList.Count; i++)
            {
                allRankList.Add(playerList[i]);
            }
            for (int i = 0; i < enemyList.Count; i++)
            {
                allRankList.Add(enemyList[i]);
            }

            allRankList.Sort((x, y) => y.Win.CompareTo(x.Win));
            for (int i = 0; i < allRankList.Count; i++)
            {
                allRankList[i].Rank = i + 1;
                Console.WriteLine($"第{allRankList[i].Rank}名：{allRankList[i].ChrName}，{allRankList[i].Win}勝{allRankList[i].Lose}敗");
            }
        }

        public static void AddFighterWin(ChrData fighter)
        {
            var validPlayer = playerList.Find(x => x.ChrName == fighter.ChrName);
            var validEnemy = enemyList.Find(x => x.ChrName == fighter.ChrName);

            if (validPlayer != null)
            { validPlayer.Win++; }
            else if (validEnemy != null)
            { validEnemy.Win++; }
        }

        public static void AddFighterLose(ChrData fighter)
        {
            var validPlayer = playerList.Find(x => x.ChrName == fighter.ChrName);
            var validEnemy = enemyList.Find(x => x.ChrName == fighter.ChrName);

            if (validPlayer != null)
            { validPlayer.Lose++; }
            else if (validEnemy != null)
            { validEnemy.Lose++; }
        }

        public void AddPlayerToList(PlayerData player)
        {
            playerList.Add(player);
        }

        public List<PlayerData> SetPlayerList()
        {
            return playerList;
        }

        public int GetEnemyCount()
        {
            return enemyList.Count;
        }

        public List<EnemyData> SetEnemyList()
        {
            return enemyList;
        }
        public void AddEnemyToList(EnemyData enemy)
        {
            enemyList.Add(enemy);
        }
    }

    public class Utility
    {
        private static Random random = new Random();
        public static int RandomNumber(int min, int max)
        {
            return random.Next(min, max + 1);
        }
    }
    //0:Scissors , 1:Rock , 2: Paper
    class Role
    {

        public static Dictionary<int, int> roleDict = new Dictionary<int, int>()
        {
             { 0,2}  //剪刀贏布
            , { 1,0} //石頭贏剪刀
            , { 2,1} //布贏石頭
        };
        public static Dictionary<int, string> handDict = new Dictionary<int, string>()
        {
            {0 ," 剪刀"},
            {1 , "石頭"},
            {2 , "布"}
        };
        public static bool SingleDuel(int playerHand, int[] enemyHand, PlayerData player, EnemyData enemy, bool win)
        {
            //(int player, int[] enemy) = TransformMark(playerHand, enemyHand);
            for (int i = 0; i < enemyHand.Length; i++)
            {
                if (roleDict[playerHand] == enemyHand[i])
                {
                    Console.WriteLine($"你出{handDict[playerHand]}贏對方{handDict[enemyHand[i]]}");
                    win = true;
                    CountWinLose(player, enemy, win);
                    return true;
                }
                else if (roleDict[enemyHand[i]] == playerHand)
                {
                    Console.WriteLine($"對方出{handDict[enemyHand[i]]}贏你的{handDict[playerHand]}");
                    win = false;
                    CountWinLose(player, enemy, win);
                    return true;

                }
                else
                {
                    Console.WriteLine($"平手，你出{handDict[playerHand]}對方出{handDict[enemyHand[i]]}");
                    return false;
                }
            }
            return false;
        }

        public static bool MultiDuel(PlayerData player, List<EnemyData> enemies, bool duelOver)
        {
            List<ChrData> allFighter = new List<ChrData>();
            allFighter.Add(player);
            foreach (var enemy in enemies)
            { allFighter.Add(enemy); }
            List<int> losePlayersValidList = new List<int>();
            int[] enemyHand = new int[enemies.Count];
            int[] allHand = new int[0]; ;
            for (int i = 0; i < allFighter.Count; i++)
            {
                losePlayersValidList.Add(0);
            }

            while (losePlayersValidList.Count(x => x == 0) > 1)
            {
                for (int i = 0; i < enemyHand.Length; i++)
                {
                    if (losePlayersValidList[i + 1] == 1)
                    { enemyHand[i] = -1; }
                    else
                    { enemyHand[i] = Utility.RandomNumber(0, 2); }
                }

                if ((allFighter.Find(x => x.ChrName == player.ChrName) == player) && losePlayersValidList[0] != 1)
                {
                    Console.WriteLine("請輸入出招：\n0.剪刀、1.石頭、2.布");
                    int playerHand;
                    while (!int.TryParse(Console.ReadLine(), out playerHand) || playerHand < 0 || playerHand > 2)
                    {
                        Console.WriteLine("請輸入有效出招：0.剪刀、1.石頭、2.布");
                    }
                    allHand = new int[] { playerHand }.Concat(enemyHand).ToArray();

                }
                else if (losePlayersValidList[0] == 1)
                {
                    allHand = enemyHand.ToArray();
                }

                var allHandCheck = allHand
                    .Where(h => h != -1)       // 忽略掉 -1
                    .Distinct()                // 剩下的手勢去除重複
                    .ToArray();
                if (allHandCheck.Length != 2)
                {
                    Console.WriteLine($"參賽者出拳狀態：{string.Join(", ", allHand)}");
                    Console.WriteLine("平手，無人出局，進入下一輪");
                    continue;
                }

                int winHand = roleDict[allHandCheck[0]] ==
                    allHandCheck[1] ? allHandCheck[1] : allHandCheck[0];

                for (int i = 0; i < allHand.Length ; i++)
                {
                    if (losePlayersValidList[i] == 1 || allHand[i] == -1)
                        continue;

                    if (roleDict[allHand[i]] == winHand)
                    {
                        Console.WriteLine($"{allFighter[i].ChrName}出{handDict[allHand[i]]}贏了");

                    }
                    else if (roleDict[allHand[i]] != winHand)
                    {
                        Game.AddFighterLose(allFighter[i]);

                        Console.WriteLine($"{allFighter[i].ChrName}出{handDict[allHand[i]]}輸了");

                        losePlayersValidList[i] = 1;
                    }
                }
                Console.WriteLine($"目前存活玩家數：{losePlayersValidList.Count(x => x == 0)}");
                Console.WriteLine($"玩家輸贏狀態：{string.Join(", ", losePlayersValidList)}");
                if (losePlayersValidList.Count(x => x == 0) == 1)
                {
                    var winnerIndex = losePlayersValidList.FindIndex(x => x == 0);
                    Console.WriteLine($"{allFighter[winnerIndex].ChrName}獲得了勝利！");
                    Game.AddFighterWin(allFighter[winnerIndex]);
                    return true; // 直接退出
                }
            }
            return false;
        }

        public static void CountWinLose(PlayerData player, EnemyData enemy, bool win)
        {
            if (!win)
            {
                player.Lose++;
                enemy.Win++;
            }
            else
            {
                player.Win++;
                enemy.Lose++;
            }

        }

        static (int player, int[] enemy) TransformMark(string playerHand, string[] enemyHand)
        {
            int player = 0;
            int[] enemy = new int[enemyHand.Length];

            switch (playerHand)
            {
                case "Rock":
                    player = 1;
                    break;
                case "Paper":
                    player = 2;
                    break;
                case "Scissors":
                    player = 3;
                    break;
            }

            for (int i = 0; i < enemyHand.Length; i++)
            {

                switch (enemyHand[i])
                {
                    case "Rock":
                        enemy[i] = 1;
                        break;
                    case "Paper":
                        enemy[i] = 2;
                        break;
                    case "Scissors":
                        enemy[i] = 3;
                        break;
                }
            }
            return (player, enemy);
        }


    }


    interface IMenuOption
    {
        void Execute(Game menu);
    }

    public class ICreatePlayerOption : IMenuOption
    {
        public void Execute(Game menu)
        {
            menu.createdPlayer = menu.createManger.CreatePlayer(menu);

        }
    }

    public class IStartGameOption : IMenuOption
    {
        public void Execute(Game menu)
        {
            menu.GameIntro();

        }

    }

    public class ICreateEnemyOption : IMenuOption
    {
        public void Execute(Game menu)
        {
            for (int i = 0; i < 10; i++)
            {
                menu.createManger.CreateEnemy(menu);
            }
        }
    }

    public class IShowWinRank : IMenuOption
    {
        public void Execute(Game menu)
        {
            menu.ShowWinRank();
        }

    }

    public class IExitOption : IMenuOption
    {
        public void Execute(Game menu)
        {
            Console.WriteLine("歡迎再度進入遊戲");
            Environment.Exit(0);
        }
    }

    public class CreateManger
    {

        public bool CreatePlayer(Game menu)
        {
            PlayerData player = new PlayerData("", 0, "", 0, 0, 0);
            Console.WriteLine("請輸入玩家名稱：");
            player.ChrName = Console.ReadLine();
            Console.WriteLine("請輸入玩家年齡：");
            player.Age = int.Parse(Console.ReadLine());
            Console.WriteLine("請輸入玩家性別：0代表女性，1代表男性：");
            string genderIutPut = Console.ReadLine();
            if (int.TryParse(genderIutPut, out int genderOutPut))
            {
                if (genderOutPut == 0)
                {
                    player.Gender = "女";
                }
                else if (genderOutPut == 1)
                {
                    player.Gender = "男";
                }
                else
                {
                    Console.WriteLine("輸入錯誤，請重新建立角色！");
                    player = new PlayerData("", 0, "", 0, 0, 0);
                    CreatePlayer(menu);
                }
                Console.WriteLine("已成功建立玩家！" +
                    $"角色名稱為：{player.ChrName}" +
                    $"年齡為：{player.Age}" +
                    $"性別為：{player.Gender}");

                menu.AddPlayerToList(player);
                menu.SetPlayerList();
                return true;

            }
            else
            {
                Console.WriteLine("輸入錯誤，請重新輸入！");
                return false;
            }
        }
        public void CreateEnemy(Game menu)
        {
            EnemyData enemy = new EnemyData("", 0, "", 0, 0, 0);
            //這裡我想編寫一個語句，依照enemyList長度決定敵人名字賦予編號

            enemy.ChrName = "敵人" + menu.GetEnemyCount();
            enemy.Age = Utility.RandomNumber(13, 90);
            enemy.Gender = Utility.RandomNumber(0, 1) == 1 ? "男" : "女";
            menu.SetEnemyList();
            menu.AddEnemyToList(enemy);

        }


    }
    public abstract class ChrData
    {
        private string chrName;
        private int age;
        private string gender;
        private int win;
        private int loss;
        private int rank = 0;

        public string ChrName { get { return chrName; } set { chrName = value; } }
        public int Age { get { return age; } set { age = value; } }
        public string Gender { get { return gender; } set { gender = value; } }
        public int Win { get { return win; } set { win = value; } }
        public int Lose { get { return loss; } set { loss = value; } }
        public int Rank { get { return rank; } set { rank = value; } }

    }
    public class PlayerData : ChrData
    {
        public PlayerData(string chrName, int age, string gender, int win, int lose, int rank)
        {
            this.ChrName = chrName;
            this.Age = age;
            this.Gender = gender;
            this.Win = win;
            this.Lose = lose;
            this.Rank = rank;
        }
    }
    public class EnemyData : ChrData
    {
        public EnemyData(string chrName, int age, string gender, int win, int lose, int rank)
        {
            this.ChrName = chrName;
            this.Age = age;
            this.Gender = gender;
            this.Win = win;
            this.Lose = lose;
            this.Rank = rank;
        }

    }
}