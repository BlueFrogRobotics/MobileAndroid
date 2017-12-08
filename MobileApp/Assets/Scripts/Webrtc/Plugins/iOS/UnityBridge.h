//
//  UnityBridge.h
//  StandaloneBuddy
//
//  Created by Awabot on 09/08/2017.
//  Copyright © 2017 Awabot. All rights reserved.
//

#import <Foundation/Foundation.h>

@interface UnityBridge : NSObject

extern "C"
{
    void ConnectToCrossbar(const char *uri,
                           const char *realm,
                           const char *certificate,
                           const char *key,
                           const char *receiver);
    
    void SetupIOSRTC(const char *localUser,
                     const char *remoteUser,
                     const char *receiver);
    
    void StartIOSRTC();
    void Call();
    void Hangup();
    void StopIOSRTC();
    void SendPeerMessage(const char *message);
    
    int  CreateTexture(bool isLocal, int width, int height);
    void UpdateTexture(bool isLocal);
    void DestroyTexture(bool isLocal);
    void PublishConnectionRequest();
    void SubscribeToChat(const char *buddyId);
    void SendChatMessage(const char *message);
    
    // Status features
    void SubscribeToStatus(const char *buddyList);
}

@end
