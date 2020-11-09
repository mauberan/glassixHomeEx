using System;
using System.Net.Http;
using System.Net.Mail;
using System.Threading.Tasks;
using System.Net;

namespace glassix.home.ex
{
    class Program
    {
        static bool IsValidEmail(string email)
        {
            try {
                var addr = new System.Net.Mail.MailAddress(email);
                return addr.Address == email;
            }
            catch {
                return false;
            }
        }

        static async Task Main(string[] args) 
        {
            //fetching joke:
            string fetchedJoke = "";

            Console.WriteLine("Hello, please enter your email: ");

            string userEmailAddress = Console.ReadLine();
            while (!Program.IsValidEmail(userEmailAddress)) {
                    Console.WriteLine("Please enter valid email: ");
                    userEmailAddress = Console.ReadLine();
            }

            try {
                HttpClient client = new HttpClient();
                HttpRequestMessage request = new HttpRequestMessage() {
                RequestUri = new Uri("https://icanhazdadjoke.com/"),
                Method = HttpMethod.Get,
                };
                client.DefaultRequestHeaders.Add("Accept", "text/plain");

                HttpResponseMessage result = await client.SendAsync(request);
                result.EnsureSuccessStatusCode();

                fetchedJoke = await result.Content.ReadAsStringAsync();

            }
            catch (Exception exception) {
                Console.WriteLine("CAUGHT EXCEPTION: while fetching joke");
                Console.WriteLine(exception);
                return;
            }

            //this is the code I used while trying to use MailGun API 
            //the account associated with creds I got was blocked

            // using var client = new HttpClient();
            // var request = new HttpRequestMessage() {
            // RequestUri = new Uri("https://api.eu.mailgun.net/v3/mg.brash.io/messages"),
            // Method = HttpMethod.Post,
            // };

            // var authenticationString = $"api:";
            // var base64EncodedAuthenticationString = Convert.ToBase64String(System.Text.ASCIIEncoding.UTF8.GetBytes(authenticationString));

            // request.Headers.Add("Authorization", "Basic " + base64EncodedAuthenticationString);
            // request.Headers.Add("from", "");
            // request.Headers.Add("to", "");
            // request.Headers.Add("subject", "Joke Email");
            // request.Headers.Add("text", "DAD JOKE message body");
            // HttpResponseMessage result = await client.SendAsync(request);
            // string responseBody = await result.Content.ReadAsStringAsync();
            // Console.WriteLine(responseBody); 

            try {
                SmtpClient smtpClient = new SmtpClient("glassix-hmail.westeurope.cloudapp.azure.com");
                smtpClient.UseDefaultCredentials = false;

                //!!! ======= deleted the password to the account ======= !!!

                smtpClient.Credentials = new NetworkCredential("test@glassix-spam.com","=======");
                smtpClient.Port = 587;
                smtpClient.EnableSsl = false;

                smtpClient.Send("ranmauber8@gmail.com", userEmailAddress, "Your random Dad Joke", fetchedJoke);
                Console.WriteLine("Your Dad Joke is on its way");
            }
            catch (Exception exception) {
                Console.WriteLine("CAUGHT EXCEPTION: while sending mail");
                Console.WriteLine(exception);
                return;
            }
        }
    }
}
