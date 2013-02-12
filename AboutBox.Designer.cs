namespace ModelViewer
{
	partial class AboutBox
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
         System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(AboutBox));
         this.iconPicture = new System.Windows.Forms.PictureBox();
         this.closeButton = new System.Windows.Forms.Button();
         this.thanksBox = new System.Windows.Forms.GroupBox();
         this.thanksTextBox = new System.Windows.Forms.TextBox();
         this.nameLabel = new System.Windows.Forms.Label();
         this.versionLabel = new System.Windows.Forms.Label();
         this.authorLabel = new System.Windows.Forms.Label();
         this.authorNameLabel = new System.Windows.Forms.Label();
         this.websiteLinkLabel = new System.Windows.Forms.LinkLabel();
         ((System.ComponentModel.ISupportInitialize)(this.iconPicture)).BeginInit();
         this.thanksBox.SuspendLayout();
         this.SuspendLayout();
         // 
         // iconPicture
         // 
         this.iconPicture.BackColor = System.Drawing.Color.Transparent;
         this.iconPicture.Image = global::ModelViewer.Properties.Resources.You_can_t_sell_yourself_for_a_buck__junior_;
         this.iconPicture.Location = new System.Drawing.Point(12, 12);
         this.iconPicture.Name = "iconPicture";
         this.iconPicture.Size = new System.Drawing.Size(134, 127);
         this.iconPicture.TabIndex = 0;
         this.iconPicture.TabStop = false;
         // 
         // closeButton
         // 
         this.closeButton.Location = new System.Drawing.Point(12, 290);
         this.closeButton.Name = "closeButton";
         this.closeButton.Size = new System.Drawing.Size(336, 32);
         this.closeButton.TabIndex = 1;
         this.closeButton.Text = "&Close";
         this.closeButton.UseVisualStyleBackColor = true;
         this.closeButton.Click += new System.EventHandler(this.closeButton_Click);
         // 
         // thanksBox
         // 
         this.thanksBox.Controls.Add(this.thanksTextBox);
         this.thanksBox.Location = new System.Drawing.Point(12, 145);
         this.thanksBox.Name = "thanksBox";
         this.thanksBox.Size = new System.Drawing.Size(336, 139);
         this.thanksBox.TabIndex = 2;
         this.thanksBox.TabStop = false;
         this.thanksBox.Text = "Special Thanks To:";
         // 
         // thanksTextBox
         // 
         this.thanksTextBox.Location = new System.Drawing.Point(6, 25);
         this.thanksTextBox.Multiline = true;
         this.thanksTextBox.Name = "thanksTextBox";
         this.thanksTextBox.ReadOnly = true;
         this.thanksTextBox.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
         this.thanksTextBox.Size = new System.Drawing.Size(324, 114);
         this.thanksTextBox.TabIndex = 0;
         this.thanksTextBox.Text = resources.GetString("thanksTextBox.Text");
         // 
         // nameLabel
         // 
         this.nameLabel.AutoSize = true;
         this.nameLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
         this.nameLabel.Location = new System.Drawing.Point(152, 12);
         this.nameLabel.Name = "nameLabel";
         this.nameLabel.Size = new System.Drawing.Size(102, 20);
         this.nameLabel.TabIndex = 3;
         this.nameLabel.Text = "Polyviewer+";
         // 
         // versionLabel
         // 
         this.versionLabel.AutoSize = true;
         this.versionLabel.Location = new System.Drawing.Point(153, 32);
         this.versionLabel.Name = "versionLabel";
         this.versionLabel.Size = new System.Drawing.Size(66, 13);
         this.versionLabel.TabIndex = 4;
         this.versionLabel.Text = "Version 0.98";
         // 
         // authorLabel
         // 
         this.authorLabel.AutoSize = true;
         this.authorLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
         this.authorLabel.Location = new System.Drawing.Point(152, 61);
         this.authorLabel.Name = "authorLabel";
         this.authorLabel.Size = new System.Drawing.Size(56, 16);
         this.authorLabel.TabIndex = 5;
         this.authorLabel.Text = "Author:";
         // 
         // authorNameLabel
         // 
         this.authorNameLabel.AutoSize = true;
         this.authorNameLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
         this.authorNameLabel.Location = new System.Drawing.Point(154, 77);
         this.authorNameLabel.Name = "authorNameLabel";
         this.authorNameLabel.Size = new System.Drawing.Size(82, 16);
         this.authorNameLabel.TabIndex = 6;
         this.authorNameLabel.Text = "Craig Young";
         // 
         // websiteLinkLabel
         // 
         this.websiteLinkLabel.AutoSize = true;
         this.websiteLinkLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
         this.websiteLinkLabel.LinkBehavior = System.Windows.Forms.LinkBehavior.HoverUnderline;
         this.websiteLinkLabel.Location = new System.Drawing.Point(154, 123);
         this.websiteLinkLabel.Name = "websiteLinkLabel";
         this.websiteLinkLabel.Size = new System.Drawing.Size(65, 16);
         this.websiteLinkLabel.TabIndex = 7;
         this.websiteLinkLabel.TabStop = true;
         this.websiteLinkLabel.Text = "Website";
         this.websiteLinkLabel.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.websiteLinkLabel_LinkClicked);
         // 
         // AboutBox
         // 
         this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
         this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
         this.ClientSize = new System.Drawing.Size(360, 334);
         this.Controls.Add(this.websiteLinkLabel);
         this.Controls.Add(this.authorNameLabel);
         this.Controls.Add(this.authorLabel);
         this.Controls.Add(this.versionLabel);
         this.Controls.Add(this.nameLabel);
         this.Controls.Add(this.thanksBox);
         this.Controls.Add(this.closeButton);
         this.Controls.Add(this.iconPicture);
         this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
         this.MaximizeBox = false;
         this.MinimizeBox = false;
         this.Name = "AboutBox";
         this.ShowIcon = false;
         this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
         this.Text = "About Polyviewer+";
         ((System.ComponentModel.ISupportInitialize)(this.iconPicture)).EndInit();
         this.thanksBox.ResumeLayout(false);
         this.thanksBox.PerformLayout();
         this.ResumeLayout(false);
         this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.PictureBox iconPicture;
		private System.Windows.Forms.Button closeButton;
		private System.Windows.Forms.GroupBox thanksBox;
		private System.Windows.Forms.TextBox thanksTextBox;
		private System.Windows.Forms.Label nameLabel;
		private System.Windows.Forms.Label versionLabel;
		private System.Windows.Forms.Label authorLabel;
		private System.Windows.Forms.Label authorNameLabel;
		private System.Windows.Forms.LinkLabel websiteLinkLabel;
	}
}