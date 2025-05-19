//  History
//  05/23/2017    V1.00 - initial beta release
//
//  06/01/2017    V1.01 - renamed GetBaseImage... function
//                        to GetMeasurementFrame...
//                        added iMeasurementFrame field to data structure
//  07/12/2017    V1.02 - fixed Speed Limit/Capture Speed value bug
//                        added Curent Lane (Lane) information
//  09/05/2017    V1.03 - added two new functions:
//                        GetMeasurementFrameWithCrosshairAndTextW and
//                        GetMeasurementFrameWithCrosshairAndTextNet
//  10/18/2017    V1.04 - corrected local time display
//                        corrected processing non-standard jmx files
//  11/15/2017    V1.05 - not released
//
//  01/25/2018    V1.06 - corrected GetTextData function,
//                        no crosshair display
//  01/26/2018    V1.07 - restored LastAlign date
//
//  02/02/2018    V1.08 - corrected Last Aligned time calculations
//                        added Lower/Higher speed limits and capture speeds
//                        added indication of limits type used by instrument
//                        added GetTextW2 function supporting date format selection



using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Drawing;
using System.Text;


namespace JMXCSample
{
  //jmx file errors
  enum LTI_ERRORS
  {
    ERR_FILE_NOT_FOUND =                      1,
    ERR_CANNOT_READ_FILE_ID_HEADER =          2,
    ERR_CANNOT_READ_FILE_HEADER =             3,
    ERR_FILE_ID_HEADER_SIZE_DOES_NOT_MATCH =  4,
    ERR_INCORRECT_FILE_HEADER_TYPE =          5,
    ERR_CRC_CHECKSUM_DOES_NOT_MATCH =         6,
    ERR_CANNOT_READ_CLIP =                    7,
    ERR_CANNOT_FIND_FIRST_IMAGE_HEADER =      8,
    ERR_CANNOT_READ_FIRST_IMAGE_HEADER =      9,
    ERR_CANNOT_READ_FIRST_IMAGE =             10,
    ERR_CANNOT_FIND_STILL_IMAGE =             11,
    ERR_CANNOT_READ_STILL_IMAGE =             12,
    ERR_BAD_USER_DATA_FORMAT =                13,
    END_OF_FILE =                             14,
    ERR_WRONG_ENCRYPTION =                    15,
    ERR_BAD_CLIP_FRAME=                       16,
    ERR_BAD_STILL_IMAGE=                      17,
    ERR_CANNOT_OPEN_FILE =                    18,
    ERR_CANNOT_CREATE_FILE =                  19,
    ERR_WRONG_ENDIAN =                        20,
    ERR_BAD_FILE_FORMAT =                     21,
    ERR_CANNOT_ALLOCATE_MEMORY =              22,
    ERR_CANNOT_READ_FILE =                    23,
    ERR_TOO_MANY_FRAMES =                     24,
    ERR_ZERO_FRAMES =                         25
  }

  //crosshair types
  enum CROSSHAIR_TYPE
  {
    CROSSHAIR_BEAM_SIZE = 0,
    CROSSHAIR_CLASSIC = 1,
    NO_CROSSHAIR = 2
  }

  //video clip types
  enum CLIP_TYPE
  {
    DBC =     0x01,
    SPEED =   0x02,
    SURVEY =  0x04,
    VIDEO =   0x08
  }

  //submode
  enum SUBMODE
  {
      NULL = 0,
      SPEED = 1,
      DBC = 2,
      REAR_PLATE = 5,
      VIDEO = 6,
      UNMANNED = 7,
      MANNED = 8,
      ROADOFFSET = 9
  }

  enum SPEED_UNITS
  {
    MPH =    0,
    KMH =    1
  }

  enum DISTANCE_UNITS
  {
    FEET =  0,
    METERS = 1
  }

  enum TC_ENCRYPTION
  {
    PASSWORDS_NO = 0,
    PASSWORDS_YES = 1
  }

  enum DATE_FORMAT
  {
    DATE_MM_DD_YYYY = 1,
    DATE_DD_MM_YYYY = 2
  }

  enum LIMITS_TYPE
  {
      LOWER_LIMITS = 0,
      HIGHER_LIMITS = 4,
      MINIMUM_LIMITS = 5
  }

  [StructLayoutAttribute(LayoutKind.Sequential, CharSet=CharSet.Unicode)]
  public struct textdataW
  {
//movie clip type
    [MarshalAs(UnmanagedType.ByValTStr, SizeConst=16)] public String ClipType;

//movie clip data - clip #, frame rate and number of frames
    [MarshalAs(UnmanagedType.ByValTStr, SizeConst=12)] public String ClipNumber;
    public int iNumberOfFrames;
    public int iMeasurementFrame;

//speed limits and capture speed settings
//valid only if Dual Speed Mode is not active
    [MarshalAs(UnmanagedType.ByValTStr, SizeConst=8)] public String SpeedLimit;
    [MarshalAs(UnmanagedType.ByValTStr, SizeConst=8)] public String CaptureSpeed;

//speed and distance units
    [MarshalAs(UnmanagedType.ByValTStr, SizeConst=8)] public String SpeedUnits;
    [MarshalAs(UnmanagedType.ByValTStr, SizeConst=8)] public String DistanceUnits;

//violation data
    [MarshalAs(UnmanagedType.ByValTStr, SizeConst=8)] public String MeasuredSpeed;
    [MarshalAs(UnmanagedType.ByValTStr, SizeConst=8)] public String MeasuredDistance;
    [MarshalAs(UnmanagedType.ByValTStr, SizeConst=8)] public String MeasuredSpeed2;
    [MarshalAs(UnmanagedType.ByValTStr, SizeConst=8)] public String MeasuredDistance2;
    [MarshalAs(UnmanagedType.ByValTStr, SizeConst=8)] public String MeasuredTBC;
    [MarshalAs(UnmanagedType.ByValTStr, SizeConst=8)] public String MeasuredDBC;
    [MarshalAs(UnmanagedType.ByValTStr, SizeConst=8)] public String MeasuredTBM;
    [MarshalAs(UnmanagedType.ByValTStr, SizeConst=8)] public String MeasuredRoadOffset;
    public int iCurrentLane;
//
    [MarshalAs(UnmanagedType.ByValTStr, SizeConst=80)] public String OperatorName;
    [MarshalAs(UnmanagedType.ByValTStr, SizeConst=40)] public String OperatorID;
    [MarshalAs(UnmanagedType.ByValTStr, SizeConst=80)] public String StreetName;
    [MarshalAs(UnmanagedType.ByValTStr, SizeConst=16)] public String StreetCode;
    [MarshalAs(UnmanagedType.ByValTStr, SizeConst=12)] public String ClipDate;
    [MarshalAs(UnmanagedType.ByValTStr, SizeConst=12)] public String ClipTime;
    [MarshalAs(UnmanagedType.ByValTStr, SizeConst=24)] public String LastAligned;
    [MarshalAs(UnmanagedType.ByValTStr, SizeConst=24)] public String CalExpires;
    [MarshalAs(UnmanagedType.ByValTStr, SizeConst=60)] public String PaidData;

//GPS and Tilt data
    [MarshalAs(UnmanagedType.ByValTStr, SizeConst=24)] public String Latitude;
    [MarshalAs(UnmanagedType.ByValTStr, SizeConst=24)] public String Longitude;

//Misc
    [MarshalAs(UnmanagedType.ByValTStr, SizeConst=24)] public String FirmwareVersion;
    [MarshalAs(UnmanagedType.ByValTStr, SizeConst=12)] public String SerialNo;
    [MarshalAs(UnmanagedType.ByValTStr, SizeConst=30)] public String Signature;
    [MarshalAs(UnmanagedType.ByValTStr, SizeConst=16)] public String SystemMode;

//Movie Clip Additional Data
    public int iCrosshairX;
    public int iCrosshairY;
    public int iImageWidth;
    public int iImageHeight;

//speed limits and capture speed settings
//valid only if Dual Speed Mode is active
    [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 8)] public String LowerSpeedLimit;
    [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 8)] public String HigherSpeedLimit;
    [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 8)] public String LowerCaptureSpeed;
    [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 8)] public String HigherCaptureSpeed;
    [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 8)] public String LimitsUsed;
  };


  [StructLayout(LayoutKind.Sequential)]
  public struct BITMAPINFOHEADER
  {
    [MarshalAs(UnmanagedType.I4)] public Int32 biSize ;
    [MarshalAs(UnmanagedType.I4)] public Int32 biWidth ;
    [MarshalAs(UnmanagedType.I4)] public Int32 biHeight ;
    [MarshalAs(UnmanagedType.I2)] public short biPlanes;
    [MarshalAs(UnmanagedType.I2)] public short biBitCount ;
    [MarshalAs(UnmanagedType.I4)] public Int32 biCompression;
    [MarshalAs(UnmanagedType.I4)] public Int32 biSizeImage;
    [MarshalAs(UnmanagedType.I4)] public Int32 biXPelsPerMeter;
    [MarshalAs(UnmanagedType.I4)] public Int32 biYPelsPerMeter;
    [MarshalAs(UnmanagedType.I4)] public Int32 biClrUsed;
    [MarshalAs(UnmanagedType.I4)] public Int32 biClrImportant;
  }

  [StructLayout(LayoutKind.Sequential)] public struct BITMAPINFO
  {
    [MarshalAs(UnmanagedType.Struct, SizeConst=40)] public BITMAPINFOHEADER bmiHeader;
    [MarshalAs(UnmanagedType.ByValArray, SizeConst=1024)] public Int32[] bmiColors;
  }

  [StructLayout(LayoutKind.Sequential, Pack = 1)]
  public unsafe struct BITMAPFILEHEADER
  {
    public Int16 bfType;
    public Int32 bfSize;
    public Int16 bfReserved1;
    public Int16 bfReserved2;
    public Int32 bfOffBits;
  };

  public class Win32Api
  {
    [DllImport("user.dll", CharSet=CharSet.Auto)]
          public static extern IntPtr GetDC(IntPtr hWnd);
    [DllImport("user.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall)]
          public static extern IntPtr GetWindowDC(IntPtr hWND);
    [DllImport("gdi.dll")]
          public static extern int GetDIBits(IntPtr hdc, IntPtr hbmp, uint uStartScan,
                uint cScanLines, byte [] lpvBits, ref BITMAPINFO lpbmi, uint uUsage);
  };

  public unsafe class Lti
  {
    [DllImport("ltijmx64.dll", CharSet = CharSet.Unicode)]
    public static extern int GetTextDataW(String FileName, ref textdataW userdata);

    [DllImport("ltijmx64.dll", CharSet = CharSet.Unicode)]
    public static extern int GetTextDataW2(String FileName, ref textdataW userdata, int dataformat);

    [DllImport("ltijmx64.dll", CharSet = CharSet.Unicode)]
    public static extern int GetMeasurementFrameWithCrosshairNet(String FileName, int CrosshairType, ref BITMAPINFO bmiImage, byte[] imgbuf, String FrameTimeStamp);

    [DllImport("ltijmx64.dll", CharSet = CharSet.Unicode)]
    public static extern int GetMeasurementFrameWithCrosshairAndTextNet(String FileName, int CrosshairType, ref BITMAPINFO bmiImage, byte[] imgbuf, String FrameTimeStamp);

    [DllImport("ltijmx64.dll", CharSet = CharSet.Unicode)]
    public static extern int GetClipFrameNet(String FileName, int FrameCounter, int CrosshairType, ref BITMAPINFO bmiImage, byte[] imgbuf, String FrameTimeStamp);

    [DllImport("ltijmx64.dll", CharSet = CharSet.Unicode)]
    public static extern int ConvertNamesToTrucamFormatNet(String PCFileName, String TrucamFileName, int Encryption);

    [DllImport("ltijmx64.dll", CharSet = CharSet.Unicode)]
    public static extern int ConvertNamesFromTrucamFormatNet(String TrucamFileName, String PCFileName, int Encryption);

    [DllImport("ltijmx64.dll", CharSet = CharSet.Unicode)]
    public static extern int ConvertLocationsToTrucamFormatNet(String PCFileName, String TrucamFileName);

    [DllImport("ltijmx64.dll", CharSet = CharSet.Unicode)]
    public static extern int ConvertLocationsFromTrucamFormatNet(String TrucamFileName, String PCFileName);
  };

  class SampleCode
  {
    const int RGBQUAD_SIZE = 4;
    const int MAX_STILL_IMAGE_WIDTH = 1280;
    const int MAX_STILL_IMAGE_HEIGHT = 960;

    [STAThread]
    static int Main(string[] args)
    {
      int res;
      int CrosshairType;
      String ClipName;
      byte[] imgbuf;

      if (args.Length == 0)
      {
        Console.WriteLine("Usage: tdw64net filename.jmx");
        return 1;
      }

      if (args.Length > 2)
      {
        Console.WriteLine("Filename with embedded spaces has to be enclosed in quots");
        return 1;
      }
      else if (args.Length == 1)
        ClipName = String.Copy(args[0]);
      else
        ClipName = String.Copy("");

      //allocate memory for jmx variables
      textdataW ClipData = new textdataW();

        //get text data
      res = Lti.GetTextDataW(ClipName, ref ClipData);
      if (res == 0)
          SaveTextData("UserData.txt", ClipData);
      else if (res == (int)LTI_ERRORS.ERR_WRONG_ENCRYPTION)
      {
          Console.WriteLine("Wrong Encryption");
          return 1;
      }
      else
      {
          Console.WriteLine("Something wrong");
          return 1;
      }

        res = Lti.GetTextDataW2(ClipName, ref ClipData, (int)DATE_FORMAT.DATE_DD_MM_YYYY);
      if (res == 0)
          SaveTextData("UserData2.txt", ClipData);
      else if (res == (int)LTI_ERRORS.ERR_WRONG_ENCRYPTION)
      {
          Console.WriteLine("Wrong Encryption");
          return 1;
      }
      else
      {
          Console.WriteLine("Something wrong");
          return 1;
      }

      imgbuf = new byte[MAX_STILL_IMAGE_WIDTH * MAX_STILL_IMAGE_HEIGHT * 3];
      BITMAPINFO bmiImage = new BITMAPINFO();

      String FrameTimeStamp = new String(' ', 12);
      String FileName = new String(' ', 260);

      CrosshairType = (int)CROSSHAIR_TYPE.CROSSHAIR_BEAM_SIZE;

      //get still image with crosshair
      res = Lti.GetMeasurementFrameWithCrosshairNet(ClipName, CrosshairType, ref bmiImage, imgbuf, FrameTimeStamp);
      if (res == 0)
        SaveBmpImage("MeasurementFrame.bmp", bmiImage, imgbuf);
      else
      {
        Console.WriteLine("Could not extract frame");
        return 1;
      }

      //get still image with crosshair and text
      res = Lti.GetMeasurementFrameWithCrosshairAndTextNet(ClipName, CrosshairType, ref bmiImage, imgbuf, FrameTimeStamp);
      if (res == 0)
          SaveBmpImage("MeasurementFrameT.bmp", bmiImage, imgbuf);
      else
      {
          Console.WriteLine("Could not extract frame");
          return 1;
      }

      if (ClipData.iNumberOfFrames > 1)
      {
        FileStream fs = new FileStream("FrameTimeStanps.txt", FileMode.Create, FileAccess.Write, FileShare.None);
        StreamWriter sw = new System.IO.StreamWriter(fs, System.Text.Encoding.Unicode);

        //movie clip data - clip #, frame rate and number of frames
        int FrameCounter = 0;
        while (true)
        {
          res = Lti.GetClipFrameNet(ClipName, FrameCounter, CrosshairType, ref bmiImage, imgbuf, FrameTimeStamp);
          FileName = String.Format("ClipFrame{0:000}.bmp", FrameCounter+1);
          if (res == 0)
          {
            SaveBmpImage(FileName, bmiImage, imgbuf);
            sw.Write("Frame {0:000} = {1}\r\n", FrameCounter+1, FrameTimeStamp);
          }
          else
            break;

          FrameCounter++;
          if (FrameCounter >= ClipData.iNumberOfFrames)
            break;
        }
        sw.Close();
        sw.Dispose();
        fs.Dispose();
      }

      return res;
    }

    static void SaveTextData(String filename, textdataW userdata)
    {
      FileStream fs = new FileStream(filename, FileMode.Create, FileAccess.Write, FileShare.None);
      StreamWriter sw = new System.IO.StreamWriter(fs, System.Text.Encoding.Unicode);

      //movie clip data - clip #, frame rate and number of frames
      sw.Write("Clip Type = {0}\r\n", userdata.ClipType);
      sw.Write("Clip Number = {0}\r\n", userdata.ClipNumber);
      sw.Write("System Mode = {0}\r\n", userdata.SystemMode);
      sw.Write("Number Of Frames = {0}\r\n", userdata.iNumberOfFrames);
      sw.Write("Measurement Frame = {0}\r\n", userdata.iMeasurementFrame);

      sw.Write("Speed Limit = {0}\r\n", userdata.SpeedLimit);
      sw.Write("Capture Speed = {0}\r\n", userdata.CaptureSpeed);

      //violation data
      sw.Write("Measured Speed = {0}\r\n", userdata.MeasuredSpeed);
      sw.Write("Measured Distance = {0}\r\n", userdata.MeasuredDistance);

      sw.Write("Lower Speed Limit = {0}\r\n", userdata.LowerSpeedLimit);
      sw.Write("Lower Capture Speed = {0}\r\n", userdata.LowerCaptureSpeed);

      sw.Write("Higher Speed Limit = {0}\r\n", userdata.HigherSpeedLimit);
      sw.Write("Higher Capture Speed = {0}\r\n", userdata.HigherCaptureSpeed);

      sw.Write("Limit Used = {0}\r\n", userdata.LimitsUsed);

      //speed and distance units
      sw.Write("Speed Units = {0}\r\n", userdata.SpeedUnits);
      sw.Write("Distance Units = {0}\r\n", userdata.DistanceUnits);

      sw.Write("Lane = {0}\r\n", userdata.iCurrentLane);

      if (userdata.ClipType == "DBC")
      {
        sw.Write("Measured Speed2 = {0}\r\n", userdata.MeasuredSpeed2);
        sw.Write("Measured Distance2 = {0}\r\n", userdata.MeasuredDistance2);
        sw.Write("Measured TBC = {0}\r\n", userdata.MeasuredTBC);
        sw.Write("Measured DBC = {0}\r\n", userdata.MeasuredDBC);
        sw.Write("Measured TBM = {0}\r\n", userdata.MeasuredTBM);
        sw.Write("Measured Road Offset = {0}\r\n", userdata.MeasuredRoadOffset);
      }
//
      sw.Write("Operator Name = {0}\r\n", userdata.OperatorName);
      sw.Write("Operator ID = {0}\r\n", userdata.OperatorID);
      sw.Write("Street Name = {0}\r\n", userdata.StreetName);
      sw.Write("Street Code = {0}\r\n", userdata.StreetCode);

      sw.Write("Clip Date = {0}\r\n", userdata.ClipDate);
      sw.Write("Clip Time Code = {0}\r\n", userdata.ClipTime);
      sw.Write("Last Aligned = {0}\r\n", userdata.LastAligned);
      sw.Write("Calibration Expires = {0}\r\n", userdata.CalExpires);

      sw.Write("Paid Data = {0}\r\n", userdata.PaidData);

      sw.Write("Latitude = {0}\r\n", userdata.Latitude);
      sw.Write("Longitude = {0}\r\n", userdata.Longitude);

      //Misc
      sw.Write("Firmware Version = {0}\r\n", userdata.FirmwareVersion);
      sw.Write("Serial No = {0}\r\n", userdata.SerialNo);

      sw.Write("Signature = {0}\r\n", userdata.Signature);

      //Movie Clip Additional Data
      sw.Write("Crosshair Position: (X, Y) = ({0}, {1})\r\n", userdata.iCrosshairX, userdata.iCrosshairY);
      sw.Write("Frame Size: (Width, Height) = ({0}, {1})\r\n", userdata.iImageWidth, userdata.iImageHeight);

      sw.Close();
      sw.Dispose();
      fs.Dispose();
    }
 
    static void SaveBmpImage(String filename, BITMAPINFO bmiImage, byte[] imgbuf)
    {
      BITMAPFILEHEADER hdr = new BITMAPFILEHEADER();

      hdr.bfType = 0x4d42;        // 0x42 = "B" 0x4d = "M"
      hdr.bfSize = (Int32)Marshal.SizeOf(hdr) +
                 bmiImage.bmiHeader.biSize + bmiImage.bmiHeader.biClrUsed * RGBQUAD_SIZE + bmiImage.bmiHeader.biSizeImage;
      hdr.bfReserved1 = 0;
      hdr.bfReserved2 = 0;
      hdr.bfOffBits = (Int32)Marshal.SizeOf(hdr) + bmiImage.bmiHeader.biSize + bmiImage.bmiHeader.biClrUsed * RGBQUAD_SIZE;

      FileStream fs = File.Create(filename);

      byte[] buffer = new byte[1280*960*3];
      GCHandle h = GCHandle.Alloc(buffer, GCHandleType.Pinned);

      // copy the file header into int byte[] mem alloc
      Marshal.StructureToPtr(hdr, h.AddrOfPinnedObject(), false);
      fs.Write(buffer, 0, Marshal.SizeOf(hdr));
      // copy the image header into int byte[] mem alloc
      Marshal.StructureToPtr(bmiImage, h.AddrOfPinnedObject(), true);
      fs.Write(buffer, 0, Marshal.SizeOf(bmiImage.bmiHeader) + bmiImage.bmiHeader.biClrUsed * 256);

      fs.Write(imgbuf, 0, bmiImage.bmiHeader.biSizeImage);
      fs.Close();

      h.Free();
      fs.Dispose();
    }
  }
}
