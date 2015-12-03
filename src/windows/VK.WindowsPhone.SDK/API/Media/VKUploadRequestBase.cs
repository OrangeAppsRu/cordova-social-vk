using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using VK.WindowsPhone.SDK.API.Networking;

namespace VK.WindowsPhone.SDK.API.Media
{
    public abstract class VKUploadRequestBase : VKRequest
    {
        class VKUploadMediaOperation : VKOperationBase
        {
            private VKOperationBase _lastOperation;

            Func<VKRequest> _getServerRequestFunc;
            Func<JToken, VKRequest> _getSaveRequestFunc;

            Action<VKResponse> _successCallback;
            Action<VKError> _errorCallback;

            VKHttpOperation.FileParameter _file;

            public VKUploadMediaOperation(Func<VKRequest> getServerRequestFunc,
                Func<JToken, VKRequest> getSaveRequestFunc,
                Action<VKResponse> successCallback,
                Action<VKError> errorCallback,
                VKHttpOperation.FileParameter file)
            {
                _getSaveRequestFunc = getSaveRequestFunc;
                _getServerRequestFunc = getServerRequestFunc;

                _successCallback = successCallback;
                _errorCallback = errorCallback;

                _file = file;
            }

            public override void Start()
            {
                State = VKOperationState.Executing;

                var serverRequest = _getServerRequestFunc();

                serverRequest.Completed += (s, o) =>
                    {
                        try
                        {
                            var response = o.Response;

                            var httpRequest = WebRequest.CreateHttp(response.Content["response"].Value<string>("upload_url"));

                            var fileDict = new Dictionary<string, VKHttpOperation.FileParameter>();
                            fileDict["file1"] = _file;
                            var uploadFileOperation = new VKJsonOperation(httpRequest, fileDict);

                            uploadFileOperation.Completed += (sender, args) =>
                                {
                                    var saveRequest = _getSaveRequestFunc(args.JsonResponse);

                                    saveRequest.Completed += (saveSender, saveArgs) =>
                                        {

                                            State = VKOperationState.Finished;

                                            _successCallback(saveArgs.Response);
                                        };

                                    saveRequest.Error += (saveSender, saveArgs) =>
                                        {
                                            _errorCallback(saveArgs.Error);
                                        };


                                };

                            uploadFileOperation.Failed += (sender, args) =>
                                {
                                    _errorCallback(args.Error);
                                };

                            uploadFileOperation.Start();
                            _lastOperation = uploadFileOperation;


                        }
                        catch (Exception exc)
                        {
                            VKError error = new VKError(VKError.JSON_FAILED);
                            error.HttpError = exc;
                            error.ErrorMessage = exc.Message;
                            _errorCallback(error);
                        }

                        serverRequest.Error += (s1, o1) =>
                            {
                                _errorCallback(o1.Error);
                            };

                        _lastOperation = serverRequest.GetOperation();

                        _lastOperation.Start();

                    };
            }

            public override void Cancel()
            {
                if (_lastOperation != null)
                {
                    _lastOperation.Cancel();
                }

                base.Cancel();
            }

        }

        protected abstract VKRequest getServerRequest();

        protected abstract VKRequest getSaveRequest(JToken response);

        public override Networking.VKOperationBase GetOperation()
        {
            var file  = new VK.WindowsPhone.SDK.API.Networking.VKHttpOperation.FileParameter(new byte[0]);

            return new VKUploadMediaOperation(() => getServerRequest(), (r) => getSaveRequest(r), OnSuccess, OnError, file);
        }

        private void OnError(VKError obj)
        {
          
        }

        private void OnSuccess(VKResponse obj)
        {
            throw new NotImplementedException();
        }

        public VKUploadRequestBase()
            : base(null)
        {

        }

    }
}
