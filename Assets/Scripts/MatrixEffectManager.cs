using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class MatrixEffectManager : MonoBehaviour {
    // _____________________________________________
    // Public
    public  Texture       font_texture;
    public  ComputeShader white_noise_generator;
    public  bool          colored;
    // _____________________________________________
    // Private 
    private Camera        cam;
    private CommandBuffer cb;
    private RenderTexture white_noise;
    //private PlayerControls playerInput;
    private int          shader_is_on = 1;

    private void Awake()
    {
        //playerInput = new PlayerControls();
    }

    private void OnEnable()
    {
        //playerInput.Enable();
    }

    private void OnDisable()
    {
        //playerInput.Disable();
    }

    void Start () {
		cam = Camera.main;
        if (!cam) Debug.LogError("Couldnt find the main camera, not such gameobject tagged as mainCamera");

        // -----------------------------------------
        white_noise = new RenderTexture(512, 512, 0)
        {
            name              = "white_noise",
            enableRandomWrite = true,
            useMipMap         = false,
            filterMode        = FilterMode.Point,
            wrapMode          = TextureWrapMode.Repeat
            
        };
        white_noise.Create();
        white_noise_generator.SetTexture(0, "_white_noise",    white_noise);
        // -----------------------------------------
        Shader.SetGlobalTexture("global_font_texture", font_texture);
        Shader.SetGlobalTexture("global_white_noise",  white_noise);
        Shader.SetGlobalInt    ("_session_rand_seed",  Random.Range(0, int.MaxValue));     
        // -----------------------------------------

        cb = new CommandBuffer()
        {
            name = "ScreenSpaceMatrixPass",
        };

        cb.DispatchCompute(white_noise_generator, 0, 512 / 8, 512 / 8, 1);
        cam.AddCommandBuffer(CameraEvent.AfterEverything, cb);
     }

    // Update is called once per frame
    void Update () {
        white_noise_generator.SetInt("_session_rand_seed", Mathf.CeilToInt(Time.time * 6.0f));
        Shader.SetGlobalInt         ("global_colored",     colored ? 1 : 0);
        /*
        if (playerInput.Player.Rain.triggered)
        {
            shader_is_on = (shader_is_on == 0) ? 1 : 0;
            Shader.SetGlobalInteger("_Global_Shader_On", shader_is_on);
        }
        */
        Shader.SetGlobalInteger("_Global_Shader_On", shader_is_on);
    }
}
