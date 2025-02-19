namespace TappUploadDei
{
    partial class FormSelectApp
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
            buttonVitronicApp = new Button();
            buttonGdsExport = new Button();
            button1 = new Button();
            SuspendLayout();
            // 
            // buttonVitronicApp
            // 
            buttonVitronicApp.Font = new Font("Segoe UI", 13.8F, FontStyle.Bold, GraphicsUnit.Point, 0);
            buttonVitronicApp.Location = new Point(97, 74);
            buttonVitronicApp.Name = "buttonVitronicApp";
            buttonVitronicApp.Size = new Size(396, 51);
            buttonVitronicApp.TabIndex = 0;
            buttonVitronicApp.Text = "Carga de archivos VITRONIC";
            buttonVitronicApp.UseVisualStyleBackColor = true;
            buttonVitronicApp.Click += buttonVitronicApp_Click;
            // 
            // buttonGdsExport
            // 
            buttonGdsExport.Font = new Font("Segoe UI", 13.8F, FontStyle.Bold, GraphicsUnit.Point, 0);
            buttonGdsExport.Location = new Point(97, 150);
            buttonGdsExport.Name = "buttonGdsExport";
            buttonGdsExport.Size = new Size(396, 51);
            buttonGdsExport.TabIndex = 1;
            buttonGdsExport.Text = "Exportar eventos GDS";
            buttonGdsExport.UseVisualStyleBackColor = true;
            buttonGdsExport.Click += buttonGdsExport_Click;
            // 
            // button1
            // 
            button1.Font = new Font("Segoe UI", 13.8F, FontStyle.Bold, GraphicsUnit.Point, 0);
            button1.Location = new Point(97, 232);
            button1.Name = "button1";
            button1.Size = new Size(396, 51);
            button1.TabIndex = 2;
            button1.Text = "Cargar archivos TRUCAM";
            button1.UseVisualStyleBackColor = true;
            // 
            // FormSelectApp
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(600, 409);
            Controls.Add(button1);
            Controls.Add(buttonGdsExport);
            Controls.Add(buttonVitronicApp);
            Name = "FormSelectApp";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "Selector de aplicaciones ";
            ResumeLayout(false);
        }

        #endregion

        private Button buttonVitronicApp;
        private Button buttonGdsExport;
        private Button button1;
    }
}