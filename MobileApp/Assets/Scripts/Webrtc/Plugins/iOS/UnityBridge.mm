//
//  UnityBridge.m
//  StandaloneBuddy
//
//  Created by Awabot on 09/08/2017.
//  Copyright © 2017 Awabot. All rights reserved.
//

#import "UnityBridge.h"
#import <iOSBuddy/IOSRTC.h>

@implementation UnityBridge

NSString *crossbarReceiver;
NSString *rtcReceiver;


static IOSRTC *iOSRTC;


#pragma mark - Interface implementation


void ConnectToCrossbar(const char *uri,
                       const char *realm,
                       const char *certificate,
                       const char *key,
                       const char *receiver)
{
    NSString *testString = [NSString stringWithUTF8String:receiver];
    crossbarReceiver = testString;
    
    iOSRTC = [IOSRTC sharedInstance];
    
    [iOSRTC connectToCrossbar:stringify(uri)
                        realm:stringify(realm)
                  certificate:stringify(certificate)
                          key:stringify(key)
            notificationBlock:notifyEnvironment];
}


void SetupIOSRTC(const char *localUser,
                 const char *remoteUser,
                 const char *receiver)
{
    NSLog(@"SETUP IOSRTC - %@", [NSString stringWithUTF8String:receiver]);
    NSString *testString2 = @"UnityWebrtc"; //[NSString stringWithUTF8String:receiver];
    rtcReceiver = testString2;
    
    [iOSRTC setupIOSRTC:stringify(localUser)
             remoteUser:stringify(remoteUser)
      notificationBlock:notifyEnvironment];
}

void StartIOSRTC()
{
    NSLog(@"SETUP IOSRTC 2 - %@", rtcReceiver);
    
    [iOSRTC startIOSRTC];
}

void Call()
{
    [iOSRTC call];
}

void Hangup()
{
    [iOSRTC hangup];
}

void StopIOSRTC()
{
    [iOSRTC stopIOSRTC];
}

void SendPeerMessage(const char *message)
{
    [iOSRTC sendThroughDataChannel:stringify(message)];
}

int CreateTexture(bool isLocal, int width, int height)
{
    return [iOSRTC createTexture:isLocal forWidth:width height:height];
}

void UpdateTexture(bool isLocal)
{
    [iOSRTC updateTexture:isLocal];
}

void DestroyTexture(bool isLocal)
{
    [iOSRTC destroyTexture:isLocal];
}

void PublishConnectionRequest()
{
    [iOSRTC publishConnectionRequest];
}

void SubscribeToChat(const char *buddyId)
{
    [iOSRTC subscribeToChat:stringify(buddyId)];
}

void SendChatMessage(const char *message)
{
    [iOSRTC sendChatMessage:stringify(message)];
}

void SubscribeToStatus(const char *buddyList)
{
    [iOSRTC subscribeToStatus:stringify(buddyList)];
}



#pragma mark - Callback management

void (^notifyEnvironment)(RTCNotificationType, NSString *) =
^(RTCNotificationType notificationType, NSString *message)
{
    switch (notificationType)
    {
        case NTConnectionStateChanged:
            NSLog(@"CONNECTION STATE CHANGED : %@ - %@ - %i", message, rtcReceiver, notificationType );
            UnitySendMessage([rtcReceiver UTF8String], [@"onRTCStateChanged" UTF8String], [message UTF8String]);
            break;
        case NTIceConnectionStateChanged:
            UnitySendMessage([rtcReceiver UTF8String], [@"setMIsWebrtcConnectionActive" UTF8String], [message UTF8String]);
            break;
        case NTLocalSizeChanged:
            NSLog(@"MESSAGE RECEIVED : %@ - %@ - %i", message, rtcReceiver, notificationType );
            UnitySendMessage([rtcReceiver UTF8String], [@"onLocalTextureSizeChanged" UTF8String], [message UTF8String]);
            break;
        case NTRemoteSizeChange:
            NSLog(@"MESSAGE RECEIVED : %@ - %@ - %i", message, [NSString stringWithUTF8String:[rtcReceiver UTF8String]], notificationType );
            UnitySendMessage([rtcReceiver UTF8String], [@"onRemoteTextureSizeChanged" UTF8String], [message UTF8String]);
            break;
        case NTPeerMessageReceived:
            NSLog(@"MESSAGE RECEIVED : %@ - %@ - %i", message, [NSString stringWithUTF8String:[rtcReceiver UTF8String]], notificationType );
            UnitySendMessage([@"RemoteControlRTC" UTF8String], [@"onMessage" UTF8String], [message UTF8String]);
            break;
        case NTCrossbarMessageReceived:
            NSLog(@"MESSAGE RECEIVED : %@ - %@ - %i", message, [NSString stringWithUTF8String:[crossbarReceiver UTF8String]], notificationType );
            UnitySendMessage([crossbarReceiver UTF8String], [@"OnMessageReceived" UTF8String], [message UTF8String]);
            break;
        case NTStatusMessageReceived:
            NSLog(@"MESSAGE RECEIVED : %@ - %@ - %i", message, [NSString stringWithUTF8String:[crossbarReceiver UTF8String]], notificationType );
            UnitySendMessage([crossbarReceiver UTF8String], [@"OnStatusMessage" UTF8String], [message UTF8String]);
            
            break;
    }
};


#pragma mark - Tools

NSString *stringify(const char *data)
{
    return [NSString stringWithUTF8String:data];
}

@end

