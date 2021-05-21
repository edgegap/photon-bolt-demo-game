using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System.Net;
using System;
using System.IO;

public class StagingController : MonoBehaviour
{
    private const string API_TOKEN = "token your_token_here_1234";
    private const string API_URL = "https://staging-api.edgegap.com/v1/deploy";

    public string StatusString = String.Empty;
    public int Status = 0;
    public bool RequestAvailable = true;

    public void RequestServer(){
        RequestAvailable = false;
        StatusString = "Sending request...";
        Status = 10;

        string json = @"{
            ""app_name"": ""demo-game"",
            ""version_name"": ""v1"",
            ""ip_list"": [
                ""162.254.107.63""
            ]
        }";

        try{
            // Create HTTP request
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(API_URL);
            
            request.Method = "POST";
            request.ContentType = "application/json";

            // Include API Token in headers
            // request.Headers.Add("Content-Type", "application/json");
            request.Headers.Add("Authorization", API_TOKEN);

            // Add JSON
            using (StreamWriter streamWriter = new StreamWriter(request.GetRequestStream())){
                streamWriter.Write(json);
                streamWriter.Flush();
            }

            StatusString = "Retrieving response...";
            Status = 20;

            // Get response
            request.GetResponse();
            
            //  // OR, save response into a variable (as a JSON string)
            //  // Get response
            //  HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            //  // Read response
            //  StreamReader reader = new StreamReader(response.GetResponseStream());
            //  string jsonResponse = reader.ReadToEnd();

            StatusString = "Requested server starting";
            Status = 100;
            Invoke("ResetStatus", 15);
        }catch(Exception e){
            Debug.Log("Request error : " + e.ToString());
            StatusString = "Couldn't request server";
            Status = -100;
            RequestAvailable = true;
        }
    }

    public void ResetStatus(){
        StatusString = String.Empty;
        Status = 0;
    }
}


