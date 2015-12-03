using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VK.WindowsPhone.SDK.API
{

    public enum VKResultCode
    {
        // didn't get response at all
        CommunicationFailed = -1,

        Succeeded = 0,

        // error codes from vk https://vk.com/dev/errors
        UnknownError = 1,
        AppDisabled = 2,
        UnknownMethod = 3,
        IncorrectSignature = 4,
        UserAuthorizationFailed = 5,
        TooManyRequestsPerSecond = 6,
        NotAllowed = 7,
        WrongSyntax = 8,
        FloodControlEnabled = 9,
        InternalServerError = 10,
        CaptchaRequired = 14,
        AccessDenied = 15,
        HttpsRequired = 16,
        ValidationRequired = 17,

        WrongParameter = 100,

        InvalidUserId = 113,

        AlbumAccessDenied = 200,
        AudioAccessDenied = 201,
        GroupAccessDenied = 203,
        AlbumLimitReached = 300,

        CaptchaControlCancelled = 100002,
        ValidationCanceslledOrFailed = 100003,
        DeserializationError = -10000,
        InvalidToken = -10001
    }
}
