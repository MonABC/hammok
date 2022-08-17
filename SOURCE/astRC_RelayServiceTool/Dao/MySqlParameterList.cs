#region

using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;

#endregion

namespace Hammock.AssetView.Platinum.Tools.RC.RelayService.Dao
{
    public class MySqlParameterList : List<MySqlParameter>
    {
        public new void Add(MySqlParameter value)
        {
            if (value.Direction != ParameterDirection.Output)
            {
                if (value.Value == null)
                {
                    value.Value = DBNull.Value;
                }
                else
                {
                    switch (value.MySqlDbType)
                    {
                        case MySqlDbType.DateTime:
                            if ((DateTime?) value.Value == DateTime.MinValue)
                            {
                                value.Value = DBNull.Value;
                            }
                            break;
                        case MySqlDbType.Int16:
                            if (Convert.ToInt16(value.Value) == Int16.MinValue)
                            {
                                value.Value = DBNull.Value;
                            }
                            break;
                        case MySqlDbType.Float:
                            if ((float?) value.Value == float.MinValue)
                            {
                                value.Value = DBNull.Value;
                            }
                            break;
                        case MySqlDbType.Int32:
                            if ((int?) value.Value == Int32.MinValue)
                            {
                                value.Value = DBNull.Value;
                            }
                            break;
                    }
                }
            }
            base.Add(value);
        }
    }
}