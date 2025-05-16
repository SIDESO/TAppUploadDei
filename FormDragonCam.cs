using System.Diagnostics;
using System.Net;
using System.Text;
using System.Web;
using Newtonsoft.Json;

namespace TappUploadDei
{


    public partial class FormDragonCam : Form, TappValidation
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
        private readonly string commandApplication = "DC";
        private readonly string AplicationName = "FilesDragonCam";


        public FormDragonCam()
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
            string tempDirectoryName = Path.Combine(tempPath, "imgs_dragon_cam");

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


            Process process = new Process();

            string basePath = AppDomain.CurrentDomain.BaseDirectory;

            //"C:\Users\carlo\OneDrive\Escritorio\C29\Utilities_C29\Utilities_Dragoncam\ImgExtract.exe" "C:\Users\carlo\OneDrive\Escritorio\C29\MuestraDragonCam" "C:\Users\carlo\OneDrive\Escritorio\C29\DesencriptadoCmdDrangonCam" "" """

            // Configura el proceso
            process.StartInfo.FileName = "C:\\SIDESO\\TappUploadDei\\Utilities\\Utilities_Dragoncam\\ImgExtract.exe";  // Nombre del archivo ejecutable (ej: "cmd.exe")
            process.StartInfo.Arguments = "\"" + path + "\"" + " \"" + tempDirectoryName + "\"" + " \"\" \"\"";      // Argumentos a pasar al proceso
            process.StartInfo.UseShellExecute = false;   // Desactiva la shell para poder controlar el proceso
            process.StartInfo.CreateNoWindow = true;      // No crea una ventana para el proceso
            process.StartInfo.RedirectStandardOutput = true; // Redirige la salida del proceso
            process.StartInfo.RedirectStandardError = true;  // Redirige los errores del proceso


            // Inicia el proceso
            process.Start();

            // salidaS del proceso
            string output = process.StandardOutput.ReadToEnd();
            string error = process.StandardError.ReadToEnd();

            process.WaitForExit(); // Espera a que el proceso termine

            // Verificar si el proceso se completó correctamente
            if (process.ExitCode == 0 || string.IsNullOrEmpty(error))
            {

                //el archivo de texto que contiene la informacion es el .txt

                string[] filesDatatxt = Directory.GetFiles(tempDirectoryName, "*.txt", SearchOption.AllDirectories);

                //validar si hay archivos .txt
                if (filesDatatxt.Length == 0)
                {
                    MessageBox.Show("No se encontraron archivo de datos en  " + tempDirectoryName, "Error");
                    return;
                }

                //las imagenes son los .jpg
                string[] fileImagesJpg = Directory.GetFiles(tempDirectoryName, "*.jpg", SearchOption.AllDirectories);

                //validar si hay archivos .jpg
                if (fileImagesJpg.Length == 0)
                {
                    MessageBox.Show("No se encontraron imágenes para cargar en " + tempDirectoryName, "Error");
                    return;
                }

                /*
                vio_file,vio_date,vio_time,location,loc_code,posted_speed,threshold_speed,vio_speed,vio_range,officer_id,officer_name,sys_id,laser_id,camera_id,gps_latitude,gps_longitude,gps_time,vio_image,boresight_x,boresight_y,boresight_width,boresight_height,cal_date,status
                SAST-12025051307384313244,MAY. 13 2025,07:38:43,Avenida 10 calle 52 Ruta Nacional 5505 - Comfanorte,SAST-1,30,32,40,159,D100,Operador,DCAM13244,13244,15183635,7.85860419,-72.50088173,,SAST-12025051307384313244.jpg,591,266,120,155,2025MAY.12,Accepted
                SAST-12025051308002413244,MAY. 13 2025,08:00:24,Avenida 10 calle 52 Ruta Nacional 5505 - Comfanorte,SAST-1,30,32,36,143,D100,Operador,DCAM13244,13244,15183635,7.85860348,-72.50088550,,SAST-12025051308002413244.jpg,591,266,120,155,2025MAY.12,Accepted
                */

                //organizar los datos del archivos .txt
                string fileData = filesDatatxt[0];
                string[] linesTxt = File.ReadAllLines(fileData);
                string[] headers = linesTxt[0].Split(",");
                string[] records = linesTxt.Skip(1).ToArray();
                List<Dictionary<string, string>> data_list = new List<Dictionary<string, string>>();
                foreach (string line in records)
                {
                    string[] record = line.Split(",");
                    //validar que el registro tenga la misma cantidad de campos que el encabezado
                    if (record.Length != headers.Length)
                    {
                        continue;
                    }

                    Dictionary<string, string> record_dic = new Dictionary<string, string>();
                    //asignar cada campo a su respectivo encabezado
                    for (int i = 0; i < headers.Length; i++)
                    {
                        record_dic.Add(headers[i], record[i]);
                    }

                    record_dic.Add("line", line);

                    //agregar el registro a la lista de registros
                    data_list.Add(record_dic);

                }




                //barra de progreso
                this.progressBarFilesLoad.Visible = true;
                this.progressBarFilesLoad.Minimum = 1;
                this.progressBarFilesLoad.Maximum = fileImagesJpg.Length;
                this.progressBarFilesLoad.Value = 1;
                this.progressBarFilesLoad.Step = 1;
                int totalFiles = 0;



                //leer los datos de la lista
                foreach (var data in data_list)
                {
                    /*
                     * 
                     *  KeyValuePair`2;KeyValuePair`2.Key;KeyValuePair`2.Value
                        [vio_file, SAST-12025051307384313244];vio_file;SAST-12025051307384313244
                        [vio_date, MAY. 13 2025];vio_date;"MAY. 13 2025"
                        [vio_time, 07:38:43];vio_time;07:38:43
                        [location, Avenida 10 calle 52 Ruta Nacional 5505 - Comfanorte];location;"Avenida 10 calle 52 Ruta Nacional 5505 - Comfanorte"
                        [loc_code, SAST-1];loc_code;SAST-1
                        [posted_speed, 30];posted_speed;30
                        [threshold_speed, 32];threshold_speed;32
                        [vio_speed, 40];vio_speed;40
                        [vio_range, 159];vio_range;159
                        [officer_id, D100];officer_id;D100
                        [officer_name, Operador];officer_name;Operador
                        [sys_id, DCAM13244];sys_id;DCAM13244
                        [laser_id, 13244];laser_id;13244
                        [camera_id, 15183635];camera_id;15183635
                        [gps_latitude, 7.85860419];gps_latitude;7.85860419
                        [gps_longitude, -72.50088173];gps_longitude;-72.50088173
                        [gps_time, ];gps_time;""
                        [vio_image, SAST-12025051307384313244.jpg];vio_image;SAST-12025051307384313244.jpg
                        [boresight_x, 591];boresight_x;591
                        [boresight_y, 266];boresight_y;266
                        [boresight_width, 120];boresight_width;120
                        [boresight_height, 155];boresight_height;155
                        [cal_date, 2025MAY.12];cal_date;2025MAY.12
                        [status, Accepted];status;Accepted

             
                    */
                    totalFiles++;
                    this.progressBarFilesLoad.PerformStep();

                    string fileImage = data["vio_image"];

                    //buscar en las imagenes jpg
                    string pathDetailPhoto = fileImagesJpg.Where(f => f.Contains(fileImage)).FirstOrDefault() ?? "";
                    if (pathDetailPhoto == "")
                    {
                        continue;
                    }
                    //feha y hora
                    string dateTimeStr = data["vio_date"].Replace(".", "").Trim() + " " + data["vio_time"].Trim();


                    string max_speed = data["posted_speed"];
                    string? captured_speed = data["vio_speed"];

                    if (dateTimeStr == "" || max_speed == "" || captured_speed == "" || pathDetailPhoto == "")
                    {
                        continue;
                    }

                    string FormatDate = "MMM dd yyyy HH:mm:ss";

                    string stringData = string.Join(",", data["line"]);

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
                                data: stringData
                                );

                    //agregar el objeto al binding source
                    bindingSourceDei.Add(d);


                }


                this.labelTotalFiles.Text = totalFiles.ToString();

                await StoreFileUploadDeiAsync();

            }
            else
            {
                //mensaje de error
                MessageBox.Show("Error al extraer los archivos de la carpeta " + path + " a " + tempDirectoryName + "out:" + output + " error: " + error, "Error");
                return;


            }



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
