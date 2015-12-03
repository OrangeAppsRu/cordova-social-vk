using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VK.WindowsPhone.SDK.Util;

namespace VK.WindowsPhone.SDK.API
{

    public class VKExecuteRequest : VKRequest
    {
        public VKExecuteRequest(string executeCode)
            : base("execute", "code", executeCode)
        {
        }
    }
     
    public class VKRequest
    {
        class ErrorRoot
        {
            public VKError error { get; set; }
        }

        private VKRequestParameters _parameters;
      
        private static readonly string REQUEST_BASE_URI_FRM = "https://api.vk.com/method/{0}";

        private static readonly string ERROR_PREFIX_GENERAL = @"{""error"":{";

        private static IVKLogger Logger
        {
            get { return VKSDK.Logger; }
        }


     
        public static VKRequest Dispatch<T>(VKRequestParameters parameters,
            Action<VKBackendResult<T>> callback,
            Func<string, T> customDeserializationFunc = null)
        {
            var request = new VKRequest(parameters);

            request.Dispatch<T>(callback, customDeserializationFunc);

            return request;
        }

        public VKRequest(VKRequestParameters parameters)
        {
            InitializeWith(parameters);
        }       

        public VKRequest(string methodName, params string[] parameters)
        {
            var vkParameters = new VKRequestParameters(methodName, parameters);
            InitializeWith(vkParameters);
        }

        private void InitializeWith(VKRequestParameters parameters)
        {
            if (parameters == null)
            {
                throw new ArgumentNullException("parameters");
            }

            _parameters = parameters;
        }

        public Task<VKBackendResult<T>> DispatchAsync<T>(Func<string, T> customDeserializationFunc = null)
        {
                var tc = new TaskCompletionSource<VKBackendResult<T>>();

                Dispatch(
                    (res) =>
                    {
                        tc.TrySetResult(res);
                    },

                    customDeserializationFunc);

                return tc.Task;
        }

        public void Dispatch<T>(
            Action<VKBackendResult<T>> callback,
            Func<string, T> customDeserializationFunc = null)
        {
            if (callback == null)
            {
                throw new ArgumentNullException("callback");
            }

            Dictionary<string, string> parametersDict = null;
            if (_parameters.Parameters != null)
            {
                parametersDict = new Dictionary<string, string>(_parameters.Parameters);
            }
            else
            {
                parametersDict = new Dictionary<string, string>();
            }

            DoDispatch<T>(
                parametersDict,
                callback,
                customDeserializationFunc);
        }

        private void DoDispatch<T>(
            Dictionary<string, string> parametersDict,
            Action<VKBackendResult<T>> callback,
            Func<string, T> customDeserializationFunc = null)
        {

            if (!parametersDict.ContainsKey("v"))
            {
                parametersDict["v"] = VKSDK.API_VERSION;
            }

            var accessToken = VKSDK.GetAccessToken();

            if (accessToken != null)
            {
                parametersDict["access_token"] = accessToken.AccessToken;
            }

            var dispatchUri = string.Format(REQUEST_BASE_URI_FRM, _parameters.MethodName);

            VKHttpRequestHelper.DispatchHTTPRequest(
                dispatchUri,
                parametersDict,
                (httpResult) =>
                {
                    if (httpResult.IsSucceeded)
                    {
                        var backendResult = GetBackendResultFromString<T>(httpResult.Data, customDeserializationFunc);

                        if (backendResult.ResultCode == VKResultCode.UserAuthorizationFailed &&
                            accessToken != null)
                        {
                            VKSDK.SetAccessTokenError(new VKError { error_code = (int)VKResultCode.UserAuthorizationFailed });
                        }                                              
                        else if (backendResult.ResultCode == VKResultCode.CaptchaRequired)
                        {
                            var captchaRequest = new VKCaptchaUserRequest
                            {
                                CaptchaSid = backendResult.Error.captcha_sid,
                                Url = backendResult.Error.captcha_img
                            };

                            VKSDK.InvokeCaptchaRequest(captchaRequest,
                                (captchaResponse) =>
                                {
                                    if (!captchaResponse.IsCancelled)
                                    {
                                        var parametersWithCaptcha = new Dictionary<string, string>(parametersDict);

                                        parametersWithCaptcha["captcha_sid"] = captchaResponse.Request.CaptchaSid;
                                        parametersWithCaptcha["captcha_key"] = captchaResponse.EnteredString;

                                        DoDispatch(parametersWithCaptcha,
                                            callback,
                                            customDeserializationFunc);
                                    }
                                    else
                                    {
                                        InvokeSafely(() => callback(new VKBackendResult<T>() { ResultCode = VKResultCode.CaptchaControlCancelled }));                                        
                                    }
                                });

                        }
                        else if (backendResult.ResultCode == VKResultCode.ValidationRequired)
                        {
                            var validationRequest = new VKValidationRequest
                            {
                                ValidationUri = backendResult.Error.redirect_uri
                            };

                            VKSDK.InvokeValidationRequest(validationRequest,
                                (vr) =>
                                {
                                    if (vr.IsSucceeded)
                                    {
                                        DoDispatch(parametersDict, callback, customDeserializationFunc);
                                    }
                                    else
                                    {
                                        InvokeSafely(() => callback(new VKBackendResult<T> { ResultCode = VKResultCode.ValidationCanceslledOrFailed }));
                                    }
                                });
                        }
                        else
                        {
                            InvokeSafely(() => callback(backendResult));
                        }
                    }
                    else
                    {
                        var backendResult = new VKBackendResult<T> { ResultCode = VKResultCode.CommunicationFailed };
                        InvokeSafely(() => callback(backendResult));
                    }
                });
        }

        private void InvokeSafely(Action action)
        {
            try
            {
                action();
            }
            catch (Exception)
            {
            }
        }

        private VKBackendResult<T> GetBackendResultFromString<T>(string dataString, Func<string, T> customDeserializationFunc)
        {                    
            var result = new VKBackendResult<T>();
            result.ResultString = dataString;

            if (dataString.StartsWith(ERROR_PREFIX_GENERAL))
            {
                try
                {
                    result.Error = JsonConvert.DeserializeObject<ErrorRoot>(dataString).error;

                    VKResultCode resultCode = VKResultCode.UnknownError;

                    if (Enum.TryParse<VKResultCode>(result.Error.error_code.ToString(), out resultCode))
                    {
                        result.ResultCode = resultCode;
                    }
                }
                catch (Exception)
                {
                    result.ResultCode = VKResultCode.UnknownError;
                }
            }
            else
            {
                result.ResultCode = VKResultCode.Succeeded;
             
                if (customDeserializationFunc != null)
                {
                    try
                    {
                        var data = customDeserializationFunc(dataString);
                        result.Data = data;
                    }
                    catch (Exception exc)
                    {
                        Logger.Error("VKRequest custom deserialization function failed on dataString=" + dataString, exc);
                        result.ResultCode = VKResultCode.DeserializationError;
                    }
                }
                else
                {
                    try
                    {
                        var data = JsonConvert.DeserializeObject<GenericRoot<T>>(dataString).response;
                        result.Data = data;
                    }
                    catch (Exception exc)
                    {
                        Logger.Error("VKRequest deserialization failed on dataString=" + dataString, exc);
                        result.ResultCode = VKResultCode.DeserializationError;
                    }
                }
            }
            return result;
        }
    }

    public class GenericRoot<T>
    {
        public T response { get; set; }
    }

}
