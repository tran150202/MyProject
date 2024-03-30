﻿using Dapper;
using Microsoft.VisualBasic;
using SV20T1020598.DomainModels;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace SV20T1020598.DataLayers.SQLServer
{
    public class EmployeeAccountDAL : _BaseDAL, IUserAccountDAL
    {
        public EmployeeAccountDAL(string connectionString) : base(connectionString)
        {
        }
        public UserAccount Authorize(string userName, string password)
        {
            UserAccount data;
            using (var cn = OpenConnection())
            {
                var sql = @"select EmployeeID as UserID, Email as UserName, FullName, Email, Photo, Password,RoleNames
                from Employees where Email = @Email AND Password = @Password";
                var parameters = new
                {
                    Email = userName,
                    Password = password,
                };
                data = cn.QuerySingleOrDefault<UserAccount>(sql, parameters);
                cn.Close();
            }
            return data;
        }

        public bool ChangePassword(string userName, string oldPassword, string newPassword)
        {
            bool result = false;
            using (var cn = OpenConnection())
            {
                var sql = @"update Employees 
                            set Password=@NewPassword 
                            where Email=@Email and Password=@OldPassword";
                var parameters = new
                {
                    Email = userName,
                    OldPassword = oldPassword,
                    NewPassword = newPassword
                };
                result = cn.Execute(sql, parameters) > 0;
                cn.Close();
            }
            return result;
        }
        public string? GetPassword(string userName)
        {
            string passWord = "";
            using (var connection = OpenConnection())
            {
                var sql = @"select PassWord from Employees where Email = @Email";
                var parameters = new
                {
                    Email = userName
                };
                passWord = connection.QueryFirstOrDefault<string>(sql: sql, param: parameters,
                                                                commandType: CommandType.Text);
                connection.Close();
            }
            return passWord;
        }
    }
}
