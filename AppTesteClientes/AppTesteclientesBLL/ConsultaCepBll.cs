
using Newtonsoft.Json.Linq;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppTesteClientesBLL
{
    public class ConsultaCepBll
    {
      /// <summary>
      /// Consumo de API, utilizando como client o componente RestSharp;
      /// </summary>
      /// <param name="cep"></param>
      /// <param name="jsonResult"></param>
      /// <returns></returns>
        public bool ConsultaCep(string cep, out string jsonResult)
        {
            jsonResult = string.Empty;
            var url = "https://viacep.com.br/ws/" + cep + "/json/";
            var client = new RestSharp.RestClient(url);
            var request = new RestRequest(url,Method.Get);
         
            RestResponse response =  client.Execute(request);
            var content = response.Content;
            if ((int)response.StatusCode == 200 && !content.Contains("erro"))
            {
                jsonResult = content;
                return true;
            }
            
            jsonResult = "Erro";

            return false;
        }


    }
}
