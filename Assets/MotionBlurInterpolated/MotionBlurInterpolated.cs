using UnityEngine;
namespace BodhiDonselaar
{
	[RequireComponent(typeof(Camera))]
	public class MotionBlurInterpolated : MonoBehaviour
	{
		private Camera cam;
		private static Material worldPosMat, motionMaterial, blur, finalMat;
		private static Texture noise;
		private RenderTexture worldPosA, worldPosB, worldPosCurrent;
		private void OnEnable()
		{
			cam = GetComponent<Camera>();
			if (worldPosMat == null) worldPosMat = new Material(Resources.Load<Shader>("PixelWorldPosition"));
			if (motionMaterial == null) motionMaterial = new Material(Resources.Load<Shader>("Motion"));
			if (blur == null) blur = new Material(Resources.Load<Shader>("Blur"));
			if (noise == null) noise = Resources.Load<Texture2D>("NoiseHigh");
			if (finalMat == null) finalMat = new Material(Resources.Load<Shader>("Final"));
			finalMat.SetTexture("_Noise", noise);
		}
		public void OnRenderImage(RenderTexture src, RenderTexture des)
		{
			motionMaterial.SetTexture("_WPOS_PREV", worldPosCurrent);
			if (worldPosCurrent == worldPosB)
			{
				RenderTexture.ReleaseTemporary(worldPosA);
				worldPosA = RenderTexture.GetTemporary(src.width, src.height, 0, RenderTextureFormat.ARGBFloat);
				worldPosCurrent = worldPosA;
			}
			else
			{
				RenderTexture.ReleaseTemporary(worldPosB);
				worldPosB = RenderTexture.GetTemporary(src.width, src.height, 0, RenderTextureFormat.ARGBFloat);
				worldPosCurrent = worldPosB;
			}

			worldPosMat.SetMatrix("_CAMERA_XYZMATRIX", (GL.GetGPUProjectionMatrix(cam.projectionMatrix, false) * cam.worldToCameraMatrix).inverse);
			Graphics.Blit(null, worldPosCurrent, worldPosMat);
			motionMaterial.SetTexture("_WPOS", worldPosCurrent);

			if (worldPosA == null || worldPosB == null) Graphics.Blit(src, des);

			RenderTexture motion = RenderTexture.GetTemporary(src.width / 8, Screen.height / 8, 0, RenderTextureFormat.RGFloat);
			RenderTexture temp = RenderTexture.GetTemporary(motion.width, motion.height, 0, RenderTextureFormat.RGFloat);
			Matrix4x4 world2Screen = cam.projectionMatrix * cam.worldToCameraMatrix;
			Shader.SetGlobalMatrix("_World2Screen", world2Screen);
			Graphics.Blit(null, motion, motionMaterial);
			for (int i = 0; i < 2; ++i)
			{
				blur.SetVector("_Direction", new Vector4(1, 0, 0, 0));
				Graphics.Blit(motion, temp, blur);
				blur.SetVector("_Direction", new Vector4(0, 1, 0, 0));
				Graphics.Blit(temp, motion, blur);
			}
			finalMat.SetTexture("_MotionPixels", motion);
			Graphics.Blit(src, des, finalMat);
			RenderTexture.ReleaseTemporary(temp);
			RenderTexture.ReleaseTemporary(motion);
		}
	}
}