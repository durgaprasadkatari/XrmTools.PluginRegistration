
namespace PluginRegistrationUsingXml
{
    partial class PluginRegistrationControl
    {
        /// <summary> 
        /// Variable nécessaire au concepteur.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Nettoyage des ressources utilisées.
        /// </summary>
        /// <param name="disposing">true si les ressources managées doivent être supprimées ; sinon, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Code généré par le Concepteur de composants

        /// <summary> 
        /// Méthode requise pour la prise en charge du concepteur - ne modifiez pas 
        /// le contenu de cette méthode avec l'éditeur de code.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(PluginRegistrationControl));
            this.toolStripMenu = new System.Windows.Forms.ToolStrip();
            this.tsbGenerateXmlFile = new System.Windows.Forms.ToolStripButton();
            this.tsbLoadPlugins = new System.Windows.Forms.ToolStripButton();
            this.treeView1 = new System.Windows.Forms.TreeView();
            this.txtRegistrationFile = new System.Windows.Forms.TextBox();
            this.txtBrowsePluginsdll = new System.Windows.Forms.TextBox();
            this.btnRegisterPlugins = new System.Windows.Forms.Button();
            this.btnbrowseRegistrationFile = new System.Windows.Forms.Button();
            this.btnBrowsePluginsDLL = new System.Windows.Forms.Button();
            this.toolStripMenu.SuspendLayout();
            this.SuspendLayout();
            // 
            // toolStripMenu
            // 
            this.toolStripMenu.ImageScalingSize = new System.Drawing.Size(24, 24);
            this.toolStripMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsbGenerateXmlFile,
            this.tsbLoadPlugins});
            this.toolStripMenu.Location = new System.Drawing.Point(0, 0);
            this.toolStripMenu.Name = "toolStripMenu";
            this.toolStripMenu.Size = new System.Drawing.Size(1400, 34);
            this.toolStripMenu.TabIndex = 4;
            this.toolStripMenu.Text = "toolStrip1";
            // 
            // tsbGenerateXmlFile
            // 
            this.tsbGenerateXmlFile.Image = ((System.Drawing.Image)(resources.GetObject("tsbGenerateXmlFile.Image")));
            this.tsbGenerateXmlFile.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsbGenerateXmlFile.Name = "tsbGenerateXmlFile";
            this.tsbGenerateXmlFile.Size = new System.Drawing.Size(240, 29);
            this.tsbGenerateXmlFile.Text = "Generate Registration File";
            this.tsbGenerateXmlFile.ToolTipText = "Generate Registration Xml File";
            this.tsbGenerateXmlFile.Click += new System.EventHandler(this.tsbGenerateXmlFile_Click);
            // 
            // tsbLoadPlugins
            // 
            this.tsbLoadPlugins.Image = ((System.Drawing.Image)(resources.GetObject("tsbLoadPlugins.Image")));
            this.tsbLoadPlugins.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsbLoadPlugins.Name = "tsbLoadPlugins";
            this.tsbLoadPlugins.Size = new System.Drawing.Size(232, 29);
            this.tsbLoadPlugins.Text = "Load Plugins/Workflows";
            this.tsbLoadPlugins.ToolTipText = "Load Plugins/Workflows";
            this.tsbLoadPlugins.Click += new System.EventHandler(this.tsbLoadPlugins_Click);
            // 
            // treeView1
            // 
            this.treeView1.Location = new System.Drawing.Point(27, 161);
            this.treeView1.Name = "treeView1";
            this.treeView1.Size = new System.Drawing.Size(1282, 525);
            this.treeView1.TabIndex = 5;
            // 
            // txtRegistrationFile
            // 
            this.txtRegistrationFile.Location = new System.Drawing.Point(27, 49);
            this.txtRegistrationFile.Name = "txtRegistrationFile";
            this.txtRegistrationFile.Size = new System.Drawing.Size(562, 26);
            this.txtRegistrationFile.TabIndex = 8;
            this.txtRegistrationFile.Text = "Browse Registration File";
            // 
            // txtBrowsePluginsdll
            // 
            this.txtBrowsePluginsdll.Location = new System.Drawing.Point(27, 97);
            this.txtBrowsePluginsdll.Name = "txtBrowsePluginsdll";
            this.txtBrowsePluginsdll.Size = new System.Drawing.Size(562, 26);
            this.txtBrowsePluginsdll.TabIndex = 9;
            this.txtBrowsePluginsdll.Text = "Browser Plugin Assembly DLL";
            // 
            // btnRegisterPlugins
            // 
            this.btnRegisterPlugins.Location = new System.Drawing.Point(691, 45);
            this.btnRegisterPlugins.Name = "btnRegisterPlugins";
            this.btnRegisterPlugins.Size = new System.Drawing.Size(205, 42);
            this.btnRegisterPlugins.TabIndex = 10;
            this.btnRegisterPlugins.Text = "Register Plugins";
            this.btnRegisterPlugins.UseVisualStyleBackColor = true;
            this.btnRegisterPlugins.Click += new System.EventHandler(this.btnRegisterPlugins_Click);
            // 
            // btnbrowseRegistrationFile
            // 
            this.btnbrowseRegistrationFile.Location = new System.Drawing.Point(610, 45);
            this.btnbrowseRegistrationFile.Margin = new System.Windows.Forms.Padding(2);
            this.btnbrowseRegistrationFile.Name = "btnbrowseRegistrationFile";
            this.btnbrowseRegistrationFile.Size = new System.Drawing.Size(46, 30);
            this.btnbrowseRegistrationFile.TabIndex = 49;
            this.btnbrowseRegistrationFile.Text = "...";
            this.btnbrowseRegistrationFile.UseVisualStyleBackColor = true;
            this.btnbrowseRegistrationFile.Click += new System.EventHandler(this.btnbrowseRegistrationFile_Click);
            // 
            // btnBrowsePluginsDLL
            // 
            this.btnBrowsePluginsDLL.Location = new System.Drawing.Point(610, 97);
            this.btnBrowsePluginsDLL.Margin = new System.Windows.Forms.Padding(2);
            this.btnBrowsePluginsDLL.Name = "btnBrowsePluginsDLL";
            this.btnBrowsePluginsDLL.Size = new System.Drawing.Size(46, 26);
            this.btnBrowsePluginsDLL.TabIndex = 51;
            this.btnBrowsePluginsDLL.Text = "...";
            this.btnBrowsePluginsDLL.UseVisualStyleBackColor = true;
            this.btnBrowsePluginsDLL.Click += new System.EventHandler(this.btnBrowsePluginsDLL_Click);
            // 
            // PluginRegistrationControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 18F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.btnBrowsePluginsDLL);
            this.Controls.Add(this.btnbrowseRegistrationFile);
            this.Controls.Add(this.btnRegisterPlugins);
            this.Controls.Add(this.txtBrowsePluginsdll);
            this.Controls.Add(this.txtRegistrationFile);
            this.Controls.Add(this.treeView1);
            this.Controls.Add(this.toolStripMenu);
            this.Margin = new System.Windows.Forms.Padding(4);
            this.Name = "PluginRegistrationControl";
            this.Size = new System.Drawing.Size(1400, 831);
            this.Load += new System.EventHandler(this.MyPluginControl_Load);
            this.toolStripMenu.ResumeLayout(false);
            this.toolStripMenu.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.ToolStrip toolStripMenu;
        private System.Windows.Forms.TreeView treeView1;
        private System.Windows.Forms.TextBox txtRegistrationFile;
        private System.Windows.Forms.TextBox txtBrowsePluginsdll;
        private System.Windows.Forms.Button btnRegisterPlugins;
        private System.Windows.Forms.Button btnbrowseRegistrationFile;
        private System.Windows.Forms.Button btnBrowsePluginsDLL;
        private System.Windows.Forms.ToolStripButton tsbLoadPlugins;
        private System.Windows.Forms.ToolStripButton tsbGenerateXmlFile;
    }
}
