
namespace DecryptApp
{
    partial class Form1
    {
        /// <summary>
        /// 設計工具所需的變數。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 清除任何使用中的資源。
        /// </summary>
        /// <param name="disposing">如果應該處置受控資源則為 true，否則為 false。</param>
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
            this.butDecode = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.txtInput = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.txtEncrypt = new System.Windows.Forms.TextBox();
            this.butEncrypt = new System.Windows.Forms.Button();
            this.txtDecodeOut = new System.Windows.Forms.TextBox();
            this.txtEncryptOut = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.labSH512 = new System.Windows.Forms.Label();
            this.labShowSH512 = new System.Windows.Forms.Label();
            this.txtBoxShowSH512 = new System.Windows.Forms.TextBox();
            this.btnSH512 = new System.Windows.Forms.Button();
            this.txtBoxInSHA512password = new System.Windows.Forms.TextBox();
            this.txtBoxInSHA512account = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // butDecode
            // 
            this.butDecode.Font = new System.Drawing.Font("新細明體", 12F);
            this.butDecode.Location = new System.Drawing.Point(332, 12);
            this.butDecode.Name = "butDecode";
            this.butDecode.Size = new System.Drawing.Size(128, 42);
            this.butDecode.TabIndex = 0;
            this.butDecode.Text = "解碼執行";
            this.butDecode.UseVisualStyleBackColor = true;
            this.butDecode.Click += new System.EventHandler(this.butDecode_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("新細明體", 12F);
            this.label1.Location = new System.Drawing.Point(12, 19);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(280, 24);
            this.label1.TabIndex = 1;
            this.label1.Text = "輸入要解碼的 Base64 字串";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("新細明體", 12F);
            this.label2.Location = new System.Drawing.Point(12, 168);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(178, 24);
            this.label2.TabIndex = 3;
            this.label2.Text = "顯示解碼後字串";
            // 
            // txtInput
            // 
            this.txtInput.Font = new System.Drawing.Font("新細明體", 12F);
            this.txtInput.Location = new System.Drawing.Point(16, 67);
            this.txtInput.Multiline = true;
            this.txtInput.Name = "txtInput";
            this.txtInput.Size = new System.Drawing.Size(1274, 98);
            this.txtInput.TabIndex = 4;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("新細明體", 12F);
            this.label3.Location = new System.Drawing.Point(12, 344);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(202, 24);
            this.label3.TabIndex = 6;
            this.label3.Text = "輸入要加密的字串";
            // 
            // txtEncrypt
            // 
            this.txtEncrypt.Font = new System.Drawing.Font("新細明體", 12F);
            this.txtEncrypt.Location = new System.Drawing.Point(-7, 387);
            this.txtEncrypt.Multiline = true;
            this.txtEncrypt.Name = "txtEncrypt";
            this.txtEncrypt.Size = new System.Drawing.Size(1297, 109);
            this.txtEncrypt.TabIndex = 7;
            // 
            // butEncrypt
            // 
            this.butEncrypt.Font = new System.Drawing.Font("新細明體", 12F);
            this.butEncrypt.Location = new System.Drawing.Point(332, 336);
            this.butEncrypt.Name = "butEncrypt";
            this.butEncrypt.Size = new System.Drawing.Size(128, 40);
            this.butEncrypt.TabIndex = 9;
            this.butEncrypt.Text = "加密執行";
            this.butEncrypt.UseVisualStyleBackColor = true;
            this.butEncrypt.Click += new System.EventHandler(this.butEncrypt_Click);
            // 
            // txtDecodeOut
            // 
            this.txtDecodeOut.Location = new System.Drawing.Point(16, 195);
            this.txtDecodeOut.Multiline = true;
            this.txtDecodeOut.Name = "txtDecodeOut";
            this.txtDecodeOut.ReadOnly = true;
            this.txtDecodeOut.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.txtDecodeOut.Size = new System.Drawing.Size(1274, 118);
            this.txtDecodeOut.TabIndex = 10;
            // 
            // txtEncryptOut
            // 
            this.txtEncryptOut.Location = new System.Drawing.Point(16, 540);
            this.txtEncryptOut.Multiline = true;
            this.txtEncryptOut.Name = "txtEncryptOut";
            this.txtEncryptOut.ReadOnly = true;
            this.txtEncryptOut.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.txtEncryptOut.Size = new System.Drawing.Size(1274, 118);
            this.txtEncryptOut.TabIndex = 11;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("新細明體", 12F);
            this.label4.Location = new System.Drawing.Point(12, 499);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(178, 24);
            this.label4.TabIndex = 12;
            this.label4.Text = "顯示加密後字串";
            // 
            // labSH512
            // 
            this.labSH512.AutoSize = true;
            this.labSH512.Font = new System.Drawing.Font("新細明體", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.labSH512.Location = new System.Drawing.Point(12, 681);
            this.labSH512.Name = "labSH512";
            this.labSH512.Size = new System.Drawing.Size(184, 24);
            this.labSH512.TabIndex = 13;
            this.labSH512.Text = "計算雜湊SHA512";
            // 
            // labShowSH512
            // 
            this.labShowSH512.AutoSize = true;
            this.labShowSH512.Font = new System.Drawing.Font("新細明體", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.labShowSH512.Location = new System.Drawing.Point(12, 841);
            this.labShowSH512.Name = "labShowSH512";
            this.labShowSH512.Size = new System.Drawing.Size(178, 24);
            this.labShowSH512.TabIndex = 15;
            this.labShowSH512.Text = "顯示計算雜湊後";
            // 
            // txtBoxShowSH512
            // 
            this.txtBoxShowSH512.Location = new System.Drawing.Point(12, 868);
            this.txtBoxShowSH512.Multiline = true;
            this.txtBoxShowSH512.Name = "txtBoxShowSH512";
            this.txtBoxShowSH512.ReadOnly = true;
            this.txtBoxShowSH512.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.txtBoxShowSH512.Size = new System.Drawing.Size(1274, 147);
            this.txtBoxShowSH512.TabIndex = 16;
            // 
            // btnSH512
            // 
            this.btnSH512.Font = new System.Drawing.Font("新細明體", 12F);
            this.btnSH512.Location = new System.Drawing.Point(438, 673);
            this.btnSH512.Name = "btnSH512";
            this.btnSH512.Size = new System.Drawing.Size(178, 40);
            this.btnSH512.TabIndex = 17;
            this.btnSH512.Text = "計算雜湊執行";
            this.btnSH512.UseVisualStyleBackColor = true;
            this.btnSH512.Click += new System.EventHandler(this.btnSH512_Click);
            // 
            // txtBoxInSHA512password
            // 
            this.txtBoxInSHA512password.Font = new System.Drawing.Font("新細明體", 12F);
            this.txtBoxInSHA512password.Location = new System.Drawing.Point(16, 764);
            this.txtBoxInSHA512password.Multiline = true;
            this.txtBoxInSHA512password.Name = "txtBoxInSHA512password";
            this.txtBoxInSHA512password.Size = new System.Drawing.Size(613, 50);
            this.txtBoxInSHA512password.TabIndex = 18;
            // 
            // txtBoxInSHA512account
            // 
            this.txtBoxInSHA512account.Font = new System.Drawing.Font("新細明體", 12F);
            this.txtBoxInSHA512account.Location = new System.Drawing.Point(672, 764);
            this.txtBoxInSHA512account.Multiline = true;
            this.txtBoxInSHA512account.Name = "txtBoxInSHA512account";
            this.txtBoxInSHA512account.Size = new System.Drawing.Size(592, 50);
            this.txtBoxInSHA512account.TabIndex = 19;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Font = new System.Drawing.Font("新細明體", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.label5.Location = new System.Drawing.Point(30, 737);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(102, 24);
            this.label5.TabIndex = 20;
            this.label5.Text = "PassWord";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Font = new System.Drawing.Font("新細明體", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.label6.Location = new System.Drawing.Point(691, 737);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(85, 24);
            this.label6.TabIndex = 21;
            this.label6.Text = "Account";
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 18F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1302, 1030);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.txtBoxInSHA512account);
            this.Controls.Add(this.txtBoxInSHA512password);
            this.Controls.Add(this.btnSH512);
            this.Controls.Add(this.txtBoxShowSH512);
            this.Controls.Add(this.labShowSH512);
            this.Controls.Add(this.labSH512);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.txtEncryptOut);
            this.Controls.Add(this.txtDecodeOut);
            this.Controls.Add(this.butEncrypt);
            this.Controls.Add(this.txtEncrypt);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.txtInput);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.butDecode);
            this.Name = "Form1";
            this.Text = "加密解密應用程式";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button butDecode;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox txtInput;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox txtEncrypt;
        private System.Windows.Forms.Button butEncrypt;
        private System.Windows.Forms.TextBox txtDecodeOut;
        private System.Windows.Forms.TextBox txtEncryptOut;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label labSH512;
        private System.Windows.Forms.Label labShowSH512;
        private System.Windows.Forms.TextBox txtBoxShowSH512;
        private System.Windows.Forms.Button btnSH512;
        private System.Windows.Forms.TextBox txtBoxInSHA512password;
        private System.Windows.Forms.TextBox txtBoxInSHA512account;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label6;
    }
}

