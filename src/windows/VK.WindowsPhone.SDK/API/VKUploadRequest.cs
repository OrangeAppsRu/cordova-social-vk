using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VK.WindowsPhone.SDK.API.Model;
using VK.WindowsPhone.SDK.Util;

namespace VK.WindowsPhone.SDK.API
{
    public class VKUploadRequest
    {

        class VKUploadServerAddress
        {
            public string upload_url { get; set; }
        }

        class VKUploadResponseData
        {
            public string server { get; set; }

            public string photo { get; set; }

            public string photos_list { get; set; }

            public string hash { get; set; }

            public string aid { get; set; }

            public long uid { get; set; }

            public long gid { get; set; }
        }

        enum UploadType
        {
            PhotoAlbumUpload,
            PhotoWallUpload,
            PhotoProfileUpload,
            PhotoMessageUpload
        }

        private UploadType _uploadType;

        private long _albumId;        

        private long _ownerId;

        private long _groupId;

        protected VKUploadRequest()
        {
        }

        public static VKUploadRequest CreatePhotoAlbumUploadRequest(long albumId, long groupId = 0)
        {
            var uploadRequest = new VKUploadRequest();

            uploadRequest._uploadType = UploadType.PhotoAlbumUpload;
            uploadRequest._albumId = albumId;
            uploadRequest._groupId = groupId;

            return uploadRequest;   
        }

        public static VKUploadRequest CreatePhotoWallUploadRequest(long ownerId=0)
        {
            var uploadRequest = new VKUploadRequest();

            uploadRequest._uploadType = UploadType.PhotoWallUpload;
            uploadRequest._ownerId = ownerId;

            return uploadRequest;
        }

        public static VKUploadRequest CreatePhotoProfileUploadRequest(long ownerId)
        {
            var uploadRequest = new VKUploadRequest();

            uploadRequest._uploadType = UploadType.PhotoProfileUpload;
            uploadRequest._ownerId = ownerId;

            return uploadRequest;
        }

        public static VKUploadRequest CreatePhotoMessageUploadRequest()
        {
            var uploadRequest = new VKUploadRequest();

            uploadRequest._uploadType = UploadType.PhotoMessageUpload;

            return uploadRequest;
        }

        public void Dispatch(
            Stream photoStream,
            Action<double> progressCallback,
            Action<VKBackendResult<VKPhoto>> callback)
        {
            switch (_uploadType)
            {
                case UploadType.PhotoAlbumUpload: 
                    DispatchPhotoAlbumUpload(photoStream, progressCallback, callback);
                    break;
                case UploadType.PhotoMessageUpload:
                    DispatchPhotoMessageUpload(photoStream, progressCallback, callback);
                    break;
                case UploadType.PhotoProfileUpload:
                    DispatchPhotoProfileUpload(photoStream, progressCallback, callback);
                    break;
                case UploadType.PhotoWallUpload:
                    DispatchPhotoWallUpload(photoStream, progressCallback, callback);
                    break;
            }
        }

        private void DispatchPhotoWallUpload(Stream photoStream, Action<double> progressCallback, Action<VKBackendResult<VKPhoto>> callback)
        {
            var parameters = new Dictionary<string, string>();
            if (_ownerId != 0)
            {
                string paramName = _ownerId < 0 ? "group_id" : "user_id";

                parameters[paramName] = Math.Abs(_ownerId).ToString();
            }

            UploadPhoto(photoStream,
                "photos.getWallUploadServer",
                parameters,
                "photos.saveWallPhoto",
                true,
                callback,
                progressCallback);
        }

        private void DispatchPhotoProfileUpload(Stream photoStream, Action<double> progressCallback, Action<VKBackendResult<VKPhoto>> callback)
        {
            var parameters = new Dictionary<string, string>();

            UploadPhoto(photoStream,
              "photos.getProfileUploadServer",
              parameters,
              "photos.saveProfilePhoto",
              false,
              callback,
              progressCallback);           
        }

        private void DispatchPhotoMessageUpload(Stream photoStream, Action<double> progressCallback, Action<VKBackendResult<VKPhoto>> callback)
        {
            var parameters = new Dictionary<string, string>();

            UploadPhoto(photoStream,
               "photos.getMessagesUploadServer",
                parameters,
                "photos.saveMessagesPhoto",
                true,
                callback,
                progressCallback);            
        }

        private void DispatchPhotoAlbumUpload(Stream photoStream, Action<double> progressCallback, Action<VKBackendResult<VKPhoto>> callback)
        {
            var parameters = new Dictionary<string, string>();

            parameters["album_id"] = _albumId.ToString();

            if (_groupId != 0)
            {
                parameters["group_id"] = _groupId.ToString();
            }


            UploadPhoto(photoStream,
               "photos.getUploadServer",
                parameters,
                "photos.save",
                true,
                callback,
                progressCallback);                  
        }

        private void UploadPhoto(Stream photoStream,
            string getServerMethodName,
            Dictionary<string, string> parameters,
            string saveMethodName,
            bool saveReturnsList,
            Action<VKBackendResult<VKPhoto>> callback,
            Action<double> progressCallback)
        {

            var vkParams = new VKRequestParameters(getServerMethodName,
               parameters);

            var getServerRequest = new VKRequest(vkParams);

            getServerRequest.Dispatch<VKUploadServerAddress>(
               (res) =>
               {
                   if (res.ResultCode == VKResultCode.Succeeded)
                   {
                       var uploadUrl = res.Data.upload_url;

                       VKHttpRequestHelper.Upload(
                            uploadUrl,
                            photoStream,
                            "file1",
                            "image",
                            (uploadRes) =>
                            {
                                if (uploadRes.IsSucceeded)
                                {
                                    var serverPhotoHashJson = uploadRes.Data;

                                    var uploadData = JsonConvert.DeserializeObject<VKUploadResponseData>(serverPhotoHashJson);
                                 
                                    if (!string.IsNullOrWhiteSpace(uploadData.server))
                                    {
                                        parameters["server"] = uploadData.server;
                                    }
                                    if (!string.IsNullOrWhiteSpace(uploadData.photos_list))
                                    {
                                        parameters["photos_list"] = uploadData.photos_list;
                                    }
                                    if (!string.IsNullOrWhiteSpace(uploadData.hash))
                                    {
                                        parameters["hash"] = uploadData.hash;
                                    }
                                    if (!string.IsNullOrWhiteSpace(uploadData.photo))
                                    {
                                        parameters["photo"] = uploadData.photo;
                                    }                                    

                                    var saveWallPhotoVKParams = new VKRequestParameters(saveMethodName,
                                        parameters);

                                    var saveWallPhotoRequest = new VKRequest(saveWallPhotoVKParams);

                                    saveWallPhotoRequest.Dispatch(
                                        callback,
                                        (jsonStr) =>
                                        {
                                            if (saveReturnsList)
                                            {
                                                var resp = JsonConvert.DeserializeObject<GenericRoot<List<VKPhoto>>>(jsonStr);

                                                return resp.response.First();
                                            }
                                            else
                                            {
                                                var resp = JsonConvert.DeserializeObject<GenericRoot<VKPhoto>>(jsonStr);

                                                return resp.response;
                                            }
                                        });

                                }
                                else
                                {
                                    callback(new VKBackendResult<VKPhoto> { ResultCode = VKResultCode.UnknownError });
                                }
                            },
                            progressCallback,
                            "Image.jpg");
                   }
                   else
                   {
                       callback(new VKBackendResult<VKPhoto> { ResultCode = res.ResultCode, Error = res.Error });
                   }
               });
        }
    }
}
