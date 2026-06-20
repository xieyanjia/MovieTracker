using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MovieTracker
{
    public class Movie
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Type { get; set; }       // 電影 / 影集
        public string Status { get; set; }     // 想看 / 看過 / 看一半
        public int Rating { get; set; }        // 1~5 星
        public string Genre { get; set; }      // 類型
        public string Notes { get; set; }      // 心得備註
        public string CoverUrl { get; set; }   // 封面圖片路徑
        public string AddedDate { get; set; }  // 新增日期
        public string WatchDate { get; set; }  // 預計觀看日期
        public bool Reminder { get; set; }     // 提醒
    }
}