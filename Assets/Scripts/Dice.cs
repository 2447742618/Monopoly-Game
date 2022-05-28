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
            //�����ӵĶ����ͽǶ�����ƽ����С��һ����Сֵʱ���ж��������Ѿ�ֹ
        }
    }

    protected bool LocalHit
    {
        get
        {
            Ray ray = new Ray(transform.position + (new Vector3(0, 2, 0) * transform.localScale.magnitude), Vector3.up * -1);
            //���������Ϸ�����һ��������ֱ���µ�����
            RaycastHit hit = new RaycastHit();
            //��������Ͷ����Ϣ
            if (GetComponent<Collider>().Raycast(ray, out hit, 3 * transform.localScale.magnitude))
            //raycast����һ��������������colider�����ߣ��������true����hit������ײ��Ϣ
            {
                LocalHitNormalized = transform.InverseTransformPoint(hit.point.x, hit.point.y, hit.point.z).normalized;
                //�� position ������ռ�任�����ؿռ䡣
                return true;
            }
            else Debug.Log("No HitINFO!");
            return false;
        }
    }

    void GetValue()
    //��õ�ǰ���ӵ�ֵ
    {
        Value = 0;
        float delta = 1;
        int side = 1;
        //�ӵ�һ�濪ʼ�ж�
        Vector3 TestHitVector;
        do
        {
            TestHitVector = HitVector(side);
            if (TestHitVector != Vector3.zero)
            {
                if (Valid(LocalHitNormalized.x, TestHitVector.x) &&
                    Valid(LocalHitNormalized.y, TestHitVector.y) &&
                    Valid(LocalHitNormalized.z, TestHitVector.z))
                //�ж��Ƿ��ڿɽ��ܷ�Χ��
                {
                    float _delta = Mathf.Abs(LocalHitNormalized.x - TestHitVector.x) + Mathf.Abs(LocalHitNormalized.y - TestHitVector.y) + Mathf.Abs(LocalHitNormalized.z - TestHitVector.z);
                    if (_delta < delta)//ȡƫ����С��һ��
                    {
                        delta = _delta;
                        Value = side;
                    }
                }
            }
            side++;//��������
        } while (TestHitVector != Vector3.zero);
    }

    private void Update()
    {
        if (!Rolling && LocalHit) GetValue();
    }

    protected bool Valid(float t, float v)
    //�ж�������������ֵ�Ƿ��ڿɽ��ܷ�Χ��
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
