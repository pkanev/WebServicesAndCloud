using System;
using RestSharp;

namespace Q04DistCalcResClient
{
    class CalculatorClient
    {
        static void Main()
        {
            Console.Write("Please enter the X value of the starting point: ");
            int startX = int.Parse(Console.ReadLine());
            Console.Write("Please enter the Y value of the starting point: ");
            int startY = int.Parse(Console.ReadLine());
            Console.Write("Please enter the X value of the ending point: ");
            int endX = int.Parse(Console.ReadLine());
            Console.Write("Please enter the Y value of the ending point: ");
            int endY = int.Parse(Console.ReadLine());

            var client = new RestClient("http://localhost:1136/");

            string address = string.Format("api/points/distance?startX={0}&startY={1}&endX={2}&endY={3}",
                startX, startY, endX, endY);

            var request = new RestRequest(address, Method.GET);

            // execute the request
            RestResponse response = (RestResponse)client.Execute(request);
            var content = response.Content; // raw content as string
            Console.WriteLine(content);
        }
    }
}
