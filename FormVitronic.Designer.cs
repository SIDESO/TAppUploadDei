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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormMainVitronic));
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
            resources.ApplyResources(buttonSearchFolder, "buttonSearchFolder");
            buttonSearchFolder.Name = "buttonSearchFolder";
            buttonSearchFolder.UseVisualStyleBackColor = true;
            buttonSearchFolder.Click += buttonSearchFolder_Click;
            // 
            // DataGridViewDeis
            // 
            DataGridViewDeis.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            resources.ApplyResources(DataGridViewDeis, "DataGridViewDeis");
            DataGridViewDeis.Name = "DataGridViewDeis";
            // 
            // comboBoxPoints
            // 
            resources.ApplyResources(comboBoxPoints, "comboBoxPoints");
            comboBoxPoints.FormattingEnabled = true;
            comboBoxPoints.Name = "comboBoxPoints";
            // 
            // label1
            // 
            resources.ApplyResources(label1, "label1");
            label1.Name = "label1";
            // 
            // buttonUploadData
            // 
            resources.ApplyResources(buttonUploadData, "buttonUploadData");
            buttonUploadData.Name = "buttonUploadData";
            buttonUploadData.UseVisualStyleBackColor = true;
            buttonUploadData.Click += buttonUploadData_Click;
            // 
            // textBoxPathFolder
            // 
            resources.ApplyResources(textBoxPathFolder, "textBoxPathFolder");
            textBoxPathFolder.Name = "textBoxPathFolder";
            textBoxPathFolder.TextChanged += textBoxPathFolder_TextChanged;
            // 
            // label2
            // 
            resources.ApplyResources(label2, "label2");
            label2.Name = "label2";
            // 
            // label3
            // 
            resources.ApplyResources(label3, "label3");
            label3.Name = "label3";
            // 
            // labelTotalFiles
            // 
            resources.ApplyResources(labelTotalFiles, "labelTotalFiles");
            labelTotalFiles.Name = "labelTotalFiles";
            // 
            // label4
            // 
            resources.ApplyResources(label4, "label4");
            label4.Name = "label4";
            // 
            // label5
            // 
            resources.ApplyResources(label5, "label5");
            label5.Name = "label5";
            // 
            // labelFilesLoad
            // 
            resources.ApplyResources(labelFilesLoad, "labelFilesLoad");
            labelFilesLoad.Name = "labelFilesLoad";
            // 
            // labelFilesError
            // 
            resources.ApplyResources(labelFilesError, "labelFilesError");
            labelFilesError.Name = "labelFilesError";
            // 
            // labelErrorsForm
            // 
            resources.ApplyResources(labelErrorsForm, "labelErrorsForm");
            labelErrorsForm.ForeColor = Color.Red;
            labelErrorsForm.Name = "labelErrorsForm";
            // 
            // FormMainVitronic
            // 
            resources.ApplyResources(this, "$this");
            AutoScaleMode = AutoScaleMode.Font;
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
