using UnityEngine;

namespace DoaT
{
    public class TheodenDebugLine : MonoBehaviour
    {
        private static readonly int Tint = Shader.PropertyToID("_Tint");

        public bool debug;
        public TheodenController controller;

        public LineRenderer Movement;
        public LineRenderer Aiming;

        public Shader Shader;

        private Material movementMaterial;
        private Material aimingMaterial;

        [ColorUsage(true, true)] public Color movementColor;
        [ColorUsage(true, true)] public Color aimingColor;

        private void Start()
        {
            movementMaterial = new Material(Shader);
            aimingMaterial = new Material(Shader);

            movementMaterial.SetColor(Tint, movementColor);
            aimingMaterial.SetColor(Tint, aimingColor);


            Movement.material = movementMaterial;
            Aiming.material = aimingMaterial;
        }

        private void Update()
        {
            if (debug)
            {
                Movement.gameObject.SetActive(true);
                Aiming.gameObject.SetActive(true);

                var I = transform.position;

                var mF = I + controller.MovementDirection;
                var aF = I + controller.AimingDirection;

                Movement.SetPositions(new Vector3[2] {I, mF});
                Aiming.SetPositions(new Vector3[2] {I, aF});
            }
            else
            {
                Movement.gameObject.SetActive(false);
                Aiming.gameObject.SetActive(false);
            }
        }
    }
}