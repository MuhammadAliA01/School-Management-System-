using FirebaseAdmin.Messaging;

namespace SchoolManagmentSystem.Services
{
    public class FirebaseNotificationService
    {
        public async Task<bool> SendNotification(
     string fcmToken,
     string title,
     string body)
        {
            try
            {
                var message = new Message
                {
                    Token = fcmToken,
                    Notification = new Notification
                    {
                        Title = title,
                        Body = body
                    }
                };

                await FirebaseMessaging.DefaultInstance.SendAsync(message);

                return true;
            }
            catch (FirebaseMessagingException ex)
            {
                Console.WriteLine($"FCM Error: {ex.Message}");

                return false;
            }
        }
    }
}