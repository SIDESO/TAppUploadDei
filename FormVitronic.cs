using System.Net;
using System.Text;
using System.Web;
using System.Xml;
using Newtonsoft.Json;

namespace TappUploadDei
{


    public partial class FormMainVitronic : Form, TappValidation
    {

        string? url = null;
        string[] args;
        string? accessToken = null;
        string? endPoint = null;
        string? userName = null;
        private BindingSource bindingSourceDei = new BindingSource();
        int countUpload = 0;
        int countError = 0;
        private HttpClient httpClient = new HttpClient();
        private readonly string ProductVersionApp = Application.ProductVersion.Split("+")[0];
        private readonly string commandApplication = "VI";
        private readonly string AplicationName = "FilesVitronic";


        public FormMainVitronic()
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

        }


        /**
         * validate the parameters and boot the application
         */
        public void ValidateParamInitial()
        {

            try
            {
                DisableControls();

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
            GetVersion();

            //obtener los puntos de detección y configurar el grid
            GetAsyncPoints();
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


            DataGridViewDeis.Columns.Add(ColumnDataGrid("Result", "Result", "Resultado"));
            DataGridViewDeis.Columns.Add(ColumnDataGrid("DateStr", "DateStr", "Fecha"));
            DataGridViewDeis.Columns.Add(ColumnDataGrid("InfractionCode", "InfractionCode", "Código"));
            DataGridViewDeis.Columns.Add(ColumnDataGrid("PanoramicPhotoName", "PanoramicPhotoName", "Foto Panoramica"));
            DataGridViewDeis.Columns.Add(ColumnDataGrid("DetailPhotoName", "DetailPhotoName", "Foto Detalle"));
            DataGridViewDeis.Columns.Add(ColumnDataGrid("MaxSpeed", "MaxSpeed", "Velocidad Máxima"));
            DataGridViewDeis.Columns.Add(ColumnDataGrid("CapturedSpeed", "CapturedSpeed", "Velocidad Capturada"));


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


        /**
         * Obtener los puntos de detección desde el servidor
         */
        private void GetAsyncPoints()
        {

            httpClient.GetAsync(endPoint + "/windows_apps_api_dei/detection_points").ContinueWith(response =>
            {
                try
                {
                    var result = response.Result;
                    var content = result.Content.ReadAsStringAsync().Result;

                    if (result.StatusCode != HttpStatusCode.OK)
                    {
                        MessageBox.Show("Error al obtener los puntos de detección, no se pudo conectar con el servidor", "Error");
                        this.Close();
                    }

                    List<DetectionPoint>? points = JsonConvert.DeserializeObject<List<DetectionPoint>>(content);


                    if (points == null || points.Count == 0)
                    {
                        MessageBox.Show("No se obtuvieron puntos de detección", "Error");
                        this.Close();
                    }

                    this.Invoke((MethodInvoker)delegate
                    {
                        comboBoxPoints.DataSource = points;
                        comboBoxPoints.DisplayMember = "Name";
                        comboBoxPoints.ValueMember = "Id";
                        EnableControls();

                    });
                }
                catch (Exception ex)
                {

                    MessageBox.Show("Error al obtener los puntos de detección, no se pudo conectar con el servidor", "Error");

                    this.Invoke((MethodInvoker)delegate
                    {
                        this.Close();
                    });
                }
            });
        }

        /**
         * Process the XML file
        */
        private async void ProcessFilesFolder()
        {

            string path = this.textBoxPathFolder.Text;

            //vaciar el grid
            bindingSourceDei.Clear();

            if (path == null || path == String.Empty)
            {
                MessageBox.Show("No se ha seleccionado una carpeta", "Error");
                return;
            }

            if (!Directory.Exists(path))
            {
                MessageBox.Show("La carpeta seleccionada no existe", "Error");
                return;
            }

            //recorrer los archivos xml

            string[] files_xml = Directory.GetFiles(path, "*.xml");
            int totalFilesXml = 0;

            foreach (string file in files_xml)
            {

                totalFilesXml++;
                /* 

                    ?xml version="1.0" encoding="UTF-8"?>
                    <?xml-stylesheet type="text/xsl" href="TuffFile.xsl"?>
                    <tuff version="19" xsi:schemaLocation="TuffFile.xsd" xmlns="http://xsd.conversion.tuffviewer.poliscan.vitronic.com/TuffFileV19" xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance">
                      <session>
                        <index>2409040708</index>
                        <dateTime>2024-09-04T07:08:39.508-05:00</dateTime>
                        <systemName>PS-959995</systemName>
                        <case>
                          <index>104</index>
                          <dateTime>2024-09-04T11:34:06.927-05:00</dateTime>
                          <location>
                            <field index="0">CARTAGO</field>
                            <field index="1">RUTA 2506 ANDALUCIA / CERRITOS *</field>
                            <field index="2">PR 77 + 750</field>
                          </location>
                          <witness index="0">admin</witness>
                          <lane>3</lane>
                          <laneAlias>C3</laneAlias>
                          <vehicle>
                            <type>car</type>
                            <direction>receding</direction>
                            <index>5892</index>
                            <detectionRange>
                              <positionVeryFirstMeasurement time="2024-09-04T11:34:05.649-05:00" unit="m" x="21.81" y1="8.94" y2="9.11"/>
                              <positionVeryLastMeasurement time="2024-09-04T11:34:06.927-05:00" unit="m" x="49.84" y1="8.56" y2="8.81"/>
                            </detectionRange>
                          </vehicle>
                          <causeSpeeding>
                            <limit lane="3" vehicleType="car" unit="km/h">50</limit>
                            <limit lane="3" vehicleType="lorry" unit="km/h">50</limit>
                            <measuredSpeed unit="km/h">79</measuredSpeed>
                            <triggerSpeed lane="3" vehicleType="car" unit="km/h">71</triggerSpeed>
                            <specLowerLimit unit="km/h">10</specLowerLimit>
                            <specUpperLimit unit="km/h">320</specUpperLimit>
                            <measuringRange>
                              <positionFirstMeasurement time="2024-09-04T11:34:05.649-05:00" unit="m" x="21.81"/>
                              <positionLastMeasurement time="2024-09-04T11:34:06.927-05:00" unit="m" x="49.84"/>
                              <numberOfMeasurements>161</numberOfMeasurements>
                            </measuringRange>
                          </causeSpeeding>
                          <image index="1" targetDesc="r">
                            <fileName>PS-959995--2409040708_05892_0104_01-1_r.jpg</fileName>
                            <name>Cam2-mvXD1012-C50mm</name>
                            <dateTime>2024-09-04T11:34:06.680-05:00</dateTime>
                            <cameraIndex>2</cameraIndex>
                            <lens>C50mm</lens>
                            <overlay>
                              <line x0="1643" y0="1452" x1="1645" y1="1138"/>
                              <line x0="1645" y0="1138" x1="1841" y1="1138"/>
                              <line x0="1841" y0="1138" x1="1884" y1="1451"/>
                              <line x0="1884" y0="1451" x1="1643" y1="1452"/>
                              <line x0="1689" y0="1367" x1="1839" y1="1367"/>
                            </overlay>
                            <infoBarHeightTop>117</infoBarHeightTop>
                            <infoBarHeightBottom>117</infoBarHeightBottom>
                            <overlayArea>
                              <x0>1402</x0>
                              <y0>989</y0>
                              <x1>2125</x1>
                              <y1>1460</y1>
                            </overlayArea>
                            <vehiclePosition time="2024-09-04T11:34:06.680-05:00" unit="m" x="44.59" y1="8.40" y2="9.20"/>
                          </image>
                          <image index="1" targetDesc="lp_r">
                            <fileName>PS-959995--2409040708_05892_0104_01-1_lp_r.jpg</fileName>
                            <name>Cam2-mvXD1012-C50mm</name>
                            <dateTime>2024-09-04T11:34:06.680-05:00</dateTime>
                            <cameraIndex>2</cameraIndex>
                            <lens>C50mm</lens>
                            <vehiclePosition time="2024-09-04T11:34:06.680-05:00" unit="m" x="44.59" y1="8.40" y2="9.20"/>
                          </image>
                          <gaugingValidityEnd>2024-12-31T12:54:46.062-05:00</gaugingValidityEnd>
                          <systemSpeedMeasuringRange begin="20" end="50" unit="m"/>
                        </case>
                      </session>
                    </tuff>


                        */
                //

                string? dateTimeStr = "";
                string? panoramic_photo_name = "";
                string? detail_photo_name = "";
                string? max_speed = "";
                string? captured_speed = "";
                string? pathPanoramicPhoto = "";
                string? pathDetailPhoto = "";

                //leer el contenido
                XmlDocument xml_document = new XmlDocument();
                xml_document.Load(file);
                XmlNodeList nodes = xml_document.GetElementsByTagName("case");
                XmlNode? caseNode = nodes[0];


                if (caseNode != null)
                {
                    dateTimeStr = caseNode["dateTime"]?.InnerText;

                    //tomar los nodos de las imagenes
                    XmlNodeList nodeImages = xml_document.GetElementsByTagName("image");

                    if (nodeImages == null)
                    {
                        return;
                    }

                    foreach (XmlNode node in nodeImages)
                    {

                        string? targetDesc = node?.Attributes["targetDesc"]?.Value;

                        //sla foto panoramica debe contener targetDesc = r
                        if (targetDesc == "r")
                        {
                            panoramic_photo_name = node["fileName"]?.InnerText;
                        }
                        else if (targetDesc == "lp_r" || targetDesc == "va_r")
                        {
                            detail_photo_name = node["fileName"]?.InnerText;
                        }
                    }

                    if (panoramic_photo_name == null || detail_photo_name == null)
                    {
                        return;
                    }

                    //path con el archivo de la imagen
                    pathPanoramicPhoto = Path.Combine(path, panoramic_photo_name);
                    pathDetailPhoto = Path.Combine(path, detail_photo_name);

                    //tomar el nodo de la velocidad
                    string vehicleType = xml_document.GetElementsByTagName("vehicle")[0]["type"]?.InnerText;

                    if (vehicleType == null)
                    {
                        return;
                    }
                    XmlNode? causeSpeeding = null;

                    if (vehicleType == "car")
                    {
                        causeSpeeding = xml_document.GetElementsByTagName("causeSpeeding")[0] ?? null;
                    }
                    else if (vehicleType == "lorry")
                    {
                        causeSpeeding = xml_document.GetElementsByTagName("causeSpeeding")[1] ?? null;
                    }


                    if (causeSpeeding != null)
                    {

                        //toma el primer limite
                        max_speed = causeSpeeding["limit"]?.InnerText;

                        captured_speed = causeSpeeding["measuredSpeed"]?.InnerText;

                    }

                }

                //validar que existan las imagenes,la velocidad maxima y la velocidad capturada
                if (dateTimeStr == null || !File.Exists(pathPanoramicPhoto) || !File.Exists(pathDetailPhoto) || captured_speed == null || max_speed == null)
                {
                    return;
                }

                //crear el objeto dei

                string FormatDate = "yyyy-MM-ddTHH:mm:ss.fffzzz";

                Dei d = new Dei(
                    licensePlate: "",
                    date: dateTimeStr,
                    formatDate: FormatDate,
                    infractionCode: "C29",
                    pointId: "",
                    panoramicVideo: "",
                    detailVideo: "",
                    panoramicPhoto: pathPanoramicPhoto,
                    detailPhoto: pathDetailPhoto,
                    maxSpeed: max_speed,
                    capturedSpeed: captured_speed,
                    commandApplication: this.commandApplication,
                    documentUploadId: "",
                    data: ""
                    );

                //agregar el objeto al binding source
                bindingSourceDei.Add(d);

            }

            this.labelTotalFiles.Text = totalFilesXml.ToString();

            await StoreFileUploadDeiAsync();


        }


        private async void SaveDadaDei()
        {


            if (this.comboBoxPoints.SelectedItem == null)
            {
                MessageBox.Show("Debe seleccionar un punto", "Error");
                return;
            }

            DetectionPoint? point = comboBoxPoints.SelectedItem as DetectionPoint;

            if (point?.Id == null)
            {
                MessageBox.Show("La selección del punto es inválida", "Error");
                return;
            }


            //recorrer la lista de deis
            foreach (Dei d in bindingSourceDei)
            {
                try
                {
                    if (d.Status == (int)Status.Success || d.Status == (int)Status.Sending)
                    {
                        continue;
                    }


                    int deiIndex = bindingSourceDei.IndexOf(d);
                    d.PointId = point.Id;
                    d.Status = (int)Status.Sending;
                    d.Result = "Enviando...";
                    bindingSourceDei.ResetItem(deiIndex);


                    //esperar a que se envie el dei y espera a que se complete
                    await StoreDeiAsync(d, deiIndex).ConfigureAwait(false);






                }
                catch (Exception ex)
                {
                    d.Status = (int)Status.Error;
                    d.Result = "ERROR: " + ex.Message;
                }

            }

            var errors_count = bindingSourceDei.Cast<Dei>().Where(d => d.Status == (int)Status.Error).Count();
            if (errors_count > 0)
            {
                BtnResendDeis();
            }
        }

        /**
         * Enviar el dei al servidor 
         * @param Dei d
         * @param int deiIndex
         */
        private async Task StoreDeiAsync(Dei d, int deiIndex)
        {

            string url = endPoint + "/windows_apps_api_dei/dei_store";
            string json = d.json();

            StringContent content = new StringContent(json, Encoding.UTF8, "application/json");

            try
            {

                // Send a request asynchronously continue when complete
                using (HttpResponseMessage response = await httpClient.PostAsync(url, content))
                {
                    try
                    {
                        // Check for success or throw exception
                        string contentResponse = await response.Content.ReadAsStringAsync();

                        if (response.IsSuccessStatusCode)
                        {
                            var definition = new { mensaje = "", dei_id = "" };
                            var data = JsonConvert.DeserializeAnonymousType(contentResponse, definition);

                            this.Invoke((MethodInvoker)delegate
                            {
                                d.Result = data.dei_id;
                                d.Status = (int)Status.Success;

                                bindingSourceDei.ResetItem(deiIndex);

                                this.labelFilesLoad.Text = (++countUpload).ToString();

                            });
                        }
                        else if (response.StatusCode != HttpStatusCode.OK)
                        {

                            string data = JsonConvert.ToString(contentResponse);

                            this.Invoke((MethodInvoker)delegate
                            {
                                d.Result = "ERROR: " + response.StatusCode + " " + data;
                                bindingSourceDei.ResetItem(deiIndex);
                                this.labelFilesError.Text = (++countError).ToString();


                            });

                        }

                        else
                        {
                            this.Invoke((MethodInvoker)delegate
                            {
                                d.Result = "ERROR: " + response.StatusCode;
                                bindingSourceDei.ResetItem(deiIndex);
                                this.labelFilesError.Text = (++countError).ToString();

                            });
                        }
                    }
                    catch (Exception ex)
                    {
                        this.Invoke((MethodInvoker)delegate
                        {
                            d.Result = "ERROR" + ex.Message;
                            bindingSourceDei.ResetItem(deiIndex);
                            this.labelFilesError.Text = (++countError).ToString();

                        });
                    }
                }
            }
            catch (Exception ex)
            {
                this.Invoke((MethodInvoker)delegate
                {
                    d.Result = "ERROR :" + ex.Message;
                    bindingSourceDei.ResetItem(deiIndex);
                    this.labelFilesError.Text = (++countError).ToString();
                });

            }
        }


        /**
          * Crear un registro para asociar todos los dei de la misma carpeta
        */
        private async Task StoreFileUploadDeiAsync()
        {
            string path = this.textBoxPathFolder.Text;
            if (path == null || path == String.Empty)
            {
                MessageBox.Show("No se ha seleccionado una carpeta", "Error");
                return;
            }

            string url = endPoint + "/windows_apps_api_dei/file_upload_dei_store";

            //quitar todo lo que este antes de :
            string file_name = path.Substring(path.IndexOf(":\\") + 2).Replace("\\", "|");


            //colocar el path en forma q_3 ,total_rows

            //el commad aplication y el campo de el path
            string json = "{\"command_application\":\"" + this.commandApplication + "\",\"file_name\":\"" + file_name + "\",\"total_rows\":" + bindingSourceDei.Count + "}";

            StringContent content = new StringContent(json, Encoding.UTF8, "application/json");

            try
            {

                // Send a request asynchronously continue when complete
                using (HttpResponseMessage response = await httpClient.PostAsync(url, content))
                {
                    try
                    {
                        // Check for success or throw exception
                        string contentResponse = await response.Content.ReadAsStringAsync();

                        if (response.IsSuccessStatusCode)
                        {

                            this.Invoke((MethodInvoker)delegate
                            {
                                var definition = new { id = "" };
                                var data = JsonConvert.DeserializeAnonymousType(contentResponse, definition);

                                //actualizar todas las deis con el document_upload_id
                                foreach (Dei d in bindingSourceDei)
                                {
                                    d.DocumentUploadId = data.id;
                                }

                            });
                        }
                        else
                        {

                            MessageBox.Show("No se pudo crear la agrupación de los archivos", "Error");
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


                            MessageBox.Show("Error al crear la agrupación de los archivos " + ex.Message, "Error");
                            this.Invoke((MethodInvoker)delegate
                            {
                                this.Close();

                            });

                        });
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al crear la agrupación de los archivos " + ex.Message, "Error");
                this.Invoke((MethodInvoker)delegate
                {
                    this.Close();
                });
            }
        }

        private void buttonSearchFolder_Click(object sender, EventArgs e)
        {
            using (var fd = new FolderBrowserDialog())
            {
                if (fd.ShowDialog() == System.Windows.Forms.DialogResult.OK && !string.IsNullOrWhiteSpace(fd.SelectedPath))
                {
                    this.textBoxPathFolder.Text = fd.SelectedPath;
                }
            }
        }

        private void buttonUploadData_Click(object sender, EventArgs e)
        {
            if (bindingSourceDei.Count == 0)
            {
                MessageBox.Show("No hay datos para enviar", "Error");
                return;
            }

            DisableControls();

            this.buttonUploadData.Text = "Enviando...";

            SaveDadaDei();


        }


        private async void textBoxPathFolder_TextChanged(object sender, EventArgs e)
        {

            ProcessFilesFolder();
            EnableControls();
        }

        /**
         * funcion para desabilitar los botones
         */
        private void DisableControls()
        {
            this.buttonUploadData.Enabled = false;
            this.buttonSearchFolder.Enabled = false;
            this.textBoxPathFolder.Enabled = false;
            this.comboBoxPoints.Enabled = false;
        }

        /**
         * funcion para habilitar los botones
         */
        private void EnableControls()
        {
            this.buttonUploadData.Enabled = true;
            this.buttonSearchFolder.Enabled = true;
            this.textBoxPathFolder.Enabled = true;
            this.comboBoxPoints.Enabled = true;
        }

        private void BtnResendDeis()
        {
            this.buttonUploadData.Enabled = true;
            this.buttonUploadData.Text = "Reintentar Errores";
        }


    }
}
