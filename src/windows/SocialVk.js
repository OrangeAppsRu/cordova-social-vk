module.exports = {

    init: function (appId, success, error) {
        self = this;
        this.vkobj = new Social.SocialVk();
        this.vkobj.addEventListener("callback", function (arg) {
            console.log("Callback from SocialVk.", arg);
            cb = self.cbmap[arg.callbackid];
            delete self.cbmap[arg.callbackid];
            if (cb != null) {
                if (arg.error.length > 0) {
                    cb.error(arg.error);
                } else {
                    cb.success(arg.result);
                }
            }
        });
        this.cbmap = {};
        this.nextCbId = 1;
        this.pushCallback = function (success, error) {
            id = this.nextCbId;
            this.nextCbId += 1;
            this.cbmap[id] = { "success": success, "error": error };
            return id;
        }
        this.vkobj.init(appId, this.pushCallback(success, error));
    },
    login: function (permissions, success, error) {
        this.vkobj.login(JSON.stringify(permissions), this.pushCallback(success, error));
    },
    share: function(sourceUrl, description, imageUrl, success, error) {
        this.vkobj.share(sourceUrl, description, imageUrl, this.pushCallback(success, error));
    },
    logout: function(success, error) {
        this.vkobj.logout(this.pushCallback(success, error));
    },
    // API methods
    users_get: function (user_ids, fields, name_case, success, error) {
        this.vkobj.users_get(user_ids, fields, name_case, this.pushCallback(success, error));
    },
    users_search: function (query_or_params, success, error) {
        this.vkobj.users_search(JSON.stringify(query_or_params), this.pushCallback(success, error));
    },
    users_isAppUser: function (user_id, success, error) {
        this.vkobj.users_isAppUser(user_id, this.pushCallback(success, error));
    },
    users_getSubscriptions: function (user_id, extended, offset, count, fields, success, error) {
        this.vkobj.users_getSubscriptions(user_id, extended, offset, count, fields, this.pushCallback(success, error));
    },
    users_getFollowers: function (user_id, offset, count, fields, name_case, success, error) {
        this.vkobj.users_getFollowers(user_id, offset, count, fields, name_case, this.pushCallback(success, error));
    },
    wall_post: function (messageOrParams, success, error) {
        error('Method not implemented yet');
    },
    photos_getUploadServer: function (album_id, group_id, success, error) {
        this.vkobj.photos_getUploadServer(album_id, group_id, this.pushCallback(success, error));
    },
    photos_getWallUploadServer: function (group_id, success, error) {
        this.vkobj.photos_getWallUploadServer(group_id, this.pushCallback(success, error));
    },
    photos_saveWallPhoto: function (imageBase64, user_id, group_id, success, error) {
        this.vkobj.photos_saveWallPhoto(imageBase64, user_id, group_id, this.pushCallback(success, error));
    },
    photos_save: function (imageBase64, album_id, group_id, success, error) {
        this.vkobj.photos_save(imageBase64, album_id, group_id, this.pushCallback(success, error));
    },
    friends_get: function (user_id, order, count, offset, fields, name_case, success, error) {
        this.vkobj.friends_get(user_id, order, count, offset, fields, name_case, this.pushCallback(success, error));
    },
    friends_getOnline: function (user_id, order, count, offset, success, error) {
        this.vkobj.friends_getOnline(user_id, order, count, offset, this.pushCallback(success, error));
    },
    friends_getMutual: function (source_uid, target_uid, order, count, offset, success, error) {
        this.vkobj.friends_getMutual(source_uid, target_uid, order, count, offset, this.pushCallback(success, error));
    },
    friends_getRecent: function (count, success, error) {
        this.vkobj.friends_getRecent(count, this.pushCallback(success, error));
    },
    friends_getRequests: function (offset, count, extended, needs_mutual, out, sort, suggested, success, error) {
        this.vkobj.friends_getRequests(offset, count, extended, needs_mutual, out, sort, suggested, this.pushCallback(success, error));
    },
    callApiMethod: function (method, params, success, error) {
        this.vkobj.callApiMethod(method, JSON.stringify(params), this.pushCallback(success, error));
    },
}
