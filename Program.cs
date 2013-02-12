using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace ModelViewer
{
	static class Program
	{
		/// <summary>
		/// The main entry point for the application.
		/// </summary>
		[STAThread]
		static void Main()
		{
			Application.EnableVisualStyles();
			Application.SetCompatibleTextRenderingDefault(true);

			GUI newGUI = new GUI();

			Application.Run(newGUI);
		}
	}
}