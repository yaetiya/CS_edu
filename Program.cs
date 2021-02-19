using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;

namespace semLinqTask
{
    class Program
    {
        static void Main(string[] args)
        {
            //Нужно дополнить модель WeatherEvent, создать список этого типа List<>
            //И заполнить его, читая файл с данными построчно через StreamReader
            //Ссылка на файл https://www.kaggle.com/sobhanmoosavi/us-weather-events

            //Написать Linq-запросы, используя синтаксис методов расширений
            //и продублировать его, используя синтаксис запросов
            //(возможно с вкраплениями методов расширений, ибо иногда первого может быть недостаточно)

            //0. Linq - сколько различных городов есть в датасете.
            //1. Сколько записей за каждый из годов имеется в датасете.
            //Потом будут еще запросы


            //-1. Вывести количество зафиксированных природных явлений в Америке в 2018 году
            //0.Вывести количество штатов, количество городов в датасете
            //1.Вывести топ 3 самых дождливых города в 2019 году в порядке убывания количества дождей(вывести город и количество дождей)
            //2.Вывести данные самых долгих(топ - 1) снегопадов в Америке по годам(за каждый из годов) - с какого времени, по какое время, в каком городе

            List<WeatherEvent> weList = new List<WeatherEvent>();

            try
            {
                // Create an instance of StreamReader to read from a file.
                // The using statement also closes the StreamReader.
                using (StreamReader sr = new StreamReader("WeatherEvents_Jan2016-Dec2020.csv"))
                {
                    string line = sr.ReadLine();
                    // Read and display lines from the file until the end of
                    // the file is reached.


                    // См ласт строчку цикла
                    var i = 0;
                    while (((line = sr.ReadLine()) != null) && (i < 300000))
                    {
                        var dataList = line.Split(',').ToList();
                        weList.Add(new WeatherEvent()
                        {
                            EventId = dataList[0],
                            Type = WeatherEvent.GetEventType(dataList[1]),
                            Severity = WeatherEvent.GetSeverity(dataList[2]),
                            StartTime = DateTime.Parse(dataList[3]),
                            EndTime = DateTime.Parse(dataList[4]),
                            TimeZone = dataList[5],
                            AirportCode = dataList[6],
                            LocationLat = decimal.Parse(dataList[7], new CultureInfo("en-US")),
                            LocationLng = decimal.Parse(dataList[8], new CultureInfo("en-US")),
                            City = dataList[9],
                            Country = dataList[10],
                            ZipCode = dataList[11]
                        });
                        // i++;
                    }
                }
            }
            catch (Exception e)
            {
                // Let the user know what went wrong.
                Console.WriteLine("The file could not be read:");
                Console.WriteLine(e.Message);
            }

            var cityList = weList
                .Select(x => x.City)
                .Distinct().ToList();


            //0. Linq - сколько различных городов есть в датасете.
            Console.WriteLine("Сколько различных городов есть в датасете.");
            Console.WriteLine(cityList.Count());

            //1. Сколько записей за каждый из годов имеется в датасете.
            Console.WriteLine("Сколько записей за каждый из годов имеется в датасете.");
            cityList.ForEach(x =>
                Console.WriteLine($"{x} - {weList.Where(y => y.City == x).Count()}")
            );


            Console.WriteLine("Вывести количество зафиксированных природных явлений в Америке в 2018 году");
            Console.WriteLine(weList.Where(x => x.TimeZone.StartsWith("US") && x.StartTime.Year == 2018).Select(x => x.Type).Distinct().Count());

            Console.WriteLine("Вывести количество штатов, количество городов в датасете");
            Console.WriteLine(weList.Select(x => x.ZipCode).Distinct().Count());
            Console.WriteLine(cityList.Count());

            Console.WriteLine("Вывести топ 3 самых дождливых города в 2019 году в порядке убывания количества дождей(вывести город и количество дождей)");
            var t = weList.Where(x => (x.StartTime.Year == 2019) && (x.Type == WeatherEventType.Rain)).Select(x => x.City).ToList();
            t.Distinct()
                .OrderBy(x => -t.Where(y => y == x).Count())
                .Take(3).ToList()
                .ForEach(x => Console.WriteLine($"{x} - {t.Where(y => y == x).Count()}"));
            Console.WriteLine("Вывести данные самых долгих(топ - 1) снегопадов в Америке по годам(за каждый из годов) - с какого времени, по какое время, в каком городе");
            var d = weList.Where(x => x.Type == WeatherEventType.Snow)
                .OrderBy(x => -(x.EndTime - x.StartTime))
                .ToList();

            Enumerable.Range(2016, 2020).ToList().ForEach(x =>
            {
                d.Where(y => y.StartTime.Year == x)
                    .Take(1).ToList()
                    .ForEach(m => Console.WriteLine($"{x} | {m.City}: {m.StartTime} - {m.EndTime}"));
            });

        }
    }

    //Дополнить модеь, согласно данным из файла
    class WeatherEvent
    {
        public string EventId { get; set; }
        public WeatherEventType Type { get; set; }
        public Severity Severity { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public string TimeZone { get; set; }
        public string AirportCode { get; set; }
        public decimal LocationLat { get; set; }
        public decimal LocationLng { get; set; }
        public string City { get; set; }
        public string Country { get; set; }
        public string ZipCode { get; set; }
        public static Severity GetSeverity(string v)
        {
            return v switch
            {
                "Light" => Severity.Light,
                "Severe" => Severity.Severe,
                "Moderate" => Severity.Moderate,
                _ => Severity.Unknown,
            };
        }

        public static WeatherEventType GetEventType(string v)
        {
            return v switch
            {
                "Snow" => WeatherEventType.Snow,
                "Rain" => WeatherEventType.Rain,
                "Fog" => WeatherEventType.Fog,
                "Cold" => WeatherEventType.Cold,
                _ => WeatherEventType.Unknown,
            };
        }

    }

    //Дополнить перечисления
    enum WeatherEventType
    {
        Unknown,
        Snow,
        Fog,
        Rain,
        Cold,
    }

    enum Severity
    {
        Unknown,
        Light,
        Severe,
        Moderate
    }
}