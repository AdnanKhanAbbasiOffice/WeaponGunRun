//
//  KeychainHelper.m
//
//
//  Created by Trang Pham on 2018-08-23.
//

#import "KeychainHelper.h"


@implementation KeychainHelper
    
    static NSString *serviceName = [[NSBundle mainBundle] bundleIdentifier];//@"com.abi.TestGetUUID";
    
+ (KeychainHelper*) instance {
    static KeychainHelper* helper = nil;
    static dispatch_once_t oneToken;
    dispatch_once(&oneToken, ^{
        helper = [[KeychainHelper alloc] init];
    });
    return helper;
}
    
- (char*)GetStr:(NSString *)key
    {
        // Do any additional setup after loading the view, typically from a nib.
        NSString * value = [UICKeyChainStore stringForKey:key];
        if(value)
        {
            NSLog(@"Value: %@", value);
            return (char*)[value UTF8String];
        }
        
        return (char*)[@"null" UTF8String];
    }
    
- (void)SStr:(NSString *)key :(NSString *)value
    {
        [UICKeyChainStore setString:value forKey:key];
    }
    
- (void)DStr:(NSString *)key
    {
        [UICKeyChainStore removeItemForKey:key];
    }

@end

static NSString* CreateNSString(const char* string)
{
    if (string != NULL)
    return [NSString stringWithUTF8String:string];
    else
    return [NSString stringWithUTF8String:""];
}

char* cStringCopy(const char* string)
{
    if (string == NULL)
    return NULL;
    
    char* res = (char*)malloc(strlen(string) + 1);
    strcpy(res, string);
    
    return res;
}

#ifdef __cplusplus
extern "C"
{
    char* GetStr(const char* key)
    {
        return cStringCopy([[KeychainHelper instance] GetStr:CreateNSString(key)]);
    }
    
    void SaveStr(const char* key, const char* value)
    {
        [[KeychainHelper instance] SStr:CreateNSString(key) :CreateNSString(value)];
    }
    
    void DeleteStr(const char* key)
    {
        [[KeychainHelper instance] DStr:CreateNSString(key)];
    }
}
#endif
