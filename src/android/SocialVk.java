package ru.trilan.socialvk;

import org.json.JSONException;
import org.json.JSONArray;
import org.json.JSONObject;

import org.apache.cordova.CallbackContext;
import org.apache.cordova.CordovaArgs;
import org.apache.cordova.CordovaPlugin;
import org.apache.cordova.PluginResult;

import android.graphics.Bitmap;
import android.graphics.BitmapFactory;
import android.content.Context;
import android.content.Intent;
import android.widget.Toast;
import android.util.Log;
import android.os.AsyncTask;
import android.app.AlertDialog;
import android.app.Activity;
import android.util.Base64;
import java.util.HashMap;
import java.util.Map;
import java.io.IOException;
import java.io.InputStream;
import java.net.URL;
import java.net.HttpURLConnection;

import com.vk.sdk.VKAccessToken;
import com.vk.sdk.VKSdk;
import com.vk.sdk.VKUIHelper;
import com.vk.sdk.VKCallback;
import com.vk.sdk.VKScope;
import com.vk.sdk.api.VKApi;
import com.vk.sdk.api.VKApiConst;
import com.vk.sdk.api.VKError;
import com.vk.sdk.api.VKRequest;
import com.vk.sdk.api.VKRequest.VKRequestListener;
import com.vk.sdk.api.VKParameters;
import com.vk.sdk.api.VKResponse;
import com.vk.sdk.dialogs.VKCaptchaDialog;
import com.vk.sdk.dialogs.VKShareDialog;
import com.vk.sdk.api.photo.VKUploadImage;
import com.vk.sdk.api.photo.VKImageParameters;
import com.vk.sdk.util.VKJsonHelper;

public class SocialVk extends CordovaPlugin {
    private static final String TAG = "SocialVk";
    private static final String ACTION_INIT = "initSocialVk";
    private static final String ACTION_LOGIN = "login";
    private static final String ACTION_LOGOUT = "logout";
    private static final String ACTION_SHARE = "share";
    private static final String ACTION_USERS_GET = "users_get";
    private static final String ACTION_USERS_SEARCH = "users_search";
    private static final String ACTION_USERS_IS_APP_USER = "users_isAppUser";
    private static final String ACTION_USERS_GET_SUBSCRIPTIONS = "users_getSubscriptions";
    private static final String ACTION_USERS_GET_FOLLOWERS = "users_getFollowers";
    private static final String ACTION_WALL_POST = "wall_post";
    private static final String ACTION_PHOTOS_GET_UPLOAD_SERVER = "photos_getUploadServer";
    private static final String ACTION_PHOTOS_GET_WALL_UPLOAD_SERVER = "photos_getWallUploadServer";
    private static final String ACTION_PHOTOS_SAVE_WALL_PHOTO = "photos_saveWallPhoto";
    private static final String ACTION_PHOTOS_SAVE = "photos_save";
    private static final String ACTION_FRIENDS_GET = "friends_get";
    private static final String ACTION_FRIENDS_GET_ONLINE = "friends_getOnline";
    private static final String ACTION_FRIENDS_GET_MUTUAL = "friends_getMutual";
    private static final String ACTION_FRIENDS_GET_RECENT = "friends_getRecent";
    private static final String ACTION_FRIENDS_GET_REQUESTS = "friends_getRequests";
    private static final String ACTION_CALL_API_METHOD = "callApiMethod";
    private CallbackContext _callbackContext;

    private String savedUrl = null;
    private String savedComment = null;
    private String savedImageUrl = null;
    final String sTokenKey = "VK_ACCESS_TOKEN";

    /**
     * Gets the application context from cordova's main activity.
     * @return the application context
     */
    private Context getApplicationContext() {
        return this.getActivity().getApplicationContext();
    }

    private Activity getActivity() {
        return (Activity)this.webView.getContext();
    }

    private void success() {
        if(_callbackContext != null) {
            _callbackContext.sendPluginResult(new PluginResult(PluginResult.Status.OK));
            _callbackContext.success();
        }
    }
    private void fail() {
        if(_callbackContext != null) {
            _callbackContext.sendPluginResult(new PluginResult(PluginResult.Status.ERROR));
            _callbackContext.error("Error");
        }
    }

    @Override
    public boolean execute(String action, CordovaArgs args, final CallbackContext callbackContext) throws JSONException {
        this._callbackContext = callbackContext;
        if(ACTION_INIT.equals(action)) {
            return init(args.getString(0));
        } else if (ACTION_LOGIN.equals(action)) {
            JSONArray permissions = args.getJSONArray(0);
            String[] perms = new String[permissions.length()];
            for(int i=0; i<permissions.length(); i++) {
                perms[i] = permissions.getString(i);
            }
            return login(perms);
        } else if(ACTION_LOGOUT.equals(action)) {
            VKSdk.logout();
            success();
            return true;
        } else if (ACTION_SHARE.equals(action)) {
            return shareOrLogin(args.getString(0), args.getString(1), args.getString(2));
        } else if (ACTION_USERS_GET.equals(action)) {
            HashMap<String, Object> params = new HashMap<String, Object>();
            params.put("user_ids", args.getString(0));
            params.put("fields", args.getString(1));
            params.put("name_case", args.getString(2));
            return usersGet(params, callbackContext);
        } else if (ACTION_USERS_SEARCH.equals(action)) {
            String q = args.optString(0);
            JSONObject params = args.optJSONObject(0);
            if(params != null)
            {
                return usersSearch(VKJsonHelper.toMap(params), callbackContext);
            } 
            else if(q != null)
            {
                HashMap<String, Object> paramsMap = new HashMap<String, Object>();
                paramsMap.put("q", q);
                return usersSearch(paramsMap, callbackContext);
            }
            else 
            {
                fail();
                return false;
            }
        } else if (ACTION_USERS_IS_APP_USER.equals(action)) {
            return usersIsAppUser(args.getInt(0), callbackContext);
        } else if (ACTION_USERS_GET_SUBSCRIPTIONS.equals(action)) {
            HashMap<String, Object> params = new HashMap<String, Object>();
            params.put("user_id", args.getInt(0));
            params.put("extended", args.getInt(1));
            params.put("offset", args.getInt(2));
            params.put("count", args.getInt(3));
            params.put("fields", args.getString(4));
            return usersGetSubscriptions(params, callbackContext);
        } else if (ACTION_USERS_GET_FOLLOWERS.equals(action)) {
            HashMap<String, Object> params = new HashMap<String, Object>();
            params.put("user_id", args.getInt(0));
            params.put("offset", args.getInt(1));
            params.put("count", args.getInt(2));
            params.put("fields", args.getString(3));
            params.put("name_case", args.getString(4));
            return usersGetFollowers(params, callbackContext);
        } else if (ACTION_WALL_POST.equals(action)) {
            String message = args.optString(0);
            JSONObject params = args.optJSONObject(0);
            if(params != null)
            {
                return wallPost(VKJsonHelper.toMap(params), callbackContext);
            } 
            else if (message != null)
            {
                HashMap<String, Object> paramsMap = new HashMap<String, Object>();
                paramsMap.put("message", message);
                return wallPost(paramsMap, callbackContext);
            }
            else
            {
                fail();
                return false;
            }
        } else if (ACTION_PHOTOS_GET_UPLOAD_SERVER.equals(action)) {
            int album_id = args.getInt(0);
            int group_id = args.getInt(1);
            return photos_getUploadServer(album_id, group_id, callbackContext);
        } else if (ACTION_PHOTOS_GET_WALL_UPLOAD_SERVER.equals(action)) {
            int group_id = args.getInt(0);
            return photos_getWallUploadServer(group_id, callbackContext);
        } else if (ACTION_PHOTOS_SAVE_WALL_PHOTO.equals(action)) {
            String imageBase64 = args.getString(0);
            int user_id = args.getInt(1);
            int group_id = args.getInt(2);
            return photos_saveWallPhoto(imageBase64, user_id, group_id, callbackContext);
        } else if (ACTION_PHOTOS_SAVE.equals(action)) {
            String imageBase64 = args.getString(0);
            int album_id = args.getInt(1);
            int group_id = args.getInt(2);
            return photos_save(imageBase64, album_id, group_id, callbackContext);
        } else if (ACTION_FRIENDS_GET.equals(action)) {
            int user_id = args.getInt(0);
            String order = args.getString(1);
            int count = args.getInt(2);
            int offset = args.getInt(3);
            String fields = args.getString(4);
            String name_case = args.getString(5);
            return friends_get(user_id, order, count, offset, fields, name_case, callbackContext);
        } else if (ACTION_FRIENDS_GET_ONLINE.equals(action)) {
            int user_id = args.getInt(0);
            String order = args.getString(1);
            int count = args.getInt(2);
            int offset = args.getInt(3);
            return friends_getOnline(user_id, order, count, offset, callbackContext);
        } else if (ACTION_FRIENDS_GET_MUTUAL.equals(action)) {
            int user_id = args.getInt(0);
            int target_id = args.getInt(1);
            String order = args.getString(2);
            int count = args.getInt(3);
            int offset = args.getInt(4);
            return friends_getMutual(user_id, target_id, order, count, offset, callbackContext);
        } else if (ACTION_FRIENDS_GET_RECENT.equals(action)) {
            int count = args.getInt(0);
            return friends_getRecent(count, callbackContext);
        } else if (ACTION_FRIENDS_GET_REQUESTS.equals(action)) {
            int offset = args.getInt(0);
            int count = args.getInt(1);
            int extended = args.getInt(2);
            int needs_mutual = args.getInt(3);
            int out = args.getInt(4);
            int sort = args.getInt(5);
            int suggested = args.getInt(6);
            return friends_getRequests(offset, count, extended, needs_mutual, out, sort, suggested, callbackContext);
        } else if (ACTION_CALL_API_METHOD.equals(action)) {
            String method = args.getString(0);
            JSONObject params = args.getJSONObject(1);
            return callApiMethod(method, params, callbackContext);
        }
        Log.e(TAG, "Unknown action: "+action);
        _callbackContext.sendPluginResult(new PluginResult(PluginResult.Status.ERROR, "Unimplemented method: "+action));
        _callbackContext.error("Unimplemented method: "+action);
        return true;
    }

    private boolean init(String appId)
    {
        this.cordova.setActivityResultCallback(this);
        Log.i(TAG, "VK initialize");
        VKSdk.initialize(getApplicationContext());
        if(_callbackContext != null) {
            _callbackContext.sendPluginResult(new PluginResult(PluginResult.Status.OK));
            _callbackContext.success();
        }
        return true;
    }

    private boolean login(String[] permissions)
    {
        VKSdk.login(getActivity(), permissions);
        return true;
    }

    private boolean shareOrLogin(final String url, final String comment, final String imageUrl)
    {
        this.cordova.setActivityResultCallback(this);
        final String[] scope = new String[]{VKScope.WALL, VKScope.PHOTOS};
        if(!VKSdk.isLoggedIn()) {
            savedUrl = url;
            savedComment = comment;
            savedImageUrl = imageUrl;
            VKSdk.login(getActivity(), scope);
        } else {
            share(url, comment, imageUrl);
        }
        return true;
    }

    private boolean share(final String url, final String comment, final String imageUrl)
    {
        if(url == null || comment == null) return false;
    
        new AsyncTask<String, Void, String>() {
            private Bitmap image = null;
            @Override protected String doInBackground(String... args) {
                if(imageUrl != null)
                    image = getBitmapFromURL(imageUrl);
                return "";
            }
            @Override protected void onPostExecute(String result) {
                VKShareDialog vsh = new VKShareDialog()
                    .setText(comment)
                    .setAttachmentLink("", url)
                    .setShareDialogListener(new VKShareDialog.VKShareDialogListener() {
                            public void onVkShareComplete(int postId) {
                                Log.i(TAG, "VK sharing complete");
                            }
                            public void onVkShareCancel() {
                                Log.i(TAG, "VK sharing cancelled");
                            }
                            public void onVkShareError(VKError err) {
                                Log.e(TAG, err.toString());
                            }
                        });
                if(image != null) {
                    vsh.setAttachmentImages(new VKUploadImage[]{
                            new VKUploadImage(image, VKImageParameters.pngImage())
                        });
                }
                vsh.show(getActivity().getFragmentManager().beginTransaction(), "VK_SHARE_DIALOG");
            }
        }.execute();

        savedUrl = null;
        savedComment = null;
        savedImageUrl = null;
        return true;
    }

    @Override public void onActivityResult(int requestCode, int resultCode, Intent data)
    {
        if(resultCode == Activity.RESULT_CANCELED && data == null) {
            // switch to another activity result callback
            super.onActivityResult(requestCode, resultCode, data);
            return;
        }
        Log.i(TAG, "onActivityResult(" + requestCode + "," + resultCode + "," + data);
        if(!VKSdk.onActivityResult(requestCode, resultCode, data, new VKCallback<VKAccessToken>() {
                @Override
                public void onResult(VKAccessToken res) {
                    // User passed Authorization
                    final String token = res.accessToken;
                    final String email = res.email;
                    Log.i(TAG, "VK new token: "+token);
                    res.saveTokenToSharedPreferences(getApplicationContext(), sTokenKey);
                    VKRequest request = VKApi.users().get(VKParameters.from(VKApiConst.FIELDS, "id, nickname, first_name, last_name, sex, bdate, timezone, photo, photo_big, city, country"));
                    request.executeWithListener(new VKRequestListener() {
                            @Override
                            public void onComplete(VKResponse response) {
                                try {
                                    JSONObject loginDetails = new JSONObject();
                                    loginDetails.put("token", token);
                                    loginDetails.put("email", email);
                                    loginDetails.put("user", response.json.getJSONArray("response"));
                                    if(_callbackContext != null) {
                                        _callbackContext.sendPluginResult(new PluginResult(PluginResult.Status.OK, loginDetails.toString()));
                                        _callbackContext.success();
                                    }
                                } catch (JSONException exception) {
                                    Log.e(TAG, "JSON error:", exception);
                                    fail();
                                }
                            }
                            @Override
                            public void onError(VKError error) {
                                String err = error.toString();
                                Log.e(TAG, err);
                                if(_callbackContext != null) {
                                    _callbackContext.sendPluginResult(new PluginResult(PluginResult.Status.ERROR, err));
                                    _callbackContext.error(error.errorMessage);
                                }
                            }
                        });
                    //share(savedUrl, savedComment, savedImageUrl);
                }

                @Override
                public void onError(VKError error) {
                    // User didn't pass Authorization
                    String err = error.toString();
                    Log.e(TAG, "VK Authorization error! "+err);
                    //new AlertDialog.Builder(getApplicationContext()).setMessage(error.errorMessage).show();
                    if(_callbackContext != null) {
                        _callbackContext.sendPluginResult(new PluginResult(PluginResult.Status.ERROR, err));
                        _callbackContext.error(error.errorMessage);
                    }
                }
            })) {
            super.onActivityResult(requestCode, resultCode, data);
        }
    }
  
    public static Bitmap getBitmapFromURL(String src) {
        try {
            URL url = new URL(src);
            HttpURLConnection connection = (HttpURLConnection) url.openConnection();
            connection.setDoInput(true);
            connection.connect();
            InputStream input = connection.getInputStream();
            Bitmap myBitmap = BitmapFactory.decodeStream(input);
            if(myBitmap == null) {
                Log.e(TAG, "Can't load image from "+src);
            }
            return myBitmap;
        } catch (IOException e) {
            // Log exception
            Log.e(TAG, "Can't fetch image from url "+src+": "+e);
            return null;
        }
    }

    private boolean usersGet(Map<String, Object> params, CallbackContext context) {
        try {
            VKRequest req = VKApi.users().get(new VKParameters(params));
            performRequest(req, context);
            return true;
        } catch(Exception ex) {
            return false;
        }
    }

    private boolean usersSearch(Map<String, Object> params, CallbackContext context) {
        try {
            VKRequest req = VKApi.users().search(new VKParameters(params));
            performRequest(req, context);
            return true;
        } catch(Exception ex) {
            return false;
        }
    }

    private boolean usersIsAppUser(int user_id, CallbackContext context) {
        try {
            VKRequest req = VKApi.users().isAppUser(user_id);
            performRequest(req, context);
            return true;
        } catch(Exception ex) {
            return false;
        }
    }

    private boolean usersGetSubscriptions(Map<String, Object> params, CallbackContext context) {
        try {
            VKRequest req = VKApi.users().getSubscriptions(new VKParameters(params));
            performRequest(req, context);
            return true;
        } catch(Exception ex) {
            return false;
        }
    }

    private boolean usersGetFollowers(Map<String, Object> params, CallbackContext context) {
        try {
            VKRequest req = VKApi.users().getFollowers(new VKParameters(params));
            performRequest(req, context);
            return true;
        } catch(Exception ex) {
            return false;
        }
    }

    private boolean wallPost(Map<String, Object> params, CallbackContext context) {
        try {
            VKRequest req = VKApi.wall().post(new VKParameters(params));
            performRequest(req, context);
            return true;
        } catch(Exception ex) {
            return false;
        }
    }

    private boolean photos_getUploadServer(int album_id, int group_id, CallbackContext context) {
        try {
            VKRequest req = VKApi.photos().getUploadServer(album_id, group_id);
            performRequest(req, context);
            return true;
        } catch(Exception ex) {
            return false;
        }
    }

    private boolean photos_getWallUploadServer(int group_id, CallbackContext context) {
        try {
            VKRequest req = VKApi.photos().getWallUploadServer(group_id);
            performRequest(req, context);
            return true;
        } catch(Exception ex) {
            return false;
        }
    }

    private boolean photos_saveWallPhoto(String imageBase64, int user_id, int group_id, CallbackContext context) {
        try {
            VKRequest req = VKApi.uploadWallPhotoRequest(new VKUploadImage(Base64ToBitmap(imageBase64), VKImageParameters.pngImage()), user_id, group_id);
            performRequest(req, context);
            return true;
        } catch(Exception ex) {
            return false;
        }
    }

    private boolean photos_save(String imageBase64, int album_id, int group_id, CallbackContext context) {
        try {
            VKRequest req = VKApi.uploadAlbumPhotoRequest(new VKUploadImage(Base64ToBitmap(imageBase64), VKImageParameters.pngImage()), album_id, group_id);
            performRequest(req, context);
            return true;
        } catch(Exception ex) {
            return false;
        }
    }

    private boolean friends_get(int user_id, String order, int count, int offset, String fields, String name_case, CallbackContext context) {
        try {
            HashMap<String, Object> params = new HashMap<String, Object>();
            params.put("user_id", user_id);
            params.put("order", order);
            params.put("count", count);
            params.put("offset", offset);
            params.put("fields", fields);
            params.put("name_case", name_case);
            VKRequest req = VKApi.friends().get(new VKParameters(params));
            performRequest(req, context);
            return true;
        } catch(Exception ex) {
            return false;
        }
    }

    private boolean friends_getOnline(int user_id, String order, int count, int offset, CallbackContext context) {
        try {
            HashMap<String, Object> params = new HashMap<String, Object>();
            params.put("user_id", user_id);
            params.put("order", order);
            params.put("count", count);
            params.put("offset", offset);
            VKRequest req = VKApi.friends().getOnline(new VKParameters(params));
            performRequest(req, context);
            return true;
        } catch(Exception ex) {
            return false;
        }
    }

    private boolean friends_getMutual(int source_id, int target_id, String order, int count, int offset, CallbackContext context) {
        try {
            HashMap<String, Object> params = new HashMap<String, Object>();
            params.put("source_id", source_id);
            params.put("target_id", target_id);
            params.put("order", order);
            params.put("count", count);
            params.put("offset", offset);
            VKRequest req = VKApi.friends().getMutual(new VKParameters(params));
            performRequest(req, context);
            return true;
        } catch(Exception ex) {
            return false;
        }
    }

    private boolean friends_getRecent(int count, CallbackContext context) {
        try {
            HashMap<String, Object> params = new HashMap<String, Object>();
            params.put("count", count);
            VKRequest req = VKApi.friends().getRecent(new VKParameters(params));
            performRequest(req, context);
            return true;
        } catch(Exception ex) {
            return false;
        }
    }

    private boolean friends_getRequests(int offset, int count, int extended, int needs_mutual, int out, int sort, int suggested, CallbackContext context) {
        try {
            HashMap<String, Object> params = new HashMap<String, Object>();
            params.put("offset", offset);
            params.put("count", count);
            params.put("extended", extended);
            params.put("needs_mutual", needs_mutual);
            params.put("out", out);
            params.put("sort", sort);
            params.put("suggested", suggested);
            VKRequest req = VKApi.friends().getRequests(new VKParameters(params));
            performRequest(req, context);
            return true;
        } catch(Exception ex) {
            return false;
        }
    }

    private boolean callApiMethod(String method, JSONObject params, CallbackContext context) {
        try {
            VKRequest req = new VKRequest(method, new VKParameters(VKJsonHelper.toMap(params)));
            performRequest(req, context);
            return true;
        } catch(Exception ex) {
            return false;
        }
    }

    private void performRequest(VKRequest request, final CallbackContext context) {
        request.executeWithListener(new VKRequestListener() {
                @Override
                public void onComplete(VKResponse response) {
                    try {
                        String result;
                        JSONObject o = response.json;
                        result = o.get("response").toString();
                        /*
                          if(o.optString("response")) result = o.getString("response");
                          else if(o.optInt("response")) result = o.getInt("response").toString();
                          else if(o.optJSONObject("response")) result = o.getJSONObject("response").toString();
                          else if(o.optJSONArray("response")) result = o.getJSONArray("response").toString();
                        */
                        context.sendPluginResult(new PluginResult(PluginResult.Status.OK, result));
                        context.success();
                    } catch (JSONException e) {
                        Log.e(TAG, "JSON exception:", e);
                        context.sendPluginResult(new PluginResult(PluginResult.Status.ERROR));
                        context.error("Error");
                    }
                }
                @Override
                public void onError(VKError error) {
                    String err = error.toString();
                    Log.e(TAG, err);
                    context.sendPluginResult(new PluginResult(PluginResult.Status.ERROR, err));
                    context.error(err);
                }
                @Override
                public void onProgress(VKRequest.VKProgressType progressType,
                                       long bytesLoaded,
                                       long bytesTotal)
                {
                    //I don't really believe in progress
                }
                @Override
                public void attemptFailed(VKRequest request, int attemptNumber, int totalAttempts) {
                    //More luck next time
                }
            });
    }

    Bitmap Base64ToBitmap(String myImageData)
    {
        byte[] imageAsBytes = Base64.decode(myImageData.getBytes(),Base64.DEFAULT);
        return BitmapFactory.decodeByteArray(imageAsBytes, 0, imageAsBytes.length);
    }
}
