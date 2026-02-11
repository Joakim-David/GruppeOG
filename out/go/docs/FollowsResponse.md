# FollowsResponse

## Properties

Name | Type | Description | Notes
------------ | ------------- | ------------- | -------------
**Follows** | Pointer to **[]string** | List of usernames the user is following | [optional] 

## Methods

### NewFollowsResponse

`func NewFollowsResponse() *FollowsResponse`

NewFollowsResponse instantiates a new FollowsResponse object
This constructor will assign default values to properties that have it defined,
and makes sure properties required by API are set, but the set of arguments
will change when the set of required properties is changed

### NewFollowsResponseWithDefaults

`func NewFollowsResponseWithDefaults() *FollowsResponse`

NewFollowsResponseWithDefaults instantiates a new FollowsResponse object
This constructor will only assign default values to properties that have it defined,
but it doesn't guarantee that properties required by API are set

### GetFollows

`func (o *FollowsResponse) GetFollows() []string`

GetFollows returns the Follows field if non-nil, zero value otherwise.

### GetFollowsOk

`func (o *FollowsResponse) GetFollowsOk() (*[]string, bool)`

GetFollowsOk returns a tuple with the Follows field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetFollows

`func (o *FollowsResponse) SetFollows(v []string)`

SetFollows sets Follows field to given value.

### HasFollows

`func (o *FollowsResponse) HasFollows() bool`

HasFollows returns a boolean if a field has been set.


[[Back to Model list]](../README.md#documentation-for-models) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to README]](../README.md)


