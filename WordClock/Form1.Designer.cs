namespace WindowsFormsApplication1 {
	partial class Form1 {
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		protected override void Dispose(bool disposing) {
			if (disposing && (components != null)) {
				components.Dispose();
			}
			base.Dispose(disposing);
		}

		#region Windows Form Designer generated code

		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent() {
			this.components = new System.ComponentModel.Container();
			this.pnDemo = new XPanel();
			this.timer1 = new System.Windows.Forms.Timer(this.components);
			this.tmAnimation = new System.Windows.Forms.Timer(this.components);
			this.SuspendLayout();
			// 
			// pnDemo
			// 
			this.pnDemo.Location = new System.Drawing.Point(12, 12);
			this.pnDemo.Name = "pnDemo";
			this.pnDemo.Size = new System.Drawing.Size(200, 100);
			this.pnDemo.TabIndex = 0;
			// 
			// timer1
			// 
			this.timer1.Interval = 1000;
			this.timer1.Tick += new System.EventHandler(this.timer1_Tick);

			// 
			// tmAnimation
			// 
			this.tmAnimation.Interval = 50;
			this.tmAnimation.Tick += TmAnimation_Tick;
			// 
			// Form1
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(916, 630);
			this.Controls.Add(this.pnDemo);
			this.Name = "Form1";
			this.Text = "Form1";
			this.Load += new System.EventHandler(this.Form1_Load);
			this.ResumeLayout(false);

		}

		

		#endregion

		private XPanel pnDemo;
		private System.Windows.Forms.Timer timer1;
		private System.Windows.Forms.Timer tmAnimation;
	}
}

