using System;
using System.Net;
using System.IO;
using System.Text;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace Iminetsoft
{
        /// <summary>
        /// Simple and fast GoDaddy (godaddy.com) API
        /// You might need a developer key and secret code first! In this case, visit https://developer.godaddy.com/keys
        /// <description>Developed and maintained by Iminetsoft | Version: 0.1 | Date: 17.01.2021</description>
        /// </summary>
        public class Godaddy
        {
                public enum RecordTypes { A, Nameserver, CNAME, MX, TXT, SRV, AAA, CAA }
                public enum StatusCodes { OK=0, EmptyResponse = 1, RequestMalformed=400, AuthInvalid=401, Unauthorized = 403, NotFound=404, ProcessInvalid=422, ProcessTimeout=429, InternalError=500, GatewayTimeout=504 }

                public string Domain { get; set; } = String.Empty;
                public RecordTypes Type { get; set; } = RecordTypes.A;
                public string Name { get; set; } = String.Empty;
                public int Ttl { get; set; } = 3600;
                public int Port { get; set; } = 1;
                public int Weight { get; set; } = 0;
                public string Key { private get; set; } = String.Empty;
                public string Secret { private get; set; } = String.Empty;

                public string Data { get; set; } = String.Empty;
                public int Priority { get; set; } = 0;

                public string UserAgent { get; set; } = "Iminetsoft.Godaddy.Net";

                public Dictionary<string, string> ResponseHeaders { get; private set; } = new Dictionary<string, string>();
                public string ExceptionMessage { private set; get; } = String.Empty;
                public StatusCodes StatusCode = StatusCodes.EmptyResponse;

                public Godaddy(){ }
                public Godaddy(string domain, string name, RecordTypes type, int ttl, string key, string secret)
                {
                        Domain = (domain.Length > 0 ? domain.Trim() : Domain);
                        Name = (name.Length > 0 ? name.Trim() : Name);
                        Type = type;
                        Ttl = (ttl >= 10 && ttl <=3600 ? ttl : Ttl);
                        Key = (key.Length > 0 ? key.Trim() : Key);
                        Secret = (secret.Length > 0 ? secret.Trim() : Secret);
                }

                /// <summary>
                /// Gets the current data of the specified domain and DNS record
                /// </summary>
                /// <returns></returns>
                public bool GetDnsRecord()
                {
                        GetSetDnsRecord();
                        return (StatusCode == StatusCodes.OK ? true : false);        
                }

                /// <summary>
                /// Gets the current IP/hostname of the specified domain and DNS record, typed A
                /// </summary>
                /// <returns></returns>
                public string GetDnsRecordString()
                {
                        GetSetDnsRecord();
                        return (StatusCode == StatusCodes.OK ? Data : "");
                }

                /// <summary>
                /// Creates or updates the specified DNS record
                /// </summary>
                /// <returns></returns>
                public bool SetDnsRecord()
                {
                        GetSetDnsRecord(true);
                        return (StatusCode == StatusCodes.OK ? true : false);
                }

                private void GetSetDnsRecord(bool UpdateData = false)
                {
                        string HtmlContent = String.Empty;
                        string apiurl = $"https://api.godaddy.com/v1/domains/{Domain}/records/{Type.ToString()}/{Name}";
                       
                        ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
                        HttpWebRequest webreq = (HttpWebRequest)System.Net.WebRequest.Create(apiurl);
                        webreq.Headers.Add("Authorization", $"sso-key {Key}:{Secret}");
                        webreq.ContentType = "application/json";
                        webreq.Accept = "application/json";
                        webreq.UserAgent = UserAgent;

                        if (UpdateData == true && RecordToJson().Length > 0)
                        {
                                webreq.Method = "PUT";                                
                                webreq.ContentLength = RecordToJson().Length;
                                using (var writer = new StreamWriter(webreq.GetRequestStream()))
                                {
                                        writer.Write(RecordToJson());
                                }
                        } 
                        else webreq.Method = "GET";

                        try
                        {
                                HttpWebResponse WebResponse = (HttpWebResponse)webreq.GetResponse();
                                Stream responseStream = responseStream = WebResponse.GetResponseStream();

                                StreamReader Reader = new StreamReader(responseStream, Encoding.Default);
                                HtmlContent = Reader.ReadToEnd();

                                ResponseHeaders.Clear();
                                foreach (string headkey in WebResponse.Headers.AllKeys)
                                {                                        
                                        ResponseHeaders.Add(headkey, WebResponse.Headers[headkey]);
                                }                               

                                if (WebResponse.StatusCode == HttpStatusCode.OK && HtmlContent.Length > 0)
                                {
                                        dynamic jsondata = JsonConvert.DeserializeObject(HtmlContent);

                                        if (jsondata != null && jsondata.Count != null && jsondata.Count > 0 && jsondata[0] != null)
                                        {
                                                if (jsondata[0].data != null) Data = jsondata[0].data.ToString().Trim();
                                                if (jsondata[0].name != null) Name = jsondata[0].name.ToString().Trim();
                                                if (jsondata[0].ttl != null) Ttl = jsondata[0].ttl;
                                                if (jsondata[0].type != null) Type = (RecordTypes)System.Enum.Parse(typeof(RecordTypes), jsondata[0].type.ToString().Trim()); //jsondata[0].type.ToString().Trim();
                                                StatusCode = StatusCodes.OK;
                                        }
                                        else StatusCode = StatusCodes.EmptyResponse;
                                } 
                                else StatusCode = StatusCodes.EmptyResponse;

                                WebResponse.Close();
                                responseStream.Close();
                        }
                        catch (Exception e)
                        {
                                ExceptionMessage = e.Message;
                                StatusCode = StatusCodes.EmptyResponse;
                        }
                }

                /// <summary>
                /// Current record export in JSON format - this needs primarly to create/update your DNS record
                /// </summary>
                /// <returns></returns>
                public string RecordToJson()
                {
                        Dictionary<string, object> pushrecord = new Dictionary<string, object>()
                        {
                                { "data", Data },
                                { "name", Name },
                                { "ttl", Ttl },
                                { "type", Type.ToString() },
                                { "port", Port },
                                { "priority", Priority },
                                { "protocol", "string" },
                                { "service", "string" },
                                { "weight", Weight },
                        };
                        return JsonConvert.SerializeObject(new object[1] { pushrecord });
                }
        }      
}
