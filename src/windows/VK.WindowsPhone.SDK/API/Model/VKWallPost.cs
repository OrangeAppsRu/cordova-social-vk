using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VK.WindowsPhone.SDK.API.Model
{
    /// <summary>
    /// https://vk.com/dev/post
    /// </summary>
    public partial class VKWallPost
    {
        public long id { get; set; }

        public long owner_id { get; set; }

        public long from_id { get; set; }

        public long date { get; set; }

        public string text { get; set; }

        public long reply_owner_id { get; set; }

        public long reply_post_id { get; set; }

        public int friends_only { get; set; }

        public VKComments comments { get; set; }

        public VKLikes likes { get; set; }

        public VKReposts reposts { get; set; }

        public string post_type { get; set; }

        public VKPostSource post_source { get; set; }

        public List<VKAttachment> attachments { get; set; }

        public VKGeo geo { get; set; }

        public long signer_id { get; set; }

        public List<VKWallPost> copy_history { get; set; }

        public int can_pin { get; set; }

        public int is_pinned { get; set; }
    }


    public partial class VKComments
    {
        public int count { get; set; }
        public int can_post { get; set; }

    }

    public partial class VKLikes
    {
        public int count { get; set; }
        public int user_likes { get; set; }
        public int can_like { get; set; }
        public int can_publish { get; set; }
    }

    public partial class VKReposts
    {
        public int count { get; set; }
        public int user_reposted { get; set; }
    }

    public partial class VKPostSource
    {
        public string platform { get; set; }
        public string type { get; set; }
        public string data { get; set; }
    }
}
