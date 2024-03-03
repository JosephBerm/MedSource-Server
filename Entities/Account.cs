﻿using IdentityModel;
using Microsoft.IdentityModel.Tokens;
using prod_server.Classes.Others;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Text.Json.Serialization;
using BCryptNet = BCrypt.Net.BCrypt;


namespace prod_server.Entities
{
    [Table("Accounts")]
    public class Account
    {
        public Account() { }
        public Account (RegisterModel registerModel)
        {
            Username = registerModel.Username;
            Password = registerModel.Password;
            Email = registerModel.Email;
            FirstName = registerModel.FirstName;
            LastName = registerModel.LastName;
        }

        [Key]
        [Column("id")]
        public int Id { get; set; }

        [Column("username")]
        [MaxLength(50)]
        public string Username { get; set; }

        [Required]
        [JsonIgnore]
        [Column("password")]
        /// <summary> Password is hashed and salted. </summary>
        public string Password { get; set; }

        [Required]
        [Column("email_address")]
        [MaxLength(50)]
        public string Email { get; set; }

        [AllowNull]
        [Column("first_name")]
        [MaxLength(50)]
        public string? FirstName { get; set; }

        [AllowNull]
        [Column("last_name")]
        [MaxLength(50)]
        public string? LastName { get; set; }

        public string CreateJwtToken()
        {
            List<Claim> claims = new List<Claim>
            {
                new Claim(JwtClaimTypes.Subject, Id.ToString()),
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("33d7b68dc5eab0934f001fc5801bd234a255aa0e7314cb43619e5604001132ece77b4a73f0fd8c67d89e43662d2d968d2b18bd20c8dbc78ac5512ccf41f381cb"));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);
            var jwtToken = new JwtSecurityToken(
                    claims: claims,
                    expires: DateTime.UtcNow.AddDays(1),
                    signingCredentials: credentials,
                    issuer: "localhost",
                    audience: "localhost"
                );
            var jetTokenString = new JwtSecurityTokenHandler().WriteToken(jwtToken);
            return jetTokenString;
        }

        public bool ValidatePassword (string password)
        {
            return BCryptNet.Verify(password, Password);
        }
    }
}
