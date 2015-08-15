using System;
using Q02DistanceCalculatorClient.DistanceService;
namespace Q02DistanceCalculatorClient
{
    class Calculator
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
            using (var calc = new DistanceCalculatorClient())
            {
                var result = calc.CalcDistance(
                    new Point() { X = startX, Y = startY },
                    new Point() { X = endX, Y = endY }
                    );
                Console.WriteLine("The distance between point A({0}, {1}) and point B({2}, {3}) is: {4:F2} units",
                    startX, startY, endX, endY, result);
            }
        }
    }
}
