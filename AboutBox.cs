using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace ModelViewer
{
	/// <summary>
	/// Form to display the "about" information.
	/// </summary>
	public partial class AboutBox : Form
	{
		public AboutBox()
		{
			InitializeComponent();
		}

		/// <summary>
		/// Opens up the default web browser and loads the website.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void websiteLinkLabel_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
		{
			System.Diagnostics.Process.Start("http://craig.young.81.googlepages.com/");
		}

		/// <summary>
		/// Close the form
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void closeButton_Click(object sender, EventArgs e)
		{
			this.Close();
		}
	}
}