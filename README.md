# Sentis - Texture Processing (Edge Detect)

Import and execute a convolutional edge-detection filter in Unity, using .ONNX and the Sentis API:

<p align="center">
  <img width="100%" src="https://github.com/Unity-Technologies/sentis-edge-detect/blob/main/.github/images/sentis-sobel.gif?raw=true" alt="sentis-sobel">
</p>

The model executes at around 0.22ms on an RTX 3080 Ti mobile, using the DirectX12 (DirectML) backend:
<p align="center">
  <img width="100%" src="https://github.com/Unity-Technologies/sentis-edge-detect/blob/main/.github/images/gpu-time.png?raw=true" alt="gpu-time">
</p>

## Requirements
- Unity 6000.1.0b6 or above
- Sentis 2.1.2 or above 

## Platform Support
- Sentis via DirectML: Windows Editors and Players using DirectX12
- Sentis via Compute: All plaforms with Compute Shader support

## How to use
1. Open the "SentisDemo" scene (`\Assets\Demo\Scenes\SentisModel.unity`)
2. Hit play
3. You can experiment with Sentis by loading other .ONNX models, replacing the edge-detect model used in this example:

<p align="left">
  <img width="50%" src="https://github.com/Unity-Technologies/sentis-edge-detect/blob/main/.github/images/settings.png?raw=true" alt="settings">
</p>

To learn more about importing Sentis models, refer to the official documentation: https://docs.unity3d.com/Packages/com.unity.sentis@2.1/manual/supported-models.html 

---------

This project demonstrates the processing of textures using Sentis. In `\Assets\Demo\Scripts\SentisModel.cs` script, we first load our model and create a Sentis worker. We also create an input tensor matching our model's input dimensions, as well as an output Render Texture:

```
    [SerializeField] private BackendType backendType = BackendType.GPUCompute;
    [SerializeField] private ModelAsset modelAsset;

    private Model model;
    private Worker worker;

    private Tensor<float> inputTensor;
    private RenderTexture outputTexture;

    private int w = 512;
    private int h = 512;

    private void Start()
    {   
        model = ModelLoader.Load(modelAsset);
        worker = new Worker(model, backendType);

        inputTensor = new Tensor<float>(new TensorShape(1, 1, h, w));
        outputTexture = new RenderTexture(w, h, 0);
    }

```

Once a frame, we convert our input RenderTexture into an input tensor. Then schedule the execution of our Sentis model. The model's output tensor is then converted back into a RenderTexture:
```

    [SerializeField] private RenderTexture inputTexture;

    private Tensor<float> outputTensor;

    private void Update()
    {
        TextureConverter.ToTensor(inputTexture, inputTensor, new TextureTransform());
        worker.Schedule(inputTensor);
        outputTensor = worker.PeekOutput() as Tensor<float>;

        TextureTransform settings = new TextureTransform().SetBroadcastChannels(false).SetDimensions(w, h, 4);
        TextureConverter.RenderToTexture(outputTensor, outputTexture, settings);
    }

```

In this basic example, we choose to display the output RenderTexture using a UI canvas and RawImage:

```

    [SerializeField] private RawImage outputImage;
    private RenderTexture outputTexture;

    private void Start()
    {
	.....
        outputImage.texture = outputTexture;
    }

```

