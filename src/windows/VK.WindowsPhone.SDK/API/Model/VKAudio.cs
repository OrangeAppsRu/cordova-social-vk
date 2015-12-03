using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VK.WindowsPhone.SDK.Util;

namespace VK.WindowsPhone.SDK.API.Model
{
    public partial class VKAudio
    {
        public long id { get; set; }

        public long owner_id { get; set; }

        private string _artist = "";

        public string artist
        {
            get { return _artist; }
            set
            {
                _artist = (value ?? "").ForUI();
                // do not allow new line
                _artist = _artist.MakeIntoOneLine();

            }
        }

        private string _title = "";

        public string title
        {
            get { return _title; }
            set
            {
                _title = (value ?? "").ForUI();
                _title = _title.MakeIntoOneLine();
            }
        }

        public int duration { get; set; }

        public string url { get; set; }

        public long lyrics_id { get; set; }

        public long album_id { get; set;}

        public long genre_id { get; set; }
    }
}
