

Partial Class Login_Authenticate
    Inherits System.Web.UI.Page


    Protected Sub Page_Load(sender As Object, e As EventArgs) Handles Me.Load
        If Not IsPostBack Then
            Session.Remove("SecurityPK")
            Session.Remove("Role")
        End If
    End Sub
End Class
