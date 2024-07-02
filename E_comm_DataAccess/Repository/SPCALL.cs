using Dapper;
using E_comm_DataAccess.Data;
using E_comm_DataAccess.Repository.IRepository;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_comm_DataAccess.Repository
{
    public class SPCALL : ISPCALL
    {
        private readonly ApplicationDbContext _context;
        private static string connectionString = " ";
        public SPCALL (ApplicationDbContext context)
        {
            _context = context;
            connectionString = _context.Database.GetDbConnection().ConnectionString;
        }

        public void Dispose()
        {
            _context.Dispose();
        }

        public void Execute(string procedureName, DynamicParameters param = null)
        {
            using (SqlConnection sqlCon =new SqlConnection(connectionString))
            {
                sqlCon.Open();
                sqlCon.Execute(procedureName, param,
                commandType: CommandType.StoredProcedure);
            }
        }

        public IEnumerable<T> List<T>(string procedureName, DynamicParameters param = null)
        {
            using (SqlConnection sqlCon = new SqlConnection(connectionString))
            {
                return sqlCon.Query<T>(procedureName, param,
                    commandType: CommandType.StoredProcedure);
            }
        }

        //public Tuple<IEnumerable<T1>, IEnumerable<T2>> List<T1, T2>(string procedureName, DynamicParameters param = null)
        //{
        //    using (SqlConnection sqlCon = new SqlConnection(connectionString))
        //    {
        //        sqlCon.Open();
        //        var result = sqlCon.QueryMultiple(procedureName, param, commandType: CommandType.StoredProcedure);
        //        var item1 = result.Read<T1>();
        //        var item2 = result.Read<T2>();
        //        if (item1 != null && item2 != null)
        //            return new Tuple<IEnumerable<T1>, IEnumerable<T2>>
        //                (new List<T1>(), new List<T2>());

        //    }
        //}

        public T OneRecord<T>(string procedureName, DynamicParameters param = null)
        {
            using(SqlConnection sqlCon=new SqlConnection(connectionString))
            {
                sqlCon.Open();
                var Value = sqlCon.Query<T>(procedureName, param, 
                    commandType: CommandType.StoredProcedure);
                return Value.FirstOrDefault();

            }
        }

        public T Single<T>(string procedureName, DynamicParameters param = null)
        {
            using (SqlConnection sqlCon = new SqlConnection(connectionString))
            {
                sqlCon.Open();
                return sqlCon.ExecuteScalar<T>(procedureName,param,
                    commandType: CommandType.StoredProcedure);
            }


        }

        public Tuple<IEnumerable<T1>, IEnumerable<T2>> List<T1, T2>
            (string procedureName, DynamicParameters param = null)
        {
            using(SqlConnection sqlcon=new SqlConnection(connectionString))
            {
                sqlcon.Open();
                var result = sqlcon.QueryMultiple(procedureName, param,
                commandType: CommandType.StoredProcedure);
                var item1 =result.Read<T1>();
                var item2 =result.Read<T2>();
                if(item1!=null && item2 !=null)
                    return new Tuple<IEnumerable<T1>, IEnumerable<T2>>(item1, item2 );
                return new Tuple<IEnumerable<T1>, IEnumerable<T2>>
                    (new List<T1>(), new List<T2>());
            }
        }
    }
}
