using System;
using UnityEngine;

namespace Assets.HeroEditor.Common.EditorScripts
{
	/// <summary>
	/// Take a screenshot in play mode [S].
	/// </summary>
	public class Screenshot : MonoBehaviour
	{
		public int SuperSize = 1;
		public string Directory = "Screenshots";

		public string GetPath()
		{
			return string.Format("{0}/Screenshot_{1:yyyy-MM-dd_HH-mm-ss}.png", Directory, DateTime.Now);
		}

		public void Update()
		{
			if (Input.GetKeyDown(KeyCode.S))
			{
				var fileName = GetPath();

				System.IO.Directory.CreateDirectory(Directory);
				ScreenCapture.CaptureScreenshot(fileName, SuperSize);
				Debug.Log(string.Format("Screenshot saved: {0}", fileName));
			}
		}
	}
}