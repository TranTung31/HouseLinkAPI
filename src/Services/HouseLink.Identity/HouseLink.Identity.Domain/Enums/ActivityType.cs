namespace HouseLink.Identity.Domain.Enums
{
    public enum ActivityType
    {
        // Authentication
        UserRegistered = 1,
        UserLoggedIn = 2,
        UserLoggedOut = 3,
        PasswordChanged = 4,
        PasswordResetRequested = 5,
        PasswordReset = 6,
        TokenRefreshed = 7,

        // Profile
        ProfileUpdated = 10,
        AvatarUpdated = 11,
        PhoneNumberUpdated = 12,

        // Account Status
        AccountActivated = 20,
        AccountDeactivated = 21,
        AccountLocked = 22,
        AccountUnlocked = 23,

        // Role & Permission
        RoleAssigned = 30,
        RoleRemoved = 31,
        PermissionGranted = 32,
        PermissionRevoked = 33,

        // Security
        LoginFailed = 40,
        SuspiciousActivityDetected = 41,
        IPAddressChanged = 42,

        // Other
        Other = 99
    }
}
