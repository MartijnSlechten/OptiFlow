using Microsoft.VisualStudio.TestTools.UnitTesting;
using OptiflowApi.Controllers;
using OptiflowApi.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace TestApi
{
    [TestClass]
    public class UnitTestUserController
    {
        UsersController usersController = new UsersController("usertest");
        User user = new User(0, "Jan", "Smith", "jan.smith@test.com", "jantest");
        User user2 = new User(1, "Mieke", "Janssen", "mieke.janssen@test.com", "mieketest");
        User user3 = new User(0, "Jantje", "Janssen", "jan.smith@test.com", "jantest");
        User user4 = new User(0, "Jantje", "Janssen", "jantje.janssen@test.com", "jantest");
        User user5 = new User(0, "Jantje", "Janssen", "mieke.janssen@test.com", "jantest");

        public async Task CleanTable()
        {
            int amount = await usersController.CountUsers();
            int lastIndex = amount - 1;

            for (var i = lastIndex; i >= 0; i--)
            {
                User deletedUser = await usersController.DeleteUserById(i);
            }
        }

        [TestMethod]
        public async Task FindUserById_WithExistingId_ReturnsUser()
        {
            bool found;

            await CleanTable();

            User createdUser = await usersController.CreateUser(user);
            User foundUser = await usersController.FindUserById(0);

            if (foundUser == null)
            {
                found = false;
            }
            else
            {
                found = true;
            }

            Assert.IsTrue(found);
            Assert.AreEqual(user.FirstName, foundUser.FirstName);
            Assert.AreEqual(user.LastName, foundUser.LastName);
            Assert.AreEqual(user.Email, foundUser.Email);
            Assert.AreEqual(Encryptor.MD5Hash(user.Password), foundUser.Password);
        }

        [TestMethod]
        public async Task FindUserById_WithNotExistingId_ReturnsNull()
        {
            bool found;

            await CleanTable();

            User foundUser = await usersController.FindUserById(5);

            if (foundUser == null)
            {
                found = false;
            }
            else
            {
                found = true;
            }

            Assert.IsFalse(found);
            Assert.AreEqual(null, foundUser);
        }

        [TestMethod]
        public async Task FindUserByEmail_WithExistingEmail_ReturnsUser()
        {
            bool found;

            await CleanTable();

            User createdUser = await usersController.CreateUser(user);
            User foundUser = await usersController.FindUserByEmail("jan.smith@test.com");

            if (foundUser == null)
            {
                found = false;
            }
            else
            {
                found = true;
            }

            Assert.IsTrue(found);
            Assert.AreEqual(user.FirstName, foundUser.FirstName);
            Assert.AreEqual(user.LastName, foundUser.LastName);
            Assert.AreEqual(user.Email, foundUser.Email);
            Assert.AreEqual(Encryptor.MD5Hash(user.Password), foundUser.Password);
        }

        [TestMethod]
        public async Task FindUserByEmail_WithNotExistingEmail_ReturnsNull()
        {
            bool found;

            await CleanTable();

            User foundUser = await usersController.FindUserByEmail("jantje.kapoentje@test.com");

            if (foundUser == null)
            {
                found = false;
            }
            else
            {
                found = true;
            }

            Assert.IsFalse(found);
            Assert.AreEqual(null, foundUser);
        }

        [TestMethod]
        public async Task CountUsers_WithZeroUsersInTable_ReturnsZero()
        {
            await CleanTable();

            int amount = await usersController.CountUsers();

            Assert.AreEqual(0, amount);
        }

        [TestMethod]
        public async Task CountUsers_WithOneUserInTable_ReturnsOne()
        {
            await CleanTable();

            User createdUser = await usersController.CreateUser(user);
            int amount = await usersController.CountUsers();

            Assert.AreEqual(1, amount);
        }

        [TestMethod]
        public async Task CountUsers_WithTwoUsersInTable_ReturnsTwo()
        {
            await CleanTable();

            User createdUser = await usersController.CreateUser(user);
            User createdUser2 = await usersController.CreateUser(user2);
            int amount = await usersController.CountUsers();

            Assert.AreEqual(2, amount);
        }

        [TestMethod]
        public async Task GetLastUser_WithZeroUsersInTable_ReturnsNull()
        {
            bool found;

            await CleanTable();

            User foundUser = await usersController.GetLastUser();

            if (foundUser == null)
            {
                found = false;
            }
            else
            {
                found = true;
            }

            Assert.IsFalse(found);
            Assert.AreEqual(null, foundUser);
        }

        [TestMethod]
        public async Task GetLastUser_WithOneUserInTable_ReturnsUser()
        {
            bool found;

            await CleanTable();

            User createdUser = await usersController.CreateUser(user);
            User foundUser = await usersController.GetLastUser();

            if (foundUser == null)
            {
                found = false;
            }
            else
            {
                found = true;
            }

            Assert.IsTrue(found);
            Assert.AreEqual(user.FirstName, foundUser.FirstName);
            Assert.AreEqual(user.LastName, foundUser.LastName);
            Assert.AreEqual(user.Email, foundUser.Email);
            Assert.AreEqual(Encryptor.MD5Hash(user.Password), foundUser.Password);
        }

        [TestMethod]
        public async Task GetLastUser_WithTwoUsersInTable_ReturnsSecondUserInTable()
        {
            bool found;

            await CleanTable();

            User createdUser = await usersController.CreateUser(user);
            User createdUser2 = await usersController.CreateUser(user2);
            User foundUser = await usersController.GetLastUser();

            if (foundUser == null)
            {
                found = false;
            }
            else
            {
                found = true;
            }

            Assert.IsTrue(found);
            Assert.AreEqual(user2.FirstName, foundUser.FirstName);
            Assert.AreEqual(user2.LastName, foundUser.LastName);
            Assert.AreEqual(user2.Email, foundUser.Email);
            Assert.AreEqual(Encryptor.MD5Hash(user2.Password), foundUser.Password);
        }

        [TestMethod]
        public async Task AuthenticateUser_WithRightEmailAndRightPassword_ReturnsTrue()
        {
            await CleanTable();

            User createdUser = await usersController.CreateUser(user);
            bool authenticated = await usersController.AuthenticateUser("jan.smith@test.com", Encryptor.MD5Hash("jantest"));

            Assert.IsTrue(authenticated);
        }

        [TestMethod]
        public async Task AuthenticateUser_WithWrongEmailAndRightPassword_ReturnsFalse()
        {
            await CleanTable();

            User createdUser = await usersController.CreateUser(user);
            bool authenticated = await usersController.AuthenticateUser("jantje.kapoentje@test.com", Encryptor.MD5Hash("jantest"));

            Assert.IsFalse(authenticated);
        }

        [TestMethod]
        public async Task AuthenticateUser_WithRightEmailAndWrongPassword_ReturnsFalse()
        {
            await CleanTable();

            User createdUser = await usersController.CreateUser(user);
            bool authenticated = await usersController.AuthenticateUser("jan.smith@test.com", Encryptor.MD5Hash("jantjetest"));

            Assert.IsFalse(authenticated);
        }

        [TestMethod]
        public async Task AuthenticateUser_WithWrongEmailAndWrongPassword_ReturnsFalse()
        {
            await CleanTable();

            User createdUser = await usersController.CreateUser(user);
            bool authenticated = await usersController.AuthenticateUser("jantje.kapoentje@test.com", Encryptor.MD5Hash("jantjetest"));

            Assert.IsFalse(authenticated);
        }

        [TestMethod]
        public async Task GetUsers_WithZeroUsersInTable_ReturnsEmptyList()
        {
            int amount = 0;

            await CleanTable();

            List<User> users = await usersController.GetUsers();
            
            foreach (User user in users)
            {
                amount++;
            }

            Assert.AreEqual(0, amount);
        }

        [TestMethod]
        public async Task GetUsers_WithOneUserInTable_ReturnsListWithOneUser()
        {
            int amount = 0;
            List<User> usersInList = new List<User>();

            usersInList.Add(user);
            await CleanTable();

            User createdUser = await usersController.CreateUser(user);
            List<User> users = await usersController.GetUsers();

            foreach (User user in users)
            {
                amount++;
            }

            Assert.AreEqual(1, amount);

            for (int i = 0; i < amount; i++)
            {
                User expectedUser = usersInList[i];
                User actualUser = users[i];

                Assert.AreEqual(expectedUser.FirstName, actualUser.FirstName);
                Assert.AreEqual(expectedUser.LastName, actualUser.LastName);
                Assert.AreEqual(expectedUser.Email, actualUser.Email);
                Assert.AreEqual(Encryptor.MD5Hash(expectedUser.Password), actualUser.Password);
            }
        }

        [TestMethod]
        public async Task GetUsers_WithTwoUsersInTable_ReturnsListWithTwoUsers()
        {
            int amount = 0;
            List<User> usersInList = new List<User>();

            usersInList.Add(user);
            usersInList.Add(user2);
            await CleanTable();

            User createdUser = await usersController.CreateUser(user);
            User createdUser2 = await usersController.CreateUser(user2);
            List<User> users = await usersController.GetUsers();

            foreach (User user in users)
            {
                amount++;
            }

            Assert.AreEqual(2, amount);

            for (int i = 0; i < amount; i++)
            {
                User expectedUser = usersInList[i];
                User actualUser = users[i];

                Assert.AreEqual(expectedUser.FirstName, actualUser.FirstName);
                Assert.AreEqual(expectedUser.LastName, actualUser.LastName);
                Assert.AreEqual(expectedUser.Email, actualUser.Email);
                Assert.AreEqual(Encryptor.MD5Hash(expectedUser.Password), actualUser.Password);
            }
        }

        [TestMethod]
        public async Task GetUserByEmail_WithExistingEmail_ReturnsUser()
        {
            bool found;

            await CleanTable();

            User createdUser = await usersController.CreateUser(user);
            User foundUser = await usersController.GetUserByEmail("jan.smith@test.com");

            if (foundUser == null)
            {
                found = false;
            }
            else
            {
                found = true;
            }

            Assert.IsTrue(found);
            Assert.AreEqual(user.FirstName, foundUser.FirstName);
            Assert.AreEqual(user.LastName, foundUser.LastName);
            Assert.AreEqual(user.Email, foundUser.Email);
            Assert.AreEqual(Encryptor.MD5Hash(user.Password), foundUser.Password);
        }

        [TestMethod]
        public async Task GetUserByEmail_WithNotExistingEmail_ReturnsNull()
        {
            bool found;

            await CleanTable();

            User foundUser = await usersController.GetUserByEmail("jantje.kapoentje@test.com");

            if (foundUser == null)
            {
                found = false;
            }
            else
            {
                found = true;
            }

            Assert.IsFalse(found);
            Assert.AreEqual(null, foundUser);
        }

        [TestMethod]
        public async Task GetUserById_WithExistingId_ReturnsUser()
        {
            bool found;

            await CleanTable();

            User createdUser = await usersController.CreateUser(user);
            User foundUser = await usersController.GetUserById(0);

            if (foundUser == null)
            {
                found = false;
            }
            else
            {
                found = true;
            }

            Assert.IsTrue(found);
            Assert.AreEqual(user.FirstName, foundUser.FirstName);
            Assert.AreEqual(user.LastName, foundUser.LastName);
            Assert.AreEqual(user.Email, foundUser.Email);
            Assert.AreEqual(Encryptor.MD5Hash(user.Password), foundUser.Password);
        }

        [TestMethod]
        public async Task GetUserById_WithNotExistingId_ReturnsNull()
        {
            bool found;

            await CleanTable();

            User foundUser = await usersController.GetUserById(5);

            if (foundUser == null)
            {
                found = false;
            }
            else
            {
                found = true;
            }

            Assert.IsFalse(found);
            Assert.AreEqual(null, foundUser);
        }

        [TestMethod]
        public async Task CreateUser_EmailNotUsedYet_ReturnsUser()
        {
            await CleanTable();

            User createdUser = await usersController.CreateUser(user);

            Assert.AreEqual(user.FirstName, createdUser.FirstName);
            Assert.AreEqual(user.LastName, createdUser.LastName);
            Assert.AreEqual(user.Email, createdUser.Email);
            Assert.AreEqual(Encryptor.MD5Hash(user.Password), createdUser.Password);
        }

        [TestMethod]
        [ExpectedException(typeof(Exception), "User with e-mail already exists!")]
        public async Task CreateUser_EmailAlreadyUsed_ReturnsException()
        {
            await CleanTable();

            User createdUser = await usersController.CreateUser(user);
            User createdUser2 = await usersController.CreateUser(user);
        }

        [TestMethod]
        public async Task UpdateUserByEmail_WithExistingEmail_KeepSameEmail_ReturnsUser()
        {
            await CleanTable();

            User createdUser = await usersController.CreateUser(user);
            User updatedUser = await usersController.UpdateUserByEmail("jan.smith@test.com", user3);

            Assert.AreEqual(user3.FirstName, updatedUser.FirstName);
            Assert.AreEqual(user3.LastName, updatedUser.LastName);
            Assert.AreEqual(user3.Email, updatedUser.Email);
            Assert.AreEqual(Encryptor.MD5Hash(user3.Password), updatedUser.Password);
        }

        [TestMethod]
        public async Task UpdateUserByEmail_WithExistingEmail_ReplaceEmailByNotExistingEmail_ReturnsUser()
        {
            await CleanTable();

            User createdUser = await usersController.CreateUser(user);
            User updatedUser = await usersController.UpdateUserByEmail("jan.smith@test.com", user4);

            Assert.AreEqual(user4.FirstName, updatedUser.FirstName);
            Assert.AreEqual(user4.LastName, updatedUser.LastName);
            Assert.AreEqual(user4.Email, updatedUser.Email);
            Assert.AreEqual(Encryptor.MD5Hash(user4.Password), updatedUser.Password);
        }

        [TestMethod]
        [ExpectedException(typeof(Exception), "User with e-mail already exists!")]
        public async Task UpdateUserByEmail_WithExistingEmail_ReplaceEmailByExistingEmail_ReturnsException()
        {
            await CleanTable();

            User createdUser = await usersController.CreateUser(user);
            User createdUser2 = await usersController.CreateUser(user2);
            User updatedUser = await usersController.UpdateUserByEmail("jan.smith@test.com", user5);
        }

        [TestMethod]
        public async Task UpdateUserByEmail_WithNotExistingEmail_ReturnsNull()
        {
            await CleanTable();

            User createdUser = await usersController.CreateUser(user);
            User updatedUser = await usersController.UpdateUserByEmail("jantje.kapoentje@test.com", user2);

            Assert.AreEqual(null, updatedUser);
        }

        [TestMethod]
        public async Task UpdateUserById_WithExistingId_KeepSameEmail_ReturnsUser()
        {
            await CleanTable();

            User createdUser = await usersController.CreateUser(user);
            User updatedUser = await usersController.UpdateUserById(0, user3);

            Assert.AreEqual(user3.FirstName, updatedUser.FirstName);
            Assert.AreEqual(user3.LastName, updatedUser.LastName);
            Assert.AreEqual(user3.Email, updatedUser.Email);
            Assert.AreEqual(Encryptor.MD5Hash(user3.Password), updatedUser.Password);
        }

        [TestMethod]
        public async Task UpdateUserById_WithExistingId_ReplaceEmailByNotExistingEmail_ReturnsUser()
        {
            await CleanTable();

            User createdUser = await usersController.CreateUser(user);
            User updatedUser = await usersController.UpdateUserById(0, user4);

            Assert.AreEqual(user4.FirstName, updatedUser.FirstName);
            Assert.AreEqual(user4.LastName, updatedUser.LastName);
            Assert.AreEqual(user4.Email, updatedUser.Email);
            Assert.AreEqual(Encryptor.MD5Hash(user4.Password), updatedUser.Password);
        }

        [TestMethod]
        [ExpectedException(typeof(Exception), "User with e-mail already exists!")]
        public async Task UpdateUserById_WithExistingId_ReplaceEmailByExistingEmail_ReturnsException()
        {
            await CleanTable();

            User createdUser = await usersController.CreateUser(user);
            User createdUser2 = await usersController.CreateUser(user2);
            User updatedUser = await usersController.UpdateUserById(0, user5);
        }

        [TestMethod]
        public async Task UpdateUserById_WithNotExistingId_ReturnsNull()
        {
            await CleanTable();

            User createdUser = await usersController.CreateUser(user);
            User updatedUser = await usersController.UpdateUserById(5, user2);

            Assert.AreEqual(null, updatedUser);
        }

        [TestMethod]
        public async Task DeleteUserByEmail_WithExistingEmail_ReturnsUser()
        {
            await CleanTable();

            User createdUser = await usersController.CreateUser(user);
            User deletedUser = await usersController.DeleteUserByEmail("jan.smith@test.com");

            Assert.AreEqual(user.FirstName, deletedUser.FirstName);
            Assert.AreEqual(user.LastName, deletedUser.LastName);
            Assert.AreEqual(user.Email, deletedUser.Email);
            Assert.AreEqual(Encryptor.MD5Hash(user.Password), deletedUser.Password);
        }

        [TestMethod]
        public async Task DeleteUserByEmail_WithNotExistingEmail_ReturnsNull()
        {
            await CleanTable();

            User createdUser = await usersController.CreateUser(user);
            User deletedUser = await usersController.DeleteUserByEmail("jantje.kapoentje@test.com");

            Assert.AreEqual(null, deletedUser);
        }

        [TestMethod]
        public async Task DeleteUserById_WithExistingId_ReturnsUser()
        {
            await CleanTable();

            User createdUser = await usersController.CreateUser(user);
            User deletedUser = await usersController.DeleteUserById(0);

            Assert.AreEqual(user.FirstName, deletedUser.FirstName);
            Assert.AreEqual(user.LastName, deletedUser.LastName);
            Assert.AreEqual(user.Email, deletedUser.Email);
            Assert.AreEqual(Encryptor.MD5Hash(user.Password), deletedUser.Password);
        }

        [TestMethod]
        public async Task DeleteUserById_WithNotExistingId_ReturnsNull()
        {
            await CleanTable();

            User createdUser = await usersController.CreateUser(user);
            User deletedUser = await usersController.DeleteUserById(5);

            Assert.AreEqual(null, deletedUser);
        }
    }
}
