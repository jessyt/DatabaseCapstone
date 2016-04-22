Imports System.Web
Imports System.Web.Services
Imports System.Web.Services.Protocols
Imports System.Web.Script.Serialization
Imports System.Collections.Generic
Imports System.Data
Imports System.Data.SqlClient
Imports System.Globalization


<System.Web.Script.Services.ScriptService()> _
<WebServiceBinding(ConformsTo:=WsiProfiles.BasicProfile1_1)> _
<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
 Public Class wsAuthenticate
    Inherits System.Web.Services.WebService

    Private datareader As New db,
    sqlErrorFound As Boolean = False,
    sqlErrorRecord As New ServerResponse

    Public Class jsonData
        Public Property Username() As String
            Get
                Return m_Username
            End Get
            Set(ByVal value As String)
                m_Username = value
            End Set
        End Property
        Private m_Username As String

        Public Property Password() As String
            Get
                Return m_Password
            End Get
            Set(ByVal value As String)
                m_Password = value
            End Set
        End Property
        Private m_Password As String

        Public Property LastName() As String
            Get
                Return m_LastName
            End Get
            Set(ByVal value As String)
                m_LastName = value
            End Set
        End Property
        Private m_LastName As String

        Public Property FirstName() As String
            Get
                Return m_FirstName
            End Get
            Set(ByVal value As String)
                m_FirstName = value
            End Set
        End Property
        Private m_FirstName As String

        Public Property GradYear() As Integer
            Get
                Return m_GradYear
            End Get
            Set(ByVal value As Integer)
                m_GradYear = value
            End Set
        End Property
        Private m_GradYear As Integer

    End Class
    <WebMethod(EnableSession:=True)> _
    Public Function AuthenticateUser(ByVal dataList As List(Of jsonData)) As String

        Dim username As String = ""
        Dim password As String = ""
        Dim isAuthenticated As String = "N"
        Dim sqlText As String = ""
        Dim valueArray As New ArrayList
        Dim ResponseText As JavaScriptSerializer = New JavaScriptSerializer

        For Each item As jsonData In dataList
            username = item.Username
            password = item.Password
        Next
        sqlText = "SELECT SM_PK, Role_FK, SM_Email, SM_Password, SM_GradYear" & vbCrLf & _
                    " FROM     Security_Master" & vbCrLf & _
                    " WHERE  (SM_Email = '" & username & "') AND (SM_Password = '" & password & "')"

        datareader.SQLText = sqlText
        datareader.GetDR()

        If datareader.SQLStatus = False Then
            Session("SQLErrorMess") = datareader.SQLErrorMess
            Throw New ApplicationException(Session("SQLErrorMess"))
        End If

        If datareader.dr.HasRows Then
            datareader.dr.Read()

            Dim dbRecord As New AuthenticateInfo
            dbRecord.SM_PK = datareader.dr("SM_PK")
            dbRecord.Role_FK = datareader.dr("Role_FK")
            valueArray.Add(dbRecord)
            dbRecord = Nothing
            Session("SecurityPK") = datareader.dr("SM_PK")
            Session("Role") = datareader.dr("Role_FK")
        Else
            Dim dbRecord As New AuthenticateInfo
            dbRecord.SM_PK = -1
            dbRecord.Role_FK = -1
            valueArray.Add(dbRecord)
            dbRecord = Nothing
            Session.Remove("SecurityPK")
            Session.Remove("Role")
        End If

        Return ResponseText.Serialize(valueArray)
    End Function
    <WebMethod(EnableSession:=True)> _
    Public Function RegisterNewUser(ByVal dataList As List(Of jsonData)) As String

        Dim lastname As String = ""
        Dim firstname As String = ""
        Dim gradyear As Integer = -1
        Dim username As String = ""
        Dim password As String = ""
        Dim sqlText As String = ""
        Dim message As String = ""
        Dim valueArray As New ArrayList
        Dim ResponseText As JavaScriptSerializer = New JavaScriptSerializer

        For Each item As jsonData In dataList
            username = item.Username
            password = item.Password
            lastname = item.LastName
            firstname = item.FirstName
            gradyear = item.GradYear
        Next
        sqlText = "SELECT SM_PK, Role_FK, SM_Email, SM_Password, SM_GradYear" & vbCrLf & _
                    " FROM     Security_Master" & vbCrLf & _
                    " WHERE  (SM_Email = '" & username & "')"

        datareader.SQLText = sqlText
        datareader.GetDR()

        If datareader.SQLStatus = False Then
            Session("SQLErrorMess") = datareader.SQLErrorMess
            Throw New ApplicationException(Session("SQLErrorMess"))
        End If

        If datareader.dr.HasRows Then
            datareader.dr.Read()
            message = "Email already exsists!"
        Else
            sqlText = "INSERT INTO Security_Master" & vbCrLf & _
            " (Role_FK, SM_LastName, SM_FirstName, SM_Email, SM_Password, SM_GradYear)" & vbCrLf & _
            " VALUES (1,'" & lastname & "','" & firstname & "','" & username & "','" & password & "'," & gradyear & ")"

            datareader.SQLText = sqlText
            datareader.ExecuteQuery()

            If datareader.SQLStatus = False Then
                Session("SQLErrorMess") = datareader.SQLErrorMess
                Throw New ApplicationException(Session("SQLErrorMess"))
            End If
            message = "New user created!"

        End If
        Return message
    End Function

    Public Class AuthenticateInfo
        Public SM_PK As Integer
        Public Role_FK As Integer
    End Class
End Class
