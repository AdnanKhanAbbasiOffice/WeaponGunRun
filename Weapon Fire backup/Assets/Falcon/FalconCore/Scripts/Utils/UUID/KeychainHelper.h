//
//  UUIDHelper.h
//  
//
//  Created by Trang Pham on 2018-08-23.
//

#import <Foundation/Foundation.h>
#import "UICKeyChainStore.h"

@interface KeychainHelper : NSObject
+ (KeychainHelper*) instance;
-(char*)GetStr:(NSString*)key;
-(void)SStr:(NSString*)key:(NSString*)value;
-(void)DStr:(NSString*)key;
@end

#ifdef __cplusplus
extern "C"
{
    char* GetStr(const char* key);
    void SaveStr(const char* key, const char* value);
    void DeleteStr(const char* key);
}
#endif
