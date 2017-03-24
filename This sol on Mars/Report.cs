using System.Collections.Generic;

namespace This_sol_on_Mars
{
    public class Report
    {
        public string terrestrial_date { get; set; }
        public int sol { get; set; }
        public double ls { get; set; }
        public double min_temp { get; set; }
        public double min_temp_fahrenheit { get; set; }
        public double max_temp { get; set; }
        public double max_temp_fahrenheit { get; set; }
        public double pressure { get; set; }
        public string pressure_string { get; set; }
        public object abs_humidity { get; set; }
        public object wind_speed { get; set; }
        public string wind_direction { get; set; }
        public string atmo_opacity { get; set; }
        public string season { get; set; }
        public string sunrise { get; set; }
        public string sunset { get; set; }
    }

    public class RootObject
    {
        public Report report { get; set; }
        public int count { get; set; }
        public object next { get; set; }
        public string previous { get; set; }
        public List<Report> results { get; set; }
    }
}
