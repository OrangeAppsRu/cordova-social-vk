using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using VK.WindowsPhone.SDK;
using VK.WindowsPhone.SDK.API;
using VK.WindowsPhone.SDK.API.Model;
using VK.WindowsPhone.SDK.Util;
using VK.WindowsPhone.SDK.Pages;
using Social.Cordova.JSON;
using System.IO;
using System.Diagnostics;
using Windows.UI.Core;

namespace Social
{
    public sealed class EventArgs {
        public int callbackid { get; set; }
        public string error { get; set; }
        public string result { get; set; }
    }

    public sealed class SocialVk
    {
        public event EventHandler<EventArgs> callback;
        private int lastCbId = 0;

        private void sendResult(int cbid = 0, string result = "", string error = "") {
            if (cbid <= 0) cbid = lastCbId;
            if (cbid <= 0) {
                Debug.WriteLine("SocialVk: Wrong callback ID!");
                return;
            }
            if (callback != null)
                callback(this, new EventArgs() { callbackid = cbid, result = result, error = error });
            else
                Debug.WriteLine("SocialVk: Callback not defined!");
        }

        public void test1(string args, int cbid) {
            string res = "input " + args;
            if (callback != null) callback(this, new EventArgs() {callbackid = cbid, result = res, error = "" });
        }

        public void init(string appId, int cbid) {
            VKSDK.AccessTokenReceived += (sender, arg) => {
                Debug.WriteLine("Access token recieved " + arg);
                JObject res = new JObject();
                res.Add("token", arg.NewToken.AccessToken);
                sendResult(0, res.ToString());
            };

            VKSDK.CaptchaRequest = (VKCaptchaUserRequest captchaUserRequest, Action<VKCaptchaUserResponse> action) => {
                Debug.WriteLine("Captcha request " + captchaUserRequest);
                sendResult(0, "", "Captcha request");
            };

            VKSDK.AccessDenied += (sender, arg) => {
                Debug.WriteLine("Access denied " + arg);
                sendResult(0, "", "Access denied");
            };

            VKSDK.Initialize(appId);
            VKSDK.WakeUpSession();

            sendResult(cbid, "VK Plugin inited");
        }

        public void login(string par, int cbid) {
            lastCbId = cbid;
            string[] _scope = JsonHelper.Deserialize<string[]>(par);
            if(_scope == null) {
                Debug.WriteLine("Empty permissions list");
                sendResult(cbid, "", "Empty permissions list." + par);
            } else {
                List<String> scope = new List<string>(_scope);
                Windows.ApplicationModel.Core.CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal,
                    () =>
                        {
                            sendResult(cbid, "", "Threading test");
                            VKSDK.Authorize(scope, false, false);
                        }
                );
            }
        }

        public void share(string sourceUrl, string description, string imageUrl, int cbid) {
            sendResult(cbid, "", "Not implemented");
        }

        public void logout(int cbid) {
            VKSDK.Logout();
            sendResult(cbid, "Logout comleted");
        }

        public void users_get(string user_ids, string fields, string name_case, int cbid) {
            VKRequest.Dispatch<List<VKUser>>(
                            new VKRequestParameters(
                                "users.get",
                                "user_ids", user_ids,
                                "fields", fields),
                            (res) => {
                                if (res.ResultCode == VKResultCode.Succeeded) {
                                    //var user = res.Data[0];
                                    //DispatchCommandResult(new PluginResult(PluginResult.Status.OK, res.ResultString), options.Last<string>());
                                    sendResult(cbid, res.ResultString);
                                } else {
                                    //DispatchCommandResult(new PluginResult(PluginResult.Status.ERROR, res.ResultString), options.Last<string>());
                                    sendResult(cbid, "", res.ResultString);
                                }
                            });
        }

        public void users_search(string query_or_params, int cbid) {
            VKRequestParameters vkrp;
            try {
                JObject p = JObject.Parse(query_or_params);
                var dict = p.ToObject<Dictionary<string, string>>();
                vkrp = new VKRequestParameters("users.search", dict);
            } catch (Exception e) {
                vkrp = new VKRequestParameters("users.search", "q", query_or_params);
            }
            VKRequest.Dispatch<object>(
                            vkrp,
                            (res) => {
                                if (res.ResultCode == VKResultCode.Succeeded) {
                                    sendResult(cbid, res.ResultString);
                                } else {
                                    sendResult(cbid, "", res.ResultString);
                                }
                            });
        }

        public void users_isAppUser(string user_id, int cbid) {
            VKRequestParameters vkrp;
            vkrp = new VKRequestParameters("users.isAppUser", "user_id", user_id);
            VKRequest.Dispatch<object>(
                            vkrp,
                            (res) => {
                                if (res.ResultCode == VKResultCode.Succeeded) {
                                    sendResult(cbid, res.ResultString);
                                } else {
                                    sendResult(cbid, "", res.ResultString);
                                }
                            },
                            (json) => new Object());
        }

        public void users_getSubscriptions(string user_id, string extended, string offset, string count, string fields, int cbid) {
            VKRequestParameters vkrp;
            vkrp = new VKRequestParameters("users.getSubscriptions", "user_id", user_id, "extended", extended, "offset", offset, "count", count, "fields", fields);
            VKRequest.Dispatch<object>(
                            vkrp,
                            (res) => {
                                if (res.ResultCode == VKResultCode.Succeeded) {
                                    sendResult(cbid, res.ResultString);
                                } else {
                                    sendResult(cbid, "", res.ResultString);
                                }
                            });
        }

        public void users_getFollowers(string user_id, string offset, string count, string fields, string name_case, int cbid) {
            VKRequestParameters vkrp;
            vkrp = new VKRequestParameters("users.getFollowers", "user_id", user_id, "offset", offset, "count", count, "fields", fields, "name_case", name_case);
            VKRequest.Dispatch<object>(
                            vkrp,
                            (res) => {
                                if (res.ResultCode == VKResultCode.Succeeded) {
                                    sendResult(cbid, res.ResultString);
                                } else {
                                    sendResult(cbid, "", res.ResultString);
                                }
                            });
        }

        public void wall_post(string par) {
            /*
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
            VKSDK.Publish(data);
            */
        }

        public void photos_getUploadServer(string album_id, string group_id, int cbid) {
            VKRequestParameters vkrp;
            vkrp = new VKRequestParameters("photos.getUploadServer", "album_id", album_id, "group_id", group_id);
            VKRequest.Dispatch<object>(
                            vkrp,
                            (res) => {
                                if (res.ResultCode == VKResultCode.Succeeded) {
                                    sendResult(cbid, res.ResultString);
                                } else {
                                    sendResult(cbid, "", res.ResultString);
                                }
                            });
        }

        public void photos_getWallUploadServer(string group_id, int cbid) {
            VKRequestParameters vkrp;
            vkrp = new VKRequestParameters("photos.getWallUploadServer", "group_id", group_id);
            VKRequest.Dispatch<object>(
                            vkrp,
                            (res) => {
                                if (res.ResultCode == VKResultCode.Succeeded) {
                                    sendResult(cbid, res.ResultString);
                                } else {
                                    sendResult(cbid, "", res.ResultString);
                                }
                            });
        }

        public void photos_saveWallPhoto(string imageBase64, string user_id, string group_id, int cbid) {
            long userId = long.Parse(user_id);
            long groupdId = long.Parse(group_id);
            byte[] bytes;
            int comma = imageBase64.IndexOf(',');
            if (comma >= 0) {
                bytes = Convert.FromBase64String(imageBase64.Substring(comma + 1));
            } else {
                bytes = Convert.FromBase64String(imageBase64);
            }
            var ms = new MemoryStream(bytes, 0, bytes.Length);
            VKUploadRequest req = VKUploadRequest.CreatePhotoWallUploadRequest(userId);
            req.Dispatch(ms, (progress) => {
            }, (res) => {
                if (res.ResultCode == VKResultCode.Succeeded) {
                    sendResult(cbid, res.ResultString);
                } else {
                    string errtext;
                    if (res.ResultString != null) errtext = res.ResultString;
                    else if (res.Error != null) errtext = res.Error.error_msg;
                    else errtext = "Unknown error";
                    sendResult(cbid, "", errtext);
                }
            });
        }

        public void photos_save(string imageBase64, string album_id, string group_id, int cbid) {
            long albumId = long.Parse(album_id);
            long groupdId = long.Parse(group_id);
            byte[] bytes;
            int comma = imageBase64.IndexOf(',');
            if (comma >= 0) {
                bytes = Convert.FromBase64String(imageBase64.Substring(comma + 1));
            } else {
                bytes = Convert.FromBase64String(imageBase64);
            }
            var ms = new MemoryStream(bytes, 0, bytes.Length);
            VKUploadRequest req = VKUploadRequest.CreatePhotoAlbumUploadRequest(albumId, groupdId);
            req.Dispatch(ms, (progress) => {
            }, (res) => {
                if (res.ResultCode == VKResultCode.Succeeded) {
                    sendResult(cbid, res.ResultString);
                } else {
                    string errtext;
                    if (res.ResultString != null) errtext = res.ResultString;
                    else if (res.Error != null) errtext = res.Error.error_msg;
                    else errtext = "Unknown error";
                    sendResult(cbid, "", errtext);
                }
            });
        }

        public void friends_get(string user_id, string order, string count, string offset, string fields, string name_case, int cbid) {
            VKRequestParameters vkrp;
            vkrp = new VKRequestParameters("friends.get", "user_id", user_id, "order", order, "count", count, "offset", offset, "fields", fields, "name_case", name_case);
            VKRequest.Dispatch<object>(
                            vkrp,
                            (res) => {
                                if (res.ResultCode == VKResultCode.Succeeded) {
                                    sendResult(cbid, res.ResultString);
                                } else {
                                    sendResult(cbid, "", res.ResultString);
                                }
                            });
        }

        public void friends_getOnline(string user_id, string order, string count, string offset, int cbid) {
            VKRequestParameters vkrp;
            vkrp = new VKRequestParameters("friends.getOnline", "user_id", user_id, "order", order, "count", count, "offset", offset);
            VKRequest.Dispatch<object>(
                            vkrp,
                            (res) => {
                                if (res.ResultCode == VKResultCode.Succeeded) {
                                    sendResult(cbid, res.ResultString);
                                } else {
                                    sendResult(cbid, "", res.ResultString);
                                }
                            });
        }

        public void friends_getMutual(string source_uid, string target_uid, string order, string count, string offset, int cbid) {
            VKRequestParameters vkrp;
            vkrp = new VKRequestParameters("friends.getMutual", "source_uid", source_uid, "target_uid", target_uid, "order", order, "count", count, "offset", offset);
            VKRequest.Dispatch<object>(
                            vkrp,
                            (res) => {
                                if (res.ResultCode == VKResultCode.Succeeded) {
                                    sendResult(cbid, res.ResultString);
                                } else {
                                    sendResult(cbid, "", res.ResultString);
                                }
                            });
        }

        public void friends_getRecent(string count, int cbid) {
            VKRequestParameters vkrp;
            vkrp = new VKRequestParameters("friends.getRecent", "count", count);
            VKRequest.Dispatch<object>(
                            vkrp,
                            (res) => {
                                if (res.ResultCode == VKResultCode.Succeeded) {
                                    sendResult(cbid, res.ResultString);
                                } else {
                                    sendResult(cbid, "", res.ResultString);
                                }
                            });
        }

        public void friends_getRequests(string offset, string count, string extended, string needs_mutual, string _out, string sort, string suggested, int cbid) {
            VKRequestParameters vkrp;
            vkrp = new VKRequestParameters("friends.getRequests", "offset", offset, "count", count, "extended", extended, "needs_mutual", needs_mutual, "out", _out, "sort", sort, "suggested", suggested);
            VKRequest.Dispatch<object>(
                            vkrp,
                            (res) => {
                                if (res.ResultCode == VKResultCode.Succeeded) {
                                    sendResult(cbid, res.ResultString);
                                } else {
                                    sendResult(cbid, "", res.ResultString);
                                }
                            });
        }

        public void callApiMethod(string method, string args, int cbid) {
            VKRequestParameters vkrp = null;
            try {
                JObject p = JObject.Parse(args);
                var dict = p.ToObject<Dictionary<string, string>>();
                vkrp = new VKRequestParameters(method, dict);
            } catch (Exception e) {

            }
            if (vkrp != null)
                VKRequest.Dispatch<object>(
                                vkrp,
                                (res) => {
                                    if (res.ResultCode == VKResultCode.Succeeded) {
                                        sendResult(cbid, res.ResultString);
                                    } else {
                                        sendResult(cbid, "", res.ResultString);
                                    }
                                });
            else
                sendResult(cbid, "", "Invalid parameters: " + method + " " + args);
        }
    }
}
