Imports System
Imports System.Web
Imports System.Web.UI
Imports System.Web.UI.WebControls
Imports System.Data
Imports System.Data.SqlClient
Imports System.Configuration
Imports Microsoft.VisualBasic

Public Class db
    Public Const WILDCARD_ID As Integer = -1
    Dim x As String
    Private m_DataTableName As String
    Private m_DatasetName As String
    Private m_SQLText As String
    Private m_SQLErrorMess As String
    Private m_SQLStatus As Boolean
    Private m_dreader As System.Data.SqlClient.SqlDataReader
    Private m_cnn As SqlConnection
    Private m_dsData As New DataSet
    Private m_dsRowCount, m_RowsEffected As Integer
    Private m_ScalarData As String

    Private _ExceptionMessage,
            _ExceptionSource,
            _ExceptionStackTrace As String

    ' Dim cnn As SqlConnection
    ReadOnly Property cnn() As SqlConnection
        Get
            Return m_cnn
        End Get
    End Property
    Property DataTableName() As String
        Get
            Return m_DataTableName
        End Get
        Set(ByVal Value As String)
            m_DataTableName = Value
        End Set
    End Property
    Property DatasetName() As String
        Get
            Return m_DatasetName
        End Get
        Set(ByVal Value As String)
            m_DatasetName = Value
        End Set
    End Property
    Property SQLText() As String
        Get
            Return m_SQLText
        End Get
        Set(ByVal Value As String)
            m_SQLText = Value
        End Set
    End Property

    ReadOnly Property ScalarData() As String
        Get
            Return m_ScalarData
        End Get
    End Property

    ReadOnly Property RowsEffected() As Integer
        Get
            Return m_RowsEffected
        End Get
    End Property

    ReadOnly Property SQLStatus() As Boolean
        Get
            Return m_SQLStatus
        End Get
    End Property
    ReadOnly Property SQLErrorMess() As String
        Get
            Return m_SQLErrorMess
        End Get
    End Property

    ReadOnly Property ExceptionMessage() As String
        Get
            Return _ExceptionMessage
        End Get
    End Property

    ReadOnly Property ExceptionSource() As String
        Get
            Return _ExceptionSource
        End Get
    End Property

    ReadOnly Property ExceptionStackTrace() As String
        Get
            Return _ExceptionStackTrace
        End Get
    End Property

    ReadOnly Property dr() As System.Data.SqlClient.SqlDataReader
        Get
            Return m_dreader
        End Get
    End Property
    ReadOnly Property ds() As DataSet
        Get
            Return m_dsData
        End Get
    End Property
    ReadOnly Property dsRowCount() As Integer
        Get
            Return m_dsData.Tables(0).Rows.Count
        End Get
    End Property

    Public Sub GetDR(Optional ByVal strConnection As String = "")

        Dim sqlCmd As SqlCommand

        If strConnection <> "" Then
            strConnection = strConnection
        Else
            strConnection = System.Configuration.ConfigurationManager.AppSettings("ConnectionString")
        End If

        Try
            m_cnn = New SqlConnection(strConnection)
            sqlCmd = New SqlCommand(m_SQLText, m_cnn)
            sqlCmd.CommandTimeout = 240
            m_cnn.Open()
            m_SQLErrorMess = "OK"
            m_SQLStatus = True

            m_dreader = sqlCmd.ExecuteReader(CommandBehavior.CloseConnection)
        Catch ex As Exception
            m_SQLErrorMess = m_SQLText & "<br><br>" & ex.Message & "<br><br>"
            m_SQLErrorMess = m_SQLErrorMess & "Source: " & ex.Source & "<br><br>"
            m_SQLErrorMess = m_SQLErrorMess & "Stack Trace: " & ex.StackTrace & "<br><br>"

            _ExceptionMessage = ex.Message
            _ExceptionSource = ex.Source
            _ExceptionStackTrace = ex.StackTrace

            m_SQLStatus = False
        End Try

    End Sub

    Public Sub GetScalar()
        Dim sqlCmd As SqlCommand
        Dim cnn As SqlConnection
        Dim strConnection As String = System.Configuration.ConfigurationManager.AppSettings("ConnectionString")

        cnn = New SqlConnection(strConnection)
        sqlCmd = New SqlCommand(m_SQLText & "; SELECT SCOPE_IDENTITY();", cnn)
        sqlCmd.CommandTimeout = 120
        cnn.Open()
        m_SQLErrorMess = "OK"
        m_SQLStatus = True

        Try
            Dim returnValue As Object
            returnValue = sqlCmd.ExecuteScalar()

            If returnValue Is Nothing OrElse IsDBNull(returnValue) Then
                m_ScalarData = "0"
            Else
                m_ScalarData = returnValue
            End If
        Catch ex As Exception
            m_SQLErrorMess = m_SQLText & "<br><br>" & ex.Message & "<br><br>"
            m_SQLErrorMess = m_SQLErrorMess & "Source: " & ex.Source & "<br><br>"
            m_SQLErrorMess = m_SQLErrorMess & "Stack Trace: " & ex.StackTrace & "<br><br>"

            _ExceptionMessage = ex.Message
            _ExceptionSource = ex.Source
            _ExceptionStackTrace = ex.StackTrace

            m_SQLStatus = False
        End Try
    End Sub

    Public Sub GetDS()

        Dim MyDataAdapter As New SqlDataAdapter
        Dim sqlCmd As SqlCommand
        '  Dim cnn As SqlConnection
        Dim DataTableName As String = ""
        Dim strConnection As String = System.Configuration.ConfigurationManager.AppSettings("ConnectionString")

        m_cnn = New SqlConnection(strConnection)
        sqlCmd = New SqlCommand(m_SQLText, m_cnn)
        sqlCmd.CommandTimeout = 2400
        m_cnn.Open()
        m_SQLErrorMess = "OK"
        m_SQLStatus = True
        MyDataAdapter = New SqlDataAdapter(sqlCmd)

        'If m_DatasetName Is Nothing Then
        '    m_DatasetName = "m_Dataset"
        'Else
        '    m_DatasetName = m_DataTableName
        'End If

        If m_DataTableName Is Nothing Then
            DataTableName = "Data"
        Else
            DataTableName = m_DataTableName
        End If

        Try
            MyDataAdapter.Fill(m_dsData, DataTableName)
        Catch ex As Exception
            m_SQLErrorMess = m_SQLText & "<br><br>" & ex.Message & "<br><br>"
            m_SQLErrorMess = m_SQLErrorMess & "Source: " & ex.Source & "<br><br>"
            m_SQLErrorMess = m_SQLErrorMess & "Stack Trace: " & ex.StackTrace & "<br><br>"

            _ExceptionMessage = ex.Message
            _ExceptionSource = ex.Source
            _ExceptionStackTrace = ex.StackTrace

            m_SQLStatus = False
        Finally
            If Not m_cnn Is Nothing Then
                m_cnn.Close()
            End If
        End Try

        If m_cnn.State.Open Then
            m_cnn.Close()
        End If

    End Sub

    Public Sub ExecuteQuery()
        Dim sqlConn As SqlConnection
        Dim strConnection As String = System.Configuration.ConfigurationManager.AppSettings("ConnectionString")

        sqlConn = New SqlConnection(strConnection)

        sqlConn.Open()

        Dim cmd As New SqlCommand(m_SQLText, sqlConn)
        cmd.CommandTimeout = 240
        m_SQLErrorMess = "OK"
        m_SQLStatus = True
        Try
            m_RowsEffected = cmd.ExecuteNonQuery()
        Catch ex As Exception
            m_SQLErrorMess = m_SQLText & "<br><br>" & ex.Message & "<br><br>"
            m_SQLErrorMess = m_SQLErrorMess & "Source: " & ex.Source & "<br><br>"
            m_SQLErrorMess = m_SQLErrorMess & "Stack Trace: " & ex.StackTrace & "<br><br>"

            _ExceptionMessage = ex.Message
            _ExceptionSource = ex.Source
            _ExceptionStackTrace = ex.StackTrace

            m_SQLStatus = False
        Finally
            sqlConn.Close()
        End Try

        If sqlConn.State.Open Then
            sqlConn.Close()
        End If

    End Sub

    Public Sub GetDRHelp()

        Dim sqlCmd As SqlCommand
        Dim cnn As SqlConnection
        Dim strConnection As String = System.Configuration.ConfigurationManager.AppSettings("ConnectionString")
        ' Dim strConnection As String = "Data Source=96.30.242.72\p4r2;Network Library=DBMSSOCN;Initial Catalog=j5c_1click_global;User ID=remote;Password=r3m0t3;"
        cnn = New SqlConnection(strConnection)
        sqlCmd = New SqlCommand(m_SQLText, cnn)
        sqlCmd.CommandTimeout = 120
        cnn.Open()
        m_SQLErrorMess = "OK"
        m_SQLStatus = True

        Try
            ' m_dreader = sqlCmd.ExecuteReader()
            m_dreader = sqlCmd.ExecuteReader(CommandBehavior.CloseConnection)
        Catch ex As Exception
            m_SQLErrorMess = m_SQLText & "<br><br>" & ex.Message & "<br><br>"
            m_SQLErrorMess = m_SQLErrorMess & "Source: " & ex.Source & "<br><br>"
            m_SQLErrorMess = m_SQLErrorMess & "Stack Trace: " & ex.StackTrace & "<br><br>"

            _ExceptionMessage = ex.Message
            _ExceptionSource = ex.Source
            _ExceptionStackTrace = ex.StackTrace

            m_SQLStatus = False
        End Try

    End Sub

    '****************************************************************
    ' NullSafeInteger
    '****************************************************************
    Public Shared Function NullSafeInteger(ByVal arg As Object, _
      Optional ByVal returnIfEmpty As Integer = WILDCARD_ID) As Integer

        Dim returnValue As Integer

        If (arg Is DBNull.Value) OrElse (arg Is Nothing) _
                         OrElse (arg Is String.Empty) Then
            returnValue = returnIfEmpty
        Else
            Try
                returnValue = CInt(arg)
            Catch
                returnValue = returnIfEmpty
            End Try
        End If

        Return returnValue

    End Function

    Public Shared Function CreateTable(ByVal obDataView As DataView) As DataTable
        If Nothing Is obDataView Then
            Throw New ArgumentNullException("DataView", "Invalid DataView object specified")
        End If
        Dim obNewDt As DataTable = obDataView.Table.Clone
        Dim idx As Integer = 0
        Dim strColNames(obNewDt.Columns.Count) As String
        For Each col As DataColumn In obNewDt.Columns
            strColNames(System.Math.Min(System.Threading.Interlocked.Increment(idx), idx - 1)) = col.ColumnName
        Next
        Dim viewEnumerator As IEnumerator = obDataView.GetEnumerator
        While viewEnumerator.MoveNext
            Dim drv As DataRowView = CType(viewEnumerator.Current, DataRowView)
            Dim dr As DataRow = obNewDt.NewRow
            Try
                For Each strName As String In strColNames
                    dr(strName) = drv(strName)
                Next
            Catch ex As Exception
                ' Trace.WriteLine(ex.Message)
            End Try
            obNewDt.Rows.Add(dr)
        End While
        Return obNewDt
    End Function
    Public Function SetStandardName() As String
        Dim mydr As New db
        Dim sqlText As String = ""
        Dim returnValue As String

        sqlText = "SELECT Config_Value, Config_PK  FROM  J5C_Config   WHERE     (Config_PK = 411)"

        mydr.SQLText = sqlText
        mydr.GetDR()

        If mydr.dr.HasRows Then

            mydr.dr.Read()

            If mydr.dr("Config_Value") Is DBNull.Value Then
                returnValue = "Standard"
            Else
                returnValue = mydr.dr("Config_Value")
            End If

        Else
            returnValue = "Standard"
        End If

        Return returnValue

    End Function

    Public Function GraphicOrganizerPath() As String

        Dim mydr As New db
        Dim sqlText As String = ""
        Dim returnValue As String

        sqlText = "SELECT Config_Value, Config_PK  FROM  J5C_Config   WHERE     (Config_PK = 412)"

        mydr.SQLText = sqlText
        mydr.GetDR()

        If mydr.dr.HasRows Then
            mydr.dr.Read()

            If mydr.dr("Config_Value") Is DBNull.Value Then
                returnValue = "NotSet"
            Else
                returnValue = mydr.dr("Config_Value")
            End If
        Else
            returnValue = "NotSet"
        End If

        Return returnValue

    End Function
    Public Function ExecuteQueryCV() As String

        Dim sqlConn As SqlConnection
        Dim RV As String = "-99"

        sqlConn = New SqlConnection(System.Configuration.ConfigurationManager.AppSettings("ConnectionString"))
        sqlConn.Open()

        Dim cmd As New SqlCommand(m_SQLText, sqlConn)
        m_SQLErrorMess = "OK"
        m_SQLStatus = True

        Try
            RV = cmd.ExecuteNonQuery()
        Catch ex As Exception
            m_SQLErrorMess = m_SQLText & "<br><br>" & ex.Message & "<br><br>"
            m_SQLErrorMess = m_SQLErrorMess & "Source: " & ex.Source & "<br><br>"
            m_SQLErrorMess = m_SQLErrorMess & "Stack Trace: " & ex.StackTrace & "<br><br>"

            _ExceptionMessage = ex.Message
            _ExceptionSource = ex.Source
            _ExceptionStackTrace = ex.StackTrace

            m_SQLStatus = False
        Finally
            sqlConn.Close()
        End Try

        Return RV

    End Function

End Class
