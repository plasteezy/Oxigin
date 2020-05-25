using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Globalization;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using Newtonsoft.Json;
using NLog;
using Npgsql;
using PhoneNumbers;
using RabbitMQ.Client;
using RestSharp;
using RestSharp.Authenticators;
using RestSharp.Extensions.MonoHttp;
using ServiceStack;

namespace Global
{
    public class Utility
    {
        private static readonly Logger log = LogManager.GetCurrentClassLogger();

        public static void SendToSlack(string msg)
        {
            var api = ConfigurationManager.AppSettings["Slack::Bot::General"];

            var http = (HttpWebRequest)WebRequest.Create(new Uri(api));
            http.Accept = "application/json";
            http.ContentType = "application/json";
            http.Method = "POST";

            var encoding = new ASCIIEncoding();

            var json = JsonConvert.SerializeObject(new {text = msg});
            var bytes = encoding.GetBytes(json);

            var newStream = http.GetRequestStream();
            newStream.Write(bytes, 0, bytes.Length);
            newStream.Close();

            var response = http.GetResponse();

            var stream = response.GetResponseStream();
            if (stream == null) return;
            var sr = new StreamReader(stream);
            sr.ReadToEnd();
        }

        public static string GetCode(bool isAlphanumeric, int codeSize)
        {
            var maxSize = codeSize;
            var key = isAlphanumeric ? "ABCDEFGHJKLMNPQRTUVWXYZ23456789" : "0123456789";

            var chars = key.ToCharArray();
            var data = new byte[1];
            var crypto = new RNGCryptoServiceProvider();
            crypto.GetNonZeroBytes(data);
            data = new byte[maxSize];
            crypto.GetNonZeroBytes(data);
            var result = new StringBuilder(maxSize);
            foreach (var b in data)
            {
                result.Append(chars[b % (chars.Length - 1)]);
            }
            return result.ToString();
        }

        public static void SendEmail(string from, string to, string message, string subject, string fromName)
        {
            var client = new RestClient
            {
                BaseUrl = new Uri(ConfigurationManager.AppSettings["Mailgun::Base"]),
                Authenticator = new HttpBasicAuthenticator("api", ConfigurationManager.AppSettings["Mailgun::Key"])
            };

            var request = new RestRequest();
            request.AddParameter("domain", "bigmsgbox.com", ParameterType.UrlSegment);
            request.Resource = "{domain}/messages";
            request.AddParameter("from", $"{fromName} <{from}>");
            request.AddParameter("to", to);
            request.AddParameter("subject", subject);
            request.AddParameter("html", message);
            request.Method = Method.POST;

            client.Execute(request);
        }

        public static string GetSummary(string message)
        {
            message = message.Trim();
            var msgLength = message.Length;

            if (msgLength <= 25) return message;
            message = message.Substring(0, 25);
            message = message + "...";
            return message;
        }

        public static void StartupCheck()
        {
            log.Info("Validating config values...");
            foreach (var item in ConfigurationManager.AppSettings.Keys)
            {
                var key = item.ToString();
                var configValue = ConfigurationManager.AppSettings[key];

                if (string.IsNullOrEmpty(configValue))
                    log.Warn("Null value provided for config entry with key {0}", key);
                else
                    log.Info("Config value for {0} validated", key);
            }

            log.Info("Config values validation complete");
        }

        public static void RabbitEnqueue(object obj, string routingKey)
        {
            var exchange = ConfigurationManager.AppSettings["Rabbit::Exchange"];
            var host = ConfigurationManager.AppSettings["Rabbit::Host"];
            var user = ConfigurationManager.AppSettings["Rabbit::User"];
            var pwd = ConfigurationManager.AppSettings["Rabbit::Pwd"];

            var factory = new ConnectionFactory { HostName = host, UserName = user, Password = pwd };
            using (var connection = factory.CreateConnection())
            {
                using (var channel = connection.CreateModel())
                {
                    channel.ExchangeDeclare(exchange, "direct");

                    var context = Encoding.UTF8.GetBytes(obj.ToJson());

                    channel.BasicPublish(exchange, routingKey, null, context);
                }
            }
        }

        public static string FormatAsNumber(int number)
        {
            return number <= 0 ? "0" : $"{number:#,#}";
        }

        public static DataTable ConvertToDataTable<T>(IList<T> list)
        {
            var propertyDescriptorCollection = TypeDescriptor.GetProperties(typeof(T));
            var table = new DataTable();
            for (var i = 0; i < propertyDescriptorCollection.Count; i++)
            {
                var propertyDescriptor = propertyDescriptorCollection[i];
                var propType = propertyDescriptor.PropertyType;
                if (propType.IsGenericType && propType.GetGenericTypeDefinition() == typeof(Nullable<>))
                {
                    table.Columns.Add(propertyDescriptor.Name, Nullable.GetUnderlyingType(propType));
                }
                else
                {
                    table.Columns.Add(propertyDescriptor.Name, propType);
                }
            }

            var values = new object[propertyDescriptorCollection.Count];
            foreach (T listItem in list)
            {
                for (int i = 0; i < values.Length; i++)
                {
                    values[i] = propertyDescriptorCollection[i].GetValue(listItem);
                }
                table.Rows.Add(values);
            }
            return table;
        }
    }
}