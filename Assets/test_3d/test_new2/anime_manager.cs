using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class anime_manager : MonoBehaviour
{
    public Animator anime;

    float temp = 1;

    //getcurrentanimatorstateinfo = �ִϸ��̼� ���̾�
    //normalizedtime = ���̾��� �ð��� ��Ÿ�� 0 -> 1
    void Update()
    {
        if(anime.GetCurrentAnimatorStateInfo(1).normalizedTime > 0.7f)
        {
            if(temp > 0)
            {
                temp -= Time.deltaTime;
            }
            anime.SetLayerWeight(1, temp); // 1��° ���̾ 0���� �ٲ� �� ���ǹ� ���� ��Ȱ��ȭ
        }
    }
}
