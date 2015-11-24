using System;
using System.Net;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Microsoft.Phone.Controls;
using Windows.ApplicationModel.Store;
using WPCordovaClassLib.Cordova;
using WPCordovaClassLib.Cordova.Commands;
using WPCordovaClassLib.Cordova.JSON;
using Newtonsoft.Json.Linq;
using VK.WindowsPhone.SDK;
using VK.WindowsPhone.SDK.API;
using VK.WindowsPhone.SDK.API.Model;
using VK.WindowsPhone.SDK.Util;
using System.Windows.Media.Imaging;
using VK.WindowsPhone.SDK.Pages;
using System.IO;

namespace WPCordovaClassLib.Cordova.Commands
{

    public class SocialVk : BaseCommand
    {
        public void initSocialVk(string par)
        {
            string[] options = JsonHelper.Deserialize<string[]>(par);
            VKSDK.AccessTokenReceived += (sender, args) =>
            {
                System.Diagnostics.Debug.WriteLine("Access token recieved " + args);
                JObject res = new JObject();
                res.Add("token", args.NewToken.AccessToken);
                DispatchCommandResult(new PluginResult(PluginResult.Status.OK, res.ToString()));
                // TODO load user's profile
            };

            VKSDK.CaptchaRequest = (VKCaptchaUserRequest captchaUserRequest, Action<VKCaptchaUserResponse> action) =>
            {
                System.Diagnostics.Debug.WriteLine("Captcha request " + captchaUserRequest);
                DispatchCommandResult(new PluginResult(PluginResult.Status.ERROR, "Captcha request"));
            };

            VKSDK.AccessDenied += (sender, args) =>
            {
                System.Diagnostics.Debug.WriteLine("Access denied " + args);
                DispatchCommandResult( new PluginResult(PluginResult.Status.ERROR, "Access denied"));
            };

            VKSDK.Initialize(options[0]);
            VKSDK.WakeUpSession();

            DispatchCommandResult(new PluginResult(PluginResult.Status.OK, "VK Plugin inited"));
        }

        public void login(string par)
        {
            string[] _scope = JsonHelper.Deserialize<string[]>( JsonHelper.Deserialize<string[]>(par)[0]);
            List<String> scope = new List<string>(_scope);
            DispatchInvoke(() =>
            {
                VKSDK.Authorize(scope, false, false);
            });
        }

        public void share(string par)
        {
            DispatchCommandResult(new PluginResult(PluginResult.Status.ERROR, "Not implemented"));
        }

        public void logout(string par)
        {
            DispatchInvoke(() =>
            {
                VKSDK.Logout();
                DispatchCommandResult(new PluginResult(PluginResult.Status.OK));
            });
        }

        public void users_get(string par)
        {
            string[] options = JsonHelper.Deserialize<string[]>(par);
            VKRequest.Dispatch<List<VKUser>>(
                            new VKRequestParameters(
                                "users.get",
                                "user_ids", options[0],
                                "fields", options[1]),
                            (res) =>
                            {
                                if (res.ResultCode == VKResultCode.Succeeded) {
                                    //var user = res.Data[0];
                                    DispatchCommandResult(new PluginResult(PluginResult.Status.OK, res.ResultString), options.Last<string>());
                                } else {
                                    DispatchCommandResult(new PluginResult(PluginResult.Status.ERROR, res.ResultString), options.Last<string>());
                                }
                            });
        }

        public void users_search(string par)
        {
            string[] options = JsonHelper.Deserialize<string[]>(par);
            VKRequestParameters vkrp;
            try {
                JObject p = JObject.Parse(options[0]);
                var dict = p.ToObject<Dictionary<string, string>>();
                vkrp = new VKRequestParameters("users.search", dict);
            } catch(Exception e) {
                vkrp = new VKRequestParameters("users.search", "q", options[0]);
            }
            VKRequest.Dispatch<object>(
                            vkrp,
                            (res) => {
                                if (res.ResultCode == VKResultCode.Succeeded) {
                                    DispatchCommandResult(new PluginResult(PluginResult.Status.OK, res.ResultString), options.Last<string>());
                                } else {
                                    DispatchCommandResult(new PluginResult(PluginResult.Status.ERROR, res.ResultString), options.Last<string>());
                                }
                            });
        }

        public void users_isAppUser(string par)
        {
            string[] options = JsonHelper.Deserialize<string[]>(par);
            VKRequestParameters vkrp;
            vkrp = new VKRequestParameters("users.isAppUser", "user_id", options[0]);
            VKRequest.Dispatch<object>(
                            vkrp,
                            (res) => {
                                if (res.ResultCode == VKResultCode.Succeeded) {
                                    DispatchCommandResult(new PluginResult(PluginResult.Status.OK, res.ResultString), options.Last<string>());
                                } else {
                                    DispatchCommandResult(new PluginResult(PluginResult.Status.ERROR, res.ResultString), options.Last<string>());
                                }
                            },
                            (json) => new Object());
        }

        public void users_getSubscriptions(string par)
        {
            string[] options = JsonHelper.Deserialize<string[]>(par);
            VKRequestParameters vkrp;
            vkrp = new VKRequestParameters("users.getSubscriptions", "user_id", options[0], "extended", options[1], "offset", options[2], "count", options[3], "fields", options[4]);
            VKRequest.Dispatch<object>(
                            vkrp,
                            (res) => {
                                if (res.ResultCode == VKResultCode.Succeeded) {
                                    DispatchCommandResult(new PluginResult(PluginResult.Status.OK, res.ResultString), options.Last<string>());
                                } else {
                                    DispatchCommandResult(new PluginResult(PluginResult.Status.ERROR, res.ResultString), options.Last<string>());
                                }
                            });
        }

        public void users_getFollowers(string par)
        {
            string[] options = JsonHelper.Deserialize<string[]>(par);
            VKRequestParameters vkrp;
            vkrp = new VKRequestParameters("users.getFollowers", "user_id", options[0], "offset", options[1], "count", options[2], "fields", options[3], "name_case", options[4]);
            VKRequest.Dispatch<object>(
                            vkrp,
                            (res) => {
                                if (res.ResultCode == VKResultCode.Succeeded) {
                                    DispatchCommandResult(new PluginResult(PluginResult.Status.OK, res.ResultString), options.Last<string>());
                                } else {
                                    DispatchCommandResult(new PluginResult(PluginResult.Status.ERROR, res.ResultString), options.Last<string>());
                                }
                            });
        }

        public void wall_post(string par)
        {
            string[] options = JsonHelper.Deserialize<string[]>(par);
            VKPublishInputData data = new VKPublishInputData();
            try {
                JObject p = JObject.Parse(options[0]);
                var dict = p.ToObject<Dictionary<string, string>>();
                data.Text = dict["message"];
                // TODO links and images
            } catch (Exception e) {
                data.Text = options[0];
            }
            DispatchInvoke(() => {
                VKSDK.Publish(data);
            });
        }

        public void photos_getUploadServer(string par)
        {
            string[] options = JsonHelper.Deserialize<string[]>(par);
            VKRequestParameters vkrp;
            vkrp = new VKRequestParameters("photos.getUploadServer", "album_id", options[0], "group_id", options[1]);
            VKRequest.Dispatch<object>(
                            vkrp,
                            (res) => {
                                if (res.ResultCode == VKResultCode.Succeeded) {
                                    DispatchCommandResult(new PluginResult(PluginResult.Status.OK, res.ResultString), options.Last<string>());
                                } else {
                                    DispatchCommandResult(new PluginResult(PluginResult.Status.ERROR, res.ResultString), options.Last<string>());
                                }
                            });
        }

        public void photos_getWallUploadServer(string par)
        {
            string[] options = JsonHelper.Deserialize<string[]>(par);
            VKRequestParameters vkrp;
            vkrp = new VKRequestParameters("photos.getWallUploadServer", "group_id", options[0]);
            VKRequest.Dispatch<object>(
                            vkrp,
                            (res) => {
                                if (res.ResultCode == VKResultCode.Succeeded) {
                                    DispatchCommandResult(new PluginResult(PluginResult.Status.OK, res.ResultString), options.Last<string>());
                                } else {
                                    DispatchCommandResult(new PluginResult(PluginResult.Status.ERROR, res.ResultString), options.Last<string>());
                                }
                            });
        }

        public void photos_saveWallPhoto(string par)
        {
            string[] options = JsonHelper.Deserialize<string[]>(par);
            long userId = long.Parse(options[1]);
            long groupdId = long.Parse(options[2]);
            byte[] bytes;
            int comma = options[0].IndexOf(',');
            if (comma >= 0) {
                bytes = Convert.FromBase64String(options[0].Substring(comma+1));
            } else {
                bytes = Convert.FromBase64String(options[0]);
            }
            var ms = new MemoryStream(bytes, 0, bytes.Length);
            VKUploadRequest req = VKUploadRequest.CreatePhotoWallUploadRequest(userId);
            req.Dispatch(ms, (progress) => {
            }, (res) => {
                if(res.ResultCode == VKResultCode.Succeeded) {
                    DispatchCommandResult(new PluginResult(PluginResult.Status.OK, res.ResultString), options.Last<string>());
                } else {
                    string errtext;
                    if (res.ResultString != null) errtext = res.ResultString;
                    else if (res.Error != null) errtext = res.Error.error_msg;
                    else errtext = "Unknown error";
                    DispatchCommandResult(new PluginResult(PluginResult.Status.ERROR, errtext), options.Last<string>());
                }
            });
        }

        public void photos_save(string par) 
        {
            string[] options = JsonHelper.Deserialize<string[]>(par);
            long albumId = long.Parse(options[1]);
            long groupdId = long.Parse(options[2]);
            byte[] bytes;
            int comma = options[0].IndexOf(',');
            if (comma >= 0) {
                bytes = Convert.FromBase64String(options[0].Substring(comma+1));
            } else {
                bytes = Convert.FromBase64String(options[0]);
            }
            var ms = new MemoryStream(bytes, 0, bytes.Length);
            VKUploadRequest req = VKUploadRequest.CreatePhotoAlbumUploadRequest(albumId, groupdId);
            req.Dispatch(ms, (progress) => {
            }, (res) => {
                if (res.ResultCode == VKResultCode.Succeeded) {
                    DispatchCommandResult(new PluginResult(PluginResult.Status.OK, res.ResultString), options.Last<string>());
                } else {
                    string errtext;
                    if (res.ResultString != null) errtext = res.ResultString;
                    else if (res.Error != null) errtext = res.Error.error_msg;
                    else errtext = "Unknown error";
                    DispatchCommandResult(new PluginResult(PluginResult.Status.ERROR, errtext), options.Last<string>());
                }
            });
        }

        public void friends_get(string par)
        {
            string[] options = JsonHelper.Deserialize<string[]>(par);
            VKRequestParameters vkrp;
            vkrp = new VKRequestParameters("friends.get", "user_id", options[0], "order", options[1], "count", options[2], "offset", options[3], "fields", options[4], "name_case", options[5]);
            VKRequest.Dispatch<object>(
                            vkrp,
                            (res) => {
                                if (res.ResultCode == VKResultCode.Succeeded) {
                                    DispatchCommandResult(new PluginResult(PluginResult.Status.OK, res.ResultString), options.Last<string>());
                                } else {
                                    DispatchCommandResult(new PluginResult(PluginResult.Status.ERROR, res.ResultString), options.Last<string>());
                                }
                            });
        }

        public void friends_getOnline(string par)
        {
            string[] options = JsonHelper.Deserialize<string[]>(par);
            VKRequestParameters vkrp;
            vkrp = new VKRequestParameters("friends.getOnline", "user_id", options[0], "order", options[1], "count", options[2], "offset", options[3]);
            VKRequest.Dispatch<object>(
                            vkrp,
                            (res) => {
                                if (res.ResultCode == VKResultCode.Succeeded) {
                                    DispatchCommandResult(new PluginResult(PluginResult.Status.OK, res.ResultString), options.Last<string>());
                                } else {
                                    DispatchCommandResult(new PluginResult(PluginResult.Status.ERROR, res.ResultString), options.Last<string>());
                                }
                            });
        }

        public void friends_getMutual(string par)
        {
            string[] options = JsonHelper.Deserialize<string[]>(par);
            VKRequestParameters vkrp;
            vkrp = new VKRequestParameters("friends.getMutual", "source_uid", options[0], "target_uid", options[1], "order", options[2], "count", options[3], "offset", options[4]);
            VKRequest.Dispatch<object>(
                            vkrp,
                            (res) => {
                                if (res.ResultCode == VKResultCode.Succeeded) {
                                    DispatchCommandResult(new PluginResult(PluginResult.Status.OK, res.ResultString), options.Last<string>());
                                } else {
                                    DispatchCommandResult(new PluginResult(PluginResult.Status.ERROR, res.ResultString), options.Last<string>());
                                }
                            });
        }

        public void friends_getRecent(string par)
        {
            string[] options = JsonHelper.Deserialize<string[]>(par);
            VKRequestParameters vkrp;
            vkrp = new VKRequestParameters("friends.getRecent", "count", options[0]);
            VKRequest.Dispatch<object>(
                            vkrp,
                            (res) => {
                                if (res.ResultCode == VKResultCode.Succeeded) {
                                    DispatchCommandResult(new PluginResult(PluginResult.Status.OK, res.ResultString), options.Last<string>());
                                } else {
                                    DispatchCommandResult(new PluginResult(PluginResult.Status.ERROR, res.ResultString), options.Last<string>());
                                }
                            });
        }

        public void friends_getRequests(string par)
        {
            string[] options = JsonHelper.Deserialize<string[]>(par);
            VKRequestParameters vkrp;
            vkrp = new VKRequestParameters("friends.getRequests", "offset", options[0], "count", options[1], "extended", options[2], "needs_mutual", options[3], "out", options[4], "sort", options[5], "suggested", options[6]);
            VKRequest.Dispatch<object>(
                            vkrp,
                            (res) => {
                                if (res.ResultCode == VKResultCode.Succeeded) {
                                    DispatchCommandResult(new PluginResult(PluginResult.Status.OK, res.ResultString), options.Last<string>());
                                } else {
                                    DispatchCommandResult(new PluginResult(PluginResult.Status.ERROR, res.ResultString), options.Last<string>());
                                }
                            });
        }

        public void callApiMethod(string par)
        {
            string[] options = JsonHelper.Deserialize<string[]>(par);
            VKRequestParameters vkrp = null;
            try {
                JObject p = JObject.Parse(options[1]);
                var dict = p.ToObject<Dictionary<string, string>>();
                vkrp = new VKRequestParameters(options[0], dict);
            } catch (Exception e) {

            }
            if(vkrp != null)
            VKRequest.Dispatch<object>(
                            vkrp,
                            (res) => {
                                if (res.ResultCode == VKResultCode.Succeeded) {
                                    DispatchCommandResult(new PluginResult(PluginResult.Status.OK, res.ResultString), options.Last<string>());
                                } else {
                                    DispatchCommandResult(new PluginResult(PluginResult.Status.ERROR, res.ResultString), options.Last<string>());
                                }
                            });
        }

        public void DispatchInvoke(Action a)
        {

#if SILVERLIGHT
            if (Deployment.Current.Dispatcher == null)
                a();
            else
                Deployment.Current.Dispatcher.BeginInvoke(a);
#else
    if ((Dispatcher != null) && (!Dispatcher.HasThreadAccess))
    {
        Dispatcher.InvokeAsync(
                    Windows.UI.Core.CoreDispatcherPriority.Normal, 
                    (obj, invokedArgs) => { a(); }, 
                    this, 
                    null
         );
    }
    else
        a();
#endif
        }
    }
}