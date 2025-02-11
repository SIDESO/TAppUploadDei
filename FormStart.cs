

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
    }
}
