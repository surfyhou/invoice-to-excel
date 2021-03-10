using System;
using System.IO;
using System.Text;
using System.Net;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;

namespace ocr
{
    class Program
    {

        static string Invoice_Ocr(byte[] img)
        {
            const String host = "https://ocrapi-invoice.taobao.com";
            const String path1 = "/ocrservice/invoice";
            const String method = "POST";
            const String appcode = "dc6db2b490bc4db3849eeb48e1dfb087";

            MemoryStream ms = new MemoryStream(img);   
            byte[] arr = new byte[ms.Length];
            ms.Position = 0;
            ms.Read(arr, 0, (int)ms.Length);
            ms.Close();

            String bodys = "{\"img\":\"}"+Convert.ToBase64String(arr)+"{\"}";
            String querys = "";
            String url = host + path1;
            HttpWebRequest httpRequest = null;
            HttpWebResponse httpResponse = null;

            if (0 < querys.Length)
            {
                url = url + "?" + querys;
            }

            if (host.Contains("https://"))
            {
                ServicePointManager.ServerCertificateValidationCallback = new RemoteCertificateValidationCallback(CheckValidationResult);
                httpRequest = (HttpWebRequest)WebRequest.CreateDefault(new Uri(url));
            }
            else
            {
                httpRequest = (HttpWebRequest)WebRequest.Create(url);
            }
            httpRequest.Method = method;
            httpRequest.Headers.Add("Authorization", "APPCODE " + appcode);
            httpRequest.ContentType = "application/json; charset=UTF-8";
            if (0 < bodys.Length)
            {
                byte[] data = Encoding.UTF8.GetBytes(bodys);
                using (Stream stream = httpRequest.GetRequestStream())
                {
                    stream.Write(data, 0, data.Length);
                }
            }
            try
            {
                httpResponse = (HttpWebResponse)httpRequest.GetResponse();
            }
            catch (WebException ex)
            {
                httpResponse = (HttpWebResponse)ex.Response;
            }
            Stream st = httpResponse.GetResponseStream();
            StreamReader reader = new StreamReader(st, Encoding.GetEncoding("utf-8"));
            String result = "";
            result = reader.ReadToEnd();
            static bool CheckValidationResult(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors errors)
            {
                return true;
            }
        return result;
        }
        static void Main(string[] args)
        {
            const int LENGTH_OF_VAT_INVOICE_NUMBER = 8;
            string[] invoice_imgs = Directory.GetFiles(@"E:/imgs/","*.*", SearchOption.AllDirectories);
            foreach (string invoice_img in invoice_imgs)
	        {
                byte[] img = File.ReadAllBytes(invoice_img);
                String result = Invoice_Ocr(img);
                String invoice_name= "";
                int i = result.IndexOf("发票号码");
                if(i>0){
                    invoice_name =  result.Substring(i + 7,LENGTH_OF_VAT_INVOICE_NUMBER);
                    }
                else{
                    invoice_name = "发票" + System.IO.Path.GetFileNameWithoutExtension(invoice_img) + "识别失败";
                }
                String file = invoice_name + ".json";
                File.WriteAllText(Path.Combine("E:/jsons/", file), result, Encoding.UTF8);
	        }
            
        }
    }
}