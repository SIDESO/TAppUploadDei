using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace GDSExtractor
{
    partial class FormGds
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

            //finaliza toda la aplicacion
            Application.Exit();
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormGds));
            panel1 = new Panel();
            buttonReconnectClient = new System.Windows.Forms.Button();
            label4 = new Label();
            panelGetEvents = new Panel();
            labelReposnseBtnGetEvents = new Label();
            dateTimeStartDate = new DateTimePicker();
            label3 = new Label();
            buttonGetEvents = new System.Windows.Forms.Button();
            label1 = new Label();
            dateTimeEndDate = new DateTimePicker();
            label2 = new Label();
            textBoxLimit = new System.Windows.Forms.TextBox();
            infoConection = new Label();
            DataGridViewDeis = new DataGridView();
            panel1.SuspendLayout();
            panelGetEvents.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)DataGridViewDeis).BeginInit();
            SuspendLayout();
            // 
            // panel1
            // 
            panel1.Controls.Add(buttonReconnectClient);
            panel1.Controls.Add(label4);
            panel1.Controls.Add(panelGetEvents);
            panel1.Controls.Add(infoConection);
            panel1.Location = new Point(7, 3);
            panel1.Name = "panel1";
            panel1.Size = new Size(1385, 143);
            panel1.TabIndex = 0;
            // 
            // buttonReconnectClient
            // 
            buttonReconnectClient.Location = new Point(18, 100);
            buttonReconnectClient.Name = "buttonReconnectClient";
            buttonReconnectClient.Size = new Size(182, 29);
            buttonReconnectClient.TabIndex = 13;
            buttonReconnectClient.Text = "Reconectar cliente GDS";
            buttonReconnectClient.UseVisualStyleBackColor = true;
            buttonReconnectClient.Click += buttonReconnectClient_Click;
            // 
            // label4
            // 
            label4.AutoSize = true;
            label4.Font = new Font("Segoe UI", 9F, FontStyle.Bold, GraphicsUnit.Point, 0);
            label4.Location = new Point(18, 6);
            label4.Name = "label4";
            label4.Size = new Size(161, 20);
            label4.TabIndex = 12;
            label4.Text = "Estado conexión GDS:";
            // 
            // panelGetEvents
            // 
            panelGetEvents.BorderStyle = BorderStyle.FixedSingle;
            panelGetEvents.Controls.Add(labelReposnseBtnGetEvents);
            panelGetEvents.Controls.Add(dateTimeStartDate);
            panelGetEvents.Controls.Add(label3);
            panelGetEvents.Controls.Add(buttonGetEvents);
            panelGetEvents.Controls.Add(label1);
            panelGetEvents.Controls.Add(dateTimeEndDate);
            panelGetEvents.Controls.Add(label2);
            panelGetEvents.Controls.Add(textBoxLimit);
            panelGetEvents.Location = new Point(341, 53);
            panelGetEvents.Name = "panelGetEvents";
            panelGetEvents.Size = new Size(1041, 87);
            panelGetEvents.TabIndex = 10;
            // 
            // labelReposnseBtnGetEvents
            // 
            labelReposnseBtnGetEvents.ForeColor = Color.Red;
            labelReposnseBtnGetEvents.Location = new Point(781, 62);
            labelReposnseBtnGetEvents.Name = "labelReposnseBtnGetEvents";
            labelReposnseBtnGetEvents.Size = new Size(246, 23);
            labelReposnseBtnGetEvents.TabIndex = 10;
            labelReposnseBtnGetEvents.UseMnemonic = false;
            // 
            // dateTimeStartDate
            // 
            dateTimeStartDate.Location = new Point(3, 33);
            dateTimeStartDate.Name = "dateTimeStartDate";
            dateTimeStartDate.Size = new Size(311, 27);
            dateTimeStartDate.TabIndex = 0;
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Font = new Font("Segoe UI", 9F, FontStyle.Bold, GraphicsUnit.Point, 0);
            label3.Location = new Point(634, 10);
            label3.Name = "label3";
            label3.Size = new Size(119, 20);
            label3.TabIndex = 7;
            label3.Text = "Limite registros";
            label3.UseMnemonic = false;
            // 
            // buttonGetEvents
            // 
            buttonGetEvents.Font = new Font("Segoe UI", 9F, FontStyle.Bold, GraphicsUnit.Point, 0);
            buttonGetEvents.Location = new Point(781, 10);
            buttonGetEvents.Name = "buttonGetEvents";
            buttonGetEvents.Size = new Size(246, 50);
            buttonGetEvents.TabIndex = 9;
            buttonGetEvents.Text = "Consultar eventos en GDS";
            buttonGetEvents.UseVisualStyleBackColor = true;
            buttonGetEvents.Click += buttonGetEvents_Click;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Font = new Font("Segoe UI", 9F, FontStyle.Bold, GraphicsUnit.Point, 0);
            label1.Location = new Point(317, 10);
            label1.Name = "label1";
            label1.Size = new Size(107, 20);
            label1.TabIndex = 6;
            label1.Text = "Fecha final (*)";
            label1.UseMnemonic = false;
            // 
            // dateTimeEndDate
            // 
            dateTimeEndDate.Location = new Point(317, 33);
            dateTimeEndDate.Name = "dateTimeEndDate";
            dateTimeEndDate.Size = new Size(311, 27);
            dateTimeEndDate.TabIndex = 1;
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Font = new Font("Segoe UI", 9F, FontStyle.Bold, GraphicsUnit.Point, 0);
            label2.Location = new Point(3, 10);
            label2.Name = "label2";
            label2.Size = new Size(112, 20);
            label2.TabIndex = 5;
            label2.Text = "Fecha inicial(*)";
            label2.UseMnemonic = false;
            // 
            // textBoxLimit
            // 
            textBoxLimit.Location = new Point(634, 33);
            textBoxLimit.Name = "textBoxLimit";
            textBoxLimit.Size = new Size(141, 27);
            textBoxLimit.TabIndex = 8;
            textBoxLimit.KeyPress += textBoxLimit_KeyPress;
            // 
            // infoConection
            // 
            infoConection.AccessibleDescription = "infoConection";
            infoConection.AccessibleName = "infoConection";
            infoConection.AutoSize = true;
            infoConection.Location = new Point(180, 9);
            infoConection.Name = "infoConection";
            infoConection.Size = new Size(0, 20);
            infoConection.TabIndex = 1;
            // 
            // DataGridViewDeis
            // 
            DataGridViewDeis.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;
            DataGridViewDeis.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.AllCells;
            DataGridViewDeis.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            DataGridViewDeis.Location = new Point(7, 152);
            DataGridViewDeis.Name = "DataGridViewDeis";
            DataGridViewDeis.RowHeadersWidth = 51;
            DataGridViewDeis.Size = new Size(1385, 477);
            DataGridViewDeis.TabIndex = 0;
            // 
            // FormGds
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1404, 734);
            Controls.Add(DataGridViewDeis);
            Controls.Add(panel1);
            FormBorderStyle = FormBorderStyle.FixedSingle;
            Icon = (Icon)resources.GetObject("$this.Icon");
            MaximizeBox = false;
            Name = "FormGds";
            RightToLeftLayout = true;
            Text = "Extractor Gds - TApp";
            panel1.ResumeLayout(false);
            panel1.PerformLayout();
            panelGetEvents.ResumeLayout(false);
            panelGetEvents.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)DataGridViewDeis).EndInit();
            ResumeLayout(false);
        }

        private void textBoxLimit_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
            {
                e.Handled = true;
            }
        }



        #endregion

        private Panel panel1;
        public Label infoConection;
        public DataGridView DataGridViewDeis;
        private DateTimePicker dateTimeStartDate;
        private DateTimePicker dateTimeEndDate;
        private Label line;
        private Label label2;
        private Label label3;
        private Label label1;
        private System.Windows.Forms.TextBox textBoxLimit;
        private System.Windows.Forms.Button buttonGetEvents;
        private Panel panelGetEvents;
        private Label labelReposnseBtnGetEvents;
        private Label label4;
        private System.Windows.Forms.Button buttonReconnectClient;
    }
}