﻿using Newtonsoft.Json;

namespace TappUploadDei
{
    internal class Dei
    {
        //placa
        [JsonProperty("license_plate")]
        public string LicensePlate { get; }

        //fecha de la deteccion
        [JsonConverter(typeof(DateTimeConverter)), JsonProperty("date")]
        public DateTime Date { get; }

        //codigo de infraccion
        [JsonProperty("infraction_code")]
        public string InfractionCode { get; }

        //punto de deteccion
        [JsonProperty("detection_point_id")]
        public string PointId { get; set; }

        //video panoramico path
        [JsonConverter(typeof(FileNameConverter)), JsonProperty("panoramic_video")]
        public string PanoramicVideo { get; set; }

        //video detalle path
        [JsonConverter(typeof(FileNameConverter)), JsonProperty("detail_video")]
        public string DetailVideo { get; set; }

        //foto panoramica path
        [JsonConverter(typeof(ImageConverter)), JsonProperty("panoramic_photo_b64")]
        public string PanoramicPhoto { get; set; }

        //foto detalle path
        [JsonConverter(typeof(ImageConverter)), JsonProperty("detail_photo_b64")]
        public string DetailPhoto { get; set; }

        //velocidad maxima permitida
        [JsonProperty("max_speed")]
        public string MaxSpeed { get; set; }

        //velocidad capturada
        [JsonProperty("captured_speed")]
        public string CapturedSpeed { get; set; }

        // usar la velocidad del documento use_max_speed
        [JsonProperty("use_max_speed")]
        public bool UseMaxSpeed { get; set; }


        //resultado en la tabla
        [property: JsonIgnore]
        public string Result { get; set; }

        //estado de la deteccion
        [property: JsonIgnore]
        public int Status { get; set; }

        //fecha en formato string
        public string DateStr => Date.ToString("dd/MM/yyyy HH:mm");
        //nombre de la foto panoramica
        [JsonProperty("panoramic_photo_name")]
        public string PanoramicPhotoName => Path.GetFileName(PanoramicPhoto);

        //nombre de la foto detalle
        [JsonProperty("detail_photo_name")]
        public string DetailPhotoName => Path.GetFileName(DetailPhoto);

        //nombre del video panoramico
        [JsonProperty("panoramic_video_name")]
        public string PanoramicVideoName => Path.GetFileName(PanoramicVideo);
        //nombre del video detalle
        [JsonProperty("detail_video_name")]
        public string DetailVideoName => Path.GetFileName(DetailVideo);

        //comando de aplicacion
        [JsonProperty("command_application")]
        public string CommandApplication { get; set; }

        //data es un json con metadata de la deteccion
        [JsonProperty("data")]
        public string Data { get; set; }

        //document_upload_id
        [JsonProperty("document_upload_id")]
        public string DocumentUploadId { get; set; }

        //id externo
        [JsonProperty("external_id")]
        public string ExternalId { get; set; }

        //camara id
        [JsonProperty("camera_id")]
        public string CameraId { get; set; }


        //serial camara
        [JsonProperty("camera_serial")]
        public string CameraSerial { get; set; }

        //nombre de adjuntos
        [JsonProperty("attachments_str")]
        public string AttachmentsStr { get; set; }


        public Dei(
            string infractionCode,
             string date = "",
            string licensePlate = "",
            string formatDate = "",
            string pointId = "",
            string panoramicVideo = "",
            string detailVideo = "",
            string panoramicPhoto = "",
            string detailPhoto = "",
            string maxSpeed = "",
            string capturedSpeed = "",
            bool useMaxSpeed = false,
            string commandApplication = "",
            string documentUploadId = "",
            string data = "",
            string externalId = "",
            string cameraId = "",
            string cameraSerial = "",
            string attachmentsStr = ""

            )
        {
            LicensePlate = licensePlate;
            string _DateStr = date;
            string FormatDate = formatDate ?? "yyyyMMddHmmss";
            InfractionCode = infractionCode;
            PointId = pointId;
            PanoramicVideo = panoramicVideo;
            DetailVideo = detailVideo;
            PanoramicPhoto = panoramicPhoto;
            DetailPhoto = detailPhoto;
            MaxSpeed = maxSpeed;
            CapturedSpeed = capturedSpeed;
            UseMaxSpeed = useMaxSpeed;
            Result = "Pendiente";
            DocumentUploadId = documentUploadId;
            Data = data;

            Date = DateTime.ParseExact(_DateStr, FormatDate, null);

            Status = (int)TappUploadDei.Status.Pending;

            CommandApplication = commandApplication;

            ExternalId = externalId;
            CameraId = cameraId;
            CameraSerial = cameraSerial;

            AttachmentsStr = attachmentsStr;



        }

        public string json()
        {
            return JsonConvert.SerializeObject(this);
        }

    }

    public class ImageConverter : JsonConverter<string>
    {
        public override void WriteJson(JsonWriter writer, string value, JsonSerializer serializer)
        {
            if (!string.IsNullOrEmpty(value))
            {
                writer.WriteValue(Convert.ToBase64String(File.ReadAllBytes(value)));
            }
            else
            {
                writer.WriteValue("");

            }
        }

        public override string ReadJson(JsonReader reader, Type objectType, string existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            return "";
        }
    }

    public class DateTimeConverter : JsonConverter<DateTime>
    {
        private readonly string _format = "yyyy-MM-dd HH:mm:ss";

        public override void WriteJson(JsonWriter writer, DateTime value, JsonSerializer serializer)
        {
            writer.WriteValue(value.ToString(_format));
        }

        public override DateTime ReadJson(JsonReader reader, Type objectType, DateTime existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            return DateTime.ParseExact((string)reader.Value, _format, null);
        }
    }

    public class FileNameConverter : JsonConverter<string>
    {
        public override void WriteJson(JsonWriter writer, string value, JsonSerializer serializer)
        {
            writer.WriteValue(value.Substring(value.LastIndexOf("/") + 1).ToString());
        }

        public override string ReadJson(JsonReader reader, Type objectType, string existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            return reader.Value.ToString();
        }
    }


    //clase de los puntos de deteccion
    public class DetectionPoint
    {
        //id del punto
        [JsonProperty("id")]
        public string Id { get; set; }
        //nombre del punto
        [JsonProperty("name")]
        public string Name { get; set; }

        //nombre completo del punto
        [JsonProperty("full_name")]
        public string FullName { get; set; }

        //
        public DetectionPoint(
            string id,
            string name,
            string fullName
            )
        {
            Id = id;
            Name = name;
            FullName = fullName;
        }

    }
}
