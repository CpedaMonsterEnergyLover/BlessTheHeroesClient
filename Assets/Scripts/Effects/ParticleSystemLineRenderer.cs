using Camera;
using UnityEngine;

namespace Effects
{
    [RequireComponent(typeof(LineRenderer)),
     RequireComponent(typeof(ParticleSystem))]
    public class ParticleSystemLineRenderer : MonoBehaviour
    {
        [SerializeField] private new ParticleSystem particleSystem;
        [SerializeField] private LineRenderer lineRenderer;
        [SerializeField] private new UnityEngine.Camera camera;

        private ParticleSystem.ShapeModule shapeModule;

        private void Start()
        {
            shapeModule = particleSystem.shape;
            camera = MainCamera.Camera;
        }

        public void UpdateMesh()
        {
            Mesh m = new Mesh();
            lineRenderer.BakeMesh(m, camera);
            shapeModule.mesh = m;
            shapeModule.shapeType = ParticleSystemShapeType.Mesh;
            shapeModule.meshShapeType = ParticleSystemMeshShapeType.Triangle;
        }
    }
}