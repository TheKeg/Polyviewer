namespace ModelViewer
{
	partial class GUI
	{
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		protected override void Dispose(bool disposing)
		{
			if (disposing && (components != null))
			{
				components.Dispose();
			}
			base.Dispose(disposing);
		}

		#region Windows Form Designer generated code

		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
         this.components = new System.ComponentModel.Container();
         System.Windows.Forms.TreeNode treeNode1 = new System.Windows.Forms.TreeNode("Ambient Occlusion Map");
         System.Windows.Forms.TreeNode treeNode2 = new System.Windows.Forms.TreeNode("Diffuse Map");
         System.Windows.Forms.TreeNode treeNode3 = new System.Windows.Forms.TreeNode("Emissive Texture");
         System.Windows.Forms.TreeNode treeNode4 = new System.Windows.Forms.TreeNode("Normal Map");
         System.Windows.Forms.TreeNode treeNode5 = new System.Windows.Forms.TreeNode("Specular Map");
         System.Windows.Forms.TreeNode treeNode6 = new System.Windows.Forms.TreeNode("Model", new System.Windows.Forms.TreeNode[] {
            treeNode1,
            treeNode2,
            treeNode3,
            treeNode4,
            treeNode5});
         System.Windows.Forms.TreeNode treeNode7 = new System.Windows.Forms.TreeNode("Camera");
         System.Windows.Forms.TreeNode treeNode8 = new System.Windows.Forms.TreeNode("Light #1");
         System.Windows.Forms.TreeNode treeNode9 = new System.Windows.Forms.TreeNode("Light #2");
         System.Windows.Forms.TreeNode treeNode10 = new System.Windows.Forms.TreeNode("Light #3");
         System.Windows.Forms.TreeNode treeNode11 = new System.Windows.Forms.TreeNode("Ground Plane");
         System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(GUI));
         this.toolbarPanel = new System.Windows.Forms.Panel();
         this.renderPanel = new System.Windows.Forms.Panel();
         this.menuStrip = new System.Windows.Forms.MenuStrip();
         this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
         this.loadModelToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
         this.openAdditionalModelToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
         this.clearModelsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
         this.toolStripSeparator4 = new System.Windows.Forms.ToolStripSeparator();
         this.loadSettingsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
         this.saveSettingsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
         this.toolStripSeparator5 = new System.Windows.Forms.ToolStripSeparator();
         this.takeScreenshotToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
         this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
         this.exitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
         this.editToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
         this.changeBackgroundToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
         this.showGroundPlaneToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
         this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
         this.resetCameraToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
         this.resetLightToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
         this.resetTexturesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
         this.toolStripSeparator3 = new System.Windows.Forms.ToolStripSeparator();
         this.enableTextureRefreshToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
         this.showLightPositionsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
         this.toolStripSeparator6 = new System.Windows.Forms.ToolStripSeparator();
         this.rebuildNormalsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
         this.invertCameraLightControlsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
         this.shadowMapSizeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
         this.shadowStripMenuItem256 = new System.Windows.Forms.ToolStripMenuItem();
         this.shadowStripMenuItem512 = new System.Windows.Forms.ToolStripMenuItem();
         this.shadowStripMenuItem1024 = new System.Windows.Forms.ToolStripMenuItem();
         this.shadowStripMenuItem2048 = new System.Windows.Forms.ToolStripMenuItem();
         this.lightToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
         this.light1ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
         this.light2ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
         this.light3ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
         this.antiAliasingToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
         this.noneToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
         this.xToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
         this.xToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
         this.xToolStripMenuItem2 = new System.Windows.Forms.ToolStripMenuItem();
         this.aboutToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
         this.aboutModelViewerToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
         this.userControl1 = new System.Windows.Forms.UserControl();
         this.viewerProperyGrid = new System.Windows.Forms.PropertyGrid();
         this.objectTreeView = new System.Windows.Forms.TreeView();
         this.optionPanel = new System.Windows.Forms.Panel();
         this.treeViewContextMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
         this.loadModelToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
         this.loadAdditionalModelToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
         this.lightSelectedStrip = new System.Windows.Forms.StatusStrip();
         this.selectedLightLabel = new System.Windows.Forms.ToolStripStatusLabel();
         this.menuStrip.SuspendLayout();
         this.optionPanel.SuspendLayout();
         this.treeViewContextMenu.SuspendLayout();
         this.lightSelectedStrip.SuspendLayout();
         this.SuspendLayout();
         // 
         // toolbarPanel
         // 
         this.toolbarPanel.AutoSize = true;
         this.toolbarPanel.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
         this.toolbarPanel.Dock = System.Windows.Forms.DockStyle.Left;
         this.toolbarPanel.Location = new System.Drawing.Point(0, 24);
         this.toolbarPanel.Name = "toolbarPanel";
         this.toolbarPanel.Size = new System.Drawing.Size(0, 761);
         this.toolbarPanel.TabIndex = 1;
         // 
         // renderPanel
         // 
         this.renderPanel.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                     | System.Windows.Forms.AnchorStyles.Left)
                     | System.Windows.Forms.AnchorStyles.Right)));
         this.renderPanel.AutoSize = true;
         this.renderPanel.BackColor = System.Drawing.SystemColors.AppWorkspace;
         this.renderPanel.Location = new System.Drawing.Point(215, 27);
         this.renderPanel.Name = "renderPanel";
         this.renderPanel.Size = new System.Drawing.Size(754, 754);
         this.renderPanel.TabIndex = 2;
         this.renderPanel.MouseDown += new System.Windows.Forms.MouseEventHandler(this.renderPanel_MouseDown);
         this.renderPanel.MouseMove += new System.Windows.Forms.MouseEventHandler(this.renderPanel_MouseMove);
         this.renderPanel.Resize += new System.EventHandler(this.GUI_Resize);
         this.renderPanel.MouseUp += new System.Windows.Forms.MouseEventHandler(this.renderPanel_MouseUp);
         // 
         // menuStrip
         // 
         this.menuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem,
            this.editToolStripMenuItem,
            this.lightToolStripMenuItem,
            this.antiAliasingToolStripMenuItem,
            this.aboutToolStripMenuItem});
         this.menuStrip.Location = new System.Drawing.Point(0, 0);
         this.menuStrip.Name = "menuStrip";
         this.menuStrip.RenderMode = System.Windows.Forms.ToolStripRenderMode.System;
         this.menuStrip.Size = new System.Drawing.Size(973, 24);
         this.menuStrip.TabIndex = 0;
         this.menuStrip.Text = "menuStrip";
         // 
         // fileToolStripMenuItem
         // 
         this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.loadModelToolStripMenuItem,
            this.openAdditionalModelToolStripMenuItem,
            this.clearModelsToolStripMenuItem,
            this.toolStripSeparator4,
            this.loadSettingsToolStripMenuItem,
            this.saveSettingsToolStripMenuItem,
            this.toolStripSeparator5,
            this.takeScreenshotToolStripMenuItem,
            this.toolStripSeparator2,
            this.exitToolStripMenuItem});
         this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
         this.fileToolStripMenuItem.Size = new System.Drawing.Size(35, 20);
         this.fileToolStripMenuItem.Text = "&File";
         // 
         // loadModelToolStripMenuItem
         // 
         this.loadModelToolStripMenuItem.Name = "loadModelToolStripMenuItem";
         this.loadModelToolStripMenuItem.ShortcutKeyDisplayString = "";
         this.loadModelToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.O)));
         this.loadModelToolStripMenuItem.Size = new System.Drawing.Size(181, 22);
         this.loadModelToolStripMenuItem.Text = "&Open Model";
         this.loadModelToolStripMenuItem.Click += new System.EventHandler(this.loadModelToolStripMenuItem_Click);
         // 
         // openAdditionalModelToolStripMenuItem
         // 
         this.openAdditionalModelToolStripMenuItem.Enabled = false;
         this.openAdditionalModelToolStripMenuItem.Name = "openAdditionalModelToolStripMenuItem";
         this.openAdditionalModelToolStripMenuItem.Size = new System.Drawing.Size(181, 22);
         this.openAdditionalModelToolStripMenuItem.Text = "Open Additional Model";
         this.openAdditionalModelToolStripMenuItem.Click += new System.EventHandler(this.openAdditionalModelToolStripMenuItem_Click);
         // 
         // clearModelsToolStripMenuItem
         // 
         this.clearModelsToolStripMenuItem.Name = "clearModelsToolStripMenuItem";
         this.clearModelsToolStripMenuItem.Size = new System.Drawing.Size(181, 22);
         this.clearModelsToolStripMenuItem.Text = "Clear Models";
         this.clearModelsToolStripMenuItem.Click += new System.EventHandler(this.clearModelsToolStripMenuItem_Click);
         // 
         // toolStripSeparator4
         // 
         this.toolStripSeparator4.Name = "toolStripSeparator4";
         this.toolStripSeparator4.Size = new System.Drawing.Size(178, 6);
         // 
         // loadSettingsToolStripMenuItem
         // 
         this.loadSettingsToolStripMenuItem.Name = "loadSettingsToolStripMenuItem";
         this.loadSettingsToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.L)));
         this.loadSettingsToolStripMenuItem.Size = new System.Drawing.Size(181, 22);
         this.loadSettingsToolStripMenuItem.Text = "Load Settings";
         this.loadSettingsToolStripMenuItem.Click += new System.EventHandler(this.loadSettingsToolStripMenuItem_Click);
         // 
         // saveSettingsToolStripMenuItem
         // 
         this.saveSettingsToolStripMenuItem.Name = "saveSettingsToolStripMenuItem";
         this.saveSettingsToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.S)));
         this.saveSettingsToolStripMenuItem.Size = new System.Drawing.Size(181, 22);
         this.saveSettingsToolStripMenuItem.Text = "Save Settings";
         this.saveSettingsToolStripMenuItem.Click += new System.EventHandler(this.saveSettingsToolStripMenuItem_Click);
         // 
         // toolStripSeparator5
         // 
         this.toolStripSeparator5.Name = "toolStripSeparator5";
         this.toolStripSeparator5.Size = new System.Drawing.Size(178, 6);
         // 
         // takeScreenshotToolStripMenuItem
         // 
         this.takeScreenshotToolStripMenuItem.Name = "takeScreenshotToolStripMenuItem";
         this.takeScreenshotToolStripMenuItem.Size = new System.Drawing.Size(181, 22);
         this.takeScreenshotToolStripMenuItem.Text = "Take Screenshot";
         this.takeScreenshotToolStripMenuItem.Click += new System.EventHandler(this.takeScreenshotToolStripMenuItem_Click);
         // 
         // toolStripSeparator2
         // 
         this.toolStripSeparator2.Name = "toolStripSeparator2";
         this.toolStripSeparator2.Size = new System.Drawing.Size(178, 6);
         // 
         // exitToolStripMenuItem
         // 
         this.exitToolStripMenuItem.Name = "exitToolStripMenuItem";
         this.exitToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Alt | System.Windows.Forms.Keys.F4)));
         this.exitToolStripMenuItem.Size = new System.Drawing.Size(181, 22);
         this.exitToolStripMenuItem.Text = "E&xit";
         this.exitToolStripMenuItem.Click += new System.EventHandler(this.exitToolStripMenuItem_Click);
         // 
         // editToolStripMenuItem
         // 
         this.editToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.changeBackgroundToolStripMenuItem,
            this.showGroundPlaneToolStripMenuItem,
            this.toolStripSeparator1,
            this.resetCameraToolStripMenuItem,
            this.resetLightToolStripMenuItem,
            this.resetTexturesToolStripMenuItem,
            this.toolStripSeparator3,
            this.enableTextureRefreshToolStripMenuItem,
            this.showLightPositionsToolStripMenuItem,
            this.toolStripSeparator6,
            this.rebuildNormalsToolStripMenuItem,
            this.invertCameraLightControlsToolStripMenuItem,
            this.shadowMapSizeToolStripMenuItem});
         this.editToolStripMenuItem.Name = "editToolStripMenuItem";
         this.editToolStripMenuItem.Size = new System.Drawing.Size(37, 20);
         this.editToolStripMenuItem.Text = "&Edit";
         // 
         // changeBackgroundToolStripMenuItem
         // 
         this.changeBackgroundToolStripMenuItem.Name = "changeBackgroundToolStripMenuItem";
         this.changeBackgroundToolStripMenuItem.ShortcutKeyDisplayString = "";
         this.changeBackgroundToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.B)));
         this.changeBackgroundToolStripMenuItem.Size = new System.Drawing.Size(208, 22);
         this.changeBackgroundToolStripMenuItem.Text = "&Change Background";
         this.changeBackgroundToolStripMenuItem.Click += new System.EventHandler(this.changeBackgroundToolStripMenuItem_Click);
         // 
         // showGroundPlaneToolStripMenuItem
         // 
         this.showGroundPlaneToolStripMenuItem.Name = "showGroundPlaneToolStripMenuItem";
         this.showGroundPlaneToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.G)));
         this.showGroundPlaneToolStripMenuItem.Size = new System.Drawing.Size(208, 22);
         this.showGroundPlaneToolStripMenuItem.Text = "Show Ground Plane";
         this.showGroundPlaneToolStripMenuItem.Click += new System.EventHandler(this.showGroundPlaneToolStripMenuItem_Click);
         // 
         // toolStripSeparator1
         // 
         this.toolStripSeparator1.Name = "toolStripSeparator1";
         this.toolStripSeparator1.Size = new System.Drawing.Size(205, 6);
         // 
         // resetCameraToolStripMenuItem
         // 
         this.resetCameraToolStripMenuItem.Name = "resetCameraToolStripMenuItem";
         this.resetCameraToolStripMenuItem.ShortcutKeyDisplayString = "";
         this.resetCameraToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.C)));
         this.resetCameraToolStripMenuItem.Size = new System.Drawing.Size(208, 22);
         this.resetCameraToolStripMenuItem.Text = "&Reset Camera";
         this.resetCameraToolStripMenuItem.Click += new System.EventHandler(this.resetCameraToolStripMenuItem_Click);
         // 
         // resetLightToolStripMenuItem
         // 
         this.resetLightToolStripMenuItem.Name = "resetLightToolStripMenuItem";
         this.resetLightToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.V)));
         this.resetLightToolStripMenuItem.Size = new System.Drawing.Size(208, 22);
         this.resetLightToolStripMenuItem.Text = "Reset &Light";
         this.resetLightToolStripMenuItem.Click += new System.EventHandler(this.resetLightToolStripMenuItem_Click);
         // 
         // resetTexturesToolStripMenuItem
         // 
         this.resetTexturesToolStripMenuItem.Name = "resetTexturesToolStripMenuItem";
         this.resetTexturesToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.B)));
         this.resetTexturesToolStripMenuItem.Size = new System.Drawing.Size(208, 22);
         this.resetTexturesToolStripMenuItem.Text = "Reset &Textures";
         this.resetTexturesToolStripMenuItem.Click += new System.EventHandler(this.resetTexturesToolStripMenuItem_Click);
         // 
         // toolStripSeparator3
         // 
         this.toolStripSeparator3.Name = "toolStripSeparator3";
         this.toolStripSeparator3.Size = new System.Drawing.Size(205, 6);
         // 
         // enableTextureRefreshToolStripMenuItem
         // 
         this.enableTextureRefreshToolStripMenuItem.Checked = true;
         this.enableTextureRefreshToolStripMenuItem.CheckState = System.Windows.Forms.CheckState.Checked;
         this.enableTextureRefreshToolStripMenuItem.Name = "enableTextureRefreshToolStripMenuItem";
         this.enableTextureRefreshToolStripMenuItem.Size = new System.Drawing.Size(208, 22);
         this.enableTextureRefreshToolStripMenuItem.Text = "Enable Texture Refresh";
         this.enableTextureRefreshToolStripMenuItem.Click += new System.EventHandler(this.enableTextureRefreshToolStripMenuItem_Click);
         // 
         // showLightPositionsToolStripMenuItem
         // 
         this.showLightPositionsToolStripMenuItem.Name = "showLightPositionsToolStripMenuItem";
         this.showLightPositionsToolStripMenuItem.Size = new System.Drawing.Size(208, 22);
         this.showLightPositionsToolStripMenuItem.Text = "Show Light Positions";
         this.showLightPositionsToolStripMenuItem.Click += new System.EventHandler(this.showLightPositionsToolStripMenuItem_Click);
         // 
         // toolStripSeparator6
         // 
         this.toolStripSeparator6.Name = "toolStripSeparator6";
         this.toolStripSeparator6.Size = new System.Drawing.Size(205, 6);
         // 
         // rebuildNormalsToolStripMenuItem
         // 
         this.rebuildNormalsToolStripMenuItem.Name = "rebuildNormalsToolStripMenuItem";
         this.rebuildNormalsToolStripMenuItem.Size = new System.Drawing.Size(208, 22);
         this.rebuildNormalsToolStripMenuItem.Text = "Rebuild Normals";
         this.rebuildNormalsToolStripMenuItem.Click += new System.EventHandler(this.rebuildNormalsToolStripMenuItem_Click);
         // 
         // invertCameraLightControlsToolStripMenuItem
         // 
         this.invertCameraLightControlsToolStripMenuItem.Name = "invertCameraLightControlsToolStripMenuItem";
         this.invertCameraLightControlsToolStripMenuItem.Size = new System.Drawing.Size(208, 22);
         this.invertCameraLightControlsToolStripMenuItem.Text = "Invert Controls";
         this.invertCameraLightControlsToolStripMenuItem.Click += new System.EventHandler(this.invertCameraLightControlsToolStripMenuItem_Click);
         // 
         // shadowMapSizeToolStripMenuItem
         // 
         this.shadowMapSizeToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.shadowStripMenuItem256,
            this.shadowStripMenuItem512,
            this.shadowStripMenuItem1024,
            this.shadowStripMenuItem2048});
         this.shadowMapSizeToolStripMenuItem.Name = "shadowMapSizeToolStripMenuItem";
         this.shadowMapSizeToolStripMenuItem.Size = new System.Drawing.Size(208, 22);
         this.shadowMapSizeToolStripMenuItem.Text = "Shadow Map Size";
         // 
         // shadowStripMenuItem256
         // 
         this.shadowStripMenuItem256.Name = "shadowStripMenuItem256";
         this.shadowStripMenuItem256.Size = new System.Drawing.Size(98, 22);
         this.shadowStripMenuItem256.Text = "256";
         this.shadowStripMenuItem256.Click += new System.EventHandler(this.shadowStripMenuItem256_Click);
         // 
         // shadowStripMenuItem512
         // 
         this.shadowStripMenuItem512.Checked = true;
         this.shadowStripMenuItem512.CheckState = System.Windows.Forms.CheckState.Checked;
         this.shadowStripMenuItem512.Name = "shadowStripMenuItem512";
         this.shadowStripMenuItem512.Size = new System.Drawing.Size(98, 22);
         this.shadowStripMenuItem512.Text = "512";
         this.shadowStripMenuItem512.Click += new System.EventHandler(this.shadowStripMenuItem512_Click);
         // 
         // shadowStripMenuItem1024
         // 
         this.shadowStripMenuItem1024.Name = "shadowStripMenuItem1024";
         this.shadowStripMenuItem1024.Size = new System.Drawing.Size(98, 22);
         this.shadowStripMenuItem1024.Text = "1024";
         this.shadowStripMenuItem1024.Click += new System.EventHandler(this.shadowStripMenuItem1024_Click);
         // 
         // shadowStripMenuItem2048
         // 
         this.shadowStripMenuItem2048.Name = "shadowStripMenuItem2048";
         this.shadowStripMenuItem2048.Size = new System.Drawing.Size(98, 22);
         this.shadowStripMenuItem2048.Text = "2048";
         this.shadowStripMenuItem2048.Click += new System.EventHandler(this.shadowStripMenuItem2048_Click);
         // 
         // lightToolStripMenuItem
         // 
         this.lightToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.light1ToolStripMenuItem,
            this.light2ToolStripMenuItem,
            this.light3ToolStripMenuItem});
         this.lightToolStripMenuItem.Name = "lightToolStripMenuItem";
         this.lightToolStripMenuItem.Size = new System.Drawing.Size(88, 20);
         this.lightToolStripMenuItem.Text = "Light Selection";
         // 
         // light1ToolStripMenuItem
         // 
         this.light1ToolStripMenuItem.Checked = true;
         this.light1ToolStripMenuItem.CheckState = System.Windows.Forms.CheckState.Checked;
         this.light1ToolStripMenuItem.Name = "light1ToolStripMenuItem";
         this.light1ToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.D1)));
         this.light1ToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
         this.light1ToolStripMenuItem.Text = "Light #1";
         this.light1ToolStripMenuItem.Click += new System.EventHandler(this.light1ToolStripMenuItem_Click);
         // 
         // light2ToolStripMenuItem
         // 
         this.light2ToolStripMenuItem.Name = "light2ToolStripMenuItem";
         this.light2ToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.D2)));
         this.light2ToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
         this.light2ToolStripMenuItem.Text = "Light #2";
         this.light2ToolStripMenuItem.Click += new System.EventHandler(this.light2ToolStripMenuItem_Click);
         // 
         // light3ToolStripMenuItem
         // 
         this.light3ToolStripMenuItem.Name = "light3ToolStripMenuItem";
         this.light3ToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.D3)));
         this.light3ToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
         this.light3ToolStripMenuItem.Text = "Light #3";
         this.light3ToolStripMenuItem.Click += new System.EventHandler(this.light3ToolStripMenuItem_Click);
         // 
         // antiAliasingToolStripMenuItem
         // 
         this.antiAliasingToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.noneToolStripMenuItem,
            this.xToolStripMenuItem,
            this.xToolStripMenuItem1,
            this.xToolStripMenuItem2});
         this.antiAliasingToolStripMenuItem.Name = "antiAliasingToolStripMenuItem";
         this.antiAliasingToolStripMenuItem.Size = new System.Drawing.Size(77, 20);
         this.antiAliasingToolStripMenuItem.Text = "Anti Aliasing";
         // 
         // noneToolStripMenuItem
         // 
         this.noneToolStripMenuItem.Checked = true;
         this.noneToolStripMenuItem.CheckState = System.Windows.Forms.CheckState.Checked;
         this.noneToolStripMenuItem.Name = "noneToolStripMenuItem";
         this.noneToolStripMenuItem.Size = new System.Drawing.Size(99, 22);
         this.noneToolStripMenuItem.Text = "None";
         this.noneToolStripMenuItem.Click += new System.EventHandler(this.noneToolStripMenuItem_Click);
         // 
         // xToolStripMenuItem
         // 
         this.xToolStripMenuItem.Name = "xToolStripMenuItem";
         this.xToolStripMenuItem.Size = new System.Drawing.Size(99, 22);
         this.xToolStripMenuItem.Text = "2x";
         this.xToolStripMenuItem.Click += new System.EventHandler(this.xToolStripMenuItem_Click);
         // 
         // xToolStripMenuItem1
         // 
         this.xToolStripMenuItem1.Name = "xToolStripMenuItem1";
         this.xToolStripMenuItem1.Size = new System.Drawing.Size(99, 22);
         this.xToolStripMenuItem1.Text = "4x";
         this.xToolStripMenuItem1.Click += new System.EventHandler(this.xToolStripMenuItem1_Click);
         // 
         // xToolStripMenuItem2
         // 
         this.xToolStripMenuItem2.Name = "xToolStripMenuItem2";
         this.xToolStripMenuItem2.Size = new System.Drawing.Size(99, 22);
         this.xToolStripMenuItem2.Text = "8x";
         this.xToolStripMenuItem2.Click += new System.EventHandler(this.xToolStripMenuItem2_Click);
         // 
         // aboutToolStripMenuItem
         // 
         this.aboutToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.aboutModelViewerToolStripMenuItem});
         this.aboutToolStripMenuItem.Name = "aboutToolStripMenuItem";
         this.aboutToolStripMenuItem.Size = new System.Drawing.Size(48, 20);
         this.aboutToolStripMenuItem.Text = "&About";
         // 
         // aboutModelViewerToolStripMenuItem
         // 
         this.aboutModelViewerToolStripMenuItem.Name = "aboutModelViewerToolStripMenuItem";
         this.aboutModelViewerToolStripMenuItem.ShortcutKeys = System.Windows.Forms.Keys.F1;
         this.aboutModelViewerToolStripMenuItem.Size = new System.Drawing.Size(185, 22);
         this.aboutModelViewerToolStripMenuItem.Text = "About Polyviewer+";
         this.aboutModelViewerToolStripMenuItem.Click += new System.EventHandler(this.aboutModelViewerToolStripMenuItem_Click);
         // 
         // userControl1
         // 
         this.userControl1.Location = new System.Drawing.Point(24, 772);
         this.userControl1.Name = "userControl1";
         this.userControl1.Size = new System.Drawing.Size(150, 150);
         this.userControl1.TabIndex = 3;
         // 
         // viewerProperyGrid
         // 
         this.viewerProperyGrid.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
         this.viewerProperyGrid.ImeMode = System.Windows.Forms.ImeMode.On;
         this.viewerProperyGrid.Location = new System.Drawing.Point(3, 469);
         this.viewerProperyGrid.Name = "viewerProperyGrid";
         this.viewerProperyGrid.Size = new System.Drawing.Size(206, 292);
         this.viewerProperyGrid.TabIndex = 4;
         this.viewerProperyGrid.PropertyValueChanged += new System.Windows.Forms.PropertyValueChangedEventHandler(this.viewerProperyGrid_PropertyValueChanged);
         // 
         // objectTreeView
         // 
         this.objectTreeView.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                     | System.Windows.Forms.AnchorStyles.Left)
                     | System.Windows.Forms.AnchorStyles.Right)));
         this.objectTreeView.CheckBoxes = true;
         this.objectTreeView.FullRowSelect = true;
         this.objectTreeView.HotTracking = true;
         this.objectTreeView.Location = new System.Drawing.Point(3, 3);
         this.objectTreeView.Name = "objectTreeView";
         treeNode1.Name = "ambientOccChildNode";
         treeNode1.Text = "Ambient Occlusion Map";
         treeNode2.Name = "diffuseChildNode";
         treeNode2.Text = "Diffuse Map";
         treeNode3.Name = "emissiveChildNode";
         treeNode3.Text = "Emissive Texture";
         treeNode4.Name = "normalChildNode";
         treeNode4.Text = "Normal Map";
         treeNode5.Name = "specularChildNode";
         treeNode5.Text = "Specular Map";
         treeNode6.Name = "modelNode";
         treeNode6.Text = "Model";
         treeNode7.Checked = true;
         treeNode7.Name = "cameraNode";
         treeNode7.Text = "Camera";
         treeNode8.Checked = true;
         treeNode8.Name = "lightOneNode";
         treeNode8.Text = "Light #1";
         treeNode9.Name = "lightTwoNode";
         treeNode9.Text = "Light #2";
         treeNode10.Name = "lightThreeNode";
         treeNode10.Text = "Light #3";
         treeNode11.Name = "groundPlane";
         treeNode11.Text = "Ground Plane";
         this.objectTreeView.Nodes.AddRange(new System.Windows.Forms.TreeNode[] {
            treeNode6,
            treeNode7,
            treeNode8,
            treeNode9,
            treeNode10,
            treeNode11});
         this.objectTreeView.RightToLeft = System.Windows.Forms.RightToLeft.No;
         this.objectTreeView.Size = new System.Drawing.Size(206, 460);
         this.objectTreeView.TabIndex = 5;
         this.objectTreeView.AfterCheck += new System.Windows.Forms.TreeViewEventHandler(this.objectTreeView_AfterCheck);
         this.objectTreeView.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.objectTreeView_AfterSelect);
         // 
         // optionPanel
         // 
         this.optionPanel.AutoSize = true;
         this.optionPanel.Controls.Add(this.viewerProperyGrid);
         this.optionPanel.Controls.Add(this.objectTreeView);
         this.optionPanel.Dock = System.Windows.Forms.DockStyle.Left;
         this.optionPanel.Location = new System.Drawing.Point(0, 24);
         this.optionPanel.Name = "optionPanel";
         this.optionPanel.Size = new System.Drawing.Size(218, 761);
         this.optionPanel.TabIndex = 4;
         // 
         // treeViewContextMenu
         // 
         this.treeViewContextMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.loadModelToolStripMenuItem1,
            this.loadAdditionalModelToolStripMenuItem});
         this.treeViewContextMenu.Name = "treeViewContextMenu";
         this.treeViewContextMenu.ShowImageMargin = false;
         this.treeViewContextMenu.Size = new System.Drawing.Size(154, 48);
         // 
         // loadModelToolStripMenuItem1
         // 
         this.loadModelToolStripMenuItem1.Name = "loadModelToolStripMenuItem1";
         this.loadModelToolStripMenuItem1.Size = new System.Drawing.Size(153, 22);
         this.loadModelToolStripMenuItem1.Text = "Load Model";
         this.loadModelToolStripMenuItem1.Click += new System.EventHandler(this.loadModelToolStripMenuItem1_Click);
         // 
         // loadAdditionalModelToolStripMenuItem
         // 
         this.loadAdditionalModelToolStripMenuItem.Enabled = false;
         this.loadAdditionalModelToolStripMenuItem.Name = "loadAdditionalModelToolStripMenuItem";
         this.loadAdditionalModelToolStripMenuItem.Size = new System.Drawing.Size(153, 22);
         this.loadAdditionalModelToolStripMenuItem.Text = "Load Additional Model";
         this.loadAdditionalModelToolStripMenuItem.Click += new System.EventHandler(this.loadAdditionalModelToolStripMenuItem_Click);
         // 
         // lightSelectedStrip
         // 
         this.lightSelectedStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.selectedLightLabel});
         this.lightSelectedStrip.LayoutStyle = System.Windows.Forms.ToolStripLayoutStyle.HorizontalStackWithOverflow;
         this.lightSelectedStrip.Location = new System.Drawing.Point(218, 763);
         this.lightSelectedStrip.Name = "lightSelectedStrip";
         this.lightSelectedStrip.Size = new System.Drawing.Size(755, 22);
         this.lightSelectedStrip.TabIndex = 5;
         this.lightSelectedStrip.Text = "Light #1 Selected";
         // 
         // selectedLightLabel
         // 
         this.selectedLightLabel.Name = "selectedLightLabel";
         this.selectedLightLabel.Size = new System.Drawing.Size(91, 17);
         this.selectedLightLabel.Text = "Light #1 Selected";
         // 
         // GUI
         // 
         this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
         this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
         this.ClientSize = new System.Drawing.Size(973, 785);
         this.Controls.Add(this.lightSelectedStrip);
         this.Controls.Add(this.optionPanel);
         this.Controls.Add(this.userControl1);
         this.Controls.Add(this.renderPanel);
         this.Controls.Add(this.toolbarPanel);
         this.Controls.Add(this.menuStrip);
         this.DoubleBuffered = true;
         this.HelpButton = true;
         this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
         this.MainMenuStrip = this.menuStrip;
         this.Name = "GUI";
         this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
         this.Text = "Polyviewer+";
         this.Deactivate += new System.EventHandler(this.GUI_Deactivate);
         this.Activated += new System.EventHandler(this.GUI_Activated);
         this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.GUI_FormClosing);
         this.menuStrip.ResumeLayout(false);
         this.menuStrip.PerformLayout();
         this.optionPanel.ResumeLayout(false);
         this.treeViewContextMenu.ResumeLayout(false);
         this.lightSelectedStrip.ResumeLayout(false);
         this.lightSelectedStrip.PerformLayout();
         this.ResumeLayout(false);
         this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.Panel toolbarPanel;
		private System.Windows.Forms.Panel renderPanel;
		private System.Windows.Forms.MenuStrip menuStrip;
		private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem aboutToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem loadModelToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem exitToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem editToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem changeBackgroundToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem resetCameraToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem resetLightToolStripMenuItem;
		private System.Windows.Forms.UserControl userControl1;
		private System.Windows.Forms.PropertyGrid viewerProperyGrid;
		private System.Windows.Forms.TreeView objectTreeView;
		private System.Windows.Forms.ToolStripMenuItem resetTexturesToolStripMenuItem;
		private System.Windows.Forms.Panel optionPanel;
		private System.Windows.Forms.ToolStripMenuItem loadSettingsToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem aboutModelViewerToolStripMenuItem;
		private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
		private System.Windows.Forms.ToolStripMenuItem lightToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem light1ToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem light2ToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem light3ToolStripMenuItem;
		private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
		private System.Windows.Forms.ToolStripMenuItem saveSettingsToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem showGroundPlaneToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem enableTextureRefreshToolStripMenuItem;
		private System.Windows.Forms.ToolStripSeparator toolStripSeparator3;
		private System.Windows.Forms.ToolStripMenuItem invertCameraLightControlsToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem takeScreenshotToolStripMenuItem;
		private System.Windows.Forms.ToolStripSeparator toolStripSeparator4;
		private System.Windows.Forms.ToolStripSeparator toolStripSeparator5;
		private System.Windows.Forms.ContextMenuStrip treeViewContextMenu;
		private System.Windows.Forms.ToolStripMenuItem loadModelToolStripMenuItem1;
		private System.Windows.Forms.ToolStripMenuItem loadAdditionalModelToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem openAdditionalModelToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem clearModelsToolStripMenuItem;
		private System.Windows.Forms.StatusStrip lightSelectedStrip;
		private System.Windows.Forms.ToolStripStatusLabel selectedLightLabel;
		private System.Windows.Forms.ToolStripMenuItem rebuildNormalsToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem antiAliasingToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem noneToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem xToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem xToolStripMenuItem1;
		private System.Windows.Forms.ToolStripMenuItem xToolStripMenuItem2;
		private System.Windows.Forms.ToolStripMenuItem shadowMapSizeToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem shadowStripMenuItem256;
		private System.Windows.Forms.ToolStripMenuItem shadowStripMenuItem512;
		private System.Windows.Forms.ToolStripMenuItem shadowStripMenuItem1024;
		private System.Windows.Forms.ToolStripMenuItem shadowStripMenuItem2048;
		private System.Windows.Forms.ToolStripMenuItem showLightPositionsToolStripMenuItem;
		private System.Windows.Forms.ToolStripSeparator toolStripSeparator6;
	}
}

