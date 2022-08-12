using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class Grenade : MonoBehaviourPunCallbacks
{
    public GameObject player;
    public GameObject point;
    private Transform bullet;   // 포물체
    public PhotonView PV;
    public bool grenthrow;
    public LayerMask layermask;
    public IEnumerator shootImpl;
    public GameObject grenadeParticle;

    private float tx;

    private float ty;

    private float tz;

    private float v;

    public float g = 20f;

    private float elapsed_time;

    public float max_height;

    private float t;

    private Vector3 start_pos;

    private Vector3 end_pos;

    private float dat;  //도착점 도달 시간 

    public void Start()
    {
        AudioManager.Instance.PlaySound("Throw", transform.position, 1f + Random.Range(-0.1f, 0.1f));
        shootImpl = ShootImpl();

        if (TrainingController.instance.training == true)
        {
            point = player.transform.Find("GrenadeEx").gameObject;

            Shoot(gameObject.transform, new Vector3(player.transform.position.x, player.transform.position.y + 3f, player.transform.position.z), point.transform.position, g, 7.5f);

            point.gameObject.SetActive(false);
            if (player != null)
            {
                player.GetComponent<Animator>().SetTrigger("Throw");
                Vector3 quater = point.transform.position - player.transform.position;
                player.transform.rotation = Quaternion.LookRotation(quater).normalized;
            }
            point.transform.position = player.transform.position;

        }
        else
        {
            if (PV.IsMine)
            {
                point = player.transform.Find("GrenadeEx").gameObject;

                Shoot(gameObject.transform, new Vector3(player.transform.position.x, player.transform.position.y + 3f, player.transform.position.z), point.transform.position, g, 7.5f);

                point.gameObject.SetActive(false);
                if (player != null)
                {
                    player.GetComponent<Animator>().SetTrigger("Throw");
                    Vector3 quater = point.transform.position - player.transform.position;
                    player.transform.rotation = Quaternion.LookRotation(quater).normalized;
                }
                point.transform.position = player.transform.position;
            }
        }

    }

    private void FixedUpdate()
    {
        if (player != null)
        {
            if (player.GetComponent<TestShoot>().boom == true && TrainingController.instance.training != true)
                PV.RPC("GrenadeBoob", RpcTarget.All);
            else if(player.GetComponent<TestShoot>().boom == true && TrainingController.instance.training == true)
                GrenadeBoob();
        }
    }

    public void Shoot(Transform bullet, Vector3 startPos, Vector3 endPos, float g, float max_height)
    {
        start_pos = startPos;

        end_pos = endPos;

        this.g = g;

        this.max_height = max_height;

        this.bullet = bullet;

        this.bullet.position = start_pos;

        var dh = endPos.y - startPos.y;

        var mh = max_height - startPos.y;

        ty = Mathf.Sqrt(2 * this.g * mh);

        float a = this.g;

        float b = -2 * ty;

        float c = 2 * dh;

        dat = (-b + Mathf.Sqrt(b * b - 4 * a * c)) / (2 * a);

        tx = -(startPos.x - endPos.x) / dat;

        tz = -(startPos.z - endPos.z) / dat;

        this.elapsed_time = 0;

        StartCoroutine(shootImpl);
    }

    IEnumerator ShootImpl()
    {

        while (true)

        {

            this.elapsed_time += Time.deltaTime;



            var tx = start_pos.x + this.tx * elapsed_time;

            var ty = start_pos.y + this.ty * elapsed_time - 0.5f * g * elapsed_time * elapsed_time;

            var tz = start_pos.z + this.tz * elapsed_time;

            var tpos = new Vector3(tx, ty, tz);

            bullet.transform.LookAt(tpos);

            bullet.transform.position = tpos;

            if (this.elapsed_time >= this.dat)

                break;

            yield return null;

        }

    }

    [PunRPC]
    public void GrenadeBoob()
    {
        if (player != null)
        {
            if (player.GetComponent<TestShoot>().consumableType == 1)
            {
                AudioManager.Instance.PlaySound("Grenade", transform.position, 1f + Random.Range(-0.1f, 0.1f));
                if (TrainingController.instance.training != true)
                    PhotonNetwork.Instantiate("GrenadeExplosion", new Vector3(transform.position.x, transform.position.y + 1f, transform.position.z), transform.rotation);
                else
                    Instantiate(grenadeParticle, new Vector3(transform.position.x, transform.position.y + 1f, transform.position.z), transform.rotation);
                float grenadeRange = player.GetComponent<PlayerSkill_Specificity>().spGrenadeRange;
                RaycastHit[] rayhists = Physics.SphereCastAll(transform.position, 7.5f + grenadeRange, Vector3.up, 0, layermask);
                foreach (RaycastHit hitObj in rayhists)
                {
                    if (hitObj.collider.tag == "Player")
                    {
                        if (TrainingController.instance.training != true)
                        {
                            TestHealth ph = hitObj.transform.GetComponent<TestHealth>();
                            ph.pV.RPC("TakeDamage", RpcTarget.AllBuffered, 60f);
                            
                            if (ph.currentHealth <= 0 && !ph.isDeath)
                            {
                                ph.TakeDamageMurderN(player);
                                player.GetComponent<TestShoot>().PV.RPC("KillCountPlus", RpcTarget.AllViaServer);
                                ph.pV.RPC("DeathCheck", RpcTarget.AllViaServer);
                            }
                            ph.pV.RPC("ExPushEnd", RpcTarget.All, transform.position,10f);
                        }
                            
                    }
                    else if (hitObj.collider.tag == "Enemy")
                    {
                        if (TrainingController.instance.training != true)
                        {
                            EnemyHealthTest eh = hitObj.transform.GetComponent<EnemyHealthTest>();
                            eh.pV.RPC("TakeDamage", RpcTarget.AllBuffered, 60f);
                            eh.TakeDamageMurderN(player);
                            if (eh.currentHealth <= 0 && !eh.isDeath)
                            {
                                player.GetComponent<TestShoot>().PV.RPC("KillCountPlus", RpcTarget.AllViaServer);
                                eh.pV.RPC("DeathCheck", RpcTarget.AllViaServer);
                            }
                            eh.pV.RPC("ExPushEnd", RpcTarget.All, transform.position, 10f);
                        }
                        else
                        {
                            hitObj.transform.GetComponent<EnemyHealthTest>().TakeDamage(60f);
                            hitObj.transform.GetComponent<EnemyHealthTest>().TakeDamageMurderN(gameObject);
                            hitObj.transform.GetComponent<EnemyHealthTest>().ExPushEnd(transform.position,10f);
                        }
                    }
                }
                if (TrainingController.instance.training != true)
                    PV.RPC("Destroy", RpcTarget.All);
                else
                    Destroy();
                grenthrow = true;
            }
            else if (player.GetComponent<TestShoot>().consumableType == 3)
            {
                AudioManager.Instance.PlaySound("Smoke", transform.position, 1f + Random.Range(-0.1f, 0.1f));
                if (TrainingController.instance.training != true)
                {
                    PhotonNetwork.Instantiate("SmokeEx", new Vector3(transform.position.x + 5, transform.position.y + 1f, transform.position.z + 5), Quaternion.identity);
                    PV.RPC("Destroy", RpcTarget.All);
                }
                else
                {
                    Instantiate(grenadeParticle, new Vector3(transform.position.x + 5, transform.position.y + 1f, transform.position.z + 5), Quaternion.identity);
                    Destroy();
                }
                grenthrow = true;
            }
            else if (player.GetComponent<TestShoot>().consumableType == 4)
            {
                if (TrainingController.instance.training != true)
                    PhotonNetwork.Instantiate("EMPEx", new Vector3(transform.position.x, transform.position.y + 1f, transform.position.z), transform.rotation);
                else
                    Instantiate(grenadeParticle, new Vector3(transform.position.x, transform.position.y + 1f, transform.position.z), transform.rotation);
                float grenadeRange = player.GetComponent<PlayerSkill_Specificity>().spGrenadeRange;
                RaycastHit[] rayhists = Physics.SphereCastAll(transform.position, 10 + grenadeRange, Vector3.up, 0, layermask);
                foreach (RaycastHit hitObj in rayhists)
                {
                    if (TrainingController.instance.training != true)
                    {
                        if (hitObj.collider.tag == "Player")
                        {
                            hitObj.transform.GetComponent<TestHealth>().pV.RPC("Emp", RpcTarget.AllViaServer);
                            hitObj.transform.GetComponent<TestHealth>().pV.RPC("Shock", RpcTarget.AllViaServer);

                        }
                        else if (hitObj.collider.tag == "Enemy")
                        {
                            hitObj.transform.GetComponent<EnemyHealthTest>().pV.RPC("EnemyShock", RpcTarget.AllViaServer);
                        }
                    }
                    else
                    {
                        if (hitObj.collider.tag == "Player")
                        {
                            hitObj.transform.GetComponent<TestHealth>().Emp();
                            hitObj.transform.GetComponent<TestHealth>().Shock();

                        }
                        else if (hitObj.collider.tag == "Enemy")
                        {
                            hitObj.transform.GetComponent<EnemyHealthTest>().EnemyShock();
                        }
                    }
                }
                if (TrainingController.instance.training != true)
                    PV.RPC("Destroy", RpcTarget.All);
                else
                    Destroy();
                grenthrow = true;
            }
            else
                return;

        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        AudioManager.Instance.PlaySound("GrenadeSet", transform.position, 1f + Random.Range(-0.1f, 0.1f));
        if(collision.gameObject.tag == "Environment")
        {
            StopCoroutine(shootImpl);
        }
    }

    [PunRPC]
    public void Destroy()
    {
        Destroy(gameObject);
    }
}
