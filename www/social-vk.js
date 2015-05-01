function SocialVk() {
    // Does nothing
};
SocialVk.prototype.init = function(appId, successCallback, errorCallback) {
    cordova.exec(successCallback, errorCallback, "SocialVk", "initSocialVk", [appId]);
};

SocialVk.prototype.login = function(successCallback, errorCallback) {
    cordova.exec(successCallback, errorCallback, "SocialVk", "login");
};

SocialVk.prototype.share = function(sourceURL, description, imageURL, successCallback, errorCallback) {
    cordova.exec(successCallback, errorCallback, "SocialVk", "share", [sourceURL, description, imageURL]);
};
module.exports = new SocialVk();

