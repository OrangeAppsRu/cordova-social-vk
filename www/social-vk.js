function SocialVk() {
    // Does nothing
};
SocialVk.prototype.init = function(appId, successCallback, errorCallback) {
    cordova.exec(successCallback, errorCallback, "SocialVk", "initSocialVk", [appId]);
};

SocialVk.prototype.login = function(permissions, successCallback, errorCallback) {
    cordova.exec(successCallback, errorCallback, "SocialVk", "login", [permissions]);
};

SocialVk.prototype.share = function(sourceURL, description, imageURL, successCallback, errorCallback) {
    cordova.exec(successCallback, errorCallback, "SocialVk", "share", [sourceURL, description, imageURL]);
};

SocialVk.prototype.logout = function(successCallback, errorCallback) {
    cordova.exec(successCallback, errorCallback, "SocialVk", "logout", []);
};

// API methods

SocialVk.prototype.users_get = function(user_ids, fields, name_case,  successCallback, errorCallback) {
    cordova.exec(successCallback, errorCallback, "SocialVk", "users_get", [user_ids, fields, name_case]);
};

// query, sort, offset, count, fields, city, country, hometown, university_country, university, university_year, university_faculty, university_chair, sex, status, age_from, age_to, birth_day, birth_month, birth_year, online, has_photo, school_country, school_city, school_class, school, school_year, religion, interests, company, position, group_id, from_list
SocialVk.prototype.users_search = function(query_or_params,  successCallback, errorCallback) {
    cordova.exec(successCallback, errorCallback, "SocialVk", "users_search", [query_or_params]);
};

SocialVk.prototype.users_isAppUser = function(user_id, successCallback, errorCallback) {
    cordova.exec(successCallback, errorCallback, "SocialVk", "users_isAppUser", [user_id]);
};

SocialVk.prototype.users_getSubscriptions = function(user_id, extended, offset, count, fields, successCallback, errorCallback) {
    cordova.exec(successCallback, errorCallback, "SocialVk", "users_getSubscriptions", [user_id, extended, offset, count, fields]);
};

SocialVk.prototype.users_getFollowers = function(user_id, offset, count, fields, name_case, successCallback, errorCallback) {
    cordova.exec(successCallback, errorCallback, "SocialVk", "users_getFollowers", [user_id, offset, count, fields, name_case]);
};

SocialVk.prototype.wall_post = function(messageOrParams, successCallback, errorCallback) {
    cordova.exec(successCallback, errorCallback, "SocialVk", "wall_post", [messageOrParams]);
};

SocialVk.prototype.photos_getUploadServer = function(album_id, group_id, successCallback, errorCallback) {
    cordova.exec(successCallback, errorCallback, "SocialVk", "photos_getUploadServer", [album_id, group_id]);
};

SocialVk.prototype.photos_getWallUploadServer = function(group_id, successCallback, errorCallback) {
    cordova.exec(successCallback, errorCallback, "SocialVk", "photos_getWallUploadServer", [group_id]);
};

SocialVk.prototype.photos_saveWallPhoto = function(imageBase64, user_id, group_id, successCallback, errorCallback) {
    cordova.exec(successCallback, errorCallback, "SocialVk", "photos_saveWallPhoto", [imageBase64, user_id, group_id]);
};

SocialVk.prototype.photos_save = function(imageBase64, album_id, group_id, successCallback, errorCallback) {
    cordova.exec(successCallback, errorCallback, "SocialVk", "photos_save", [imageBase64, album_id, group_id]);
};

SocialVk.prototype.friends_get = function(user_id, order, count, offset, fields, name_case, successCallback, errorCallback) {
    cordova.exec(successCallback, errorCallback, "SocialVk", "friends_get", [user_id, order, count, offset, fields, name_case]);
}

SocialVk.prototype.friends_getOnline = function(user_id, order, count, offset, successCallback, errorCallback) {
    cordova.exec(successCallback, errorCallback, "SocialVk", "friends_getOnline", [user_id, order, count, offset]);
}

SocialVk.prototype.friends_getMutual = function(source_uid, target_uid, order, count, offset, successCallback, errorCallback) {
    cordova.exec(successCallback, errorCallback, "SocialVk", "friends_getMutual", [source_uid, target_uid, order, count, offset]);
}

SocialVk.prototype.friends_getRecent = function(count, successCallback, errorCallback) {
    cordova.exec(successCallback, errorCallback, "SocialVk", "friends_getRecent", [count]);
}

SocialVk.prototype.friends_getRequests = function(offset, count, extended, needs_mutual, out, sort, suggested, successCallback, errorCallback) {
    cordova.exec(successCallback, errorCallback, "SocialVk", "friends_getRequests", [offset, count, extended, needs_mutual, out, sort, suggested]);
}

SocialVk.prototype.callApiMethod = function(method, params, successCallback, errorCallback) {
    cordova.exec(successCallback, errorCallback, "SocialVk", "callApiMethod", [method, params]);
}

module.exports = new SocialVk();

