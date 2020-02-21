using System;
using System.Net;

namespace FavoritMotors.Integration.RestApiAdapter
{
    public class ResponseData
    {
        /// <summary>Дата/время начала форматрования пераметров вызова метода удаленного сервиса</summary>
		public readonly DateTime StartTime;

        public ResponseData()
        {
            StartTime = DateTime.Now;
        }


        private HttpWebRequest _httpWebRequest;
        public HttpWebRequest HttpWebRequest
        {
            get { return _httpWebRequest; }

            internal set
            {
                _httpWebRequest = value;

                RequestMethodType = value.Method;
                Host = value.Host;
                ActionURL = value.Address.ToString();
            }
        }


        private HttpWebResponse _httpWebResponse;
        public HttpWebResponse HttpWebResponse
        {
            get
            {
                return _httpWebResponse;
            }

            internal set
            {
                _httpWebResponse = value;

                StatusCode = value.StatusCode;
                StatusName = value.StatusDescription;
                ResponseURL = value.ResponseUri.AbsoluteUri;
            }
        }
        

        /// <summary>Тип вызова матода (GET, POST, PUT.....)</summary>
        public string RequestMethodType { get; private set; }

        /// <summary>DNS имя хоста удаленного сервиса</summary>
        public string Host { get; private set; }

        /// <summary>Адрес вызываемого метода удаленного сервиса</summary>
        public string ActionURL { get; private set; }





        /// <summary>Объект входных параметров метода удаленного сервиса</summary>
        public object Request { get; internal set; }

        /// <summary>Request сериализованный в JSON для передачи удаленному сервису</summary>
        public string JsonRequest { get; internal set; }


        /// <summary>Объект десериализованный из JSON-ответа, полученного от удаленного сервиса</summary>
        public object Response { get; internal set; }

        /// <summary>JSON-ответ, полученный от удаленного сервиса</summary>
        public string JsonResponse { get; internal set; }



        /// <summary>Код результата выполнения метода (ответ от удаленного сервиса)</summary>
        public HttpStatusCode StatusCode { get; private set; }

        /// <summary>Описание кода результата выполнения</summary>
        public string StatusName { get; private set; }
        /// <summary>Адрес вызываемого метода удаленного сервиса (из ответа)</summary>
        public string ResponseURL { get; private set; }



        /// <summary>Объект содержащий десериализованную ошибку</summary>
        public object DerilizationErrorMessage { get; internal set; }

        /// <summary>Текст ошибки (Это может быть возвращенная логическая ошибка от удаленного сервиса)</summary>
        public string ErrorMessage { get; internal set; }

        public Exception ResponseException { get; internal set; }

        /// <summary>При возникновении исключения - тип исключения</summary>
        public string ExceptionTypeName { get { return ResponseException.GetType().ToString(); } }

        ///// <summary>При возникновении исключения - сообщение о ошибке</summary>
        public string ExceptionMessage { get { return ResponseException.Message; } }

        /// <summary>При возникновении исключения во время десериализации - дополнительное сообщение содержащее поле в котором произошла ошибка</summary>
        public string ExceptionHelpMessage { get { return ResponseException.HelpLink; } }




        /// <summary>Время сериализации в JSON Request объекта от начала выполнения метода FavoritMotors.Integration.RESTAdapter.RunREST</summary>
        public double JsonSerializeTime { get; internal set; }

        /// <summary>Время записи в поток JSON-Request объекта от начала выполнения метода FavoritMotors.Integration.RESTAdapter.RunREST</summary>
        public double RequestStreamWriteTime { get; internal set; }

        /// <summary>Время получения ответа от удаленного сервиса от начала выполнения метода FavoritMotors.Integration.RESTAdapter.RunREST</summary>
        public double GetResponseTime { get; internal set; }

        /// <summary>Время чтения из потока JSON-ответа от начала выполнения метода FavoritMotors.Integration.RESTAdapter.RunREST</summary>
        public double ResponseStreamReadTime { get; internal set; }

        /// <summary>Время десериализации JSON-ответа от начала выполнения метода FavoritMotors.Integration.RESTAdapter.RunREST</summary>
        public double JsonDeserializeTime { get; internal set; }

        /// <summary>Время начала обработки исключения от начала выполнения метода FavoritMotors.Integration.RESTAdapter.RunREST</summary>
        public double WebExceptionBeginTime { get; internal set; }

        /// <summary>Время окончатия обработки исключения от начала выполнения метода FavoritMotors.Integration.RESTAdapter.RunREST</summary>
        public double WebExceptionEndTime { get; internal set; }

        /// <summary>Время выполнения метода FavoritMotors.Integration.RESTAdapter.RunREST</summary>
        public double EndTime { get; internal set; }



        /// <summary>
        /// Флаг возникновения ошибки при выполнении метода FavoritMotors.Integration.RESTAdapter.RunREST
        /// </summary>
        public bool IsError
        {
            get
            {
                return (StatusCode != HttpStatusCode.OK && StatusCode != HttpStatusCode.Created) || ResponseException == null;
            }
        }
    }
}
