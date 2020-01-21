namespace OnionViewer
{
    partial class FormViewer
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormViewer));
            this.niViewer = new System.Windows.Forms.NotifyIcon(this.components);
            this.cmsNI = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.tsmiOpen = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiClose = new System.Windows.Forms.ToolStripMenuItem();
            this.tcBrowser = new System.Windows.Forms.TabControl();
            this.tpB0 = new System.Windows.Forms.TabPage();
            this.gbConnections = new System.Windows.Forms.GroupBox();
            this.tvwConnections = new System.Windows.Forms.TreeView();
            this.gbStreams = new System.Windows.Forms.GroupBox();
            this.tvwStreams = new System.Windows.Forms.TreeView();
            this.gbCircuits = new System.Windows.Forms.GroupBox();
            this.tvwCircuits = new System.Windows.Forms.TreeView();
            this.tpB1 = new System.Windows.Forms.TabPage();
            this.pgConfig = new System.Windows.Forms.PropertyGrid();
            this.tpB2 = new System.Windows.Forms.TabPage();
            this.lbRouters = new System.Windows.Forms.ListBox();
            this.gwbMain = new Gecko.GeckoWebBrowser();
            this.lbLog = new System.Windows.Forms.Label();
            this.txtAddress = new System.Windows.Forms.TextBox();
            this.cmsNI.SuspendLayout();
            this.tcBrowser.SuspendLayout();
            this.tpB0.SuspendLayout();
            this.gbConnections.SuspendLayout();
            this.gbStreams.SuspendLayout();
            this.gbCircuits.SuspendLayout();
            this.tpB1.SuspendLayout();
            this.tpB2.SuspendLayout();
            this.SuspendLayout();
            // 
            // niViewer
            // 
            this.niViewer.ContextMenuStrip = this.cmsNI;
            this.niViewer.Icon = ((System.Drawing.Icon)(resources.GetObject("niViewer.Icon")));
            this.niViewer.Text = "Onion Viewer";
            this.niViewer.Visible = true;
            // 
            // cmsNI
            // 
            this.cmsNI.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsmiOpen,
            this.tsmiClose});
            this.cmsNI.Name = "cmsNI";
            this.cmsNI.Size = new System.Drawing.Size(129, 64);
            // 
            // tsmiOpen
            // 
            this.tsmiOpen.Name = "tsmiOpen";
            this.tsmiOpen.Size = new System.Drawing.Size(128, 30);
            this.tsmiOpen.Text = "Open";
            // 
            // tsmiClose
            // 
            this.tsmiClose.Name = "tsmiClose";
            this.tsmiClose.Size = new System.Drawing.Size(128, 30);
            this.tsmiClose.Text = "Close";
            // 
            // tcBrowser
            // 
            this.tcBrowser.Controls.Add(this.tpB0);
            this.tcBrowser.Controls.Add(this.tpB1);
            this.tcBrowser.Controls.Add(this.tpB2);
            this.tcBrowser.Dock = System.Windows.Forms.DockStyle.Left;
            this.tcBrowser.Location = new System.Drawing.Point(0, 0);
            this.tcBrowser.Name = "tcBrowser";
            this.tcBrowser.SelectedIndex = 0;
            this.tcBrowser.Size = new System.Drawing.Size(400, 424);
            this.tcBrowser.TabIndex = 3;
            this.tcBrowser.Visible = false;
            // 
            // tpB0
            // 
            this.tpB0.Controls.Add(this.gbConnections);
            this.tpB0.Controls.Add(this.gbStreams);
            this.tpB0.Controls.Add(this.gbCircuits);
            this.tpB0.Location = new System.Drawing.Point(4, 30);
            this.tpB0.Name = "tpB0";
            this.tpB0.Padding = new System.Windows.Forms.Padding(3);
            this.tpB0.Size = new System.Drawing.Size(392, 390);
            this.tpB0.TabIndex = 0;
            this.tpB0.Text = "Status";
            // 
            // gbConnections
            // 
            this.gbConnections.Controls.Add(this.tvwConnections);
            this.gbConnections.Dock = System.Windows.Forms.DockStyle.Fill;
            this.gbConnections.Location = new System.Drawing.Point(3, 105);
            this.gbConnections.Name = "gbConnections";
            this.gbConnections.Size = new System.Drawing.Size(386, 152);
            this.gbConnections.TabIndex = 1;
            this.gbConnections.TabStop = false;
            this.gbConnections.Text = "Current connections";
            // 
            // tvwConnections
            // 
            this.tvwConnections.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tvwConnections.FullRowSelect = true;
            this.tvwConnections.Location = new System.Drawing.Point(3, 25);
            this.tvwConnections.Name = "tvwConnections";
            this.tvwConnections.Size = new System.Drawing.Size(380, 124);
            this.tvwConnections.TabIndex = 0;
            // 
            // gbStreams
            // 
            this.gbStreams.Controls.Add(this.tvwStreams);
            this.gbStreams.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.gbStreams.Location = new System.Drawing.Point(3, 257);
            this.gbStreams.Name = "gbStreams";
            this.gbStreams.Size = new System.Drawing.Size(386, 130);
            this.gbStreams.TabIndex = 2;
            this.gbStreams.TabStop = false;
            this.gbStreams.Text = "Current streams";
            // 
            // tvwStreams
            // 
            this.tvwStreams.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tvwStreams.FullRowSelect = true;
            this.tvwStreams.Location = new System.Drawing.Point(3, 25);
            this.tvwStreams.Name = "tvwStreams";
            this.tvwStreams.Size = new System.Drawing.Size(380, 102);
            this.tvwStreams.TabIndex = 0;
            // 
            // gbCircuits
            // 
            this.gbCircuits.Controls.Add(this.tvwCircuits);
            this.gbCircuits.Dock = System.Windows.Forms.DockStyle.Top;
            this.gbCircuits.Location = new System.Drawing.Point(3, 3);
            this.gbCircuits.Name = "gbCircuits";
            this.gbCircuits.Size = new System.Drawing.Size(386, 102);
            this.gbCircuits.TabIndex = 0;
            this.gbCircuits.TabStop = false;
            this.gbCircuits.Text = "Current circuits";
            // 
            // tvwCircuits
            // 
            this.tvwCircuits.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tvwCircuits.FullRowSelect = true;
            this.tvwCircuits.Location = new System.Drawing.Point(3, 25);
            this.tvwCircuits.Name = "tvwCircuits";
            this.tvwCircuits.Size = new System.Drawing.Size(380, 74);
            this.tvwCircuits.TabIndex = 0;
            // 
            // tpB1
            // 
            this.tpB1.Controls.Add(this.pgConfig);
            this.tpB1.Location = new System.Drawing.Point(4, 30);
            this.tpB1.Name = "tpB1";
            this.tpB1.Padding = new System.Windows.Forms.Padding(3);
            this.tpB1.Size = new System.Drawing.Size(392, 390);
            this.tpB1.TabIndex = 1;
            this.tpB1.Text = "Configuration";
            // 
            // pgConfig
            // 
            this.pgConfig.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pgConfig.Location = new System.Drawing.Point(3, 3);
            this.pgConfig.Name = "pgConfig";
            this.pgConfig.PropertySort = System.Windows.Forms.PropertySort.Alphabetical;
            this.pgConfig.Size = new System.Drawing.Size(386, 384);
            this.pgConfig.TabIndex = 0;
            // 
            // tpB2
            // 
            this.tpB2.Controls.Add(this.lbRouters);
            this.tpB2.Location = new System.Drawing.Point(4, 30);
            this.tpB2.Name = "tpB2";
            this.tpB2.Size = new System.Drawing.Size(392, 390);
            this.tpB2.TabIndex = 2;
            this.tpB2.Text = "Routers";
            // 
            // lbRouters
            // 
            this.lbRouters.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lbRouters.FormattingEnabled = true;
            this.lbRouters.ItemHeight = 21;
            this.lbRouters.Location = new System.Drawing.Point(0, 0);
            this.lbRouters.Name = "lbRouters";
            this.lbRouters.Size = new System.Drawing.Size(392, 390);
            this.lbRouters.TabIndex = 0;
            // 
            // gwbMain
            // 
            this.gwbMain.ConsoleMessageEventReceivesConsoleLogCalls = true;
            this.gwbMain.Dock = System.Windows.Forms.DockStyle.Fill;
            this.gwbMain.FrameEventsPropagateToMainWindow = false;
            this.gwbMain.Location = new System.Drawing.Point(400, 29);
            this.gwbMain.Name = "gwbMain";
            this.gwbMain.Size = new System.Drawing.Size(618, 365);
            this.gwbMain.TabIndex = 6;
            this.gwbMain.UseHttpActivityObserver = false;
            // 
            // lbLog
            // 
            this.lbLog.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.lbLog.Location = new System.Drawing.Point(400, 394);
            this.lbLog.Name = "lbLog";
            this.lbLog.Size = new System.Drawing.Size(618, 30);
            this.lbLog.TabIndex = 5;
            // 
            // txtAddress
            // 
            this.txtAddress.Dock = System.Windows.Forms.DockStyle.Top;
            this.txtAddress.Location = new System.Drawing.Point(400, 0);
            this.txtAddress.Name = "txtAddress";
            this.txtAddress.ReadOnly = true;
            this.txtAddress.Size = new System.Drawing.Size(618, 29);
            this.txtAddress.TabIndex = 4;
            // 
            // FormViewer
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.ClientSize = new System.Drawing.Size(1018, 424);
            this.Controls.Add(this.gwbMain);
            this.Controls.Add(this.lbLog);
            this.Controls.Add(this.txtAddress);
            this.Controls.Add(this.tcBrowser);
            this.Font = new System.Drawing.Font("Segoe UI", 8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "FormViewer";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Onion Viewer";
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            this.cmsNI.ResumeLayout(false);
            this.tcBrowser.ResumeLayout(false);
            this.tpB0.ResumeLayout(false);
            this.gbConnections.ResumeLayout(false);
            this.gbStreams.ResumeLayout(false);
            this.gbCircuits.ResumeLayout(false);
            this.tpB1.ResumeLayout(false);
            this.tpB2.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.NotifyIcon niViewer;
        private System.Windows.Forms.ContextMenuStrip cmsNI;
        private System.Windows.Forms.ToolStripMenuItem tsmiOpen;
        private System.Windows.Forms.ToolStripMenuItem tsmiClose;
        private System.Windows.Forms.TabControl tcBrowser;
        private System.Windows.Forms.TabPage tpB0;
        private System.Windows.Forms.GroupBox gbConnections;
        private System.Windows.Forms.TreeView tvwConnections;
        private System.Windows.Forms.GroupBox gbStreams;
        private System.Windows.Forms.TreeView tvwStreams;
        private System.Windows.Forms.GroupBox gbCircuits;
        private System.Windows.Forms.TreeView tvwCircuits;
        private System.Windows.Forms.TabPage tpB1;
        private System.Windows.Forms.PropertyGrid pgConfig;
        private System.Windows.Forms.TabPage tpB2;
        private System.Windows.Forms.ListBox lbRouters;
        private Gecko.GeckoWebBrowser gwbMain;
        private System.Windows.Forms.Label lbLog;
        private System.Windows.Forms.TextBox txtAddress;
    }
}

