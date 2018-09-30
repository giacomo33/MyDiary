using Microsoft.WindowsAzure.MobileServices;
using Newtonsoft.Json;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace MyDiary.App.Models
{
    [Microsoft.WindowsAzure.MobileServices.DataTable("Entry")]
    public class DiaryEntry
    {
        [JsonProperty(PropertyName = "id")]
        public string Id { get; set; }

        [JsonProperty(PropertyName = "title")]
        public string Title { get; set; }

        [JsonProperty(PropertyName = "description")]
        public string Description { get; set; }

        [Version]
        public string AzureVersion { get; set; }
        [CreatedAt]
        public DateTime CreatedOn { get; set; }
        [UpdatedAt]
        public DateTime Updated { get; set; }

        public string UserId { get; set; }

        public string ShortTitle
        {
            get
            {
                return Title.Length <= 20 ? Title : Title.Substring(0, 20) + "...";
            }
        }

        public override string ToString()
        {
            return Title;
        }
    }
}
