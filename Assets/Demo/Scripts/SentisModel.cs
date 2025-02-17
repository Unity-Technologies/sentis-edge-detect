using System;
using System.Collections;
using Unity.Sentis;
using Unity.Sentis.Layers;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class SentisModel : MonoBehaviour
{
    [SerializeField] private BackendType backendType = BackendType.GPUCompute;
    [SerializeField] private ModelAsset modelAsset;
    [SerializeField] private RenderTexture inputTexture;
    [SerializeField] private RawImage outputImage;

    private int w = 512;
    private int h = 512;

    private Model model;
    private Worker worker;
    private Tensor<float> inputTensor;
    private Tensor<float> outputTensor;
    private RenderTexture outputTexture;

    private void Start()
    {   
        inputTensor = new Tensor<float>(new TensorShape(1, 1, h, w));
        model = ModelLoader.Load(modelAsset);
        worker = new Worker(model, backendType);
        outputTexture = new RenderTexture(w, h, 0);
        outputImage.texture = outputTexture;
    }

    private void Update()
    {
        TextureConverter.ToTensor(inputTexture, inputTensor, new TextureTransform());
        worker.Schedule(inputTensor);
        outputTensor = worker.PeekOutput() as Tensor<float>;

        TextureTransform settings = new TextureTransform().SetBroadcastChannels(false).SetDimensions(w, h, 4);
        TextureConverter.RenderToTexture(outputTensor, outputTexture, settings);
    }

    private void OnDisable()
    {
        inputTexture.Release();
        worker.Dispose();
        inputTensor.Dispose();
        outputTensor.Dispose();
        outputTexture.Release();
    }
}