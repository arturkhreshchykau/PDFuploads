using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace PDFupload.Models
{
    public class UploadFile
    {
        public int Id { get; set; }
        public string FileName { get; set; }
        public byte[] FileContent { get; set; }
        public DateTime Date { get; set; }
    }
}