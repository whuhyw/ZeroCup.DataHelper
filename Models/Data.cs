using System;
using System.Collections.Generic;
using System.Text;

namespace ZeroCup.DataHelper.Models
{
    public class ScenicSpot
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Zen { get; set; }
        public SpotDetails Details { get; set; }
    }

    public class SpotDetails
    {
        public string Summary { get; set; }
        public List<string> Images { get; set; }
        public double[] Coordinates { get; set; } // [经度, 纬度]
    }

    public class Category
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string CoverImage { get; set; }
        public string Description { get; set; }
        public List<ScenicSpot> Attractions { get; set; }
    }

    public class ScenicData
    {
        public List<Category> Categories { get; set; }
    }
}
