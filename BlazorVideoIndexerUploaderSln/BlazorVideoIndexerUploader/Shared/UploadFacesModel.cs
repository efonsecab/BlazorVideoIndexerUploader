using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace BlazorVideoIndexerUploader.Shared
{
    public class UploadFacesModel
    {
        [Required]
        public string PersonModelId { get; set; }
        [Required]
        public string PersonName { get; set; }
        public List<PersonImage> PersonImages { get; set; }
    }

    public class PersonImage
    {
        public bool IsSelected { get; set; }
        [Url]
        public string ImageUrl { get; set; }
    }
}
