

using GDSExtractor;

namespace TappUploadDei
{
    public partial class FormSelectApp : Form
    {
        public FormSelectApp()
        {
            InitializeComponent();
        }

        private void buttonVitronicApp_Click(object sender, EventArgs e)
        {

            //creamos una instancia de la clase FormVitronicApp
            FormMainVitronic formVitronicApp = new FormMainVitronic();
            //mostramos el formulario

            formVitronicApp.Show();
            //cerramos el formulario actual  sin finalizar la aplicacion
            this.Dispose(false);


        }

        private void buttonGdsExport_Click(object sender, EventArgs e)
        {
            //creamos una instancia de la clase FormGds
            FormGds formGdsApp = new FormGds();
            //mostramos el formulario

            formGdsApp.Show();
            //cerramos el formulario actual  sin finalizar la aplicacion
            this.Dispose(false);
        }
    }
}
