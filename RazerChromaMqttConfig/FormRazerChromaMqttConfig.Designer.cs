namespace RazerChromaMqttConfig
{
    partial class FormRazerChromaMqttConfig
    {
        /// <summary>
        /// Erforderliche Designervariable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Verwendete Ressourcen bereinigen.
        /// </summary>
        /// <param name="disposing">True, wenn verwaltete Ressourcen gelöscht werden sollen; andernfalls False.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Vom Windows Form-Designer generierter Code

        /// <summary>
        /// Erforderliche Methode für die Designerunterstützung.
        /// Der Inhalt der Methode darf nicht mit dem Code-Editor geändert werden.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.tbMqttHost = new System.Windows.Forms.TextBox();
            this.labMqttHost = new System.Windows.Forms.Label();
            this.labMqttPort = new System.Windows.Forms.Label();
            this.tbMqttPort = new System.Windows.Forms.TextBox();
            this.labMqttUser = new System.Windows.Forms.Label();
            this.tbMqttUser = new System.Windows.Forms.TextBox();
            this.labMqttPW = new System.Windows.Forms.Label();
            this.tbMqttPW = new System.Windows.Forms.TextBox();
            this.labMqttPreTopic = new System.Windows.Forms.Label();
            this.tbMqttPreTopic = new System.Windows.Forms.TextBox();
            this.gbMQTT = new System.Windows.Forms.GroupBox();
            this.butSaveRestart = new System.Windows.Forms.Button();
            this.butSave = new System.Windows.Forms.Button();
            this.gbService = new System.Windows.Forms.GroupBox();
            this.butServiceStop = new System.Windows.Forms.Button();
            this.butServiceStrat = new System.Windows.Forms.Button();
            this.labServieState = new System.Windows.Forms.Label();
            this.updateTimer = new System.Windows.Forms.Timer(this.components);
            this.gbMQTT.SuspendLayout();
            this.gbService.SuspendLayout();
            this.SuspendLayout();
            // 
            // tbMqttHost
            // 
            this.tbMqttHost.Location = new System.Drawing.Point(102, 22);
            this.tbMqttHost.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.tbMqttHost.Name = "tbMqttHost";
            this.tbMqttHost.Size = new System.Drawing.Size(173, 20);
            this.tbMqttHost.TabIndex = 3;
            this.tbMqttHost.Text = "127.0.0.01";
            // 
            // labMqttHost
            // 
            this.labMqttHost.AutoSize = true;
            this.labMqttHost.Location = new System.Drawing.Point(6, 25);
            this.labMqttHost.Name = "labMqttHost";
            this.labMqttHost.Size = new System.Drawing.Size(29, 13);
            this.labMqttHost.TabIndex = 1;
            this.labMqttHost.Text = "Host";
            // 
            // labMqttPort
            // 
            this.labMqttPort.AutoSize = true;
            this.labMqttPort.Location = new System.Drawing.Point(6, 49);
            this.labMqttPort.Name = "labMqttPort";
            this.labMqttPort.Size = new System.Drawing.Size(26, 13);
            this.labMqttPort.TabIndex = 3;
            this.labMqttPort.Text = "Port";
            // 
            // tbMqttPort
            // 
            this.tbMqttPort.Location = new System.Drawing.Point(102, 46);
            this.tbMqttPort.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.tbMqttPort.Name = "tbMqttPort";
            this.tbMqttPort.Size = new System.Drawing.Size(173, 20);
            this.tbMqttPort.TabIndex = 4;
            this.tbMqttPort.Text = "1883";
            // 
            // labMqttUser
            // 
            this.labMqttUser.AutoSize = true;
            this.labMqttUser.Location = new System.Drawing.Point(6, 73);
            this.labMqttUser.Name = "labMqttUser";
            this.labMqttUser.Size = new System.Drawing.Size(29, 13);
            this.labMqttUser.TabIndex = 5;
            this.labMqttUser.Text = "User";
            // 
            // tbMqttUser
            // 
            this.tbMqttUser.Location = new System.Drawing.Point(102, 70);
            this.tbMqttUser.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.tbMqttUser.Name = "tbMqttUser";
            this.tbMqttUser.Size = new System.Drawing.Size(173, 20);
            this.tbMqttUser.TabIndex = 5;
            // 
            // labMqttPW
            // 
            this.labMqttPW.AutoSize = true;
            this.labMqttPW.Location = new System.Drawing.Point(6, 97);
            this.labMqttPW.Name = "labMqttPW";
            this.labMqttPW.Size = new System.Drawing.Size(25, 13);
            this.labMqttPW.TabIndex = 7;
            this.labMqttPW.Text = "PW";
            // 
            // tbMqttPW
            // 
            this.tbMqttPW.Location = new System.Drawing.Point(102, 94);
            this.tbMqttPW.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.tbMqttPW.Name = "tbMqttPW";
            this.tbMqttPW.Size = new System.Drawing.Size(173, 20);
            this.tbMqttPW.TabIndex = 6;
            // 
            // labMqttPreTopic
            // 
            this.labMqttPreTopic.AutoSize = true;
            this.labMqttPreTopic.Location = new System.Drawing.Point(6, 121);
            this.labMqttPreTopic.Name = "labMqttPreTopic";
            this.labMqttPreTopic.Size = new System.Drawing.Size(53, 13);
            this.labMqttPreTopic.TabIndex = 9;
            this.labMqttPreTopic.Text = "Pre Topic";
            // 
            // tbMqttPreTopic
            // 
            this.tbMqttPreTopic.Location = new System.Drawing.Point(102, 118);
            this.tbMqttPreTopic.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.tbMqttPreTopic.Name = "tbMqttPreTopic";
            this.tbMqttPreTopic.Size = new System.Drawing.Size(173, 20);
            this.tbMqttPreTopic.TabIndex = 7;
            this.tbMqttPreTopic.Text = "chroma/";
            // 
            // gbMQTT
            // 
            this.gbMQTT.Controls.Add(this.butSaveRestart);
            this.gbMQTT.Controls.Add(this.butSave);
            this.gbMQTT.Controls.Add(this.tbMqttHost);
            this.gbMQTT.Controls.Add(this.labMqttPreTopic);
            this.gbMQTT.Controls.Add(this.labMqttHost);
            this.gbMQTT.Controls.Add(this.tbMqttPreTopic);
            this.gbMQTT.Controls.Add(this.tbMqttPort);
            this.gbMQTT.Controls.Add(this.labMqttPW);
            this.gbMQTT.Controls.Add(this.labMqttPort);
            this.gbMQTT.Controls.Add(this.tbMqttPW);
            this.gbMQTT.Controls.Add(this.tbMqttUser);
            this.gbMQTT.Controls.Add(this.labMqttUser);
            this.gbMQTT.Location = new System.Drawing.Point(12, 109);
            this.gbMQTT.Name = "gbMQTT";
            this.gbMQTT.Size = new System.Drawing.Size(283, 173);
            this.gbMQTT.TabIndex = 10;
            this.gbMQTT.TabStop = false;
            this.gbMQTT.Text = ",";
            // 
            // butSaveRestart
            // 
            this.butSaveRestart.Location = new System.Drawing.Point(153, 141);
            this.butSaveRestart.Name = "butSaveRestart";
            this.butSaveRestart.Size = new System.Drawing.Size(120, 22);
            this.butSaveRestart.TabIndex = 10;
            this.butSaveRestart.Text = "Save + Restart";
            this.butSaveRestart.UseVisualStyleBackColor = true;
            this.butSaveRestart.Click += new System.EventHandler(this.butSaveRestart_Click);
            // 
            // butSave
            // 
            this.butSave.Location = new System.Drawing.Point(102, 141);
            this.butSave.Name = "butSave";
            this.butSave.Size = new System.Drawing.Size(45, 22);
            this.butSave.TabIndex = 8;
            this.butSave.Text = "Save";
            this.butSave.UseVisualStyleBackColor = true;
            this.butSave.Click += new System.EventHandler(this.butSave_Click);
            // 
            // gbService
            // 
            this.gbService.Controls.Add(this.butServiceStop);
            this.gbService.Controls.Add(this.butServiceStrat);
            this.gbService.Controls.Add(this.labServieState);
            this.gbService.Location = new System.Drawing.Point(12, 12);
            this.gbService.Name = "gbService";
            this.gbService.Size = new System.Drawing.Size(283, 91);
            this.gbService.TabIndex = 11;
            this.gbService.TabStop = false;
            this.gbService.Text = "Service";
            // 
            // butServiceStop
            // 
            this.butServiceStop.Location = new System.Drawing.Point(98, 41);
            this.butServiceStop.Name = "butServiceStop";
            this.butServiceStop.Size = new System.Drawing.Size(86, 22);
            this.butServiceStop.TabIndex = 2;
            this.butServiceStop.Text = "Stop";
            this.butServiceStop.UseVisualStyleBackColor = true;
            this.butServiceStop.Click += new System.EventHandler(this.butServiceStop_Click);
            // 
            // butServiceStrat
            // 
            this.butServiceStrat.Location = new System.Drawing.Point(6, 41);
            this.butServiceStrat.Name = "butServiceStrat";
            this.butServiceStrat.Size = new System.Drawing.Size(86, 22);
            this.butServiceStrat.TabIndex = 1;
            this.butServiceStrat.Text = "Start";
            this.butServiceStrat.UseVisualStyleBackColor = true;
            this.butServiceStrat.Click += new System.EventHandler(this.butServiceStrat_Click);
            // 
            // labServieState
            // 
            this.labServieState.AutoSize = true;
            this.labServieState.Location = new System.Drawing.Point(6, 16);
            this.labServieState.Name = "labServieState";
            this.labServieState.Size = new System.Drawing.Size(76, 13);
            this.labServieState.TabIndex = 0;
            this.labServieState.Text = "labServieState";
            // 
            // updateTimer
            // 
            this.updateTimer.Interval = 500;
            this.updateTimer.Tick += new System.EventHandler(this.updateTimer_Tick);
            // 
            // FormRazerChromaMqttConfig
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(306, 288);
            this.Controls.Add(this.gbService);
            this.Controls.Add(this.gbMQTT);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.Name = "FormRazerChromaMqttConfig";
            this.Text = "Razer Chroma Mqtt Config";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.gbMQTT.ResumeLayout(false);
            this.gbMQTT.PerformLayout();
            this.gbService.ResumeLayout(false);
            this.gbService.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TextBox tbMqttHost;
        private System.Windows.Forms.Label labMqttHost;
        private System.Windows.Forms.Label labMqttPort;
        private System.Windows.Forms.TextBox tbMqttPort;
        private System.Windows.Forms.Label labMqttUser;
        private System.Windows.Forms.TextBox tbMqttUser;
        private System.Windows.Forms.Label labMqttPW;
        private System.Windows.Forms.TextBox tbMqttPW;
        private System.Windows.Forms.Label labMqttPreTopic;
        private System.Windows.Forms.TextBox tbMqttPreTopic;
        private System.Windows.Forms.GroupBox gbMQTT;
        private System.Windows.Forms.GroupBox gbService;
        private System.Windows.Forms.Label labServieState;
        private System.Windows.Forms.Button butServiceStop;
        private System.Windows.Forms.Button butServiceStrat;
        private System.Windows.Forms.Timer updateTimer;
        private System.Windows.Forms.Button butSave;
        private System.Windows.Forms.Button butSaveRestart;
    }
}

