using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;
using UnityEngine.Rendering.Universal;
using System.IO;

namespace SalaSegurancaVR.EditorTools
{
    /// <summary>
    /// Gera automaticamente a cena "SalaSegurancaVR": chao, paredes, teto, mesa,
    /// monitor, cofre, camera CFTV, porta, iluminacao, skybox e um Jogador
    /// navegavel no PC. Menu: "Sala Seguranca VR / Construir Cena".
    /// Todos os nomes de objetos, secoes e materiais estao em portugues.
    /// </summary>
    public static class SalaSegurancaBuilder
    {
        private const string ScenePath = "Assets/Scenes/SalaSegurancaVR.unity";
        private const string MatFolder = "Assets/Materials";

        [MenuItem("Sala Seguranca VR/Construir Cena")]
        public static void BuildScene()
        {
            // Nova cena vazia para montar tudo do zero.
            Scene scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);

            // Limpa materiais antigos e recria a pasta.
            if (AssetDatabase.IsValidFolder(MatFolder))
                AssetDatabase.DeleteAsset(MatFolder);
            EnsureFolder(MatFolder);

            // ---- Materiais ----
            Material matChao    = MakeMat("Mat_Chao",     new Color(0.20f, 0.22f, 0.26f), 0.15f);
            Material matParede  = MakeMat("Mat_Parede",   new Color(0.55f, 0.57f, 0.60f), 0.05f);
            Material matTeto    = MakeMat("Mat_Teto",     new Color(0.75f, 0.76f, 0.78f), 0.05f);
            Material matMesa    = MakeMat("Mat_Mesa",     new Color(0.45f, 0.30f, 0.18f), 0.10f);
            Material matMetal   = MakeMat("Mat_Metal",    new Color(0.30f, 0.32f, 0.35f), 0.75f);
            Material matTela    = MakeMat("Mat_Tela",     new Color(0.05f, 0.35f, 0.55f), 0.60f, emissive:true, emissionColor:new Color(0.05f, 0.45f, 0.70f));
            Material matCofre   = MakeMat("Mat_Cofre",    new Color(0.12f, 0.14f, 0.16f), 0.65f);
            Material matDestaque= MakeMat("Mat_Destaque", new Color(0.80f, 0.15f, 0.10f), 0.30f);
            Material matPorta   = MakeMat("Mat_Porta",    new Color(0.35f, 0.22f, 0.12f), 0.10f);

            // ================= HIERARQUIA =================
            GameObject hGerenc  = Header("--- GERENCIAMENTO ---");
            GameObject hJogador = Header("--- PLAYER ---");
            GameObject hAmbiente= Header("--- AMBIENTE ---");
            GameObject hObjetos = Header("--- OBJETOS DA SALA DE SEGURANCA ---");
            GameObject hLuzes   = Header("--- ILUMINACAO ---");

            // Sistema de Eventos (Gerenciamento)
            GameObject sistemaEventos = new GameObject("SistemaDeEventos",
                typeof(UnityEngine.EventSystems.EventSystem),
                typeof(UnityEngine.InputSystem.UI.InputSystemUIInputModule));
            sistemaEventos.transform.SetParent(hGerenc.transform);

            // ---- PLAYER (camera navegavel no PC) ----
            GameObject jogador = new GameObject("Player_XR");
            jogador.transform.SetParent(hJogador.transform);
            jogador.transform.position = new Vector3(0f, 1.6f, -3.5f);

            GameObject camGO = new GameObject("Camera_Principal", typeof(Camera), typeof(AudioListener), typeof(UniversalAdditionalCameraData));
            camGO.tag = "MainCamera";
            camGO.transform.SetParent(jogador.transform);
            camGO.transform.localPosition = Vector3.zero;

            var controller = jogador.AddComponent<SalaSegurancaVR.PCPlayerController>();
            controller.cameraTransform = camGO.transform;

            // ---- AMBIENTE ----
            Prim(PrimitiveType.Plane, "Chao", hAmbiente.transform, new Vector3(0, 0, 0), new Vector3(1f, 1f, 1f), matChao);

            const float w = 5f;    // meia-largura
            const float h = 3f;    // altura da parede
            const float t = 0.15f; // espessura

            Prim(PrimitiveType.Cube, "Parede_Norte", hAmbiente.transform, new Vector3(0, h/2, w),  new Vector3(w*2, h, t), matParede);
            Prim(PrimitiveType.Cube, "Parede_Sul",   hAmbiente.transform, new Vector3(0, h/2, -w), new Vector3(w*2, h, t), matParede);
            Prim(PrimitiveType.Cube, "Parede_Leste", hAmbiente.transform, new Vector3(w, h/2, 0),  new Vector3(t, h, w*2), matParede);
            Prim(PrimitiveType.Cube, "Parede_Oeste", hAmbiente.transform, new Vector3(-w, h/2, 0), new Vector3(t, h, w*2), matParede);

            Prim(PrimitiveType.Cube, "Teto", hAmbiente.transform, new Vector3(0, h, 0), new Vector3(w*2, t, w*2), matTeto);

            // ---- OBJETOS DA SALA DE SEGURANCA ----
            // Mesa de monitoramento
            GameObject mesa = new GameObject("Mesa");
            mesa.transform.SetParent(hObjetos.transform);
            mesa.transform.position = new Vector3(0, 0, 2.5f);
            Prim(PrimitiveType.Cube, "Mesa_Tampo", mesa.transform, new Vector3(0, 0.75f, 2.5f), new Vector3(2.0f, 0.08f, 0.9f), matMesa);
            MakeLeg(mesa.transform, "Perna_1", new Vector3(-0.9f, 0.375f, 2.15f), matMetal);
            MakeLeg(mesa.transform, "Perna_2", new Vector3( 0.9f, 0.375f, 2.15f), matMetal);
            MakeLeg(mesa.transform, "Perna_3", new Vector3(-0.9f, 0.375f, 2.85f), matMetal);
            MakeLeg(mesa.transform, "Perna_4", new Vector3( 0.9f, 0.375f, 2.85f), matMetal);

            // Monitor (suporte + tela)
            GameObject monitor = new GameObject("Monitor");
            monitor.transform.SetParent(hObjetos.transform);
            monitor.transform.position = new Vector3(0, 0, 2.7f);
            Prim(PrimitiveType.Cylinder, "Monitor_Suporte", monitor.transform, new Vector3(0, 0.9f, 2.75f), new Vector3(0.06f, 0.1f, 0.06f), matMetal);
            Prim(PrimitiveType.Cube, "Monitor_Tela", monitor.transform, new Vector3(0, 1.25f, 2.78f), new Vector3(1.0f, 0.6f, 0.05f), matTela);

            // Cofre (corpo + porta + macaneta)
            GameObject cofre = new GameObject("Cofre");
            cofre.transform.SetParent(hObjetos.transform);
            cofre.transform.position = new Vector3(-3.8f, 0, 3.8f);
            Prim(PrimitiveType.Cube, "Cofre_Corpo", cofre.transform, new Vector3(-3.8f, 0.5f, 3.8f), new Vector3(1.0f, 1.0f, 0.9f), matCofre);
            Prim(PrimitiveType.Cube, "Cofre_Porta", cofre.transform, new Vector3(-3.8f, 0.5f, 3.36f), new Vector3(0.8f, 0.8f, 0.06f), matMetal);
            Prim(PrimitiveType.Cylinder, "Cofre_Macaneta", cofre.transform, new Vector3(-3.8f, 0.5f, 3.30f), new Vector3(0.05f, 0.12f, 0.05f), matDestaque).transform.Rotate(90, 0, 0);

            // Camera CFTV decorativa (suporte + corpo + lente)
            GameObject cftv = new GameObject("Camera_CFTV");
            cftv.transform.SetParent(hObjetos.transform);
            cftv.transform.position = new Vector3(4.4f, 2.6f, 4.4f);
            Prim(PrimitiveType.Cube, "CFTV_Suporte", cftv.transform, new Vector3(4.6f, 2.7f, 4.6f), new Vector3(0.1f, 0.1f, 0.4f), matMetal);
            Prim(PrimitiveType.Cube, "CFTV_Corpo", cftv.transform, new Vector3(4.4f, 2.55f, 4.4f), new Vector3(0.25f, 0.25f, 0.5f), matCofre);
            GameObject lente = Prim(PrimitiveType.Cylinder, "CFTV_Lente", cftv.transform, new Vector3(4.25f, 2.5f, 4.25f), new Vector3(0.12f, 0.1f, 0.12f), matMetal);
            lente.transform.localRotation = Quaternion.Euler(90, 45, 0);

            // Porta (decorativa na parede sul)
            Prim(PrimitiveType.Cube, "Porta", hObjetos.transform, new Vector3(2.5f, 1.05f, -4.9f), new Vector3(1.0f, 2.1f, 0.1f), matPorta);

            // ---- ILUMINACAO ----
            GameObject dirGO = new GameObject("Luz_Direcional", typeof(Light));
            dirGO.transform.SetParent(hLuzes.transform);
            Light dir = dirGO.GetComponent<Light>();
            dir.type = LightType.Directional;
            dir.intensity = 1.0f;
            dir.shadows = LightShadows.Soft;
            dirGO.transform.rotation = Quaternion.Euler(50f, -30f, 0f);

            GameObject tetoGO = new GameObject("Luz_Teto", typeof(Light));
            tetoGO.transform.SetParent(hLuzes.transform);
            Light pl = tetoGO.GetComponent<Light>();
            pl.type = LightType.Point;
            pl.range = 12f;
            pl.intensity = 2.5f;
            pl.color = new Color(1f, 0.96f, 0.85f);
            tetoGO.transform.position = new Vector3(0, h - 0.3f, 0);

            // ---- SKYBOX ----
            Material sky = new Material(Shader.Find("Skybox/Procedural"));
            sky.SetFloat("_AtmosphereThickness", 1.0f);
            sky.SetColor("_SkyTint", new Color(0.5f, 0.55f, 0.65f));
            sky.SetColor("_GroundColor", new Color(0.25f, 0.25f, 0.28f));
            sky.SetFloat("_Exposure", 1.1f);
            AssetDatabase.CreateAsset(sky, MatFolder + "/Skybox_SalaSeguranca.mat");
            RenderSettings.skybox = sky;
            RenderSettings.ambientMode = UnityEngine.Rendering.AmbientMode.Skybox;
            DynamicGI.UpdateEnvironment();

            // ---- Salvar cena ----
            EnsureFolder("Assets/Scenes");
            EditorSceneManager.MarkSceneDirty(scene);
            EditorSceneManager.SaveScene(scene, ScenePath);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            Debug.Log("[SalaSegurancaBuilder] Cena construida e salva em " + ScenePath);
            EditorUtility.DisplayDialog("Sala Seguranca VR",
                "Cena 'SalaSegurancaVR' construida em portugues e salva com sucesso!\n\nPressione Play para navegar com WASD + mouse.", "OK");
        }

        // ---------- Helpers ----------
        private static GameObject Header(string name)
        {
            var go = new GameObject(name);
            go.transform.position = Vector3.zero;
            go.isStatic = true;
            return go;
        }

        private static GameObject Prim(PrimitiveType type, string name, Transform parent, Vector3 pos, Vector3 scale, Material mat)
        {
            GameObject go = GameObject.CreatePrimitive(type);
            go.name = name;
            go.transform.SetParent(parent);
            go.transform.position = pos;
            go.transform.localScale = scale;
            var r = go.GetComponent<Renderer>();
            if (r != null && mat != null) r.sharedMaterial = mat;
            return go;
        }

        private static void MakeLeg(Transform parent, string name, Vector3 pos, Material mat)
        {
            Prim(PrimitiveType.Cube, name, parent, pos, new Vector3(0.08f, 0.75f, 0.08f), mat);
        }

        private static Material MakeMat(string name, Color color, float smoothness, bool emissive = false, Color emissionColor = default)
        {
            Shader s = Shader.Find("Universal Render Pipeline/Lit");
            if (s == null) s = Shader.Find("Standard");
            Material m = new Material(s);
            m.SetColor("_BaseColor", color);
            m.SetColor("_Color", color);
            if (m.HasProperty("_Smoothness")) m.SetFloat("_Smoothness", smoothness);
            if (m.HasProperty("_Metallic")) m.SetFloat("_Metallic", smoothness > 0.5f ? 0.8f : 0.0f);
            if (emissive)
            {
                m.EnableKeyword("_EMISSION");
                m.globalIlluminationFlags = MaterialGlobalIlluminationFlags.RealtimeEmissive;
                m.SetColor("_EmissionColor", emissionColor);
            }
            AssetDatabase.CreateAsset(m, MatFolder + "/" + name + ".mat");
            return m;
        }

        private static void EnsureFolder(string path)
        {
            if (AssetDatabase.IsValidFolder(path)) return;
            string parent = Path.GetDirectoryName(path).Replace('\\', '/');
            string leaf = Path.GetFileName(path);
            if (!AssetDatabase.IsValidFolder(parent)) EnsureFolder(parent);
            AssetDatabase.CreateFolder(parent, leaf);
        }
    }
}
