using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FavoritMotors.Integration.RestApiAdapter
{
    public class Logger : ILogger
    {
        public Logger(ResponseData result, string logFileDir, string logFileName)
        {
            Result = result;
            LogFileDir = logFileDir;
            LogFileName = logFileName;
        }

        private ResponseData Result { get; set; }
        private string LogFileDir { get; set; }
        private string LogFileName { get; set; }

        /// <summary>
        /// Запись информации в LOG-файл
        /// </summary>
        /// <param name="ResponseData">Объект, содержащий результаты вызова метода удаленного сервиса/param>
        private void WriteLog(ResponseData ResponseData)
        {

            double previousTime = 0D;
            string timingFormat = "\t{0,10:0.0000}";

            string message = String.Format("\r\n\r\n______________________________________\r\n\r\n{0:dd.MM.yyyy} {0:HH:mm:ss.fff}   -   {1}", ResponseData.StartTime, ResponseData.ActionURL);



            if (ResponseData.Request != null)
            {
                message += String.Format("\r\nRequestType: {0}", ResponseData.Request.GetType().FullName);
            }

            if (ResponseData.Response != null)
            {
                message += String.Format("\r\nResponseType: {0}", ResponseData.Response.GetType().FullName);
            }


            if (!String.IsNullOrEmpty(ResponseData.JsonRequest))
            {
                message += String.Format("\r\n\r\n--- JsonRequest: ---\r\n{0}", ResponseData.JsonRequest);
            }

            if (!String.IsNullOrEmpty(ResponseData.JsonResponse))
            {
                message += String.Format("\r\n\r\n--- JsonResponse: ---\r\n{0}", ResponseData.JsonResponse);
            }


            if (!String.IsNullOrEmpty(ResponseData.ResponseURL))
            {
                message += String.Format("\r\n\r\nResponseURL: {0}", ResponseData.ResponseURL);
                message += String.Format("\r\nStatusCode: {0} [ {1} - {2} ]\r\n", ResponseData.StatusCode, (int)ResponseData.StatusCode, ResponseData.StatusName);
            }


            if (ResponseData.IsError)
            {
                message += "\r\n";

                if (!String.IsNullOrEmpty(ResponseData.ErrorMessage))
                {
                    message += String.Format("\r\n\r\nErrorMessage:\r\n{0}", ResponseData.ErrorMessage);
                }

                if (!String.IsNullOrEmpty(ResponseData.ExceptionTypeName))
                {
                    message += String.Format("\r\n\r\nExceptionTypeName:\r\n{0}", ResponseData.ExceptionTypeName.ToString());
                }

                if (!String.IsNullOrEmpty(ResponseData.ExceptionMessage))
                {
                    message += String.Format("\r\n\r\nExceptionMessage:\r\n{0}", ResponseData.ExceptionMessage);
                }
            }


            if (ResponseData.JsonSerializeTime > 0D)
            {
                message += String.Format("\r\nJsonSerializeTime:" + timingFormat, ResponseData.JsonSerializeTime - previousTime);
                previousTime = ResponseData.JsonSerializeTime;
            }

            if (ResponseData.RequestStreamWriteTime > 0D)
            {
                message += String.Format("\r\nRequestStreamWriteTime:" + timingFormat, ResponseData.RequestStreamWriteTime - previousTime);
                previousTime = ResponseData.RequestStreamWriteTime;
            }

            if (ResponseData.GetResponseTime > 0D)
            {
                message += String.Format("\r\nGetResponseTime:" + timingFormat, ResponseData.GetResponseTime - previousTime);
                previousTime = ResponseData.GetResponseTime;
            }

            if (ResponseData.ResponseStreamReadTime > 0D)
            {
                message += String.Format("\r\nResponseStreamReadTime:" + timingFormat, ResponseData.ResponseStreamReadTime - previousTime);
                previousTime = ResponseData.ResponseStreamReadTime;
            }

            if (ResponseData.JsonDeserializeTime > 0D)
            {
                message += String.Format("\r\nJsonDeserializeTime:" + timingFormat, ResponseData.JsonDeserializeTime - previousTime);
                previousTime = ResponseData.JsonDeserializeTime;
            }

            if (ResponseData.WebExceptionBeginTime > 0D)
            {
                message += String.Format("\r\nWebExceptionBeginTime: " + timingFormat, ResponseData.WebExceptionBeginTime - previousTime);
                previousTime = ResponseData.WebExceptionBeginTime;
            }

            if (ResponseData.WebExceptionEndTime > 0D)
            {
                message += String.Format("\r\nWebExceptionEndTime:" + timingFormat, ResponseData.WebExceptionEndTime - previousTime);
                previousTime = ResponseData.WebExceptionEndTime;
            }

            if (ResponseData.EndTime > 0D)
            {
                message += String.Format("\r\n\r\nFullTime:\t" + timingFormat, ResponseData.EndTime);
            }


            try
            {
                System.IO.File.AppendAllText
                    (
                        String.Format(@"{0}\{1}_{2:yyyy-MM-dd}.log", LogFileDir, LogFileName, DateTime.Now.Date), message
                    );
            }
            catch { }
        }
    }
}
