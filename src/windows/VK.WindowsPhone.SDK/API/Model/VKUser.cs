using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VK.WindowsPhone.SDK.Util;

namespace VK.WindowsPhone.SDK.API.Model
{
    /// <summary>
    /// https://vk.com/dev/fields
    /// </summary>
    public partial class VKUser
    {
        public long id
        {
            get;
            set;
        }

        private string _fName = "";
        public string first_name
        {
            get { return _fName; }
            set
            {
                _fName = (value ?? "").ForUI();
            }
        }

        private string _lName = "";
        public string last_name
        {
            get { return _lName; }
            set
            {
                _lName = (value ?? "").ForUI();
            }
        }

        public string deactivated
        {
            get;
            set;
        }

        public int hidden
        {
            get;
            set;
        }

        public int verified
        {
            get;
            set;
        }

        public int blacklisted
        {
            get;
            set;
        }

        public int sex
        {
            get;
            set;
        }

        public string bdate { get; set; }

        public VKCity city { get; set; }

        public VKCountry country { get; set; }

        public string home_town { get; set; }

        public string photo_50 { get; set; }

        public string photo_100 { get; set; }

        public string photo_200_orig { get; set; }

        public string photo_200 { get; set; }

        public string photo_400_orig { get; set; }

        public string photo_max { get; set; }

        public string photo_max_orig { get; set; }

        public int online { get; set; }

        public List<long> lists { get; set; }

        public string domain { get; set; }

        public int has_mobile { get; set; }

        public string mobile_phone { get; set; }

        public string home_phone { get; set; }

        public string site { get; set; }

        
        
        public long university { get; set; }

        public string university_name { get; set; }

        public long faculty { get; set; }

        public string faculty_name { get; set; }

        public int graduation { get; set; }


        public List<VKUniversity> universities { get; set; }

        public List<VKSchool> schools { get; set; }
        
        public string status { get; set; }

        public VKAudio status_audio { get; set; }

        public int followers_count { get; set; }

        public int common_count { get; set; }

        public VKCounters counters { get; set; }

        public VKUserOccupation occupation
        {
            get;
            set;
        }

        public string nickname { get; set; }

        public List<VKUserRelative> relatives { get; set; }

        public int relation { get; set; }

        public VKUserPersonal personal { get; set; }

        public string facebook { get; set; }

        public string twitter { get; set; }

        public string livejournal { get; set; }

        public string instagram { get; set; }


        public VKUserExports exports { get; set;}

        public int wall_comments { get; set; }

        private string _activity = "";
        public string activities
        {
            get { return _activity; }
            set
            {
                _activity = (value ?? "").ForUI();
            }
        }


        private string _interests = "";
        public string interests
        {
            get { return _interests; }
            set
            {
                _interests = (value ?? "").ForUI();
            }
        }

        private string _movies = "";
        public string movies
        {
            get
            {
                return _movies;
            }
            set
            {
                _movies = (value ?? "").ForUI();
            }
        }

        private string _tv = "";
        public string tv
        {
            get { return _tv; }
            set
            {
                _tv = (value ?? "").ForUI();
            }
        }

        private string _books = "";
        public string books
        {
            get { return _books; }
            set
            {
                _books = (value ?? "").ForUI();

            }
        }

        private string _games = "";
        public string games
        {
            get { return _games; }
            set
            {
                _games = (value ?? "").ForUI();
            }
        }

        private string _about = "";
        public string about
        {
            get { return _about; }
            set
            {
                _about = (value ?? "").ForUI();
            }
        }

        private string _quotes = "";
        public string quotes
        {
            get { return _quotes; }
            set
            {
                _quotes = (value ?? "").ForUI();
            }
        }


        public int can_post { get; set; }

        public int can_see_all_posts { get; set; }

        public int can_see_audio { get; set; }

        public int can_write_private_message { get; set; }

        public int timezone { get; set; }

        public string screen_name { get; set; }

        public string maiden_name { get; set; }

    }


    public partial class VKUserExports
    {
        public int twitter { get; set; }

        public int facebook { get; set; }

        public int livejournal { get; set; }

        public int instagram { get; set; }
    }

    public partial class VKUserPersonal
    {
        public int political { get; set; }

        public List<string> langs { get; set; }

        public string religion
        {
            get;
            set;
        }


        private string _inspired_by = "";
        public string inspired_by
        {
            get
            {
                return _inspired_by;
            }
            set
            {
                _inspired_by = (value ?? "").ForUI();
            }
        }

        public int people_main
        {
            get;
            set;
        }

        public int life_main
        {
            get;
            set;
        }

        public int smoking
        {
            get;
            set;
        }

        public int alcohol
        {
            get;
            set;
        }
    }

    public partial class VKUserRelative
    {       
        public long id { get; set; }

        public string name {get;set;}

        public string type { get; set; }
    }

    public partial class VKUserOccupation
    {
        public static readonly string OCCUPATION_TYPE_WORK = "work";
        public static readonly string OCCUPATION_TYPE_SCHOOL = "school";
        public static readonly string OCCUPATION_TYPE_UNIVERSITY = "university";

        public string type { get; set; }

        public long id { get; set; }

        public string name { get; set; }
    }

    public partial class VKCounters
    {

        public int albums { get; set; }

        public int videos { get; set; }

        public int audios { get; set; }

        public int notes { get; set; }
        public int groups { get; set; }

        public int photos { get; set; }
        public int friends { get; set; }
        public int online_friends { get; set; }
        public int mutual_friends { get; set; }
        public int user_videos { get; set; }
        public int user_photos { get; set; }
        public int followers { get; set; }
        public int subscriptions { get; set; }

        public int topics { get; set; }
        public int docs { get; set; }

        public int pages { get; set; }

    }


    public partial class VKUserStatus
    {
        public long time { get; set; }
 
        public int platform { get; set; }
    }

    public partial class VKUniversity
    {
        public long id { get; set; }

        public long country { get; set; }

        public long city { get; set; }

        public string name { get; set; }

        public long faculty { get; set; }

        public string faculty_name { get; set; }

        public long chair { get; set; }

        public string chair_name { get; set; }

        public int graduation { get; set; }
    
    }

    public partial class VKSchool
    {
        public long id { get; set; }

        public long country { get; set; }

        public long city { get; set; }

        public string name { get; set; }

        public int year_from { get; set; }

        public int year_to { get; set; }

        public int year_graduated { get; set; }

        public string @class {get;set;}

        public string speciality { get; set; }

        public long type { get; set; }

        public string type_str { get; set; }

    }
}
