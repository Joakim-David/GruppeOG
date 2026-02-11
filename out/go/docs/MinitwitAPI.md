# \MinitwitAPI

All URIs are relative to *http://localhost*

Method | HTTP request | Description
------------- | ------------- | -------------
[**GetFollow**](MinitwitAPI.md#GetFollow) | **Get** /fllws/{username} | 
[**GetLatestValue**](MinitwitAPI.md#GetLatestValue) | **Get** /latest | 
[**GetMessages**](MinitwitAPI.md#GetMessages) | **Get** /msgs | 
[**GetMessagesPerUser**](MinitwitAPI.md#GetMessagesPerUser) | **Get** /msgs/{username} | 
[**PostFollow**](MinitwitAPI.md#PostFollow) | **Post** /fllws/{username} | 
[**PostMessagesPerUser**](MinitwitAPI.md#PostMessagesPerUser) | **Post** /msgs/{username} | 
[**PostRegister**](MinitwitAPI.md#PostRegister) | **Post** /register | 



## GetFollow

> FollowsResponse GetFollow(ctx, username).Authorization(authorization).Latest(latest).No(no).Execute()





### Example

```go
package main

import (
	"context"
	"fmt"
	"os"
	openapiclient "github.com/GIT_USER_ID/GIT_REPO_ID"
)

func main() {
	username := "username_example" // string | 
	authorization := "authorization_example" // string | Authorization string of the form `Basic c2ltdWxhdG9yOnN1cGVyX3NhZmUh`. Used to authenticate as simulator
	latest := int32(56) // int32 | Optional: `latest` value to update (optional)
	no := int32(56) // int32 | Optional: `no` limits result count (optional) (default to 100)

	configuration := openapiclient.NewConfiguration()
	apiClient := openapiclient.NewAPIClient(configuration)
	resp, r, err := apiClient.MinitwitAPI.GetFollow(context.Background(), username).Authorization(authorization).Latest(latest).No(no).Execute()
	if err != nil {
		fmt.Fprintf(os.Stderr, "Error when calling `MinitwitAPI.GetFollow``: %v\n", err)
		fmt.Fprintf(os.Stderr, "Full HTTP response: %v\n", r)
	}
	// response from `GetFollow`: FollowsResponse
	fmt.Fprintf(os.Stdout, "Response from `MinitwitAPI.GetFollow`: %v\n", resp)
}
```

### Path Parameters


Name | Type | Description  | Notes
------------- | ------------- | ------------- | -------------
**ctx** | **context.Context** | context for authentication, logging, cancellation, deadlines, tracing, etc.
**username** | **string** |  | 

### Other Parameters

Other parameters are passed through a pointer to a apiGetFollowRequest struct via the builder pattern


Name | Type | Description  | Notes
------------- | ------------- | ------------- | -------------

 **authorization** | **string** | Authorization string of the form &#x60;Basic c2ltdWxhdG9yOnN1cGVyX3NhZmUh&#x60;. Used to authenticate as simulator | 
 **latest** | **int32** | Optional: &#x60;latest&#x60; value to update | 
 **no** | **int32** | Optional: &#x60;no&#x60; limits result count | [default to 100]

### Return type

[**FollowsResponse**](FollowsResponse.md)

### Authorization

No authorization required

### HTTP request headers

- **Content-Type**: Not defined
- **Accept**: application/json

[[Back to top]](#) [[Back to API list]](../README.md#documentation-for-api-endpoints)
[[Back to Model list]](../README.md#documentation-for-models)
[[Back to README]](../README.md)


## GetLatestValue

> LatestValue GetLatestValue(ctx).Execute()





### Example

```go
package main

import (
	"context"
	"fmt"
	"os"
	openapiclient "github.com/GIT_USER_ID/GIT_REPO_ID"
)

func main() {

	configuration := openapiclient.NewConfiguration()
	apiClient := openapiclient.NewAPIClient(configuration)
	resp, r, err := apiClient.MinitwitAPI.GetLatestValue(context.Background()).Execute()
	if err != nil {
		fmt.Fprintf(os.Stderr, "Error when calling `MinitwitAPI.GetLatestValue``: %v\n", err)
		fmt.Fprintf(os.Stderr, "Full HTTP response: %v\n", r)
	}
	// response from `GetLatestValue`: LatestValue
	fmt.Fprintf(os.Stdout, "Response from `MinitwitAPI.GetLatestValue`: %v\n", resp)
}
```

### Path Parameters

This endpoint does not need any parameter.

### Other Parameters

Other parameters are passed through a pointer to a apiGetLatestValueRequest struct via the builder pattern


### Return type

[**LatestValue**](LatestValue.md)

### Authorization

No authorization required

### HTTP request headers

- **Content-Type**: Not defined
- **Accept**: application/json

[[Back to top]](#) [[Back to API list]](../README.md#documentation-for-api-endpoints)
[[Back to Model list]](../README.md#documentation-for-models)
[[Back to README]](../README.md)


## GetMessages

> []Message GetMessages(ctx).Authorization(authorization).Latest(latest).No(no).Execute()





### Example

```go
package main

import (
	"context"
	"fmt"
	"os"
	openapiclient "github.com/GIT_USER_ID/GIT_REPO_ID"
)

func main() {
	authorization := "authorization_example" // string | Authorization string of the form `Basic c2ltdWxhdG9yOnN1cGVyX3NhZmUh`. Used to authenticate as simulator
	latest := int32(56) // int32 | Optional: `latest` value to update (optional)
	no := int32(56) // int32 | Optional: `no` limits result count (optional) (default to 100)

	configuration := openapiclient.NewConfiguration()
	apiClient := openapiclient.NewAPIClient(configuration)
	resp, r, err := apiClient.MinitwitAPI.GetMessages(context.Background()).Authorization(authorization).Latest(latest).No(no).Execute()
	if err != nil {
		fmt.Fprintf(os.Stderr, "Error when calling `MinitwitAPI.GetMessages``: %v\n", err)
		fmt.Fprintf(os.Stderr, "Full HTTP response: %v\n", r)
	}
	// response from `GetMessages`: []Message
	fmt.Fprintf(os.Stdout, "Response from `MinitwitAPI.GetMessages`: %v\n", resp)
}
```

### Path Parameters



### Other Parameters

Other parameters are passed through a pointer to a apiGetMessagesRequest struct via the builder pattern


Name | Type | Description  | Notes
------------- | ------------- | ------------- | -------------
 **authorization** | **string** | Authorization string of the form &#x60;Basic c2ltdWxhdG9yOnN1cGVyX3NhZmUh&#x60;. Used to authenticate as simulator | 
 **latest** | **int32** | Optional: &#x60;latest&#x60; value to update | 
 **no** | **int32** | Optional: &#x60;no&#x60; limits result count | [default to 100]

### Return type

[**[]Message**](Message.md)

### Authorization

No authorization required

### HTTP request headers

- **Content-Type**: Not defined
- **Accept**: application/json

[[Back to top]](#) [[Back to API list]](../README.md#documentation-for-api-endpoints)
[[Back to Model list]](../README.md#documentation-for-models)
[[Back to README]](../README.md)


## GetMessagesPerUser

> []Message GetMessagesPerUser(ctx, username).Authorization(authorization).Latest(latest).No(no).Execute()





### Example

```go
package main

import (
	"context"
	"fmt"
	"os"
	openapiclient "github.com/GIT_USER_ID/GIT_REPO_ID"
)

func main() {
	username := "username_example" // string | 
	authorization := "authorization_example" // string | Authorization string of the form `Basic c2ltdWxhdG9yOnN1cGVyX3NhZmUh`. Used to authenticate as simulator
	latest := int32(56) // int32 | Optional: `latest` value to update (optional)
	no := int32(56) // int32 | Optional: `no` limits result count (optional) (default to 100)

	configuration := openapiclient.NewConfiguration()
	apiClient := openapiclient.NewAPIClient(configuration)
	resp, r, err := apiClient.MinitwitAPI.GetMessagesPerUser(context.Background(), username).Authorization(authorization).Latest(latest).No(no).Execute()
	if err != nil {
		fmt.Fprintf(os.Stderr, "Error when calling `MinitwitAPI.GetMessagesPerUser``: %v\n", err)
		fmt.Fprintf(os.Stderr, "Full HTTP response: %v\n", r)
	}
	// response from `GetMessagesPerUser`: []Message
	fmt.Fprintf(os.Stdout, "Response from `MinitwitAPI.GetMessagesPerUser`: %v\n", resp)
}
```

### Path Parameters


Name | Type | Description  | Notes
------------- | ------------- | ------------- | -------------
**ctx** | **context.Context** | context for authentication, logging, cancellation, deadlines, tracing, etc.
**username** | **string** |  | 

### Other Parameters

Other parameters are passed through a pointer to a apiGetMessagesPerUserRequest struct via the builder pattern


Name | Type | Description  | Notes
------------- | ------------- | ------------- | -------------

 **authorization** | **string** | Authorization string of the form &#x60;Basic c2ltdWxhdG9yOnN1cGVyX3NhZmUh&#x60;. Used to authenticate as simulator | 
 **latest** | **int32** | Optional: &#x60;latest&#x60; value to update | 
 **no** | **int32** | Optional: &#x60;no&#x60; limits result count | [default to 100]

### Return type

[**[]Message**](Message.md)

### Authorization

No authorization required

### HTTP request headers

- **Content-Type**: Not defined
- **Accept**: application/json

[[Back to top]](#) [[Back to API list]](../README.md#documentation-for-api-endpoints)
[[Back to Model list]](../README.md#documentation-for-models)
[[Back to README]](../README.md)


## PostFollow

> PostFollow(ctx, username).Authorization(authorization).Payload(payload).Latest(latest).Execute()





### Example

```go
package main

import (
	"context"
	"fmt"
	"os"
	openapiclient "github.com/GIT_USER_ID/GIT_REPO_ID"
)

func main() {
	username := "username_example" // string | 
	authorization := "authorization_example" // string | Authorization string of the form `Basic c2ltdWxhdG9yOnN1cGVyX3NhZmUh`. Used to authenticate as simulator
	payload := *openapiclient.NewFollowAction() // FollowAction | 
	latest := int32(56) // int32 | Optional: `latest` value to update (optional)

	configuration := openapiclient.NewConfiguration()
	apiClient := openapiclient.NewAPIClient(configuration)
	r, err := apiClient.MinitwitAPI.PostFollow(context.Background(), username).Authorization(authorization).Payload(payload).Latest(latest).Execute()
	if err != nil {
		fmt.Fprintf(os.Stderr, "Error when calling `MinitwitAPI.PostFollow``: %v\n", err)
		fmt.Fprintf(os.Stderr, "Full HTTP response: %v\n", r)
	}
}
```

### Path Parameters


Name | Type | Description  | Notes
------------- | ------------- | ------------- | -------------
**ctx** | **context.Context** | context for authentication, logging, cancellation, deadlines, tracing, etc.
**username** | **string** |  | 

### Other Parameters

Other parameters are passed through a pointer to a apiPostFollowRequest struct via the builder pattern


Name | Type | Description  | Notes
------------- | ------------- | ------------- | -------------

 **authorization** | **string** | Authorization string of the form &#x60;Basic c2ltdWxhdG9yOnN1cGVyX3NhZmUh&#x60;. Used to authenticate as simulator | 
 **payload** | [**FollowAction**](FollowAction.md) |  | 
 **latest** | **int32** | Optional: &#x60;latest&#x60; value to update | 

### Return type

 (empty response body)

### Authorization

No authorization required

### HTTP request headers

- **Content-Type**: application/json
- **Accept**: application/json

[[Back to top]](#) [[Back to API list]](../README.md#documentation-for-api-endpoints)
[[Back to Model list]](../README.md#documentation-for-models)
[[Back to README]](../README.md)


## PostMessagesPerUser

> PostMessagesPerUser(ctx, username).Authorization(authorization).Payload(payload).Latest(latest).Execute()





### Example

```go
package main

import (
	"context"
	"fmt"
	"os"
	openapiclient "github.com/GIT_USER_ID/GIT_REPO_ID"
)

func main() {
	username := "username_example" // string | 
	authorization := "authorization_example" // string | Authorization string of the form `Basic c2ltdWxhdG9yOnN1cGVyX3NhZmUh`. Used to authenticate as simulator
	payload := *openapiclient.NewPostMessage("Hello, World!") // PostMessage | 
	latest := int32(56) // int32 | Optional: `latest` value to update (optional)

	configuration := openapiclient.NewConfiguration()
	apiClient := openapiclient.NewAPIClient(configuration)
	r, err := apiClient.MinitwitAPI.PostMessagesPerUser(context.Background(), username).Authorization(authorization).Payload(payload).Latest(latest).Execute()
	if err != nil {
		fmt.Fprintf(os.Stderr, "Error when calling `MinitwitAPI.PostMessagesPerUser``: %v\n", err)
		fmt.Fprintf(os.Stderr, "Full HTTP response: %v\n", r)
	}
}
```

### Path Parameters


Name | Type | Description  | Notes
------------- | ------------- | ------------- | -------------
**ctx** | **context.Context** | context for authentication, logging, cancellation, deadlines, tracing, etc.
**username** | **string** |  | 

### Other Parameters

Other parameters are passed through a pointer to a apiPostMessagesPerUserRequest struct via the builder pattern


Name | Type | Description  | Notes
------------- | ------------- | ------------- | -------------

 **authorization** | **string** | Authorization string of the form &#x60;Basic c2ltdWxhdG9yOnN1cGVyX3NhZmUh&#x60;. Used to authenticate as simulator | 
 **payload** | [**PostMessage**](PostMessage.md) |  | 
 **latest** | **int32** | Optional: &#x60;latest&#x60; value to update | 

### Return type

 (empty response body)

### Authorization

No authorization required

### HTTP request headers

- **Content-Type**: application/json
- **Accept**: application/json

[[Back to top]](#) [[Back to API list]](../README.md#documentation-for-api-endpoints)
[[Back to Model list]](../README.md#documentation-for-models)
[[Back to README]](../README.md)


## PostRegister

> PostRegister(ctx).Payload(payload).Latest(latest).Execute()





### Example

```go
package main

import (
	"context"
	"fmt"
	"os"
	openapiclient "github.com/GIT_USER_ID/GIT_REPO_ID"
)

func main() {
	payload := *openapiclient.NewRegisterRequest("Username_example", "Email_example", "Pwd_example") // RegisterRequest | 
	latest := int32(56) // int32 | Optional: `latest` value to update (optional)

	configuration := openapiclient.NewConfiguration()
	apiClient := openapiclient.NewAPIClient(configuration)
	r, err := apiClient.MinitwitAPI.PostRegister(context.Background()).Payload(payload).Latest(latest).Execute()
	if err != nil {
		fmt.Fprintf(os.Stderr, "Error when calling `MinitwitAPI.PostRegister``: %v\n", err)
		fmt.Fprintf(os.Stderr, "Full HTTP response: %v\n", r)
	}
}
```

### Path Parameters



### Other Parameters

Other parameters are passed through a pointer to a apiPostRegisterRequest struct via the builder pattern


Name | Type | Description  | Notes
------------- | ------------- | ------------- | -------------
 **payload** | [**RegisterRequest**](RegisterRequest.md) |  | 
 **latest** | **int32** | Optional: &#x60;latest&#x60; value to update | 

### Return type

 (empty response body)

### Authorization

No authorization required

### HTTP request headers

- **Content-Type**: application/json
- **Accept**: application/json

[[Back to top]](#) [[Back to API list]](../README.md#documentation-for-api-endpoints)
[[Back to Model list]](../README.md#documentation-for-models)
[[Back to README]](../README.md)

