using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace UsefulThings
{
    public class Ad
    {
        public string Id { get; set; } 
        public string Url { get; set; }
        public string ImgPath { get; set; }
        public string UploadImageStatus { get; set; }
        public string Title { get; set; }
        public string Price { get; set; }
        public string Href { get; set; }
        public string Type { get; set; }
        public string WasSeen { get; set; }
        public string IsDelete { get; set; }
        public string PrePrice { get; set; }
        public string AvitoId { get; set; }
        public string Time { get; set; }
        public string Phone { get; set; }
        public string Name { get; set; }
        public string City { get; set; }
        public string Description { get; set; }
        public string ImgLink { get; set; }
        public string Cat1 { get; set; }
        public string Cat2 { get; set; }
        public string Cat3 { get; set; }
        public Bitmap BitMap { get; set; }

        public Ad() { }
    }
}
