//
//  SocialVk.m

#import "SocialVk.h"
#import <VKontakte/VKBundle.h>

@implementation SocialVk {
    CDVInvokedUrlCommand *savedCommand;
    void (^vkCallBackBlock)(NSString *);
    BOOL inited;
    NSMutableDictionary *loginDetails;
}

@synthesize clientId;

- (void) initSocialVk:(CDVInvokedUrlCommand*)command
{
    CDVPluginResult* pluginResult = nil;
    
    
    if(!inited) {
        NSString *appId = [[NSString alloc] initWithString:[command.arguments objectAtIndex:0]];
        [VKSdk initializeWithDelegate:self andAppId:appId];
        if(![VKSdk wakeUpSession]) {
            NSLog(@"VK init error!");
        }
        
        NSLog(@"SocialVk Plugin initalized");
        
        [[NSNotificationCenter defaultCenter] addObserver:self selector:@selector(myOpenUrl:) name:CDVPluginHandleOpenURLNotification object:nil];
        inited = YES;
    }
    
    pluginResult = [CDVPluginResult resultWithStatus:CDVCommandStatus_OK];
    [self.commandDelegate sendPluginResult:pluginResult callbackId:command.callbackId];
}

-(UIViewController*)findViewController
{
    id vc = self.webView;
    do {
        vc = [vc nextResponder];
    } while([vc isKindOfClass:UIView.class]);
    return vc;
}

-(void)myOpenUrl:(NSNotification*)notification
{
    NSURL *url = notification.object;
    if(![url isKindOfClass:NSURL.class]) return;
    BOOL wasHandled = [VKSdk processOpenURL:url fromApplication:nil];
}

-(void) login:(CDVInvokedUrlCommand *)command
{
    if(![VKSdk isLoggedIn]) {
        [self vkLoginWithBlock:^(NSString *token) {
            if(token) {
                VKRequest *req = [VKRequest requestWithMethod:@"users.get" andParameters:@{@"fields": @"id, nickname, first_name, last_name, sex, bdate, timezone, photo, photo_big, city, country"} andHttpMethod:@"GET"];
                [req executeWithResultBlock:^(VKResponse *response) {
                    NSLog(@"User response %@", response);
                    
                    CDVPluginResult* pluginResult = nil;
                    loginDetails = [NSMutableDictionary new];
                    loginDetails[@"token"] = token;
                    if([response.json isKindOfClass:NSArray.class] && [(NSArray*)response.json count]>0 )
                        loginDetails[@"user"] = [response.json objectAtIndex:0];
                    pluginResult = [CDVPluginResult resultWithStatus:CDVCommandStatus_OK messageAsDictionary:loginDetails];
                    [self.commandDelegate sendPluginResult:pluginResult callbackId:command.callbackId];
                } errorBlock:^(NSError *error) {
                    NSLog(@"Cant load user details");
                    CDVPluginResult* pluginResult = [CDVPluginResult resultWithStatus:CDVCommandStatus_ERROR];
                    [self.commandDelegate sendPluginResult:pluginResult callbackId:command.callbackId];
                }];
            } else {
                NSLog(@"Cant login to VKontakte");
                CDVPluginResult* pluginResult = [CDVPluginResult resultWithStatus:CDVCommandStatus_ERROR];
                [self.commandDelegate sendPluginResult:pluginResult callbackId:command.callbackId];
            }
        }];
    } else {
        if(loginDetails) {
            CDVPluginResult* pluginResult = [CDVPluginResult resultWithStatus:CDVCommandStatus_OK messageAsDictionary:loginDetails];
            [self.commandDelegate sendPluginResult:pluginResult callbackId:command.callbackId];
        } else {
            VKAccessToken *token = [VKSdk getAccessToken];
            VKRequest *req = [VKRequest requestWithMethod:@"users.get" andParameters:@{@"fields": @"sex,bdate,city,country,screen_name,photo_50,photo_200_orig"} andHttpMethod:@"GET"];
            [req executeWithResultBlock:^(VKResponse *response) {
                NSLog(@"User response %@", response);
                CDVPluginResult* pluginResult = nil;
                loginDetails = [NSMutableDictionary new];
                loginDetails[@"token"] = token.accessToken;
                if([response.json isKindOfClass:NSArray.class] && [(NSArray*)response.json count]>0 )
                    loginDetails[@"user"] = [response.json objectAtIndex:0];
                pluginResult = [CDVPluginResult resultWithStatus:CDVCommandStatus_OK messageAsDictionary:loginDetails];
                [self.commandDelegate sendPluginResult:pluginResult callbackId:command.callbackId];
            } errorBlock:^(NSError *error) {
                NSLog(@"Cant load user details");
                CDVPluginResult* pluginResult = [CDVPluginResult resultWithStatus:CDVCommandStatus_ERROR];
                [self.commandDelegate sendPluginResult:pluginResult callbackId:command.callbackId];
            }];
        }
    }
}

-(void) share:(CDVInvokedUrlCommand*)command {
    NSBundle *vkb = [VKBundle vkLibraryResourcesBundle];
    NSLog(@"VK Bundle path %@", vkb.bundlePath);
    
    savedCommand = command;
    NSString *sourceURL = [command.arguments objectAtIndex:0];
    NSString* description = [command.arguments objectAtIndex:1];
    NSString* imageURL = [command.arguments objectAtIndex:2];
    //NSData *imageData = [NSData dataWithContentsOfURL:[NSURL URLWithString:imageURL]];

    if(![VKSdk isLoggedIn]) {
        [self vkLoginWithBlock:^(NSString *token) {
            CDVPluginResult* pluginResult = nil;
            if(token) {
                VKShareDialogController *sh = [VKShareDialogController new];
                [sh setWantsFullScreenLayout:YES];
                [sh setOtherAttachmentsStrings:@[sourceURL]];
                sh.text =  description;
                //sh.uploadImages = @[[VKUploadImage uploadImageWithData:imageData andParams:nil]];
                UIViewController *vc = [self findViewController];
                [sh performSelector:@selector(presentIn:) withObject:vc afterDelay:2.f];
                pluginResult = [CDVPluginResult resultWithStatus:CDVCommandStatus_OK];
                [self.commandDelegate sendPluginResult:pluginResult callbackId:command.callbackId];
            } else {
                NSLog(@"Cant login to VKontakte");
                pluginResult = [CDVPluginResult resultWithStatus:CDVCommandStatus_ERROR];
                [self.commandDelegate sendPluginResult:pluginResult callbackId:command.callbackId];
            }
        }];
    } else {
        VKShareDialogController *sh = [VKShareDialogController new];
        [sh setWantsFullScreenLayout:YES];
        [sh setOtherAttachmentsStrings:@[sourceURL]];
        sh.text =  description;
        //sh.uploadImages = @[[VKUploadImage uploadImageWithData:imageData andParams:nil]];
        UIViewController *vc = [self findViewController];
        [sh presentIn:vc];
        CDVPluginResult* pluginResult = [CDVPluginResult resultWithStatus:CDVCommandStatus_OK];
        [self.commandDelegate sendPluginResult:pluginResult callbackId:command.callbackId];
    }
}

-(void)vkLoginWithBlock:(void (^)(NSString *))block
{
    vkCallBackBlock = [block copy];
    [VKSdk authorize:@[VK_PER_WALL, VK_PER_OFFLINE] revokeAccess:NO forceOAuth:YES inApp:YES display:VK_DISPLAY_IOS];
}

-(void)logout:(CDVInvokedUrlCommand *)command
{
    [VKSdk forceLogout];
    CDVPluginResult* pluginResult = [CDVPluginResult resultWithStatus:CDVCommandStatus_OK];
    [self.commandDelegate sendPluginResult:pluginResult callbackId:command.callbackId];
}

#pragma mark - VKSdkDelegate

-(void) vkSdkReceivedNewToken:(VKAccessToken*) newToken
{
    NSLog(@"VK Token %@", newToken.accessToken);
    if(vkCallBackBlock) vkCallBackBlock(newToken.accessToken);
}

- (void)vkSdkAcceptedUserToken:(VKAccessToken *)token
{
    NSLog(@"VK Token %@", token.accessToken);
}

- (void)vkSdkRenewedToken:(VKAccessToken *)newToken
{
    NSLog(@"VK Token %@", newToken.accessToken);
}

-(void) vkSdkUserDeniedAccess:(VKError*) authorizationError
{
    NSLog(@"VK Error %@", authorizationError);
    if(vkCallBackBlock) vkCallBackBlock(nil);
}

-(void) vkSdkShouldPresentViewController:(UIViewController *)controller
{
    [[self findViewController] presentViewController:controller animated:YES completion:nil];
}

-(void) vkSdkTokenHasExpired:(VKAccessToken *)expiredToken
{
    
}

-(void) vkSdkNeedCaptchaEnter:(VKError *)captchaError
{
    NSLog(@"Need captcha %@", captchaError);
}

-(BOOL)vkSdkAuthorizationAllowFallbackToSafari
{
    return NO;
}

-(BOOL)vkSdkIsBasicAuthorization
{
    return YES;
}

@end
