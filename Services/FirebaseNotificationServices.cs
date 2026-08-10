using FirebaseAdmin.Messaging;

namespace SchoolManagmentSystem.Services
{
    public class FirebaseNotificationService
    {
        public async Task<string> SendNotification(
            string fcmToken,
            string title,
            string body)
        {
            var message = new Message()
            {
                Token = fcmToken,

                Notification = new Notification()
                {
                    Title = title,
                    Body = body
                }
            };

            string response = await FirebaseMessaging.DefaultInstance
                .SendAsync(message);

            return response;
        }
    }
}