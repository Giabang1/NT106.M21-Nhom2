
namespace Server
{
    partial class MiniServer
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
            this.tbView = new System.Windows.Forms.TextBox();
            this.tbView2 = new System.Windows.Forms.TextBox();
            this.tbPlayer = new System.Windows.Forms.TextBox();
            this.tbView3 = new System.Windows.Forms.TextBox();
            this.btnStop = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // tbView
            // 
            this.tbView.Location = new System.Drawing.Point(45, 34);
            this.tbView.Multiline = true;
            this.tbView.Name = "tbView";
            this.tbView.Size = new System.Drawing.Size(686, 159);
            this.tbView.TabIndex = 0;
            // 
            // tbView2
            // 
            this.tbView2.Location = new System.Drawing.Point(61, 255);
            this.tbView2.Name = "tbView2";
            this.tbView2.Size = new System.Drawing.Size(145, 22);
            this.tbView2.TabIndex = 1;
            // 
            // tbPlayer
            // 
            this.tbPlayer.Location = new System.Drawing.Point(61, 344);
            this.tbPlayer.Name = "tbPlayer";
            this.tbPlayer.Size = new System.Drawing.Size(145, 22);
            this.tbPlayer.TabIndex = 2;
            // 
            // tbView3
            // 
            this.tbView3.Location = new System.Drawing.Point(455, 255);
            this.tbView3.Name = "tbView3";
            this.tbView3.Size = new System.Drawing.Size(145, 22);
            this.tbView3.TabIndex = 3;
            // 
            // btnStop
            // 
            this.btnStop.Location = new System.Drawing.Point(444, 332);
            this.btnStop.Name = "btnStop";
            this.btnStop.Size = new System.Drawing.Size(177, 46);
            this.btnStop.TabIndex = 4;
            this.btnStop.Text = "EXIT";
            this.btnStop.UseVisualStyleBackColor = true;
            this.btnStop.Click += new System.EventHandler(this.btnStop_Click);
            // 
            // MiniServer
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.btnStop);
            this.Controls.Add(this.tbView3);
            this.Controls.Add(this.tbPlayer);
            this.Controls.Add(this.tbView2);
            this.Controls.Add(this.tbView);
            this.Name = "MiniServer";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "MiniServer";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox tbView;
        private System.Windows.Forms.TextBox tbView2;
        private System.Windows.Forms.TextBox tbPlayer;
        private System.Windows.Forms.TextBox tbView3;
        private System.Windows.Forms.Button btnStop;
    }
}

