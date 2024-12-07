using UnityEngine;

namespace Cheese.GOAP.Demo
{
    public class HeightMapGenerator : MonoBehaviour
    {
        public MeshFilter mf;
        public MeshCollider mc;

        public float width;
        public float height;

        public int collums;
        public int rows;

        public Texture2D texture;

        public float minAlt;
        public float maxAlt;

        public AnimationCurve edgeSmoothing;

        private void Start()
        {
            //GenerateTerrainMesh();
        }

        public float GetAlt(Vector2 pos)
        {
            return Mathf.Lerp(minAlt, maxAlt, texture.GetPixelBilinear(pos.x, pos.y).maxColorComponent * edgeSmoothing.Evaluate(Mathf.Min(pos.y, 1f - pos.y, pos.x, 1f - pos.x)));
        }

        [ContextMenu("Generate")]
        private void GenerateTerrainMesh()
        {
            Vector3[] verts;
            Vector2[] uvs;
            int[] triangles;

            int arrayLength = (collums + 1) * (rows + 1);

            Vector3[] resizedArray = new Vector3[arrayLength];
            Vector2[] resizedArray2 = new Vector2[arrayLength];


            float xLength = width / (float)collums;
            float yLength = height / (float)rows;


            for (int x = 0; x < collums + 1; x++)
            {
                for (int y = 0; y < rows + 1; y++)
                {
                    Vector3 pos = new Vector3(x * xLength, GetAlt(new Vector2((float)x / (float)collums, (float)y / (float)rows)), y * yLength);
                    resizedArray[x * (rows + 1) + y] = pos;
                    resizedArray2[x * (rows + 1) + y] = new Vector2(x / (float)collums, y / (float)rows);
                }
            }

            verts = resizedArray;
            uvs = resizedArray2;

            int[] resizedArray3 = new int[collums * rows * 6];
            int count = 0;

            for (int x = 0; x < rows; x++)
            {
                for (int y = 0; y < collums; y++)
                {

                    resizedArray3[count] = x + 1 + (y + 0) * (rows + 1);
                    count += 1;
                    resizedArray3[count] = x + 0 + (y + 1) * (rows + 1);
                    count += 1;
                    resizedArray3[count] = x + 0 + (y + 0) * (rows + 1);
                    count += 1;


                    resizedArray3[count] = x + 1 + (y + 1) * (rows + 1);
                    count += 1;
                    resizedArray3[count] = x + 0 + (y + 1) * (rows + 1);
                    count += 1;
                    resizedArray3[count] = x + 1 + (y + 0) * (rows + 1);
                    count += 1;
                }
            }

            triangles = resizedArray3;

            OnTileCompleted(verts, uvs, triangles);
        }

        public void OnTileCompleted(Vector3[] verts, Vector2[] uvs, int[] triangles)
        {
            Mesh mesh = new Mesh();
            mesh.Clear();

            mesh.vertices = verts;
            mesh.uv = uvs;
            mesh.triangles = triangles;

            SetMesh(mesh);
        }

        public void SetMesh(Mesh mesh)
        {
            mf.mesh = mesh;

            mf.mesh.RecalculateBounds();
            mf.mesh.RecalculateNormals();
            mf.mesh.RecalculateTangents();

            if (mc != null)
            {
                mc.sharedMesh = null;
                mc.sharedMesh = mf.mesh;
            }
        }
    }
}