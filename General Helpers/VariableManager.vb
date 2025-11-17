Public Class VariableManager
    Private Shared gshr_vm As New VariableManager
    Private g_lstVars As New List(Of VariableBase)

    Public Shared ReadOnly Property GlobalManager As VariableManager
        Get
            Return gshr_vm
        End Get
    End Property

    Public Sub SetLongValue(ByVal varName As String, ByVal value As Long)
        Dim var As VariableBase = GetVariable(varName)

        If var Is Nothing Then
            var = New VariableLong(varName)
            g_lstVars.Add(var)
        Else
            If Not TypeOf var Is VariableLong Then
                Throw New Exception("Variable already exists and is of a different type.")
            End If


        End If

        DirectCast(var, VariableLong).Value = value
    End Sub

    Public Function GetLongValue(ByVal varName As String) As Long
        Dim var As VariableLong = GetVariable(varName)

        If var IsNot Nothing Then
            Return var.Value
        End If
        Return 0
    End Function

    Public Function GetVariable(ByVal varName As String) As VariableBase
        Return g_lstVars.FirstOrDefault(Function(v) v.VarName.Equals(varName, StringComparison.CurrentCultureIgnoreCase))
    End Function

End Class

Public MustInherit Class VariableBase
    Private _varName As String

    Public Sub New(ByVal varName As String)
        _varName = varName
    End Sub

    Public ReadOnly Property VarName As String
        Get
            Return _varName
        End Get
    End Property

End Class

Public MustInherit Class Variable(Of T)
    Inherits VariableBase

    Public Sub New(ByVal varName As String)
        MyBase.New(varName)
    End Sub

    Public Property Value As T
End Class

Public Class VariableLong
    Inherits Variable(Of Long)

    Public Sub New(ByVal varName As String)
        MyBase.New(varName)
    End Sub
End Class