using Camera;
using UnityEngine;

namespace Pooling
{
    public class ParticleSystemLineRenderer : MonoBehaviour
    {
        [SerializeField] private LineRenderer lineRenderer;
        [SerializeField] private new UnityEngine.Camera camera;

        private ParticleSystem.ShapeModule shapeModule;

        private void Start() => camera = MainCamera.Camera;

        public Mesh UpdateMesh(ParticleSystem particles)
        {
            Mesh m = new Mesh();
            lineRenderer.BakeMesh(m, camera);
            shapeModule = particles.shape;
            shapeModule.mesh = m;
            shapeModule.shapeType = ParticleSystemShapeType.Mesh;
            shapeModule.meshShapeType = ParticleSystemMeshShapeType.Triangle;
            return m;
        }
        
        public void UpdateMesh(ParticleSystem particles, Mesh m)
        {
            shapeModule = particles.shape;
            shapeModule.mesh = m;
            shapeModule.shapeType = ParticleSystemShapeType.Mesh;
            shapeModule.meshShapeType = ParticleSystemMeshShapeType.Triangle;
        }
    }
}