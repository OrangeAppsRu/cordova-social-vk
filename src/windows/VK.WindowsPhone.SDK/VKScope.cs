using System;
using System.Collections.Generic;

namespace VK.WindowsPhone.SDK
{
    public class VKScope
    {
        public const String NOTIFY = "notify";
        public const String FRIENDS = "friends";
        public const String PHOTOS = "photos";
        public const String AUDIO = "audio";
        public const String VIDEO = "video";
        public const String DOCS = "docs";
        public const String NOTES = "notes";
        public const String PAGES = "pages";
        public const String STATUS = "status";
        public const String WALL = "wall";
        public const String GROUPS = "groups";
        public const String MESSAGES = "messages";
        public const String NOTIFICATIONS = "notifications";
        public const String STATS = "stats";
        public const String ADS = "ads";
        public const String OFFLINE = "offline";
        public const String NOHTTPS = "nohttps";
        public const String DIRECT = "direct";

        /// <summary>
        /// Converts integer permissions value into List of constants
        /// </summary>
        /// <param name="permissionsValue">Integer permissons value</param>
        /// <returns>List containing constant strings of permissions (scope)</returns>
        public static List<String> ParseVKPermissionsFromInteger(int permissionsValue)
        {
            var res = new List<String>();
            if ((permissionsValue & 1) > 0) res.Add(NOTIFY);
            if ((permissionsValue & 2) > 0) res.Add(FRIENDS);
            if ((permissionsValue & 4) > 0) res.Add(PHOTOS);
            if ((permissionsValue & 8) > 0) res.Add(AUDIO);
            if ((permissionsValue & 16) > 0) res.Add(VIDEO);
            if ((permissionsValue & 128) > 0) res.Add(PAGES);
            if ((permissionsValue & 1024) > 0) res.Add(STATUS);
            if ((permissionsValue & 2048) > 0) res.Add(NOTES);
            if ((permissionsValue & 4096) > 0) res.Add(MESSAGES);
            if ((permissionsValue & 8192) > 0) res.Add(WALL);
            if ((permissionsValue & 32768) > 0) res.Add(ADS);
            if ((permissionsValue & 65536) > 0) res.Add(OFFLINE);
            if ((permissionsValue & 131072) > 0) res.Add(DOCS);
            if ((permissionsValue & 262144) > 0) res.Add(GROUPS);
            if ((permissionsValue & 524288) > 0) res.Add(NOTIFICATIONS);
            if ((permissionsValue & 1048576) > 0) res.Add(STATS);
            return res;
        }
    }
}
