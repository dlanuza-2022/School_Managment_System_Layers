using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.DataBase
{
    public class clsDataBase
    {

        #region Private variables

        private SqlConnection _objSqlConnection; 
        /*The SqlConnection allows us to create the connection with the data base.*/
        private SqlDataAdapter _objSqlAdapter;
        /*It allows us to do a read of data, we use it to do a SELECT on the database.
         It retrieves information. */
        private SqlCommand _objSqlCommand;
        /*It allows us to send the commands to Create, Update and Delete info.*/
        private DataSet _dataSetResults;
        /*It´s a list of tables.*/
        private DataTable _dataTableParameters;
        /*In this table we will build the paramenters that we will 
         * give to the SqlDataAdapter and the SqlCommand.*/
        private string _tableName, _storeProcedureName, _DBErrorMessage, _scalarValue, _DBName;
        private bool _scalar;

        #endregion

        #region Public variables (Properties)
        //Encapsulation using properties
        public SqlConnection ObjSqlConnection { get => _objSqlConnection; set => _objSqlConnection = value; }
        public SqlDataAdapter ObjSqlDataAdapter { get => _objSqlAdapter; set => _objSqlAdapter = value; }
        public SqlCommand ObjSqlCommand { get => _objSqlCommand; set => _objSqlCommand = value; }
        public DataSet DataSetResults { get => _dataSetResults; set => _dataSetResults = value; }
        public DataTable DataTableParameters { get => _dataTableParameters; set => _dataTableParameters = value; }
        public string TableName { get => _tableName; set => _tableName = value; }
        public string StoreProcedureName { get => _storeProcedureName; set => _storeProcedureName = value; }
        public string DBErrorMessage { get => _DBErrorMessage; set => _DBErrorMessage = value; }
        public string ScalarValue { get => _scalarValue; set => _scalarValue = value; }
        public string DBName { get => _DBName; set => _DBName = value; }
        public bool Scalar { get => _scalar; set => _scalar = value; }

        #endregion

        #region Constructors

        public clsDataBase()
        {
            DataTableParameters = new DataTable("SpParametros");
            DataTableParameters.Columns.Add("Nombre"); //0
            DataTableParameters.Columns.Add("TipoDato"); //1
            DataTableParameters.Columns.Add("Valor");  //2

            DBName = "DB_BasePruebas";
        }

        #endregion

        #region Private methods

        private void CreateDataBaseConnection(ref clsDataBase ObjDataBase)
        {
            switch(ObjDataBase.DBName)
            {
                case "DB_BasePruebas":

                    ObjDataBase.ObjSqlConnection = new SqlConnection(Properties.Settings.Default.StringConnection_DB_BasePruebas);

                    break;
      
                default:
                    break;
            }
        }

        private void ValidateDataBaseConnection(ref clsDataBase ObjDataBase)
        {
            if (ObjDataBase.ObjSqlConnection.State == ConnectionState.Closed)
            {
                ObjDataBase.ObjSqlConnection.Open();
            }
            else
            {
                ObjDataBase.ObjSqlConnection.Close();
                ObjDataBase.ObjSqlConnection.Dispose();
            }
        }

        private void AddParameters(ref clsDataBase ObjDataBase)
        {
            if (ObjDataBase.DataTableParameters != null)
            {
                SqlDbType SQLDataType = new SqlDbType();

                foreach (DataRow item in ObjDataBase.DataTableParameters.Rows)
                {
                    switch (item[1])
                    {
                        case "1":
                            SQLDataType = SqlDbType.Bit; //The bit is a boolean
                            break;
                        case "2":
                            SQLDataType = SqlDbType.TinyInt; 
                            break;
                        case "3":
                            SQLDataType = SqlDbType.SmallInt; 
                            break;
                        case "4":
                            SQLDataType = SqlDbType.Int; 
                            break;
                        case "5":
                            SQLDataType = SqlDbType.BigInt; 
                            break;
                        case "6":
                            SQLDataType = SqlDbType.Decimal; 
                            break;
                        case "7":
                            SQLDataType = SqlDbType.SmallMoney;
                            break;
                        case "8":
                            SQLDataType = SqlDbType.Money; 
                            break;
                        case "9":
                            SQLDataType = SqlDbType.Float; 
                            break;
                        case "10":
                            SQLDataType = SqlDbType.Real; 
                            break;
                        case "11":
                            SQLDataType = SqlDbType.Date; 
                            break;
                        case "12":
                            SQLDataType = SqlDbType.Time; 
                            break;
                        case "13":
                            SQLDataType = SqlDbType.SmallDateTime; //3 bytes
                            break;
                        case "14":
                            SQLDataType = SqlDbType.DateTime; // 8 bytes
                            break;
                        case "15":
                            SQLDataType = SqlDbType.Char; 
                            break;
                        case "16":
                            SQLDataType = SqlDbType.NChar; 
                            break;
                        case "17":
                            SQLDataType = SqlDbType.VarChar; 
                            break;
                        case "18":
                            SQLDataType = SqlDbType.NVarChar; 
                            break;
                        default:
                            break;
                    }


                    if (ObjDataBase.Scalar)
                    {
                        if (item[2].ToString().Equals(string.Empty))
                        {
                            ObjDataBase.ObjSqlCommand.Parameters.Add(item[0].ToString(), SQLDataType).Value = DBNull.Value;
                        }
                        else
                        {
                            ObjDataBase.ObjSqlCommand.Parameters.Add(item[0].ToString(), SQLDataType).Value = item[2].ToString();
                        }
                    }
                    else
                    {
                        if (item[2].ToString().Equals(string.Empty))
                        {
                            ObjDataBase.ObjSqlDataAdapter.SelectCommand.Parameters.Add(item[0].ToString(), SQLDataType).Value = DBNull.Value;
                        }
                        else
                        {
                            ObjDataBase.ObjSqlDataAdapter.SelectCommand.Parameters.Add(item[0].ToString(), SQLDataType).Value = item[2].ToString();
                        }
                    }

                }
            }
        }

        private void PrepareDataBaseConnection(ref clsDataBase ObjDataBase)
        {
            CreateDataBaseConnection(ref ObjDataBase);
            ValidateDataBaseConnection(ref ObjDataBase);
        }
        private void ExecuteDataAdapter(ref clsDataBase ObjDataBase)
        {
            try
            {
                PrepareDataBaseConnection(ref ObjDataBase);

                ObjDataBase.ObjSqlDataAdapter = new SqlDataAdapter(ObjDataBase.StoreProcedureName, ObjDataBase.ObjSqlConnection);
                ObjDataBase.ObjSqlDataAdapter.SelectCommand.CommandType = CommandType.StoredProcedure;
                AddParameters(ref ObjDataBase);
                ObjDataBase.DataSetResults = new DataSet();
                ObjDataBase.ObjSqlDataAdapter.Fill(ObjDataBase.DataSetResults, ObjDataBase.DBName);
            }
            catch(Exception ex)
            {
                ObjDataBase.DBErrorMessage = ex.Message.ToString();
            }
            finally
            {
                if(ObjDataBase.ObjSqlConnection.State == ConnectionState.Open)
                {
                    ValidateDataBaseConnection(ref ObjDataBase);
                }
            }
        }
        private void ExecuteCommand(ref clsDataBase ObjDataBase)
        {
            try
            {
                PrepareDataBaseConnection(ref ObjDataBase);
                ObjDataBase.ObjSqlCommand = new SqlCommand(ObjDataBase.StoreProcedureName, ObjDataBase.ObjSqlConnection)
                {
                    CommandType = CommandType.StoredProcedure
                };

                if (ObjDataBase.Scalar)
                {
                    ObjDataBase.ScalarValue = ObjDataBase.ObjSqlCommand.ExecuteScalar().ToString().Trim();
                }
                else
                {
                    ObjDataBase.ObjSqlCommand.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                ObjDataBase.DBErrorMessage = ex.Message.ToString();

            }
            finally
            {
                if (ObjDataBase.ObjSqlConnection.State == ConnectionState.Open)
                {
                    ValidateDataBaseConnection(ref ObjDataBase);
                }
            }
        }
        #endregion

        #region Public methods

        public void CRUD(ref clsDataBase ObjDataBase)
        {
            if (ObjDataBase.Scalar)
            {
                ExecuteCommand(ref ObjDataBase);
            }
            else
            {
                ExecuteDataAdapter(ref ObjDataBase);
            }
        }

        #endregion

    }
}
