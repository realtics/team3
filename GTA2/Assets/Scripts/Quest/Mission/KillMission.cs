using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class KillMission : Quest
{
    [SerializeField]
    NPC killTarget;
    [SerializeField]
    int rewardMoney;


    UnityEngine.Rendering.CompareFunction comparison = UnityEngine.Rendering.CompareFunction.Always;

    void Awake()
    {
        isCorrect = false;
        questStatus = QuestStatus.Kill;
        killTarget.gameObject.SetActive(false);

        phoneArrow = Instantiate(phoneArrowPref);
        phoneArrow.transform.parent = WorldUIManager.Instance.transform;

        Image image = phoneArrow.GetComponent<Image>();
        Material existingGlobalMat = image.materialForRendering;
        Material updatedMaterial = new Material(existingGlobalMat);
        updatedMaterial.SetInt("unity_GUIZTestMode", (int)comparison);
        image.material = updatedMaterial;
    }

    void Update()
    {
        if (isCorrect)
        {
            correctAndOffDel += Time.deltaTime;
            if (correctAndOffTime < correctAndOffDel)
            {
                gameObject.SetActive(false);
            }
        }
        else
        {
            if (questArrow == null)
            {
                WorldUIManager.Instance.UpdateArrow(phoneArrow, startPhone.transform.position);
                return;
            }

            WorldUIManager.Instance.UpdateArrow(questArrow, killTarget.transform.position);
        }
    }


    public override void StartQuest()
    {
        killTarget.gameObject.SetActive(true);
        phoneArrow.gameObject.SetActive(false);
        QuestManager.Instance.StartQuest(this);
        QuestUIManager.Instance.ToastStartQuest(title, infoPath);


        SoundManager.Instance.PlayClip(startClip, SoundPlayMode.OneShotPlay);
        questArrow = WorldUIManager.Instance.SpwanArrow();
        questArrow.transform.parent = WorldUIManager.Instance.transform;
    }

    public override bool CheckCondition()
    {
        if (killTarget.isDie)
        {
            WorldUIManager.Instance.despwanArrow(questArrow);
            WantedLevel.instance.ResetWantedLevel();
            SoundManager.Instance.PlayClip(completeClip, SoundPlayMode.OneShotPlay);
            return true;
        }
        return false;
    }

    public override void PushReward()
    {
        QuestUIManager.Instance.ToastEndQuest(endPath);
        GameManager.Instance.IncreaseMoney(rewardMoney);
        isCorrect = true;
    }

    void OnDrawGizmos()
    {
        if (killTarget != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawLine(startPhone.transform.position, killTarget.transform.position);
        }
    }
}




