using System;
using System.Data;
using System.Data.SqlClient;
using System.Data.Common;
using System.Configuration;

namespace DataBase
{
	 #region Class Cliente
	 public class Cliente
	{
		#region Atributos
		private readonly string strConn = string.Empty;
		public DbTransaction sqlTrans;
		public bool isTransaction;
		private SqlConnection cn = null;
		private SqlCommand cmd = null;
		#endregion

		#region Construtor
		public Cliente()
		{
			try{ strConn = ConfigurationManager.ConnectionStrings["conn"].ConnectionString; }
			catch(Exception ex){ throw ex; }
		}
		#endregion
	
		#region Métodos

		public void OpenConnection()
		{
			cn = new SqlConnection(strConn);
			cn.Open();
			cmd.Connection = cn;
		}

		public void CloseConnection()
		{
			if (cn != null)
			{
				if (cn.State == ConnectionState.Open && sqlTrans == null)
				{
					cn.Close();
					cn.Dispose();
				}
			}
			if (cmd != null) cmd.Dispose();
		}

		public void OpenConnectionTrans()
		{
			if (isTransaction)
			{
				if (sqlTrans == null)
				{
					OpenConnection();
					cmd.Transaction = cmd.Connection.BeginTransaction();
					sqlTrans = cmd.Transaction;
				}
				else
				{
					cmd.Connection = (SqlConnection)sqlTrans.Connection;
					cmd.Transaction = (SqlTransaction)sqlTrans;
				}
			}
			else OpenConnection();
		}

		public DbTransaction Save(ref int? idCliente_, string nome_)
		{
			try
			{
				cmd = new SqlCommand();
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.CommandText = "usp_SaveCliente";

				if (idCliente_ == null) cmd.Parameters.AddWithValue("@idCliente", DBNull.Value);
				else cmd.Parameters.AddWithValue("@idCliente", idCliente_);

				if (nome_ == null) cmd.Parameters.AddWithValue("@nome", DBNull.Value);
				else cmd.Parameters.AddWithValue("@nome", nome_);

				OpenConnectionTrans();
				idCliente_ = Convert.ToInt32("0" + cmd.ExecuteScalar());
				return sqlTrans;
			}
			catch (SqlException sex){ throw sex; }
			catch (Exception ex){ throw ex; }
			finally { CloseConnection(); }
		}
		public DbTransaction Delete(int? idCliente_, string nome_)
		{
			try
			{
				cmd = new SqlCommand();
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.CommandText = "usp_DelCliente";

				if (idCliente_ == null) cmd.Parameters.AddWithValue("@idCliente", DBNull.Value);
				else cmd.Parameters.AddWithValue("@idCliente", idCliente_);

				if (nome_ == null) cmd.Parameters.AddWithValue("@nome", DBNull.Value);
				else cmd.Parameters.AddWithValue("@nome", nome_);

				OpenConnectionTrans();
				cmd.ExecuteNonQuery();
				return sqlTrans;
			}
			catch (SqlException sex){ throw sex; }
			catch (Exception ex){ throw ex; }
			finally { CloseConnection(); }
		}
		public DbDataReader Get(int? idCliente_, string nome_)
		{
			try
			{
				cmd = new SqlCommand();
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.CommandText = "usp_GetCliente";

				if (idCliente_ == null) cmd.Parameters.AddWithValue("@idCliente", DBNull.Value);
				else cmd.Parameters.AddWithValue("@idCliente", idCliente_);

				if (nome_ == null) cmd.Parameters.AddWithValue("@nome", DBNull.Value);
				else cmd.Parameters.AddWithValue("@nome", nome_);

				OpenConnection();
				return cmd.ExecuteReader();
			}
			catch (SqlException sex){ throw sex; }
			catch (Exception ex){ throw ex; }
		}
		#endregion
	}
	 #endregion
}