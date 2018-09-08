using Newtonsoft.Json;

namespace ConsoleApp1
{
    class Info
    {
        [JsonProperty("startWeek")]
        public int StartWeek { get; set; }

        [JsonProperty("dayOfWeek")]
        public int DayOfWeek { get; set; }

        [JsonProperty("department")]
        public string Department { get; set; }

        [JsonProperty("description")]
        public string Description { get; set; }

        [JsonProperty("startSection")]
        public int StartSection { get; set; }

        [JsonProperty("totalSection")]
        public int TotalSection { get; set; }

        [JsonProperty("type")]
        public string Type { get; set; }

        public ClassRoom Room { get; set; }

        public override string ToString()
        {
            return $"课程: {Description}, 教室: {Room.Name}, 时间: 第 {StartWeek} 周星期 {DayOfWeek} 第 {StartSection} - {TotalSection + StartSection - 1} 节";
        }
    }
}
