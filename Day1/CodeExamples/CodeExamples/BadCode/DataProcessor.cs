using Microsoft.Data.SqlClient;

namespace CodeExamples.BadCode;

public class DataProcessor
{
    public static string connectionString = "Server=myServerAddress;Database=myDataBase;User Id=myUsername;Password=myPassword;";
    
    
    public List<int> ProcessData(List<string> data)
    {
        var result = new List<int>();
        for(int i = 0; i < data.Count; i++)
        {
            if(data[i] != null && data[i] != "")
            {
                try {
                    int val = Int32.Parse(data[i]);
                    result.Add(val * 2);
                }
                catch(Exception ex) {
                    Console.WriteLine("Error: " + ex.Message);
                }
            }
        }
        return result;
    }
    
    public void SaveToDatabase(List<int> items)
    {
        SqlConnection conn = new SqlConnection(connectionString);
        conn.Open();
        foreach(var item in items)
        {
            string sql = "INSERT INTO Items (Value) VALUES (" + item + ")";
            SqlCommand cmd = new SqlCommand(sql, conn);
            cmd.ExecuteNonQuery();
            cmd.Dispose();
        }
        conn.Close();
    }
}

// Spaghetti code with deep nesting
public bool ValidateUser(string username, string password, bool isAdmin)
{
    if(username != null)
    {
        if(username.Length >= 4)
        {
            if(password != null)
            {
                if(password.Length >= 8)
                {
                    if(isAdmin)
                    {
                        if(username == "admin" && password == "admin123")
                        {
                            return true;
                        }
                        else
                        {
                            return false;
                        }
                    }
                    else
                    {
                        return true;
                    }
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
        }
        else
        {
            return false;
        }
    }
    else
    {
        return false;
    }
}