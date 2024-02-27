using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using webapi.Models;
using webapi.Services;

namespace webapi.Data
{
    public class webapiContext : IdentityDbContext<User>
    {
        public webapiContext(DbContextOptions<webapiContext> options)
            : base(options)
        {

        }

        public DbSet<webapi.Models.Contact> Contacts { get; set; } = default!;

        public DbSet<webapi.Models.Donation> Donations { get; set; } = default!;

        public DbSet<webapi.Models.DonatedDevice> DonatedDevices { get; set; } = default!;

        public DbSet<webapi.Models.User> User { get; set; } = default!;

        public DbSet<DonationReview> DonationReviews { get; set; } = default!;

        public DbSet<CurrentDevice> CurrentDevices { get; set; } = default!;
        public DbSet<CurrentDeviceHistory> CurrentDevicesHistory { get; set; }

        public DbSet<Campaign> Campaigns { get; set; } = default!;

        public DbSet<Provision> Provisions { get; set; } = default!;

        public DbSet<ProvisionedDevice> ProvisionedDevices { get; set; } = default!;

        public DbSet<DeviceType> DeviceTypes { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<User>().HasAlternateKey(x => x.DisplayId);
            
        }

        public async Task InitializeDonation(ApplicationUserManager UserManager)
        {
            if (!Donations.Any())
            {
                Donation utahHealthDonation = new Donation
                {
                    DonationDate = DateTime.Today,
                    DonorName = "Utah Health",
                    DonorLocation = "Salt Lake City",
                    TotalDeviceCount = 2,
                    DonatedDevices = new List<DonatedDevice>()
                };
                DonatedDevice wheelchair = new DonatedDevice
                {

                    DeviceType = new DeviceType
                    {
                        Category = "Wheelchair",
                        CategoryNormalized = "WHEELCHAIR",
                        Size = "Adult",
                        SizeNormalized = "ADULT",
                        Type = "Normal",
                        TypeNormalized = "NORMAL"
                    },
                    DeviceCount = 2,
                    Donation = utahHealthDonation,

                };

                utahHealthDonation.DonatedDevices.Add(wheelchair);

                Donations.Add(utahHealthDonation);

                var user = UserManager.FindByNameAsync("EliHunt@gmail.com");
                if (user != null)
                {

                    DonationReview review = new DonationReview
                    {
                        DonationCreatedBy = user.Result,
                        CreatedOn = DateTime.Now,
                        DonationStatus = "ToDo",
                        Donation = utahHealthDonation
                    };
                    utahHealthDonation.DonationReview = review;
                }


                Donation interMountainDonation = new Donation
                {
                    DonationDate = DateTime.Today,
                    DonorName = "Intermountain Health",
                    DonorLocation = "Salt Lake City",
                    TotalDeviceCount = 2,
                    DonatedDevices = new List<DonatedDevice>()
                };
                DonatedDevice crutches = new DonatedDevice
                {

                    DeviceType = new DeviceType
                    {
                        Category = "Crutches",
                        CategoryNormalized = "CRUTCHES",
                        Type = "Normal",
                        TypeNormalized = "NORMAL",
                        Size = "Adult",
                        SizeNormalized = "ADULT"
                    },
                    DeviceCount = 2,
                    Donation = interMountainDonation,

                };

                interMountainDonation.DonatedDevices.Add(crutches);

                Donations.Add(interMountainDonation);


                if (user != null)
                {

                    DonationReview review = new DonationReview
                    {
                        DonationCreatedBy = user.Result,
                        CreatedOn = DateTime.Now,
                        DonationStatus = "ToDo",
                        Donation = interMountainDonation
                    };
                    interMountainDonation.DonationReview = review;
                }


                Campaign campaign = new Campaign
                {
                    CampaignName = "Navajo Campaign 2024",
                    StartDate = DateTime.Now
                };
                utahHealthDonation.Campaign = campaign;

                interMountainDonation.Campaign = campaign;

                await SaveChangesAsync();

            }
        }


        public async Task InitializeContacts()
        {
            if (!Contacts.Any())
            {
                List<string> fNames = new List<string> { "Jeff", "Max", "Matt", "Peter" };
                List<string> lNames = new List<string> { "Smith", "Johnson", "Jones", "Hill" };

                Random r = new Random();

                foreach (var firstName in fNames)
                {
                    Contact c = new Contact
                    {
                        Email = "email@test.com",
                        FullName = firstName + " " + lNames[r.Next(lNames.Count)],
                        Organization = "Utah Health",
                        PhoneNum = "555-555-5555",
                        Role = "Equipment Supervisor"
                    };
                    Contacts.Add(c);
                }

                await SaveChangesAsync();
            }
        }

        public async Task InitializeCurrentDevice()
        {
            if (!CurrentDevices.Any())
            {
                CurrentDevice device = new CurrentDevice
                {
                    DeviceType = (from d in  DeviceTypes where d.CategoryNormalized == "WHEELCHAIR" select d).First(),
                    Grade = "B",
                    Location = "Salt Lake City Warehouse".ToUpper()
                };
                CurrentDevices.Add(device);
                await SaveChangesAsync();
            }
        }


        public async Task InitializeUser(ApplicationUserManager UserManager)
        {
            if (!UserManager.Users.Any())
            {

                var user = new User
                {
                    FullName = "John Smith",
                    Title = "One Time Volunteer",
                    Department = "Volunteer",
                    Email = "JohnSmith@gmail.com",
                    EmailConfirmed = true,
                    StartDate = DateTime.Now,
                    UserName = "JohnSmith@gmail.com",
                    PhoneNumber = "555-555-5555"
                };

                var user2 = new User
                {
                    FullName = "Eli Hunt",
                    Title = "Head Admin",
                    Department = "HQ",
                    Email = "EliHunt@gmail.com",
                    EmailConfirmed = true,
                    StartDate = DateTime.Now,
                    UserName = "EliHunt@gmail.com",
                    PhoneNumber = "555-555-5555"
                };

                var user3 = new User
                {
                    FullName = "Alex Appell",
                    Title = "Warehouse Manager",
                    Department = "Warehouse",
                    Email = "AlexAppell@email.com",
                    EmailConfirmed = true,
                    StartDate = DateTime.Now,
                    UserName = "AlexAppell@email.com",
                    PhoneNumber = "555-555-5555"
                };

                await UserManager.CreateAsync(user, "Password1@");
                await UserManager.CreateAsync(user2, "Password1@");
                await UserManager.CreateAsync(user3, "Password1@");

            }


        }


        public async Task InitializeRole(RoleManager<IdentityRole> roleManager, ApplicationUserManager userManager)
        {
            if (!roleManager.Roles.Any())
            {
                await roleManager.CreateAsync(new IdentityRole("Admin"));
                await roleManager.CreateAsync(new IdentityRole("Reviewer"));
                await roleManager.CreateAsync(new IdentityRole("Employee"));


                var user = await userManager.FindByNameAsync("JohnSmith@gmail.com");

                await userManager.AddToRoleAsync(user, "Employee");

                await userManager.AddToRoleAsync(await userManager.FindByNameAsync("AlexAppell@email.com"), "Reviewer");


                await userManager.AddToRoleAsync(await userManager.FindByNameAsync("EliHunt@gmail.com"), "Admin");

            }


        }




    }
}