//
//  SocialVk.h

#import <Cordova/CDV.h>
#import <VKontakte/VKSdk.h>

@interface SocialVk : CDVPlugin <VKSdkDelegate>
{
    NSString*     clientId;
}

@property (nonatomic, retain) NSString*     clientId;

- (void)initSocialVk:(CDVInvokedUrlCommand*)command;
- (void)login:(CDVInvokedUrlCommand*)command;
- (void)share:(CDVInvokedUrlCommand*)command;
- (void)logout:(CDVInvokedUrlCommand*)command;


@end
