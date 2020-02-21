using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace FavoritMotors.Integration.RestApiAdapter
{
    public class Adapter
    {
        /// <summary>
        /// Вызов метода удаленного сервиса
        /// </summary>
        /// <typeparam name="TRequest">Тип контракта (.NET) входного параметра (Если нет - можно указать любой, обязательно - Nullable)</typeparam>
        /// <typeparam name="TResponse">Тип контракта (.NET) ожидаемого по результатам выполнения метода</typeparam>
        /// <param name="HttpWebRequest">Экземпляр HttpWebRequest созданный в вызывающем сервисе</param>
        /// <param name="Request">Контракт (.NET) входного параметра, если не используется - NULL</param>
        /// <returns>Объект ResponseData</returns>
        public ResponseData RunREST<TRequest, TResponse>(HttpWebRequest HttpWebRequest, TRequest Request)
        {
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();


            ResponseData result = new ResponseData()
            {
                HttpWebRequest = HttpWebRequest,
                Request = Request
            };


            try
            {
                if (Request != null)
                {
                    //Newtonsoft.Json.JsonSerializerSettings jsonSerializerSettings = new Newtonsoft.Json.JsonSerializerSettings()
                    //{
                    //	NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore
                    //};

                    result.JsonRequest = Newtonsoft.Json.JsonConvert.SerializeObject(Request);
                    result.JsonSerializeTime = stopwatch.Elapsed.TotalMilliseconds;


                    using (Stream requestStream = HttpWebRequest.GetRequestStream())
                    {
                        requestStream.Write(_enc.GetBytes(result.JsonRequest), 0, _enc.GetByteCount(result.JsonRequest));
                        result.RequestStreamWriteTime = stopwatch.Elapsed.TotalMilliseconds;
                    }
                }


                using (HttpWebResponse response = (HttpWebResponse)HttpWebRequest.GetResponse())
                {
                    result.HttpWebResponse = response;

                    using (var reader = new StreamReader(response.GetResponseStream()))
                    {
                        result.GetResponseTime = stopwatch.Elapsed.TotalMilliseconds;
                        result.JsonResponse = reader.ReadToEnd();
                        result.ResponseStreamReadTime = stopwatch.Elapsed.TotalMilliseconds;


                        if (response.StatusCode != HttpStatusCode.OK && response.StatusCode != HttpStatusCode.Created)
                        {
                            result.ErrorMessage = result.JsonResponse;
                        }
                        else
                        {
                            // При возникновении исключения NewtonSoft не выдает поле в котором происходит ошибка во время
                            // десериализации, поэтому реализован костыль, который бросает дополнительное сообщение,
                            // содержащее название такого поля. Результат хранится в ExceptionHelpMessage.

                            Newtonsoft.Json.JsonSerializer jsonSerializer = new Newtonsoft.Json.JsonSerializer();
                            jsonSerializer.Error += AddToError;

                            result.Response = jsonSerializer.Deserialize<TResponse>(
                                new Newtonsoft.Json.JsonTextReader(new StringReader(result.JsonResponse)));

                            //result.Response = (Newtonsoft.Json.JsonConvert.DeserializeObject<TResponse>(result.JsonResponse));
                            result.JsonDeserializeTime = stopwatch.Elapsed.TotalMilliseconds;
                        }
                    }

                    response.Close();
                }
            }
            catch (WebException ex)
            {
                if (ex.Response != null)
                {
                    result.WebExceptionBeginTime = stopwatch.Elapsed.TotalMilliseconds;
                    HttpWebResponse response = (HttpWebResponse)ex.Response;

                    result.HttpWebResponse = response;

                    using (var reader = new StreamReader(response.GetResponseStream()))
                    {
                        result.ErrorMessage = reader.ReadToEnd();
                    }
                }

                result.ResponseException = ex;

                result.WebExceptionEndTime = stopwatch.Elapsed.TotalMilliseconds;
            }
            catch (Exception ex)
            {
                result.ResponseException = ex;
            }
            
            result.EndTime = stopwatch.Elapsed.TotalMilliseconds;
            
            TryPrcessException(result);

            TryWriteLog(result);
                                 
            return result;
        }

        void AddToError(object sender, Newtonsoft.Json.Serialization.ErrorEventArgs e)
        {
            e.ErrorContext.Error.HelpLink = "Неудалось распарсить в поле: " + e.ErrorContext.Member;
            e.ErrorContext.Handled = false;
        }

        private void TryPrcessException(ResponseData result)
        {
            try
            {
                if (result.ResponseException != null && CustomExceptionHendler != null)
                {
                    CustomExceptionHendler.Process(result.ResponseException);
                }
            }
            catch
            {
            }
        }

        private void TryWriteLog(ResponseData result)
        {
            if (IsLog)
            {
                if (Logger == null)
                    Logger = new Logger(result, LogFileDir, LogFileName);

                if (!IsLogErrorOnly || result.IsError)
                {
                    Logger.WriteLog(result);
                }
            }
        }

       

        private static UTF8Encoding _enc = new UTF8Encoding();


        public ILogger Logger { get; set; }
        public string LogFileDir { get; set; }
        public string LogFileName { get; set; }
        public bool IsLogErrorOnly { get; set; }
        public bool IsLog { get { return !String.IsNullOrEmpty(LogFileDir) && !String.IsNullOrEmpty(LogFileName); } }
        public IExceptionHendler CustomExceptionHendler { get; set; }
    }
}
