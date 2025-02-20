using System.Text;
using Gds.Messages;
using Gds.Messages.Data;
using Gds.Messages.Header;
using Gds.Utils;
using GDSExtractor;
using messages.Gds.Websocket;
using Newtonsoft.Json;

namespace TappUploadDei
{
    public class SdkGds
    {

        public static CountdownEvent countdown = new CountdownEvent(1);
        public class Reference<T>
        {
            public T Value { get; set; }
            public Reference(T val)
            {
                Value = val;
            }
        }

        public class TestListener(Reference<AsyncGDSClient> client, FormGds formGds) : IGDSMessageListener
        {
            private readonly Reference<AsyncGDSClient> client = client;

            private readonly string userName = formGds.userNameGds;

            private readonly string commandApplication = formGds.commandApplication;

            readonly FormGds formGds = formGds;

            public override void OnConnectionSuccess(MessageHeader header, ConnectionAckData data)
            {

                formGds.Invoke((MethodInvoker)delegate
                {
                    formGds.infoConection.Text = "Cliente conectado";

                });


            }

            public override void OnDisconnect()
            {
                formGds.Invoke((MethodInvoker)delegate
                {
                    formGds.infoConection.Text = "Cliente desconectado";

                });
            }

            public override void OnConnectionFailure(Either<Exception, KeyValuePair<MessageHeader, ConnectionAckData>> cause)
            {


                formGds.Invoke((MethodInvoker)delegate
                {
                    var message = string.Format("El cliente a fallado! Motivo: {0}", cause.ToString());
                    formGds.infoConection.Text = message;

                });


            }

            //Este método recibe la respuesta de cualquier query realizada
            public override void OnQueryRequestAck11(MessageHeader header, QueryRequestAckData data)
            {
                if (data.Status != StatusCode.OK)
                {
                    formGds.Invoke((MethodInvoker)delegate
                    {
                        var message = string.Format("Estado: {0}, Motivo: {1}", data.Status, data.Exception);
                        formGds.infoConection.Text = message;

                    });

                    return;
                }

                formGds.Invoke((MethodInvoker)delegate
                {
                    var message = string.Format("Se recibieron {0} eventos.", data.AckData.NumberOfHits);
                    formGds.infoConection.Text = message;

                });

                var dataRecords = data.AckData.Records;
                //var fields = data.AckData.FieldDescriptors;

                //enviar a procesar cada resultados
                foreach (var record in dataRecords)
                {

                    ProcessDataForListDei(record);
                }

                if (data.AckData.HasMorePage)
                {
                    client.Value.SendMessage(
                       MessageManager.GetHeader(userName, DataType.NextQueryPageRequest),
                       MessageManager.GetNextQueryPageRequest(data.AckData.QueryContextDescriptor, 10000L)
                   );
                }

            }

            //Este método no hace nada pero es necesario por la arqitectura del SDK y GDS
            public override void OnAttachmentRequestAck5(MessageHeader header, AttachmentRequestAckData data)
            {
                Console.WriteLine("Attachment request ack message received with '" + data.Status + "' status code");
                if (data.AckData != null)
                {
                    byte[] attachment = data.AckData.Result.Attachment;
                    if (attachment == null)
                    {
                        //if you requested the binary, the attachment will be sent as an 'attachment response' type message at a later time
                        //si su solicitud aun no esta lista, el adjunto se enviará como un mensaje de tipo 'respuesta de adjunto' en un momento posterior
                        //en el metodo OnAttachmentResponse6
                    }
                    else
                    {

                        string attachmentId = data.AckData.Result.AttachmentId;

                        string event_id = data.AckData.Result.OwnerIds[0];

                        WriteAttachment(attachment, attachmentId, event_id);
                    }




                }
            }

            /*
             * Este método recibe los datos de los adjuntos solicitados
             * 
             */
            public override void OnAttachmentResponse6(MessageHeader header, AttachmentResponseData data)
            {
                //Para saber a que adjunto corresponde la respuesta, se puede obtener el messageID del header
                //Este fue el que nosotros generamos al solicitar el adjunto

                byte[] attachment = data.Result.Attachment;

                //enviar adjunto a transito app
                if (attachment != null)
                {
                    WriteAttachment(attachment, data.Result.AttachmentId, data.Result.OwnerIds[0]);
                }

                //Una vez recibido el adjunto, debemos informar a GDS que lo recibimos correctamente
                //De lo contrario, GDS lo volverá a enviar indefinidamente
                client.Value.SendAttachmentResponseAck7(StatusCode.OK,
                        new AttachmentResponseAckTypeData(StatusCode.Created,
                            new AttachmentResponseAckResult(
                                data.Result.RequestIds,
                                data.Result.OwnerTable,
                                data.Result.AttachmentId
                            )
                        )
                    );

            }


            /**
             * método para procesar la información y enviarla a transito app
             * 
            */
            private void ProcessDataForListDei(List<object> record)
            {

                string id = "";
                string plate = "";
                string date = "";
                string captured_speed = "";
                string serial = "";
                string data = "";
                string resultado = "OK";
                string attachments_str = "";
                object[] attachments_ids = null;


                //los valores de los campos cambian de acuerdo al usuario
                if (userName == "developer")
                {
                    //developer
                    //id 51
                    //atachments 71
                    //camara 99
                    //speed 101- 105
                    //fecha 0 viene en formato timestamp
                    id = record[51]?.ToString();
                    plate = record[105]?.ToString();
                    date = record[0]?.ToString();
                    captured_speed = record[101]?.ToString();
                    serial = record[99]?.ToString();
                    attachments_str = JsonConvert.SerializeObject(record[71]);
                    attachments_ids = record[71] as object[];

                }

                if (userName == "admin")
                {
                    //adjuntos tambien parecen ser 3,tambien en la 91
                    //fecha 5
                    //id 71
                    //placa 98
                    //velocida  121
                    //camara 119

                    id = record[71].ToString();
                    plate = record[98]?.ToString();
                    date = record[5]?.ToString();
                    captured_speed = record[121]?.ToString();
                    serial = record[119]?.ToString();
                    attachments_str = JsonConvert.SerializeObject(record[91]);
                    attachments_ids = record[91] as object[];

                }

                //la fecha viene en formato timestamp
                DateTime dateTime = DateTimeOffset.FromUnixTimeMilliseconds(long.Parse(date)).DateTime;

                string dateTimeStr = dateTime.ToString("yyyy-MM-dd HH:mm:ss");

                string FormatDate = "yyyy-MM-dd HH:mm:ss";

                Dei dei = new Dei(
                       licensePlate: plate,
                       date: dateTimeStr,
                       formatDate: FormatDate,
                       infractionCode: "C29",
                       pointId: "",
                       panoramicVideo: "",
                       detailVideo: "",
                       panoramicPhoto: "",
                       detailPhoto: "",
                       capturedSpeed: captured_speed,
                       commandApplication: this.commandApplication,
                       documentUploadId: "",
                       data: "",
                       externalId: id,
                       cameraId: serial
                   );

                //agregar el objeto al binding source
                formGds.Invoke((MethodInvoker)delegate
                {
                    formGds.bindingSourceDei.Add(dei);
                });



                //crear la data que se va a enviar a transito app
                //Dei dei = new Dei
                //{
                //    event_id = id,
                //    license_plate = plate,
                //    max_speed = max_speed, //average_speed
                //    date = date,
                //    camera_serial = serial, //entry_device_id
                //    resultado = "OK",
                //    data = "",
                //    attachments_str = attachments_str,
                //    attachments_ids = attachments_ids

                //};

                //si existen adjuntos, obtenerlos de gds
                if (attachments_ids != null && attachments_ids.Length > 0)
                {
                    foreach (var attachment_id in attachments_ids)
                    {
                        if (attachment_id != null)
                        {
                            getAttachment(attachment_id.ToString(), dei.ExternalId);
                        }
                    }
                }


            }


            /**
             * obtenr adjuntos
             */
            public void getAttachment(string attachmentId, string eventId)
            {
                //enviar la consulta de los adjuntos
                string query = "SELECT * FROM \"multi_event-@attachment\" WHERE id='" + attachmentId + "' and ownerid='" + eventId + "' FOR UPDATE WAIT 86400";

                client.Value.SendAttachmentRequest4(query);

            }



            private async void createDeiTappAsync(Dei d, int deiIndex)
            {
                HttpClient httpClient = formGds.HttpClientF();

                string url = formGds.GetEndpoint() + "/windows_apps_api/dei_create";
                string json = d.json();
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                var response = await httpClient.PostAsync(url, content);
                var contentResponse = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                {
                    var definition = new { success = "", id = "" };
                    var data = JsonConvert.DeserializeAnonymousType(contentResponse, definition);

                    formGds.Invoke((MethodInvoker)delegate
                    {
                        DataGridViewRow row = formGds.DataGridViewDeis.Rows[deiIndex];
                        row.Cells[0].Value = data.id;
                        // d.resultado = data.id;

                    });
                }
                else
                {
                    formGds.Invoke((MethodInvoker)delegate
                    {
                        //  d.resultado = "ERROR";

                    });
                }
            }

            /**
             * escribir el adjunto en disco
             * 
             */
            private void WriteAttachment(byte[] attachment, string attachmentId, string event_id)
            {

                //descargar el archivo
                string path = Path.Combine("C:\\images_gds\\", event_id + "___" + attachmentId + ".jpg");


                File.WriteAllBytes(path, attachment);
            }

            /**
             * obtener eventos de gds al llamar a este metodo
             * @param start_date fecha de inicio
             * @param end_date fecha de fin
             * @param limit limite de eventos a obtener
             * 
             */
            public string GetEvents(string start_date, string end_date, int limit)
            {

                //si no existe el cliente, no hacer nada
                if (!client.Value.IsConnected)
                {
                    return "Cliente no conectado";
                }

                //limpiar el binding source
                formGds.Invoke((MethodInvoker)delegate
                {
                    formGds.bindingSourceDei.Clear();
                });


                //limpiar la tabla
                formGds.Invoke((MethodInvoker)delegate
                {
                    formGds.DataGridViewDeis.Rows.Clear();
                });

                //consultar tablas accesibles
                //SELECT * FROM "@gds.config.store.tables"
                //consultar estructura de tablas
                //"SELECT * FROM \"@gds.config.store.schema\" WHERE table='multi_event'"

                //consulta con limites de cantidad
                //SELECT * FROM multi_event LIMIT 10
                //consulta con limites de fecha
                //SELECT * FROM multi_event WHERE \"@timestamp\" = 1738100361000 LIMIT


                //convertir las fechas en timestamp

                //crear y realizar la consulta
                string query = "SELECT * FROM multi_event WHERE \"@timestamp\" >= " + start_date + " AND \"@timestamp\" <= " + end_date + " LIMIT " + limit;

                client.Value.SendQueryRequest10(query, ConsistencyType.NONE, 10000L);

                return "Consulta enviada correctamente";
            }

            /**
             * desconectar el cliente
            */
            public void DisconnectClient()
            {


                try
                {
                    if (client.Value.IsConnected)
                    {
                        client.Value.Close();
                    }
                }
                catch (Exception ex)
                {

                    formGds.Invoke((MethodInvoker)delegate
                    {
                        var message = string.Format("Error al desconectar el cliente " + ex.Message);
                        formGds.infoConection.Text = message;

                    });


                }


            }

        }





    }
}
