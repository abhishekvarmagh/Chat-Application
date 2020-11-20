using Microsoft.Extensions.Configuration;
using Model.DTO;
using Model.Models;
using Model.Utils;
using Repository.Interface;
using System;
using System.Data;
using System.Data.SqlClient;
using Model.Utils.Interface;

namespace Repository.Implementation
{
    public class UserRepository : IUserRepository
    {
        private readonly string connectionString;
        private readonly SqlConnection con;
        private readonly IConfiguration configuration;
        private readonly IMSMQ msmq;

        public UserRepository(IConfiguration configuration, IMSMQ msmq)
        {
            this.configuration = configuration;
            this.msmq = msmq;
            connectionString = configuration.GetSection("ConnectionStrings").GetSection("ChatApplicationDBConnection").Value;
            con = new SqlConnection(connectionString);
        }

        public int UserRegistration(UserRegistrationDTO userRegistrationDTO)
        {
            try
            {
                SqlCommand cmd = this.StoredProcedureCommand("spAddUser");
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@email", userRegistrationDTO.email);
                cmd.Parameters.AddWithValue("@full_name", userRegistrationDTO.fullName);
                cmd.Parameters.AddWithValue("@password", EncodePasswordToBase64(userRegistrationDTO.password));
                cmd.Parameters.AddWithValue("@phone_no", userRegistrationDTO.phoneNumber);
                cmd.Parameters.Add("@user_id", SqlDbType.Int).Direction = ParameterDirection.Output;

                con.Open();
                cmd.ExecuteNonQuery();
                string id = cmd.Parameters["@user_id"].Value.ToString();
                con.Close();
                if (id != "")
                {
                    msmq.Send("Registration", "Registered Successfully", "abhishektheverma@gmail.com");
                    return Convert.ToInt32(id);
                }
                return -1;
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
            finally
            {
                con.Close();
            }
        }

        public UserDetails UserLogin(UserLoginDTO userLoginDTO)
        {
            try
            {
                UserDetails userDetails = null;
                SqlCommand cmd = this.StoredProcedureCommand("spGetUserDetails");
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@email", userLoginDTO.email);
                con.Open();
                SqlDataReader rdr = cmd.ExecuteReader();
                if (rdr.HasRows)
                {
                    while (rdr.Read())
                    {
                        if (userLoginDTO.password.Equals(DecodeFrom64(rdr["password"].ToString())))
                        {
                            userDetails = new UserDetails();
                            userDetails.id = Convert.ToInt32(rdr["id"]);
                            userDetails.fullName = rdr["full_name"].ToString();
                            userDetails.email = rdr["email"].ToString();
                            userDetails.token = JWTToken.TokenGenerator(userDetails.id, configuration);
                        }
                        break;
                    }
                    con.Close();
                    return userDetails;
                }
                return null;
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
            finally
            {
                con.Close();
            }
        }

        public SqlCommand StoredProcedureCommand(string procedureName)
        {
            using (SqlCommand cmd = new SqlCommand(procedureName, con))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                return cmd;
            }
        }

        public string EncodePasswordToBase64(string password)
        {
            try
            {
                byte[] encData_byte = new byte[password.Length];
                encData_byte = System.Text.Encoding.UTF8.GetBytes(password);
                string encodedData = Convert.ToBase64String(encData_byte);
                return encodedData;
            }
            catch (Exception ex)
            {
                throw new Exception("Error in base64Encode" + ex.Message);
            }
        }

        public string DecodeFrom64(string encodedData)
        {
            System.Text.UTF8Encoding encoder = new System.Text.UTF8Encoding();
            System.Text.Decoder utf8Decode = encoder.GetDecoder();
            byte[] todecode_byte = Convert.FromBase64String(encodedData);
            int charCount = utf8Decode.GetCharCount(todecode_byte, 0, todecode_byte.Length);
            char[] decoded_char = new char[charCount];
            utf8Decode.GetChars(todecode_byte, 0, todecode_byte.Length, decoded_char, 0);
            string result = new String(decoded_char);
            return result;
        }
    }
}
