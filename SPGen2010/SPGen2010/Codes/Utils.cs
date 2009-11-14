using System;
using System.Data;
using System.Data.SqlClient;
using System.Collections.Generic;
using System.Text;
using Microsoft.SqlServer.Management.Smo;
using System.Text.RegularExpressions;
using System.IO;
using System.Diagnostics;
using SPGen2010.Codes;

namespace SPGen2010.Codes.Helpers
{
    /// <summary>
    /// ������������ �ṩ�������ɴ���ĸ�������
    /// </summary>
    public static class Utils
    {
        #region Const Strings

        public const string EP_DefaultDesc = "MS_Description";
        public const string EP_Settings = "SPGenSettings_";
        public const string EP_ParmDesc = "SPGen_ParmDesc_";
        public const string EP_Caption_ = "SPGen_Caption_";
        public const string EP_Caption = "SPGen_Caption";
        public const string EP_PrimaryKeys = "SPGenSettings_PrimaryKeys";
        public const string EP_BaseTableName = "SPGenSettings_BaseTable";
        public const string EP_BaseTableSchema = "SPGenSettings_BaseTableSchema";
        public const string EP_MethodName = "SPGenSettings_MethodName";
        public const string EP_BelongTo = "SPGenSettings_BelongTo";
        public const string EP_Behavior = "SPGenSettings_Behavior";
        public const string EP_IsSingleLineResult = "SPGenSettings_IsSingleLineResult";
        public const string EP_ResultType = "SPGenSettings_ResultType";
        public const string EP_ResultTypeSchema = "SPGenSettings_ResultTypeSchema";
        public const string EP_ResultType_Int = "int";
        public const string EP_ResultType_DataSet = "DataSet";
        public const string EP_ResultType_DataTable = "DataTable";
        public const string EP_ResultType_UserDefinedTableType = "UserDefinedTableType";
        public const string EP_ResultType_Object = "Object";

        #endregion

        #region Get/Set Description ( Extended Properties )

        /// <summary>
        /// �����ݿ���󣨿⣬����ͼ�����̣��������ֶΣ����������Զ��屸ע�� key = null ��Ϊ���� MS_Description ������ ��
        /// </summary>
        public static string GetDescription(this object o)
        {
            return GetDescription(o, null);
        }
        /// <summary>
        /// �����ݿ���󣨿⣬����ͼ�����̣��������ֶΣ����������Զ��屸ע�� key = null ��Ϊ���� MS_Description ������ ��
        /// </summary>
        public static string GetDescription(this object o, string key)
        {
            if (string.IsNullOrEmpty(key)) key = Utils.EP_DefaultDesc;

            ExtendedProperty ep = null;
            if (o.GetType() == typeof(Database)) ep = ((Database)o).ExtendedProperties[key];
            else if (o.GetType() == typeof(Table)) ep = ((Table)o).ExtendedProperties[key];
            else if (o.GetType() == typeof(View)) ep = ((View)o).ExtendedProperties[key];
            else if (o.GetType() == typeof(UserDefinedTableType)) ep = ((UserDefinedTableType)o).ExtendedProperties[key];
            else if (o.GetType() == typeof(UserDefinedFunction)) ep = ((UserDefinedFunction)o).ExtendedProperties[key];
            else if (o.GetType() == typeof(StoredProcedure)) ep = ((StoredProcedure)o).ExtendedProperties[key];
            else if (o.GetType() == typeof(Column))
            {
                Column p = (Column)o;
                if (p.Parent.GetType() == typeof(Table))
                {
                    if (key == Utils.EP_DefaultDesc)
                    {
                        ep = p.ExtendedProperties[key];
                    }
                    else ep = ((Table)p.Parent).ExtendedProperties[key + p.Name];
                }
                else if (p.Parent.GetType() == typeof(UserDefinedTableType))
                {
                    if (key == Utils.EP_DefaultDesc)
                    {
                        ep = p.ExtendedProperties[key];
                    }
                    else ep = ((UserDefinedTableType)p.Parent).ExtendedProperties[key + p.Name];
                }
                else if (p.Parent.GetType() == typeof(View))
                {
                    if (key == Utils.EP_DefaultDesc)
                    {
                        ep = ((View)p.Parent).ExtendedProperties[Utils.EP_Settings + p.Name];
                    }
                    else ep = ((View)p.Parent).ExtendedProperties[key + p.Name];
                }
                else if (p.Parent.GetType() == typeof(UserDefinedFunction))
                {
                    if (key == Utils.EP_DefaultDesc)
                    {
                        ep = ((UserDefinedFunction)p.Parent).ExtendedProperties[Utils.EP_Settings + p.Name];
                    }
                    else ep = ((UserDefinedFunction)p.Parent).ExtendedProperties[key + p.Name];
                }
            }
            else if (o.GetType() == typeof(UserDefinedFunctionParameter))
            {
                UserDefinedFunctionParameter p = (UserDefinedFunctionParameter)o;
                if (key == Utils.EP_DefaultDesc)
                {
                    ep = ((UserDefinedFunction)p.Parent).ExtendedProperties[Utils.EP_ParmDesc + p.Name];
                }
                else ep = ((UserDefinedFunction)p.Parent).ExtendedProperties[key + p.Name];
            }
            else if (o.GetType() == typeof(StoredProcedureParameter))
            {
                StoredProcedureParameter p = (StoredProcedureParameter)o;
                if (key == Utils.EP_DefaultDesc)
                {
                    ep = ((StoredProcedure)p.Parent).ExtendedProperties[Utils.EP_ParmDesc + p.Name];
                }
                else ep = ((StoredProcedure)p.Parent).ExtendedProperties[key + p.Name];
            }
            if (ep != null)
            {
                // �־�ϲ���ȡ
                string s = ep.Value as string;
                if (s.Length == 3600) return s + GetDescription(o, key + "_");
                else return s;
            }
            return string.Empty;
        }

        /// <summary>
        /// д���ݿ���󣨿⣬����ͼ�����̣��������ֶΣ��������ı�ע
        /// </summary>
        public static void SetDescription(this object o, string value)
        {
            SetDescription(o, null, value);
        }
        /// <summary>
        /// д���ݿ���󣨿⣬����ͼ�����̣��������ֶΣ����������Զ��屸ע�� key = null ��Ϊ���� MS_Description ������ ��
        /// </summary>
        public static void SetDescription(this object o, string key, string value)
        {
            if (string.IsNullOrEmpty(key)) key = Utils.EP_DefaultDesc;
            if (value == null) value = "";

            // �־�� ( MSSQL ��չ������ 3600 �������ҵ����ƣ�
            if (value.Length > 3600)
            {
                // ���Ӵ�
                SetDescription(o, key + "_", value.Substring(3600));
                value = value.Substring(0, 3600);
            }
            else if (value.Length > 0)
            {
                // ɾ����ķ־���չ���� ������Ϊ����� 100 ��
                for (int i = 1; i < 100; i++)
                {
                    DeleteDescription(o, key + new string('_', i));
                }
            }

            if (o.GetType() == typeof(Database))
            {
                Database p = (Database)o;
                if (p.ExtendedProperties.Contains(key))
                {
                    if (string.IsNullOrEmpty(value))
                    {
                        p.ExtendedProperties[key].Drop();
                        p.Alter();
                    }
                    else if (p.ExtendedProperties[key].Value as string != value)
                    {
                        p.ExtendedProperties[key].Value = value;
                        p.Alter();
                    }
                }
                else
                {
                    if (!string.IsNullOrEmpty(value))
                    {
                        p.ExtendedProperties.Add(new ExtendedProperty(p, key, value));
                        p.Alter();
                    }
                }
            }
            else if (o.GetType() == typeof(Table))
            {
                Table p = (Table)o;
                if (p.ExtendedProperties.Contains(key))
                {
                    if (string.IsNullOrEmpty(value))
                    {
                        p.ExtendedProperties[key].Drop();
                        p.Alter();
                    }
                    else if (p.ExtendedProperties[key].Value as string != value)
                    {
                        p.ExtendedProperties[key].Value = value;
                        p.Alter();
                    }
                }
                else
                {
                    if (!string.IsNullOrEmpty(value))
                    {
                        p.ExtendedProperties.Add(new ExtendedProperty(p, key, value));
                        p.Alter();
                    }
                }
            }
            else if (o.GetType() == typeof(UserDefinedTableType))
            {
                UserDefinedTableType p = (UserDefinedTableType)o;
                if (p.ExtendedProperties.Contains(key))
                {
                    if (string.IsNullOrEmpty(value))
                    {
                        p.ExtendedProperties[key].Drop();
                        p.Alter();
                    }
                    else if (p.ExtendedProperties[key].Value as string != value)
                    {
                        p.ExtendedProperties[key].Value = value;
                        p.Alter();
                    }
                }
                else
                {
                    if (!string.IsNullOrEmpty(value))
                    {
                        p.ExtendedProperties.Add(new ExtendedProperty(p, key, value));
                        p.Alter();
                    }
                }
            }
            else if (o.GetType() == typeof(View))
            {
                View p = (View)o;
                if (p.ExtendedProperties.Contains(key))
                {
                    if (string.IsNullOrEmpty(value))
                    {
                        p.ExtendedProperties[key].Drop();
                        p.Alter();
                    }
                    else if (p.ExtendedProperties[key].Value as string != value)
                    {
                        p.ExtendedProperties[key].Value = value;
                        p.Alter();
                    }
                }
                else
                {
                    if (!string.IsNullOrEmpty(value))
                    {
                        p.ExtendedProperties.Add(new ExtendedProperty(p, key, value));
                        p.Alter();
                    }
                }
            }
            else if (o.GetType() == typeof(Column))
            {
                Column p = (Column)o;
                if (p.Parent.GetType() == typeof(Table)
                    || p.Parent.GetType() == typeof(UserDefinedTableType))
                {
                    if (key == Utils.EP_DefaultDesc)
                    {
                        if (p.ExtendedProperties.Contains(key))
                        {
                            if (string.IsNullOrEmpty(value))
                            {
                                p.ExtendedProperties[key].Drop();
                                p.Alter();
                            }
                            else if (p.ExtendedProperties[key].Value as string != value)
                            {
                                p.ExtendedProperties[key].Value = value;
                                p.Alter();
                            }
                        }
                        else
                        {
                            if (!string.IsNullOrEmpty(value))
                            {
                                p.ExtendedProperties.Add(new ExtendedProperty(p, key, value));
                                p.Alter();
                            }
                        }
                    }
                    else
                    {
                        SetDescription(p.Parent, key + p.Name, value);
                    }
                }
                else if (p.Parent.GetType() == typeof(View))
                {
                    if (key == Utils.EP_DefaultDesc)
                    {
                        SetDescription(p.Parent, Utils.EP_Settings + p.Name, value);
                    }
                    else SetDescription(p.Parent, key + p.Name, value);
                }
                else if (p.Parent.GetType() == typeof(UserDefinedFunction))
                {
                    if (key == Utils.EP_DefaultDesc)
                    {
                        SetDescription(p.Parent, Utils.EP_Settings + p.Name, value);
                    }
                    else SetDescription(p.Parent, key + p.Name, value);
                }
            }
            else if (o.GetType() == typeof(UserDefinedFunction))
            {
                UserDefinedFunction p = (UserDefinedFunction)o;
                if (p.ExtendedProperties.Contains(key))
                {
                    if (string.IsNullOrEmpty(value))
                    {
                        p.ExtendedProperties[key].Drop();
                        p.Alter();
                    }
                    else if (p.ExtendedProperties[key].Value as string != value)
                    {
                        p.ExtendedProperties[key].Value = value;
                        p.Alter();
                    }
                }
                else
                {
                    if (!string.IsNullOrEmpty(value))
                    {
                        p.ExtendedProperties.Add(new ExtendedProperty(p, key, value));
                        p.Alter();
                    }
                }
            }
            else if (o.GetType() == typeof(UserDefinedFunctionParameter))
            {
                UserDefinedFunctionParameter p = (UserDefinedFunctionParameter)o;
                if (key == Utils.EP_DefaultDesc)
                {
                    SetDescription(p.Parent, Utils.EP_ParmDesc + p.Name, value);
                }
                else SetDescription(p.Parent, key + p.Name, value);
            }
            else if (o.GetType() == typeof(StoredProcedure))
            {
                StoredProcedure p = (StoredProcedure)o;
                if (p.ExtendedProperties.Contains(key))
                {
                    if (string.IsNullOrEmpty(value))
                    {
                        p.ExtendedProperties[key].Drop();
                        p.Alter();
                    }
                    else if (p.ExtendedProperties[key].Value as string != value)
                    {
                        p.ExtendedProperties[key].Value = value;
                        p.Alter();
                    }
                }
                else
                {
                    if (!string.IsNullOrEmpty(value))
                    {
                        p.ExtendedProperties.Add(new ExtendedProperty(p, key, value));
                        p.Alter();
                    }
                }
            }
            else if (o.GetType() == typeof(StoredProcedureParameter))
            {
                StoredProcedureParameter p = (StoredProcedureParameter)o;
                if (key == Utils.EP_DefaultDesc)
                {
                    SetDescription(p.Parent, Utils.EP_ParmDesc + p.Name, value);
                }
                else SetDescription(p.Parent, key + p.Name, value);
            }
        }


        /// <summary>
        /// ɾ���ݿ���󣨿⣬����ͼ�����̣��������ֶΣ����������Զ��屸ע�� key = null ��Ϊ���� MS_Description ������ ��
        /// </summary>
        public static void DeleteDescription(this object o, string key)
        {
            if (string.IsNullOrEmpty(key)) key = Utils.EP_DefaultDesc;

            if (o.GetType() == typeof(Database))
            {
                Database p = (Database)o;
                if (p.ExtendedProperties.Contains(key))
                {
                    p.ExtendedProperties[key].Drop();
                    p.Alter();
                }
            }
            else if (o.GetType() == typeof(Table))
            {
                Table p = (Table)o;
                if (p.ExtendedProperties.Contains(key))
                {
                    p.ExtendedProperties[key].Drop();
                    p.Alter();
                }
            }
            else if (o.GetType() == typeof(UserDefinedTableType))
            {
                UserDefinedTableType p = (UserDefinedTableType)o;
                if (p.ExtendedProperties.Contains(key))
                {
                    p.ExtendedProperties[key].Drop();
                    p.Alter();
                }
            }
            else if (o.GetType() == typeof(View))
            {
                View p = (View)o;
                if (p.ExtendedProperties.Contains(key))
                {
                    p.ExtendedProperties[key].Drop();
                    p.Alter();
                }
            }
            else if (o.GetType() == typeof(Column))
            {
                Column p = (Column)o;
                if (p.Parent.GetType() == typeof(Table)
                    || p.Parent.GetType() == typeof(UserDefinedTableType))
                {
                    if (key == Utils.EP_DefaultDesc)
                    {
                        if (p.ExtendedProperties.Contains(key))
                        {
                            p.ExtendedProperties[key].Drop();
                            p.Alter();
                        }
                    }
                    else
                    {
                        DeleteDescription(p.Parent, key + p.Name);
                    }
                }
                else if (p.Parent.GetType() == typeof(View))
                {
                    if (key == Utils.EP_DefaultDesc)
                    {
                        DeleteDescription(p.Parent, Utils.EP_Settings + p.Name);
                    }
                    else DeleteDescription(p.Parent, key + p.Name);
                }
                else if (p.Parent.GetType() == typeof(UserDefinedFunction))
                {
                    if (key == Utils.EP_DefaultDesc)
                    {
                        DeleteDescription(p.Parent, Utils.EP_Settings + p.Name);
                    }
                    else DeleteDescription(p.Parent, key + p.Name);
                }
            }
            else if (o.GetType() == typeof(UserDefinedFunction))
            {
                UserDefinedFunction p = (UserDefinedFunction)o;
                if (p.ExtendedProperties.Contains(key))
                {
                    p.ExtendedProperties[key].Drop();
                    p.Alter();
                }
            }
            else if (o.GetType() == typeof(UserDefinedFunctionParameter))
            {
                UserDefinedFunctionParameter p = (UserDefinedFunctionParameter)o;
                if (key == Utils.EP_DefaultDesc)
                {
                    DeleteDescription(p.Parent, Utils.EP_ParmDesc + p.Name);
                }
                else DeleteDescription(p.Parent, key + p.Name);
            }
            else if (o.GetType() == typeof(StoredProcedure))
            {
                StoredProcedure p = (StoredProcedure)o;
                if (p.ExtendedProperties.Contains(key))
                {
                    p.ExtendedProperties[key].Drop();
                    p.Alter();
                }
            }
            else if (o.GetType() == typeof(StoredProcedureParameter))
            {
                StoredProcedureParameter p = (StoredProcedureParameter)o;
                if (key == Utils.EP_DefaultDesc)
                {
                    DeleteDescription(p.Parent, Utils.EP_ParmDesc + p.Name);
                }
                else DeleteDescription(p.Parent, key + p.Name);
            }
        }


        #endregion


        #region Get/Set Caption

        /// <summary>
        /// ���ֶΣ�����ͼ���������� Caption
        /// </summary>
        public static string GetCaption(this Column c)
        {
            string s = GetDescription(c, Utils.EP_Caption_);
            if (string.IsNullOrEmpty(s)) return c.Name;
            return s;
        }


        /// <summary>
        /// д�ֶΣ�����ͼ���������� Caption
        /// </summary>
        public static void SetCaption(this Column c, string value)
        {
            if (c.Name == value) SetDescription(c, Utils.EP_Caption_, string.Empty);
            else SetDescription(c, Utils.EP_Caption_, value);
        }


        /// <summary>
        /// �����ݿ����� Caption
        /// </summary>
        public static string GetCaption(this object o)
        {
            string s = GetDescription(o, Utils.EP_Caption);
            if (string.IsNullOrEmpty(s))
            {
                if (o.GetType() == typeof(Table)) return ((Table)o).Name;
                else if (o.GetType() == typeof(View)) return ((View)o).Name;
                else if (o.GetType() == typeof(UserDefinedTableType)) return ((UserDefinedTableType)o).Name;
                else if (o.GetType() == typeof(UserDefinedFunction)) return ((UserDefinedFunction)o).Name;
                return o.ToString();
            }
            return s;
        }

        /// <summary>
        /// д���ݿ����� Caption
        /// </summary>
        public static void SetCaption(this object o, string value)
        {
            if (o.GetType() == typeof(Table) && value == ((Table)o).Name
                || o.GetType() == typeof(View) && value == ((View)o).Name
                || o.GetType() == typeof(UserDefinedTableType) && value == ((UserDefinedTableType)o).Name
                || o.GetType() == typeof(UserDefinedFunction) && value == ((UserDefinedFunction)o).Name
                || value == o.ToString()) SetDescription(o, Utils.EP_Caption, string.Empty);
            else SetDescription(o, Utils.EP_Caption, value);
        }

        #endregion


        #region Get/Set View's PrimaryKeyColumnNames

        /// <summary>
        /// ����һ����ͼ���ֹ�ָ���������ֶ����б�
        /// </summary>
        public static List<string> GetPrimaryKeyColumnNames(this View v)
        {
            string s = GetDescription(v, Utils.EP_PrimaryKeys);
            if (s.Length > 0)
            {
                return new List<string>(s.Split(','));
            }
            return new List<string>();
        }

        /// <summary>
        /// ����һ����ͼ���ֹ�ָ���������ֶ����б�
        /// </summary>
        public static void SetPrimaryKeyColumnNames(this View v, List<string> cns)
        {
            if (cns == null || cns.Count == 0) SetDescription(v, Utils.EP_PrimaryKeys, string.Empty);
            else
            {
                StringBuilder sb = new StringBuilder();
                for (int i = 0; i < cns.Count; i++)
                {
                    if (i > 0) sb.Append(@",");
                    sb.Append(cns[i]);
                }
                SetDescription(v, Utils.EP_PrimaryKeys, sb.ToString());
            }
        }


        /// <summary>
        /// ����һ����ͼ���ֹ�ָ���������ֶ��б�
        /// </summary>
        public static List<Column> GetPrimaryKeyColumns(this View v)
        {
            string s = GetDescription(v, Utils.EP_PrimaryKeys);
            if (s.Length > 0)
            {
                List<string> ss = new List<string>(s.Split(','));
                List<Column> cs = new List<Column>();
                foreach (Column c in v.Columns) if (ss.Contains(c.Name)) cs.Add(c);
                return cs;
            }
            return new List<Column>();
        }

        /// <summary>
        /// ����һ����ͼ���ֹ�ָ���������ֶ��б�
        /// </summary>
        public static void SetPrimaryKeyColumns(this View v, List<Column> cs)
        {
            if (cs == null || cs.Count == 0) SetDescription(v, Utils.EP_PrimaryKeys, string.Empty);
            else
            {
                StringBuilder sb = new StringBuilder();
                for (int i = 0; i < cs.Count; i++)
                {
                    if (i > 0) sb.Append(@",");
                    sb.Append(cs[i].Name);
                }
                SetDescription(v, Utils.EP_PrimaryKeys, sb.ToString());
            }
        }

        #endregion

        #region Get/Set View's Base Table

        /// <summary>
        /// ����һ����ͼ��ָ��Ļ���û�з��ؿ�
        /// </summary>
        public static Table GetBaseTable(this View v)
        {
            string n = Utils.GetDescription(v, Utils.EP_BaseTableName);
            if (string.IsNullOrEmpty(n)) return null;
            string s = Utils.GetDescription(v, Utils.EP_BaseTableSchema);
            if (string.IsNullOrEmpty(s)) return v.Parent.Tables[n];
            return v.Parent.Tables[n, s];
        }

        /// <summary>
        /// ����һ����ͼ��ָ��Ļ�������û�з��� ��
        /// </summary>
        public static string GetBaseTableName(this View v)
        {
            return Utils.GetDescription(v, Utils.EP_BaseTableName);
        }
        /// <summary>
        /// ����һ����ͼ��ָ��Ļ���ܹ�����û�з��� ��
        /// </summary>
        public static string GetBaseTableSchema(this View v)
        {
            return Utils.GetDescription(v, Utils.EP_BaseTableSchema);
        }

        /// <summary>
        /// ����һ����ͼ�Ļ���
        /// </summary>
        public static void SetBaseTable(this View v, string tn, string schema)
        {
            if (string.IsNullOrEmpty(tn) || tn == "None")
            {
                Utils.SetDescription(v, Utils.EP_BaseTableName, string.Empty);
                Utils.SetDescription(v, Utils.EP_BaseTableSchema, string.Empty);
            }
            else
            {
                Utils.SetDescription(v, Utils.EP_BaseTableName, tn);
                Utils.SetDescription(v, Utils.EP_BaseTableSchema, schema);
            }
        }

        #endregion

        // ���� SP/Func ��ص��������Կ�������Ϊһ�� XML ����

        #region Get/Set SP/Func's MethodName

        /// <summary>
        /// ȡ�洢���̵ķ�����
        /// </summary>
        public static string GetMethodName(this StoredProcedure sp)
        {
            string s = Utils.GetDescription(sp, Utils.EP_MethodName);
            if (string.IsNullOrEmpty(s)) return GetEscapeName(sp.Name);
            return s;
        }

        /// <summary>
        /// ��洢���̵ķ�����
        /// </summary>
        public static void SetMethodName(this StoredProcedure sp, string value)
        {
            if (string.IsNullOrEmpty(value) || value == GetEscapeName(sp.Name)) Utils.SetDescription(sp, Utils.EP_MethodName, string.Empty);
            else Utils.SetDescription(sp, Utils.EP_MethodName, value);
        }


        /// <summary>
        /// ȡ�����ķ�����
        /// </summary>
        public static string GetMethodName(this UserDefinedFunction sp)
        {
            string s = Utils.GetDescription(sp, Utils.EP_MethodName);
            if (string.IsNullOrEmpty(s)) return GetEscapeName(sp.Name);
            return s;
        }

        /// <summary>
        /// �躯���ķ�����
        /// </summary>
        public static void SetMethodName(this UserDefinedFunction sp, string value)
        {
            if (string.IsNullOrEmpty(value) || value == GetEscapeName(sp.Name)) Utils.SetDescription(sp, Utils.EP_MethodName, string.Empty);
            else Utils.SetDescription(sp, Utils.EP_MethodName, value);
        }

        #endregion

        #region Get/Set SP/Func's BelongTo

        /// <summary>
        /// ȡ�洢���̵Ĺ�����
        /// </summary>
        public static string GetBelongTo(this StoredProcedure sp)
        {
            string s = Utils.GetDescription(sp, Utils.EP_BelongTo);
            if (string.IsNullOrEmpty(s)) return string.Empty;
            return s;
        }

        /// <summary>
        /// ��洢���̵Ĺ�����
        /// </summary>
        public static void SetBelongTo(this StoredProcedure sp, string value)
        {
            Utils.SetDescription(sp, Utils.EP_BelongTo, value);
        }


        /// <summary>
        /// ȡ�����Ĺ�����
        /// </summary>
        public static string GetBelongTo(this UserDefinedFunction sp)
        {
            string s = Utils.GetDescription(sp, Utils.EP_BelongTo);
            if (string.IsNullOrEmpty(s)) return string.Empty;
            return s;
        }

        /// <summary>
        /// �躯���Ĺ�����
        /// </summary>
        public static void SetBelongTo(this UserDefinedFunction sp, string value)
        {
            Utils.SetDescription(sp, Utils.EP_BelongTo, value);
        }

        #endregion

        #region Get/Set SP/Func's Behavior

        /// <summary>
        /// ȡ�洢���̵����ݲ�����Ϊ
        /// </summary>
        public static string GetBehavior(this StoredProcedure sp)
        {
            string s = Utils.GetDescription(sp, Utils.EP_Behavior);
            if (string.IsNullOrEmpty(s)) return "None";
            return s;
        }

        /// <summary>
        /// ��洢���̵����ݲ�����Ϊ
        /// </summary>
        public static void SetBehavior(this StoredProcedure sp, string value)
        {
            if (string.IsNullOrEmpty(value) || value == "None") Utils.SetDescription(sp, Utils.EP_Behavior, string.Empty);
            else Utils.SetDescription(sp, Utils.EP_Behavior, value);
        }


        /// <summary>
        /// ȡ���������ݲ�����Ϊ
        /// </summary>
        public static string GetBehavior(this UserDefinedFunction sp)
        {
            string s = Utils.GetDescription(sp, Utils.EP_Behavior);
            if (string.IsNullOrEmpty(s)) return "None";
            return s;
        }

        /// <summary>
        /// �躯�������ݲ�����Ϊ
        /// </summary>
        public static void SetBehavior(this UserDefinedFunction sp, string value)
        {
            if (string.IsNullOrEmpty(value) || value == "None") Utils.SetDescription(sp, Utils.EP_Behavior, string.Empty);
            else Utils.SetDescription(sp, Utils.EP_Behavior, value);
        }

        #endregion

        #region Get/Set SP/Func's ResultType

        /// <summary>
        /// ȡ�洢���̵ķ��ؽ������
        /// </summary>
        public static string GetResultType(this StoredProcedure sp)
        {
            string s = Utils.GetDescription(sp, Utils.EP_ResultType);
            if (string.IsNullOrEmpty(s)) return Utils.EP_ResultType_Int;
            return s;
        }

        /// <summary>
        /// ��洢���̵ķ��ؽ������
        /// </summary>
        public static void SetResultType(this StoredProcedure sp, string value)
        {
            if (value == Utils.EP_ResultType_Int) Utils.SetDescription(sp, Utils.EP_ResultType, string.Empty);
            else Utils.SetDescription(sp, Utils.EP_ResultType, value);
        }


        /// <summary>
        /// ȡ�����ķ��ؽ������
        /// </summary>
        public static string GetResultType(this UserDefinedFunction sp)
        {
            string s = Utils.GetDescription(sp, Utils.EP_ResultType);
            if (string.IsNullOrEmpty(s)) return Utils.EP_ResultType_Int;
            return s;
        }

        /// <summary>
        /// �躯���ķ��ؽ������
        /// </summary>
        public static void SetResultType(this UserDefinedFunction sp, string value)
        {
            if (value == Utils.EP_ResultType_Int) Utils.SetDescription(sp, Utils.EP_ResultType, string.Empty);
            else Utils.SetDescription(sp, Utils.EP_ResultType, value);
        }



        /// <summary>
        /// ȡ�洢���̵ķ��ؽ����������Schema
        /// </summary>
        public static string GetResultTypeSchema(this StoredProcedure sp)
        {
            return Utils.GetDescription(sp, Utils.EP_ResultTypeSchema);
        }

        /// <summary>
        /// ��洢���̵ķ��ؽ����������Schema
        /// </summary>
        public static void SetResultTypeSchema(this StoredProcedure sp, string value)
        {
            Utils.SetDescription(sp, Utils.EP_ResultTypeSchema, value);
        }


        /// <summary>
        /// ȡ�����ķ��ؽ����������Schema
        /// </summary>
        public static string GetResultTypeSchema(this UserDefinedFunction sp)
        {
            return Utils.GetDescription(sp, Utils.EP_ResultTypeSchema);
        }

        /// <summary>
        /// �躯���ķ��ؽ����������Schema
        /// </summary>
        public static void SetResultTypeSchema(this UserDefinedFunction sp, string value)
        {
            Utils.SetDescription(sp, Utils.EP_ResultTypeSchema, value);
        }

        #endregion

        #region Get/Set SP/Func's IsSingleLineResult

        /// <summary>
        /// ȡ�洢���̵ķ��ؽ�������Ƿ�Ϊ����
        /// </summary>
        public static bool GetIsSingleLineResult(this StoredProcedure sp)
        {
            string s = Utils.GetDescription(sp, Utils.EP_IsSingleLineResult);
            if (string.IsNullOrEmpty(s)) return false;
            return bool.Parse(s);
        }

        /// <summary>
        /// ��洢���̵ķ��ؽ�������Ƿ�Ϊ����
        /// </summary>
        public static void SetIsSingleLineResult(this StoredProcedure sp, bool value)
        {
            if (value) Utils.SetDescription(sp, Utils.EP_IsSingleLineResult, value.ToString());
            else Utils.SetDescription(sp, Utils.EP_IsSingleLineResult, string.Empty);
        }


        /// <summary>
        /// ȡ�����ķ��ؽ�������Ƿ�Ϊ����
        /// </summary>
        public static bool GetIsSingleLineResult(this UserDefinedFunction sp)
        {
            string s = Utils.GetDescription(sp, Utils.EP_IsSingleLineResult);
            if (string.IsNullOrEmpty(s)) return false;
            return bool.Parse(s);
        }

        /// <summary>
        /// �躯���ķ��ؽ�������Ƿ�Ϊ����
        /// </summary>
        public static void SetIsSingleLineResult(this UserDefinedFunction sp, bool value)
        {
            if (value) Utils.SetDescription(sp, Utils.EP_IsSingleLineResult, value.ToString());
            else Utils.SetDescription(sp, Utils.EP_IsSingleLineResult, string.Empty);
        }

        #endregion


        #region GetDatabase

        /// <summary>
        /// �������ݶ������ڵ� Database ����
        /// </summary>
        public static Database GetDatabase(this object o)
        {
            if (o.GetType() == typeof(Database)) return (Database)o;
            else if (o.GetType() == typeof(Column))
            {
                Column p = (Column)o;
                if (p.Parent.GetType() == typeof(Table))
                {
                    return ((Table)p.Parent).Parent;
                }
                else if (p.Parent.GetType() == typeof(UserDefinedTableType))
                {
                    return ((UserDefinedTableType)p.Parent).Parent;
                }
                else if (p.Parent.GetType() == typeof(View))
                {
                    return ((View)p.Parent).Parent;
                }
                else if (p.Parent.GetType() == typeof(UserDefinedFunction))
                {
                    return ((UserDefinedFunction)p.Parent).Parent;
                }
            }
            else if (o.GetType() == typeof(StoredProcedureParameter)) return ((StoredProcedureParameter)o).Parent.Parent;
            else if (o.GetType() == typeof(UserDefinedFunctionParameter)) return ((UserDefinedFunctionParameter)o).Parent.Parent;
            else if (o.GetType() == typeof(UserDefinedFunction)) return ((UserDefinedFunction)o).Parent;

            throw new Exception("δ�������������");
        }

        #endregion

        #region GetSqlDataType

        /// <summary>
        /// ���� systemtype ���ִ���ʽ����������������������Ӧ�� SqlDataType ö��
        /// </summary>
        /// <param name="s">����������</param>
        /// <param name="maxLength">�����ж��Ƿ񷵻� nvarcharMax, varbinaryMax, varcharMax</param>
        public static SqlDataType GetSqlDataType(this string s, int maxLength)
        {
            switch (s)
            {
                case "bigint":
                    return SqlDataType.BigInt;
                case "decimal":
                    return SqlDataType.Decimal;
                case "int":
                    return SqlDataType.Int;
                case "numeric":
                    return SqlDataType.Decimal;
                case "smallint":
                    return SqlDataType.SmallInt;
                case "money":
                    return SqlDataType.Money;
                case "tinyint":
                    return SqlDataType.TinyInt;
                case "smallmoney":
                    return SqlDataType.SmallMoney;
                case "bit":
                    return SqlDataType.Bit;
                case "float":
                    return SqlDataType.Float;
                case "real":
                    return SqlDataType.Real;
                case "datetime":
                    return SqlDataType.DateTime;
                case "smalldatetime":
                    return SqlDataType.SmallDateTime;
                case "char":
                    return SqlDataType.Char;
                case "text":
                    return SqlDataType.Text;
                case "varchar":
                    if (maxLength == -1) return SqlDataType.VarCharMax;
                    return SqlDataType.VarChar;
                case "nchar":
                    return SqlDataType.NChar;
                case "ntext":
                    return SqlDataType.NText;
                case "nvarchar":
                    if (maxLength == -1) return SqlDataType.NVarCharMax;
                    return SqlDataType.NVarChar;
                case "binary":
                    return SqlDataType.Binary;
                case "image":
                    return SqlDataType.Image;
                case "varbinary":
                    if (maxLength == -1) return SqlDataType.VarBinaryMax;
                    return SqlDataType.VarBinary;
                case "uniqueidentifier":
                    return SqlDataType.UniqueIdentifier;
                case "timestamp":
                    return SqlDataType.Timestamp;


                // todo: ��Щ������������һ����

                case "sql_variant":
                    return SqlDataType.Variant;

                case "userdefineddatatype":
                    return SqlDataType.UserDefinedDataType;
                case "userdefinedtype":
                    return SqlDataType.UserDefinedType;
                case "userdefinedtabletype":
                    return SqlDataType.UserDefinedTableType;

                case "datetime2":
                    return SqlDataType.DateTime2;
                case "datetimeoffset":
                    return SqlDataType.DateTimeOffset;
                case "date":
                    return SqlDataType.Date;
                case "time":
                    return SqlDataType.Time;

                case "xml":
                    return SqlDataType.Xml;

            }
            throw new Exception("δ�������������:" + s.ToString());
        }

        #endregion

        #region GetSummary

        /// <summary>
        /// ����Ϊ ����ε� summary ���ֶ���ʽ���ı�ע�����ʽ��ÿһ�е�ǰ�������б��
        /// todo: ����ת��
        /// </summary>
        public static string GetSummary(this object o, int numTabs)
        {
            return GetSummary(o, string.Empty, numTabs);
        }

        /// <summary>
        /// ��������һ�� Tab ��
        /// </summary>
        public const string Tabs = "																																																								";

        /// <summary>
        /// ����Ϊ ����ε� summary ���ֶ���ʽ���ı�ע�����ʽ��ÿһ�е�ǰ�������б�ܡ���������渽��һЩ�ִ���ǰ��ɿ� numTabs �� Tab ��
        /// todo: ����ת��
        /// </summary>
        public static string GetSummary(this object o, string attach, int numTabs)
        {
            string str = GetDescription(o) + attach;
            string tabs = Tabs.Substring(0, numTabs);
            if (string.IsNullOrEmpty(str))
            {
                return @"
" + tabs + @"/// <summary>
" + tabs + @"/// 
" + tabs + @"/// </summary>";
            }
            StringBuilder sb = new StringBuilder();
            sb.Append(@"
" + tabs + @"/// <summary>");
            using (TextReader tr = new StringReader(str))
            {
                while (true)
                {
                    string s = tr.ReadLine();
                    if (s == null) break;
                    if (s.Contains("--"))
                    {
                        if (s.StartsWith("-- ============================")) continue;
                    }
                    sb.Append(@"
" + tabs + @"/// " + s);
                }
            }
            sb.Append(@"
" + tabs + @"/// </summary>");
            return sb.ToString();
        }

        #endregion

        #region GetDataType

        /// <summary>
        /// �����û��Զ����������ͷ���һ������
        /// </summary>
        public static DataType GetDataType(this UserDefinedDataType udt)
        {
            SqlDataType sdt = GetSqlDataType(udt.SystemType, udt.MaxLength);
            DataType t = new DataType(sdt);
            t.MaximumLength = udt.MaxLength;
            t.NumericPrecision = udt.NumericPrecision;
            t.NumericScale = udt.NumericScale;
            // udt.Nullable
            return t;
        }


        /// <summary>
        /// ����һ���ֶ�������������Ӧ�� C# ��������
        /// </summary>
        public static string GetDataType(this Column c)
        {
            return GetDataType(GetDatabase(c), c.DataType);
        }
        /// <summary>
        /// ����һ�����̲���������������Ӧ�� C# ��������
        /// </summary>
        public static string GetDataType(this StoredProcedureParameter p)
        {
            return GetDataType(GetDatabase(p), p.DataType);
        }
        /// <summary>
        /// ����һ����������������������Ӧ�� C# ��������
        /// </summary>
        public static string GetDataType(this UserDefinedFunctionParameter p)
        {
            return GetDataType(GetDatabase(p), p.DataType);
        }
        public static string GetDataType(this UserDefinedFunction f)
        {
            return GetDataType(GetDatabase(f), f.DataType);
        }
        /// <summary>
        /// ����һ��SQL������������Ӧ�� C# ��������
        /// </summary>
        public static string GetDataType(this Database db, DataType dt)
        {
            switch (dt.SqlDataType)
            {
                case SqlDataType.Bit:
                    return "bool";
                case SqlDataType.TinyInt:
                    return "byte";
                case SqlDataType.SmallInt:
                    return "short";
                case SqlDataType.Int:
                    return "int";
                case SqlDataType.BigInt:
                    return "System.Int64";
                case SqlDataType.Decimal:
                case SqlDataType.Numeric:
                case SqlDataType.Money:
                case SqlDataType.SmallMoney:
                    return "decimal";

                case SqlDataType.Float:
                    return "double";

                case SqlDataType.Real:
                    return "float";
                case SqlDataType.DateTime:
                case SqlDataType.SmallDateTime:
                case SqlDataType.DateTime2:
                case SqlDataType.DateTimeOffset:
                case SqlDataType.Time:
                case SqlDataType.Date:
                    return "System.DateTime";
                case SqlDataType.Char:
                case SqlDataType.Text:
                case SqlDataType.VarChar:
                case SqlDataType.NChar:
                case SqlDataType.NText:
                case SqlDataType.NVarChar:
                case SqlDataType.NVarCharMax:
                case SqlDataType.VarCharMax:
                case SqlDataType.Xml:
                    return "string";
                case SqlDataType.Binary:
                case SqlDataType.Image:
                case SqlDataType.VarBinary:
                case SqlDataType.VarBinaryMax:
                case SqlDataType.Timestamp:
                    return "byte[]";
                case SqlDataType.UniqueIdentifier:
                    return "System.Guid";

                case SqlDataType.UserDefinedDataType:
                    return GetDataType(db, GetDataType(db.UserDefinedDataTypes[dt.Name, dt.Schema]));

                case SqlDataType.UserDefinedType:

                default:
                    return "object";
            }
        }


        #endregion

        #region GetDataReaderMethod

        /// <summary>
        /// �����������ͷ����� DataReader ���ֶζ�ȡ�����õķ�����
        /// </summary>
        public static string GetDataReaderMethod(this Column c)
        {
            return GetDataReaderMethod(GetDatabase(c), c.DataType);
        }
        public static string GetDataReaderMethod(this Database db, DataType dt)
        {
            switch (dt.SqlDataType)
            {
                case SqlDataType.Bit:
                    return "GetBoolean";
                case SqlDataType.TinyInt:
                    return "GetByte";
                case SqlDataType.SmallInt:
                    return "GetInt16";
                case SqlDataType.Int:
                    return "GetInt32";
                case SqlDataType.BigInt:
                    return "GetInt64";
                case SqlDataType.Decimal:
                case SqlDataType.Numeric:
                case SqlDataType.Money:
                case SqlDataType.SmallMoney:
                    return "GetDecimal";
                case SqlDataType.Float:
                    return "GetDouble";
                case SqlDataType.Real:
                    return "GetFloat";
                case SqlDataType.DateTime:
                case SqlDataType.SmallDateTime:
                case SqlDataType.DateTime2:
                case SqlDataType.DateTimeOffset:
                case SqlDataType.Date:
                case SqlDataType.Time:
                    return "GetDateTime";
                case SqlDataType.Char:
                case SqlDataType.Text:
                case SqlDataType.VarChar:
                case SqlDataType.NChar:
                case SqlDataType.NText:
                case SqlDataType.NVarChar:
                case SqlDataType.NVarCharMax:
                case SqlDataType.VarCharMax:
                case SqlDataType.Xml:
                    return "GetString";

                case SqlDataType.Binary:
                case SqlDataType.Image:
                case SqlDataType.VarBinary:
                case SqlDataType.VarBinaryMax:
                case SqlDataType.Timestamp:
                    return "GetSqlBinary";				// GetSqlBinary ���������ں�� .Value

                case SqlDataType.UniqueIdentifier:
                    return "GetGuid";

                case SqlDataType.UserDefinedDataType:
                    return GetDataReaderMethod(db, GetDataType(db.UserDefinedDataTypes[dt.Name, dt.Schema]));

                case SqlDataType.UserDefinedType:

                // todo: hierachyid, geography, ...

                default:
                    return "GetValue";

            }
        }


        #endregion

        #region GetNullableDataType

        /// <summary>
        /// ����һ���ֶ�������������Ӧ�� C# �������ͣ��ɿ����ͣ�
        /// </summary>
        public static string GetNullableDataType(this Column c)
        {
            return GetNullableDataType(GetDatabase(c), c.DataType);
        }
        /// <summary>
        /// ����һ����ֵ��������ֵ��������Ӧ�� C# �������ͣ��ɿ����ͣ�
        /// </summary>
        public static string GetNullableDataType(this UserDefinedFunction f)
        {
            return GetNullableDataType(GetDatabase(f), f.DataType);
        }
        /// <summary>
        /// ����һ����������������������Ӧ�� C# �������ͣ��ɿ����ͣ�
        /// </summary>
        public static string GetNullableDataType(this UserDefinedFunctionParameter p)
        {
            return GetNullableDataType(GetDatabase(p), p.DataType);
        }
        /// <summary>
        /// ����һ�����̲���������������Ӧ�� C# �������ͣ��ɿ����ͣ�
        /// </summary>
        public static string GetNullableDataType(this StoredProcedureParameter p)
        {
            return GetNullableDataType(GetDatabase(p), p.DataType);
        }
        /// <summary>
        /// ����һ���ֶ�������������Ӧ�� C# �������ͣ��ɿ����ͣ�
        /// </summary>
        public static string GetNullableDataType(this Database db, DataType dt)
        {
            switch (dt.SqlDataType)
            {
                case SqlDataType.Bit:
                    return "bool?";
                case SqlDataType.TinyInt:
                    return "byte?";
                case SqlDataType.SmallInt:
                    return "short?";
                case SqlDataType.Int:
                    return "int?";
                case SqlDataType.BigInt:
                    return "Int64?";
                case SqlDataType.Decimal:
                case SqlDataType.Numeric:
                case SqlDataType.Money:
                case SqlDataType.SmallMoney:
                    return "decimal?";
                case SqlDataType.Float:
                    return "double?";
                case SqlDataType.Real:
                    return "float?";
                case SqlDataType.DateTime:
                case SqlDataType.SmallDateTime:
                case SqlDataType.DateTime2:
                case SqlDataType.DateTimeOffset:
                case SqlDataType.Date:
                case SqlDataType.Time:
                    return "System.DateTime?";
                case SqlDataType.Char:
                case SqlDataType.Text:
                case SqlDataType.VarChar:
                case SqlDataType.NChar:
                case SqlDataType.NText:
                case SqlDataType.NVarChar:
                case SqlDataType.NVarCharMax:
                case SqlDataType.VarCharMax:
                case SqlDataType.Xml:
                    return "string";
                case SqlDataType.Binary:
                case SqlDataType.Image:
                case SqlDataType.VarBinary:
                case SqlDataType.VarBinaryMax:
                case SqlDataType.Timestamp:
                    return "byte[]";
                case SqlDataType.UniqueIdentifier:
                    return "System.Guid?";

                case SqlDataType.UserDefinedDataType:
                    return GetNullableDataType(db, GetDataType(db.UserDefinedDataTypes[dt.Name, dt.Schema]));

                case SqlDataType.UserDefinedType:

                // todo: hierachyid, geography, ...

                default:
                    return "object";
            }
        }

        #endregion

        #region GetObjectDataSourceParameterDataType


        /// <summary>
        /// ����һ���ֶ�������������Ӧ�� C# ObjectDataSource �����������������
        /// </summary>
        public static string GetObjectDataSourceParameterDataType(this Column c)
        {
            return GetObjectDataSourceParameterDataType(GetDatabase(c), c.DataType);
        }
        /// <summary>
        /// ����һ����ֵ��������ֵ��������Ӧ�� C# ObjectDataSource �����������������
        /// </summary>
        public static string GetObjectDataSourceParameterDataType(this UserDefinedFunction f)
        {
            return GetObjectDataSourceParameterDataType(GetDatabase(f), f.DataType);
        }
        /// <summary>
        /// ����һ����������������������Ӧ�� C# ObjectDataSource �����������������
        /// </summary>
        public static string GetObjectDataSourceParameterDataType(this UserDefinedFunctionParameter p)
        {
            return GetObjectDataSourceParameterDataType(GetDatabase(p), p.DataType);
        }
        /// <summary>
        /// ����һ�����̲���������������Ӧ�� C# ObjectDataSource �����������������
        /// </summary>
        public static string GetObjectDataSourceParameterDataType(this StoredProcedureParameter p)
        {
            return GetObjectDataSourceParameterDataType(GetDatabase(p), p.DataType);
        }

        public static string GetObjectDataSourceParameterDataType(this Database db, DataType dt)
        {
            switch (dt.SqlDataType)
            {
                case SqlDataType.SmallInt:
                    return "short";
                case SqlDataType.Int:
                    return "int";
                case SqlDataType.BigInt:
                    return "System.Int64";
                case SqlDataType.Decimal:
                case SqlDataType.Numeric:
                case SqlDataType.Money:
                case SqlDataType.SmallMoney:
                    return "decimal";
                case SqlDataType.TinyInt:
                    return "byte";
                case SqlDataType.Float:
                    return "double";
                case SqlDataType.Real:
                    return "float";
                case SqlDataType.Bit:
                    return "bool";
                case SqlDataType.DateTime:
                case SqlDataType.SmallDateTime:
                case SqlDataType.DateTime2:
                case SqlDataType.DateTimeOffset:
                case SqlDataType.Date:
                case SqlDataType.Time:
                    return "System.DateTime";
                case SqlDataType.UserDefinedTableType:
                    return "System.Data.DataTable";
                case SqlDataType.Binary:
                case SqlDataType.Image:
                case SqlDataType.VarBinary:
                case SqlDataType.VarBinaryMax:
                case SqlDataType.Timestamp:
                    return "byte[]";
                case SqlDataType.Char:
                case SqlDataType.Text:
                case SqlDataType.VarChar:
                case SqlDataType.NChar:
                case SqlDataType.NText:
                case SqlDataType.NVarChar:
                case SqlDataType.UniqueIdentifier:
                case SqlDataType.NVarCharMax:
                case SqlDataType.VarCharMax:
                case SqlDataType.Xml:
                    return "string";

                case SqlDataType.UserDefinedDataType:
                    return GetObjectDataSourceParameterDataType(db, GetDataType(db.UserDefinedDataTypes[dt.Name, dt.Schema]));

                case SqlDataType.UserDefinedType:

                // todo: hierachyid, geography, ...

                default:
                    return "object";
            }
        }



        /// <summary>
        /// �����ֶ��������ͷ��� C# Ĭ��ֵ�����
        /// </summary>
        public static string GetObjectDataSourceParameterDefaultValue(this Column c)
        {
            return GetObjectDataSourceParameterDefaultValue(GetDatabase(c), c.DataType);
        }
        /// <summary>
        /// ���ݹ��̲����������ͷ��� C# Ĭ��ֵ�����
        /// </summary>
        public static string GetObjectDataSourceParameterDefaultValue(this StoredProcedureParameter p)
        {
            return GetObjectDataSourceParameterDefaultValue(GetDatabase(p), p.DataType);
        }
        /// <summary>
        /// ���ݺ��������������ͷ��� C# Ĭ��ֵ�����
        /// </summary>
        public static string GetObjectDataSourceParameterDefaultValue(this UserDefinedFunctionParameter p)
        {
            return GetObjectDataSourceParameterDefaultValue(GetDatabase(p), p.DataType);
        }
        /// <summary>
        /// �����������ͷ��� C# Ĭ��ֵ�����
        /// </summary>
        public static string GetObjectDataSourceParameterDefaultValue(this Database db, DataType dt)
        {
            switch (dt.SqlDataType)
            {
                case SqlDataType.BigInt:
                case SqlDataType.Decimal:
                case SqlDataType.Int:
                case SqlDataType.Numeric:
                case SqlDataType.SmallInt:
                case SqlDataType.Money:
                case SqlDataType.TinyInt:
                case SqlDataType.SmallMoney:
                case SqlDataType.Float:
                case SqlDataType.Real:
                    return "0";
                case SqlDataType.Bit:
                    return "false";
                case SqlDataType.DateTime:
                case SqlDataType.SmallDateTime:
                case SqlDataType.DateTime2:
                case SqlDataType.DateTimeOffset:
                    return "1900-1-1";
                case SqlDataType.UserDefinedTableType:
                    return "null";
                case SqlDataType.Char:
                case SqlDataType.Text:
                case SqlDataType.VarChar:
                case SqlDataType.NChar:
                case SqlDataType.NText:
                case SqlDataType.NVarChar:
                case SqlDataType.Binary:
                case SqlDataType.UniqueIdentifier:
                case SqlDataType.NVarCharMax:
                case SqlDataType.VarCharMax:
                case SqlDataType.Xml:


                case SqlDataType.UserDefinedDataType:
                    return GetObjectDataSourceParameterDefaultValue(db, GetDataType(db.UserDefinedDataTypes[dt.Name, dt.Schema]));

                case SqlDataType.UserDefinedType:

                // todo: hierachyid, geography, ...

                default:
                    return string.Empty;
            }
        }

        #endregion

        #region GetSqlDbType

        /// <summary>
        /// ����һ���ֶ�������������Ӧ�� DbType
        /// </summary>
        public static string GetSqlDbType(this Column c)
        {
            return GetSqlDbType(GetDatabase(c), c.DataType);
        }
        /// <summary>
        /// ����һ�����̲���������������Ӧ�� DbType
        /// </summary>
        public static string GetSqlDbType(this StoredProcedureParameter p)
        {
            return GetSqlDbType(GetDatabase(p), p.DataType);
        }
        /// <summary>
        /// ����һ����������������������Ӧ�� SqlDbType
        /// </summary>
        public static string GetSqlDbType(this UserDefinedFunctionParameter p)
        {
            return GetSqlDbType(GetDatabase(p), p.DataType);
        }
        public static string GetSqlDbType(this Database db, DataType dt)
        {

            switch (dt.SqlDataType)
            {
                case SqlDataType.BigInt:
                    return "System.Data.SqlDbType.BigInt";
                case SqlDataType.Decimal:
                    return "System.Data.SqlDbType.Decimal";
                case SqlDataType.Int:
                    return "System.Data.SqlDbType.Int";
                case SqlDataType.Numeric:
                    return "System.Data.SqlDbType.Decimal";
                case SqlDataType.SmallInt:
                    return "System.Data.SqlDbType.SmallInt";
                case SqlDataType.Money:
                    return "System.Data.SqlDbType.Money";
                case SqlDataType.TinyInt:
                    return "System.Data.SqlDbType.TinyInt";
                case SqlDataType.SmallMoney:
                    return "System.Data.SqlDbType.SmallMoney";
                case SqlDataType.Bit:
                    return "System.Data.SqlDbType.Bit";
                case SqlDataType.Float:
                    return "System.Data.SqlDbType.Float";
                case SqlDataType.Real:
                    return "System.Data.SqlDbType.Real";
                case SqlDataType.DateTime:
                    return "System.Data.SqlDbType.DateTime";
                case SqlDataType.SmallDateTime:
                    return "System.Data.SqlDbType.SmallDateTime";
                case SqlDataType.Char:
                    return "System.Data.SqlDbType.Char";
                case SqlDataType.Text:
                    return "System.Data.SqlDbType.Text";
                case SqlDataType.VarChar:
                case SqlDataType.VarCharMax:
                    return "System.Data.SqlDbType.VarChar";
                case SqlDataType.NChar:
                    return "System.Data.SqlDbType.NChar";
                case SqlDataType.NText:
                    return "System.Data.SqlDbType.NText";
                case SqlDataType.NVarChar:
                case SqlDataType.NVarCharMax:
                    return "System.Data.SqlDbType.NVarChar";
                case SqlDataType.Binary:
                    return "System.Data.SqlDbType.Binary";
                case SqlDataType.Image:
                    return "System.Data.SqlDbType.Image";
                case SqlDataType.VarBinary:
                case SqlDataType.VarBinaryMax:
                    return "System.Data.SqlDbType.VarBinary";
                case SqlDataType.UniqueIdentifier:
                    return "System.Data.SqlDbType.UniqueIdentifier";

                case SqlDataType.UserDefinedDataType:
                    return GetSqlDbType(db, GetDataType(db.UserDefinedDataTypes[dt.Name, dt.Schema]));

                case SqlDataType.UserDefinedType:
                    return "System.Data.SqlDbType.Udt";

                case SqlDataType.UserDefinedTableType:
                    return "System.Data.SqlDbType.Structured";

                case SqlDataType.DateTime2:
                    return "System.Data.SqlDbType.DateTime2";
                case SqlDataType.DateTimeOffset:
                    return "System.Data.SqlDbType.DateTimeOffset";
                case SqlDataType.Date:
                    return "System.Data.SqlDbType.Date";
                case SqlDataType.Time:
                    return "System.Data.SqlDbType.Time";

                case SqlDataType.Xml:
                    return "System.Data.SqlDbType.Xml";

                case SqlDataType.Timestamp:
                    return "System.Data.SqlDbType.Timestamp";

                default:
                    return "System.Data.SqlDbType.Variant";
            }
        }

        #endregion



        #region GetDbTypeLength

        /// <summary>
        /// ����һ���ֶ�������������Ӧ�� SqlDb �������͵���������
        /// </summary>
        public static string GetDbTypeLength(this Column c)
        {
            return GetDbTypeLength(GetDatabase(c), c.DataType);
        }
        /// <summary>
        /// ����һ�����̲���������������Ӧ�� SqlDb �������͵���������
        /// </summary>
        public static string GetDbTypeLength(this StoredProcedureParameter p)
        {
            return GetDbTypeLength(GetDatabase(p), p.DataType);
        }
        /// <summary>
        /// ����һ����������������������Ӧ�� SqlDb �������͵���������
        /// </summary>
        public static string GetDbTypeLength(this UserDefinedFunctionParameter p)
        {
            return GetDbTypeLength(GetDatabase(p), p.DataType);
        }
        /// <summary>
        /// ����һ��SQL������������Ӧ�� SqlDb �������͵���������
        /// </summary>
        public static string GetDbTypeLength(this Database db, DataType dt)
        {
            switch (dt.SqlDataType)
            {
                case SqlDataType.UserDefinedDataType:
                    return GetDbTypeLength(db, GetDataType(db.UserDefinedDataTypes[dt.Name, dt.Schema]));
                case SqlDataType.Decimal:
                case SqlDataType.Char:
                case SqlDataType.VarChar:
                case SqlDataType.NChar:
                case SqlDataType.NVarChar:
                case SqlDataType.Timestamp:
                    return dt.MaximumLength.ToString();
                case SqlDataType.Text:
                case SqlDataType.NText:
                case SqlDataType.Binary:
                case SqlDataType.Image:
                case SqlDataType.VarBinary:
                case SqlDataType.NVarCharMax:
                case SqlDataType.VarCharMax:
                case SqlDataType.VarBinaryMax:
                case SqlDataType.Xml:
                    return int.MaxValue.ToString();
                case SqlDataType.UniqueIdentifier:
                case SqlDataType.BigInt:
                case SqlDataType.Int:
                case SqlDataType.Numeric:
                case SqlDataType.SmallInt:
                case SqlDataType.Money:
                case SqlDataType.TinyInt:
                case SqlDataType.SmallMoney:
                case SqlDataType.Bit:
                case SqlDataType.Float:
                case SqlDataType.Real:
                case SqlDataType.Date:
                case SqlDataType.Time:
                case SqlDataType.DateTime:
                case SqlDataType.SmallDateTime:
                case SqlDataType.DateTime2:
                case SqlDataType.DateTimeOffset:
                case SqlDataType.Geography:
                case SqlDataType.Geometry:
                case SqlDataType.HierarchyId:
                case SqlDataType.UserDefinedType:

                default:
                    return "0";
            }
        }

        #endregion

        #region GetDefaultValue

        /// <summary>
        /// �����ֶ��������ͷ��� C# Ĭ��ֵ�����
        /// </summary>
        public static string GetDefaultValue(this Column c)
        {
            return GetDefaultValue(GetDatabase(c), c.DataType);
        }
        /// <summary>
        /// ���ݹ��̲����������ͷ��� C# Ĭ��ֵ�����
        /// </summary>
        public static string GetDefaultValue(this StoredProcedureParameter p)
        {
            return GetDefaultValue(GetDatabase(p), p.DataType);
        }
        /// <summary>
        /// ���ݺ��������������ͷ��� C# Ĭ��ֵ�����
        /// </summary>
        public static string GetDefaultValue(this UserDefinedFunctionParameter p)
        {
            return GetDefaultValue(GetDatabase(p), p.DataType);
        }
        /// <summary>
        /// �����������ͷ��� C# Ĭ��ֵ�����
        /// </summary>
        public static string GetDefaultValue(this Database db, DataType dt)
        {
            switch (dt.SqlDataType)
            {
                case SqlDataType.UserDefinedDataType:
                    return GetDefaultValue(db, GetDataType(db.UserDefinedDataTypes[dt.Name, dt.Schema]));

                case SqlDataType.Bit:
                    return "false";

                case SqlDataType.BigInt:
                case SqlDataType.Decimal:
                case SqlDataType.Int:
                case SqlDataType.Numeric:
                case SqlDataType.SmallInt:
                case SqlDataType.Money:
                case SqlDataType.TinyInt:
                case SqlDataType.SmallMoney:
                case SqlDataType.Float:
                case SqlDataType.Real:
                    return "0";

                case SqlDataType.Date:
                case SqlDataType.Time:
                case SqlDataType.DateTime:
                case SqlDataType.SmallDateTime:
                case SqlDataType.DateTime2:
                case SqlDataType.DateTimeOffset:
                    return "System.DateTime.Now";

                case SqlDataType.Binary:
                case SqlDataType.Image:
                case SqlDataType.VarBinary:
                case SqlDataType.VarBinaryMax:
                case SqlDataType.Timestamp:
                    return "new byte[] { }";

                case SqlDataType.UniqueIdentifier:
                    return "System.Guid.Empty";

                case SqlDataType.Geography:
                case SqlDataType.Geometry:
                case SqlDataType.UserDefinedTableType:
                case SqlDataType.HierarchyId:
                    return "null";

                case SqlDataType.Char:
                case SqlDataType.Text:
                case SqlDataType.VarChar:
                case SqlDataType.NChar:
                case SqlDataType.NText:
                case SqlDataType.NVarChar:
                case SqlDataType.NVarCharMax:
                case SqlDataType.VarCharMax:
                case SqlDataType.Xml:
                default:
                    return "\"\"";
            }
        }

        #endregion

        #region Get SP's ParmDeclareStr

        /// <summary>
        /// �����ֶ���������ȡ����/�������������ִ�
        /// </summary>
        public static string GetParmDeclareStr(this Column c)
        {
            switch (c.DataType.SqlDataType)
            {
                case SqlDataType.Int:
                case SqlDataType.BigInt:
                case SqlDataType.Numeric:
                case SqlDataType.SmallInt:
                case SqlDataType.Money:
                case SqlDataType.TinyInt:
                case SqlDataType.SmallMoney:
                case SqlDataType.Bit:
                case SqlDataType.Float:
                case SqlDataType.Real:
                case SqlDataType.Text:
                case SqlDataType.NText:
                case SqlDataType.Image:
                case SqlDataType.Date:
                case SqlDataType.Time:
                case SqlDataType.DateTime:
                case SqlDataType.SmallDateTime:
                case SqlDataType.DateTime2:
                case SqlDataType.DateTimeOffset:
                case SqlDataType.Timestamp:
                case SqlDataType.UniqueIdentifier:
                case SqlDataType.UserDefinedTableType:
                case SqlDataType.UserDefinedDataType:
                case SqlDataType.UserDefinedType:
                case SqlDataType.Geography:
                case SqlDataType.Geometry:
                case SqlDataType.HierarchyId:
                case SqlDataType.Xml:
                case SqlDataType.Variant:
                    return c.DataType.Name.ToUpper();

                case SqlDataType.Decimal:
                    return c.DataType.Name.ToUpper() + " (" + c.DataType.NumericPrecision.ToString() + "," + c.DataType.NumericScale.ToString() + ")";

                default:
                    return c.DataType.Name.ToUpper() + "(" + (c.DataType.MaximumLength == -1 ? "MAX" : c.DataType.MaximumLength.ToString()) + ")";
            }
        }

        #endregion


        #region Check Type

        /// <summary>
        /// �ж��Ƿ�Ϊ�ִ����������ͣ�ƴ�ִ�ʱ��Ҫ�����ţ�
        /// </summary>
        public static bool CheckNeedQuote(this Column c)
        {
            return Utils.CheckIsStringType(c) || Utils.CheckIsDateTimeType(c);
        }

        /// <summary>
        /// �жϲ������ֶ����������Ƿ�Ϊ��ֵ���͡�
        /// </summary>
        public static bool CheckIsValueType(this Column c)
        {
            return CheckIsValueType(GetDatabase(c), c.DataType);
        }
        /// <summary>
        /// �жϲ������ֶ����������Ƿ�Ϊ��ֵ���͡�
        /// </summary>
        public static bool CheckIsValueType(this Database db, DataType dt)
        {
            switch (dt.SqlDataType)
            {
                case SqlDataType.UserDefinedDataType:
                    return CheckIsValueType(db, GetDataType(db.UserDefinedDataTypes[dt.Name, dt.Schema]));
                case SqlDataType.BigInt:
                case SqlDataType.Decimal:
                case SqlDataType.Int:
                case SqlDataType.Numeric:
                case SqlDataType.SmallInt:
                case SqlDataType.Money:
                case SqlDataType.TinyInt:
                case SqlDataType.SmallMoney:
                case SqlDataType.Bit:
                case SqlDataType.Float:
                case SqlDataType.Real:
                case SqlDataType.Date:
                case SqlDataType.Time:
                case SqlDataType.DateTime:
                case SqlDataType.SmallDateTime:
                case SqlDataType.DateTime2:
                case SqlDataType.DateTimeOffset:
                case SqlDataType.UniqueIdentifier:
                    return true;

                default:
                    return false;
            }
        }


        /// <summary>
        /// �ж�һ���ֶε����������Ƿ�Ϊ ���ִ��ࡱ
        /// </summary>
        public static bool CheckIsStringType(this Column c)
        {
            return CheckIsStringType(GetDatabase(c), c.DataType);
        }
        /// <summary>
        /// �ж�һ�����������������Ƿ�Ϊ ���ִ��ࡱ
        /// </summary>
        public static bool CheckIsStringType(this UserDefinedFunctionParameter p)
        {
            return CheckIsStringType(GetDatabase(p), p.DataType);
        }
        /// <summary>
        /// �ж�һ�����̵����������Ƿ�Ϊ ���ִ��ࡱ
        /// </summary>
        public static bool CheckIsStringType(this StoredProcedureParameter p)
        {
            return CheckIsStringType(GetDatabase(p), p.DataType);
        }
        /// <summary>
        /// �ж�һ����������ֵ�����������Ƿ�Ϊ ���ִ��ࡱ
        /// </summary>
        public static bool CheckIsStringType(this UserDefinedFunction f)
        {
            return CheckIsStringType(GetDatabase(f), f.DataType);
        }
        /// <summary>
        /// �ж�һ�����������Ƿ�Ϊ ���ִ��ࡱ
        /// </summary>
        public static bool CheckIsStringType(this Database db, DataType dt)
        {
            SqlDataType sdt = dt.SqlDataType;
            if (sdt == SqlDataType.UserDefinedDataType)
                return CheckIsStringType(db, GetDataType(db.UserDefinedDataTypes[dt.Name, dt.Schema]));
            return (sdt == SqlDataType.Char
                    || sdt == SqlDataType.Text
                    || sdt == SqlDataType.VarChar
                    || sdt == SqlDataType.NChar
                    || sdt == SqlDataType.NText
                    || sdt == SqlDataType.NVarChar
                    || sdt == SqlDataType.NVarCharMax
                    || sdt == SqlDataType.VarCharMax
                    || sdt == SqlDataType.Xml);

            // todo: sql08 hierachyid
        }



        /// <summary>
        /// �ж�һ���ֶε����������Ƿ�Ϊ �������ࡱ
        /// </summary>
        public static bool CheckIsDateTimeType(this Column c)
        {
            return CheckIsDateTimeType(GetDatabase(c), c.DataType);
        }
        /// <summary>
        /// �ж�һ�����������������Ƿ�Ϊ �������ࡱ
        /// </summary>
        public static bool CheckIsDateTimeType(this UserDefinedFunctionParameter p)
        {
            return CheckIsDateTimeType(GetDatabase(p), p.DataType);
        }
        /// <summary>
        /// �ж�һ�����̵����������Ƿ�Ϊ �������ࡱ
        /// </summary>
        public static bool CheckIsDateTimeType(this StoredProcedureParameter p)
        {
            return CheckIsDateTimeType(GetDatabase(p), p.DataType);
        }
        /// <summary>
        /// �ж�һ����������ֵ�����������Ƿ�Ϊ �������ࡱ
        /// </summary>
        public static bool CheckIsDateTimeType(this UserDefinedFunction f)
        {
            return CheckIsDateTimeType(GetDatabase(f), f.DataType);
        }
        /// <summary>
        /// �ж�һ�����������Ƿ�Ϊ �������ࡱ
        /// </summary>
        public static bool CheckIsDateTimeType(this Database db, DataType dt)
        {
            SqlDataType sdt = dt.SqlDataType;
            if (sdt == SqlDataType.UserDefinedDataType)
                return CheckIsDateTimeType(db, GetDataType(db.UserDefinedDataTypes[dt.Name, dt.Schema]));
            return (sdt == SqlDataType.DateTimeOffset
                    || sdt == SqlDataType.DateTime2
                    || sdt == SqlDataType.DateTime
                    || sdt == SqlDataType.Date
                    || sdt == SqlDataType.Time);
        }



        /// <summary>
        /// �ж�һ���ֶε����������Ƿ�Ϊ ��Guid �ࡱ
        /// </summary>
        public static bool CheckIsGuidType(this Column c)
        {
            return CheckIsGuidType(GetDatabase(c), c.DataType);
        }
        /// <summary>
        /// �ж�һ�����������������Ƿ�Ϊ ��Guid �ࡱ
        /// </summary>
        public static bool CheckIsGuidType(this UserDefinedFunctionParameter p)
        {
            return CheckIsGuidType(GetDatabase(p), p.DataType);
        }
        /// <summary>
        /// �ж�һ�����̵����������Ƿ�Ϊ ��Guid �ࡱ
        /// </summary>
        public static bool CheckIsGuidType(this StoredProcedureParameter p)
        {
            return CheckIsGuidType(GetDatabase(p), p.DataType);
        }
        /// <summary>
        /// �ж�һ����������ֵ�����������Ƿ�Ϊ ��Guid �ࡱ
        /// </summary>
        public static bool CheckIsGuidType(this UserDefinedFunction f)
        {
            return CheckIsGuidType(GetDatabase(f), f.DataType);
        }
        /// <summary>
        /// �ж�һ�����������Ƿ�Ϊ ��Guid �ࡱ
        /// </summary>
        public static bool CheckIsGuidType(this Database db, DataType dt)
        {
            SqlDataType sdt = dt.SqlDataType;
            if (sdt == SqlDataType.UserDefinedDataType)
                return CheckIsGuidType(db, GetDataType(db.UserDefinedDataTypes[dt.Name, dt.Schema]));
            return (sdt == SqlDataType.UniqueIdentifier);
        }


        /// <summary>
        /// �ж�һ���ֶε����������Ƿ�Ϊ ��Boolean �ࡱ
        /// </summary>
        public static bool CheckIsBooleanType(this Column c)
        {
            return CheckIsBooleanType(GetDatabase(c), c.DataType);
        }
        /// <summary>
        /// �ж�һ�����������������Ƿ�Ϊ ��Boolean �ࡱ
        /// </summary>
        public static bool CheckIsBooleanType(this UserDefinedFunctionParameter p)
        {
            return CheckIsBooleanType(GetDatabase(p), p.DataType);
        }
        /// <summary>
        /// �ж�һ�����̵����������Ƿ�Ϊ ��Boolean �ࡱ
        /// </summary>
        public static bool CheckIsBooleanType(this StoredProcedureParameter p)
        {
            return CheckIsBooleanType(GetDatabase(p), p.DataType);
        }
        /// <summary>
        /// �ж�һ����������ֵ�����������Ƿ�Ϊ ��Boolean �ࡱ
        /// </summary>
        public static bool CheckIsBooleanType(this UserDefinedFunction f)
        {
            return CheckIsBooleanType(GetDatabase(f), f.DataType);
        }
        /// <summary>
        /// �ж�һ�����������Ƿ�Ϊ ��Boolean �ࡱ
        /// </summary>
        public static bool CheckIsBooleanType(this Database db, DataType dt)
        {
            SqlDataType sdt = dt.SqlDataType;
            if (sdt == SqlDataType.UserDefinedDataType)
                return CheckIsGuidType(db, GetDataType(db.UserDefinedDataTypes[dt.Name, dt.Schema]));
            return (sdt == SqlDataType.Bit);
        }




        /// <summary>
        /// �ж�һ���ֶε����������Ƿ�Ϊ ������ �ࡱ
        /// </summary>
        public static bool CheckIsNumericType(this Column c)
        {
            return CheckIsNumericType(GetDatabase(c), c.DataType);
        }
        /// <summary>
        /// �ж�һ�����������������Ƿ�Ϊ ������ �ࡱ
        /// </summary>
        public static bool CheckIsNumericType(this UserDefinedFunctionParameter p)
        {
            return CheckIsNumericType(GetDatabase(p), p.DataType);
        }
        /// <summary>
        /// �ж�һ�����̵����������Ƿ�Ϊ ������ �ࡱ
        /// </summary>
        public static bool CheckIsNumericType(this StoredProcedureParameter p)
        {
            return CheckIsNumericType(GetDatabase(p), p.DataType);
        }
        /// <summary>
        /// �ж�һ����������ֵ�����������Ƿ�Ϊ ������ �ࡱ
        /// </summary>
        public static bool CheckIsNumericType(this UserDefinedFunction f)
        {
            return CheckIsNumericType(GetDatabase(f), f.DataType);
        }		/// <summary>
        /// �ж�һ�����������Ƿ�Ϊ �������ࡱ
        /// </summary>
        public static bool CheckIsNumericType(this Database db, DataType dt)
        {
            SqlDataType sdt = dt.SqlDataType;
            if (sdt == SqlDataType.UserDefinedDataType)
                return CheckIsGuidType(db, GetDataType(db.UserDefinedDataTypes[dt.Name, dt.Schema]));
            return (sdt == SqlDataType.BigInt
                || sdt == SqlDataType.Decimal
                || sdt == SqlDataType.Float
                || sdt == SqlDataType.Int
                || sdt == SqlDataType.Money
                || sdt == SqlDataType.Numeric
                || sdt == SqlDataType.Real
                || sdt == SqlDataType.SmallInt
                || sdt == SqlDataType.SmallMoney
                || sdt == SqlDataType.TinyInt);
        }




        /// <summary>
        /// �ж�һ���ֶε����������Ƿ�Ϊ ��Binary �ࡱ
        /// </summary>
        public static bool CheckIsBinaryType(this Column c)
        {
            return CheckIsBinaryType(GetDatabase(c), c.DataType);
        }
        /// <summary>
        /// �ж�һ�����������������Ƿ�Ϊ ��Binary �ࡱ
        /// </summary>
        public static bool CheckIsBinaryType(this UserDefinedFunctionParameter p)
        {
            return CheckIsBinaryType(GetDatabase(p), p.DataType);
        }
        /// <summary>
        /// �ж�һ�����̵����������Ƿ�Ϊ ��Binary �ࡱ
        /// </summary>
        public static bool CheckIsBinaryType(this StoredProcedureParameter p)
        {
            return CheckIsBinaryType(GetDatabase(p), p.DataType);
        }
        /// <summary>
        /// �ж�һ����������ֵ�����������Ƿ�Ϊ ��Binary �ࡱ
        /// </summary>
        public static bool CheckIsBinaryType(this UserDefinedFunction f)
        {
            return CheckIsBinaryType(GetDatabase(f), f.DataType);
        }
        /// <summary>
        /// �ж�һ�����������Ƿ�Ϊ ��Binary �ࡱ
        /// </summary>
        public static bool CheckIsBinaryType(this Database db, DataType dt)
        {
            SqlDataType sdt = dt.SqlDataType;
            if (sdt == SqlDataType.UserDefinedDataType)
                return CheckIsGuidType(db, GetDataType(db.UserDefinedDataTypes[dt.Name, dt.Schema]));
            return (sdt == SqlDataType.Image
                    || sdt == SqlDataType.Binary
                    || sdt == SqlDataType.Timestamp
                    || sdt == SqlDataType.VarBinaryMax
                    || sdt == SqlDataType.VarBinary);
        }



        ///// <summary>
        ///// �ж�һ�����������Ƿ�Ϊ ��Nullable<> �ࡱ  (ֵ��)
        ///// </summary>
        //public static bool CheckIsNullableType(DataType sdt)
        //{
        //    return 
        //}

        /// <summary>
        /// �ж�һ���ִ��Ƿ�Ϊ C# �Ĺؼ���
        /// http://msdn2.microsoft.com/en-us/library/x53a06bb.aspx
        /// </summary>
        public static bool CheckIsKeywords(this string s)
        {
            s = s.ToLower();
            return s == "abstract" ||
                s == "event" ||
                s == "new" ||
                s == "struct" ||
                s == "as" ||
                s == "explicit" ||
                s == "null" ||
                s == "switch" ||
                s == "bas" ||
                s == "extern" ||
                s == "object" ||
                s == "this" ||

                s == "boolean" ||
                s == "false" ||
                s == "operator" ||
                s == "throw" ||

                s == "break" ||
                s == "finally" ||
                s == "out" ||
                s == "true" ||

                s == "byte" ||
                s == "fixed" ||
                s == "override" ||
                s == "try" ||

                s == "case" ||
                s == "float" ||
                s == "params" ||
                s == "typeof" ||

                s == "catch" ||
                s == "for" ||
                s == "private" ||
                s == "uint" ||

                s == "char" ||
                s == "foreach" ||
                s == "protected" ||
                s == "ulong" ||

                s == "checked" ||
                s == "goto" ||
                s == "public" ||
                s == "unchecked" ||

                s == "class" ||
                s == "if" ||
                s == "readonly" ||
                s == "unsafe" ||

                s == "const" ||
                s == "implicit" ||
                s == "ref" ||
                s == "ushort" ||

                s == "continue" ||
                s == "in" ||
                s == "return" ||
                s == "using" ||

                s == "decimal" ||
                s == "int32" ||
                s == "sbyte" ||
                s == "virtual" ||

                s == "default" ||
                s == "interface" ||
                s == "sealed" ||
                s == "volatile" ||

                s == "delegate" ||
                s == "public" ||
                s == "int16" ||
                s == "void" ||

                s == "do" ||
                s == "is" ||
                s == "sizeof" ||
                s == "while" ||

                s == "double" ||
                s == "lock" ||
                s == "stackalloc" ||


                s == "else" ||
                s == "int64" ||
                s == "static" ||


                s == "enum" ||
                s == "namespace" ||
                s == "string";
        }

        /// <summary>
        /// ��һ����ͼ�Ƿ�Ϊ �������Ļ���������ָ���Լ���������
        /// </summary>
        public static bool CheckIsTree(this View v)
        {
            return CheckIsTree(GetBaseTable(v));
        }


        /// <summary>
        /// ��һ�����Ƿ�Ϊ �����������ָ���Լ���������
        /// </summary>
        public static bool CheckIsTree(this Table t)
        {
            if (t == null) return false;
            List<Column> pks = Utils.GetPrimaryKeyColumns(t);
            if (pks == null || pks.Count == 0)		//û��������
            {
                return false;
            }

            if (t.ForeignKeys.Count == 0)
            {
                return false;
            }

            foreach (ForeignKey fk in t.ForeignKeys)
            {
                if (fk.ReferencedTable != t.Name || fk.ReferencedTableSchema != t.Schema) continue;
                int equaled = 0;
                foreach (ForeignKeyColumn fkc in fk.Columns)		//�ж��Ƿ�һ�����Լ�������ֶζ����ڵ�ǰ��
                {
                    if (fkc.Parent.Parent == t) equaled++;
                }
                if (equaled == fk.Columns.Count)					//��ǰ��Ϊ����
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// ��� ������ ·���� �Ƿ���Ҫ������
        /// </summary>
        public static bool CheckNeedQuote(this string s)
        {
            return s.Contains(" ") || s.Contains("&") || s.Contains("(") || s.Contains(")") || s.Contains("[") ||
                    s.Contains("]") || s.Contains("{") || s.Contains("}") || s.Contains("^") || s.Contains("=") ||
                    s.Contains(";") || s.Contains("!") || s.Contains("'") || s.Contains("+") || s.Contains(",") ||
                    s.Contains("`") || s.Contains("~");
        }


        /// <summary>
        /// �ж�һ�����Ƿ������ó�һ����ͼ�Ļ���
        /// </summary>
        public static bool CheckIsBaseTable(this View v, Table t)
        {
            return Utils.GetBaseTableName(v) == t.Name && Utils.GetBaseTableSchema(v) == t.Schema;
        }
        /// <summary>
        /// �ж�һ�����Ƿ������ó�һ����ͼ�Ļ���
        /// </summary>
        public static bool CheckIsBaseTable(this Table t, View v)
        {
            return Utils.GetBaseTableName(v) == t.Name && Utils.GetBaseTableSchema(v) == t.Schema;
        }

        /// <summary>
        /// �ж�һ�����Ƿ����ʸ��һ����ͼ�Ļ���
        /// </summary>
        public static bool CheckCanbeBaseTable(this View v, Table t)
        {
            //��� t �Ƿ�Ϊ v ���ֶ��Ӽ�
            bool isContain = true;
            foreach (Column c in t.Columns)
            {
                if (!v.Columns.Contains(c.Name))
                {
                    isContain = false;
                    break;
                }
            }
            return isContain;
        }
        /// <summary>
        /// �ж�һ�����Ƿ����ʸ��һ����ͼ�Ļ���
        /// </summary>
        public static bool CheckCanbeBaseTable(this Table t, View v)
        {
            return CheckCanbeBaseTable(v, t);
        }



        /// <summary>
        /// ���һ���ֶ��Ƿ�Ϊ�����������ֶ�֮һ
        /// </summary>
        /// <param name="c"></param>
        /// <returns></returns>
        public static bool CheckIsForeignKey(this Column c)
        {
            Table t = (Table)c.Parent;
            foreach (ForeignKey fk in t.ForeignKeys)
            {
                foreach (ForeignKeyColumn fkc in fk.Columns)
                {
                    Column o = t.Columns[fkc.Name];
                    if (c == o) return true;
                }
            }
            return false;
        }


        

        #endregion

        #region Escape

        /// <summary>
        /// �Ƿ�Ϊ�����еķ����������������ɼܹ���ǰ׺������ǰ�ֹ���ʼ����
        /// </summary>
        public static string SchemaSplitter = null;

        /// <summary>
        /// ȡת�����ֶ����ƣ����������������������ȣ��ִ���ȥ�ո񣬹����࣬���Ա���еķǷ��ַ�������� C# ��������ͬ�����ֶ�����ǰ��� _��
        /// </summary>
        public static string GetEscapeName(this object o)
        {
            if (o.GetType() == typeof(Column))
            {
                Column c = (Column)o;
                if (c.Parent.GetType() == typeof(Table))
                {
                    Table t = (Table)c.Parent;
                    return GetEscapeName(c.Name) + (t.Name == c.Name ? "_" : "");
                }
                else if (c.Parent.GetType() == typeof(UserDefinedTableType))
                {
                    UserDefinedTableType t = (UserDefinedTableType)c.Parent;
                    return GetEscapeName(c.Name) + (t.Name == c.Name ? "_" : "");
                }
                else if (c.Parent.GetType() == typeof(View))
                {
                    View t = (View)c.Parent;
                    return GetEscapeName(c.Name) + (t.Name == c.Name ? "_" : "");
                }
                else if (c.Parent.GetType() == typeof(UserDefinedFunction))
                {
                    UserDefinedFunction t = (UserDefinedFunction)c.Parent;
                    return GetEscapeName(c.Name) + (t.Name == c.Name ? "_" : "");
                }
            }
            else if (o.GetType() == typeof(Table))
            {
                Table t = (Table)o;
                return (!string.IsNullOrEmpty(SchemaSplitter) ? (GetEscapeName(t.Schema) + SchemaSplitter) : "") + GetEscapeName(t.Name);
            }
            else if (o.GetType() == typeof(UserDefinedTableType))
            {
                UserDefinedTableType t = (UserDefinedTableType)o;
                return (!string.IsNullOrEmpty(SchemaSplitter) ? (GetEscapeName(t.Schema) + SchemaSplitter) : "") + GetEscapeName(t.Name);
            }
            else if (o.GetType() == typeof(View))
            {
                View t = (View)o;
                return (!string.IsNullOrEmpty(SchemaSplitter) ? (GetEscapeName(t.Schema) + SchemaSplitter) : "") + GetEscapeName(t.Name);
            }
            else if (o.GetType() == typeof(UserDefinedFunction))
            {
                UserDefinedFunction t = (UserDefinedFunction)o;
                return (!string.IsNullOrEmpty(SchemaSplitter) ? (GetEscapeName(t.Schema) + SchemaSplitter) : "") + GetEscapeName(t.Name);
            }
            else if (o.GetType() == typeof(UserDefinedFunctionParameter))
            {
                UserDefinedFunctionParameter t = (UserDefinedFunctionParameter)o;
                return GetEscapeName(t.Name.Substring(1));
            }
            else if (o.GetType() == typeof(StoredProcedure))
            {
                StoredProcedure t = (StoredProcedure)o;
                return (!string.IsNullOrEmpty(SchemaSplitter) ? (GetEscapeName(t.Schema) + SchemaSplitter) : "") + GetEscapeName(t.Name);
            }
            else if (o.GetType() == typeof(StoredProcedureParameter))
            {
                StoredProcedureParameter t = (StoredProcedureParameter)o;
                return GetEscapeName(t.Name.Substring(1));
            }
            else if (o.GetType() == typeof(ForeignKeyColumn))
            {
                ForeignKeyColumn t = (ForeignKeyColumn)o;
                return GetEscapeName(t.Name);
            }
            else if (o.GetType() == typeof(ForeignKey))
            {
                ForeignKey t = (ForeignKey)o;
                return GetEscapeName(t.Name);
            }
            else if (o.GetType() == typeof(DataType))
            {
                DataType t = (DataType)o;
                return (!string.IsNullOrEmpty(SchemaSplitter) ? (GetEscapeName(t.Schema) + SchemaSplitter) : "") + GetEscapeName(t.Name);
            }

            throw new Exception("δ�������������");
        }

        /// <summary>
        /// ȡת�������ƣ����������������������ȣ��ִ���ȥ�ո񣬹����࣬���Ա���еķǷ��ַ�������� C# ��������ͬ�����ֶ�����ǰ��� _��
        /// </summary>
        public static string GetEscapeName(this string s)
        {
            s = s.Trim();
            if (CheckIsKeywords(s)) return "_" + s;
            if (s[0] >= '0' && s[0] <= '9') s = "_" + s;
            return s.Replace(' ', '_')
                .Replace(',', '_')
                .Replace('.', '_')
                .Replace(';', '_')
                .Replace(':', '_')
                .Replace('~', '_')
                .Replace('(', '_')
                .Replace(')', '_')
                .Replace('#', '_')
                .Replace('\\', '_')
                .Replace('/', '_')
                .Replace('=', '_')
                .Replace('>', '_')
                .Replace('<', '_')
                .Replace('+', '_')
                .Replace('-', '_')
                .Replace('*', '_')
                .Replace('%', '_')
                .Replace('&', '_')
                .Replace('|', '_')
                .Replace('^', '_')
                .Replace('\'', '_')
                .Replace('"', '_')
                .Replace('[', '_')
                .Replace(']', '_')
                .Replace('!', '_')
                .Replace('@', '_')
                .Replace('$', '_');
        }

        /// <summary>
        /// ȡת����λ�� RowFilter �е��ֶ����ִ�����
        /// </summary>
        public static string GetEscapeRowFilterName(this string s)
        {
            if (s.Contains("~")
                || s.Contains("(")
                || s.Contains(")")
                || s.Contains("#")
                || s.Contains("\\")
                || s.Contains("/")
                || s.Contains("=")
                || s.Contains(">")
                || s.Contains("<")
                || s.Contains("+")
                || s.Contains("-")
                || s.Contains("*")
                || s.Contains("%")
                || s.Contains("&")
                || s.Contains("|")
                || s.Contains("^")
                || s.Contains("\'")
                || s.Contains("\"")
                || s.Contains("[")
                || s.Contains("]")
                || s.Contains(" ")
                || s.Contains(",")
                || s.Contains(".")
                || s.Contains(";")
                || s.Contains(":")
                )
            {
                return "[" + s.Replace("]", @"\]") + "]";
            }
            return s;
        }

        /// <summary>
        /// ȡת�������ݿ����������ʶ����
        /// </summary>
        public static string GetEscapeSqlObjectName(this string s)
        {
            return s.Replace("]", @"]]");
        }


        #endregion

        #region FormatString

        /// <summary>
        /// �����ո��һ���ִ�
        /// </summary>
        public const string Spaces = "                                                                                                                                                      ";

        /// <summary>
        /// �� s1 ��ʽ��Ϊ lenOfs1 �ֳ����ӿո�
        /// </summary>
        public static string FormatString(string s1, string s2, int lenOfs1)
        {
            int s1len = System.Text.Encoding.Default.GetByteCount(s1);
            if (s1len < lenOfs1) return s1 + Spaces.Substring(0, lenOfs1 - s1len) + s2;
            return s1 + " " + s2;
        }

        /// <summary>
        /// �� s1, s2 ��ʽ��Ϊ lenOfs1 lenOfs2 �ֳ����ӿո�
        /// </summary>
        public static string FormatString(string s1, string s2, string s3, int lenOfs1, int lenOfs2)
        {
            int s1len = System.Text.Encoding.Default.GetByteCount(s1);
            int s2len = System.Text.Encoding.Default.GetByteCount(s2);
            if (s1len < lenOfs1) s1 += Spaces.Substring(0, lenOfs1 - s1len);
            else s1 += " ";
            if (s2len < lenOfs2) s2 += Spaces.Substring(0, lenOfs2 - s2len);
            else s2 += " ";
            return s1 + s2 + s3;
        }

        #endregion


        #region DAL_GEN_Config


        /// <summary>
        /// ָ��ǰ���ݿ�� DAL ��������
        /// </summary>
        public static DS _CurrrentDALGenSetting = null;

        /// <summary>
        /// ָ��ǰ���ݿ�� DAL �������� ����� ��ǰ���÷���
        /// </summary>
        public static DS.SchemesRow _CurrrentDALGenSetting_CurrentScheme = null;


        /// <summary>
        /// Ϊ DS ʵ����� DAL �������� ��ʼ����
        /// </summary>
        public static void FillDatabaseDALGenSettingDS(this Database db, DS ds)
        {
            string s = Utils.GetDescription(db, "SPGenSettings_DALGen");
            if (!string.IsNullOrEmpty(s)) ds.ReadXml(new MemoryStream(Encoding.UTF8.GetBytes(s)));

            DS.TypeNamesRow t_Tables;
            DS.TypeNamesRow t_Views;
            DS.TypeNamesRow t_StoredProcedures;
            DS.TypeNamesRow t_Functions;

            // �ӵ�ǰ ds �в���Ĭ������
            t_Tables = ds.TypeNames.FindByTypeName("Tables");
            t_Views = ds.TypeNames.FindByTypeName("Views");
            t_StoredProcedures = ds.TypeNames.FindByTypeName("StoredProcedures");
            t_Functions = ds.TypeNames.FindByTypeName("Functions");

            // δ�ҵ������
            if (t_Tables == null) t_Tables = ds.TypeNames.AddTypeNamesRow("Tables");
            if (t_Views == null) t_Views = ds.TypeNames.AddTypeNamesRow("Views");
            if (t_StoredProcedures == null) t_StoredProcedures = ds.TypeNames.AddTypeNamesRow("StoredProcedures");
            if (t_Functions == null) t_Functions = ds.TypeNames.AddTypeNamesRow("Functions");

            DS.SchemesRow s_Default;
            s_Default = ds.Schemes.FindBySchemesID(1);
            if (s_Default == null)
            {
                string ns = Utils.GetDescription(db, "SPGenSettings_DefaultNamespace");
                if (string.IsNullOrEmpty(ns)) ns = "DAL";

                bool isGenWCFAttribute = false;
                s = Utils.GetDescription(db, "SPGenSettings_IsGenWCFAttribute");
                if (!string.IsNullOrEmpty(s)) isGenWCFAttribute = bool.Parse(s);

                bool isGenSchemaSupport = false;
                s = Utils.GetDescription(db, "SPGenSettings_SchemaHandleMode");
                if (s == "_") isGenSchemaSupport = true;

                s_Default = ds.Schemes.AddSchemesRow(ns, "Ĭ�ϵ����ɷ���", isGenSchemaSupport, isGenWCFAttribute, false, true, false, false, false, false, true, true, true, true, false, 1, "Ĭ��");

                s = Utils.GetDescription(db, "SPGenSettings_Filters");
                if (string.IsNullOrEmpty(s))
                {
                    ds.SchemesFilters.AddSchemesFiltersRow(t_Tables, true, ".", s_Default, "Ĭ�����������û���", "");
                    ds.SchemesFilters.AddSchemesFiltersRow(t_Views, true, ".", s_Default, "Ĭ�����������û���ͼ", "");
                    ds.SchemesFilters.AddSchemesFiltersRow(t_StoredProcedures, true, ".", s_Default, "Ĭ�����������û��洢����", "");
                    ds.SchemesFilters.AddSchemesFiltersRow(t_Functions, true, ".", s_Default, "Ĭ�����������û�����", "");

                    ds.SchemesFilters.AddSchemesFiltersRow(t_Tables, false, "^aspnet_.", s_Default, "Ĭ������ membership ��ر�", "dbo");
                    ds.SchemesFilters.AddSchemesFiltersRow(t_Views, false, "^vw_aspnet_.", s_Default, "Ĭ������ membership �����ͼ", "dbo");
                    ds.SchemesFilters.AddSchemesFiltersRow(t_StoredProcedures, false, "^aspnet_.", s_Default, "Ĭ������ membership ��ع���", "dbo");
                }
                else
                {
                    try
                    {
                        ds.Filters.ReadXml(new MemoryStream(Encoding.UTF8.GetBytes(s)));
                    }
                    catch { }
                    foreach (DS.FiltersRow r in ds.Filters)
                    {
                        ds.SchemesFilters.AddSchemesFiltersRow(r.TypeNamesRow, r.IsAllow, r.FilterString, s_Default, r.Memo, "");
                    }
                    ds.Filters.Clear();
                }
            }

            ds.AcceptChanges();
        }

        /// <summary>
        /// ���� DAL �������õ���ǰ������
        /// </summary>
        public static void LoadDatabaseDALGenSettingDS(this Database db)
        {
            _CurrrentDALGenSetting = new DS();
            FillDatabaseDALGenSettingDS(db, _CurrrentDALGenSetting);
            _CurrrentDALGenSetting_CurrentScheme = _CurrrentDALGenSetting.Schemes.FindBySchemesID(1);
            SchemaSplitter = _CurrrentDALGenSetting_CurrentScheme.IsSupportSchema ? "_" : null;
        }
        /// <summary>
        /// ���� DAL �������õ���ǰ������ ָ�����÷������
        /// </summary>
        public static void LoadDatabaseDALGenSettingDS(this Database db, int schemeID)
        {
            _CurrrentDALGenSetting = new DS();
            FillDatabaseDALGenSettingDS(db, _CurrrentDALGenSetting);
            _CurrrentDALGenSetting_CurrentScheme = _CurrrentDALGenSetting.Schemes.FindBySchemesID(schemeID);
            SchemaSplitter = _CurrrentDALGenSetting_CurrentScheme.IsSupportSchema ? "_" : null;
        }
        /// <summary>
        /// ����ǰ�������е� DAL �������ñ��浽���ݿ�
        /// </summary>
        public static void SaveDatabaseDALGenSettingDS(this Database db)
        {
            StringBuilder sb = new StringBuilder();
            StringWriter sw = new StringWriter(sb);
            _CurrrentDALGenSetting.AcceptChanges();
            _CurrrentDALGenSetting.WriteXml(sw);
            Utils.SetDescription(db, "SPGenSettings_DALGen", sb.ToString());
        }
        /// <summary>
        /// ��ָ���� DAL �������ñ��浽���ݿ�
        /// </summary>
        public static void SaveDatabaseDALGenSettingDS(this Database db, DS ds)
        {
            StringBuilder sb = new StringBuilder();
            StringWriter sw = new StringWriter(sb);
            ds.AcceptChanges();
            ds.WriteXml(sw);
            Utils.SetDescription(db, "SPGenSettings_DALGen", sb.ToString());
        }



        #endregion

        #region Get Database Objects ( User's Tables, Functions, StoredProcedures, Columns )

        /// <summary>
        /// ��������� ����� �ֵ䣨���û�з��� 0 �����ֵ䣩
        /// </summary>
        public static Dictionary<Column, Column> GetTreePKFKColumns(this Table t)
        {
            Dictionary<Column, Column> ccs = new Dictionary<Column, Column>();
            foreach (ForeignKey fk in t.ForeignKeys)
            {
                if (fk.ReferencedTable != t.Name || fk.ReferencedTableSchema != t.Schema) continue;
                int equaled = 0;
                foreach (ForeignKeyColumn fkc in fk.Columns)		//�ж��Ƿ�һ�����Լ�������ֶζ����ڵ�ǰ��
                {
                    if (fkc.Parent.Parent == t) equaled++;
                }
                if (equaled == fk.Columns.Count)					//��ǰ��Ϊ����
                {
                    for (int i = 0; i < fk.Columns.Count; i++)
                    {
                        ForeignKeyColumn fkc = fk.Columns[i];
                        Column f = t.Columns[fkc.Name];
                        Column p = t.Columns[fkc.ReferencedColumn];
                        ccs.Add(p, f);
                    }
                    return ccs;
                }
            }
            return ccs;
        }

        /// <summary>
        /// ����һ������������ϣ����û�з��� 0 �����б�
        /// </summary>
        public static List<Column> GetPrimaryKeyColumns(this Table t)
        {
            List<Column> cs = new List<Column>();
            foreach (Column c in t.Columns)
            {
                if (c.InPrimaryKey) cs.Add(c);
            }
            return cs;
        }


        /// <summary>
        /// ����һ����ķ��������ϣ����û�з��� 0 �����б�
        /// </summary>
        public static List<Column> GetNonPrimaryKeyColumns(this Table t)
        {
            List<Column> cs = new List<Column>();
            foreach (Column c in t.Columns)
            {
                if (!c.InPrimaryKey) cs.Add(c);
            }
            return cs;
        }

        /// <summary>
        /// ����һ������������ֶΣ����û�з��ؿգ�
        /// </summary>
        public static Column GetIdentityColumn(this Table t)
        {
            foreach (Column c in t.Columns)
            {
                if (c.Identity) return c;
            }
            return null;
        }

        /// <summary>
        /// ����һ����ͼ�ķ��������ϣ����û�з��� 0 �����б�
        /// </summary>
        public static List<Column> GetNonPrimaryKeyColumns(this View v)
        {
            List<Column> pkcs = GetPrimaryKeyColumns(v);
            List<Column> cs = new List<Column>();
            foreach (Column c in v.Columns)
            {
                if (!pkcs.Contains(c)) cs.Add(c);
            }
            return cs;
        }

        /// <summary>
        /// ����һ����ֵ�������������ϣ����û�з��� 0 �����б�
        /// </summary>
        public static List<Column> GetPrimaryKeyColumns(this UserDefinedFunction f)
        {
            List<Column> cs = new List<Column>();
            foreach (Column c in f.Columns)
            {
                if (c.InPrimaryKey) cs.Add(c);
            }
            return cs;
        }

        /// <summary>
        /// ����һ����ֵ�����ķ��������ϣ����û�з��� 0 �����б�
        /// </summary>
        public static List<Column> GetNonPrimaryKeyColumns(this UserDefinedFunction f)
        {
            List<Column> cs = new List<Column>();
            foreach (Column c in f.Columns)
            {
                if (!c.InPrimaryKey) cs.Add(c);
            }
            return cs;
        }


        /// <summary>
        /// ����һ���û��Զ�������͵��������ϣ����û�з��� 0 �����б�
        /// </summary>
        public static List<Column> GetPrimaryKeyColumns(this UserDefinedTableType t)
        {
            List<Column> cs = new List<Column>();
            foreach (Column c in t.Columns)
            {
                if (c.InPrimaryKey) cs.Add(c);
            }
            return cs;
        }

        /// <summary>
        /// ����һ���û��Զ�������͵ķ��������ϣ����û�з��� 0 �����б�
        /// </summary>
        public static List<Column> GetNonPrimaryKeyColumns(this UserDefinedTableType t)
        {
            List<Column> cs = new List<Column>();
            foreach (Column c in t.Columns)
            {
                if (!c.InPrimaryKey) cs.Add(c);
            }
            return cs;
        }


        /// <summary>
        /// ����һ�����еĿ�д�ֶμ��� ���ų������У������У�Timestamp �У������û�з��� 0 �����б�
        /// </summary>
        public static List<Column> GetWriteableColumns(this Table t)
        {
            List<Column> cs = new List<Column>();
            foreach (Column c in t.Columns)
            {
                if (c.Computed || c.Identity || c.DataType.SqlDataType == SqlDataType.Timestamp) continue;		// || c.RowGuidCol
                cs.Add(c);
            }
            return cs;
        }


        /// <summary>
        /// ����һ����ͼ�еĿ�д�ֶμ��� ���жϻ���Ŀ�д����Ӷ����أ�������û����Ϊ��д�������û�з��� 0 �����б�
        /// </summary>
        public static List<Column> GetWriteableColumns(this View v)
        {
            Table t = GetBaseTable(v);
            if (t != null)
            {
                List<Column> cs = new List<Column>();
                foreach (Column c in t.Columns)
                {
                    if (c.Computed || c.Identity || c.DataType.SqlDataType == SqlDataType.Timestamp) continue;		// || c.RowGuidCol
                    cs.Add(v.Columns[c.Name]);
                }
                foreach (Column c in v.Columns)
                {
                    if (!cs.Exists(new Predicate<Column>(delegate(Column o) { return o.Name == c.Name; })))
                    {
                        cs.Add(c);
                    }
                }
                return cs;
            }
            else
            {
                List<Column> cs = new List<Column>();
                foreach (Column c in v.Columns)
                {
                    cs.Add(c);
                }
                return cs;
            }
        }

        /// <summary>
        /// ����һ����ֵ�����еĿ�д�ֶμ��� ���ݶ������п�д ���� Timestamp �������û�з��� 0 �����б�
        /// </summary>
        public static List<Column> GetWriteableColumns(this UserDefinedFunction t)
        {
            List<Column> cs = new List<Column>();
            foreach (Column c in t.Columns)
            {
                if (c.DataType.SqlDataType == SqlDataType.Timestamp) continue;
                cs.Add(c);
            }
            return cs;
        }


        /// <summary>
        /// ����һ���û��Զ���������еĿ�д�ֶμ��� ���ų������У������У�Timestamp�������û�з��� 0 �����б�
        /// </summary>
        public static List<Column> GetWriteableColumns(this UserDefinedTableType t)
        {
            List<Column> cs = new List<Column>();
            foreach (Column c in t.Columns)
            {
                if (c.Computed || c.Identity || c.DataType.SqlDataType == SqlDataType.Timestamp) continue;		// || c.RowGuidCol
                cs.Add(c);
            }
            return cs;
        }

        /// <summary>
        /// ����һ�����еı�д�ֶμ��ϣ����û�з��� 0 �����б�
        /// </summary>
        public static List<Column> GetMustWriteColumns(this Table t)
        {
            List<Column> cs = new List<Column>();
            foreach (Column c in t.Columns)
            {
                if (c.Identity || c.Computed || c.Nullable || c.DefaultConstraint != null) continue;
                cs.Add(c);
            }
            return cs;
        }

        /// <summary>
        /// ����һ���û��Զ���������еı�д�ֶμ��ϣ����û�з��� 0 �����б�
        /// </summary>
        public static List<Column> GetMustWriteColumns(this UserDefinedTableType t)
        {
            List<Column> cs = new List<Column>();
            foreach (Column c in t.Columns)
            {
                if (c.Identity || c.Computed || c.Nullable || c.DefaultConstraint != null) continue;
                cs.Add(c);
            }
            return cs;
        }


        /// <summary>
        /// ����һ�����еĿ������ֶμ��� ���ų������ƣ�ͼƬ���ı��������У������û�з��� 0 �����б�
        /// </summary>
        public static List<Column> GetSortableColumns(this Table t)
        {
            List<Column> cs = new List<Column>();
            foreach (Column c in t.Columns)
            {
                if (Utils.CheckIsBinaryType(c)) continue;
                cs.Add(c);
            }
            return cs;
        }

        /// <summary>
        /// ����һ����������еĿ������ֶμ��� ���ų������ƣ�ͼƬ���ı��������У������û�з��� 0 �����б�
        /// </summary>
        public static List<Column> GetSortableColumns(this UserDefinedFunction t)
        {
            List<Column> cs = new List<Column>();
            foreach (Column c in t.Columns)
            {
                if (Utils.CheckIsBinaryType(c)) continue;
                cs.Add(c);
            }
            return cs;
        }


        /// <summary>
        /// ����һ����ͼ�еĿ������ֶμ��� ���ų������ƣ�ͼƬ���ı��������У������û�з��� 0 �����б�
        /// </summary>
        public static List<Column> GetSortableColumns(this View v)
        {
            List<Column> cs = new List<Column>();
            foreach (Column c in v.Columns)
            {
                if (Utils.CheckIsBinaryType(c)) continue;
                cs.Add(c);
            }
            return cs;
        }


        /// <summary>
        /// ����һ���û��Զ�������ͽ���еĿ������ֶμ��� ���ų������ƣ�ͼƬ���ı��������У������û�з��� 0 �����б�
        /// </summary>
        public static List<Column> GetSortableColumns(this UserDefinedTableType t)
        {
            List<Column> cs = new List<Column>();
            foreach (Column c in t.Columns)
            {
                if (Utils.CheckIsBinaryType(c)) continue;
                cs.Add(c);
            }
            return cs;
        }


        /// <summary>
        /// ����һ�����еĿ�ģ����ѯ�ֶμ��� ���ִ��ࣩ�����û�з��� 0 �����б�
        /// </summary>
        public static List<Column> GetSearchableColumns(this Table t)
        {
            List<Column> cs = new List<Column>();
            foreach (Column c in t.Columns)
            {
                if (CheckIsStringType(c))
                {
                    cs.Add(c);
                }
            }
            return cs;
        }
        /// <summary>
        /// ����һ����ֵ��������еĿ�ģ����ѯ�ֶμ��� ���ִ��ࣩ�����û�з��� 0 �����б�
        /// </summary>
        public static List<Column> GetSearchableColumns(this UserDefinedFunction t)
        {
            List<Column> cs = new List<Column>();
            foreach (Column c in t.Columns)
            {
                if (CheckIsStringType(c))
                {
                    cs.Add(c);
                }
            }
            return cs;
        }

        /// <summary>
        /// ����һ����ͼ�еĿ�ģ����ѯ�ֶμ��� ���ִ��ࣩ�����û�з��� 0 �����б�
        /// </summary>
        public static List<Column> GetSearchableColumns(this View v)
        {
            List<Column> cs = new List<Column>();
            foreach (Column c in v.Columns)
            {
                if (CheckIsStringType(c))
                {
                    cs.Add(c);
                }
            }
            return cs;
        }

        /// <summary>
        /// ����һ�� �û��Զ�������� �еĿ�ģ����ѯ�ֶμ��� ���ִ��ࣩ�����û�з��� 0 �����б�
        /// </summary>
        public static List<Column> GetSearchableColumns(this UserDefinedTableType v)
        {
            List<Column> cs = new List<Column>();
            foreach (Column c in v.Columns)
            {
                if (CheckIsStringType(c))
                {
                    cs.Add(c);
                }
            }
            return cs;
        }













        /// <summary>
        /// �����û������û�з��� 0 �����б�
        /// </summary>
        public static List<Table> GetUserTables(this Database db, DS ds)
        {
            DS.SchemesFiltersRow[] rs = (DS.SchemesFiltersRow[])ds.SchemesFilters.Select("TypeName = 'Tables' AND SchemesID = " + _CurrrentDALGenSetting_CurrentScheme.SchemesID.ToString(), "SortOrder", DataViewRowState.CurrentRows);

            List<Table> os = new List<Table>();
            foreach (Table o in db.Tables)
            {
                if (o.IsSystemObject)			// �����ϵͳ����, ��ȥ�����Ƿ������ȫƥ��Ĺ�����Ϣ
                {
                    bool b = true;
                    foreach (DS.SchemesFiltersRow r in rs)
                    {
                        if (("^" + o.Name + "$").Equals(r.FilterString, StringComparison.CurrentCultureIgnoreCase) && o.Schema.Equals(r.Schema, StringComparison.CurrentCultureIgnoreCase) && r.IsAllow)
                        {
                            b = false;
                            break;
                        }
                    }
                    if (b) continue;
                }

                foreach (DS.SchemesFiltersRow r in rs)
                {
                    try
                    {
                        if (Regex.IsMatch(o.Name, r.FilterString, RegexOptions.IgnoreCase) && (r.Schema == "" || o.Schema.Equals(r.Schema, StringComparison.CurrentCultureIgnoreCase)))
                        {
                            if (r.IsAllow)
                            {
                                if (!os.Contains(o)) os.Add(o);
                            }
                            else if (os.Contains(o)) os.Remove(o);
                        }
                    }
                    catch { }
                }
            }
            return os;
        }

        /// <summary>
        /// �����û������û�з��� 0 �����б�
        /// </summary>
        public static List<Table> GetUserTables(this Database db)
        {
            if (_CurrrentDALGenSetting == null) LoadDatabaseDALGenSettingDS(db);
            return GetUserTables(db, _CurrrentDALGenSetting);
        }

        /// <summary>
        /// �����û����̣����û�з��� 0 �����б�
        /// </summary>
        public static List<StoredProcedure> GetUserStoredProcedures(this Database db, DS ds)
        {
            DS.SchemesFiltersRow[] rs = (DS.SchemesFiltersRow[])ds.SchemesFilters.Select("TypeName = 'StoredProcedures' AND SchemesID = " + _CurrrentDALGenSetting_CurrentScheme.SchemesID.ToString(), "SortOrder", DataViewRowState.CurrentRows);

            List<StoredProcedure> os = new List<StoredProcedure>();
            foreach (StoredProcedure o in db.StoredProcedures)
            {
                if (o.IsSystemObject)			// �����ϵͳ����, ��ȥ�����Ƿ������ȫƥ��Ĺ�����Ϣ
                {
                    bool b = true;
                    foreach (DS.SchemesFiltersRow r in rs)
                    {
                        if (("^" + o.Name + "$").Equals(r.FilterString, StringComparison.CurrentCultureIgnoreCase) && o.Schema.Equals(r.Schema, StringComparison.CurrentCultureIgnoreCase) && r.IsAllow)
                        {
                            b = false;
                            break;
                        }
                    }
                    if (b) continue;
                }


                foreach (DS.SchemesFiltersRow r in rs)
                {
                    try
                    {
                        if (Regex.IsMatch(o.Name, r.FilterString, RegexOptions.IgnoreCase) && (r.Schema == "" || o.Schema.Equals(r.Schema, StringComparison.CurrentCultureIgnoreCase)))
                        {
                            if (r.IsAllow)
                            {
                                if (!os.Contains(o)) os.Add(o);
                            }
                            else if (os.Contains(o)) os.Remove(o);
                        }
                    }
                    catch { }
                }
            }
            return os;
        }


        /// <summary>
        /// �����û����̣����û�з��� 0 �����б�
        /// </summary>
        public static List<StoredProcedure> GetUserStoredProcedures(this Database db)
        {
            if (_CurrrentDALGenSetting == null) LoadDatabaseDALGenSettingDS(db);
            return GetUserStoredProcedures(db, _CurrrentDALGenSetting);
        }

        /// <summary>
        /// �����û����������û�з��� 0 �����б�
        /// </summary>
        public static List<UserDefinedFunction> GetUserFunctions(this Database db, DS ds)
        {
            DS.SchemesFiltersRow[] rs = (DS.SchemesFiltersRow[])ds.SchemesFilters.Select("TypeName = 'Functions' AND SchemesID = " + _CurrrentDALGenSetting_CurrentScheme.SchemesID.ToString(), "SortOrder", DataViewRowState.CurrentRows);

            List<UserDefinedFunction> os = new List<UserDefinedFunction>();
            foreach (UserDefinedFunction o in db.UserDefinedFunctions)
            {
                if (o.IsSystemObject)			// �����ϵͳ����, ��ȥ�����Ƿ������ȫƥ��Ĺ�����Ϣ
                {
                    bool b = true;
                    foreach (DS.SchemesFiltersRow r in rs)
                    {
                        if (("^" + o.Name + "$").Equals(r.FilterString, StringComparison.CurrentCultureIgnoreCase) && o.Schema.Equals(r.Schema, StringComparison.CurrentCultureIgnoreCase) && r.IsAllow)
                        {
                            b = false;
                            break;
                        }
                    }
                    if (b) continue;
                }


                foreach (DS.SchemesFiltersRow r in rs)
                {
                    try
                    {
                        if (Regex.IsMatch(o.Name, r.FilterString, RegexOptions.IgnoreCase) && (r.Schema == "" || o.Schema.Equals(r.Schema, StringComparison.CurrentCultureIgnoreCase)))
                        {
                            if (r.IsAllow)
                            {
                                if (!os.Contains(o)) os.Add(o);
                            }
                            else if (os.Contains(o)) os.Remove(o);
                        }
                    }
                    catch { }
                }
            }
            return os;
        }

        /// <summary>
        /// �����û����������û�з��� 0 �����б�
        /// </summary>
        public static List<UserDefinedFunction> GetUserFunctions(this Database db)
        {
            if (_CurrrentDALGenSetting == null) LoadDatabaseDALGenSettingDS(db);
            return GetUserFunctions(db, _CurrrentDALGenSetting);
        }

        /// <summary>
        /// �����û���������ֵ�������û�з��� 0 �����б�
        /// </summary>
        public static List<UserDefinedFunction> GetUserFunctions_TableValue(this Database db)
        {
            List<UserDefinedFunction> os = new List<UserDefinedFunction>();
            foreach (UserDefinedFunction o in GetUserFunctions(db))
            {
                if (o.FunctionType == UserDefinedFunctionType.Table || o.FunctionType == UserDefinedFunctionType.Inline) os.Add(o);
            }
            return os;
        }

        /// <summary>
        /// �����û���������ֵ�������û�з��� 0 �����б�
        /// </summary>
        public static List<UserDefinedFunction> GetUserFunctions_ScalarValue(this Database db)
        {
            List<UserDefinedFunction> os = new List<UserDefinedFunction>();
            foreach (UserDefinedFunction o in GetUserFunctions(db))
            {
                if (o.FunctionType == UserDefinedFunctionType.Scalar) os.Add(o);
            }
            return os;
        }


        /// <summary>
        /// �����û���ͼ�����û�з��� 0 �����б�
        /// </summary>
        public static List<View> GetUserViews(this Database db, DS ds)
        {
            DS.SchemesFiltersRow[] rs = (DS.SchemesFiltersRow[])ds.SchemesFilters.Select("TypeName = 'Views' AND SchemesID = " + _CurrrentDALGenSetting_CurrentScheme.SchemesID.ToString(), "SortOrder", DataViewRowState.CurrentRows);

            List<View> os = new List<View>();
            foreach (View o in db.Views)
            {
                if (o.IsSystemObject)			// �����ϵͳ����, ��ȥ�����Ƿ������ȫƥ��Ĺ�����Ϣ
                {
                    bool b = true;
                    foreach (DS.SchemesFiltersRow r in rs)
                    {
                        if (("^" + o.Name + "$").Equals(r.FilterString, StringComparison.CurrentCultureIgnoreCase) && o.Schema.Equals(r.Schema, StringComparison.CurrentCultureIgnoreCase) && r.IsAllow)
                        {
                            b = false;
                            break;
                        }
                    }
                    if (b) continue;
                }


                foreach (DS.SchemesFiltersRow r in rs)
                {
                    try
                    {
                        if (Regex.IsMatch(o.Name, r.FilterString, RegexOptions.IgnoreCase) && (r.Schema == "" || o.Schema.Equals(r.Schema, StringComparison.CurrentCultureIgnoreCase)))
                        {
                            if (r.IsAllow)
                            {
                                if (!os.Contains(o)) os.Add(o);
                            }
                            else if (os.Contains(o)) os.Remove(o);
                        }
                    }
                    catch { }
                }
            }
            return os;
        }

        /// <summary>
        /// �����û���ͼ�����û�з��� 0 �����б�
        /// </summary>
        public static List<View> GetUserViews(this Database db)
        {
            if (_CurrrentDALGenSetting == null) LoadDatabaseDALGenSettingDS(db);
            return GetUserViews(db, _CurrrentDALGenSetting);
        }

        /// <summary>
        /// �����û���������ͣ����û�з��� 0 �����б�
        /// </summary>
        public static List<UserDefinedTableType> GetUserDefinedTableTypes(this Database db)
        {
            List<UserDefinedTableType> os = new List<UserDefinedTableType>();
            if (db.CompatibilityLevel >= CompatibilityLevel.Version100)
            {
                foreach (UserDefinedTableType o in db.UserDefinedTableTypes)
                {
                    if (o.IsUserDefined) os.Add(o);
                }
            }
            return os;
        }


        /// <summary>
        /// �����û��������ݣ����������ͣ����û�з��� 0 �����б�
        /// </summary>
        public static List<UserDefinedDataType> GetUserDefinedDataTypes(this Database db)
        {
            List<UserDefinedDataType> os = new List<UserDefinedDataType>();
            foreach (UserDefinedDataType o in db.UserDefinedDataTypes)
            {
                os.Add(o);
            }
            return os;
        }

        /// <summary>
        /// �����û�����ܹ��б����û�з��� 0 �����б�
        /// </summary>
        public static List<Schema> GetUserSchemas(this Database db)
        {
            List<Schema> ss = new List<Schema>();
            foreach (Schema s in db.Schemas)
            {
                if (s.Name == "db_accessadmin"
                    || s.Name == "db_backupoperator"
                    || s.Name == "db_datareader"
                    || s.Name == "db_datawriter"
                    || s.Name == "db_ddladmin"
                    || s.Name == "db_denydatareader"
                    || s.Name == "db_denydatawriter"
                    || s.Name == "db_owner"
                    || s.Name == "db_securityadmin"
                    || s.Name == "dbo"
                    || s.Name == "guest"
                    || s.Name == "INFORMATION_SCHEMA"
                    || s.Name == "sys")
                    continue;
                ss.Add(s);
            }
            return ss;
        }


        /// <summary>
        /// ���ر�����ʺ�����ʾ���ֶΣ����ȼ�����һ���޳��ֶΣ���һ�����޳��ֶΣ���һ�������ֶΣ�
        /// todo: ��������
        /// </summary>
        public static Column GetDisplayColumn(this Table t)
        {
            foreach (Column c in t.Columns)
            {
                if (CheckIsStringType(c) && (c.DataType.MaximumLength > 0 && c.DataType.MaximumLength < 4000)) return c;
            }
            foreach (Column c in t.Columns)
            {
                if (CheckIsStringType(c)) return c;
            }
            foreach (Column c in t.Columns)
            {
                if (c.InPrimaryKey) return c;
            }

            return t.Columns[0];
        }



        /// <summary>
        /// ����һ�����ݶ������չ���Լ���
        /// </summary>
        public static List<ExtendedProperty> GetExtendedProperties(this Database db)
        {
            List<ExtendedProperty> result = new List<ExtendedProperty>();
            foreach (ExtendedProperty ep in db.ExtendedProperties) result.Add(ep);
            return result;
        }

        /// <summary>
        /// ����һ�����ݶ������չ���Լ���
        /// </summary>
        public static List<ExtendedProperty> GetExtendedProperties(this Table t)
        {
            List<ExtendedProperty> result = new List<ExtendedProperty>();
            foreach (ExtendedProperty ep in t.ExtendedProperties) result.Add(ep);
            return result;
        }

        #endregion


    }
}
