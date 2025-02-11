namespace TappUploadDei
{
    partial class FormMainVitronic
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);

            //finaliza toda la aplicacion
            Application.Exit();
        }

        #region Windows Form Designer generated code

        /// <summary>
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            buttonSearchFolder = new Button();
            DataGridViewDeis = new DataGridView();
            comboBoxPoints = new ComboBox();
            label1 = new Label();
            buttonUploadData = new Button();
            textBoxPathFolder = new TextBox();
            label2 = new Label();
            label3 = new Label();
            labelTotalFiles = new Label();
            label4 = new Label();
            label5 = new Label();
            labelFilesLoad = new Label();
            labelFilesError = new Label();
            labelErrorsForm = new Label();
            ((System.ComponentModel.ISupportInitialize)DataGridViewDeis).BeginInit();
            SuspendLayout();
            // 
            // buttonSearchFolder
            // 
            buttonSearchFolder.Font = new Font("Segoe UI Semibold", 10.8F, FontStyle.Bold, GraphicsUnit.Point, 0);
            buttonSearchFolder.Location = new Point(1212, 36);
            buttonSearchFolder.Name = "buttonSearchFolder";
            buttonSearchFolder.Size = new Size(183, 38);
            buttonSearchFolder.TabIndex = 0;
            buttonSearchFolder.Text = "Examinar";
            buttonSearchFolder.UseVisualStyleBackColor = true;
            buttonSearchFolder.Click += buttonSearchFolder_Click;
            // 
            // DataGridViewDeis
            // 
            DataGridViewDeis.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            DataGridViewDeis.Location = new Point(12, 87);
            DataGridViewDeis.Name = "DataGridViewDeis";
            DataGridViewDeis.RowHeadersWidth = 51;
            DataGridViewDeis.Size = new Size(1383, 451);
            DataGridViewDeis.TabIndex = 2;
            // 
            // comboBoxPoints
            // 
            comboBoxPoints.Font = new Font("Segoe UI", 12F, FontStyle.Regular, GraphicsUnit.Point, 0);
            comboBoxPoints.FormattingEnabled = true;
            comboBoxPoints.Location = new Point(758, 593);
            comboBoxPoints.Name = "comboBoxPoints";
            comboBoxPoints.Size = new Size(467, 36);
            comboBoxPoints.TabIndex = 3;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Font = new Font("Segoe UI Semibold", 12F, FontStyle.Bold, GraphicsUnit.Point, 0);
            label1.Location = new Point(758, 562);
            label1.Name = "label1";
            label1.Size = new Size(197, 28);
            label1.TabIndex = 4;
            label1.Text = "Seleccionar el punto";
            // 
            // buttonUploadData
            // 
            buttonUploadData.Font = new Font("Segoe UI Semibold", 12F, FontStyle.Bold, GraphicsUnit.Point, 0);
            buttonUploadData.Location = new Point(1231, 562);
            buttonUploadData.Name = "buttonUploadData";
            buttonUploadData.Size = new Size(164, 67);
            buttonUploadData.TabIndex = 5;
            buttonUploadData.Text = "Cargar datos";
            buttonUploadData.UseVisualStyleBackColor = true;
            buttonUploadData.Click += buttonUploadData_Click;
            // 
            // textBoxPathFolder
            // 
            textBoxPathFolder.Font = new Font("Segoe UI", 12F, FontStyle.Regular, GraphicsUnit.Point, 0);
            textBoxPathFolder.Location = new Point(12, 34);
            textBoxPathFolder.Name = "textBoxPathFolder";
            textBoxPathFolder.Size = new Size(1194, 34);
            textBoxPathFolder.TabIndex = 7;
            textBoxPathFolder.TextChanged += textBoxPathFolder_TextChanged;
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Font = new Font("Segoe UI", 9F, FontStyle.Bold, GraphicsUnit.Point, 0);
            label2.Location = new Point(12, 11);
            label2.Name = "label2";
            label2.Size = new Size(315, 20);
            label2.TabIndex = 8;
            label2.Text = "Ruta de la carpeta que contiene los archivos";
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Font = new Font("Segoe UI Semibold", 12F, FontStyle.Bold, GraphicsUnit.Point, 0);
            label3.Location = new Point(32, 553);
            label3.Name = "label3";
            label3.Size = new Size(189, 28);
            label3.TabIndex = 9;
            label3.Text = "Archivos analizados";
            // 
            // labelTotalFiles
            // 
            labelTotalFiles.AutoSize = true;
            labelTotalFiles.Font = new Font("Segoe UI Semibold", 12F, FontStyle.Bold, GraphicsUnit.Point, 0);
            labelTotalFiles.Location = new Point(104, 581);
            labelTotalFiles.Name = "labelTotalFiles";
            labelTotalFiles.Size = new Size(23, 28);
            labelTotalFiles.TabIndex = 10;
            labelTotalFiles.Text = "0";
            // 
            // label4
            // 
            label4.AutoSize = true;
            label4.Font = new Font("Segoe UI Semibold", 12F, FontStyle.Bold, GraphicsUnit.Point, 0);
            label4.Location = new Point(271, 553);
            label4.Name = "label4";
            label4.Size = new Size(179, 28);
            label4.TabIndex = 11;
            label4.Text = "Archivos Cargados";
            // 
            // label5
            // 
            label5.AutoSize = true;
            label5.Font = new Font("Segoe UI Semibold", 12F, FontStyle.Bold, GraphicsUnit.Point, 0);
            label5.Location = new Point(492, 553);
            label5.Name = "label5";
            label5.Size = new Size(198, 28);
            label5.TabIndex = 12;
            label5.Text = "Archivos con errores";
            // 
            // labelFilesLoad
            // 
            labelFilesLoad.AutoSize = true;
            labelFilesLoad.Font = new Font("Segoe UI Semibold", 12F, FontStyle.Bold, GraphicsUnit.Point, 0);
            labelFilesLoad.Location = new Point(331, 584);
            labelFilesLoad.Name = "labelFilesLoad";
            labelFilesLoad.Size = new Size(23, 28);
            labelFilesLoad.TabIndex = 13;
            labelFilesLoad.Text = "0";
            // 
            // labelFilesError
            // 
            labelFilesError.AutoSize = true;
            labelFilesError.Font = new Font("Segoe UI Semibold", 12F, FontStyle.Bold, GraphicsUnit.Point, 0);
            labelFilesError.Location = new Point(574, 587);
            labelFilesError.Name = "labelFilesError";
            labelFilesError.Size = new Size(23, 28);
            labelFilesError.TabIndex = 14;
            labelFilesError.Text = "0";
            // 
            // labelErrorsForm
            // 
            labelErrorsForm.AutoSize = true;
            labelErrorsForm.ForeColor = Color.Red;
            labelErrorsForm.Location = new Point(32, 638);
            labelErrorsForm.Name = "labelErrorsForm";
            labelErrorsForm.Size = new Size(0, 20);
            labelErrorsForm.TabIndex = 15;
            // 
            // FormMainVitronic
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1407, 644);
            Controls.Add(labelErrorsForm);
            Controls.Add(labelFilesError);
            Controls.Add(labelFilesLoad);
            Controls.Add(label5);
            Controls.Add(label4);
            Controls.Add(labelTotalFiles);
            Controls.Add(label3);
            Controls.Add(label2);
            Controls.Add(textBoxPathFolder);
            Controls.Add(buttonUploadData);
            Controls.Add(label1);
            Controls.Add(comboBoxPoints);
            Controls.Add(DataGridViewDeis);
            Controls.Add(buttonSearchFolder);
            Name = "FormMainVitronic";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "Archivos Vitronic TApp ";
            ((System.ComponentModel.ISupportInitialize)DataGridViewDeis).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Button buttonSearchFolder;
        private DataGridView DataGridViewDeis;
        private ComboBox comboBoxPoints;
        private Label label1;
        private Button buttonUploadData;
        private TextBox textBoxPathFolder;
        private Label label2;
        private Label label3;
        private Label labelTotalFiles;
        private Label label4;
        private Label label5;
        private Label labelFilesLoad;
        private Label labelFilesError;
        private Label labelErrorsForm;
    }
}
