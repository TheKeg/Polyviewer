using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.IO;
using System.Windows.Forms;

using Microsoft.DirectX;
using Microsoft.DirectX.Direct3D;

namespace ModelViewer
{
	/// <summary>
	/// Handles events from the user interface.
	/// </summary>
	public partial class GUI : Form
	{
		private ResourceManager resourceManager;	// Engine to handle rendering.
		private Color background;						// Color to clear the render panel to.
		private Timer timer;								// Timer to call the render method.
		private FileInfo fi;								// Used to check the last modified data.
		private AboutBox aBox;							// About Dialog form.
		private int xAxis, yAxis, oldX, oldY;		// x, y coordinates for old and current mouse position.
		private bool mouseClicked;						// if the mouse is held down in the rendering pane.
		private int invertControl;						// Multiplier for if the user wishes to invert camera control.
		private int selectedLight;						// Which light is selected.

		/// <summary>
		/// Constructor. Initializes default values.
		/// </summary>
		public GUI()
		{
			InitializeComponent();

			aBox = new AboutBox();

			timer = new Timer();
			timer.Interval = 15;
			timer.Tick += new EventHandler(RenderPanel);

			background = Color.DarkGray;
			xAxis = yAxis = 0;
			invertControl = 1;
			selectedLight = 0;
			mouseClicked = false;
			
			resourceManager = new ResourceManager(renderPanel);

			// Set the property grid's selected object to the first model.
			viewerProperyGrid.SelectedObject = resourceManager.getModel(0);

			// Setup event handlers for resizing and mousewheels.
			this.ClientSizeChanged += new EventHandler(GUI_Resize);
			this.MouseWheel += new MouseEventHandler(mouseWheel);
			renderPanel.MouseWheel += new MouseEventHandler(mouseWheel);
			objectTreeView.MouseWheel += new MouseEventHandler(mouseWheel);
			viewerProperyGrid.MouseWheel += new MouseEventHandler(mouseWheel);

			// Setup the right click menu for the treeview.
			objectTreeView.ContextMenuStrip = treeViewContextMenu;

			// Copy the texture nodes over to the ground plane node.
			for (int i = 0; i < objectTreeView.Nodes[0].Nodes.Count; i++)
			{
				objectTreeView.Nodes[5].Nodes.Add((TreeNode)objectTreeView.Nodes[0].Nodes[i].Clone());
			}

			// Expand all the nodes within the treeview.
			objectTreeView.ExpandAll();
			objectTreeView.Nodes[5].Collapse();

			resourceManager.Renderer.RenderShadowMap(resourceManager.GetLight(0), 0, resourceManager.Camera.Projection);

			// Start the timer for rendering the scene.
			timer.Start();
		}

		/// <summary>
		/// Event that is called with the timer. Renders the scene.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="eArgs"></param>
		public void RenderPanel(object sender, EventArgs eArgs)
		{
			resourceManager.RenderScene(background);
		}

		/// <summary>
		/// Handles loading from the menu. just calls the other method.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void loadModelToolStripMenuItem_Click(object sender, EventArgs e)
		{
			mouseClicked = false; // precaution to prevent camera/light movement.
			string filename;
			OpenFileDialog fdlg = new OpenFileDialog();

			// Sets the parameters for the open dialog.			
			fdlg.Title = "Load Model";
			fdlg.Filter = "3D files (*.obj, *.x)|*.obj;*.x";
			fdlg.Filter += "|Obj Files (*.obj)|*.obj";
			fdlg.Filter += "|DirectX (*.x)|*.x";
			fdlg.FilterIndex = 1;
			fdlg.RestoreDirectory = true;

			if (fdlg.ShowDialog() == DialogResult.OK)
			{
				filename = fdlg.FileName;

				// String to get the filename
				string[] modelName = filename.Split('\\');

				// Clears any models already loaded.
				if (resourceManager.ModelCount > 1)
					clearModelsToolStripMenuItem_Click(sender, e);

				// Loads the model and sets the data.
				resourceManager.getModel(0).LoadModel(filename);
				resourceManager.getModel(0).Path = filename;
				resourceManager.getModel(0).Enabled = true;

				resourceManager.Camera.ResetCamera();

				for (int i = 0; i < 3; i++)
					resourceManager.GetLight(i).resetLight();

				resourceManager.setupZoom();

				resetTexturesToolStripMenuItem_Click(sender, e);

				// Set the node text to the filename sans directory information.
				objectTreeView.Nodes[0].Text = modelName[modelName.Length - 1];
				objectTreeView.Nodes[0].Checked = true;

				// Set the model as the selected object in the property grid.
				viewerProperyGrid.SelectedObject = resourceManager.getModel(0);

				// Render the shadow map
				resourceManager.Renderer.RenderShadowMap(resourceManager.GetLight(0), 0, resourceManager.Camera.Projection);

				// Enables the option to load additional models.
				openAdditionalModelToolStripMenuItem.Enabled = true;
				loadAdditionalModelToolStripMenuItem.Enabled = true;
			}

         for (int i = 0; i < objectTreeView.Nodes[0].Nodes.Count; i++)
         {
            objectTreeView.Nodes[0].Nodes[i].Checked = false;
         }
		}

      /// <summary>
      /// Cleans up and closes the form.
      /// </summary>
      /// <param name="sender"></param>
      /// <param name="e"></param>
      private void exitToolStripMenuItem_Click(object sender, EventArgs e)
		{
			Close();
		}

		/// <summary>
		/// Changes the background color for the renderer.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void changeBackgroundToolStripMenuItem_Click(object sender, EventArgs e)
		{
			//Creates a new color dialog
			ColorDialog cd = new ColorDialog();

			// Sets the paramaters for the dialog.
			cd.AnyColor = true;
			cd.AllowFullOpen = true;
			cd.FullOpen = true;
			cd.Color = background;

			if (cd.ShowDialog() == DialogResult.OK)
			{
				// Sets the background to the selected color.
				background = cd.Color;
			}
		}

		/// <summary>
		/// Resets the camera position.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void resetCameraToolStripMenuItem_Click(object sender, EventArgs e)
		{
			// Resets the camera to the default position.
			resourceManager.Camera.ResetCamera();

			// Zooms out the camera enough to view the whole model.
			resourceManager.setupZoom();

			// Refresh the property grid.
			viewerProperyGrid.Refresh();
		}

		/// <summary>
		/// Menu option for reseting the light.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void resetLightToolStripMenuItem_Click(object sender, EventArgs e)
		{
			// Iterate through all 3 lights and reset them.
			for (int i = 0; i < 3; i++)
				resourceManager.GetLight(i).resetLight();

			resourceManager.Renderer.RenderShadowMap(resourceManager.GetLight(selectedLight), selectedLight, resourceManager.Camera.Projection);

			// Disable the second and third lights.
			resourceManager.GetLight(1).Enabled = false;
			resourceManager.GetLight(2).Enabled = false;

			// Uncheck the second and third lights.
			objectTreeView.Nodes[objectTreeView.Nodes.Count - 3].Checked = false;
			objectTreeView.Nodes[objectTreeView.Nodes.Count - 2].Checked = false;

			light1ToolStripMenuItem_Click(sender, e);
			resourceManager.Renderer.RenderShadowMap(resourceManager.GetLight(selectedLight), selectedLight, resourceManager.Camera.Projection);

			viewerProperyGrid.Refresh();
		}

		/// <summary>
		/// Resets all textures to blank.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void resetTexturesToolStripMenuItem_Click(object sender, EventArgs e)
		{
			// Iterates through all the models and clears all their textures.
			for (int i = 0; i < resourceManager.ModelCount; i++)
			{
				resourceManager.getModel(i).Diffuse.Path = "";
				resourceManager.getModel(i).Normal.Path = "";
				resourceManager.getModel(i).Specular.Path = "";
				resourceManager.getModel(i).Ambient.Path = "";
				resourceManager.getModel(i).Emissive.Path = "";
			}

			resourceManager.GroundPlane.Ambient.Path = "";
			resourceManager.GroundPlane.Diffuse.Path = "";
			resourceManager.GroundPlane.Emissive.Path = "";
			resourceManager.GroundPlane.Normal.Path = "";
			resourceManager.GroundPlane.Specular.Path = "";
		}

		/// <summary>
		/// Handles when the mouse is clicked.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void renderPanel_MouseDown(object sender, MouseEventArgs e)
		{
			// Sets the x and y positions to the current mouse position.
			xAxis = oldX = e.X;
			yAxis = oldY = e.Y;

			mouseClicked = true;
		}

		/// <summary>
		/// Handles mouse movement when a button is clicked.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void renderPanel_MouseMove(object sender, MouseEventArgs e)
		{
			if (mouseClicked)
			{
				float xValue, yValue;

				// Set the values to the new position.
				xAxis = e.X;
				yAxis = e.Y;

				// Get the distance moved.
				xValue = xAxis - oldX;
				yValue = yAxis - oldY;

				// Handles rotation of the camera/light.
				if (e.Button == MouseButtons.Left)
				{
					// Adjusts the values.
					xValue *= 0.005f * invertControl;
					yValue *= 0.005f * invertControl;

					// If the ctrl button is held down the light is rotated, else the camera is.
					if (Control.ModifierKeys == Keys.Control)
					{
						resourceManager.GetLight(selectedLight).rotateLight(xValue, yValue);
						
						if(resourceManager.GetLight(selectedLight).Shadows && resourceManager.GetLight(selectedLight).Enabled && selectedLight == 0)
						   resourceManager.Renderer.RenderShadowMap(resourceManager.GetLight(selectedLight), selectedLight, resourceManager.Camera.Projection);
					}
					else
						resourceManager.Camera.RotateCamera(xValue, yValue);
				}
				// Handles zooming of the camera.
				else if (e.Button == MouseButtons.Right)
				{
					if (yValue < 0)
					{
						resourceManager.Camera.ZoomCamera(true); // Zoom the camera in
					}
					else if (yValue > 0)
					{
						resourceManager.Camera.ZoomCamera(false); // Zoom the camera out.
					}
				}
				// Handles moving the camera.
				else if (e.Button == MouseButtons.Middle)
				{
					// Adjusts the values for smother movement.
					xValue *= -0.1f * invertControl;
					yValue *= -0.1f * invertControl;

					resourceManager.Camera.MoveCamera(xValue, yValue);
				}

				// Sets the current mouse position to the old position.
				oldX = xAxis;
				oldY = yAxis;
			}
		}

		/// <summary>
		/// Handles when the mouse button is release.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void renderPanel_MouseUp(object sender, MouseEventArgs e)
		{
			mouseClicked = false;
			viewerProperyGrid.Refresh();
		}

		/// <summary>
		/// Method to handle scrolling of the mouse wheel.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		void mouseWheel(object sender, MouseEventArgs e)
		{
			// Scroll wheel alters the light intensity if ctrl is held down.
			if (Control.ModifierKeys == Keys.Control)
			{
				resourceManager.GetLight(selectedLight).Intensity += e.Delta * 0.0005f;
			}
			// Default case is to zoom the camera in/out.
			else
			{
				if (e.Delta > 0)
				{
					resourceManager.Camera.ZoomCamera(true); // Zoom the camera in
				}
				else if (e.Delta < 0)
				{
					resourceManager.Camera.ZoomCamera(false); // Zoom the camera out.
				}
			}

			viewerProperyGrid.Refresh();
		}

		/// <summary>
		/// Handles switching the object shown in the property grid.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void objectTreeView_AfterSelect(object sender, TreeViewEventArgs e)
		{
			try
			{
				switch (objectTreeView.SelectedNode.Name)
				{
					case "cameraNode":
						viewerProperyGrid.SelectedObject = resourceManager.Camera;
						break;

					case "lightOneNode":
						viewerProperyGrid.SelectedObject = resourceManager.GetLight(0);
                  light1ToolStripMenuItem_Click(sender, e);
						break;

					case "lightTwoNode":
						viewerProperyGrid.SelectedObject = resourceManager.GetLight(1);
                  light2ToolStripMenuItem_Click(sender, e);
						break;

					case "lightThreeNode":
						viewerProperyGrid.SelectedObject = resourceManager.GetLight(2);
                  light3ToolStripMenuItem_Click(sender, e);
						break;

					default:
						// Handles selection of the model nodes and their texture child nodes.
						if (objectTreeView.SelectedNode.Name.Contains("model"))
							viewerProperyGrid.SelectedObject = resourceManager.getModel(objectTreeView.SelectedNode.Index);
						else if (objectTreeView.SelectedNode.Name.Contains("ground"))
							viewerProperyGrid.SelectedObject = resourceManager.GroundPlane;
						else if (objectTreeView.SelectedNode.Parent.Name.Contains("model"))
							selectTextureProperty(objectTreeView.SelectedNode.Parent.Index);
						else if (objectTreeView.SelectedNode.Parent.Name.Contains("ground"))
							selectTextureProperty(-1);

						break;
				}


			}
			catch (NullReferenceException)
			{ }

			return;
		}

		/// <summary>
		/// Compares the child node names against the selected tree node.
		/// </summary>
		/// <param name="num">Index of the model into the model list.</param>
		private void selectTextureProperty(int num)
		{
			switch (objectTreeView.SelectedNode.Name)
			{
				case "ambientOccChildNode":
					if(num >= 0)
						viewerProperyGrid.SelectedObject = resourceManager.getModel(num).Ambient;
					else
						viewerProperyGrid.SelectedObject = resourceManager.GroundPlane.Ambient;
					break;

				case "diffuseChildNode":
					if (num >= 0)
						viewerProperyGrid.SelectedObject = resourceManager.getModel(num).Diffuse;
					else
						viewerProperyGrid.SelectedObject = resourceManager.GroundPlane.Diffuse;
					break;

				case "emissiveChildNode":
					if (num >= 0)
						viewerProperyGrid.SelectedObject = resourceManager.getModel(num).Emissive;
					else
						viewerProperyGrid.SelectedObject = resourceManager.GroundPlane.Emissive;
					break;

				case "normalChildNode":
					if (num >= 0)
						viewerProperyGrid.SelectedObject = resourceManager.getModel(num).Normal;
					else
						viewerProperyGrid.SelectedObject = resourceManager.GroundPlane.Normal;
					break;

				case "specularChildNode":
					if (num >= 0)
						viewerProperyGrid.SelectedObject = resourceManager.getModel(num).Specular;
					else
						viewerProperyGrid.SelectedObject = resourceManager.GroundPlane.Specular;
					break;
			}
		}

		/// <summary>
		/// Checks to see if the property changed is the model. If it is, the camera is zoomed properly and reset.
		/// </summary>
		/// <param name="s"></param>
		/// <param name="e"></param>
		private void viewerProperyGrid_PropertyValueChanged(object s, PropertyValueChangedEventArgs e)
		{
			PropertyGrid tmp = (PropertyGrid)s;

			// Resets the camera and lights if the default model's property is changed 
			// (only option is to load a new mesh).
			if (tmp.SelectedObject.Equals(resourceManager.getModel(0)))
			{
				resourceManager.Camera.ResetCamera();

				for (int i = 0; i < 3; i++)
					resourceManager.GetLight(i).resetLight();

				resourceManager.setupZoom();
			}

			objectTreeView.SelectedNode.Checked = true;
		}

		/// <summary>
		/// Enables/Disables various parameters.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void objectTreeView_AfterCheck(object sender, TreeViewEventArgs e)
		{
			try
			{
				switch (e.Node.Name)
				{
					// Ignore the check value it is the camera.
					case "cameraNode":
						break;

					case "lightOneNode":
						resourceManager.GetLight(0).Enabled = e.Node.Checked;
						break;

					case "lightTwoNode":
						resourceManager.GetLight(1).Enabled = e.Node.Checked;
						break;

					case "lightThreeNode":
						resourceManager.GetLight(2).Enabled = e.Node.Checked;
						break;

					default:
						if (e.Node.Name.Contains("model"))
							resourceManager.getModel(e.Node.Index).Enabled = e.Node.Checked;
						else if (e.Node.Name.Contains("ground"))
						{
							resourceManager.GroundPlane.Enabled = e.Node.Checked;
							showGroundPlaneToolStripMenuItem.Checked = e.Node.Checked;
						}
						else if (e.Node.Parent.Name.Contains("model"))
							setEnabledState(e.Node.Parent.Index, e.Node.Name, e.Node.Checked);
						else if (e.Node.Parent.Name.Contains("ground"))
							setEnabledState(-1, e.Node.Name, e.Node.Checked);

						break;
				}
			}
			catch (StackOverflowException) { }
			catch (NullReferenceException) { }
		}

		/// <summary>
		/// Enables/Disables a texture node.
		/// </summary>
		/// <param name="num">Index into the model list.</param>
		/// <param name="name">Name of the node.</param>
		/// <param name="checkedState">The checked state.</param>
		private void setEnabledState(int num, string name, bool checkedState)
		{
			switch (name)
			{
				case "ambientOccChildNode":
					if (num >= 0)
					{
						if (!resourceManager.getModel(num).Ambient.Path.Equals(""))
							resourceManager.getModel(num).Ambient.Enable = checkedState;
					}
					else
					{
						if (!resourceManager.GroundPlane.Ambient.Path.Equals(""))
							resourceManager.GroundPlane.Ambient.Enable = checkedState;
					}
					break;

				case "diffuseChildNode":
					if (num >= 0)
					{
						if (!resourceManager.getModel(num).Diffuse.Path.Equals(""))
							resourceManager.getModel(num).Diffuse.Enable = checkedState;
					}
					else
					{
						if (!resourceManager.GroundPlane.Diffuse.Path.Equals(""))
							resourceManager.GroundPlane.Diffuse.Enable = checkedState;
					}
					break;

				case "emissiveChildNode":
					if (num >= 0)
					{
						if (!resourceManager.getModel(num).Emissive.Path.Equals(""))
							resourceManager.getModel(num).Emissive.Enable = checkedState;
					}
					else
					{
						if (!resourceManager.GroundPlane.Emissive.Path.Equals(""))
							resourceManager.GroundPlane.Emissive.Enable = checkedState;
					}
					break;

				case "normalChildNode":
					if (num >= 0)
					{
						if (!resourceManager.getModel(num).Normal.Path.Equals(""))
							resourceManager.getModel(num).Normal.Enable = checkedState;
					}
					else
					{
						if (!resourceManager.GroundPlane.Normal.Path.Equals(""))
							resourceManager.GroundPlane.Normal.Enable = checkedState;
					}
					break;

				case "specularChildNode":
					if (num >= 0)
					{
						if (!resourceManager.getModel(num).Specular.Path.Equals(""))
							resourceManager.getModel(num).Specular.Enable = checkedState;
					}
					else
					{
						if (!resourceManager.GroundPlane.Specular.Path.Equals(""))
							resourceManager.GroundPlane.Specular.Enable = checkedState;
					}
					break;
			}
		}

		/// <summary>
		/// Event handler for when the user returns to this program.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void GUI_Activated(object sender, EventArgs e)
		{
			// Checks to see if the option to not check textures for updates is enabled.
			if (!enableTextureRefreshToolStripMenuItem.Checked)
			{
				timer.Start();
				return;
			}

			// Goes through the model list checking each texture if the path is not null
			// and then checks to see if the texture has been modified since last loaded in.
			for (int i = 0; i < resourceManager.ModelCount; i++)
			{
				if (!resourceManager.getModel(i).Diffuse.Path.Equals(""))
				{
					fi = new FileInfo(resourceManager.getModel(i).Diffuse.Path);

					if (fi.LastWriteTime.CompareTo(resourceManager.getModel(i).Diffuse.LastModified) > 0)
					{
						resourceManager.getModel(i).Diffuse.Texture = TextureLoader.FromFile(resourceManager.Device, 
																													resourceManager.getModel(i).Diffuse.Path, 
																													0, 0, 3, Usage.None, Format.A8B8G8R8,
																													Pool.Managed, Filter.Linear, Filter.Box, 0);
					}
				}

				if (!resourceManager.getModel(i).Ambient.Path.Equals(""))
				{
					fi = new FileInfo(resourceManager.getModel(i).Ambient.Path);

					if (fi.LastWriteTime.CompareTo(resourceManager.getModel(i).Ambient.LastModified) > 0)
					{
						resourceManager.getModel(i).Ambient.Texture = TextureLoader.FromFile(resourceManager.Device, 
																													resourceManager.getModel(i).Ambient.Path, 
																													0, 0, 3, Usage.None, Format.A8B8G8R8,
																													Pool.Managed, Filter.Linear, Filter.Box, 0);
					}
				}

				if (!resourceManager.getModel(i).Specular.Path.Equals(""))
				{
					fi = new FileInfo(resourceManager.getModel(i).Specular.Path);

					if (fi.LastWriteTime.CompareTo(resourceManager.getModel(i).Specular.LastModified) > 0)
					{
						resourceManager.getModel(i).Specular.Texture = TextureLoader.FromFile(resourceManager.Device, 
																													 resourceManager.getModel(i).Specular.Path, 
																													 0, 0, 3, Usage.None, Format.A8B8G8R8,
																													 Pool.Managed, Filter.Linear, Filter.Box, 0);
					}
				}

				if (!resourceManager.getModel(i).Normal.Path.Equals(""))
				{
					fi = new FileInfo(resourceManager.getModel(i).Normal.Path);

					if (fi.LastWriteTime.CompareTo(resourceManager.getModel(i).Normal.LastModified) > 0)
					{
						resourceManager.getModel(i).Normal.Texture = TextureLoader.FromFile(resourceManager.Device, 
																												  resourceManager.getModel(i).Normal.Path, 
																												  0, 0, 3, Usage.None, Format.A8B8G8R8,
																												  Pool.Managed, Filter.Linear, Filter.Box, 0);
					}
				}

				if (!resourceManager.getModel(i).Emissive.Path.Equals(""))
				{
					fi = new FileInfo(resourceManager.getModel(i).Emissive.Path);

					if (fi.LastWriteTime.CompareTo(resourceManager.getModel(i).Emissive.LastModified) > 0)
					{
						resourceManager.getModel(i).Emissive.Texture = TextureLoader.FromFile(resourceManager.Device, 
																													 resourceManager.getModel(i).Emissive.Path, 
																													 0, 0, 3, Usage.None, Format.A8B8G8R8, 
																													 Pool.Managed, Filter.Linear, Filter.Box, 0);
					}
				}
			}

			if (!resourceManager.GroundPlane.Diffuse.Path.Equals(""))
			{
				fi = new FileInfo(resourceManager.GroundPlane.Diffuse.Path);

				if (fi.LastWriteTime.CompareTo(resourceManager.GroundPlane.Diffuse.LastModified) > 0)
				{
					resourceManager.GroundPlane.Diffuse.Texture = TextureLoader.FromFile(resourceManager.Device,
																												resourceManager.GroundPlane.Diffuse.Path,
																												0, 0, 3, Usage.None, Format.A8B8G8R8,
																												Pool.Managed, Filter.Linear, Filter.Box, 0);
				}
			}

			if (!resourceManager.GroundPlane.Ambient.Path.Equals(""))
			{
				fi = new FileInfo(resourceManager.GroundPlane.Ambient.Path);

				if (fi.LastWriteTime.CompareTo(resourceManager.GroundPlane.Ambient.LastModified) > 0)
				{
					resourceManager.GroundPlane.Ambient.Texture = TextureLoader.FromFile(resourceManager.Device,
																												resourceManager.GroundPlane.Ambient.Path,
																												0, 0, 3, Usage.None, Format.A8B8G8R8,
																												Pool.Managed, Filter.Linear, Filter.Box, 0);
				}
			}

			if (!resourceManager.GroundPlane.Specular.Path.Equals(""))
			{
				fi = new FileInfo(resourceManager.GroundPlane.Specular.Path);

				if (fi.LastWriteTime.CompareTo(resourceManager.GroundPlane.Specular.LastModified) > 0)
				{
					resourceManager.GroundPlane.Specular.Texture = TextureLoader.FromFile(resourceManager.Device,
																												 resourceManager.GroundPlane.Specular.Path,
																												 0, 0, 3, Usage.None, Format.A8B8G8R8,
																												 Pool.Managed, Filter.Linear, Filter.Box, 0);
				}
			}

			if (!resourceManager.GroundPlane.Normal.Path.Equals(""))
			{
				fi = new FileInfo(resourceManager.GroundPlane.Normal.Path);

				if (fi.LastWriteTime.CompareTo(resourceManager.GroundPlane.Normal.LastModified) > 0)
				{
					resourceManager.GroundPlane.Normal.Texture = TextureLoader.FromFile(resourceManager.Device,
																											  resourceManager.GroundPlane.Normal.Path,
																											  0, 0, 3, Usage.None, Format.A8B8G8R8,
																											  Pool.Managed, Filter.Linear, Filter.Box, 0);
				}
			}

			if (!resourceManager.GroundPlane.Emissive.Path.Equals(""))
			{
				fi = new FileInfo(resourceManager.GroundPlane.Emissive.Path);

				if (fi.LastWriteTime.CompareTo(resourceManager.GroundPlane.Emissive.LastModified) > 0)
				{
					resourceManager.GroundPlane.Emissive.Texture = TextureLoader.FromFile(resourceManager.Device,
																												 resourceManager.GroundPlane.Emissive.Path,
																												 0, 0, 3, Usage.None, Format.A8B8G8R8,
																												 Pool.Managed, Filter.Linear, Filter.Box, 0);
				}
			}

			timer.Start();
		}

		/// <summary>
		/// Event handler for when this program loses focus.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void GUI_Deactivate(object sender, EventArgs e)
		{
			// Stops the timer, thus stopping rendering when not in focus.
			timer.Stop();
		}

		/// <summary>
		/// Event handler for when the program is closing.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void GUI_FormClosing(object sender, FormClosingEventArgs e)
		{
			// Stop the timer.
			timer.Stop();

			// Call the dispose method to release resources.
			resourceManager.Dispose();
		}

		/// <summary>
		/// Event handler for when the user resizes the window.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void GUI_Resize(object sender, EventArgs e)
		{
			resourceManager.adjustCamera(renderPanel.Width, renderPanel.Height);

			if (!resourceManager.Renderer.SceneRender.Disposed)
				resourceManager.Renderer.SceneRender.Dispose();

			if (!resourceManager.Renderer.ShadowRender.Disposed)
				resourceManager.Renderer.ShadowRender.Dispose();

			resourceManager.Renderer.ReleaseRenderTextures();

			if(renderPanel.Width > 0 || renderPanel.Height > 0)
				resourceManager.Device.Reset(resourceManager.PresentParams);

			//resourceManager.device_DeviceResizing();
			//resourceManager.device_DeviceReset();
		}

		/// <summary>
		/// Show the about dailog panel.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void aboutModelViewerToolStripMenuItem_Click(object sender, EventArgs e)
		{
			aBox.ShowDialog(this);
		}

		/// <summary>
		/// Selects the first light.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void light1ToolStripMenuItem_Click(object sender, EventArgs e)
		{
			if (resourceManager.GetLight(0).Enabled)
			{
				// Set the proper checked state for the menu items.
				light1ToolStripMenuItem.Checked = true;
				light2ToolStripMenuItem.Checked = false;
				light3ToolStripMenuItem.Checked = false;

				selectedLight = 0;
				selectedLightLabel.Text = "Light #1 Selected";
				viewerProperyGrid.SelectedObject = resourceManager.GetLight(0);
				objectTreeView.SelectedNode = objectTreeView.Nodes[objectTreeView.Nodes.Count - 4];

				//resourceManager.Renderer./(resourceManager.GetLight(selectedLight), selectedLight, resourceManager.Camera.Projection);
			}
		}

		/// <summary>
		/// Selects the second light.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void light2ToolStripMenuItem_Click(object sender, EventArgs e)
		{
			if (resourceManager.GetLight(1).Enabled)
			{
				// Set the proper checked state for the menu items.
				light1ToolStripMenuItem.Checked = false;
				light2ToolStripMenuItem.Checked = true;
				light3ToolStripMenuItem.Checked = false;

				selectedLight = 1;
				selectedLightLabel.Text = "Light #2 Selected";
				viewerProperyGrid.SelectedObject = resourceManager.GetLight(1);
				objectTreeView.SelectedNode = objectTreeView.Nodes[objectTreeView.Nodes.Count - 3];

				//resourceManager.Renderer.RenderShadowMap(resourceManager.GetLight(selectedLight), selectedLight);
			}
		}

		/// <summary>
		/// Selects the third light.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void light3ToolStripMenuItem_Click(object sender, EventArgs e)
		{
			if (resourceManager.GetLight(2).Enabled)
			{
				// Set the proper checked state for the menu items.
				light1ToolStripMenuItem.Checked = false;
				light2ToolStripMenuItem.Checked = false;
				light3ToolStripMenuItem.Checked = true;

				selectedLight = 2;
				selectedLightLabel.Text = "Light #3 Selected";
				viewerProperyGrid.SelectedObject = resourceManager.GetLight(2);
				objectTreeView.SelectedNode = objectTreeView.Nodes[objectTreeView.Nodes.Count - 2];

				//resourceManager.Renderer.RenderShadowMap(resourceManager.GetLight(selectedLight), selectedLight);
			}
		}

		/// <summary>
		/// Toggles the ground plane.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void showGroundPlaneToolStripMenuItem_Click(object sender, EventArgs e)
		{
			// Toggle the menu checked state, model enabled state, and the tree node's value.
			showGroundPlaneToolStripMenuItem.Checked = !showGroundPlaneToolStripMenuItem.Checked;
			resourceManager.GroundPlane.Enabled = showGroundPlaneToolStripMenuItem.Checked;
			objectTreeView.Nodes[objectTreeView.Nodes.Count - 1].Checked = showGroundPlaneToolStripMenuItem.Checked;

			resourceManager.Renderer.RenderShadowMap(resourceManager.GetLight(selectedLight), selectedLight, resourceManager.Camera.Projection);
		}

		/// <summary>
		/// Toggles the option to update textures upon regaining focus.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void enableTextureRefreshToolStripMenuItem_Click(object sender, EventArgs e)
		{
			enableTextureRefreshToolStripMenuItem.Checked = !enableTextureRefreshToolStripMenuItem.Checked;
		}

		/// <summary>
		/// Writes out the information of the loaded scene to a comma seperated value style file.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void saveSettingsToolStripMenuItem_Click(object sender, EventArgs e)
		{
			SaveFileDialog sfd = new SaveFileDialog();

			sfd.AddExtension = true;
			sfd.Title = "Save Settings";
			sfd.Filter = "Model Viewer Settings (*.mvs)|*.mvs";
			sfd.RestoreDirectory = true;
			sfd.OverwritePrompt = true;

			if (sfd.ShowDialog() == DialogResult.OK)
			{
				StreamWriter sw = new StreamWriter(sfd.FileName);

				sw.WriteLine("#Model Viewer Settings Version 1.0");
				sw.WriteLine(resourceManager.ModelCount);

				for (int i = 1; i < resourceManager.ModelCount; i++)
				{
					sw.WriteLine(resourceManager.getModel(i));
					sw.WriteLine(resourceManager.getModel(i).Ambient);
					sw.WriteLine(resourceManager.getModel(i).Diffuse);
					sw.WriteLine(resourceManager.getModel(i).Emissive);
					sw.WriteLine(resourceManager.getModel(i).Normal);
					sw.WriteLine(resourceManager.getModel(i).Specular);
				}

				sw.WriteLine(resourceManager.Camera);
				sw.WriteLine(resourceManager.GetLight(0));
				sw.WriteLine(resourceManager.GetLight(1));
				sw.WriteLine(resourceManager.GetLight(2));
				sw.WriteLine(background.ToArgb());

				sw.Close();
			}
		}

		/// <summary>
		/// Loads in the scene settings from a file.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void loadSettingsToolStripMenuItem_Click(object sender, EventArgs e)
		{
			OpenFileDialog ofd = new OpenFileDialog();

			ofd.Title = "Load Settings";
			ofd.Filter = "Model Viewer Settings (*.mvs)|*.mvs";
			ofd.RestoreDirectory = true;

			if (ofd.ShowDialog() == DialogResult.OK)
			{
				StreamReader sr = new StreamReader(ofd.FileName);
				int modelCount;

				// Ignore the first line and get the model count from the second line.
				sr.ReadLine();
				modelCount = int.Parse(sr.ReadLine());

				// Go through each model and load in the settings.
				for (int i = 0; i < modelCount; i++)
				{
					string modelPath = sr.ReadLine();
					string[] modelName = modelPath.Split('\\');

					// Add a model to the list if there is multiple models.
					if (i > 1)
						resourceManager.addNewModel(new Model(resourceManager.Device, resourceManager.Effect));

					resourceManager.getModel(i).LoadModel(modelPath);
					resourceManager.getModel(i).Path = modelPath;
					resourceManager.getModel(i).Ambient.FromString(sr.ReadLine());
					resourceManager.getModel(i).Diffuse.FromString(sr.ReadLine());
					resourceManager.getModel(i).Emissive.FromString(sr.ReadLine());
					resourceManager.getModel(i).Normal.FromString(sr.ReadLine());
					resourceManager.getModel(i).Specular.FromString(sr.ReadLine());

					// Make a copy of the bash model node if there is multiple models.
					if (i > 1)
					{
						TreeNode newNode = (TreeNode)objectTreeView.Nodes[0].Clone();

						objectTreeView.Nodes.Insert(objectTreeView.Nodes.Count - 5, newNode);
						objectTreeView.ExpandAll();
					}

					objectTreeView.Nodes[i].Text = modelName[modelName.Length - 1];
					objectTreeView.Nodes[i].Checked = true;

					if (resourceManager.getModel(i).Ambient.Enable)
						objectTreeView.Nodes[i].Nodes[0].Checked = true;

					if (resourceManager.getModel(i).Diffuse.Enable)
						objectTreeView.Nodes[i].Nodes[1].Checked = true;

					if (resourceManager.getModel(i).Emissive.Enable)
						objectTreeView.Nodes[i].Nodes[2].Checked = true;

					if (resourceManager.getModel(i).Normal.Enable)
						objectTreeView.Nodes[i].Nodes[3].Checked = true;

					if (resourceManager.getModel(i).Specular.Enable)
						objectTreeView.Nodes[i].Nodes[4].Checked = true;
				}

				resourceManager.setupZoom();

				resourceManager.Camera.FromString(sr.ReadLine());
				resourceManager.GetLight(0).FromString(sr.ReadLine());
				resourceManager.GetLight(1).FromString(sr.ReadLine());
				resourceManager.GetLight(2).FromString(sr.ReadLine());
				background = Color.FromArgb(int.Parse(sr.ReadLine()));

				if (resourceManager.GetLight(0).Enabled)
					objectTreeView.Nodes[objectTreeView.Nodes.Count - 4].Checked = true;

				if (resourceManager.GetLight(1).Enabled)
					objectTreeView.Nodes[objectTreeView.Nodes.Count - 3].Checked = true;

				if (resourceManager.GetLight(2).Enabled)
					objectTreeView.Nodes[objectTreeView.Nodes.Count - 2].Checked = true;

				sr.Close();

				resourceManager.Camera.MoveCamera(0.0f, 0.0f);

				openAdditionalModelToolStripMenuItem.Enabled = true;
				loadAdditionalModelToolStripMenuItem.Enabled = true;

			}
		}

		/// <summary>
		/// Menu option to toggle inverting the camera controls.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void invertCameraLightControlsToolStripMenuItem_Click(object sender, EventArgs e)
		{
			invertCameraLightControlsToolStripMenuItem.Checked = !invertCameraLightControlsToolStripMenuItem.Checked;

			invertControl *= -1;
		}

		/// <summary>
		/// Method to take a screenshot of the current scene.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void takeScreenshotToolStripMenuItem_Click(object sender, EventArgs e)
		{
			SaveFileDialog sfd = new SaveFileDialog();

			sfd.AddExtension = true;
			sfd.Title = "Save Settings";
			sfd.Filter = "Bitmap *.bmp|*.bmp";
			sfd.Filter += "|DirectX *.dds|*.dds";
			sfd.Filter += "|Jpeg *.jpg|*.jpg";
			sfd.Filter += "|Png *.png|*.png";
			sfd.Filter += "|Targa *.tga|*.tga";
			sfd.RestoreDirectory = true;
			sfd.OverwritePrompt = true;

			if (sfd.ShowDialog() == DialogResult.OK)
			{
				// Switch to handle the format to save the screenshot in.
				switch (sfd.FilterIndex)
				{
					case 1:
						resourceManager.screenshot(sfd.FileName, ImageFileFormat.Bmp, background);
						break;

					case 2:
						resourceManager.screenshot(sfd.FileName, ImageFileFormat.Dds, background);
						break;

					case 3:
						resourceManager.screenshot(sfd.FileName, ImageFileFormat.Jpg, background);
						break;

					case 4:
						resourceManager.screenshot(sfd.FileName, ImageFileFormat.Png, background);
						break;

					case 5:
						resourceManager.screenshot(sfd.FileName, ImageFileFormat.Tga, background);
						break;
				}
			}
		}

		/// <summary>
		/// Method called from the context menu to load a model.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void loadModelToolStripMenuItem1_Click(object sender, EventArgs e)
		{
			loadModelToolStripMenuItem_Click(sender, e);
		}

		/// <summary>
		/// Method to load in an additional model.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void loadAdditionalModelToolStripMenuItem_Click(object sender, EventArgs e)
		{
			mouseClicked = false; // precaution to prevent camera/light movement.
			string filename;
			OpenFileDialog fdlg = new OpenFileDialog();

			// Sets the parameters for the open dialog.			
			fdlg.Title = "Load Model";
			fdlg.Filter = "3D files (*.obj, *.x)|*.obj;*.x";
			fdlg.Filter += "|Obj Files (*.obj)|*.obj";
			fdlg.Filter += "|DirectX (*.x)|*.x";
			fdlg.FilterIndex = 2;
			fdlg.RestoreDirectory = true;

			if (fdlg.ShowDialog() == DialogResult.OK)
			{
				filename = fdlg.FileName;

				string[] modelName = filename.Split('\\');

				Model newModel = new Model(resourceManager.Device, resourceManager.Effect);

				// Add a new model and set the path.
				resourceManager.addNewModel(newModel);
				resourceManager.getModel(resourceManager.ModelCount - 1).LoadModel(filename);
				resourceManager.getModel(resourceManager.ModelCount - 1).Path = filename;

				// Clone the base node and add it after the last known model.
				TreeNode newNode = (TreeNode)objectTreeView.Nodes[0].Clone();
				newNode.Text = modelName[modelName.Length - 1];
				newNode.Checked = true;

				objectTreeView.Nodes.Insert(objectTreeView.Nodes.Count - 5, newNode);
				objectTreeView.ExpandAll();

				resourceManager.setupZoom();
				resourceManager.Renderer.RenderShadowMap(resourceManager.GetLight(selectedLight), selectedLight, resourceManager.Camera.Projection);
			}
		}

		/// <summary>
		/// Method called from the context menu to load an additional model.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void openAdditionalModelToolStripMenuItem_Click(object sender, EventArgs e)
		{
			loadAdditionalModelToolStripMenuItem_Click(sender, e);
		}

		/// <summary>
		/// Method to clean up the tree node and remove all models.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void clearModelsToolStripMenuItem_Click(object sender, EventArgs e)
		{
			resourceManager.ClearModels();

			// Remove all extra model nodes.
			for (int i = 1; i < objectTreeView.Nodes.Count - 5; i++)
			{
				objectTreeView.Nodes.RemoveAt(i);
			}

			// Set the default node values.
			objectTreeView.Nodes[0].Text = "Model";
			objectTreeView.Nodes[0].Checked = false;
			objectTreeView.Nodes[3].Checked = false;
			objectTreeView.Nodes[4].Checked = false;

			// Uncheck all texture nodes.
			for (int i = 0; i < objectTreeView.Nodes[0].Nodes.Count; i++)
				objectTreeView.Nodes[0].Nodes[i].Checked = false;

			// Disable the option to load additional models.
			openAdditionalModelToolStripMenuItem.Enabled = false;
			loadAdditionalModelToolStripMenuItem.Enabled = false;
		}

		/// <summary>
		/// Method to rebuild the normals for all loaded meshes.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void rebuildNormalsToolStripMenuItem_Click(object sender, EventArgs e)
		{
			for (int i = 0; i < resourceManager.ModelCount; i++)
			{
				resourceManager.getModel(i).rebuildNormals();
			}
		}

		private void noneToolStripMenuItem_Click(object sender, EventArgs e)
		{
			timer.Stop();

			PresentParameters tempParams = (PresentParameters)resourceManager.PresentParams;

			if (tempParams.MultiSample == MultiSampleType.None)
				return;

			
			tempParams.MultiSample = MultiSampleType.None;

			resourceManager.PresentParams = tempParams;

			try
			{
				resourceManager.Renderer.SceneRender.OnLostDevice();
				resourceManager.Renderer.ShadowRender.OnLostDevice();

				resourceManager.Renderer.SceneRender.Dispose();
				resourceManager.Renderer.ShadowRender.Dispose();

				resourceManager.Renderer.ReleaseRenderTextures();

				resourceManager.Device.Reset(tempParams);

				resourceManager.Renderer.RenderShadowMap(resourceManager.GetLight(0), 0, resourceManager.Camera.Projection);

				xToolStripMenuItem.Checked = false;
				xToolStripMenuItem1.Checked = false;
				xToolStripMenuItem2.Checked = false;
				noneToolStripMenuItem.Checked = true;
			}
			catch (Exception ex)
			{
				resourceManager.Renderer.RebuildRenderTargets(renderPanel.Width, renderPanel.Height);
				resourceManager.Renderer.RebuildShadowMaps(resourceManager.Renderer.ShadowSize);

				Console.WriteLine(ex);
			}

			timer.Start();
		}

		private void xToolStripMenuItem_Click(object sender, EventArgs e)
		{
			timer.Stop();

			PresentParameters tempParams = (PresentParameters)resourceManager.PresentParams;

			if (tempParams.MultiSample == MultiSampleType.TwoSamples)
				return;

			if (Manager.CheckDeviceMultiSampleType(0, DeviceType.Hardware, Format.X8R8G8B8, true, MultiSampleType.TwoSamples))
				tempParams.MultiSample = MultiSampleType.TwoSamples;
			else
				return;

			resourceManager.PresentParams = tempParams;

			try
			{
				resourceManager.Renderer.SceneRender.OnLostDevice();
				resourceManager.Renderer.ShadowRender.OnLostDevice();

				resourceManager.Renderer.SceneRender.Dispose();
				resourceManager.Renderer.ShadowRender.Dispose();

				resourceManager.Renderer.ReleaseRenderTextures();

				resourceManager.Device.Reset(tempParams);

				resourceManager.Renderer.RenderShadowMap(resourceManager.GetLight(0), 0, resourceManager.Camera.Projection);

				xToolStripMenuItem.Checked = true;
				xToolStripMenuItem1.Checked = false;
				xToolStripMenuItem2.Checked = false;
				noneToolStripMenuItem.Checked = false;
			}
			catch (Exception ex)
			{
				resourceManager.Renderer.RebuildRenderTargets(renderPanel.Width, renderPanel.Height);
				resourceManager.Renderer.RebuildShadowMaps(resourceManager.Renderer.ShadowSize);

				Console.WriteLine(ex);
			}

			timer.Start();
		}

		private void xToolStripMenuItem1_Click(object sender, EventArgs e)
		{
			timer.Stop();

			PresentParameters tempParams = (PresentParameters)resourceManager.PresentParams;

			if (tempParams.MultiSample == MultiSampleType.FourSamples)
				return;

			if (Manager.CheckDeviceMultiSampleType(0, DeviceType.Hardware, Format.X8R8G8B8, true, MultiSampleType.FourSamples))
				tempParams.MultiSample = MultiSampleType.FourSamples;
			else
				return;

			resourceManager.PresentParams = tempParams;

			try
			{
				resourceManager.Renderer.SceneRender.OnLostDevice();
				resourceManager.Renderer.ShadowRender.OnLostDevice();

				resourceManager.Renderer.SceneRender.Dispose();
				resourceManager.Renderer.ShadowRender.Dispose();

				resourceManager.Renderer.ReleaseRenderTextures();

				resourceManager.Device.Reset(tempParams);

				resourceManager.Renderer.RenderShadowMap(resourceManager.GetLight(0), 0, resourceManager.Camera.Projection);

				xToolStripMenuItem.Checked = false;
				xToolStripMenuItem1.Checked = true;
				xToolStripMenuItem2.Checked = false;
				noneToolStripMenuItem.Checked = false;
			}
			catch (Exception ex)
			{
				resourceManager.Renderer.RebuildRenderTargets(renderPanel.Width, renderPanel.Height);
				resourceManager.Renderer.RebuildShadowMaps(resourceManager.Renderer.ShadowSize);

				Console.WriteLine(ex);
			}

			timer.Start();
		}

		private void xToolStripMenuItem2_Click(object sender, EventArgs e)
		{
			timer.Stop();

			PresentParameters tempParams = (PresentParameters)resourceManager.PresentParams;

			if (tempParams.MultiSample == MultiSampleType.EightSamples)
				return;

			if (Manager.CheckDeviceMultiSampleType(0, DeviceType.Hardware, Format.X8R8G8B8, true, MultiSampleType.EightSamples))
				tempParams.MultiSample = MultiSampleType.EightSamples;
			else
				return;

			resourceManager.PresentParams = tempParams;

			try
			{
				resourceManager.Renderer.SceneRender.OnLostDevice();
				resourceManager.Renderer.ShadowRender.OnLostDevice();

				resourceManager.Renderer.SceneRender.Dispose();
				resourceManager.Renderer.ShadowRender.Dispose();

				resourceManager.Renderer.ReleaseRenderTextures();

				resourceManager.Device.Reset(tempParams);

				resourceManager.Renderer.RenderShadowMap(resourceManager.GetLight(0), 0, resourceManager.Camera.Projection);

				xToolStripMenuItem.Checked = false;
				xToolStripMenuItem1.Checked = false;
				xToolStripMenuItem2.Checked = true;
				noneToolStripMenuItem.Checked = false;
			}
			catch (Exception ex)
			{
				resourceManager.Renderer.RebuildRenderTargets(renderPanel.Width, renderPanel.Height);
				resourceManager.Renderer.RebuildShadowMaps(resourceManager.Renderer.ShadowSize);

				Console.WriteLine(ex);
			}

			timer.Start();
		}

		private void shadowStripMenuItem256_Click(object sender, EventArgs e)
		{
			if (resourceManager.Renderer.ShadowSize == 256)
				return;

			resourceManager.Renderer.ShadowSize = 256;

			if (resourceManager.GetLight(0).Shadows)
			   resourceManager.Renderer.RenderShadowMap(resourceManager.GetLight(0), 0, resourceManager.Camera.Projection);

			shadowStripMenuItem256.Checked = true;
			shadowStripMenuItem512.Checked = false;
			shadowStripMenuItem1024.Checked = false;
			shadowStripMenuItem2048.Checked = false;
		}

		private void shadowStripMenuItem512_Click(object sender, EventArgs e)
		{
			if (resourceManager.Renderer.ShadowSize == 512)
				return;

			resourceManager.Renderer.ShadowSize = 512;

			if (resourceManager.GetLight(0).Shadows)
			   resourceManager.Renderer.RenderShadowMap(resourceManager.GetLight(0), 0, resourceManager.Camera.Projection);

			shadowStripMenuItem256.Checked = false;
			shadowStripMenuItem512.Checked = true;
			shadowStripMenuItem1024.Checked = false;
			shadowStripMenuItem2048.Checked = false;
		}

		private void shadowStripMenuItem1024_Click(object sender, EventArgs e)
		{
			if (resourceManager.Renderer.ShadowSize == 1024)
				return;

			resourceManager.Renderer.ShadowSize = 1024;

			if (resourceManager.GetLight(0).Shadows)
			   resourceManager.Renderer.RenderShadowMap(resourceManager.GetLight(0), 0, resourceManager.Camera.Projection);

			shadowStripMenuItem256.Checked = false;
			shadowStripMenuItem512.Checked = false;
			shadowStripMenuItem1024.Checked = true;
			shadowStripMenuItem2048.Checked = false;
		}

		private void shadowStripMenuItem2048_Click(object sender, EventArgs e)
		{
			if (resourceManager.Renderer.ShadowSize == 2048)
				return;

			resourceManager.Renderer.ShadowSize = 2048;

			if (resourceManager.GetLight(0).Shadows)
			   resourceManager.Renderer.RenderShadowMap(resourceManager.GetLight(0), 0, resourceManager.Camera.Projection);

			shadowStripMenuItem256.Checked = false;
			shadowStripMenuItem512.Checked = false;
			shadowStripMenuItem1024.Checked = false;
			shadowStripMenuItem2048.Checked = true;
		}

		private void showLightPositionsToolStripMenuItem_Click(object sender, EventArgs e)
		{
			showLightPositionsToolStripMenuItem.Checked = !showLightPositionsToolStripMenuItem.Checked;

			resourceManager.ShowLights = showLightPositionsToolStripMenuItem.Checked;
		}

	}
}