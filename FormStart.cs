using System.Net;
using System.Web;
using GDSExtractor;
using Newtonsoft.Json;

namespace TappUploadDei
{
    public partial class FormSelectApp : Form
    {

        string? url = null;
        string[] args;
        string? accessToken = null;
        string? endPoint = null;
        string? userName = null;
        private HttpClient httpClient = new HttpClient();

        public FormSelectApp()
        {
            args = Environment.GetCommandLineArgs();

            InitializeComponent();

            //desactivamos todos los botones

            buttonVitronicApp.Enabled = false;
            buttonGdsExport.Enabled = false;
            buttonTruCamExport.Enabled = false;
            buttonDragonCamExport.Enabled = false;


            //verificamos que el aplicativo fue iniciado desde TránsitoApp

            if (args.Length < 2)
            {
                MessageBox.Show("Este aplicativo requiere ser iniciado desde TránsitoApp", "Error");
                this.Close();
                return;
            }

            this.url = args[1];

            ValidateParamInitial();
        }

        /**
         * validate the parameters and boot the application
         */
        public void ValidateParamInitial()
        {

            try
            {

                if (url == null)
                {
                    MessageBox.Show("No se encontró la url de arranque", "Error");
                    this.Close();
                    return;
                }

                string queryString = url.Split("?")[1];
                var paramsCollection = HttpUtility.ParseQueryString(queryString);

                accessToken = paramsCollection["accessToken"];
                if (accessToken == null || accessToken == String.Empty)
                {
                    MessageBox.Show("No se encontró el token web", "Error");
                    this.Close();
                }

                endPoint = paramsCollection["endPoint"];
                if (string.IsNullOrEmpty(endPoint))
                {
                    MessageBox.Show("No se definió un endpoint");
                    this.Close();
                }

                userName = paramsCollection["userName"];
                if (string.IsNullOrEmpty(userName))
                {
                    MessageBox.Show("No se encontró nombre del usuario");
                    this.Close();
                }

                this.Text = "Bienvenido " + userName;

            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al iniciar la aplicación " + url + " " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                this.Close();
                return;
            }

            //autenticar el cliente
            httpClient.DefaultRequestHeaders.TryAddWithoutValidation("Authorization", "Bearer " + accessToken);
            httpClient.DefaultRequestHeaders.TryAddWithoutValidation("Accept", "application/json");
            GetAppsActives();


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


        private void buttonTruCamExport_Click(object sender, EventArgs e)
        {

            //creamos una instancia de la clase FormTruCam
            FormTruCam formTruCamApp = new FormTruCam();
            //mostramos el formulario
            formTruCamApp.Show();
            //cerramos el formulario actual  sin finalizar la aplicacion
            this.Dispose(false);

        }

        private void buttonDragonCamExport_Click(object sender, EventArgs e)
        {

            //creamos una instancia de la clase FormDragonCam
            FormDragonCam formTruCamApp = new FormDragonCam();
            //mostramos el formulario
            formTruCamApp.Show();
            //cerramos el formulario actual  sin finalizar la aplicacion
            this.Dispose(false);

        }

        /*
         * 
         * consultar las aplicaciones activas
         */
        public void GetAppsActives()
        {
            try
            {
                httpClient.GetAsync(endPoint + "/windows_apps_api_dei/get_active_windows_app").ContinueWith(response =>
                {
                    try
                    {
                        var result = response.Result;
                        var content = result.Content.ReadAsStringAsync().Result;
                        if (result.StatusCode != HttpStatusCode.OK)
                        {
                            MessageBox.Show("Error al obtener la lista de aplicaciones, no se pudo conectar con el servidor", "Error");
                            this.Close();
                        }

                        if (content == null || content == String.Empty)
                        {
                            MessageBox.Show("No se obtuvo la lista de aplicaciones", "Error");
                            this.Close();
                        }
                        /*
                         * [{"app_name":"FilesVitronic"},{"app_name":"FilesTruCam"},{"app_name":"FilesDragonCam"}]
                         */

                        //deserializamos el json
                        var apps_actives = JsonConvert.DeserializeObject<List<AppW>>(content);

                        foreach (string app in apps_actives.Select(x => x.app_name))
                        {

                            this.Invoke((MethodInvoker)delegate
                                {
                                    switch (app)
                                    {
                                        case "FilesVitronic":
                                            buttonVitronicApp.Enabled = true;
                                            break;
                                        case "GDSExport":
                                            buttonGdsExport.Enabled = true;
                                            break;
                                        case "FilesTruCam":
                                            buttonTruCamExport.Enabled = true;
                                            break;
                                        case "FilesDragonCam":
                                            buttonDragonCamExport.Enabled = true;
                                            break;
                                    }

                                });

                        }

                    }
                    catch (Exception ex)
                    {
                        this.Invoke((MethodInvoker)delegate
                        {
                            MessageBox.Show("Error al obtener la versión de la aplicación, no se pudo conectar con el servidor" + " " + ex.Message, "Error");
                            this.Close();
                        });
                    }

                });


            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al obtener la versión de la aplicación, no se pudo conectar con el servidor", "Error");
                this.Close();
            }

        }


    }

    public class AppW
    {
        [JsonProperty("app_name")]
        public string app_name { get; set; }

    }
}
