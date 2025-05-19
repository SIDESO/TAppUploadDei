using System.Diagnostics;
using System.Net;
using System.Text;
using System.Web;
using Newtonsoft.Json;

namespace TappUploadDei
{


    public partial class FormTruCam : Form, TappValidation
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
        private readonly string ProductVersionApp = "1.0.0";//Application.ProductVersion.Split("+")[0];
        private readonly string commandApplication = "TC";
        private readonly string AplicationName = "FilesTruCam";


        public FormTruCam()
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
         * Procesar lo archivos de la carpeta seleccionada
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

            string tempPath = Path.GetTempPath();

            // Nombre del directorio temporal único
            string tempDirectoryName = Path.Combine(tempPath, "imagenes_trucamtmp");

            // Verificar si el directorio temporal ya existe y eliminarlo
            if (Directory.Exists(tempDirectoryName))
            {
                Directory.Delete(tempDirectoryName, true);
            }

            // Crear el directorio temporal
            Directory.CreateDirectory(tempDirectoryName);

            // Verificar si se creó correctamente
            if (!Directory.Exists(tempDirectoryName))
            {
                MessageBox.Show("No se pudo crear el directorio temporal para el almacenamiento de los achivos", "Error");
                return;
            }

            //recorrer todos los archivos de la carpeta y subcarpetas

            string[] files = Directory.GetFiles(path, "*.*", SearchOption.AllDirectories);
            int totalFiles = 0;

            //barra de progreso
            this.progressBarFilesLoad.Visible = true;
            this.progressBarFilesLoad.Minimum = 1;
            this.progressBarFilesLoad.Maximum = files.Length;
            this.progressBarFilesLoad.Value = 1;
            this.progressBarFilesLoad.Step = 1;



            //leer recursivamente todos los archivos de la carpeta
            foreach (string file in files)
            {

                if (File.Exists(file))
                {
                    totalFiles++;
                    this.progressBarFilesLoad.PerformStep();

                    //crear un directorio dentro del temporal con el nombre del archivo sin la extension
                    string fileName = Path.GetFileName(file);
                    string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(file);
                    string extractPath = Path.Combine(tempDirectoryName, fileNameWithoutExtension);

                    //crear carpeta del archivo
                    Directory.CreateDirectory(extractPath);

                    Process process = new Process();

                    string appDir = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location) ?? "";


                    //"C:\Users\carlo\OneDrive\Escritorio\C29\Utilities_C29\Utilities_Trucam\tdw64net.exe" "C:\Users\carlo\OneDrive\Escritorio\C29\Muestra Trucam\1747149347_Nt200_0513_151547.jmx"

                    // Configura el proceso
                    process.StartInfo.FileName = appDir + "\\Utilities_C29\\Utilities_Trucam\\tdw64net.exe";  // Nombre del archivo ejecutable (ej: "cmd.exe")
                    process.StartInfo.Arguments = "\"" + file + "\"";      // Argumentos a pasar al proceso
                    process.StartInfo.UseShellExecute = false;   // Desactiva la shell para poder controlar el proceso
                    process.StartInfo.CreateNoWindow = true;      // No crea una ventana para el proceso
                    process.StartInfo.RedirectStandardOutput = true; // Redirige la salida del proceso
                    process.StartInfo.RedirectStandardError = true;  // Redirige los errores del proceso
                    process.StartInfo.WorkingDirectory = extractPath; // Establece el directorio de trabajo del proceso

                    // Inicia el proceso
                    process.Start();

                    // Lee la salida del proceso
                    string output = process.StandardOutput.ReadToEnd();

                    string error = process.StandardError.ReadToEnd();

                    process.WaitForExit(); // Espera a que el proceso termine

                    // Verificar si el proceso se completó correctamente
                    if (process.ExitCode == 0)
                    {

                        //buscar el archivo de texto
                        string pathFileTxt = Path.Combine(extractPath, "UserData.txt");
                        //buscar una imagen con el nombre MeasurementFrameT.bmp
                        string pathImage = Path.Combine(extractPath, "MeasurementFrameT.bmp");

                        //si alguno de los dos no existe continuar
                        if (!File.Exists(pathImage) || !File.Exists(pathFileTxt))
                        {
                            continue;
                        }

                        string fileNameImagePng = "trucam_" + fileNameWithoutExtension + ".jpg";

                        //convertir la imagen a png
                        string pathImagePng = Path.Combine(extractPath, fileNameImagePng);


                        if (File.Exists(pathImagePng))
                        {
                            File.Delete(pathImagePng);
                        }

                        //convertir la imagen a png
                        using (var image = Image.FromFile(pathImage))
                        {
                            image.Save(pathImagePng, System.Drawing.Imaging.ImageFormat.Jpeg);
                        }



                        //leer el archivo de texto y obtener los datos
                        string[] lines = File.ReadAllLines(pathFileTxt);

                        /*
                         * 
                         *  Clip Type = SPEED
                            Clip Number = 12710
                            System Mode = SPEED
                            Number Of Frames = 32
                            Measurement Frame = 23
                            Speed Limit = 50
                            Capture Speed = 52
                            Measured Speed = 54
                            Measured Distance = 120.1
                            Lower Speed Limit = 50
                            Lower Capture Speed = 52
                            Higher Speed Limit = 50
                            Higher Capture Speed = 52
                            Limit Used = Lower
                            Speed Units = km/h
                            Distance Units = m
                            Lane = 1
                            Operator Name = 10O
                            Operator ID = 100
                            Street Name = Avenida 10 Urbanozacion Montebello 2 - Ruta Nacional 5505
                            Street Code = SAST 4
                            Clip Date = 13/05/2025
                            Clip Time Code = 15:54:21
                            Last Aligned = 17/04/2024  12:17:27
                            Calibration Expires = N/A
                            Paid Data = 
                            Latitude = 7° 48' 12.43" N
                            Longitude = 72° 31' 12.88" W
                            Firmware Version = 4.7.69  100.200.1.19
                            Serial No = TC011735
                            Signature = 
                            Crosshair Position: (X, Y) = (640, 480)
                            Frame Size: (Width, Height) = (1280, 960)
                         */

                        Dictionary<string, string> dataTxt = new Dictionary<string, string>();

                        foreach (string line in lines)
                        {
                            //partir la linea por el =
                            string[] parts = line.Split('=');

                            //guardar en la lista
                            dataTxt.Add(parts[0].Trim(), parts[1].Trim());

                        }


                        string? dateTimeStr = "";
                        string? max_speed = "";
                        string? captured_speed = "";
                        string? pathDetailPhoto = pathImagePng ?? "";

                        if (dataTxt.ContainsKey("Clip Date") && dataTxt.ContainsKey("Clip Time Code"))
                        {
                            dateTimeStr = dataTxt["Clip Date"] + " " + dataTxt["Clip Time Code"];
                        }

                        {
                            max_speed = dataTxt["Speed Limit"].Trim();
                        }

                        if (dataTxt.ContainsKey("Capture Speed"))
                        {
                            captured_speed = dataTxt["Capture Speed"].Trim();
                        }

                        if (dateTimeStr == "" || max_speed == "" || captured_speed == "" || pathDetailPhoto == "" | fileNameImagePng == "")
                        {

                            continue;
                        }


                        string stringLines = string.Join(",", lines);


                        string FormatDate = "dd/MM/yyyy HH:mm:ss";

                        Dei d = new Dei(
                            licensePlate: "",
                            date: dateTimeStr,
                            formatDate: FormatDate,
                            infractionCode: "C29",
                            pointId: "",
                            panoramicVideo: "",
                            detailVideo: "",
                            panoramicPhoto: "",
                            detailPhoto: pathDetailPhoto,
                            maxSpeed: max_speed,
                            capturedSpeed: captured_speed,
                            useMaxSpeed: true,
                            commandApplication: this.commandApplication,
                            documentUploadId: "",
                            data: stringLines
                            );

                        //agregar el objeto al binding source
                        bindingSourceDei.Add(d);


                    }
                    else
                    {

                        continue;
                    }



                }

            }


            this.labelTotalFiles.Text = totalFiles.ToString();

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
                this.Invoke((MethodInvoker)delegate
                {
                    BtnResendDeis();

                });
            }
            else
            {
                this.Invoke((MethodInvoker)delegate
                {

                    BtnFinalizeDeis();
                });
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
                            string contentR = JsonConvert.ToString(contentResponse);

                            MessageBox.Show("No se pudo crear la agrupación de los archivos" + " " + contentR + " " + response.StatusCode, "Error");
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

        private void BtnFinalizeDeis()
        {
            this.buttonUploadData.Enabled = false;
            this.buttonUploadData.Text = "Carga Finalizada";
        }




    }
}
