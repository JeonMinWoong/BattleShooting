using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cam : MonoBehaviour
{
    public struct St_ObstacleRendererInfo
    {
        public int InstanceId;
        public MeshRenderer Mesh_Renderer;
        public Shader OrinShader;
    }

    public GameObject playerTr;
    public GameObject camTr;
    public Dictionary<int, St_ObstacleRendererInfo> Dic_SavedObstaclesRendererInfo = new Dictionary<int, St_ObstacleRendererInfo>();
    public List<St_ObstacleRendererInfo> Lst_TransparentedRenderer = new List<St_ObstacleRendererInfo>();
    private Color ColorTransparent = new Color(1f, 1f, 1f, 0.3f);
    private string ShaderColorParamName = "_Color" + "";
    private RaycastHit[] TransparentHits;
    public LayerMask TransparentRayLayer;

    private void Start()
    {
        playerTr = GameObject.Find("PlayerTest");
    }

    void Update()
    {
        Camera_TransparentProcess_Operation();
    }

    void Camera_TransparentProcess_Operation()
    {
        if (playerTr == null) return;



        // 반투명했던거 다시 월래 쉐이더로 복귀
        if (Lst_TransparentedRenderer.Count > 0)
        {
            for (int i = 0; i < Lst_TransparentedRenderer.Count; i++)
            {
                Lst_TransparentedRenderer[i].Mesh_Renderer.material.shader = Lst_TransparentedRenderer[i].OrinShader;
            }

            Lst_TransparentedRenderer.Clear();
        }



        Vector3 CharPos = playerTr.transform.position + playerTr.transform.TransformDirection(0, 1.5f, 0);
        float Distance = (camTr.transform.position - CharPos).magnitude;

        Vector3 DirToCam = (camTr.transform.position - CharPos).normalized;


        //플레이어몸에서 몸뒤 체크해서 걸리는오브젝트 반투명
        //HitRayTransparentObject(CharPos, DirToCharbehind, Distance);
        //플레이어 몸에어 카메라까지 체크해서 걸리는오브젝트 반투명
        HitRayTransparentObject(CharPos, DirToCam, Distance);




    }


    void HitRayTransparentObject(Vector3 start, Vector3 direction, float distance)
    {
        TransparentHits = Physics.RaycastAll(start, direction, distance, TransparentRayLayer);

        for (int i = 0; i < TransparentHits.Length; i++)
        {
            int instanceid = TransparentHits[i].collider.GetInstanceID();

            //레이에 걸린 장애물이 컬렉션에 없으면 저장하기
            if (!Dic_SavedObstaclesRendererInfo.ContainsKey(instanceid))
            {
                MeshRenderer obsRenderer = TransparentHits[i].collider.gameObject.GetComponent<MeshRenderer>();
                St_ObstacleRendererInfo rendererInfo = new St_ObstacleRendererInfo();
                rendererInfo.InstanceId = instanceid; // 고유 인스턴스아이디
                rendererInfo.Mesh_Renderer = obsRenderer; // 메시렌더러
                rendererInfo.OrinShader = obsRenderer.material.shader; // 장애물의쉐이더

                Dic_SavedObstaclesRendererInfo[instanceid] = rendererInfo;
            }

            // 쉐이더 반투명으로 변경
            Dic_SavedObstaclesRendererInfo[instanceid].Mesh_Renderer.material.shader = Shader.Find("Legacy Shaders/Transparent/Diffuse");
            //알파값 줄인 쉐이더 색 변경
            Dic_SavedObstaclesRendererInfo[instanceid].Mesh_Renderer.material.SetColor(ShaderColorParamName, ColorTransparent);

            Lst_TransparentedRenderer.Add(Dic_SavedObstaclesRendererInfo[instanceid]);
        }
    }
}
