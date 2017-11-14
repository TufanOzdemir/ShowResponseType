using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ShowResponseType
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private async void GOBtnClick(object sender, EventArgs e)
        {
            try
            {
                listBox1.Items.Clear();
                using (HttpClient client = new HttpClient())
                {
                    var responseModel = await client.GetAsync(textBox1.Text.ToString());
                    var jsonString = responseModel.Content.ReadAsStringAsync().Result;
                    JObject jsonObj = JObject.Parse(jsonString);
                    Dictionary<string, object> data = JsonConvert.DeserializeObject<Dictionary<string, object>>(jsonString);
                    ParseJSONObject(data, 0);
                }
            }
            catch (Exception ex)
            {
                listBox1.Items.Add("Hata" + ex.ToString());
            }
        }

        private void ParseJSONObject(Dictionary<string, object> data, int count)
        {
            string whiteSpaceCounter = "";
            for (int i = 0; i < count; i++)
            {
                whiteSpaceCounter += "\t";
            }
            foreach (var item in data)
            {
                listBox1.Items.Add(whiteSpaceCounter + item.Value.GetType().Name + "\t" + item.Key);
                if (item.Value.GetType() == typeof(JObject))
                {
                    JObject jsonObj = (JObject)item.Value;
                    Dictionary<string, object> dictObj = jsonObj.ToObject<Dictionary<string, object>>();
                    ParseJSONObject(dictObj, ++count);
                }
                else if (item.Value.GetType() == typeof(JArray))
                {
                    JArray jsonObj = (JArray)item.Value;
                    List<Dictionary<string, object>> dictionaryListObject = jsonObj.ToObject<List<Dictionary<string, object>>>();
                    int countPlus = ++count;
                    foreach (var dictObj in dictionaryListObject)
                    {
                        ParseJSONObject(dictObj, countPlus);
                    }
                }
            }
        }
    }
}
