﻿using CooperationHullMainSite.Models.ActionNetworkAPI;
using Microsoft.Extensions.Options;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace CooperationHullMainSite.Services
{
    public class ActionNetworkCalls : IActionNetworkCalls
    {

        private readonly ILogger<ActionNetworkCalls> _logger;
        private ActionNetworkConfig config { get; set; }

        public ActionNetworkCalls(IOptions<ActionNetworkConfig> configuration,
                                    ILogger<ActionNetworkCalls> logger)
        {
            config = configuration.Value;
            _logger = logger;
        }

        public async Task<int> GetNumberSigned(string formName)
        {
            ActionNetworkFormData data = await GetFormData(formName);

            return data.total_submissions;
        }

        public async Task<bool> SubmitForm(string formName, object formData)
        {

            // docs - https://actionnetwork.org/docs/v2/record_submission_helper

            var data = new ActionNetworkPerson();
            data.family_name = "Roberts";
            data.given_name = "Jane";
            data.email_addresses.Add(new ActionNetworkEmail("jane@notmail.com"));
            data.phone_number.Add(new ActionNetworkPhone("01482 112233"));

            var formInfo = FindFormConfigInfo("test_form");
            string endpoint = $"forms/{formInfo.FormGUID}/submissions";

            var temp = await PutDataAPICall(endpoint, new ActionNetworkPersonSignupHelper(data));

            return temp;
        }


        private FormData FindFormConfigInfo(string formName)
        {
            return config.FormList.FirstOrDefault(x => x.FormName == formName);
        }

        private async Task<ActionNetworkFormData> GetFormData(string formName)
        {
            var formInfo = FindFormConfigInfo(formName);

            string endpoint = $"forms/{formInfo.FormGUID}";
            var result = await GetDataAPICall(endpoint, new object());
            var item = JsonSerializer.Deserialize<ActionNetworkFormData>(result);
            return item;
        }


        private  async Task<string> GetDataAPICall(string endpoint, object dataModel)
        {
            var data = "";

            using (HttpClient client = new HttpClient())
            {
                client.BaseAddress = new Uri(config.baseURL);
                client.DefaultRequestHeaders.Add("OSDI-API-Token", config.APIKey);

                HttpResponseMessage response = await client.GetAsync(endpoint);

               if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                 data = await response.Content.ReadAsStringAsync();
                }
               else {
                    _logger.LogError($"API Call to {endpoint} failed: {response.StatusCode.ToString()}");
                }

            }

            return data;
        }


        private async Task<bool> PutDataAPICall(string endpoint, object dataModel)
        {
            bool result = false;

            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, endpoint);


            JsonSerializerOptions options = new()
            {
                DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
            };

            //TODO - object serialization - ignore empty lists (steal https://stackoverflow.com/questions/71409542/how-to-ignore-empty-list-when-serializing-to-json) bottom answer extension
            //sort out date format serilization
            //sort out status field serializatoin (plain string rather than enum ?)
            // work out how to deal with custom form fields
            // work out why mobile number wasn't pulled in

            var jsonString = JsonSerializer.Serialize(dataModel, options);

            request.Content = new StringContent(jsonString,
                                                Encoding.UTF8,
                                                "application/json");

            using (HttpClient client = new HttpClient())
            {
                client.BaseAddress = new Uri(config.baseURL);
                client.DefaultRequestHeaders.Add("OSDI-API-Token", config.APIKey);

                HttpResponseMessage response = await client.SendAsync(request);

                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                  var  data = await response.Content.ReadAsStringAsync();
                    result = true;
                }
                else
                {
                    _logger.LogError($"API Call to {endpoint} failed: {response.StatusCode.ToString()}");
                }

            }

            return result;
        }



    }
}
