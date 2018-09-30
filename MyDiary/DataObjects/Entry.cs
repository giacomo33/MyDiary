using Microsoft.Azure.Mobile.Server;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace MyDiary.DataObjects
{
    [Table("Entry")]
    public class Entry : EntityData
    {
        [Column("Title")]
        public string Title { get; set; }
        [Column("Description")]
        public string Description { get; set; }
        [Column("UserId")]
        public string UserId { get; set; }
    }
}