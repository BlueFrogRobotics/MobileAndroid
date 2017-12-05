//
//  Webrtc.h
//  StandaloneBuddy
//
//  Created by Super on 11/07/2017.
//  Copyright © 2017 Super. All rights reserved.
//

#import <Foundation/Foundation.h>

typedef enum {
    NTConnectionStateChanged,
    NTLocalSizeChanged,
    NTRemoteSizeChange,
    NTPeerMessageReceived,
    NTCrossbarMessageReceived,
    NTStatusMessageReceived
} RTCNotificationType;

@interface IOSRTC : NSObject

// TODO CLEAN NOTIFICATIONBLOCK STUFF

+ (IOSRTC *) sharedInstance;

- (void) connectToCrossbar:(NSString *)uri
                     realm:(NSString *)realm
               certificate:(NSString *)certificate
                       key:(NSString *)key
         notificationBlock:(void (^)(RTCNotificationType, NSString *))notificationBlock;

- (void) setupIOSRTC:(NSString *)localUser
          remoteUser:(NSString *)remoteUser
   notificationBlock:(void (^)(RTCNotificationType, NSString *))notificationBlock;

- (void) startIOSRTC;
- (void) call;
- (void) hangup;
- (void) stopIOSRTC;
- (void) sendThroughDataChannel:(NSString *)message;

- (int)  createTexture:(bool)isLocal forWidth:(int)width height:(int)height;
- (void) updateTexture:(bool)isLocal;
- (void) destroyTexture:(bool)isLocal;

- (void) publishConnectionRequest;
- (void) subscribeToChat:(NSString *)buddyId;
- (void) sendChatMessage:(NSString *)message;
- (void) subscribeToStatus:(NSString *)buddyList;

@end
