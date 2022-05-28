using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class RollDice : MonoBehaviour
{
    public static float RollSpeed = 0.25F;

    public static bool Rolling = false;

    protected float RollTime = 0;

    private static GameObject SpawnPoint = null;

    private static Object pf=null;

    private static ArrayList ToBeRolledQueue = new ArrayList();

    private static ArrayList AllDice = new ArrayList();

    private static ArrayList RollingQueue = new ArrayList();

    private void Awake()
    {
        pf = Resources.Load("Prefabs/Dice");

    }

    public static GameObject Prefab(Vector3 position, Vector3 rotation, Vector3 scale)
    {
        if (pf != null)
        {
            GameObject inst = (GameObject)GameObject.Instantiate(pf, Vector3.zero, Quaternion.identity);
            if (inst != null)
            {
                inst.transform.position = position;
                inst.transform.Rotate(rotation);
                inst.transform.localScale = scale;
                return inst;
            }
        }
        else Debug.Log("Prefab not found!");
        return null;
    }

    public static void Roll()
    {
        Clear();

        int cnt = 2;
        Rolling = true;

        for (int i = 0; i < cnt; i++)
        {
            string name = "SpawnPoint_" + (char)(i + '0');
            SpawnPoint = GameObject.Find(name);
            if (SpawnPoint == null) Debug.Log("SpawnPoint not found!");
            Vector3 spawnPoint = SpawnPoint.transform.position;

            //spawnPoint.x = spawnPoint.x - 1 + Random.value * 2;
            //spawnPoint.y = spawnPoint.y - 2 + Random.value * 2;

            GameObject dice = Prefab(spawnPoint, Vector3.zero, Vector3.one * 3);
            dice.transform.Rotate(new Vector3(Random.value * 360, Random.value * 360, Random.value * 360));
            dice.SetActive(false);

            RollingDice rollingDice = new RollingDice(dice, spawnPoint, Force());
            AllDice.Add(rollingDice);
            ToBeRolledQueue.Add(rollingDice);
        }

    }

    public static int GetValue()
    {
        int value = 0;
        for (int i = 0; i < AllDice.Count; i++)
        {
            RollingDice rollingDice = (RollingDice)AllDice[i];
            value += rollingDice.value;
            
        }
        return value;
    }

    public static void Clear()
    {
        for (int i = 0; i < AllDice.Count; i++) GameObject.Destroy(((RollingDice)AllDice[i]).gameObject);

        AllDice.Clear();
        RollingQueue.Clear();
        ToBeRolledQueue.Clear();

        Rolling = false;
    }

    private void Update()
    {
        if (Rolling)
        {
            RollTime += Time.deltaTime;
            if (ToBeRolledQueue.Count > 0 && RollTime > RollSpeed)
            {
                RollingDice rollingDice = (RollingDice)ToBeRolledQueue[0];
                GameObject dice = rollingDice.gameObject;
                dice.SetActive(true);
                dice.GetComponent<Rigidbody>().AddForce((Vector3)rollingDice.force, ForceMode.Impulse);
                dice.GetComponent<Rigidbody>().AddTorque(new Vector3(-50 * Random.value * dice.transform.localScale.magnitude, -50 * Random.value * dice.transform.localScale.magnitude, -50 * Random.value * dice.transform.localScale.magnitude), ForceMode.Impulse);
                RollingQueue.Add(rollingDice);
                ToBeRolledQueue.RemoveAt(0);
                RollTime = 0;
            }
            else
            {
                if (ToBeRolledQueue.Count == 0)
                {
                    if (!IsRolling()) Rolling = false;
                    
                }
            }
        }
    }

    private bool IsRolling()
    {
        int i = 0;
        while (i < RollingQueue.Count)
        {
            RollingDice rollingDice = (RollingDice)RollingQueue[i];
            if (!rollingDice.rolling) RollingQueue.Remove(rollingDice);
            else i++;
        }
        return (RollingQueue.Count > 0);
    }

    private static Vector3 Force()
    {
        SpawnPoint = GameObject.Find("SpawnPoint_1");
        Vector3 RollTarget = new Vector3(5.5F, 2.5F, -3.5F );
        return Vector3.Lerp(SpawnPoint.transform.position, RollTarget, 1).normalized * -45;
    }
}


public class RollingDice
{
    public GameObject gameObject = null;
    public Dice dice;

    public Vector3 spawnPoint;
    public Vector3 force;

    public bool rolling
    { get { return dice.Rolling; } }

    public int value
    { get { return dice.Value; } }

    public RollingDice() { }

    public RollingDice(GameObject gameObject, Vector3 spawnPoint, Vector3 force)
    {
        this.gameObject = gameObject;
        this.spawnPoint = spawnPoint;
        this.force = force;
        dice = (Dice)gameObject.GetComponent(typeof(Dice)); //从gameObject中获得Dice的实例  
    }
}
