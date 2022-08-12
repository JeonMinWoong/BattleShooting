using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SpCount : MonoBehaviour
{
    public int[] spCount;
    public TextMeshProUGUI[] spText;
    public Button[] spSubBt;
    public Button[] spPlusBt;
    public SpPoint spPoint;
    public int spType;
    public SpBar spbar;
    public TextMeshProUGUI[] spShowText;

    private void Awake()
    {
        for (int i = 0; i < 6; i++)
        {
            spCount[i] = 0;
        }
    }

    public void Start()
    {

        if (spType == 0)
            SpGun();
        else if (spType == 1)
            SpUtility();
        else
            SpDefense();
    }

    public void SpGun()
    {
        spCount[0] = AuthManager.instance.userSpCount1_1;
        spCount[1] = AuthManager.instance.userSpCount1_2;
        spCount[2] = AuthManager.instance.userSpCount1_3;
        spCount[3] = AuthManager.instance.userSpCount1_4;
        spCount[4] = AuthManager.instance.userSpCount1_5;
        spCount[5] = AuthManager.instance.userSpCount1_6;

        if (AuthManager.instance.userSpCount1_1 > 1)
            spShowText[0].text = string.Format("Attack + {0}%", (0.05f * AuthManager.instance.userSpCount1_1) * 100);
        if (AuthManager.instance.userSpCount1_2 > 1)
            spShowText[1].text = string.Format("Bullet Speed + {0}%", (0.1f * AuthManager.instance.userSpCount1_2) * 100);
        if (AuthManager.instance.userSpCount1_3 > 1)
            spShowText[2].text = string.Format("Range + {0}m", (0.5f * AuthManager.instance.userSpCount1_3) * 10);
        if (AuthManager.instance.userSpCount1_4 > 1)
            spShowText[3].text = string.Format("Attack Speed + {0}%", (0.10f * AuthManager.instance.userSpCount1_4) * 100);
        if (AuthManager.instance.userSpCount1_5 > 1)
            spShowText[4].text = string.Format("Size + {0}%", (0.10f * AuthManager.instance.userSpCount1_5) * 100);
        if (AuthManager.instance.userSpCount1_6 > 1)
            spShowText[5].text = string.Format("Attack + {0}%\nAttack Speed + {1}%\nBullet Speed + {2}%\nGrenade Range + {3}m", (0.02f * AuthManager.instance.userSpCount1_6) * 100, (0.05f * AuthManager.instance.userSpCount1_6) * 100
            , (0.05f * AuthManager.instance.userSpCount1_6) * 100, (0.25f * AuthManager.instance.userSpCount1_6) * 10);
    }

    public void SpUtility()
    {
        spCount[0] = AuthManager.instance.userSpCount2_1;
        spCount[1] = AuthManager.instance.userSpCount2_2;
        spCount[2] = AuthManager.instance.userSpCount2_3;
        spCount[3] = AuthManager.instance.userSpCount2_4;
        spCount[4] = AuthManager.instance.userSpCount2_5;
        spCount[5] = AuthManager.instance.userSpCount2_6;

        if (AuthManager.instance.userSpCount2_1 > 1)
            spShowText[0].text = string.Format("Speed + {0}%", (0.10f * AuthManager.instance.userSpCount2_1) * 100);
        if (AuthManager.instance.userSpCount2_2 > 1)
            spShowText[1].text = string.Format("Heath Recovery + {0}", 0.1f + (0.03f * AuthManager.instance.userSpCount2_2));
        if (AuthManager.instance.userSpCount2_3 > 1)
            spShowText[2].text = string.Format("Game Set Money + {0}$", (5 * AuthManager.instance.userSpCount2_3));
        if (AuthManager.instance.userSpCount2_4 > 1)
            spShowText[3].text = string.Format("Bullet Count + {0}%", (0.1f * AuthManager.instance.userSpCount2_4) * 100);
        if (AuthManager.instance.userSpCount2_5 > 1)
            spShowText[4].text = string.Format("Consum Count + {0}%", (0.25f * AuthManager.instance.userSpCount2_5) * 100);
        if (AuthManager.instance.userSpCount2_6 > 1)
            spShowText[5].text = string.Format("Speed Plus + {0}%\nHeath Recovery + {1}\nMoney Plus + {2}$\nBullet Plus + {3}%", (0.05f * AuthManager.instance.userSpCount2_6) * 100, 0.03f * AuthManager.instance.userSpCount2_6
            , 3 * AuthManager.instance.userSpCount2_6, (0.05f * AuthManager.instance.userSpCount2_6) * 100);
    }

    public void SpDefense()
    {
        spCount[0] = AuthManager.instance.userSpCount3_1;
        spCount[1] = AuthManager.instance.userSpCount3_2;
        spCount[2] = AuthManager.instance.userSpCount3_3;
        spCount[3] = AuthManager.instance.userSpCount3_4;
        spCount[4] = AuthManager.instance.userSpCount3_5;
        spCount[5] = AuthManager.instance.userSpCount3_6;

        if (AuthManager.instance.userSpCount3_1 > 1)
            spShowText[0].text = string.Format("Start with Shield + {0}", (3 * (AuthManager.instance.userSpCount3_1+1)));
        if (AuthManager.instance.userSpCount3_2 > 1)
            spShowText[1].text = string.Format("Defense + {0}%", (0.03f * AuthManager.instance.userSpCount3_2) * 100);
        if (AuthManager.instance.userSpCount3_3 > 1)
            spShowText[2].text = string.Format("Max Heath + {0}", (5 * AuthManager.instance.userSpCount3_3));
        if (AuthManager.instance.userSpCount3_4 > 1)
            spShowText[3].text = string.Format("Shield Regen + {0} , 30Sec", (5 + (3f * AuthManager.instance.userSpCount3_4)));
        if (AuthManager.instance.userSpCount3_5 > 1)
            spShowText[4].text = string.Format("Armor + {0}%", (0.05f * AuthManager.instance.userSpCount3_5) * 100);
        if (AuthManager.instance.userSpCount3_6 > 1)
            spShowText[5].text = string.Format("Start with Shield + {0}\nArmor + {1}%\nMax Heath + {2}\nShield Regen + {3} , 30Sec", (2 * AuthManager.instance.userSpCount3_6), (0.02f * AuthManager.instance.userSpCount3_6) * 100
            , 3 * AuthManager.instance.userSpCount3_6, 3 + (2 * AuthManager.instance.userSpCount3_6));
    }


    private void Update()
    {

        if (spType == 0)
            SpGun();
        else if (spType == 1)
            SpUtility();
        else
            SpDefense();

        for (int i = 0; i < 6; i++)
        {
            spText[i].text = spCount[i].ToString();
            if (spCount[i] == 0)
            {
                spSubBt[i].interactable = false;
            }
            else
            {
                if (AuthManager.instance.userLevel < 10)
                {
                    spSubBt[0].interactable = true;
                    spSubBt[1].interactable = true;
                    spSubBt[2].interactable = true;
                    spSubBt[3].interactable = false;
                    spSubBt[4].interactable = false;
                }
                else if(AuthManager.instance.userLevel < 20)
                {
                    spSubBt[0].interactable = true;
                    spSubBt[1].interactable = true;
                    spSubBt[2].interactable = true;
                    spSubBt[3].interactable = true;
                    spSubBt[4].interactable = true;
                    spSubBt[5].interactable = false;
                }
                else
                {
                    spSubBt[i].interactable = true;
                }
            }
            if (spCount[i] >= 5 || spPoint.playerSpPoint == 0)
            {
                spPlusBt[i].interactable = false;
            }
            else
            {
                if (AuthManager.instance.userLevel < 10)
                {
                    spPlusBt[0].interactable = true;
                    spPlusBt[1].interactable = true;
                    spPlusBt[2].interactable = true;
                    spPlusBt[3].interactable = false;
                    spPlusBt[4].interactable = false;
                }
                else if (AuthManager.instance.userLevel < 20)
                {
                    spPlusBt[0].interactable = true;
                    spPlusBt[1].interactable = true;
                    spPlusBt[2].interactable = true;
                    spPlusBt[3].interactable = true;
                    spPlusBt[4].interactable = true;
                    spPlusBt[5].interactable = false;
                }
                else
                {
                    spSubBt[i].interactable = true;
                }
            }
        }
    }

    public void Plus(int _Count)
    {
        if (spPoint.playerSpPoint == 0)
            return;

        if (spType == 0)
        {
            switch (_Count)
            {
                case 0:
                    AuthManager.instance.userSpCount1_1++;
                    break;
                case 1:
                    AuthManager.instance.userSpCount1_2++;
                    break;
                case 2:
                    AuthManager.instance.userSpCount1_3++;
                    break;
                case 3:
                    AuthManager.instance.userSpCount1_4++;
                    break;
                case 4:
                    AuthManager.instance.userSpCount1_5++;
                    break;
                case 5:
                    AuthManager.instance.userSpCount1_6++;
                    break;
            }
        }
        else if (spType == 1)
        {
            switch (_Count)
            {
                case 0:
                    AuthManager.instance.userSpCount2_1++;
                    break;
                case 1:
                    AuthManager.instance.userSpCount2_2++;
                    break;
                case 2:
                    AuthManager.instance.userSpCount2_3++;
                    break;
                case 3:
                    AuthManager.instance.userSpCount2_4++;
                    break;
                case 4:
                    AuthManager.instance.userSpCount2_5++;
                    break;
                case 5:
                    AuthManager.instance.userSpCount2_6++;
                    break;
            }
        }
        else
        {
            switch (_Count)
            {
                case 0:
                    AuthManager.instance.userSpCount3_1++;
                    break;
                case 1:
                    AuthManager.instance.userSpCount3_2++;
                    break;
                case 2:
                    AuthManager.instance.userSpCount3_3++;
                    break;
                case 3:
                    AuthManager.instance.userSpCount3_4++;
                    break;
                case 4:
                    AuthManager.instance.userSpCount3_5++;
                    break;
                case 5:
                    AuthManager.instance.userSpCount3_6++;
                    break;
            }
        }
        spPoint.SpPointSub();
    }

    public void Subtract(int _Count)
    {
        if (spType == 0)
        {
            switch (_Count)
            {
                case 0:
                    AuthManager.instance.userSpCount1_1--;
                    if(AuthManager.instance.userSpCount1_1 == 0)
                    {
                        spbar.OffSp(0);
                    }
                    break;
                case 1:
                    AuthManager.instance.userSpCount1_2--;
                    if (AuthManager.instance.userSpCount1_2 == 0)
                    {
                        spbar.OffSp(1);
                    }
                    break;
                case 2:
                    AuthManager.instance.userSpCount1_3--;
                    if (AuthManager.instance.userSpCount1_3 == 0)
                    {
                        spbar.OffSp(2);
                    }
                    break;
                case 3:
                    AuthManager.instance.userSpCount1_4--;
                    if (AuthManager.instance.userSpCount1_4 == 0)
                    {
                        spbar.OffSp(3);
                    }
                    break;
                case 4:
                    AuthManager.instance.userSpCount1_5--;
                    if (AuthManager.instance.userSpCount1_5 == 0)
                    {
                        spbar.OffSp(4);
                    }
                    break;
                case 5:
                    AuthManager.instance.userSpCount1_6--;
                    if (AuthManager.instance.userSpCount1_6 == 0)
                    {
                        spbar.OffSp(5);
                    }
                    break;
            }
        }
        else if (spType == 1)
        {
            switch (_Count)
            {
                case 0:
                    AuthManager.instance.userSpCount2_1--;
                    if (AuthManager.instance.userSpCount2_1 == 0)
                    {
                        spbar.OffSp(6);
                    }
                    break;
                case 1:
                    AuthManager.instance.userSpCount2_2--;
                    if (AuthManager.instance.userSpCount2_2 == 0)
                    {
                        spbar.OffSp(7);
                    }
                    break;
                case 2:
                    AuthManager.instance.userSpCount2_3--;
                    if (AuthManager.instance.userSpCount2_3 == 0)
                    {
                        spbar.OffSp(8);
                    }
                    break;
                case 3:
                    AuthManager.instance.userSpCount2_4--;
                    if (AuthManager.instance.userSpCount2_4 == 0)
                    {
                        spbar.OffSp(9);
                    }
                    break;
                case 4:
                    AuthManager.instance.userSpCount2_5--;
                    if (AuthManager.instance.userSpCount2_5 == 0)
                    {
                        spbar.OffSp(10);
                    }
                    break;
                case 5:
                    AuthManager.instance.userSpCount2_6--;
                    if (AuthManager.instance.userSpCount2_6 == 0)
                    {
                        spbar.OffSp(11);
                    }
                    break;
            }
        }
        else
        {
            switch (_Count)
            {
                case 0:
                    AuthManager.instance.userSpCount3_1--;
                    if (AuthManager.instance.userSpCount3_1 == 0)
                    {
                        spbar.OffSp(12);
                    }
                    break;
                case 1:
                    AuthManager.instance.userSpCount3_2--;
                    if (AuthManager.instance.userSpCount3_2 == 0)
                    {
                        spbar.OffSp(13);
                    }
                    break;
                case 2:
                    AuthManager.instance.userSpCount3_3--;
                    if (AuthManager.instance.userSpCount3_3 == 0)
                    {
                        spbar.OffSp(14);
                    }
                    break;
                case 3:
                    AuthManager.instance.userSpCount3_4--;
                    if (AuthManager.instance.userSpCount3_4 == 0)
                    {
                        spbar.OffSp(15);
                    }
                    break;
                case 4:
                    AuthManager.instance.userSpCount3_5--;
                    if (AuthManager.instance.userSpCount3_5 == 0)
                    {
                        spbar.OffSp(16);
                    }
                    break;
                case 5:
                    AuthManager.instance.userSpCount3_6--;
                    if (AuthManager.instance.userSpCount3_6 == 0)
                    {
                        spbar.OffSp(17);
                    }
                    break;
            }
        }
       
        spPoint.SpPointPlus();
    }
}
