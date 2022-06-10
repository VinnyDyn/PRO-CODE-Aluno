using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Runtime.Serialization.Json;
using System.Text;

namespace Plugins.Service
{
    public class MockAPIManager
    {
        public MockAPIManager()
        {
        }

        public HttpWebRequest CreateWebRequest(string url, string agent = null, string method = "GET")
        {
            var request = (HttpWebRequest)WebRequest.Create(url);
            request.Method = method;
            if (!string.IsNullOrEmpty(agent))
                request.UserAgent = agent;
            request.ContentType = "application/json";
            return request;
        }

        public object ExecuteRequest<T>(HttpWebRequest request, object body = null)
        {
            object return_;
            try
            {
                if (body != null && (request.Method == "POST" || request.Method == "PUT"))
                {
                    string json = string.Empty;
                    using (var memoryStream = new MemoryStream())
                    {
                        var serializer = new DataContractJsonSerializer(this.GetType());
                        serializer.WriteObject(memoryStream, this);
                        json = Encoding.UTF8.GetString(memoryStream.ToArray());
                        var bytes = Encoding.UTF8.GetBytes(json);
                        request.ContentLength = bytes.Length;

                        using (var stream = request.GetRequestStream())
                            stream.Write(bytes, 0, bytes.Length);
                    }
                }

                using (var response = request.GetResponse())
                {
                    using (var stream = response.GetResponseStream())
                    {
                        StreamReader reader = new StreamReader(stream);
                        object objResponse = reader.ReadToEnd();
                        using (var memoryStream = new MemoryStream(Encoding.Unicode.GetBytes(objResponse.ToString())))
                        {
                            if (typeof(T) == typeof(Dictionary<string, object>))
                            {
                                var settings = new DataContractJsonSerializerSettings { UseSimpleDictionaryFormat = true };
                                var dataContractJsonSerializer = new DataContractJsonSerializer(typeof(Dictionary<string, object>), settings);
                                return_ = dataContractJsonSerializer.ReadObject(memoryStream);
                            }
                            else
                            {
                                var dataContractJsonSerializer = new DataContractJsonSerializer(typeof(T));
                                return_ = dataContractJsonSerializer.ReadObject(memoryStream);
                            }
                        }
                    }
                }

                return return_;
            }
            catch (Exception e)
            {
                throw e;
            }
        }
    }
}
