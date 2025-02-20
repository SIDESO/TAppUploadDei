using System.Net;
using System.Web;
using messages.Gds.Websocket;
using TappUploadDei;
using static TappUploadDei.SdkGds;

namespace GDSExtractor
{
    public partial class FormGds : Form, TappValidation
    {
        readonly string uriGds = "ws://127.0.0.1:8888/gate";
        public string userNameGds = "developer";

        string? url = null;
        string[] args;
        string? accessToken = null;
        string? endPoint = null;
        string? userName = null;
        public BindingSource bindingSourceDei = new BindingSource();
        int countUpload = 0;
        int countError = 0;
        private HttpClient httpClient = new HttpClient();
        private readonly string ProductVersionApp = Application.ProductVersion.Split("+")[0];
        public readonly string commandApplication = "GDS";
        private readonly string AplicationName = "GDSExport";

        public TestListener listener;

        //get endpoint
        public string GetEndpoint()
        {
            return this.endPoint ?? "";
        }
        //get




        public FormGds()
        {

            args = Environment.GetCommandLineArgs();

            InitializeComponent();

            if (args.Length < 2)
            {
                MessageBox.Show("Este aplicativo requiere ser iniciado desde TránsitoApp", "Error");
                this.Close();
                return;
            }

            this.url = args[1];

            ValidateParamInitial();


            //crear el directorio de imagenes
            // Specify the directory you want to manipulate.
            string path = @"c:\images_gds";

            try
            {
                // Determine whether the directory exists.
                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }

            }
            catch (Exception e)
            {
                Console.WriteLine("The process failed: {0}", e.ToString());

            }

            ConnectClient();

        }

        /**
         * crear y conetra con el GDS una instancia de AsyncGDSClient
         */
        private void ConnectClient()
        {
            Reference<AsyncGDSClient> clientRef = new Reference<AsyncGDSClient>(null);
            listener = new TestListener(clientRef, this);
            AsyncGDSClient client = AsyncGDSClient.GetBuilder()
                .WithListener(listener)
                .WithTimeout(10000)
                .WithUserName(this.userNameGds)
                .WithURI(this.uriGds)
                .WithPingPongInterval(10000)
                .Build();
            clientRef.Value = client;
            client.Connect();

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

                this.Text = this.Text + " " + AplicationName + " V " + ProductVersionApp + " _ " + userName;


                //command = paramsCollection["command"];
                //if (string.IsNullOrEmpty(command))
                //{
                //    MessageBox.Show("No se encontró la accion requerida");
                //    this.Close();
                //}

                //obtener los puntos de detección y configurar el grid




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

            //obtener la version de la aplicacion
            this.GetVersion();

            //llenar el grid
            DataGridDei();




        }

        /**
         * obtenter la version de la aplicacion desde el servidor
         */
        public void GetVersion()
        {
            try
            {
                httpClient.GetAsync(endPoint + "/windows_app_api_version/" + AplicationName).ContinueWith(response =>
                {
                    try
                    {
                        var result = response.Result;
                        var content = result.Content.ReadAsStringAsync().Result;
                        if (result.StatusCode != HttpStatusCode.OK)
                        {
                            MessageBox.Show("Error al obtener la versión de la aplicación, no se pudo conectar con el servidor", "Error");
                            this.Close();
                        }

                        if (content == null || content == String.Empty)
                        {
                            MessageBox.Show("No se obtuvo la versión de la aplicación", "Error");
                            this.Close();
                        }

                        string get_version = content ?? "";


                        if (get_version != ProductVersionApp)
                        {
                            MessageBox.Show("La versión de la aplicación no coincide con la versión del servidor", "Error");
                            this.Invoke((MethodInvoker)delegate
                            {
                                this.Close();
                            });
                        }

                    }
                    catch (Exception ex)
                    {
                        this.Invoke((MethodInvoker)delegate
                        {
                            MessageBox.Show("Error al obtener la versión de la aplicación, no se pudo conectar con el servidor", "Error");
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


        public void DataGridDei()
        {
            DataGridViewDeis.AutoGenerateColumns = false;
            DataGridViewDeis.DataSource = bindingSourceDei;


            DataGridViewDeis.Columns.Add(ColumnDataGrid("ExternalId", "ExternalId", "Id del evento"));
            DataGridViewDeis.Columns.Add(ColumnDataGrid("LicensePlate", "LicensePlate", "Placa"));
            DataGridViewDeis.Columns.Add(ColumnDataGrid("DateStr", "DateStr", "Fecha"));
            DataGridViewDeis.Columns.Add(ColumnDataGrid("PanoramicPhotoName", "PanoramicPhotoName", "Foto Panoramica"));
            DataGridViewDeis.Columns.Add(ColumnDataGrid("DetailPhotoName", "DetailPhotoName", "Foto Detalle"));
            DataGridViewDeis.Columns.Add(ColumnDataGrid("MaxSpeed", "MaxSpeed", "Velocidad Máxima"));
            DataGridViewDeis.Columns.Add(ColumnDataGrid("CapturedSpeed", "CapturedSpeed", "Velocidad Capturada"));
            DataGridViewDeis.Columns.Add(ColumnDataGrid("CameraId", "CameraId", "CameraId"));
            DataGridViewDeis.Columns.Add(ColumnDataGrid("Result", "Result", "Resultado"));

            //agregar las columnas al datagrid
            this.DataGridViewDeis.Columns.Add("id", "ID del evento");
            this.DataGridViewDeis.Columns.Add("plate", "Placa");
            this.DataGridViewDeis.Columns.Add("date", "Fecha");
            this.DataGridViewDeis.Columns.Add("max_speed", "Velocidad");
            this.DataGridViewDeis.Columns.Add("camera", "Camara");
            this.DataGridViewDeis.Columns.Add("resultado", "Resultado");
            this.DataGridViewDeis.Columns.Add("msg_id", "uuuid mensaje");
            this.DataGridViewDeis.Columns.Add("attachments", "Adjuntos");


        }

        public DataGridViewColumn ColumnDataGrid(string dataPropertyName, string name, string headerText)
        {
            DataGridViewColumn column = new DataGridViewTextBoxColumn();
            column.DataPropertyName = dataPropertyName;
            column.Name = name;
            column.HeaderText = headerText;
            column.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            return column;
        }


        public HttpClient HttpClientF()
        {
            var httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.TryAddWithoutValidation("Authorization", "Bearer " + accessToken);
            httpClient.DefaultRequestHeaders.TryAddWithoutValidation("Accept", "application/json");

            return httpClient;
        }

        private void buttonGetEvents_Click(object sender, EventArgs e)
        {
            int limit = 0;


            if (this.textBoxLimit.Text == null || this.textBoxLimit.Text == String.Empty)
            {
                limit = 10;
            }

            //sttart date
            if (this.dateTimeStartDate.Value == null)
            {
                MessageBox.Show("Debe seleccionar una fecha de inicio", "Error");
                return;
            }
            DateTime start_date_d = DateTimeExtensions.StartOfDay(this.dateTimeStartDate.Value);

            string start_date = DateTimeExtensions.ToUnixTimeMilliSeconds(start_date_d);


            //end date
            if (this.dateTimeEndDate.Value == null)
            {
                MessageBox.Show("Debe seleccionar una fecha de fin", "Error");
                return;
            }

            DateTime end_date_d = DateTimeExtensions.EndOfDay(this.dateTimeEndDate.Value);
            string end_date = DateTimeExtensions.ToUnixTimeMilliSeconds(end_date_d);

            //comparar las fechas
            if (start_date_d > end_date_d)
            {
                MessageBox.Show("La fecha de inicio no puede ser mayor a la fecha de fin", "Error");
                return;
            }


            string msg = this.listener.GetEvents(start_date, end_date, limit);

            this.labelReposnseBtnGetEvents.Text = msg;


        }

        private void buttonReconnectClient_Click(object sender, EventArgs e)
        {
            this.listener.DisconnectClient();
            ConnectClient();
        }


    }
}
