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
        return this.webView.getContext();
    }

    private Activity getActivity() {
        return (Activity)this.webView.getContext();
    }

    private void success() {
        _callbackContext.sendPluginResult(new PluginResult(PluginResult.Status.OK));
        _callbackContext.success();
    }
    private void fail() {
        _callbackContext.sendPluginResult(new PluginResult(PluginResult.Status.ERROR));
        _callbackContext.error("Error");
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
        } else if (ACTION_SHARE.equals(action)) {
            return shareOrLogin(args.getString(0), args.getString(1), args.getString(2));
        } else if (ACTION_USERS_GET.equals(action)) {
        } else if (ACTION_USERS_SEARCH.equals(action)) {
        } else if (ACTION_USERS_IS_APP_USER.equals(action)) {
        } else if (ACTION_USERS_GET_SUBSCRIPTIONS.equals(action)) {
        } else if (ACTION_USERS_GET_FOLLOWERS.equals(action)) {
        } else if (ACTION_WALL_POST.equals(action)) {
        } else if (ACTION_PHOTOS_GET_UPLOAD_SERVER.equals(action)) {
        } else if (ACTION_PHOTOS_GET_WALL_UPLOAD_SERVER.equals(action)) {
        } else if (ACTION_PHOTOS_SAVE_WALL_PHOTO.equals(action)) {
        } else if (ACTION_PHOTOS_SAVE.equals(action)) {
        } else if (ACTION_FRIENDS_GET.equals(action)) {
        } else if (ACTION_FRIENDS_GET_ONLINE.equals(action)) {
        } else if (ACTION_FRIENDS_GET_MUTUAL.equals(action)) {
        } else if (ACTION_FRIENDS_GET_RECENT.equals(action)) {
        } else if (ACTION_FRIENDS_GET_REQUESTS.equals(action)) {
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

        _callbackContext.sendPluginResult(new PluginResult(PluginResult.Status.OK));
        _callbackContext.success();
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
        Log.i(TAG, "onActivityResult(" + requestCode + "," + resultCode + "," + data);
        super.onActivityResult(requestCode, resultCode, data);
        if(resultCode != 0) {
            VKCallback<VKAccessToken> callback = new VKCallback<VKAccessToken>() {
                    @Override
                    public void onResult(VKAccessToken res) {
                        // User passed Authorization
                        Log.i(TAG, "VK new token: "+res.accessToken);
                        res.saveTokenToSharedPreferences(getApplicationContext(), sTokenKey);
                        success();
                        share(savedUrl, savedComment, savedImageUrl);
                    }

                    @Override
                    public void onError(VKError error) {
                        // User didn't pass Authorization
                        new AlertDialog.Builder(getApplicationContext()).setMessage(error.errorMessage).show();
                        Log.w(TAG, "VK Authorization error! "+error.errorMessage);
                        fail();
                    }
                };
            VKSdk.onActivityResult(requestCode, resultCode, data, callback);
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
                    context.sendPluginResult(new PluginResult(PluginResult.Status.OK, response.responseString));
                    context.success();
                }
                @Override
                public void onError(VKError error) {
                    Log.e(TAG, error.errorMessage + error.errorReason);
                    context.sendPluginResult(new PluginResult(PluginResult.Status.ERROR, error.errorMessage));
                    context.error(error.errorMessage);
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
}
