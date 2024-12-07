using UnityEngine;

namespace Cheese.GOAP.Demo
{
    [System.Serializable]
    public class NoiseHelper
    {
        public float noiseSpeed;
        public float noiseIntensity;
        public int noiseLayers;

        private float xOffset;
        private float timeOffset;

        public void Setup()
        {
            xOffset = Random.Range(-100f, 100f);
            timeOffset = Random.Range(-60f, 60f);
        }

        public float Evaluate()
        {
            float sum = 0;
            for (int i = 0; i < noiseLayers; i++)
            {
                sum += EvaluateLayer(i);
            }
            return sum;
        }

        public float EvaluateLayer(int layer)
        {
            float scale = Mathf.Pow(2, layer);
            return ((Mathf.PerlinNoise(xOffset, timeOffset + Time.time * noiseSpeed * scale) / scale) * 2f - 1f) * noiseIntensity;
        }
    }

    [System.Serializable]
    public class NoiseHelper3D
    {
        public float noiseSpeed;
        public float noiseIntensity;
        public int noiseLayers;

        private NoiseHelper xNoise = new NoiseHelper();
        private NoiseHelper yNoise = new NoiseHelper();
        private NoiseHelper zNoise = new NoiseHelper();

        public void Setup()
        {
            xNoise.noiseSpeed = noiseSpeed;
            xNoise.noiseIntensity = noiseIntensity;
            xNoise.noiseLayers = noiseLayers;
            xNoise.Setup();

            yNoise.noiseSpeed = noiseSpeed;
            yNoise.noiseIntensity = noiseIntensity;
            yNoise.noiseLayers = noiseLayers;
            yNoise.Setup();

            zNoise.noiseSpeed = noiseSpeed;
            zNoise.noiseIntensity = noiseIntensity;
            zNoise.noiseLayers = noiseLayers;
            zNoise.Setup();

        }

        public Vector3 Evaluate()
        {
            return new Vector3(xNoise.Evaluate(), yNoise.Evaluate(), zNoise.Evaluate());
        }
    }
}
