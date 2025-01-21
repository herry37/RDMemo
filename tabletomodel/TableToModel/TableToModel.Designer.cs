namespace TableToModel
{
    partial class TableToModel
    {
        /// <summary>
        /// 設計工具所需的變數。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 清除任何使用中的資源。
        /// </summary>
        /// <param name="disposing">如果應該處置 Managed 資源則為 true，否則為 false。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form 設計工具產生的程式碼

        /// <summary>
        /// 此為設計工具支援所需的方法 - 請勿使用程式碼編輯器修改
        /// 這個方法的內容。
        /// </summary>
        private void InitializeComponent()
        {
            label1 = new Label();
            label2 = new Label();
            label3 = new Label();
            label4 = new Label();
            txtID = new TextBox();
            txtPass = new TextBox();
            txtIP = new TextBox();
            btnCheck = new Button();
            btnModel = new Button();
            label5 = new Label();
            txtTableName = new TextBox();
            txtModel = new TextBox();
            label6 = new Label();
            txtDbName = new TextBox();
            label7 = new Label();
            comDBType = new ComboBox();
            label8 = new Label();
            SuspendLayout();
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Font = new Font("新細明體", 14F, FontStyle.Bold, GraphicsUnit.Point, 136);
            label1.ForeColor = SystemColors.HotTrack;
            label1.Location = new Point(15, 80);
            label1.Margin = new Padding(4, 0, 4, 0);
            label1.Name = "label1";
            label1.Size = new Size(205, 28);
            label1.TabIndex = 0;
            label1.Text = "Server/Host:Port";
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Font = new Font("新細明體", 16F, FontStyle.Bold, GraphicsUnit.Point, 136);
            label2.Location = new Point(12, 12);
            label2.Margin = new Padding(4, 0, 4, 0);
            label2.Name = "label2";
            label2.Size = new Size(185, 32);
            label2.TabIndex = 1;
            label2.Text = "Db連線資訊";
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Font = new Font("新細明體", 12F, FontStyle.Bold, GraphicsUnit.Point, 136);
            label3.ForeColor = SystemColors.HotTrack;
            label3.Location = new Point(12, 202);
            label3.Margin = new Padding(4, 0, 4, 0);
            label3.Name = "label3";
            label3.Size = new Size(141, 24);
            label3.TabIndex = 2;
            label3.Text = "PASSWORD";
            // 
            // label4
            // 
            label4.AutoSize = true;
            label4.Font = new Font("新細明體", 14F, FontStyle.Bold, GraphicsUnit.Point, 136);
            label4.ForeColor = SystemColors.HotTrack;
            label4.Location = new Point(16, 138);
            label4.Margin = new Padding(4, 0, 4, 0);
            label4.Name = "label4";
            label4.Size = new Size(42, 28);
            label4.TabIndex = 3;
            label4.Text = "ID";
            // 
            // txtID
            // 
            txtID.Font = new Font("新細明體", 14F, FontStyle.Regular, GraphicsUnit.Point, 136);
            txtID.Location = new Point(232, 135);
            txtID.Margin = new Padding(4);
            txtID.Name = "txtID";
            txtID.Size = new Size(266, 41);
            txtID.TabIndex = 4;
            // 
            // txtPass
            // 
            txtPass.Font = new Font("新細明體", 14F, FontStyle.Regular, GraphicsUnit.Point, 136);
            txtPass.Location = new Point(228, 196);
            txtPass.Margin = new Padding(4);
            txtPass.Name = "txtPass";
            txtPass.Size = new Size(266, 41);
            txtPass.TabIndex = 5;
            // 
            // txtIP
            // 
            txtIP.Font = new Font("新細明體", 14F, FontStyle.Regular, GraphicsUnit.Point, 136);
            txtIP.Location = new Point(227, 73);
            txtIP.Margin = new Padding(4);
            txtIP.Name = "txtIP";
            txtIP.Size = new Size(550, 41);
            txtIP.TabIndex = 6;
            // 
            // btnCheck
            // 
            btnCheck.Font = new Font("新細明體", 11F, FontStyle.Bold, GraphicsUnit.Point, 136);
            btnCheck.Location = new Point(658, 291);
            btnCheck.Margin = new Padding(4);
            btnCheck.Name = "btnCheck";
            btnCheck.Size = new Size(160, 45);
            btnCheck.TabIndex = 7;
            btnCheck.Text = "測試連線";
            btnCheck.UseVisualStyleBackColor = true;
            btnCheck.Click += btnCheck_Click;
            // 
            // btnModel
            // 
            btnModel.Font = new Font("新細明體", 11F, FontStyle.Bold, GraphicsUnit.Point, 136);
            btnModel.Location = new Point(878, 290);
            btnModel.Margin = new Padding(4);
            btnModel.Name = "btnModel";
            btnModel.Size = new Size(136, 50);
            btnModel.TabIndex = 8;
            btnModel.Text = "產Model";
            btnModel.UseVisualStyleBackColor = true;
            btnModel.Click += btnModel_Click;
            // 
            // label5
            // 
            label5.AutoSize = true;
            label5.Font = new Font("Microsoft JhengHei UI", 13F, FontStyle.Bold);
            label5.ForeColor = SystemColors.HotTrack;
            label5.Location = new Point(-1, 296);
            label5.Margin = new Padding(4, 0, 4, 0);
            label5.Name = "label5";
            label5.Size = new Size(221, 33);
            label5.TabIndex = 8;
            label5.Text = "Table/Collection";
            // 
            // txtTableName
            // 
            txtTableName.Font = new Font("新細明體", 14F, FontStyle.Regular, GraphicsUnit.Point, 136);
            txtTableName.Location = new Point(227, 290);
            txtTableName.Margin = new Padding(4);
            txtTableName.Name = "txtTableName";
            txtTableName.Size = new Size(335, 41);
            txtTableName.TabIndex = 11;
            // 
            // txtModel
            // 
            txtModel.Location = new Point(16, 377);
            txtModel.Margin = new Padding(4);
            txtModel.Multiline = true;
            txtModel.Name = "txtModel";
            txtModel.Size = new Size(1378, 694);
            txtModel.TabIndex = 100;
            // 
            // label6
            // 
            label6.AutoSize = true;
            label6.Font = new Font("新細明體", 14F, FontStyle.Bold, GraphicsUnit.Point, 136);
            label6.ForeColor = Color.Blue;
            label6.Location = new Point(799, 156);
            label6.Margin = new Padding(4, 0, 4, 0);
            label6.Name = "label6";
            label6.Size = new Size(121, 28);
            label6.TabIndex = 51;
            label6.Text = "Db Name";
            // 
            // txtDbName
            // 
            txtDbName.Font = new Font("新細明體", 14F, FontStyle.Regular, GraphicsUnit.Point, 136);
            txtDbName.Location = new Point(990, 152);
            txtDbName.Margin = new Padding(4);
            txtDbName.Name = "txtDbName";
            txtDbName.Size = new Size(191, 41);
            txtDbName.TabIndex = 52;
            // 
            // label7
            // 
            label7.AutoSize = true;
            label7.Font = new Font("新細明體", 12F, FontStyle.Bold, GraphicsUnit.Point, 136);
            label7.Location = new Point(836, 73);
            label7.Margin = new Padding(4, 0, 4, 0);
            label7.Name = "label7";
            label7.Size = new Size(266, 24);
            label7.TabIndex = 101;
            label7.Text = "ex：11.111.111.111,2011";
            // 
            // comDBType
            // 
            comDBType.Font = new Font("新細明體", 13F);
            comDBType.FormattingEnabled = true;
            comDBType.Location = new Point(1085, 294);
            comDBType.Margin = new Padding(4);
            comDBType.Name = "comDBType";
            comDBType.Size = new Size(147, 34);
            comDBType.TabIndex = 103;
            // 
            // label8
            // 
            label8.AutoSize = true;
            label8.Font = new Font("新細明體", 13F);
            label8.Location = new Point(1079, 240);
            label8.Margin = new Padding(4, 0, 4, 0);
            label8.Name = "label8";
            label8.Size = new Size(142, 26);
            label8.TabIndex = 104;
            label8.Text = "資料庫類型";
            // 
            // TableToModel
            // 
            AutoScaleDimensions = new SizeF(11F, 23F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1409, 1087);
            Controls.Add(label8);
            Controls.Add(comDBType);
            Controls.Add(label7);
            Controls.Add(txtDbName);
            Controls.Add(label6);
            Controls.Add(txtModel);
            Controls.Add(txtTableName);
            Controls.Add(label5);
            Controls.Add(btnModel);
            Controls.Add(btnCheck);
            Controls.Add(txtIP);
            Controls.Add(txtPass);
            Controls.Add(txtID);
            Controls.Add(label4);
            Controls.Add(label3);
            Controls.Add(label2);
            Controls.Add(label1);
            Margin = new Padding(4);
            Name = "TableToModel";
            Text = "產Model";
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox txtID;
        private System.Windows.Forms.TextBox txtPass;
        private System.Windows.Forms.TextBox txtIP;
        private System.Windows.Forms.Button btnCheck;
        private System.Windows.Forms.Button btnModel;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TextBox txtTableName;
        private System.Windows.Forms.TextBox txtModel;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.TextBox txtDbName;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.ComboBox comDBType;
        private System.Windows.Forms.Label label8;
    }
}
