using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using log4net;
using System.Data;
using System.Collections;
using System.Configuration;
using System.IO;
using System.Xml;
using System.Xml.Serialization;

using System.Data.SqlClient;
using System.Data.SqlTypes;


namespace DatabaseHelper
{
    public class DataEntityHelper
    {
        public static string CONNECTION_STRING = (string)ConfigurationManager.AppSettings["ConnectionString"] == null ? "" : ConfigurationManager.AppSettings["ConnectionString"];

        private static ILog Log = LogManager.GetLogger(typeof(DataEntityHelper));


        static public Dictionary<String, String> insertRecords  (Dictionary<String, String>  addresses )
        {
           Dictionary<String, String> failed = new Dictionary<String, String>();
           try
           {

               using (SqlConnection DbConnection = new SqlConnection(CONNECTION_STRING))
               {
                   DbConnection.Open();

                   foreach (var key in addresses.Keys)
                   {

                       string directEmailAddress = key;
                       string forwardingEmailAddress = addresses[key];
                     
                      
                       Console.WriteLine("Now working on direct address " +key + "----->" + addresses[key]);
                       try
                       {
                           using (SqlCommand sqlCommand = new SqlCommand("AddForwardingRule", DbConnection))
                           {
                               sqlCommand.CommandType = CommandType.StoredProcedure;
                               sqlCommand.CommandTimeout = 300; 
                               sqlCommand.Parameters.AddWithValue("@DirectEmailAddress", directEmailAddress);
                               sqlCommand.Parameters.AddWithValue("@ForwardingEmailAddress", forwardingEmailAddress);


                               // Your output parameter in Stored Procedure           
                               var outputParameter = new SqlParameter
                               {
                                   ParameterName = "@resultText",
                                   Direction = ParameterDirection.Output,
                                   DbType=(DbType)SqlDbType.VarChar,
                                   Size = 256
                               }; 
                               sqlCommand.Parameters.Add(outputParameter);
                               sqlCommand.ExecuteNonQuery();
                               string returnValue = sqlCommand.Parameters["@resultText"].Value.ToString();  

                                
                               if (returnValue == null)
                               {
                                   Log.Info(String.Format("No rule applied for {0} to {1}  ", directEmailAddress, forwardingEmailAddress ));
                               }
                               else
                               {

                                   string returnText = (string)returnValue;
                               
                               

                                 Log.Info(String.Format("For forwarding rule {0} to {1} result was: {2}  ", directEmailAddress, forwardingEmailAddress, returnText));

                                if (!returnText.Contains("1"))
                                {
                                    failed.Add(directEmailAddress, forwardingEmailAddress);
                                    

                                }
                               }
                 
                           }

                       }
                       catch (SqlException ex)
                       {
                           failed.Add(directEmailAddress, forwardingEmailAddress);
                           Log.Error(String.Format("No Rule could be set-DirectEmailAddress:{0},forwardingAddress:{1}", directEmailAddress, forwardingEmailAddress), ex);
                            
                        }

                   }
                   if (failed.Count>0){

                       Log.DebugFormat("*******************************************************************************************************");
                       Log.DebugFormat("Not all forwading rules were set thru this program for the following addresses");
                       Log.DebugFormat("*******************************************************************************************************");
                   }
                   foreach (var key in failed.Keys)
                   {
                       string directEmailAddress = key;
                       string forwardingEmailAddress = addresses[key]; 
                       Log.DebugFormat("DirectEmailAddress:{0},forwardingAddress:{1}", directEmailAddress, forwardingEmailAddress);

                   }



               }

            }
              catch (SqlException ex)
                       {
                            
                           Log.Error(String.Format("Could not create records  " ));
                           return  addresses;
                       }


           return failed;
        } 
    }
}
