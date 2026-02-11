# MyApi.Client.Api.MinitwitApi

All URIs are relative to *http://localhost*

| Method | HTTP request | Description |
|--------|--------------|-------------|
| [**GetFollow**](MinitwitApi.md#getfollow) | **GET** /fllws/{username} |  |
| [**GetLatestValue**](MinitwitApi.md#getlatestvalue) | **GET** /latest |  |
| [**GetMessages**](MinitwitApi.md#getmessages) | **GET** /msgs |  |
| [**GetMessagesPerUser**](MinitwitApi.md#getmessagesperuser) | **GET** /msgs/{username} |  |
| [**PostFollow**](MinitwitApi.md#postfollow) | **POST** /fllws/{username} |  |
| [**PostMessagesPerUser**](MinitwitApi.md#postmessagesperuser) | **POST** /msgs/{username} |  |
| [**PostRegister**](MinitwitApi.md#postregister) | **POST** /register |  |

<a id="getfollow"></a>
# **GetFollow**
> FollowsResponse GetFollow (string username, string authorization, int latest = null, int no = null)



Get list of users followed by the given user.  - Query param `?no=` limits result count. - Optionally updates a 'latest' global value via `?latest=` query param.


### Parameters

| Name | Type | Description | Notes |
|------|------|-------------|-------|
| **username** | **string** |  |  |
| **authorization** | **string** | Authorization string of the form &#x60;Basic c2ltdWxhdG9yOnN1cGVyX3NhZmUh&#x60;. Used to authenticate as simulator |  |
| **latest** | **int** | Optional: &#x60;latest&#x60; value to update | [optional]  |
| **no** | **int** | Optional: &#x60;no&#x60; limits result count | [optional] [default to 100] |

### Return type

[**FollowsResponse**](FollowsResponse.md)

### Authorization

No authorization required

### HTTP request headers

 - **Content-Type**: Not defined
 - **Accept**: application/json


### HTTP response details
| Status code | Description | Response headers |
|-------------|-------------|------------------|
| **200** | Success |  -  |
| **403** | Unauthorized - Must include correct Authorization header |  -  |
| **404** | User not found (no response body) |  -  |

[[Back to top]](#) [[Back to API list]](../../README.md#documentation-for-api-endpoints) [[Back to Model list]](../../README.md#documentation-for-models) [[Back to README]](../../README.md)

<a id="getlatestvalue"></a>
# **GetLatestValue**
> LatestValue GetLatestValue ()



Returns the latest ID saved


### Parameters
This endpoint does not need any parameter.
### Return type

[**LatestValue**](LatestValue.md)

### Authorization

No authorization required

### HTTP request headers

 - **Content-Type**: Not defined
 - **Accept**: application/json


### HTTP response details
| Status code | Description | Response headers |
|-------------|-------------|------------------|
| **200** | Success |  -  |
| **500** | Internal Server Error |  -  |

[[Back to top]](#) [[Back to API list]](../../README.md#documentation-for-api-endpoints) [[Back to Model list]](../../README.md#documentation-for-models) [[Back to README]](../../README.md)

<a id="getmessages"></a>
# **GetMessages**
> List&lt;Message&gt; GetMessages (string authorization, int latest = null, int no = null)



Get recent messages.  - Filters out flagged messages - Returns a list of recent messages (max defined by `?no=` param) - Optionally updates a 'latest' global value via `?latest=` query param.


### Parameters

| Name | Type | Description | Notes |
|------|------|-------------|-------|
| **authorization** | **string** | Authorization string of the form &#x60;Basic c2ltdWxhdG9yOnN1cGVyX3NhZmUh&#x60;. Used to authenticate as simulator |  |
| **latest** | **int** | Optional: &#x60;latest&#x60; value to update | [optional]  |
| **no** | **int** | Optional: &#x60;no&#x60; limits result count | [optional] [default to 100] |

### Return type

[**List&lt;Message&gt;**](Message.md)

### Authorization

No authorization required

### HTTP request headers

 - **Content-Type**: Not defined
 - **Accept**: application/json


### HTTP response details
| Status code | Description | Response headers |
|-------------|-------------|------------------|
| **200** | Success |  -  |
| **403** | Unauthorized - Must include correct Authorization header |  -  |

[[Back to top]](#) [[Back to API list]](../../README.md#documentation-for-api-endpoints) [[Back to Model list]](../../README.md#documentation-for-models) [[Back to README]](../../README.md)

<a id="getmessagesperuser"></a>
# **GetMessagesPerUser**
> List&lt;Message&gt; GetMessagesPerUser (string username, string authorization, int latest = null, int no = null)



Get messages for a specific user.  - Returns messages authored by the specified user - Filtered by unflagged - Returns a list of recent messages for the user (max defined by `?no=` param) - Optionally updates a 'latest' global value via `?latest=` query param.


### Parameters

| Name | Type | Description | Notes |
|------|------|-------------|-------|
| **username** | **string** |  |  |
| **authorization** | **string** | Authorization string of the form &#x60;Basic c2ltdWxhdG9yOnN1cGVyX3NhZmUh&#x60;. Used to authenticate as simulator |  |
| **latest** | **int** | Optional: &#x60;latest&#x60; value to update | [optional]  |
| **no** | **int** | Optional: &#x60;no&#x60; limits result count | [optional] [default to 100] |

### Return type

[**List&lt;Message&gt;**](Message.md)

### Authorization

No authorization required

### HTTP request headers

 - **Content-Type**: Not defined
 - **Accept**: application/json


### HTTP response details
| Status code | Description | Response headers |
|-------------|-------------|------------------|
| **200** | Success |  -  |
| **403** | Unauthorized - Must include correct Authorization header |  -  |
| **404** | User not found (no response body) |  -  |

[[Back to top]](#) [[Back to API list]](../../README.md#documentation-for-api-endpoints) [[Back to Model list]](../../README.md#documentation-for-models) [[Back to README]](../../README.md)

<a id="postfollow"></a>
# **PostFollow**
> void PostFollow (string username, string authorization, FollowAction payload, int latest = null)



Follow or unfollow a user on behalf of `username`.  - Body must contain either `follow: <user>` or `unfollow: <user>`


### Parameters

| Name | Type | Description | Notes |
|------|------|-------------|-------|
| **username** | **string** |  |  |
| **authorization** | **string** | Authorization string of the form &#x60;Basic c2ltdWxhdG9yOnN1cGVyX3NhZmUh&#x60;. Used to authenticate as simulator |  |
| **payload** | [**FollowAction**](FollowAction.md) |  |  |
| **latest** | **int** | Optional: &#x60;latest&#x60; value to update | [optional]  |

### Return type

void (empty response body)

### Authorization

No authorization required

### HTTP request headers

 - **Content-Type**: application/json
 - **Accept**: application/json


### HTTP response details
| Status code | Description | Response headers |
|-------------|-------------|------------------|
| **204** | No Content |  -  |
| **403** | Unauthorized - Must include correct Authorization header |  -  |
| **404** | User not found (no response body) |  -  |

[[Back to top]](#) [[Back to API list]](../../README.md#documentation-for-api-endpoints) [[Back to Model list]](../../README.md#documentation-for-models) [[Back to README]](../../README.md)

<a id="postmessagesperuser"></a>
# **PostMessagesPerUser**
> void PostMessagesPerUser (string username, string authorization, PostMessage payload, int latest = null)



Post a new message as a specific user.  - Message must include `content` in the body - Stored with timestamp and `flagged=0` - Returns empty body on success - Optionally updates a 'latest' global value via `?latest=` query param.


### Parameters

| Name | Type | Description | Notes |
|------|------|-------------|-------|
| **username** | **string** |  |  |
| **authorization** | **string** | Authorization string of the form &#x60;Basic c2ltdWxhdG9yOnN1cGVyX3NhZmUh&#x60;. Used to authenticate as simulator |  |
| **payload** | [**PostMessage**](PostMessage.md) |  |  |
| **latest** | **int** | Optional: &#x60;latest&#x60; value to update | [optional]  |

### Return type

void (empty response body)

### Authorization

No authorization required

### HTTP request headers

 - **Content-Type**: application/json
 - **Accept**: application/json


### HTTP response details
| Status code | Description | Response headers |
|-------------|-------------|------------------|
| **204** | No Content |  -  |
| **403** | Unauthorized - Must include correct Authorization header |  -  |

[[Back to top]](#) [[Back to API list]](../../README.md#documentation-for-api-endpoints) [[Back to Model list]](../../README.md#documentation-for-models) [[Back to README]](../../README.md)

<a id="postregister"></a>
# **PostRegister**
> void PostRegister (RegisterRequest payload, int latest = null)



Register a new user. - Optionally updates a 'latest' global value via `?latest=` query param.


### Parameters

| Name | Type | Description | Notes |
|------|------|-------------|-------|
| **payload** | [**RegisterRequest**](RegisterRequest.md) |  |  |
| **latest** | **int** | Optional: &#x60;latest&#x60; value to update | [optional]  |

### Return type

void (empty response body)

### Authorization

No authorization required

### HTTP request headers

 - **Content-Type**: application/json
 - **Accept**: application/json


### HTTP response details
| Status code | Description | Response headers |
|-------------|-------------|------------------|
| **204** | No Content |  -  |
| **400** | Bad Request | Possible reasons:  - missing username  - invalid email  - password missing  - username already taken |  -  |

[[Back to top]](#) [[Back to API list]](../../README.md#documentation-for-api-endpoints) [[Back to Model list]](../../README.md#documentation-for-models) [[Back to README]](../../README.md)

