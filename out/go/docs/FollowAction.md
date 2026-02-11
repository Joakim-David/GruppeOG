# FollowAction

## Properties

Name | Type | Description | Notes
------------ | ------------- | ------------- | -------------
**Follow** | Pointer to **string** | Username to follow (optional, either this or \&quot;unfollow\&quot;) | [optional] 
**Unfollow** | Pointer to **string** | Username to unfollow (optional, either this or \&quot;follow\&quot;) | [optional] 

## Methods

### NewFollowAction

`func NewFollowAction() *FollowAction`

NewFollowAction instantiates a new FollowAction object
This constructor will assign default values to properties that have it defined,
and makes sure properties required by API are set, but the set of arguments
will change when the set of required properties is changed

### NewFollowActionWithDefaults

`func NewFollowActionWithDefaults() *FollowAction`

NewFollowActionWithDefaults instantiates a new FollowAction object
This constructor will only assign default values to properties that have it defined,
but it doesn't guarantee that properties required by API are set

### GetFollow

`func (o *FollowAction) GetFollow() string`

GetFollow returns the Follow field if non-nil, zero value otherwise.

### GetFollowOk

`func (o *FollowAction) GetFollowOk() (*string, bool)`

GetFollowOk returns a tuple with the Follow field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetFollow

`func (o *FollowAction) SetFollow(v string)`

SetFollow sets Follow field to given value.

### HasFollow

`func (o *FollowAction) HasFollow() bool`

HasFollow returns a boolean if a field has been set.

### GetUnfollow

`func (o *FollowAction) GetUnfollow() string`

GetUnfollow returns the Unfollow field if non-nil, zero value otherwise.

### GetUnfollowOk

`func (o *FollowAction) GetUnfollowOk() (*string, bool)`

GetUnfollowOk returns a tuple with the Unfollow field if it's non-nil, zero value otherwise
and a boolean to check if the value has been set.

### SetUnfollow

`func (o *FollowAction) SetUnfollow(v string)`

SetUnfollow sets Unfollow field to given value.

### HasUnfollow

`func (o *FollowAction) HasUnfollow() bool`

HasUnfollow returns a boolean if a field has been set.


[[Back to Model list]](../README.md#documentation-for-models) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to README]](../README.md)


