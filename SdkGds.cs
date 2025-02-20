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
                var fields_descriptors = data.AckData.FieldDescriptors;

                //enviar a procesar cada resultados
                foreach (var record in dataRecords)
                {

                    Dictionary<string, object> record_dic = new Dictionary<string, object>();

                    for (int i = 0; i < fields_descriptors.Count(); i++)
                    {

                        record_dic.Add(fields_descriptors[i].FieldName, record[i]);
                    }

                    ProcessDataForListDei(record_dic);
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
            private void ProcessDataForListDei(Dictionary<string, object> record)
            {
                /*
                 * 
                 KeyValuePair`2;KeyValuePair`2.Key;KeyValuePair`2.Value
[@timestamp, 1732977786000];@timestamp;1732977786000
[@to_valid, 9223372036854775807];@to_valid;9223372036854775807
[@ttl, 9223372036854775807];@ttl;9223372036854775807
[@@version, 2,3/2];@@version;2
[action_eval_state, ];action_eval_state;""
[action_eval_sub_states, ];action_eval_sub_states;""
[action_groups, ];action_groups;""
[action_rules, ];action_rules;""
[action_type, ];action_type;""
[actions, ];actions;""
[adr_bgcolor, ];adr_bgcolor;""
[adr_color, ];adr_color;""
[adr_confidence, ];adr_confidence;""
[adr_frame, ];adr_frame;""
[adr_text, ];adr_text;""
[adr_type, ];adr_type;""
[all_carriage_id, ];all_carriage_id;""
[anpr_bgcolor, ];anpr_bgcolor;""
[anpr_color, ];anpr_color;""
[average_speed, ];average_speed;""
[average_speed_benchmark, ];average_speed_benchmark;""
[average_speed_result_state, ];average_speed_result_state;""
[avg_calc_state, ];avg_calc_state;""
[avs_section_name, ];avs_section_name;""
[axles_hauler, ];axles_hauler;""
[axles_trailer, ];axles_trailer;""
[belt_confidence, ];belt_confidence;""
[belt_result, ];belt_result;""
[carriage_id, ];carriage_id;""
[category, 410];category;410
[country_long, ];country_long;""
[deleted_attachments, ];deleted_attachments;""
[description, ];description;""
[detector, ];detector;""
[device, speedcam];device;speedcam
[device_speed, ];device_speed;""
[direction, -1];direction;-1
[entry_device_id, ];entry_device_id;""
[entry_device_name, ];entry_device_name;""
[entry_location_id, ];entry_location_id;""
[entry_location_name, ];entry_location_name;""
[event_id_section_entry, ];event_id_section_entry;""
[event_type, ];event_type;""
[extra_data, ];extra_data;""
[extra_image, ];extra_image;""
[front_cut_image, ];front_cut_image;""
[front_plate_image, ];front_plate_image;""
[gds_user_hook_state, ];gds_user_hook_state;""
[geo_point, POINT (0.000000 0.000000)];geo_point;"POINT (0.000000 0.000000)"
[group_id, ];group_id;""
[height, ];height;""
[id, GDSI241130144306000_120ccae_205];id;GDSI241130144306000_120ccae_205
[images, ];images;""
[images_section_entry, ];images_section_entry;""
[lane_id, ];lane_id;""
[latitude, 0];latitude;0
[left_images, ];left_images;""
[length, ];length;""
[location, SIMUT SAS ];location;"SIMUT SAS "
[longitude, 0];longitude;0
[measure_point, S1-TEST];measure_point;S1-TEST
[mmr_category, ];mmr_category;""
[mmr_category_confidence, 0];mmr_category_confidence;0
[mmr_color, ];mmr_color;""
[mmr_color_confidence, 0];mmr_color_confidence;0
[mmr_make, ];mmr_make;""
[mmr_model, ];mmr_model;""
[mmr_model_confidence, 0];mmr_model_confidence;0
[mmr_submodel, ];mmr_submodel;""
[nationality, ];nationality;""
[ntp_status, ];ntp_status;""
[overview_image, System.Object[]];overview_image;System.Object[]
[overview_plate_image, ];overview_plate_image;""
[pax_compound, ];pax_compound;""
[pax_confidence, ];pax_confidence;""
[pax_front, ];pax_front;""
[pax_rear, ];pax_rear;""
[pax_total, ];pax_total;""
[plate, ];plate;""
[plate_confidence, ];plate_confidence;""
[plate_confidence_rear, ];plate_confidence_rear;""
[plate_frame, ];plate_frame;""
[rear_country_long, ];rear_country_long;""
[rear_cut_image, ];rear_cut_image;""
[rear_nationality, ];rear_nationality;""
[rear_plate, ];rear_plate;""
[rear_plate_image, ];rear_plate_image;""
[rear_state_long, ];rear_state_long;""
[rear_state_short, ];rear_state_short;""
[reason, ];reason;""
[relation_id, ];relation_id;""
[request_ids, ];request_ids;""
[right_images, ];right_images;""
[s1_description, test];s1_description;test
[s1_event_type, SEBK];s1_event_type;SEBK
[s1_location, test];s1_location;test
[s1_operator, admin];s1_operator;admin
[section_length, ];section_length;""
[section_tolerance, ];section_tolerance;""
[source, 120ccae];source;120ccae
[speed, 0];speed;0
[speed_limit_passenger, 50];speed_limit_passenger;50
[speed_limit_ratio, 0];speed_limit_ratio;0
[speed_limit_truck, 50];speed_limit_truck;50
[speed_penalty_limit_passenger, 50];speed_penalty_limit_passenger;50
[speed_penalty_limit_truck, 50];speed_penalty_limit_truck;50
[speeder, True];speeder;True
[state_long, ];state_long;""
[state_short, ];state_short;""
[store_timestamp, ];store_timestamp;""
[strip_image, ];strip_image;""
[target, ];target;""
[target_details, ];target_details;""
[target_failed, ];target_failed;""
[timestamp, 1732977786000];timestamp;1732977786000
[timestamp_section_entry, ];timestamp_section_entry;""
[timestamp_section_exit, ];timestamp_section_exit;""
[tolerance, ];tolerance;""
[train_direction, ];train_direction;""
[transaction_closed, ];transaction_closed;""
[transaction_id, ];transaction_id;""
[transition_time, ];transition_time;""
[type, Sebesség túllépés - kézi];type;"Sebesség túllépés - kézi"
[vehicle_type, HEAVY];vehicle_type;HEAVY
[video, ];video;""
[videos_section_entry, ];videos_section_entry;""
[width, ];width;""


                 
                 * */
                string id = record["id"]?.ToString() ?? "";
                string plate = record["plate"]?.ToString() ?? record["rear_plate"]?.ToString() ?? "";
                string date = record["@timestamp"]?.ToString() ?? "";
                string captured_speed = record["speed"]?.ToString() ?? "";
                string serial = record["source"]?.ToString() ?? "";
                string data = "";
                string attachments_str = JsonConvert.SerializeObject(record["overview_image"]);
                object[] attachments_ids = record["overview_image"] as object[];


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
                       panoramicPhoto: attachments_str,
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
                string query = "SELECT * FROM multi_event WHERE \"@timestamp\" >= " + start_date + " AND \"@timestamp\" <= " + end_date;

                if (limit != 0)
                {
                    query = query + " LIMIT " + limit;
                }

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
