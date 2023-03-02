using SharpDX;
using SharpDX.Direct3D11;
using SharpDX.DXGI;

namespace ComputerGraphics.camera
{
    public class Camera
    {
        public Vector3 position = new Vector3(0, 0, -3);
        public Vector3 target = new Vector3(0, 0, 0);
        public Vector3 up = new Vector3(0, 1, 0); //dont understand, X rotate, Y mirror, Z ?
        public float nearPlane = 0.1f;
        public float farPlane = 1000f;
        public Matrix viewMatrix;
        public Matrix projectionMatrix;
        public Matrix viewProjectionMatrix;

        public void Update()
        {
            viewMatrix = Matrix.LookAtLH(position, target, up);
            projectionMatrix = Matrix.PerspectiveFovLH(MathUtil.PiOverFour, 1.0f, 0.1f, 1000.0f);
            Matrix worldMatrix = Matrix.Identity;
            viewProjectionMatrix = worldMatrix * viewMatrix * projectionMatrix;
        }   
    }
}
