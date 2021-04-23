using BlogsAPI.Models;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BlogsAPI.DataManagers
{
    public static class BlogsDataManager
    {
        public static async Task<IEnumerable<Blog>> GetAsync(string connectionString)
        {

            var blogs = new List<Blog>();            
            var sp = "spBlogsGetAll";

            try
            {
                using (var connection = new MySqlConnection(connectionString))
                {

                    await connection.OpenAsync();
                    var command = new MySqlCommand(sp, connection);
                    command.CommandType = System.Data.CommandType.StoredProcedure;
                    var reader = await command.ExecuteReaderAsync();
                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            var blog = new Blog
                            {
                                Title = reader.GetString(0),
                                Content = reader.GetString(1)
                                
                            };

                            blogs.Add(blog);

                        }

                    }

                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return blogs;
        }

        public static async Task<bool> CreateAsync(Blog blog, string connectionString)
        {
            ulong count = 0;
            var sp = "spBlogsInsert";

            try
            {
                using (var connection = new MySqlConnection(connectionString))
                {
                    await connection.OpenAsync();
                    var command = new MySqlCommand(sp, connection);
                    command.CommandType = System.Data.CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@title", blog.Title);
                    command.Parameters.AddWithValue("@content", blog.Content);
                   

                    count = (ulong)command.ExecuteScalarAsync().Result;

                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }

            if (count > 0)
                return true;
            else
                return false;
        }
    }
}
