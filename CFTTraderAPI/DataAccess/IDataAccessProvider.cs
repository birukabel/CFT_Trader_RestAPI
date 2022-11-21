using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CFTTraderAPI.DataAccess
{
    public interface IDataAccessProvider
    {
        public DataSet ExecuteDataSet(string strConnectionString, string strSchema, string strStoredProcedureName, ArrayList arrListParamName, ArrayList arrListParamValue, ref string strErrMsg);
        public DataTable ExecuteDataTable(string strConnectionString, string strSchema, string strStoredProcedureName, ArrayList arrListParamName, ArrayList arrListParamValue, ref string strErrMsg);
        public DataTable ExecuteDataTable(string strConnectionString, string strSchema, string strStoredProcedureName, ArrayList arrListParamName, ArrayList arrListParamValue, ArrayList arrListParamNameForUserDefinedTypes, List<DataTable> lstParameterValuesForUserDefinedTypes, ArrayList arrListParamterTypeNameForUserDefinedTypes, ref string strErrMsg);

        public  DataSet ExecuteDataSet(string strConnectionString, string strSchema, string strStoredProcedureName, ArrayList arrListParamName, ArrayList arrListParamValue, ArrayList arrListParamNameForUserDefinedTypes, List<DataTable> lstParameterValuesForUserDefinedTypes, ArrayList arrListParamterTypeNameForUserDefinedTypes, ref string strErrMsg);

        /// <summary>
        /// Retrieve Non Parameter Storeprocedures
        /// </summary>
        /// <param name="strConnectionString"></param>
        /// <param name="strSchema"></param>
        /// <param name="strStoredProcedureName"></param>
        /// <param name="strErrMsg"></param>
        /// <returns></returns>
        public  DataTable ExecuteDataTable(string strConnectionString, string strSchema, string strStoredProcedureName, ref string strErrMsg);


        /// <summary>
        /// Method that executes stored procedure with Insert, Delete, and Update methods
        /// </summary>
        /// <param name="strConnectionString"></param>
        /// <param name="strSchema"></param>
        /// <param name="strStoredProcedureName"></param>
        /// <param name="arrListParamName"></param>
        /// <param name="arrListParamValue"></param>
        /// <param name="strErrMsg"></param>
        /// <returns></returns>
        public  bool ExecuteNonQuery(string strConnectionString, string strSchema, string strStoredProcedureName, ArrayList arrListParamName, ArrayList arrListParamValue, ref string strErrMsg);

        /// <summary>
        /// Methods to return single value for example returning username by userid
        /// </summary>
        /// <param name="strConnectionString"></param>
        /// <param name="strSchema"></param>
        /// <param name="strStoredProcedureName"></param>
        /// <param name="arrListParamName"></param>
        /// <param name="arrListParamValue"></param>
        /// <param name="strErrMsg"></param>
        /// <returns></returns>
        public  object ExecuteScalar(string strConnectionString, string strSchema, string strStoredProcedureName, ArrayList arrListParamName, ArrayList arrListParamValue, ref string strErrMsg);

        public  object ExecuteScalar(string strConnectionString, string strSchema, string strStoredProcedureName, ref string strErrMsg);

        public string ExecuteSqlTransaction(SqlCommand saveOrderCommand, SqlCommand buySellTicketCommand, SqlCommand tradeCommand, SqlCommand updateOrderStatus, SqlCommand updateRemainingQty);

        /// <summary>
        /// Method that accepts DataTable dt and converts it to a generic list item. For this method to work DataTable schema shall be same as List propertis.
        /// this Method calls GetItem() to interate through each column of each row 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="dt"></param>
        /// <returns></returns>
        public  List<T> ConvertDataTable<T>(DataTable dt);

        /// <summary>
        /// Method that accepts DataRow object and iterates through each columns in the passed in DataRow and maps each to generic type T. Note that 
        /// for this method to work DataRow columns schema shall be same as the properties of the genric type T
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="dr"></param>
        /// <returns></returns>
        public  T GetItem<T>(DataRow dr);

        public  DataTable ConstructTVP(List<Guid> lst);
    }
}