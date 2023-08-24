namespace NordVpnAccountsChecker
{
    public static class Message
    {
        public static string AccountBlocked = "Account locked for security reasons. We sent you an email with how to unlock it. Alternatively, you can reset your password or use another way to log in.";
        public static string TooManyRequests = "You sent too many requests";
        public static string TooManyRequestsTryIn5Mins = "Too many failed attempts. Please try again in 5 minutes.";
        public static string IncorrectPassword = "The password you entered is incorrect. Please try again.";
    }
}
