using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace MySQLLite
{
    class MySQLData
    {
        public Exception E;
        public ArrayList Columns;
        public ArrayList Datas;
    }

    class MySQL
    {
        static private string _URL = "http://127.0.0.1/php/MySQLTool.php";
        static private String push(string json_data)
        {
            var httpWebRequest = (HttpWebRequest)WebRequest.Create(_URL);
            httpWebRequest.Method = "POST";
            httpWebRequest.ContentType = "application/json";
            httpWebRequest.ContentLength = json_data.Length;

            using (var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
            {
                streamWriter.Write(json_data);
                streamWriter.Close();
            }

            var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();
            using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
            {
                var result = streamReader.ReadToEnd();
                return result;
            }
            return "";
        }

        static public ArrayList GetTables(String dbName)
        {
            //{ "debug":1, "mode":0, "dbname":"vivobaby"}
            ArrayList result = new ArrayList();

            IDictionary json = new Dictionary<string, Object>();
            json.Add("mode", "0");
            json.Add("dbname", dbName);
            string json_data = JsonConvert.SerializeObject(json);//存放序列後的文字
            String context = push(json_data);

            JArray jArray = JArray.Parse(context);
            foreach (String tableName in jArray)
            {
                result.Add(tableName);
            }
            
            return result;
        }

        static public MySQLData GetData(String dbName, String tableName, String SQL)
        {
            //{ "debug":1, "mode":"1", "sql":"SELECT * FROM devagingtestreport", "table":"devagingtestreport"}
            MySQLData result = new MySQLData();

            IDictionary json = new Dictionary<string, Object>();
            json.Add("mode", "1");
            json.Add("dbname", dbName);
            json.Add("table", tableName);
            json.Add("sql", SQL);
            string json_data = JsonConvert.SerializeObject(json);//存放序列後的文字
            String context = push(json_data);
            Debug.WriteLine(context);

            JObject jObject = null;
            String records = null;
            JArray columns = null;
            JArray datas = null;

            try
            {
                jObject = JObject.Parse(context);
                records = (String)jObject.GetValue("records");
                columns = (JArray)jObject.GetValue("columns");
                datas = (JArray)jObject.GetValue("data");
            }
            catch (Newtonsoft.Json.JsonReaderException e)
            {
                result.E = new Exception("Error:SQL Cmd");
                return result;
            }
            

            ArrayList colAry = new ArrayList();
            foreach (String item in columns)
            {
                int num = 0;
                if (colAry.Contains(item))
                {
                    num = 0;
                    while (true)
                    {
                        num++;
                        if (colAry.Contains(item + num) == false)
                        {
                            break;
                        }
                    }
                   
                }
                if (num == 0)
                {
                    colAry.Add(item);
                }
                else
                {
                    colAry.Add(item + num);
                }
                
            }

            ArrayList dataAry = new ArrayList();
            foreach (JArray row in datas)
            {
                Dictionary<String, String> dict = new Dictionary<String, String>();
                dataAry.Add(dict);
                for (int i=0; i<colAry.Count; i++)
                {
                    String key = (String)colAry[i];
                    String value = (String)row[i];
                    int num = 0;
                    if (dict.Keys.Contains(key))
                    {
                        num = 0;
                        while (true)
                        {
                            num++;
                            if (dict.Keys.Contains(key + num) == false)
                            {
                                break;
                            }
                        }
                    }

                    if (num == 0)
                    {
                        dict.Add(key, value);
                    }
                    else
                    {
                        dict.Add(key + num, value);                        
                    }
                   
                }
            }

           
            result.E = new Exception("");
            result.Columns = colAry;
            result.Datas = dataAry;

            return result;
        }

    }
}
