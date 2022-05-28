using System;
using System.Threading;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;
using Cinemachine;
using System.Collections;

public enum BUFFUI { InPrison1, InPrison2, Stay1, Slow1, Slow2, Slow3, TurnAround }
public enum BUFF { Prison, Stay, Slow }//TurnAround通过人物方向判断而不放在BUFF中
public enum CARD { Stay, Slow, TurnAround }

public class MonopolyGame : MonoBehaviour
{
    //todo:改为消息列表，允许多个消息
    private string SystemMessageText = "";

    //系统消息文本

    private Text RoundText = null;
    private int Round = 1;

    public static int BuffLogoNum = 4;//当前存在四种BuffLogo
    private int CardNum = 3;
    private int BuffNum = 3;//记录需要每回合更新的Buff状态数量(不包括TurnAround)

    //回合总数

    public static bool GameBegin = false;

    public static int Player = 0; //表示玩家选择的形象
    private string[] Name = new string[3];

    //Ready变量
    public static bool RollReady = false;
    private bool GameReady = false;
    private bool MoveReady = false;
    private bool UpdateDataReady = false;
    private bool NextTurnReady = false;

    //表状态的布尔变量
    public static bool PurchaseConfirm = false;
    public static int CardTarget = -1; //目标
    public static int CardUsed = -1; //使用的道具卡
    private bool Pause = false;

    //Done变量（注意根据需要调整初始化的值）
    private bool RollDone = false;
    private bool UpdateDataDone = false;
    private bool MoveDone = false;

    //Doing变量
    private bool Moving = false;
    private bool Rolling = false;
    private bool Updating = false;
    private bool Purchasing = false;

    protected float RollTime = 0;
    protected float IntervalTime = 0;

    public static float CheckTime = 4F;
    public static float SetTime = 2F;

    public static ArrayList CardType = new ArrayList();
    public static ArrayList Card = new ArrayList();
    private int[] Money = new int[3];
    private int[] MoneyResult = new int[3]; //金钱变化的目标数

    private int[] mp = new int[32];

    //建立块对其类型的映射，0表示普通方块，1表示随机事件方块，2345分别表示四种特殊方块
    private int[] Col = new int[32]; //保存每个块的颜色
    private int[] Level = new int[32]; //建筑等级

    private ArrayList Belongings = new ArrayList();

    //游戏对象、实体
    private GameObject[] BlockArr = new GameObject[32];
    private GameObject[] Character = new GameObject[3];
    private NavMeshAgent[] agent = new NavMeshAgent[3];
    private Material[] ChangeColor = new Material[3];
    public CinemachineVirtualCamera[] camera = new CinemachineVirtualCamera[4];

    private GameObject CardBag;

    private GameObject Map = null;

    //UI
    private GameObject Canvas = null;
    private GameObject[] StatusBar = new GameObject[3];
    private GameObject[] MoneyUI = new GameObject[3];
    private GameObject[] NameUI = new GameObject[3];
    private GameObject[] CardUI = new GameObject[3];
    private Texture2D[] Photo = new Texture2D[3];
    private Texture2D[] CardPicture = new Texture2D[3];
    public static GameObject[] BuffBar = new GameObject[3];
    public static Texture2D[] BuffPicture = new Texture2D[10];

    private GameObject SystemMessage = null;
    public static GameObject GoButton = null;

    //
    private int UpdateSpeed = 1;

    private bool gameOver = false;

    public static int Turn = 0; //记录当前是哪个人物的回合

    private int[] PosID = new int[3]; //三个角色的当前所处块的ID
    private Vector3[] BlockPos = new Vector3[32]; //记录每个块的位置
    private int[] AIPlayeID = new int[3];
    public static int[] Dir = new int[3]; //表示三名角色的方向
    public static int[,] Buff = new int[3, 10]; //注意二维写法，两个维度分别表示人物及buff类型

    private int CurBuff = -1; //表示当前角色buff状态

    private bool CheckPastStartPoint = false;
    private bool CheckInPrison = false;

    public static bool NextTurnCheckCard = true;
    private int LastPos;//记录人物每次移动的起点，用于判断是否经过起始出发点

    private void Awake() //用于获取所有组件、实体等
    {
        Map = GameObject.Find("Map_0");
        if (Map == null)
            Debug.Log("Map not found!");

        for (int i = 0; i < 3; i++)
        {
            string name = "Character_" + i.ToString();
            Character[i] = GameObject.Find(name);
            agent[i] = Character[i].GetComponent<NavMeshAgent>();
        }

        for (int i = 0; i < 32; i++)
        {
            string name = "Block_" + i.ToString();
            BlockArr[i] = Map.transform.Find(name).gameObject;
        }

        for (int i = 0; i < 32; i++)
            BlockPos[i] = BlockArr[i].transform.Find("CenterPoint").gameObject.transform.position;

        ChangeColor[0] = (Material)Resources.Load("Material/Block_Blue");
        ChangeColor[1] = (Material)Resources.Load("Material/Block_Red");
        ChangeColor[2] = (Material)Resources.Load("Material/Block_Purple");

        //UI
        Canvas = GameObject.Find("Canvas");
        if (Canvas == null)
            Debug.Log("canvas not found!");

        SystemMessage = Canvas.transform.Find("SystemMessage").gameObject;

        GoButton = Canvas.transform.Find("GoButton").gameObject;
        GoButton.GetComponent<Button>().interactable = false; //初始化之前设置GO按钮为不可用

        //获取人物头像素材
        Photo[0] = (Texture2D)Resources.Load("DIY/Photo/Man");
        Photo[1] = (Texture2D)Resources.Load("DIY/Photo/Wolf");
        Photo[2] = (Texture2D)Resources.Load("DIY/Photo/Zombie");

        //获取道具卡素材
        CardBag = Canvas.transform.Find("CardBag").gameObject;
        CardPicture[(int)CARD.Stay] = (Texture2D)Resources.Load("DIY/Card/Stay");
        CardPicture[(int)CARD.Slow] = (Texture2D)Resources.Load("DIY/Card/Slow");
        CardPicture[(int)CARD.TurnAround] = (Texture2D)Resources.Load("DIY/Card/TurnAround");

        //获得BuffLogo素材
        BuffPicture[(int)BUFFUI.InPrison1] = (Texture2D)Resources.Load("DIY/BuffLogo/InPrison1");
        BuffPicture[(int)BUFFUI.InPrison2] = (Texture2D)Resources.Load("DIY/BuffLogo/InPrison2");
        BuffPicture[(int)BUFFUI.Stay1] = (Texture2D)Resources.Load("DIY/BuffLogo/Stay1");
        BuffPicture[(int)BUFFUI.Slow1] = (Texture2D)Resources.Load("DIY/BuffLogo/Slow1");
        BuffPicture[(int)BUFFUI.Slow2] = (Texture2D)Resources.Load("DIY/BuffLogo/Slow2");
        BuffPicture[(int)BUFFUI.Slow3] = (Texture2D)Resources.Load("DIY/BuffLogo/Slow3");
        BuffPicture[(int)BUFFUI.TurnAround] = (Texture2D)Resources.Load("DIY/BuffLogo/TurnAround");

        RoundText = GameObject.Find("RoundText").GetComponent<Text>();
    }

    private void Init() //初始化窗口关闭后，进行初始化操作
    {
        Player = UIEvent.CharacterSelect;
        Turn = Player; //每次从玩家开始

        //为三个角色分配名字
        int cnt = 0;
        for (int i = 0; i < 3; i++)
        {
            if (i == Player)
                Name[i] = UIEvent.PlayerName.Length == 0 ? "Player" : UIEvent.PlayerName;
            else
                Name[i] = "AI_Player" + (++cnt).ToString();
        }

        //角色选定之后再按顺序获取UI组件
        for (int i = 0, cur = Player; i < 3; i++, cur = (cur + 1) % 3)
        {
            string name = "StatusBar_" + i.ToString();
            StatusBar[cur] = Canvas.transform.Find(name).gameObject;
            MoneyUI[cur] = StatusBar[cur].transform.Find("Money").gameObject;
            NameUI[cur] = StatusBar[cur].transform.Find("Name").gameObject;
            CardUI[cur] = StatusBar[cur].transform.Find("Card").gameObject;
            BuffBar[cur] = StatusBar[cur].transform.Find("BuffBar").gameObject;
            if (NameUI[cur] == null || MoneyUI[cur] == null || CardUI[cur] == null)
                Debug.Log("UI not found!");
        }

        //初始化UI
        for (int i = 0, cur = Player; i < 3; i++, cur = (cur + 1) % 3)
        {
            if (i == Player)
                CardUI[i].GetComponent<Text>().text = "3";
            else
                CardUI[i].GetComponent<Text>().text = "0";

            MoneyUI[i].GetComponent<Text>().text = Money[i].ToString();
            NameUI[i].GetComponent<Text>().text = Name[i];
        }

        GoButton.GetComponent<Button>().interactable = true; //启用GO按钮和道具卡
        for (int i = 0; i < Card.Count; i++)
        {
            GameObject obj = (GameObject)Card[i];
            obj.GetComponent<Button>().interactable = true;
        }

        //状态栏头像
        for (int i = 0, cur = Player; i < 3; i++, cur = (cur + 1) % 3)
        {
            StatusBar[cur].transform.Find("Photo").GetComponent<Image>().sprite = Sprite.Create(
                Photo[cur],
                new Rect(0, 0, Photo[cur].width, Photo[cur].height),
                Vector2.zero
            );
            StatusBar[cur].transform.Find("Photo").GetComponent<Image>().color = new Color(
                255,
                255,
                255,
                255
            );
        }
    }

    private void Start() //用于初始化游戏数据及UI
    {
        UIEvent.InitWindowShow = true; //显示初始化窗口

        //初始化游戏数据
        Money[0] = Money[1] = Money[2] = 5000;
        MoneyResult[0] = MoneyResult[1] = MoneyResult[2] = 5000;

        PosID[0] = PosID[1] = PosID[2] = 0;
        Dir[0] = Dir[1] = Dir[2] = 1;

        Round = 1;

        mp[4] = mp[12] = mp[20] = mp[28] = 1;
        mp[0] = 2;
        mp[8] = 3;
        mp[16] = 4;
        mp[24] = 5;

        for (int i = 0; i < 32; i++)
            Col[i] = -1;

        //初始化相机设置
        camera[0].gameObject.SetActive(false);
        camera[1].gameObject.SetActive(false);
        camera[2].gameObject.SetActive(false);
        camera[3].gameObject.SetActive(true);

        //给AI玩家分配ID
        int cnt = 0;
        for (int i = 0; i < 3; i++)
        {
            if (i == Player)
                continue;
            else
                AIPlayeID[i] = ++cnt;
        }

        System.Random rand = new System.Random();
        int random = rand.Next();
        //随机为玩家获得三张不同的道具卡
        for (int i = 0; i < 3; i++)
        {
            int sel = (random + i) % CardNum;

            GameObject obj = new GameObject(
                "Card_" + i.ToString(),
                typeof(RectTransform),
                typeof(Button),
                typeof(Image)
            );
            obj.GetComponent<RectTransform>().sizeDelta = new Vector2(160, 200);
            obj.transform.SetParent(CardBag.transform);
            obj.GetComponent<Image>().sprite = Sprite.Create(
                CardPicture[sel],
                new Rect(0, 0, CardPicture[sel].width, CardPicture[sel].height),
                Vector2.zero
            );
            obj.GetComponent<Button>().onClick.AddListener(CardController.UseCard);

            CardType.Add(sel);
            Card.Add(obj);
        }

        //保存每个角色拥有的财产
        Belongings.Add(new ArrayList());
        Belongings.Add(new ArrayList());
        Belongings.Add(new ArrayList());
    }

    private void Roll()
    {
        if (Buff[Turn, (int)BUFF.Stay] > 0) //停留卡
        {
            SystemMessageText = "玩家" + Name[Turn] + "受停留卡影响，本次回合无法移动";
            SystemMessage.GetComponent<Text>().text = SystemMessageText;
            UIEvent.SystemMessageShow = true;
            UpdateDataReady = true;//
        }
        else if (Buff[Turn, (int)BUFF.Slow] > 0) //乌龟卡
        {
            SystemMessageText = "玩家" + Name[Turn] + "受乌龟卡影响，本次回合只能前进一步";
            SystemMessage.GetComponent<Text>().text = SystemMessageText;
            UIEvent.SystemMessageShow = true;
            MoveReady = true;
        }
        else
        {
            RollDice.Roll();
            RollTime = 0;
            Rolling = true;
        }
        //判断是否在监狱或被使用停留卡

        RollDone = true;
    }

    private void Move()
    {
        LastPos = PosID[Turn];//记录出发点

        camera[3].gameObject.SetActive(false);
        camera[Turn].gameObject.SetActive(true);

        int value = RollDice.GetValue();
        if (Buff[Turn, (int)BUFF.Slow] > 0) value = 1;

        PosID[Turn] = (PosID[Turn] + Dir[Turn] * value + 32) % 32; //考虑运动方向
        agent[Turn].SetDestination(BlockPos[PosID[Turn]]);

        Moving = true;
        MoveDone = true;
    }

    private void UpdateData()
    {
        int pos = PosID[Turn];
        //更改MoneyResult而不是Money

        switch (mp[pos])
        {
            case 0: //普通方块
                if (Col[pos] == -1)
                {
                    if (Turn == Player)
                    {
                        if (Money[Player] > 100) //钱数满足
                        {
                            PurchaseConfirm = false;
                            Purchasing = true;
                            UIEvent.PurchaseWindowShow = true; //弹出购买窗口
                            UpdateDataDone = true;
                            return; //在此返回，不能将Updating设置为true
                        }
                        else
                        {
                            //否则系统消息提示金钱不足，不弹出购买窗口
                            SystemMessageText = "金钱不足，无法购买";
                            SystemMessage.GetComponent<Text>().text = SystemMessageText;
                            UIEvent.SystemMessageShow = true;
                        }
                    }
                    else
                    {
                        if (Money[Turn] > 100)
                        {
                            MoneyResult[Turn] -= 100;
                            Col[pos] = Turn;
                            Level[pos] = 1;
                            BlockArr[pos].transform
                                .Find("BlockColor")
                                .gameObject.GetComponent<Renderer>()
                                .material = ChangeColor[Turn];
                        }
                    }
                }
                else if (Col[pos] == Turn)
                {
                    //如果升级,显示建筑升级动画
                    Level[pos] = Mathf.Min(4, Level[pos] + 1);
                }
                else
                {
                    int sum = Mathf.Min(Level[pos] * 500, Money[Turn]);
                    MoneyResult[Turn] -= sum;
                    MoneyResult[Col[pos]] += sum;
                }
                break;

            case 1: //随机事件方块
                System.Random rand = new System.Random();
                int random = rand.Next();
                int mode = random % 2; //表示获得、失去金钱
                int sign = mode == 0 ? -1 : 1;
                int select = random % 3;
                int num = rand.Next(100, Convert.ToInt32(1.0 * Money[select] / 3));
                MoneyResult[select] += sign * num;

                //输出随机事件文本
                SystemMessageText =
                    "随机事件触发，玩家" + Name[select] + (mode == 0 ? "失去" : "获得") + num.ToString() + "$";
                SystemMessage.GetComponent<Text>().text = SystemMessageText;
                UIEvent.SystemMessageShow = true;

                break;

            case 2: //起点
                break;
            //起点效果，玩家名字+随机一处建筑等级提升

            case 3: //飞机场
                //TODO:可以去往任意一个方块(传送效果)
                break;

            case 4: //监狱
                Buff[Turn, 0] = 3; //回合结束buff会减一，因此应置Buff[Turn,0]为3
                SystemMessageText = "玩家" + Name[Turn] + "被关进监狱，2回合内无法进行操作";
                SystemMessage.GetComponent<Text>().text = SystemMessageText;
                UIEvent.SystemMessageShow = true;
                break;

            case 5: //地价翻倍
                SystemMessageText = "地价翻倍效果，玩家" + Name[Turn] + "随机一处地价翻倍";
                SystemMessage.GetComponent<Text>().text = SystemMessageText;
                UIEvent.SystemMessageShow = true;

                //TODO:随机一处地价翻倍
                break;

            default:
                break;
        }

        //更新状态
        UpdateDataDone = true;
        Updating = true;

        //自适应更新速度，保证更新时间在合理范围
        float MaxDif = 0;
        for (int i = 0; i < 3; i++)
            MaxDif = Mathf.Max(MaxDif, Mathf.Abs(Money[i] - MoneyResult[i]));
        UpdateSpeed = Convert.ToInt32(Mathf.Ceil(MaxDif / 180f));
    }

    private void GameOver()
    {
        //结算、玩家胜利/失败
    }

    private bool UpdateOver()
    {
        return (
            Money[0] == MoneyResult[0] && Money[1] == MoneyResult[1] && Money[2] == MoneyResult[2]
        );
    }


    private void Update() //考虑bool状态变量的优化、简化
    {
        if (GameReady == false && UIEvent.InitFinish)
        {
            Init();
            GameReady = true;
        }

        if (GameReady == false)
            return;

        //0停留卡、1乌龟卡、2转向卡
        if (CardUsed != -1 && CardTarget != -1) //道具卡使用
        {
            if (CardUsed == (int)CARD.Stay)
                Buff[CardTarget, (int)BUFF.Stay] = 1;
            else if (CardUsed == (int)CARD.Slow)
                Buff[CardTarget, (int)BUFF.Slow] = 3;
            else if (CardUsed == (int)CARD.TurnAround)
                Dir[CardTarget] = -Dir[CardTarget]; //转向

            MonopolyGame.UpdateBuffBar(MonopolyGame.CardTarget);//更新状态栏
            CardUsed = CardTarget = -1;

            CardUI[Player].GetComponent<Text>().text = Card.Count.ToString();//更新道具卡数量显示
        }

        //在监狱中不能进行任何操作
        if (CheckInPrison)
        {
            SystemMessageText = "玩家" + Name[Turn] + "距离出狱还有" + (Buff[Turn, 0]).ToString() + "个回合";
            SystemMessage.GetComponent<Text>().text = SystemMessageText;
            UIEvent.SystemMessageShow = true;
            Pause = true;
            IntervalTime = 1; //停顿一秒
            NextTurnReady = true; //直接进入下一个回合
            CurBuff = -1;
        }

        if (Turn == Player)
        {
            if (RollReady && !RollDone)
                Roll();
        }
        else if (Pause) //AI玩家回合开始前暂停一小段时间
        {
            IntervalTime += Time.deltaTime;
            if (IntervalTime > SetTime)
                Pause = false;
            else
                return;
        }
        else if (!RollDone)
            Roll();

        if (Rolling)
        {
            RollTime += Time.deltaTime;
            if (RollTime > CheckTime)
            {
                Rolling = false;
                MoveReady = true;
            }
        }

        if (MoveReady && !MoveDone)
            Move();

        if (Moving)
        {
            if (LastPos != 0)//不能从起始出发点开始移动
            {
                if ((agent[Turn].nextPosition - BlockPos[0]).sqrMagnitude < 270f) //实时判断是否经过起点
                {
                    
                    SystemMessageText = "经过起点，玩家" + Name[Turn] + "的随机一处建筑等级提升";
                    SystemMessage.GetComponent<Text>().text = SystemMessageText;
                    UIEvent.SystemMessageShow = true;

                    CheckPastStartPoint = true;

                    //TODO:随机选择一处建筑物升级
                }
            }


            if (
                Mathf.Abs(agent[Turn].destination.x - agent[Turn].nextPosition.x) < 0.05f
                && Mathf.Abs(agent[Turn].destination.y - agent[Turn].nextPosition.y) < 0.05f
                && Mathf.Abs(agent[Turn].destination.z - agent[Turn].nextPosition.z) < 0.05f
            )
            {
                UpdateDataReady = true;
                Moving = false;

                camera[Turn].gameObject.SetActive(false);
                camera[3].gameObject.SetActive(true);
            }
        }

        if (UpdateDataReady && !UpdateDataDone)
            UpdateData();

        if (Purchasing && UIEvent.PurchaseFinish)
        {
            if (PurchaseConfirm) //确认购买
            {
                int pos = PosID[Turn]; //player
                MoneyResult[Turn] -= 100;
                Col[pos] = Turn;
                Level[pos] = 1;
                BlockArr[pos].transform
                    .Find("BlockColor")
                    .gameObject.GetComponent<Renderer>()
                    .material = ChangeColor[Turn];
            }
            Purchasing = false;
            Updating = true;
            UIEvent.PurchaseFinish = false;
        }

        if (Updating)
        {
            //每一帧进行一定量的修改(取决于UpdateSpeed)
            for (int i = 0; i < 3; i++)
            {
                int sign = MoneyResult[i] > Money[i] ? 1 : -1;
                Money[i] += sign * Math.Min(UpdateSpeed, Math.Abs(MoneyResult[i] - Money[i]));
            }

            //更新UI
            for (int i = 0; i < 3; i++)
                MoneyUI[i].GetComponent<Text>().text = Money[i].ToString();

            if (UpdateOver())
            {
                Updating = false;
                NextTurnReady = true;
            }
        }

        if (gameover)
            GameOver();

        //下一回合
        if (NextTurnReady)
        {
            //更新当前回合角色的buff状态
            for (int i = 0; i < BuffNum; i++)
                if (Buff[Turn, i] > 0)
                    Buff[Turn, i]--;

            UpdateBuffBar(Turn);//更新状态栏

            Turn = (Turn + 1) % 3;

            if (Turn == Player)
            {
                for (int i = 0; i < Card.Count; i++)
                {
                    GameObject obj = (GameObject)Card[i];
                    obj.GetComponent<Button>().interactable = true;
                }
                GoButton.GetComponent<Button>().interactable = true;
            }

            //判断下一回合的角色是否在监狱中
            CheckInPrison = false;
            if (Buff[Turn, (int)BUFF.Prison] > 0)
                CheckInPrison = true;

            RollReady = MoveReady = UpdateDataReady = NextTurnReady = false;
            RollDone = MoveDone = UpdateDataDone = false;

            if (Turn == Player)
            {
                Round++;
                RoundText.text = "Round" + Round.ToString();
            }
            else
            {
                Pause = true;
                IntervalTime = 0;
            }
        }


    }

    private bool gameover
    {
        get
        {
            return Round > 20 || Money[Player] <= 0 || (Convert.ToInt32(Money[0] <= 0) + Convert.ToInt32(Money[1] <= 0) + Convert.ToInt32(Money[2] <= 0) == 2); //第三个条件表示两个AI破产    
        }
    }

    //用于更新BUFF状态栏的函数
    public static void UpdateBuffBar(int Target)
    {
        int count = BuffBar[Target].transform.childCount;
        for (int i = 0; i < count; i++) Destroy(BuffBar[Target].transform.GetChild(i).gameObject);

        //监狱
        if (Buff[Target, (int)BUFF.Prison] > 0)
        {
            GameObject obj = new GameObject("BuffLogo", typeof(Image));
            obj.transform.SetParent(BuffBar[Target].transform);

            Texture2D img = BuffPicture[Buff[Target, (int)BUFF.Prison] == 1 ? (int)BUFFUI.InPrison1 : (int)BUFFUI.InPrison2];
            obj.GetComponent<Image>().sprite = Sprite.Create(img, new Rect(0, 0, img.width, img.height), Vector2.zero);
        }

        //停留
        if (Buff[Target, (int)BUFF.Stay] > 0)
        {
            GameObject obj = new GameObject("BuffLogo", typeof(Image));
            obj.transform.SetParent(BuffBar[Target].transform);

            Texture2D img = BuffPicture[(int)BUFFUI.Stay1];
            obj.GetComponent<Image>().sprite = Sprite.Create(img, new Rect(0, 0, img.width, img.height), Vector2.zero);
        }

        //乌龟
        if (Buff[Target, (int)BUFF.Slow] > 0)
        {
            GameObject obj = new GameObject("BuffLogo", typeof(Image));
            obj.transform.SetParent(BuffBar[Target].transform);

            Texture2D img = null;
            if (Buff[Target, (int)BUFF.Slow] == 1) img = BuffPicture[(int)BUFFUI.Slow1];
            else if (Buff[Target, (int)BUFF.Slow] == 2) img = BuffPicture[(int)BUFFUI.Slow2];
            else if (Buff[Target, (int)BUFF.Slow] == 3) img = BuffPicture[(int)BUFFUI.Slow3];

            obj.GetComponent<Image>().sprite = Sprite.Create(img, new Rect(0, 0, img.width, img.height), Vector2.zero);
        }

        //转向
        if (Dir[Target] == -1)
        {
            GameObject obj = new GameObject("BuffLogo", typeof(Image));
            obj.transform.SetParent(BuffBar[Target].transform);

            Texture2D img = BuffPicture[(int)BUFFUI.TurnAround];
            obj.GetComponent<Image>().sprite = Sprite.Create(img, new Rect(0, 0, img.width, img.height), Vector2.zero);
        }
    }
}
