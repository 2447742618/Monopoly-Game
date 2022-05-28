using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dice : MonoBehaviour
{
    public int Value = 0;

    protected Vector3 LocalHitNormalized;

    protected float ValidMargin = 0.45F;

    public bool Rolling
    {
        get
        {
            return !(GetComponent<Rigidbody>().velocity.sqrMagnitude < 1e-5F && GetComponent<Rigidbody>().angularVelocity.sqrMagnitude < 1e-5F);
            //当骰子的动量和角动量的平方都小于一个极小值时，判定该骰子已静止
        }
    }

    protected bool LocalHit
    {
        get
        {
            Ray ray = new Ray(transform.position + (new Vector3(0, 2, 0) * transform.localScale.magnitude), Vector3.up * -1);
            //在骰子正上方创建一个方向竖直向下的射线
            RaycastHit hit = new RaycastHit();
            //创建射线投射信息
            if (GetComponent<Collider>().Raycast(ray, out hit, 3 * transform.localScale.magnitude))
            //raycast创建一个忽略其他所有colider的射线，如果返回true，则hit保存碰撞信息
            {
                LocalHitNormalized = transform.InverseTransformPoint(hit.point.x, hit.point.y, hit.point.z).normalized;
                //将 position 从世界空间变换到本地空间。
                return true;
            }
            else Debug.Log("No HitINFO!");
            return false;
        }
    }

    void GetValue()
    //获得当前骰子的值
    {
        Value = 0;
        float delta = 1;
        int side = 1;
        //从第一面开始判断
        Vector3 TestHitVector;
        do
        {
            TestHitVector = HitVector(side);
            if (TestHitVector != Vector3.zero)
            {
                if (Valid(LocalHitNormalized.x, TestHitVector.x) &&
                    Valid(LocalHitNormalized.y, TestHitVector.y) &&
                    Valid(LocalHitNormalized.z, TestHitVector.z))
                //判断是否在可接受范围内
                {
                    float _delta = Mathf.Abs(LocalHitNormalized.x - TestHitVector.x) + Mathf.Abs(LocalHitNormalized.y - TestHitVector.y) + Mathf.Abs(LocalHitNormalized.z - TestHitVector.z);
                    if (_delta < delta)//取偏差最小的一面
                    {
                        delta = _delta;
                        Value = side;
                    }
                }
            }
            side++;//遍历六面
        } while (TestHitVector != Vector3.zero);
    }

    private void Update()
    {
        if (!Rolling && LocalHit) GetValue();
    }

    protected bool Valid(float t, float v)
    //判断两个浮点数差值是否在可接受范围内
    {
        if (t > (v - ValidMargin) && t < (v + ValidMargin))
            return true; 
        else
            return false;
    }

    protected Vector3 HitVector(int side)
    {
        switch (side)
        {
            case 1: return new Vector3(0F, 0F, 1F);
            case 2: return new Vector3(0F, -1F, 0F);
            case 3: return new Vector3(-1F, 0F, 0F);
            case 4: return new Vector3(1F, 0F, 0F);
            case 5: return new Vector3(0F, 1F, 0F);
            case 6: return new Vector3(0F, 0F, -1F);
        }
        return Vector3.zero;
    }
}
