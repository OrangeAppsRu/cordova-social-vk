package ru.trilan.socialvk;

import org.json.JSONException;

import org.apache.cordova.CallbackContext;
import org.apache.cordova.CordovaArgs;
import org.apache.cordova.CordovaPlugin;
import org.apache.cordova.PluginResult;

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

import com.vk.sdk.VKAccessToken;
import com.vk.sdk.VKSdk;
import com.vk.sdk.VKSdkListener;
import com.vk.sdk.VKUIHelper;
import com.vk.sdk.api.VKError;
import com.vk.sdk.dialogs.VKCaptchaDialog;
import com.vk.sdk.VKScope;

public class SocialVk extends CordovaPlugin {
  private static final String TAG = "SocialVk";
  private static final String ACTION_INIT = "initSocialVk";
  private static final String ACTION_SHARE = "share";
  private CallbackContext _callbackContext;

  /**
   * Gets the application context from cordova's main activity.
   * @return the application context
   */
  private Context getApplicationContext() {
    return this.webView.getContext();
  }
	
  @Override
  public boolean execute(String action, CordovaArgs args, final CallbackContext callbackContext) throws JSONException {
    this._callbackContext = callbackContext;
    if(ACTION_INIT.equals(action)) {
      return init(args.getString(0));
    } else if (ACTION_SHARE.equals(action)) {
      return shareOrLogin(args.getString(0), args.getString(1), args.getString(2));
    } else {
      Log.i(TAG, "Unknown action: "+action);
    }
    return true;
  }

  private boolean init(String appId)
  {
    this.cordova.setActivityResultCallback(this);
    final String sTokenKey = "VK_ACCESS_TOKEN";
    VKSdkListener sdkListener = new VKSdkListener() {
        private void success() {
          _callbackContext.sendPluginResult(new PluginResult(PluginResult.Status.OK));
          _callbackContext.success();
        }
        private void fail() {
          _callbackContext.sendPluginResult(new PluginResult(PluginResult.Status.ERROR));
          _callbackContext.error("Error");
        }
        
        @Override
        public void onCaptchaError(VKError captchaError) {
          new VKCaptchaDialog(captchaError).show();
          fail();
        }
        
        @Override
        public void onTokenExpired(VKAccessToken expiredToken) {
          //VKSdk.authorize(scope);
          Log.w(TAG, "VK token expired");
          success();
        }
                    
        @Override
        public void onAccessDenied(VKError authorizationError) {
          new AlertDialog.Builder(webView.getContext())
            .setMessage(authorizationError.errorMessage)
            .show();
          Log.w(TAG, "VK Access denied!");
          fail();
        }
                            
        @Override
        public void onReceiveNewToken(VKAccessToken newToken) {
          Log.i(TAG, "VK new token: "+newToken.accessToken);
          newToken.saveTokenToSharedPreferences(webView.getContext(), sTokenKey);
          success();
        }
            
        @Override
        public void onAcceptUserToken(VKAccessToken token) {
          Log.i(TAG, "VK accept token: "+token.accessToken);
          success();
        }
      };
        
    Log.i(TAG, "VK initialize");
    VKSdk.initialize(sdkListener, appId, VKAccessToken.tokenFromSharedPreferences(webView.getContext(), sTokenKey));
    VKUIHelper.onCreate((Activity)webView.getContext());

    //_callbackContext.sendPluginResult(new PluginResult(PluginResult.Status.OK));
    //_callbackContext.success();
    return true;
  }

  private boolean shareOrLogin(final String url, final String comment, final String imageUrl)
  {
    this.cordova.setActivityResultCallback(this);
    final String[] scope = new String[]{VKScope.WALL};
    if(!VKSdk.isLoggedIn()) {
      VKSdk.authorize(scope, false, true);
    } else {
      // TODO sharing
    }
    /*
    //определяем callback на операции с получением токена
    odnoklassnikiObject.setTokenRequestListener(new OkTokenRequestListener() {
        @Override
        public void onSuccess(String token) {
          Log.i(TAG, "Odnoklassniki accessToken = " + token);
          if (token == null)
            Toast.makeText(webView.getContext(), "Не удалось авторизоваться в приложении через \"Одноклассников\"."
                           + "\nОшибка на сервере \"Одноклассников\".", Toast.LENGTH_LONG).show();
          else
            share(url, comment);
        }

        @Override
        public void onCancel() {
          Log.i(TAG, "Auth cancel");
          Toast.makeText(webView.getContext(), "Не удалось авторизоваться в приложении через \"Одноклассников\"."
                         + "\nПроверьте соединение с Интернетом.", Toast.LENGTH_LONG).show();
        }

        @Override
        public void onError() {
          Log.i(TAG, "Auth error");
          Toast.makeText(webView.getContext(), "Ошибка во время авторизации в приложении через \"Одноклассников\".",
                         Toast.LENGTH_LONG).show();
        }
      });
    //вызываем запрос авторизации. После OAuth будет вызван callback, определенный для объекта
    odnoklassnikiObject.requestAuthorization(webView.getContext(), false, OkScope.VALUABLE_ACCESS);
    */
    return true;
  }

  private boolean share(final String url, final String comment)
  {
    /*
    final Map<String, String> params = new HashMap<String, String>();
    params.put("linkUrl", url);
    params.put("comment", comment);
    new AsyncTask<String, Void, String>() {
      @Override protected String doInBackground(String... args) {
        try {
          return odnoklassnikiObject.request("share.addLink", params, "get");
        } catch (IOException e) {
          e.printStackTrace();
          _callbackContext.error("Error");
        }
        return null;
      }
      @Override protected void onPostExecute(String result) {
        Log.i(TAG, "OK share result" + result);
        _callbackContext.success();
      }
    }.execute();
    */
    return true;
  }

  @Override public void onActivityResult(int requestCode, int resultCode, Intent data)
  {
    Log.i(TAG, "onActivityResult(" + requestCode + "," + resultCode + "," + data);
    super.onActivityResult(requestCode, resultCode, data);
    if(resultCode != 0)
      VKUIHelper.onActivityResult((Activity)webView.getContext(), requestCode, resultCode, data);
  }
  
}
