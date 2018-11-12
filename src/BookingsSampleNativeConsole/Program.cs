// ---------------------------------------------------------------------------
// <copyright file="Program.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
// ---------------------------------------------------------------------------

namespace BookingsSampleNativeConsole
{
    using System;

    using Microsoft.Bookings.Client;
    using Microsoft.IdentityModel.Clients.ActiveDirectory;
    using Microsoft.OData.Client;
    using System.Linq;
    using System.Net;

    public class Program
    {
        // See README.MD for instructions on how to get your own values for these two settings.
        // See also https://docs.microsoft.com/en-us/azure/active-directory/develop/active-directory-authentication-scenarios#native-application-to-web-api
        private static string clientApplicationAppId = "2e654850-6c44-47ac-acdd-2a471afd4216";
        private static Uri clientApplicationRedirectUri = new Uri("https://login.microsoftonline.com/common/oauth2/nativeclient") ;

        public static void Main()
        {
            try
            {

                // ADAL: https://docs.microsoft.com/en-us/azure/active-directory/develop/active-directory-authentication-libraries
                var authenticationContext = new AuthenticationContext(GraphService.DefaultAadInstance, TokenCache.DefaultShared);
                var authenticationResult = authenticationContext.AcquireTokenAsync(
                    GraphService.ResourceId,
                    clientApplicationAppId,
                    clientApplicationRedirectUri,
                    new PlatformParameters(PromptBehavior.RefreshSession)).Result;

                var graphService = new GraphService(
                    GraphService.ServiceRoot,
                    () => authenticationResult.CreateAuthorizationHeader());

               
                var bookingBusinesses = graphService.BookingBusinesses.ToArray();
                foreach (var _ in bookingBusinesses)
                {
                    Console.WriteLine(_.DisplayName);
                }

                
                Console.WriteLine("Type the name of the booking business to create a new Appointment , exit type enter");
                

                var businessName = Console.ReadLine();
                if (string.IsNullOrWhiteSpace(businessName))
                {
                    return;
                }

               
                var bookingBusiness = bookingBusinesses.FirstOrDefault(_ => _.DisplayName == businessName);
                
                var business = graphService.BookingBusinesses.ByKey(bookingBusiness.Id);
               /*
                var newAppointment = business.Appointments.NewEntityWithChangeTracking();
                newAppointment.CustomerEmailAddress = "test@wicresoft.com";
                newAppointment.CustomerName = "customer11";
                
                newAppointment.ServiceId = business.Services.First().Id; // assuming we didn't deleted all services; we might want to double check first like we did with staff.

                // Add multiple members (all members in this case ) to an Appointment
                foreach (var member in business.StaffMembers)
                {
                    newAppointment.StaffMemberIds.Add(member.Id);
                }

                newAppointment.Reminders.Add(new BookingReminder { Message = "Hello", Offset = TimeSpan.FromHours(1), Recipients = BookingReminderRecipients.AllAttendees });
                var start = DateTime.Today.AddDays(1).AddHours(13).ToUniversalTime();
                var end = start.AddHours(1);
                newAppointment.Start = new DateTimeTimeZone { DateTime = start.ToString("o"), TimeZone = "UTC" };
                newAppointment.End = new DateTimeTimeZone { DateTime = end.ToString("o"), TimeZone = "UTC" };
                Console.WriteLine("Creating appointment...");
                graphService.SaveChanges(SaveChangesOptions.PostOnlySetProperties);
                Console.WriteLine("Appointment created.");

                foreach (var appointment in business.Appointments.GetAllPages())
                {
                    Console.WriteLine($"{DateTime.Parse(appointment.Start.DateTime).ToLocalTime()}: {appointment.ServiceName} with {appointment.CustomerName}");
                }

                Console.WriteLine("Publishing booking business public page...");
                business.Publish().Execute();

                Console.WriteLine(business.GetValue().PublicUrl);
                */

                //cretae customers
                createCustomers(graphService, business);


            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }

            Console.WriteLine("Done. Press any key to exit.");
            Console.ReadKey();
        }


        static void createCustomers(GraphService graphService, BookingBusinessSingle business)
        {
            BookingCustomer customer = business.Customers.NewEntityWithChangeTracking();
            customer.DisplayName = "customer6";
            customer.EmailAddress = "test@163.com";
            graphService.SaveChanges(SaveChangesOptions.PostOnlySetProperties);

            Console.WriteLine("done");
        }
    }
}