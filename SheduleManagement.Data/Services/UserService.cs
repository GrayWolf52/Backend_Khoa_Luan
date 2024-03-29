﻿using SheduleManagement.Data.EF;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SheduleManagement.Data.Services
{
    public class UserService
    {
        private readonly ScheduleManagementDbContext _dbContext;
        public UserService(ScheduleManagementDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        public (string, int) CheckLogin(string userName, string Password)
        {
            var user = _dbContext.Users.FirstOrDefault(x => x.UserName == userName && x.PassWord == Password);
            if (user == null)
            {
                return ("Tài khoản hoặc mật khẩu không chính xác.", 0);
            }
            return (String.Empty, user.Id);
        }
        public (string, Users) AddUser(string username, string firstname, string lastname, string password, string phone, string address, string position, string department)
        {
            try
            {
                if (_dbContext.Users.Any(x => x.UserName ==  username))
                {
                    return ("User name exist", null);
                }
                _dbContext.Users.Add(new Users
                {
                    UserName = username,
                    FirstName = firstname,
                    LastName = lastname,
                    PassWord = password,
                    Phone = phone,
                    Address = address,
                    Position = position,
                    Department = department,
                    CreateDate = DateTime.Now
                });
                _dbContext.SaveChanges();
                return (string.Empty, _dbContext.Users.Where(x => x.UserName == username).FirstOrDefault());
            }
            catch (Exception ex)
            {
                return (ex.Message, null);
            }
        }
        public (string, Users) UpdateUser(Users users)
        {
            try
            {
                var useUpdate = _dbContext.Users.Find(users.Id);
                useUpdate.FirstName = users.FirstName;
                useUpdate.LastName = users.LastName;
                useUpdate.Phone = users.Phone;
                useUpdate.Address = users.Address;
                useUpdate.Department = users.Department;
                useUpdate.Position = users.Position;
                _dbContext.SaveChanges();
                return (string.Empty, users);
            }
            catch (Exception ex)
            {
                return (ex.Message, null);
            }
        }
        public string DeleteUser(int Id)
        {
            Users userDel = null;
            try
            {
                userDel = _dbContext.Users.Find(Id);
                _dbContext.Users.Remove(userDel);
                _dbContext.SaveChanges();
                return String.Empty;
            }
            catch (Exception ex)
            {
                //return ex.Message;
                //Ghi log lỗi lại.
                return (userDel == null ? "Không tìm thấy thông tin người dùng tương ứng." : $"Xóa người dùng {userDel} không thành công.");
            }
        }
        public (string, List<Users>) Search(string prefix, List<int> exclusions)
        {
            prefix = prefix.ToLower();
            try
            {
                return (String.Empty, _dbContext.Users
                    .Where(x => (!exclusions.Contains(x.Id)) && (x.UserName.ToLower() == prefix
                    || x.UserName.ToLower().Contains(prefix)
                    || x.Phone == prefix
                    || x.Phone.Contains(prefix)
                    || (x.LastName.ToLower() + " " + x.FirstName.ToLower()) == prefix
                    || (x.LastName.ToLower() + " " + x.FirstName.ToLower()).Contains(prefix)))
                    .OrderBy(x => x.UserName.ToLower() == prefix)
                    .ThenBy(x => x.Phone == prefix)
                    .ThenBy(x => (x.LastName.ToLower() + " " + x.FirstName.ToLower()) == prefix)
                    .ThenBy(x => x.UserName)
                    .Take(3)
                    .ToList());
            }
            catch (Exception ex)
            {
                return (ex.Message, null);
            }
        }
        public string ChangePassword(int userId, string password)
        {
            try
            {
                var user = _dbContext.Users.FirstOrDefault(x => x.Id == userId);
                if (user == null) return "Không tìm thấy thông tin tài khoản tương ứng.";
                user.PassWord = password;
                _dbContext.SaveChanges();
                return String.Empty;
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }
        public (string, List<Users>) GetAll()
        {
            try
            {
                return (String.Empty, _dbContext.Users.ToList());
            }
            catch (Exception ex)
            {
                return (ex.Message, null);
            }
        }
        public (string, Users) GetById(int id)
        {
            try
            {
                return (String.Empty, _dbContext.Users.Find(id));
            }
            catch (Exception ex)
            {
                return (ex.Message, null);
            }
        }
    }
}
